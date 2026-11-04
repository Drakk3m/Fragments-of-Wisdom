using System;
using System.Linq;
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.Resources;
using Game.Domain.Combat.StatusEffects;
using Game.Domain.Skills;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void RunRefillTurnLifecycleSmokeTests()
        {
            var fixture = new ResonanceOwnershipFixture(2, 10, 2);
            fixture.Resonance.TrySpendActive(6);
            var poison = new StatusEffectInstance(StatusEffectType.Poison, 3, false, 7);
            fixture.Player.StatusEffects.Apply(poison);
            bool applied = new ResonanceRefillAction().Execute(fixture.Resonance, fixture.Runtime, 4);
            Console.WriteLine("  AUD-06 refill: applied={0}, HP={1}, Poison duration={2}, turn={3}, active={4}, reserve={5}",
                applied, fixture.Player.Stats.CurrentHealth, poison.RemainingDuration, fixture.Runtime.TurnNumber,
                fixture.Resonance.Active.Current, fixture.Resonance.Reserve.Current);
            Assert(applied && fixture.Resonance.Active.Current == 8 && fixture.Resonance.Reserve.Current == 46
                && fixture.Runtime.TurnNumber == 2, "AUD-06 refill transfers resource and consumes exactly one turn.");
            Assert(fixture.Player.Stats.CurrentHealth == 93 && poison.RemainingDuration == 2,
                "AUD-06 refill must apply Poison damage and decrement duration exactly once.");

            AssertRefillStatusMatrix();
            AssertRefillExpiration();
            AssertRefillLethalBoundaries();
            AssertRefillLegacyHooks();
            AssertRefillTransferCompatibility();
            AssertRefillCombatWorkflow();
            AssertRefillHookTransferFailure();
        }

        private static void AssertRefillStatusMatrix()
        {
            var cases = new[]
            {
                new StatusEffectType[0],
                new[] { StatusEffectType.Poison },
                new[] { StatusEffectType.Burn },
                new[] { StatusEffectType.Regen },
                new[] { StatusEffectType.Regen, StatusEffectType.RegenBlock },
                new[] { StatusEffectType.SkillSeal },
                new[] { StatusEffectType.Poison, StatusEffectType.Burn, StatusEffectType.Regen },
                new[] { StatusEffectType.Poison, StatusEffectType.Burn, StatusEffectType.Regen, StatusEffectType.RegenBlock }
            };
            foreach (var types in cases)
            {
                var refill = BuildRefillFixture();
                var attack = BuildRefillFixture();
                foreach (var fixture in new[] { refill, attack })
                {
                    fixture.Player.ReceiveDamage(20);
                    foreach (var type in types)
                        fixture.Player.StatusEffects.Apply(new StatusEffectInstance(type, 3, false,
                            type == StatusEffectType.Regen ? 9 : 7));
                    fixture.Enemy.StatusEffects.Apply(new StatusEffectInstance(StatusEffectType.Poison, 3, false, 5));
                }
                int expectedHealth = 80 - types.Count(t => t == StatusEffectType.Poison || t == StatusEffectType.Burn) * 7
                    + (types.Contains(StatusEffectType.Regen) && !types.Contains(StatusEffectType.RegenBlock) ? 9 : 0);
                Assert(new ResonanceRefillAction().Execute(refill.Resonance, refill.Runtime, 4),
                    "AUD-06 valid refill is not a skill and is not blocked by SkillSeal.");
                attack.Engine.ExecuteTurn(attack.Runtime, TurnRequest.BasicAttack(attack.Player, attack.Enemy));
                Assert(refill.Player.Stats.CurrentHealth == expectedHealth
                    && refill.Player.Stats.CurrentHealth == attack.Player.Stats.CurrentHealth,
                    "AUD-06 refill uses the same DOT/Regen boundary as canonical attacks.");
                Assert(refill.Player.StatusEffects.Active.All(s => s.RemainingDuration == 2)
                    && refill.Player.StatusEffects.Active.Count == types.Length,
                    "AUD-06 all active status durations tick once, without skips or duplicates.");
                Assert(refill.Player.Resource.Current == 15 && refill.Player.Resource.Current == attack.Player.Resource.Current
                    && refill.Runtime.TurnNumber == 2 && !refill.Runtime.CombatEnded,
                    "AUD-06 refill shares normal Essence regeneration and advances exactly once.");
                Assert(refill.Enemy.Stats.CurrentHealth == 100 && refill.Enemy.Resource.Current == 20
                    && refill.Enemy.StatusEffects.GetFirst(StatusEffectType.Poison).RemainingDuration == 3,
                    "AUD-06 refill does not attack, regenerate or tick the opponent.");
                Assert(refill.Progress.Experience == 0 && !refill.Skill.Applied && refill.CostObserver.Calls == 0,
                    "AUD-06 refill neither executes a skill nor invokes cost passives or grants skill XP.");
                AssertRefillEventOrder(refill, GameEventType.TurnStarted, GameEventType.ResourceSpent,
                    GameEventType.ResourceRegenerated, GameEventType.TurnEnded);
            }

            var ordered = BuildRefillFixture();
            ordered.Player.StatusEffects.Apply(new StatusEffectInstance(StatusEffectType.Regen, 2, false, 7));
            ordered.Player.StatusEffects.Apply(new StatusEffectInstance(StatusEffectType.Poison, 2, false, 7));
            new ResonanceRefillAction().Execute(ordered.Resonance, ordered.Runtime, 4);
            Assert(ordered.Player.Stats.CurrentHealth == 100,
                "AUD-06 DOT precedes capped healing even when Regen was registered first.");
        }

        private static void AssertRefillExpiration()
        {
            foreach (var type in new[] { StatusEffectType.Poison, StatusEffectType.Burn })
            {
                var fixture = BuildRefillFixture();
                var status = new StatusEffectInstance(type, 1, false, 7);
                fixture.Player.StatusEffects.Apply(status);
                Assert(new ResonanceRefillAction().Execute(fixture.Resonance, fixture.Runtime, 4)
                    && fixture.Player.Stats.CurrentHealth == 93 && status.RemainingDuration == 0
                    && !fixture.Player.StatusEffects.HasActive(type),
                    "AUD-06 final DOT tick occurs before status expiration.");
                Assert(new ResonanceRefillAction().Execute(fixture.Resonance, fixture.Runtime, 0)
                    && fixture.Player.Stats.CurrentHealth == 93 && fixture.Runtime.TurnNumber == 3,
                    "AUD-06 expired DOT cannot tick on a subsequent successful no-op refill.");
            }
        }

        private static void AssertRefillLethalBoundaries()
        {
            foreach (bool legacy in new[] { false, true })
            foreach (bool burn in new[] { false, true })
            {
                var fixture = BuildRefillFixture();
                fixture.Player.ReceiveDamage(95);
                var status = new StatusEffectInstance(burn ? StatusEffectType.Burn : StatusEffectType.Poison, 3, false, 7);
                IDebuff debuff = burn ? (IDebuff)new BurnDebuff(3, 7, 0.5f) : new PoisonDebuff(3, 7);
                if (legacy)
                    fixture.Player.Debuffs.Add(debuff, fixture.Player,
                        new CombatContext(fixture.Player, fixture.Enemy, fixture.Runtime.EventLog));
                else
                    fixture.Player.StatusEffects.Apply(status);
                bool applied = new ResonanceRefillAction().Execute(fixture.Resonance, fixture.Runtime, 4);
                Assert(applied == !legacy && !fixture.Player.IsAlive && fixture.Runtime.CombatEnded
                    && fixture.Runtime.TurnNumber == 1,
                    "AUD-06 lethal boundary ends combat without advancing; only a completed transfer returns true.");
                Assert(fixture.Resonance.Active.Current == (legacy ? 4 : 8)
                    && fixture.Resonance.Reserve.Current == (legacy ? 50 : 46),
                    "AUD-06 legacy start death precedes transfer; end-boundary death follows transfer.");
                Assert(legacy ? debuff.RemainingTurns == 3 : status.RemainingDuration == 2,
                    "AUD-06 lethal start skips completion; lethal end performs exactly one canonical boundary.");
                Assert(fixture.Player.Resource.Current == (legacy ? 10 : 15)
                    && fixture.Progress.Experience == 0 && fixture.Enemy.Stats.CurrentHealth == 100,
                    "AUD-06 lethal refill preserves canonical regeneration order without XP/offense.");
                if (legacy)
                    AssertRefillEventOrder(fixture, GameEventType.DebuffApplied, GameEventType.TurnStarted,
                        GameEventType.StatusTick, GameEventType.CombatEnded);
                else
                    AssertRefillEventOrder(fixture, GameEventType.TurnStarted, GameEventType.ResourceSpent,
                        GameEventType.ResourceRegenerated, GameEventType.CombatEnded);
                int events = fixture.Runtime.EventLog.Events.Count;
                Assert(!new ResonanceRefillAction().Execute(fixture.Resonance, fixture.Runtime, 1)
                    && fixture.Runtime.EventLog.Events.Count == events && fixture.Runtime.TurnNumber == 1
                    && (legacy ? debuff.RemainingTurns == 3 : status.RemainingDuration == 2),
                    "AUD-06 terminal defeat cannot cause another lifecycle tick or completion event.");
            }
        }

        private static void AssertRefillLegacyHooks()
        {
            var fixture = BuildRefillFixture();
            var context = new CombatContext(fixture.Player, fixture.Enemy, fixture.Runtime.EventLog);
            var poison = new PoisonDebuff(3, 6);
            var burn = new BurnDebuff(3, 4, 0.5f);
            foreach (var debuff in new IDebuff[] { poison, burn, new ResourceBlockDebuff(1),
                new BlindDebuff(1, 1f), new SilenceDebuff(1) })
                fixture.Player.Debuffs.Add(debuff, fixture.Player, context);
            Assert(new ResonanceRefillAction().Execute(fixture.Resonance, fixture.Runtime, 4)
                && fixture.Player.Stats.CurrentHealth == 90 && poison.RemainingTurns == 2 && burn.RemainingTurns == 2,
                "AUD-06 legacy Poison/Burn start hooks and after-action duration handling execute once.");
            Assert(fixture.Player.Resource.Current == 10 && fixture.Player.Flags.ResourceRegenBlocked
                && fixture.Player.Debuffs.Active.Count == 2,
                "AUD-06 ResourceBlock suppresses Essence regeneration and expiring debuffs are removed.");
            Assert(fixture.Resonance.Active.Current == 8 && fixture.Enemy.Stats.CurrentHealth == 100
                && fixture.Progress.Experience == 0 && ContainsEvent(fixture.Runtime.EventLog, GameEventType.ActionBlocked),
                "AUD-06 Blind has no offensive work to modify; legacy Silence remains diagnostic only, not a new refill blocker.");
            fixture.Engine.ExecuteTurn(fixture.Runtime, TurnRequest.BasicAttack(fixture.Player, fixture.Enemy));
            Assert(!fixture.Player.Flags.ResourceRegenBlocked && fixture.Player.Resource.Current == 15
                && fixture.Player.Stats.CurrentHealth == 80 && poison.RemainingTurns == 1
                && burn.RemainingTurns == 1 && fixture.Runtime.TurnNumber == 3,
                "AUD-06 next canonical action resets stale flags and runs each remaining hook once.");

            var stale = BuildRefillFixture();
            stale.Player.Flags.ResourceRegenBlocked = true;
            new ResonanceRefillAction().Execute(stale.Resonance, stale.Runtime, 4);
            Assert(!stale.Player.Flags.ResourceRegenBlocked && stale.Player.Resource.Current == 15,
                "AUD-06 refill itself resets stale turn flags before evaluating live debuffs.");
        }

        private static void AssertRefillTransferCompatibility()
        {
            foreach (int reserve in new[] { 0, 2 })
            {
                var fixture = BuildRefillFixture();
                fixture.Resonance.Reserve.TryConsume(50 - reserve);
                var poison = new StatusEffectInstance(StatusEffectType.Poison, 3, false, 7);
                fixture.Player.StatusEffects.Apply(poison);
                fixture.Player.Flags.ResourceRegenBlocked = true;
                Assert(!new ResonanceRefillAction().Execute(fixture.Resonance, fixture.Runtime, 4)
                    && fixture.Resonance.Active.Current == 4 && fixture.Resonance.Reserve.Current == reserve,
                    "AUD-06 empty/insufficient reserve rejects without partial transfer.");
                Assert(fixture.Runtime.TurnNumber == 1 && fixture.Player.Stats.CurrentHealth == 100
                    && poison.RemainingDuration == 3 && fixture.Player.Resource.Current == 10
                    && fixture.Player.Flags.ResourceRegenBlocked,
                    "AUD-06 failed admission remains free: no hooks, flags, regeneration, status tick or turn consumed.");
                AssertRefillEventOrder(fixture, GameEventType.ActionBlocked);
                Assert(fixture.Runtime.EventLog.Events[0].Message == ResonanceRefillAction.EmptyReserveBlockReason,
                    "AUD-06 transfer rejection retains the existing stable reason.");
            }
            foreach (int amount in new[] { 0, -1, int.MinValue })
            {
                var fixture = BuildRefillFixture();
                fixture.Resonance.Reserve.TryConsume(50);
                fixture.Player.StatusEffects.Apply(new StatusEffectInstance(StatusEffectType.Poison, 3, false, 7));
                Assert(new ResonanceRefillAction().Execute(fixture.Resonance, fixture.Runtime, amount)
                    && fixture.Resonance.Active.Current == 4 && fixture.Resonance.Reserve.Current == 0,
                    "AUD-06 nonpositive refill remains a successful resource no-op even with empty reserve.");
                Assert(fixture.Player.Stats.CurrentHealth == 93 && fixture.Runtime.TurnNumber == 2,
                    "AUD-06 successful no-op refills retain turn cost and now obey status consequences.");
            }
            foreach (int amount in new[] { 6, 7, int.MaxValue })
            {
                var fixture = BuildRefillFixture();
                fixture.Resonance.Reserve.TryConsume(44);
                Assert(new ResonanceRefillAction().Execute(fixture.Resonance, fixture.Runtime, amount)
                    && fixture.Resonance.Active.Current == 10 && fixture.Resonance.Reserve.Current == 0,
                    "AUD-06 only missing capacity is consumed, including oversized requests; total Resonance is conserved.");
            }
            var full = BuildRefillFixture();
            full.Resonance.Active.Refill(6);
            full.Resonance.Reserve.TryConsume(50);
            Assert(new ResonanceRefillAction().Execute(full.Resonance, full.Runtime, int.MaxValue)
                && full.Resonance.Active.Current == 10 && full.Resonance.Reserve.Current == 0 && full.Runtime.TurnNumber == 2,
                "AUD-06 full active pool preserves successful no-op/one-turn semantics with empty reserve.");
            foreach (bool nullProfile in new[] { false, true })
            {
                var fixture = BuildRefillFixture();
                bool threw = false;
                try { new ResonanceRefillAction().Execute(nullProfile ? null : fixture.Resonance,
                    nullProfile ? fixture.Runtime : null, 4); }
                catch (ArgumentNullException ex) { threw = ex.ParamName == (nullProfile ? "resonanceProfile" : "runtime"); }
                Assert(threw && fixture.Runtime.EventLog.Events.Count == 0 && fixture.Resonance.Active.Current == 4,
                    "AUD-06 null argument exceptions remain explicit and precede mutation.");
            }
            var separate = BuildRefillFixture();
            var runtimeWithoutProfile = BuildCombatRuntime(separate.Player, separate.Enemy);
            Assert(new ResonanceRefillAction().Execute(separate.Resonance, runtimeWithoutProfile, 4)
                && runtimeWithoutProfile.ResonanceProfile == null && runtimeWithoutProfile.TurnNumber == 2,
                "AUD-06 separately supplied player profile remains compatible with the original runtime constructor.");
        }

        private static void AssertRefillCombatWorkflow()
        {
            var fixture = BuildRefillFixture();
            var poison = new StatusEffectInstance(StatusEffectType.Poison, 3, false, 7);
            fixture.Player.StatusEffects.Apply(poison);
            new ResonanceRefillAction().Execute(fixture.Resonance, fixture.Runtime, 4);
            fixture.Engine.ExecuteTurn(fixture.Runtime, fixture.SkillRequest(true));
            Assert(fixture.Skill.Applied && fixture.Progress.Experience == 10 && fixture.CostObserver.Calls == 1
                && fixture.Resonance.Active.Current == 6 && fixture.Resonance.Reserve.Current == 46,
                "AUD-06 refill then player skill uses only canonical skill cost/effect/XP handling.");
            Assert(fixture.Player.Stats.CurrentHealth == 86 && poison.RemainingDuration == 1
                && fixture.Player.Resource.Current == 10 && fixture.Enemy.Stats.CurrentHealth < 100
                && fixture.Runtime.TurnNumber == 3,
                "AUD-06 refill and skill consume distinct turns with one status boundary per action.");
            var enemySkill = new TrackingCombatSkill(fixture.Skill.Id);
            var enemyProgress = new SkillProgress(fixture.Progress.Definition);
            fixture.Engine.ExecuteTurn(fixture.Runtime,
                TurnRequest.SkillAction(fixture.Enemy, fixture.Player, enemySkill, enemyProgress));
            Assert(!enemySkill.Applied && enemyProgress.Experience == 0 && fixture.Resonance.Active.Current == 6
                && fixture.Resonance.Reserve.Current == 46 && fixture.Player.Stats.CurrentHealth == 86
                && poison.RemainingDuration == 1 && fixture.Runtime.TurnNumber == 4,
                "AUD-06 does not grant enemies player Resonance ownership or tick the non-acting player.");
        }

        private static void AssertRefillHookTransferFailure()
        {
            var fixture = BuildRefillFixture();
            fixture.Player.Debuffs.Add(new RefillReserveDrainDebuff(fixture.Resonance), fixture.Player,
                new CombatContext(fixture.Player, fixture.Enemy, fixture.Runtime.EventLog));
            fixture.Player.StatusEffects.Apply(new StatusEffectInstance(StatusEffectType.Poison, 3, false, 7));
            Assert(!new ResonanceRefillAction().Execute(fixture.Resonance, fixture.Runtime, 4)
                && fixture.Resonance.Active.Current == 4 && fixture.Resonance.Reserve.Current == 0,
                "AUD-06 honors actual transfer failure if a custom hook changes resource after admission.");
            Assert(fixture.Runtime.TurnNumber == 2 && fixture.Player.Stats.CurrentHealth == 93
                && fixture.Player.Debuffs.Active.Count == 0,
                "AUD-06 an already-started turn completes once on a hook-induced transfer failure.");
            AssertRefillEventOrder(fixture, GameEventType.DebuffApplied, GameEventType.TurnStarted,
                GameEventType.ActionBlocked, GameEventType.ResourceRegenerated, GameEventType.DebuffExpired, GameEventType.TurnEnded);
        }

        private static ResonanceOwnershipFixture BuildRefillFixture()
        {
            var fixture = new ResonanceOwnershipFixture(2, 10, 2);
            fixture.Resonance.TrySpendActive(6);
            fixture.Player.Resource.TrySpend(10);
            return fixture;
        }

        private static void AssertRefillEventOrder(ResonanceOwnershipFixture fixture, params GameEventType[] expected)
        {
            Assert(fixture.Runtime.EventLog.Events.Select(e => e.Type).SequenceEqual(expected),
                "AUD-06 events preserve canonical order with exactly one start and at most one end, and no offensive events.");
        }

        // Exercises the real extensible hook boundary without adding any production debuff type.
        private sealed class RefillReserveDrainDebuff : BaseDebuff
        {
            private readonly ResonanceProfile resonance;
            public RefillReserveDrainDebuff(ResonanceProfile resonance) : base(1) { this.resonance = resonance; }
            public override string Name { get { return "Audit reserve drain"; } }
            public override DebuffType Type { get { return DebuffType.ResourceControl; } }
            public override void OnBeforeAction(CombatContext context)
            {
                resonance.Reserve.TryConsume(resonance.Reserve.Current);
            }
        }
    }
}
