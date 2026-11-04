using System;
using System.Linq;
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.StatusEffects;
using Game.Domain.Combat.Resources;
using Game.Domain.Skills;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void RunSilenceActionIntegritySmokeTests()
        {
            var fixture = new ResonanceOwnershipFixture(2, 10, 2);
            fixture.Player.Debuffs.Add(new SilenceDebuff(3), fixture.Player,
                new CombatContext(fixture.Player, fixture.Enemy, fixture.Runtime.EventLog));
            fixture.Engine.ExecuteTurn(fixture.Runtime, fixture.SkillRequest(true));
            Console.WriteLine("  AUD-08 Silence: blocked={0}, applied={1}, XP={2}, enemyHP={3}, Essence={4}, Resonance={5}",
                CountEvents(fixture.Runtime.EventLog, GameEventType.ActionBlocked), fixture.Skill.Applied,
                fixture.Progress.Experience, fixture.Enemy.Stats.CurrentHealth, fixture.Player.Resource.Current,
                fixture.Resonance.Active.Current);
            Assert(!fixture.Skill.Applied && fixture.Progress.Experience == 0
                && fixture.Enemy.Stats.CurrentHealth == 100 && fixture.Player.Resource.Current == 20
                && fixture.Resonance.Active.Current == 10,
                "AUD-08 a skill reported blocked by Silence must not execute, grant XP, damage or spend skill resources.");

            AssertSilenceBlockingMatrix();
            AssertSilenceNonSkillActions();
            AssertSilenceExpirationWorkflow();
            AssertSilenceLethalBoundaries();
            AssertSilenceEnemyOwnership();
        }

        private static void AssertSilenceBlockingMatrix()
        {
            foreach (int mode in new[] { 0, 1, 2 })
            foreach (string scenario in new[] { "silence", "seal", "both", "validator", "essence", "resonance", "multiple" })
            {
                if (mode == 0 && scenario == "validator") continue;
                var f = new ResonanceOwnershipFixture(2, 10, mode);
                var silence = new SilenceDebuff(2);
                if (scenario != "seal") AddSilenceAuditDebuff(f, f.Player, silence);
                if (scenario == "multiple") AddSilenceAuditDebuff(f, f.Player, new SilenceDebuff(3));
                if (scenario == "seal" || scenario == "both")
                    f.Player.StatusEffects.Apply(new StatusEffectInstance(StatusEffectType.SkillSeal, 2, false));
                var marker = new BlindDebuff(4, 0f);
                AddSilenceAuditDebuff(f, f.Player, marker);
                if (scenario == "essence") f.Player.Resource.TrySpend(20);
                if (scenario == "resonance") f.Resonance.TrySpendActive(10);
                var skill = new SilenceEffectProbe(scenario == "validator" ? "wrong_skill_id" : f.Skill.Id);
                int essence = f.Player.Resource.Current;
                int active = f.Resonance.Active.Current;
                int offset = f.Runtime.EventLog.Events.Count;
                f.Engine.ExecuteTurn(f.Runtime, TurnRequest.SkillAction(f.Player, f.Enemy, skill, f.Progress));
                string expected = scenario == "validator" ? CombatSkillUseBlockReason.SkillProgressIdMismatch.ToString()
                    : scenario == "seal" ? "is sealed" : "is silenced";
                AssertSilenceRejectedEffects(f, f.Player, f.Enemy, skill, f.Progress, offset, essence, active, expected);
                Assert(marker.RemainingTurns == 3 && f.Player.Debuffs.Active.Contains(marker)
                    && (scenario == "seal" || silence.RemainingTurns == 1),
                    "AUD-08 completion ticks legacy durations once without executing the skill's cleanse.");
                Assert(f.Player.StatusEffects.Active.All(s => s.RemainingDuration == 1)
                    && (scenario != "multiple" || f.Player.Debuffs.Active.OfType<SilenceDebuff>().Count() == 2),
                    "AUD-08 multiple blockers retain independent durations with one diagnostic.");
                Assert(f.Runtime.TurnNumber == 2 && !f.Runtime.CombatEnded
                    && f.Runtime.EventLog.Events.Skip(offset).Select(e => e.Type).SequenceEqual(new[]
                        { GameEventType.TurnStarted, GameEventType.ActionBlocked, GameEventType.ResourceRegenerated, GameEventType.TurnEnded }),
                    "AUD-08 living blocked skill completes the canonical turn exactly once.");
            }

            var hooks = new ResonanceOwnershipFixture(2, 10, 2);
            AddSilenceAuditDebuff(hooks, hooks.Player, new SilenceDebuff(2));
            AddSilenceAuditDebuff(hooks, hooks.Player, new ResourceBlockDebuff(2));
            AddSilenceAuditDebuff(hooks, hooks.Player, new PoisonDebuff(2, 3));
            hooks.Player.Resource.TrySpend(5);
            hooks.Player.StatusEffects.Apply(new StatusEffectInstance(StatusEffectType.Burn, 2, false, 4));
            hooks.Engine.ExecuteTurn(hooks.Runtime, hooks.SkillRequest(true));
            Assert(hooks.Player.Stats.CurrentHealth == 93 && hooks.Player.Resource.Current == 15
                && hooks.Player.Flags.ResourceRegenBlocked && hooks.Runtime.TurnNumber == 2
                && hooks.Player.Debuffs.Active.All(d => d.RemainingTurns == 1)
                && hooks.Player.StatusEffects.GetFirst(StatusEffectType.Burn).RemainingDuration == 1,
                "AUD-08 preserves legitimate start damage, resource-block hooks and completion DOT on blocked turns.");
            Assert(CountEvents(hooks.Runtime.EventLog, GameEventType.StatusTick) == 1
                && CountEvents(hooks.Runtime.EventLog, GameEventType.ActionBlocked) == 1
                && CountEvents(hooks.Runtime.EventLog, GameEventType.ResourceRegenerated) == 0,
                "AUD-08 does not discard unrelated hook consequences or invent regeneration events.");
        }

        private static void AssertSilenceRejectedEffects(ResonanceOwnershipFixture f, CombatEntity actor,
            CombatEntity defender, SilenceEffectProbe skill, SkillProgress progress, int offset,
            int essenceBefore, int activeBefore, string reason)
        {
            var events = f.Runtime.EventLog.Events.Skip(offset).ToArray();
            Assert(skill.ApplyCalls == 0 && progress.Experience == 0 && f.CostObserver.Calls == 0,
                "AUD-08 blocked skills cannot invoke Apply, grant XP or run cost passives.");
            Assert(actor.Resource.Current == Math.Min(actor.Resource.Max, essenceBefore + 5)
                && f.Resonance.Active.Current == activeBefore && f.Resonance.Reserve.Current == 50,
                "AUD-08 blocked skill spending is zero; ordinary end-turn Essence regeneration remains distinct.");
            Assert(defender.Stats.CurrentHealth == 100 && defender.Debuffs.Active.Count == 0,
                "AUD-08 blocked skill cannot cause offensive damage or apply a debuff.");
            Assert(!events.Any(e => e.Type == GameEventType.SkillUsed || e.Type == GameEventType.ResourceSpent
                || e.Type == GameEventType.DamageDealt || e.Type == GameEventType.DebuffApplied
                || e.Type == GameEventType.CleanseResolved || e.Type == GameEventType.ResourceNotEnough),
                "AUD-08 rejected skill has no success/effect/cost events or lower-priority cost rejection.");
            Assert(events.Count(e => e.Type == GameEventType.ActionBlocked) == 1
                && events.Single(e => e.Type == GameEventType.ActionBlocked).Message.Contains(reason),
                "AUD-08 selected blocker emits exactly one meaningful diagnostic with deterministic precedence.");
        }

        private static void AssertSilenceNonSkillActions()
        {
            foreach (bool refill in new[] { false, true })
            foreach (int blockers in new[] { 1, 2, 3 })
            {
                var f = new ResonanceOwnershipFixture(2, 10, 2);
                var silence = new SilenceDebuff(2);
                if ((blockers & 1) != 0) AddSilenceAuditDebuff(f, f.Player, silence);
                if ((blockers & 2) != 0)
                    f.Player.StatusEffects.Apply(new StatusEffectInstance(StatusEffectType.SkillSeal, 2, false));
                f.Resonance.TrySpendActive(6);
                if (refill)
                    Assert(new ResonanceRefillAction().Execute(f.Resonance, f.Runtime, 4),
                        "AUD-08 refill remains permitted under Silence and/or SkillSeal.");
                else
                    f.Engine.ExecuteTurn(f.Runtime, TurnRequest.BasicAttack(f.Player, f.Enemy));
                Assert(f.Runtime.TurnNumber == 2 && f.Progress.Experience == 0 && f.CostObserver.Calls == 0
                    && CountEvents(f.Runtime.EventLog, GameEventType.ActionBlocked) == 0,
                    "AUD-08 non-skill actions must not emit misleading Silence/SkillSeal rejection.");
                Assert(f.Enemy.Stats.CurrentHealth == (refill ? 100 : 94)
                    && f.Resonance.Active.Current == (refill ? 8 : 4)
                    && f.Resonance.Reserve.Current == (refill ? 46 : 50),
                    "AUD-08 normal attack/refill effects remain unchanged.");
                Assert(((blockers & 1) == 0 || silence.RemainingTurns == 1)
                    && f.Player.StatusEffects.Active.All(s => s.RemainingDuration == 1),
                    "AUD-08 allowed non-skill turns still decrement blocker durations once.");
            }
        }

        private static void AssertSilenceExpirationWorkflow()
        {
            foreach (int duration in new[] { 1, 2 })
            {
                var f = new ResonanceOwnershipFixture(2, 10, 2);
                var silence = new SilenceDebuff(duration);
                AddSilenceAuditDebuff(f, f.Player, silence);
                var skill = new SilenceEffectProbe(f.Skill.Id);
                for (int turn = 1; turn <= duration; turn++)
                {
                    f.Engine.ExecuteTurn(f.Runtime, TurnRequest.SkillAction(f.Player, f.Enemy, skill, f.Progress));
                    Assert(skill.ApplyCalls == 0 && f.Progress.Experience == 0 && silence.RemainingTurns == duration - turn
                        && f.Runtime.TurnNumber == turn + 1 && CountEvents(f.Runtime.EventLog, GameEventType.ActionBlocked) == turn,
                        "AUD-08 a start-active Silence blocks even its final turn; repeated attempts remain safe.");
                }
                Assert(!f.Player.Debuffs.Active.Contains(silence)
                    && CountEvents(f.Runtime.EventLog, GameEventType.DebuffExpired) == 1,
                    "AUD-08 normal completion expires Silence, without retroactively executing the attempt.");
                f.Engine.ExecuteTurn(f.Runtime, TurnRequest.SkillAction(f.Player, f.Enemy, skill, f.Progress));
                Assert(skill.ApplyCalls == 1 && f.Progress.Experience == 10 && f.CostObserver.Calls == 1
                    && f.Player.Resource.Current == 15 && f.Resonance.Active.Current == 8
                    && f.Enemy.Stats.CurrentHealth < 100 && f.Enemy.Debuffs.Active.Count == 1,
                    "AUD-08 fresh turn after expiration can spend costs, apply damage/debuff and grant XP normally.");
                Assert(CountEvents(f.Runtime.EventLog, GameEventType.SkillUsed) == 1
                    && CountEvents(f.Runtime.EventLog, GameEventType.CleanseResolved) == 1
                    && CountEvents(f.Runtime.EventLog, GameEventType.ActionBlocked) == duration,
                    "AUD-08 positive control proves the probe's effects and cleanse are real, and blockage never leaks across contexts.");
            }
            var expired = new ResonanceOwnershipFixture(2, 10, 2);
            AddSilenceAuditDebuff(expired, expired.Player, new SilenceDebuff(0));
            expired.Engine.ExecuteTurn(expired.Runtime, expired.SkillRequest(true));
            Assert(expired.Skill.Applied && CountEvents(expired.Runtime.EventLog, GameEventType.ActionBlocked) == 0,
                "AUD-08 an already-expired Silence is not an active blocker.");
        }

        private static void AssertSilenceLethalBoundaries()
        {
            foreach (bool legacy in new[] { false, true })
            foreach (bool burn in new[] { false, true })
            {
                var f = new ResonanceOwnershipFixture(2, 10, 2);
                var silence = new SilenceDebuff(2);
                AddSilenceAuditDebuff(f, f.Player, silence);
                f.Player.ReceiveDamage(95);
                if (legacy)
                    AddSilenceAuditDebuff(f, f.Player, burn ? (IDebuff)new BurnDebuff(3, 7, 0.5f) : new PoisonDebuff(3, 7));
                else
                    f.Player.StatusEffects.Apply(new StatusEffectInstance(
                        burn ? StatusEffectType.Burn : StatusEffectType.Poison, 3, false, 7));
                f.Engine.ExecuteTurn(f.Runtime, f.SkillRequest(true));
                Assert(!f.Player.IsAlive && f.Runtime.CombatEnded && f.Runtime.TurnNumber == 1
                    && silence.RemainingTurns == (legacy ? 2 : 1),
                    "AUD-08 terminal start/end damage preserves canonical lifecycle and suppresses turn advancement.");
                Assert(!f.Skill.Applied && f.Progress.Experience == 0 && f.CostObserver.Calls == 0
                    && f.Resonance.Active.Current == 10 && f.Player.Resource.Current == 20 && f.Enemy.Stats.CurrentHealth == 100,
                    "AUD-08 neither lethal boundary permits blocked skill effects, XP or costs.");
                Assert(CountEvents(f.Runtime.EventLog, GameEventType.CombatEnded) == 1
                    && CountEvents(f.Runtime.EventLog, GameEventType.TurnEnded) == 0
                    && CountEvents(f.Runtime.EventLog, GameEventType.ActionBlocked) == (legacy ? 0 : 1)
                    && f.Runtime.EventLog.Events.Last().Type == GameEventType.CombatEnded,
                    "AUD-08 start death precedes blocking diagnostics; end death follows the single rejection, without normal completion.");
            }
        }

        private static void AssertSilenceEnemyOwnership()
        {
            foreach (int cost in new[] { 0, 2 })
            {
                var f = new ResonanceOwnershipFixture(cost, 10, 2);
                var silence = new SilenceDebuff(1);
                AddSilenceAuditDebuff(f, f.Enemy, silence);
                var skill = new SilenceEffectProbe(f.Skill.Id);
                int offset = f.Runtime.EventLog.Events.Count;
                f.Engine.ExecuteTurn(f.Runtime, TurnRequest.SkillAction(f.Enemy, f.Player, skill, f.Progress));
                AssertSilenceRejectedEffects(f, f.Enemy, f.Player, skill, f.Progress, offset, 20, 10, "is silenced");
                f.Engine.ExecuteTurn(f.Runtime, TurnRequest.SkillAction(f.Enemy, f.Player, skill, f.Progress));
                Assert(f.Resonance.Active.Current == 10 && f.Resonance.Reserve.Current == 50
                    && skill.ApplyCalls == (cost == 0 ? 1 : 0) && f.Progress.Experience == (cost == 0 ? 10 : 0)
                    && f.Enemy.Resource.Current == (cost == 0 ? 15 : 20),
                    "AUD-08 enemy skill after expiration still obeys AUD-05 player-only Resonance ownership.");
                Assert(cost == 0 ? f.Player.Stats.CurrentHealth < 100
                    : f.Runtime.EventLog.Events.Last(e => e.Type == GameEventType.ActionBlocked).Message.Contains(
                        CombatSkillUseBlockReason.UnsupportedResonanceOwnership.ToString()),
                    "AUD-08 zero-Resonance enemy skill resumes; positive ownership violation remains blocked.");
            }
        }

        private static void AddSilenceAuditDebuff(ResonanceOwnershipFixture f, CombatEntity actor, IDebuff debuff)
        {
            actor.Debuffs.Add(debuff, actor, new CombatContext(actor,
                actor == f.Player ? f.Enemy : f.Player, f.Runtime.EventLog));
        }

        // Test-only executable probe verifies skipped effects as well as a successful post-expiry control.
        private sealed class SilenceEffectProbe : ICombatSkill
        {
            public string Id { get; private set; }
            public string Name { get { return "AUD-08 effect probe"; } }
            public int ApplyCalls { get; private set; }
            public SilenceEffectProbe(string id) { Id = id; }
            public void Apply(CombatContext context, SkillProgress progress)
            {
                ApplyCalls++;
                context.EventLog.Add(GameEventType.SkillUsed, "AUD-08 effect probe executed.");
                context.FlatDamageBonus += 10;
                context.RequestDebuff(new PoisonDebuff(3, 3), 1f);
                context.RequestCleanse(CleanseType.Partial);
            }
        }
    }
}
