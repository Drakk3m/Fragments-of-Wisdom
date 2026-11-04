using System;
using System.Linq;
using System.Text.Json;
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.Passives;
using Game.Domain.Combat.Resolvers;
using Game.Domain.Combat.Resources;
using Game.Domain.Combat.StatusEffects;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void RunTerminalRefillBoundarySmokeTests()
        {
            var probe = new TerminalRefillFixture();
            probe.F.Runtime.EndCombat();
            string before = probe.Capture();
            bool result = new ResonanceRefillAction().Execute(probe.F.Resonance, probe.F.Runtime, 4);
            Console.WriteLine("  AUD-07 current-main probe: accepted={0}, active={1}, reserve={2}, turn={3}, unchanged={4}",
                result, probe.F.Resonance.Active.Current, probe.F.Resonance.Reserve.Current,
                probe.F.Runtime.TurnNumber, probe.Capture() == before);
            Assert(!result && probe.Capture() == before,
                "AUD-07 AUD-06 already rejects explicit ended combat without mutation.");

            var cases = new (string Name, Action<TerminalRefillFixture> Prepare, bool PlayerAlive, bool EnemyAlive, bool Ended)[]
            {
                ("ended with living actors", t => t.F.Runtime.EndCombat(), true, true, true),
                ("ended with defeated player", t => { t.F.Player.ReceiveDamage(100); t.F.Runtime.EndCombat(); }, false, true, true),
                ("ended with defeated enemy", t => { t.F.Enemy.ReceiveDamage(100); t.F.Runtime.EndCombat(); }, true, false, true),
                ("defeated player without ended flag", t => t.F.Player.ReceiveDamage(100), false, true, false),
                ("defeated enemy without ended flag", t => t.F.Enemy.ReceiveDamage(100), true, false, false),
                ("normal attack defeat", t =>
                {
                    t.F.Enemy.ReceiveDamage(99);
                    t.Engine.ExecuteTurn(t.F.Runtime, TurnRequest.BasicAttack(t.F.Player, t.F.Enemy));
                }, true, false, true),
                ("skill defeat", t =>
                {
                    t.F.Enemy.ReceiveDamage(99);
                    t.Engine.ExecuteTurn(t.F.Runtime, t.F.SkillRequest(true));
                }, true, false, true),
                ("Poison boundary defeat", t => EndTerminalRefillWithStatus(t, false, false), false, true, true),
                ("Burn boundary defeat", t => EndTerminalRefillWithStatus(t, true, false), false, true, true),
                ("legacy Poison start defeat", t => EndTerminalRefillWithStatus(t, false, true), false, true, true),
                ("legacy Burn start defeat", t => EndTerminalRefillWithStatus(t, true, true), false, true, true)
            };
            foreach (var scenario in cases)
            {
                var fixture = new TerminalRefillFixture();
                scenario.Prepare(fixture);
                Assert(fixture.F.Player.IsAlive == scenario.PlayerAlive && fixture.F.Enemy.IsAlive == scenario.EnemyAlive
                    && fixture.F.Runtime.CombatEnded == scenario.Ended,
                    "AUD-07 public workflow reaches expected terminal state: " + scenario.Name);
                // A terminal call must not even reset stale turn flags.
                fixture.F.Player.Flags.ResourceRegenBlocked = true;
                foreach (int amount in new[] { 4, 0, -1, int.MinValue, int.MaxValue })
                for (int attempt = 0; attempt < 2; attempt++)
                {
                    string captured = fixture.Capture();
                    Assert(!new ResonanceRefillAction().Execute(fixture.F.Resonance, fixture.F.Runtime, amount),
                        "AUD-07 terminal rejection for " + scenario.Name + ", amount=" + amount + ", retry=" + attempt);
                    Assert(fixture.Capture() == captured,
                        "AUD-07 terminal refill preserves pools, actors, statuses, debuffs, clock, events, XP, passives and hooks: " + scenario.Name);
                }
            }
            AssertTerminalRefillNullPrecedence();
            AssertTerminalRefillActiveControl();
        }

        private static void EndTerminalRefillWithStatus(TerminalRefillFixture fixture, bool burn, bool legacy)
        {
            fixture.F.Player.ReceiveDamage(95);
            if (legacy)
                fixture.F.Player.Debuffs.Add(burn ? (IDebuff)new BurnDebuff(3, 7, 0.5f) : new PoisonDebuff(3, 7),
                    fixture.F.Player, new CombatContext(fixture.F.Player, fixture.F.Enemy, fixture.F.Runtime.EventLog));
            else
                fixture.F.Player.StatusEffects.Apply(new StatusEffectInstance(
                    burn ? StatusEffectType.Burn : StatusEffectType.Poison, 3, false, 7));
            fixture.Engine.ExecuteTurn(fixture.F.Runtime, TurnRequest.BasicAttack(fixture.F.Player, fixture.F.Enemy));
        }

        private static void AssertTerminalRefillNullPrecedence()
        {
            foreach (bool terminal in new[] { false, true })
            foreach (int mode in new[] { 0, 1, 2 })
            {
                var fixture = new TerminalRefillFixture();
                if (terminal) fixture.F.Runtime.EndCombat();
                string before = fixture.Capture();
                bool correctException = false;
                try
                {
                    new ResonanceRefillAction().Execute(mode == 1 ? fixture.F.Resonance : null,
                        mode == 0 ? fixture.F.Runtime : null, int.MinValue);
                }
                catch (ArgumentNullException ex)
                {
                    correctException = ex.ParamName == (mode == 1 ? "runtime" : "resonanceProfile");
                }
                Assert(correctException, "AUD-07 null arguments retain precedence, including both-null and terminal contexts.");
                Assert(fixture.Capture() == before, "AUD-07 argument exceptions precede any observable mutation.");
            }
        }

        private static void AssertTerminalRefillActiveControl()
        {
            var fixture = new TerminalRefillFixture();
            int eventsBefore = fixture.F.Runtime.EventLog.Events.Count;
            Assert(new ResonanceRefillAction().Execute(fixture.F.Resonance, fixture.F.Runtime, 4)
                && fixture.F.Resonance.Active.Current == 8 && fixture.F.Resonance.Reserve.Current == 46
                && fixture.F.Runtime.TurnNumber == 2 && !fixture.F.Runtime.CombatEnded,
                "AUD-07 positive control: living active combat still accepts a valid refill.");
            Assert(fixture.PlayerHooks.StartCalls == 1 && fixture.PlayerHooks.BeforeCalls == 1 && fixture.PlayerHooks.TickCalls == 1
                && fixture.EnemyHooks.StartCalls == 0 && fixture.EnemyHooks.BeforeCalls == 0 && fixture.EnemyHooks.TickCalls == 0
                && fixture.F.Player.Resource.Current == 15 && fixture.F.Enemy.Resource.Current == 17
                && fixture.F.Progress.Experience == 0 && fixture.Passives.CostCalls == 0,
                "AUD-07 positive control proves hook observers detect the real lifecycle without skill side effects.");
            Assert(fixture.F.Runtime.EventLog.Events.Skip(eventsBefore).Select(e => e.Type).SequenceEqual(new[]
                { GameEventType.TurnStarted, GameEventType.ResourceSpent, GameEventType.ResourceRegenerated, GameEventType.TurnEnded }),
                "AUD-07 active refill retains canonical event order; terminal rejection adds no events instead.");
        }

        private sealed class TerminalRefillFixture
        {
            public readonly ResonanceOwnershipFixture F = BuildRefillFixture();
            public readonly TerminalRefillHookObserver PlayerHooks = new TerminalRefillHookObserver();
            public readonly TerminalRefillHookObserver EnemyHooks = new TerminalRefillHookObserver();
            public readonly TerminalRefillPassiveObserver Passives = new TerminalRefillPassiveObserver();
            public readonly TurnBasedCombatEngine Engine;

            public TerminalRefillFixture()
            {
                Engine = new TurnBasedCombatEngine(new FixedRandomSource(0.0, 0.99),
                    new CombatPassivePipeline(new ICombatPassiveEffect[] { Passives }), F.Gateway);
                Engine.StartCombat(F.Runtime);
                F.Enemy.Resource.TrySpend(3);
                foreach (var actor in new[] { F.Player, F.Enemy })
                {
                    actor.StatusEffects.Apply(new StatusEffectInstance(StatusEffectType.RegenBlock, 4, false));
                    actor.Debuffs.Add(actor == F.Player ? PlayerHooks : EnemyHooks, actor,
                        new CombatContext(actor, actor == F.Player ? F.Enemy : F.Player, F.Runtime.EventLog));
                }
            }

            public string Capture()
            {
                // Materialize values before the call, not live references to mutable domain objects.
                return JsonSerializer.Serialize(new
                {
                    Active = new { F.Resonance.Active.Current, F.Resonance.Active.Max },
                    Reserve = new { F.Resonance.Reserve.Current, F.Resonance.Reserve.Max },
                    Actors = new[] { F.Player, F.Enemy }.Select(a => new
                    {
                        a.Id, a.Stats.CurrentHealth, a.Stats.MaxHealth,
                        Essence = new { a.Resource.Current, a.Resource.Max },
                        a.Flags.ResourceRegenBlocked, a.Flags.FirstDebuffAttemptThisCombat,
                        Statuses = a.StatusEffects.Active.Select(s => new { s.Type, s.RemainingDuration, s.Potency, s.AllowsStacking }).ToArray(),
                        Debuffs = a.Debuffs.Active.Select(d => new { d.Name, d.Type, d.RemainingTurns }).ToArray()
                    }).ToArray(),
                    F.Runtime.TurnNumber, F.Runtime.CombatEnded, F.Progress.Experience, F.Progress.Level, F.Skill.Applied,
                    Hooks = new[] { PlayerHooks, EnemyHooks }.Select(h => new { h.StartCalls, h.BeforeCalls, h.TickCalls }).ToArray(),
                    Passives.StartCalls, Passives.CostCalls, Passives.DebuffCalls,
                    Events = F.Runtime.EventLog.Events.Select(e => new { e.Type, e.Message }).ToArray()
                });
            }
        }

        private sealed class TerminalRefillHookObserver : IDebuff
        {
            public string Name { get { return "AUD-07 lifecycle observer"; } }
            public DebuffType Type { get { return DebuffType.ResourceControl; } }
            public int RemainingTurns { get; private set; } = 4;
            public bool IsExpired { get { return RemainingTurns <= 0; } }
            public int StartCalls { get; private set; }
            public int BeforeCalls { get; private set; }
            public int TickCalls { get; private set; }
            public void OnTurnStart(CombatEntity target, CombatContext context) { StartCalls++; }
            public void OnBeforeAction(CombatContext context) { BeforeCalls++; }
            public void Tick() { TickCalls++; RemainingTurns--; }
        }

        private sealed class TerminalRefillPassiveObserver : ICombatPassiveEffect
        {
            public int StartCalls { get; private set; }
            public int CostCalls { get; private set; }
            public int DebuffCalls { get; private set; }
            public void OnCombatStart(CombatContext context) { StartCalls++; }
            public void OnBeforeSkillCostCalculated(CombatContext context) { CostCalls++; }
            public void OnBeforeDebuffApplied(DebuffApplicationContext context) { DebuffCalls++; }
        }
    }
}
