using System;
using System.Collections.Generic;
using Game.Application.Integration;
using Game.Application.Runtime;
using Game.Domain.Classes;
using Game.Domain.Common;
using Game.Domain.Codex;
using Game.Domain.Combat;
using Game.Domain.Combat.Passives;
using Game.Domain.Combat.Resources;
using Game.Domain.Skills;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void RunCanonicalSkillDefinitionSmokeTests()
        {
            AssertCanonicalSkillDefinitionCannotBeBypassed();
            AssertCanonicalMetadataConflictsAreBlocked();
            AssertCanonicalSkillExecutionAndCopies();
            AssertCanonicalSkillInvalidInputs();
            AssertCanonicalSkillMentorRetentionWorkflow();
            AssertCanonicalSkillCompatibilityBoundaries();
        }

        private static void AssertCanonicalSkillDefinitionCannotBeBypassed()
        {
            var fixture = new CanonicalSkillFixture();
            var alternative = CanonicalSkillFixture.Definition(requiredUnlock: "");
            var progress = new SkillProgress(alternative);
            var request = fixture.Request(progress);
            var validation = fixture.Gateway.Validate(request, fixture.Runtime);

            fixture.Engine.ExecuteTurn(fixture.Runtime, request);

            Console.WriteLine(
                "  AUD-04 weaker same-ID probe: allowed={0}, turn={1}, Essence={2}, "
                + "Resonance={3}, enemy HP={4}, applied={5}, XP={6}",
                validation.IsAllowed, fixture.Runtime.TurnNumber,
                fixture.Player.Resource.Current, fixture.Resonance.Active.Current,
                fixture.Enemy.Stats.CurrentHealth, fixture.Skill.Applied, progress.Experience);

            Assert(!validation.IsAllowed && !fixture.Skill.Applied,
                "AUD-04: an alternative same-ID definition must not bypass the canonical unlock requirement.");
            Assert(validation.BlockReason == CombatSkillUseBlockReason.SkillDefinitionConflict,
                "AUD-04: weaker metadata should expose a stable conflict reason.");
        }

        private static void AssertCanonicalMetadataConflictsAreBlocked()
        {
            var locked = new CanonicalSkillFixture();
            AssertCanonicalRequestBlocked(locked, locked.Request(new SkillProgress(locked.Canonical)),
                CombatSkillUseBlockReason.V3SkillLocked, "canonical locked skill");

            var conflicts = new Dictionary<string, SkillDefinition>
            {
                { "absent unlock", CanonicalSkillFixture.Definition(requiredUnlock: "") },
                { "null unlock", CanonicalSkillFixture.Definition(requiredUnlock: null) },
                { "different unlock", CanonicalSkillFixture.Definition(requiredUnlock: "unlock_weaker") },
                { "name", CanonicalSkillFixture.Definition(name: "Different Name") },
                { "max level/mastery discount", CanonicalSkillFixture.Definition(maxLevel: 1) },
                { "base Essence cost", CanonicalSkillFixture.Definition(baseCost: 0) },
                { "per-level Essence cost", CanonicalSkillFixture.Definition(costPerLevel: 0) },
                { "category", CanonicalSkillFixture.Definition(category: SkillCategory.Utility) },
                { "targeting", CanonicalSkillFixture.Definition(targeting: SkillTargeting.Self) },
                { "effect", CanonicalSkillFixture.Definition(effect: SkillEffectType.Heal) },
                { "Resonance cost", CanonicalSkillFixture.Definition(resonanceCost: 0) }
            };

            foreach (var conflict in conflicts)
            {
                var fixture = new CanonicalSkillFixture();
                fixture.Unlock();
                AssertCanonicalRequestBlocked(fixture, fixture.Request(new SkillProgress(conflict.Value)),
                    CombatSkillUseBlockReason.SkillDefinitionConflict, conflict.Key);
            }

            var weaker = new CanonicalSkillFixture();
            weaker.Unlock("unlock_weaker");
            AssertCanonicalRequestBlocked(weaker,
                weaker.Request(new SkillProgress(conflicts["different unlock"])),
                CombatSkillUseBlockReason.SkillDefinitionConflict, "satisfied alternative unlock");
        }

        private static void AssertCanonicalRequestBlocked(
            CanonicalSkillFixture fixture, TurnRequest request,
            CombatSkillUseBlockReason expected, string scenario)
        {
            int experience = request.SkillProgress == null ? 0 : request.SkillProgress.Experience;
            int level = request.SkillProgress == null ? 0 : request.SkillProgress.Level;
            var result = fixture.Gateway.Validate(request, fixture.Runtime);
            Assert(!result.IsAllowed && result.BlockReason == expected
                && result.FailureStage == CombatSkillValidationOrchestrationFailureStage.Validation,
                "AUD-04 validation reason: " + scenario);
            Assert(fixture.Runtime.TurnNumber == 1 && fixture.Player.Resource.Current == 20
                && fixture.Enemy.Stats.CurrentHealth == 100 && !fixture.Skill.Applied,
                "AUD-04 validation must be read-only: " + scenario);

            fixture.Engine.ExecuteTurn(fixture.Runtime, request);

            Assert(CountEvents(fixture.Runtime.EventLog, GameEventType.ActionBlocked) == 1
                && FindEventMessage(fixture.Runtime.EventLog, GameEventType.ActionBlocked)
                    .Contains(expected.ToString()), "AUD-04 blocked event: " + scenario);
            Assert(fixture.Runtime.TurnNumber == 2
                && CountEvents(fixture.Runtime.EventLog, GameEventType.TurnEnded) == 1,
                "AUD-04 blocked attempt must consume its turn: " + scenario);
            Assert(fixture.Player.Resource.Current == 20
                && CountEvents(fixture.Runtime.EventLog, GameEventType.ResourceSpent) == 0
                && CountEvents(fixture.Runtime.EventLog, GameEventType.ResourceNotEnough) == 0,
                "AUD-04 blocked attempt must not spend Essence: " + scenario);
            Assert(fixture.Resonance.Active.Current == 6 && fixture.Resonance.Reserve.Current == 10,
                "AUD-04 blocked attempt must not spend Resonance: " + scenario);
            Assert(!fixture.Skill.Applied && fixture.Enemy.Stats.CurrentHealth == 100
                && CountEvents(fixture.Runtime.EventLog, GameEventType.SkillUsed) == 0,
                "AUD-04 blocked attempt must not apply effects: " + scenario);
            Assert(request.SkillProgress == null
                || (request.SkillProgress.Experience == experience && request.SkillProgress.Level == level),
                "AUD-04 blocked attempt must not change skill XP or level: " + scenario);
            Assert(ReferenceEquals(fixture.State.SkillRegistry.Get(CanonicalSkillFixture.SkillId),
                    fixture.Canonical) && !fixture.State.MasteryProfile.IsMastered(CanonicalSkillFixture.SkillId),
                "AUD-04 rejection must not replace canonical metadata or grant mastery: " + scenario);
        }

        private static void AssertCanonicalSkillExecutionAndCopies()
        {
            foreach (bool useCopy in new[] { false, true })
            {
                var fixture = new CanonicalSkillFixture();
                fixture.Unlock();
                var definition = useCopy ? CanonicalSkillFixture.Definition() : fixture.Canonical;
                Assert(ReferenceEquals(definition, fixture.Canonical) != useCopy,
                    "AUD-04 copy case must use a distinct definition with a distinct cost object.");
                Assert(!useCopy || !ReferenceEquals(definition.Cost, fixture.Canonical.Cost),
                    "AUD-04 value equality must not require SkillCost reference identity.");
                var progress = new SkillProgress(definition);
                var request = fixture.Request(progress);
                var result = fixture.Gateway.Validate(request, fixture.Runtime);
                Assert(result.IsAllowed && result.BlockReason == CombatSkillUseBlockReason.None,
                    "AUD-04 canonical and value-identical unlocked definitions must pass.");

                fixture.Engine.ExecuteTurn(fixture.Runtime, request);

                Assert(fixture.Skill.Applied && fixture.Enemy.Stats.CurrentHealth == 94
                    && CountEvents(fixture.Runtime.EventLog, GameEventType.ActionBlocked) == 0,
                    "AUD-04 valid canonical metadata must preserve skill effects.");
                Assert(fixture.Player.Resource.Current == 14 && fixture.Resonance.Active.Current == 4
                    && CountEvents(fixture.Runtime.EventLog, GameEventType.ResourceSpent) == 2,
                    "AUD-04 execution charges 11 Essence and 2 Resonance, then regenerates 5 Essence.");
                Assert(progress.Experience == 10 && progress.Level == 1 && fixture.Runtime.TurnNumber == 2,
                    "AUD-04 valid execution preserves XP and turn semantics.");
                Assert(ReferenceEquals(progress.Definition, definition)
                    && ReferenceEquals(fixture.State.SkillRegistry.Get(definition.Id), fixture.Canonical),
                    "AUD-04 validation must not replace request progress or registry ownership.");
            }

            var unrestricted = new CanonicalSkillFixture(CanonicalSkillFixture.Definition(requiredUnlock: ""));
            unrestricted.Engine.ExecuteTurn(unrestricted.Runtime,
                unrestricted.Request(new SkillProgress(unrestricted.Canonical)));
            Assert(unrestricted.Skill.Applied,
                "AUD-04 a canonical skill with no required unlock remains executable.");

            var levelled = new CanonicalSkillFixture();
            levelled.Unlock();
            var levelledProgress = new SkillProgress(CanonicalSkillFixture.Definition());
            levelledProgress.GainExperience(100);
            levelled.Engine.ExecuteTurn(levelled.Runtime, levelled.Request(levelledProgress));
            Assert(levelled.Skill.Applied && levelledProgress.Level == 2
                && levelledProgress.Experience == 10 && levelled.Player.Resource.Current == 13,
                "AUD-04 identical copies preserve existing level, XP, and canonical per-level cost.");
        }

        private static void AssertCanonicalSkillInvalidInputs()
        {
            var fixture = new CanonicalSkillFixture();
            AssertCanonicalRequestBlocked(fixture, fixture.Request(new SkillProgress(null)),
                CombatSkillUseBlockReason.SkillDefinitionMissing, "null definition");
            fixture = new CanonicalSkillFixture();
            AssertCanonicalRequestBlocked(fixture, fixture.Request(null),
                CombatSkillUseBlockReason.SkillProgressMissing, "null progress");
            fixture = new CanonicalSkillFixture();
            AssertCanonicalRequestBlocked(fixture, TurnRequest.SkillAction(fixture.Player, fixture.Enemy,
                null, new SkillProgress(fixture.Canonical)), CombatSkillUseBlockReason.SkillMissing, "null skill");
            fixture = new CanonicalSkillFixture(registerSkill: false);
            var unknownResult = fixture.Gateway.Validate(fixture.Request(new SkillProgress(fixture.Canonical)),
                fixture.Runtime);
            Assert(!unknownResult.IsAllowed
                && unknownResult.BlockReason == CombatSkillUseBlockReason.SkillDefinitionUnregistered,
                "AUD-04 unregistered definitions must fail before unlock evaluation.");
            fixture.Engine.ExecuteTurn(fixture.Runtime, fixture.Request(new SkillProgress(fixture.Canonical)));
            Assert(!fixture.Skill.Applied && fixture.Enemy.Stats.CurrentHealth == 100
                && fixture.Player.Resource.Current == 20 && fixture.Resonance.Active.Current == 6,
                "AUD-04 unknown skill must not execute or spend resources.");

            var nullCost = new SkillDefinition(CanonicalSkillFixture.SkillId, "Audit Skill", 3, null);
            fixture = new CanonicalSkillFixture();
            AssertCanonicalRequestBlocked(fixture, fixture.Request(new SkillProgress(nullCost)),
                CombatSkillUseBlockReason.SkillDefinitionInvalid, "caller cost missing");
            fixture = new CanonicalSkillFixture(nullCost);
            AssertCanonicalRequestBlocked(fixture, fixture.Request(new SkillProgress(fixture.Canonical)),
                CombatSkillUseBlockReason.SkillDefinitionInvalid, "canonical cost missing");

            var validator = new CombatSkillUseValidator();
            Assert(validator.Validate(null, CombatSkillUseValidationContext.LegacyBypass()).BlockReason
                == CombatSkillUseBlockReason.RequestMissing,
                "AUD-04 null request fails safely at the validator boundary.");
            Assert(validator.Validate(fixture.Request(new SkillProgress(fixture.Canonical)),
                (CombatSkillUseValidationContext)null).BlockReason
                == CombatSkillUseBlockReason.ValidationContextMissing,
                "AUD-04 null context fails safely.");
        }

        private static void AssertCanonicalSkillMentorRetentionWorkflow()
        {
            var fixture = new CanonicalSkillFixture();
            fixture.Unlock();
            var progress = new SkillProgress(fixture.Canonical);
            var request = fixture.Request(progress);
            Assert(fixture.Gateway.Validate(request, fixture.Runtime).IsAllowed,
                "AUD-04 legitimate unlock plus current class allows canonical skill.");
            fixture.State.ClassRegistry.Register(new ClassDefinition("class_other", "Other", new string[0]));
            fixture.State.ClassProfile.ChangeClassId("class_other", fixture.State.ClassRegistry);
            Assert(fixture.Gateway.Validate(request, fixture.Runtime).BlockReason
                == CombatSkillUseBlockReason.V4SkillBlocked,
                "AUD-04 unlock alone must not bypass class/mastery restrictions.");

            var teaching = new MentorTeachingDefinition(MentorId.Lyra,
                CanonicalSkillFixture.SkillId, 20, MasteryRecord.MasteredProgress);
            var service = new MentorTeachingService();
            var unavailable = service.Evaluate(teaching, fixture.State.MentorRelationshipProfile,
                fixture.State.MentorRegistry, fixture.State.SkillRegistry);
            Assert(!unavailable.IsAvailable && !new MentorTeachingApplicationService().Apply(unavailable,
                fixture.State.MasteryProfile, fixture.State.SkillRegistry).Applied,
                "AUD-04 unavailable mentor teaching cannot grant retention mastery.");
            fixture.State.MentorRelationshipProfile.Discover(MentorId.Lyra, fixture.State.MentorRegistry);
            fixture.State.MentorRelationshipProfile.IncreaseTrust(MentorId.Lyra, 25, fixture.State.MentorRegistry);
            var available = service.Evaluate(teaching, fixture.State.MentorRelationshipProfile,
                fixture.State.MentorRegistry, fixture.State.SkillRegistry);
            Assert(available.IsAvailable && new MentorTeachingApplicationService().Apply(available,
                fixture.State.MasteryProfile, fixture.State.SkillRegistry).Applied,
                "AUD-04 legitimate mentor teaching still grants mastery.");
            Assert(fixture.Gateway.Validate(request, fixture.Runtime).IsAllowed,
                "AUD-04 canonically unlocked mastered skill is retained after class switch.");
            fixture.Engine.ExecuteTurn(fixture.Runtime, request);
            Assert(fixture.Skill.Applied && progress.Experience == 10,
                "AUD-04 mentor-retained skill executes through the real factory/engine path.");

            var lockedMastery = new CanonicalSkillFixture();
            lockedMastery.State.MasteryProfile.IncreaseProgress(CanonicalSkillFixture.SkillId,
                MasteryRecord.MasteredProgress, lockedMastery.State.SkillRegistry);
            var lockedResult = lockedMastery.Gateway.Validate(
                lockedMastery.Request(new SkillProgress(lockedMastery.Canonical)), lockedMastery.Runtime);
            Assert(!lockedResult.IsAllowed && lockedResult.BlockReason == CombatSkillUseBlockReason.V3SkillLocked,
                "AUD-04 retention mastery must not override the canonical unlock requirement.");
            var conflictResult = lockedMastery.Gateway.Validate(lockedMastery.Request(new SkillProgress(
                CanonicalSkillFixture.Definition(requiredUnlock: ""))), lockedMastery.Runtime);
            Assert(!conflictResult.IsAllowed
                && conflictResult.BlockReason == CombatSkillUseBlockReason.SkillDefinitionConflict,
                "AUD-04 mastered state must not legitimize conflicting caller metadata.");
        }

        private static void AssertCanonicalSkillCompatibilityBoundaries()
        {
            var fixture = new CanonicalSkillFixture();
            var request = fixture.Request(new SkillProgress(fixture.Canonical));
            var validator = new CombatSkillUseValidator();
            Assert(validator.Validate(request, CombatSkillUseValidationContext.LegacyBypass()).IsAllowed,
                "AUD-04 explicit LegacyBypass retains structural-only validation.");
            var enemyRequest = TurnRequest.SkillAction(fixture.Enemy, fixture.Player,
                fixture.Skill, new SkillProgress(fixture.Canonical));
            var enemyResult = fixture.Gateway.Validate(enemyRequest, fixture.Runtime);
            Assert(enemyResult.IsAllowed && enemyResult.ValidationMode == CombatSkillUseValidationMode.Enemy,
                "AUD-04 enemy classification retains its explicit non-player contract.");

            var compatibility = validator.Validate(fixture.Request(new SkillProgress(
                CanonicalSkillFixture.Definition(requiredUnlock: ""))), true,
                fixture.State.UnlockProfile, fixture.State.ClassProfile, fixture.State.ClassRegistry,
                fixture.State.MasteryProfile, fixture.State.SkillRegistry);
            Assert(!compatibility.IsAllowed
                && compatibility.BlockReason == CombatSkillUseBlockReason.SkillDefinitionConflict,
                "AUD-04 compatibility overload must also enforce canonical metadata when a registry is supplied.");
            Assert(validator.Validate(request, true, fixture.State.UnlockProfile,
                null, null, null, null).BlockReason == CombatSkillUseBlockReason.V3SkillLocked,
                "AUD-04 V3-only compatibility validation without a registry retains its unlock check.");

            var basic = TurnRequest.BasicAttack(fixture.Player, fixture.Enemy);
            Assert(!fixture.Gateway.Validate(basic, fixture.Runtime).IsAllowed,
                "AUD-04 validator itself rejects non-skill requests.");
            fixture.Engine.ExecuteTurn(fixture.Runtime, basic);
            Assert(fixture.Enemy.Stats.CurrentHealth == 94
                && CountEvents(fixture.Runtime.EventLog, GameEventType.BasicAttackUsed) == 1
                && CountEvents(fixture.Runtime.EventLog, GameEventType.ActionBlocked) == 0,
                "AUD-04 engine basic attack preserves its validator bypass contract.");

            var regeneration = new CanonicalSkillFixture();
            regeneration.Player.Resource.TrySpend(10);
            regeneration.Engine.ExecuteTurn(regeneration.Runtime,
                regeneration.Request(new SkillProgress(regeneration.Canonical)));
            Assert(regeneration.Player.Resource.Current == 15 && !regeneration.Skill.Applied
                && regeneration.Runtime.TurnNumber == 2,
                "AUD-04 blocked actions retain normal end-turn regeneration, not whole-turn rollback.");
        }

        private sealed class CanonicalSkillFixture
        {
            public const string SkillId = "skill_aud04";
            public const string ClassId = "class_aud04";
            public const string RequiredUnlock = "unlock_aud04";
            public readonly SkillDefinition Canonical;
            public readonly PlayerProgressionRuntimeState State;
            public readonly CombatEntity Player;
            public readonly CombatEntity Enemy;
            public readonly ResonanceProfile Resonance;
            public readonly CombatRuntimeContext Runtime;
            public readonly TrackingCombatSkill Skill = new TrackingCombatSkill(SkillId);
            public readonly CombatSkillTurnValidatorGateway Gateway;
            public readonly TurnBasedCombatEngine Engine;

            public CanonicalSkillFixture(SkillDefinition canonical = null, bool registerSkill = true)
            {
                Canonical = canonical ?? Definition();
                var skills = new SkillDefinitionRegistry();
                if (registerSkill)
                {
                    skills.Register(Canonical);
                }
                var classes = new ClassDefinitionRegistry();
                classes.Register(new ClassDefinition(ClassId, "Audit Class", new[] { SkillId }));
                State = new PlayerProgressionRuntimeState(classes, skills,
                    BuildTestMentorRegistry(), new ClassSkillTreeBindingRegistry());
                State.ClassProfile.AssignClassId(ClassId, classes);
                Player = BuildCombatEntity("aud04_player", 10, 5, 100, 0, 0, 0, 2f, 20);
                Enemy = BuildCombatEntity("aud04_enemy", 5, 5, 100, 0, 0, 0, 1f, 10);
                Resonance = new ResonanceProfile(new ActiveResonance(6), new ResonanceReserve(10));
                Runtime = BuildCombatRuntime(Player, Enemy, Resonance);
                Gateway = new CombatSkillTurnValidatorGateway(State.UnlockProfile,
                    State.ClassProfile, State.MasteryProfile, classes, skills);
                Engine = new ProgressionAwareCombatEngineFactory().CreateEngine(State,
                    new FixedRandomSource(0.0, 0.99), new List<ICombatPassiveEffect>());
            }

            public TurnRequest Request(SkillProgress progress)
            {
                return TurnRequest.SkillAction(Player, Enemy, Skill, progress);
            }

            public void Unlock(string id = RequiredUnlock)
            {
                State.UnlockProfile.RecordUnlock(new UnlockRecord(id, CodexUnlockType.EntryDiscovery,
                    null, "", 0, 0, new string[0]));
            }

            public static SkillDefinition Definition(string requiredUnlock = RequiredUnlock,
                string name = "Audit Skill", int maxLevel = 3, int baseCost = 10, int costPerLevel = 1,
                SkillCategory category = SkillCategory.Combat,
                SkillTargeting targeting = SkillTargeting.SingleEnemy,
                SkillEffectType effect = SkillEffectType.Damage, int resonanceCost = 2)
            {
                return new SkillDefinition(SkillId, name, maxLevel, new SkillCost(baseCost, costPerLevel),
                    category, targeting, effect, requiredUnlock, resonanceCost);
            }
        }
    }
}
