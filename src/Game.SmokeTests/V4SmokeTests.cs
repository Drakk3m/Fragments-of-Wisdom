using System.Collections.Generic;
using System.Text.Json;
using Game.Domain.Classes;
using Game.Domain.Combat;
using Game.Domain.Codex;
using Game.Domain.Missions;
using Game.Domain.Player;
using Game.Domain.Progress;
using Game.Domain.Skills;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void RunV4SmokeTests()
        {
            RunSection(
                "Class foundation assertions",
                delegate
                {
                    AssertClassCanBeAssigned();
                    AssertClassCanBeChanged();
                    AssertClassExposesAllowedSkillFacts();
                    AssertClassDefinitionCanBeRegisteredAndResolved();
                    AssertDuplicateClassIdIsRejectedSafely();
                    AssertCurrentClassDefinitionResolvesFromRegistry();
                    AssertClassChangeStoresOnlyClassId();
                    AssertUnknownClassIdFailsSafely();
                    AssertClassChangeDoesNotEraseUnlockProfileState();
                    AssertInvalidClassIsRejectedSafely();
                    AssertClassSnapshotCapturesCurrentClassId();
                    AssertClassSnapshotRestoreRecreatesClassProfileState();
                    AssertClassSnapshotRestoreUsesRegistryValidation();
                    AssertUnknownClassSnapshotFailsSafelyWithoutCorruptingProfile();
                    AssertOlderJsonWithoutClassDataRestoresSafely();
                });

            RunSection(
                "Skill definition registry assertions",
                delegate
                {
                    AssertSkillDefinitionCanBeRegisteredAndResolved();
                    AssertDuplicateSkillIdHandlingIsSafe();
                    AssertClassAllowedSkillIdsResolveThroughRegistry();
                    AssertUnknownSkillIdFailsSafelyThroughRegistry();
                });

            RunSection(
                "Skill tree progression assertions",
                delegate
                {
                    AssertSkillTreeNodeCanBeRegisteredAndResolved();
                    AssertSkillTreeNodeWithRegisteredSkillIdIsValid();
                    AssertSkillTreeNodeWithExistingPrerequisiteRegistersSuccessfully();
                    AssertSkillTreeNodeWithMissingPrerequisiteIsRejected();
                    AssertSkillTreeNodeWithUnknownSkillIdIsRejected();
                    AssertSkillTreePrerequisiteBlocksUnlockUntilMet();
                    AssertUnlockedPrerequisiteAllowsNextNode();
                    AssertDuplicateSkillTreeNodeIdIsRejectedSafely();
                    AssertValidMultiNodeTreeRemainsUsableByUnlockPolicy();
                });

            RunSection(
                "Skill tree unlock application assertions",
                delegate
                {
                    AssertAllowedSkillTreeNodeUnlockMutatesProgress();
                    AssertBlockedSkillTreePrerequisiteDoesNotMutateProgress();
                    AssertUndiscoveredSkillTreeNodeDoesNotMutateProgress();
                    AssertUnknownSkillTreeNodeApplicationFailsSafely();
                    AssertDuplicateSkillTreeNodeUnlockIsSafe();
                    AssertDirectSkillTreeUnlockMutationIsNotPublic();
                });

            RunSection(
                "Scoped skill tree progress assertions",
                delegate
                {
                    AssertDuplicateNodeIdsRemainScopedBySkillTree();
                    AssertUnlockingNodeInOneTreeDoesNotUnlockAnother();
                    AssertClassBindingUsesResolvedSkillTreeProgressScope();
                    AssertSnapshotCapturesAndRestoresScopedSkillTreeProgress();
                    AssertLegacyUnscopedSkillTreeJsonCompatibilityIsExplicit();
                    AssertValidScopedSnapshotPassesRegistryAwareValidation();
                });

            RunSection(
                "Class skill tree binding assertions",
                delegate
                {
                    AssertClassCanBindToSkillTree();
                    AssertDuplicateClassSkillTreeBindingIsRejectedSafely();
                    AssertUnknownClassSkillTreeBindingFailsSafely();
                    AssertUnknownSkillTreeBindingFailsSafely();
                    AssertCurrentClassCanResolveSkillTreeBinding();
                });

            RunSection(
                "Class skill tree binding save snapshot assertions",
                delegate
                {
                    AssertClassSkillTreeBindingSnapshotCapturesBindings();
                    AssertClassSkillTreeBindingSnapshotRestoreRecreatesBindings();
                    AssertClassSkillTreeBindingSnapshotRestoreValidatesClassIds();
                    AssertClassSkillTreeBindingSnapshotRestoreValidatesSkillTreeIds();
                    AssertUnknownClassSkillTreeBindingSnapshotIdsDoNotCorruptExistingBindings();
                    AssertOlderJsonWithoutClassSkillTreeBindingsRestoresSafely();
                });

            RunSection(
                "Skill tree save snapshot assertions",
                delegate
                {
                    AssertSkillTreeSnapshotCapturesDiscoveredAndUnlockedNodes();
                    AssertSkillTreeSnapshotRestoreRecreatesProgressState();
                    AssertSkillTreeSnapshotRestoreValidatesNodeIdsThroughDefinition();
                    AssertUnknownSkillTreeSnapshotNodeDoesNotCorruptProfile();
                    AssertOlderJsonWithoutSkillTreeProgressRestoresSafely();
                });

            RunSection(
                "V4 restore coordinator assertions",
                delegate
                {
                    AssertV4RestoreCoordinatorRestoresAllProfiles();
                    AssertV4RestoreCoordinatorSuccessHasNoFailureReasons();
                    AssertV4RestoreCoordinatorReportsClassFailure();
                    AssertV4RestoreCoordinatorReportsMasteryFailure();
                    AssertV4RestoreCoordinatorReportsMentorFailure();
                    AssertV4RestoreCoordinatorReportsSkillTreeFailure();
                    AssertV4RestoreCoordinatorPartialFailureIsNotFullSuccess();
                    AssertV4RestoreCoordinatorRestoresClassSkillTreeBindings();
                    AssertV4RestoreCoordinatorReportsClassSkillTreeBindingFailure();
                    AssertV4RestoreCoordinatorBindingFailureIsNotFullSuccess();
                    AssertV4RestoreCoordinatorRestoredBindingResolvesCurrentClassSkillTree();
                    AssertV4RestoreCoordinatorClassPreflightPreventsAllMutation();
                    AssertV4RestoreCoordinatorMasteryPreflightPreventsAllMutation();
                    AssertV4RestoreCoordinatorMentorPreflightPreventsAllMutation();
                    AssertV4RestoreCoordinatorSkillTreePreflightPreventsAllMutation();
                    AssertV4RestoreCoordinatorBindingPreflightPreventsAllMutation();
                    AssertV4RestoreCoordinatorPreflightPreservesFailureDetails();
                });

            RunSection(
                "Mastery foundation assertions",
                delegate
                {
                    AssertMasteryProgressIncreases();
                    AssertMasteryCanReachMasteredState();
                    AssertDuplicateMasteryApplicationIsSafe();
                    AssertMasteryProgressValidatesThroughRegistry();
                    AssertUnknownSkillMasteryFailsSafely();
                });

            RunSection(
                "Mastery save snapshot assertions",
                delegate
                {
                    AssertMasterySnapshotCapturesRecords();
                    AssertMasterySnapshotRestoreRecreatesState();
                    AssertMasterySnapshotRestoreValidatesSkillIdsThroughRegistry();
                    AssertUnknownSkillMasterySnapshotDoesNotCorruptProfile();
                    AssertOlderJsonWithoutMasteryDataRestoresSafely();
                });

            RunSection(
                "Mastered skill retention assertions",
                delegate
                {
                    AssertClassAllowedSkillIsUsableByRetentionPolicy();
                    AssertMasteredCrossClassSkillIsUsableByRetentionPolicy();
                    AssertNonClassNonMasteredSkillIsBlockedByRetentionPolicy();
                    AssertUnknownSkillRetentionFailsSafely();
                    AssertMasteredSkillRetentionWorksWithoutCurrentClass();
                });

            RunSection(
                "Combat-facing skill access assertions",
                delegate
                {
                    AssertCurrentClassSkillIsCombatUsable();
                    AssertMasteredRetainedSkillIsCombatUsableAfterClassSwitch();
                    AssertNonMasteredCrossClassSkillIsBlockedForCombat();
                    AssertUnknownCombatSkillAccessFailsSafely();
                    AssertCombatSkillAccessMatchesRetentionPolicyFacts();
                });

            RunSection(
                "Combat skill use validator assertions",
                delegate
                {
                    AssertValidPlayerCombatSkillUsePassesValidation();
                    AssertLockedV3CombatSkillUseIsBlocked();
                    AssertNonMasteredCrossClassCombatSkillUseIsBlocked();
                    AssertMasteredRetainedCombatSkillUsePassesValidation();
                    AssertEnemyCombatSkillUsePassesWithoutV4Profiles();
                    AssertCombatSkillUseIdMismatchIsBlocked();
                    AssertNullCombatSkillUseInputsFailSafely();
                });

            RunSection(
                "Combat skill use validation context assertions",
                delegate
                {
                    AssertLegacyValidationContextBypassesProgressionRules();
                    AssertEnemyValidationContextBypassesPlayerProgressionRules();
                    AssertPlayerValidationContextRequiresCompleteState();
                    AssertPartialPlayerValidationContextFailsSafely();
                    AssertCompletePlayerValidationContextAllowsValidAccess();
                    AssertCompletePlayerValidationContextBlocksInvalidAccess();
                });

            RunSection(
                "Combat skill use validation context provider assertions",
                delegate
                {
                    AssertValidationContextProviderCreatesLegacyBypass();
                    AssertValidationContextProviderCreatesEnemy();
                    AssertValidationContextProviderCreatesPlayer();
                    AssertValidationContextProviderRejectsMissingPlayerDependencies();
                    AssertValidationContextProviderContextsWorkWithValidator();
                });

            RunSection(
                "Mentor foundation assertions",
                delegate
                {
                    AssertMentorCanBeDiscovered();
                    AssertDuplicateMentorDiscoveryIsIdempotent();
                    AssertMentorTrustCanIncrease();
                    AssertUnavailableMentorIsReportedAsUnavailable();
                    AssertUnknownOrNullMentorIsRejectedSafely();
                });

            RunSection(
                "Mentor definition registry assertions",
                delegate
                {
                    AssertMentorDefinitionCanBeRegisteredAndResolved();
                    AssertDuplicateMentorIdIsRejectedSafely();
                    AssertMentorDiscoveryValidatesThroughRegistry();
                    AssertMentorTrustIncreaseValidatesThroughRegistry();
                    AssertUnknownMentorIdFailsSafelyThroughRegistry();
                });

            RunSection(
                "Mentor save snapshot assertions",
                delegate
                {
                    AssertMentorRelationshipSnapshotCapturesRecords();
                    AssertMentorRelationshipSnapshotRestoreRecreatesState();
                    AssertMentorRelationshipSnapshotRestoreValidatesMentorIdsThroughRegistry();
                    AssertUnknownMentorRelationshipSnapshotDoesNotCorruptProfile();
                    AssertOlderJsonWithoutMentorRelationshipDataRestoresSafely();
                });

            RunSection(
                "Mentor teaching foundation assertions",
                delegate
                {
                    AssertMentorTeachingAvailableWhenTrustRequirementIsMet();
                    AssertMentorTeachingBlockedWhenMentorIsUnknown();
                    AssertMentorTeachingBlockedWhenSkillIsUnknown();
                    AssertMentorTeachingBlockedWhenMentorIsNotDiscovered();
                    AssertMentorTeachingBlockedWhenTrustIsInsufficient();
                    AssertMentorTeachingResultExposesSkillAndMasteryFacts();
                });

            RunSection(
                "Mentor teaching application assertions",
                delegate
                {
                    AssertZeroProgressMentorTeachingIsNotApplied();
                    AssertAvailableMentorTeachingAppliesMasteryProgress();
                    AssertBlockedMentorTeachingDoesNotMutateMastery();
                    AssertMentorTeachingApplicationUnknownSkillFailsSafely();
                    AssertMentorTeachingApplicationCanMasterSkill();
                    AssertDuplicateMentorTeachingApplicationAfterMasteryIsSafe();
                });

            RunSection(
                "Integrated workflow assertions",
                delegate
                {
                    AssertClassMentorTeachingMasteryWorkflow();
                    AssertMasteredSkillRetentionIntegratedWorkflow();
                    AssertCombatFacingSkillAccessIntegratedWorkflow();
                    AssertCombatSkillUseValidatorIntegratedWorkflow();
                    AssertCombatSkillUseValidationContextIntegratedWorkflow();
                    AssertSkillTreeProgressionIntegratedWorkflow();
                    AssertSkillTreeUnlockApplicationIntegratedWorkflow();
                    AssertClassSkillTreeBindingIntegratedWorkflow();
                    AssertClassSkillTreeBindingSaveRestoreIntegratedWorkflow();
                    AssertScopedSkillTreeProgressIntegratedWorkflow();
                });
        }

        private static void AssertClassCanBeAssigned()
        {
            var profile = new ClassProfile();
            var classDefinition = ClassBootstrap.BuildTlamatini();

            bool assigned = profile.AssignClass(classDefinition);

            Assert(
                assigned,
                "Valid class should be assignable.");

            Assert(
                profile.CurrentClassId == ClassId.Tlamatini
                && profile.ResolveCurrentClass(ClassBootstrap.BuildDefinitionRegistry()).Id == ClassId.Tlamatini,
                "Class profile should store the assigned current class id.");
        }

        private static void AssertClassCanBeChanged()
        {
            var profile = new ClassProfile();
            var firstClass = ClassBootstrap.BuildTlamatini();
            var secondClass = BuildWandererClass();

            profile.AssignClass(firstClass);
            bool changed = profile.ChangeClass(secondClass);

            Assert(
                changed,
                "Valid class change should be accepted.");

            Assert(
                profile.CurrentClassId == ClassId.Wanderer
                && profile.ResolveCurrentClass(BuildTestClassRegistry()).Id == secondClass.Id,
                "Class profile should replace current class id on change.");
        }

        private static void AssertClassExposesAllowedSkillFacts()
        {
            var classDefinition = ClassBootstrap.BuildTlamatini();
            var profile = new ClassProfile();
            var registry = ClassBootstrap.BuildDefinitionRegistry();

            profile.AssignClass(classDefinition);

            Assert(
                classDefinition.AllowedSkillIds.Count == 3
                && classDefinition.AllowsSkill("skill_deep_sight"),
                "Class definition should expose allowed skill ids.");

            Assert(
                profile.CurrentClassAllowsSkill("skill_balance_ritual", registry)
                && !profile.CurrentClassAllowsSkill("skill_unknown", registry),
                "Class profile should expose current class skill access facts.");
        }

        private static void AssertClassDefinitionCanBeRegisteredAndResolved()
        {
            var registry = new ClassDefinitionRegistry();
            var classDefinition = BuildWandererClass();

            bool registered = registry.Register(classDefinition);

            Assert(
                registered,
                "Class definition registry should accept valid class definitions by stable id.");

            Assert(
                registry.Contains(ClassId.Wanderer)
                && registry.Get(ClassId.Wanderer) == classDefinition,
                "Class definition registry should resolve the registered definition by id.");
        }

        private static void AssertDuplicateClassIdIsRejectedSafely()
        {
            var registry = new ClassDefinitionRegistry();
            var original = new ClassDefinition(
                ClassId.Wanderer,
                "Original Wanderer",
                new[] { "skill_minor_cleanse" });
            var duplicate = new ClassDefinition(
                ClassId.Wanderer,
                "Duplicate Wanderer",
                new[] { "skill_deep_sight" });

            bool firstRegistered = registry.Register(original);
            bool duplicateRegistered = registry.Register(duplicate);

            Assert(
                firstRegistered && !duplicateRegistered,
                "Class definition registry should reject duplicate class ids safely.");

            Assert(
                registry.Get(ClassId.Wanderer) == original
                && registry.Get(ClassId.Wanderer).Name == "Original Wanderer",
                "Duplicate class ids should not replace the original definition.");
        }

        private static void AssertCurrentClassDefinitionResolvesFromRegistry()
        {
            var profile = new ClassProfile();
            var registry = ClassBootstrap.BuildDefinitionRegistry();

            profile.AssignClassId(ClassId.Tlamatini, registry);
            var currentClass = profile.ResolveCurrentClass(registry);

            Assert(
                currentClass != null && currentClass.Id == ClassId.Tlamatini,
                "Current class definition should resolve from profile id and registry.");
        }

        private static void AssertClassChangeStoresOnlyClassId()
        {
            var profile = new ClassProfile();
            var registry = BuildTestClassRegistry();

            profile.AssignClassId(ClassId.Tlamatini, registry);
            bool changed = profile.ChangeClassId(ClassId.Wanderer, registry);

            Assert(
                changed && profile.CurrentClassId == ClassId.Wanderer,
                "Class change by registry should store only the stable class id.");

            Assert(
                profile.ResolveCurrentClass(registry).Name == "Wanderer",
                "Current class facts should be resolved from the registry after class change.");
        }

        private static void AssertUnknownClassIdFailsSafely()
        {
            var profile = new ClassProfile();
            var registry = ClassBootstrap.BuildDefinitionRegistry();

            bool assigned = profile.AssignClassId("class_unknown", registry);

            Assert(
                !assigned,
                "Unknown class id should fail safely.");

            Assert(
                profile.CurrentClassId == string.Empty
                && profile.ResolveCurrentClass(registry) == null,
                "Unknown class id should not mutate current class state.");
        }

        private static void AssertClassChangeDoesNotEraseUnlockProfileState()
        {
            var classProfile = new ClassProfile();
            var unlockProfile = new UnlockProfile();

            unlockProfile.RecordUnlock(new UnlockRecord(
                CodexUnlockIds.NahualPathDiscovered,
                CodexUnlockType.EntryDiscovery,
                null,
                CodexEntryIds.NahualPath,
                0,
                0,
                new List<string>()));

            classProfile.AssignClass(ClassBootstrap.BuildTlamatini());
            classProfile.ChangeClass(BuildWandererClass());

            Assert(
                unlockProfile.HasUnlock(CodexUnlockIds.NahualPathDiscovered),
                "Class changes should not erase existing unlock profile state.");
        }

        private static void AssertInvalidClassIsRejectedSafely()
        {
            var profile = new ClassProfile();
            var validClass = ClassBootstrap.BuildTlamatini();
            var invalidClass = new ClassDefinition(
                "",
                "Invalid",
                new List<string>());

            bool nullAssigned = profile.AssignClass(null);
            bool validAssigned = profile.AssignClass(validClass);
            bool invalidChanged = profile.ChangeClass(invalidClass);

            Assert(
                !nullAssigned && validAssigned && !invalidChanged,
                "Null or invalid classes should be rejected safely.");

            Assert(
                profile.CurrentClassId == ClassId.Tlamatini,
                "Rejected class changes should preserve the current class.");
        }

        private static ClassDefinition BuildWandererClass()
        {
            return new ClassDefinition(
                ClassId.Wanderer,
                "Wanderer",
                new[]
                {
                    "skill_minor_cleanse"
                });
        }

        private static ClassDefinitionRegistry BuildTestClassRegistry()
        {
            var registry = ClassBootstrap.BuildDefinitionRegistry();
            registry.Register(BuildWandererClass());
            return registry;
        }

        private static void AssertClassSnapshotCapturesCurrentClassId()
        {
            var classProfile = new ClassProfile();
            classProfile.AssignClass(ClassBootstrap.BuildTlamatini());

            var snapshot = new V2ProgressSnapshotService().CreateSnapshot(
                new CodexProfile(),
                new MissionJournalProfile(),
                new UnlockProfile(),
                classProfile);

            Assert(
                snapshot.ClassProfile != null
                && snapshot.ClassProfile.CurrentClassId == ClassId.Tlamatini,
                "Snapshot should capture current class id only.");
        }

        private static void AssertClassSnapshotRestoreRecreatesClassProfileState()
        {
            var sourceProfile = new ClassProfile();
            sourceProfile.AssignClass(ClassBootstrap.BuildTlamatini());
            var service = new V2ProgressSnapshotService();
            var snapshot = service.CreateSnapshot(
                new CodexProfile(),
                new MissionJournalProfile(),
                new UnlockProfile(),
                sourceProfile);
            var restoredProfile = new ClassProfile();

            bool restored = service.RestoreClassProfile(
                snapshot,
                restoredProfile,
                ClassBootstrap.BuildDefinitionRegistry());

            Assert(
                restored,
                "Known class snapshot should restore successfully.");

            Assert(
                restoredProfile.CurrentClassId == ClassId.Tlamatini,
                "Restore should recreate class profile current class id.");
        }

        private static void AssertClassSnapshotRestoreUsesRegistryValidation()
        {
            var snapshot = new V2ProgressSnapshot(
                V2ProgressSnapshotVersions.CurrentSchemaVersion,
                new List<V2CodexEntrySnapshot>(),
                new List<V2MissionLeadSnapshot>(),
                new List<V2ActiveMissionSnapshot>(),
                new List<V2UnlockSnapshot>(),
                new V2ClassProfileSnapshot(ClassId.Wanderer));
            var profile = new ClassProfile();

            bool restored = new V2ProgressSnapshotService().RestoreClassProfile(
                snapshot,
                profile,
                ClassBootstrap.BuildDefinitionRegistry());

            Assert(
                !restored,
                "Class snapshot restore should reject ids missing from the registry.");

            Assert(
                profile.CurrentClassId == string.Empty,
                "Rejected class snapshot restore should not mutate profile state.");
        }

        private static void AssertUnknownClassSnapshotFailsSafelyWithoutCorruptingProfile()
        {
            var profile = new ClassProfile();
            profile.AssignClass(ClassBootstrap.BuildTlamatini());
            var snapshot = new V2ProgressSnapshot(
                V2ProgressSnapshotVersions.CurrentSchemaVersion,
                new List<V2CodexEntrySnapshot>(),
                new List<V2MissionLeadSnapshot>(),
                new List<V2ActiveMissionSnapshot>(),
                new List<V2UnlockSnapshot>(),
                new V2ClassProfileSnapshot("class_unknown"));

            bool restored = new V2ProgressSnapshotService().RestoreClassProfile(
                snapshot,
                profile,
                ClassBootstrap.BuildDefinitionRegistry());

            Assert(
                !restored,
                "Unknown class id should fail safely during restore.");

            Assert(
                profile.CurrentClassId == ClassId.Tlamatini,
                "Unknown class id restore should preserve existing class profile state.");
        }

        private static void AssertOlderJsonWithoutClassDataRestoresSafely()
        {
            string json = JsonSerializer.Serialize(new
            {
                SchemaVersion = V2ProgressSnapshotVersions.CurrentSchemaVersion,
                CodexEntries = new object[0],
                MissionLeads = new object[0],
                ActiveMissions = new object[0],
                Unlocks = new object[0]
            });
            var snapshot = new V2ProgressSnapshotJsonSerializer().Deserialize(json);
            var profile = new ClassProfile();

            bool restored = new V2ProgressSnapshotService().RestoreClassProfile(
                snapshot,
                profile,
                ClassBootstrap.BuildDefinitionRegistry());

            Assert(
                restored,
                "Older JSON without class data should restore safely.");

            Assert(
                profile.CurrentClassId == string.Empty,
                "Older JSON without class data should leave class profile unchanged.");
        }

        private static void AssertMasteryProgressIncreases()
        {
            var profile = new MasteryProfile();
            var skillRegistry = BuildTestSkillRegistry();

            bool increased = profile.IncreaseProgress(
                "skill_deep_sight",
                25,
                skillRegistry);
            var record = profile.GetRecord("skill_deep_sight");

            Assert(
                increased,
                "Known skill mastery progress should increase.");

            Assert(
                record != null
                && record.Progress == 25
                && record.Rank == MasteryRank.Practiced,
                "Mastery profile should track practiced progress for a skill.");
        }

        private static void AssertMasteryCanReachMasteredState()
        {
            var profile = new MasteryProfile();
            var skillRegistry = BuildTestSkillRegistry();

            profile.IncreaseProgress("skill_balance_ritual", 40, skillRegistry);
            bool increased = profile.IncreaseProgress(
                "skill_balance_ritual",
                80,
                skillRegistry);
            var record = profile.GetRecord("skill_balance_ritual");

            Assert(
                increased,
                "Known skill mastery progress should continue increasing until mastered.");

            Assert(
                record.Progress == MasteryRecord.MasteredProgress
                && record.Rank == MasteryRank.Mastered
                && profile.IsMastered("skill_balance_ritual"),
                "Mastery progress should clamp at mastered state.");
        }

        private static void AssertDuplicateMasteryApplicationIsSafe()
        {
            var profile = new MasteryProfile();
            var skillRegistry = BuildTestSkillRegistry();

            bool firstIncrease = profile.IncreaseProgress(
                "skill_minor_cleanse",
                MasteryRecord.MasteredProgress,
                skillRegistry);
            bool duplicateIncrease = profile.IncreaseProgress(
                "skill_minor_cleanse",
                10,
                skillRegistry);
            var record = profile.GetRecord("skill_minor_cleanse");

            Assert(
                firstIncrease && !duplicateIncrease,
                "Duplicate mastery application after mastered state should be ignored safely.");

            Assert(
                profile.Records.Count == 1
                && record.Progress == MasteryRecord.MasteredProgress
                && profile.IsMastered("skill_minor_cleanse"),
                "Duplicate mastery application should not duplicate records or exceed mastered progress.");
        }

        private static void AssertMasteryProgressValidatesThroughRegistry()
        {
            var profile = new MasteryProfile();
            var skillRegistry = new SkillDefinitionRegistry();
            var skillDefinition = BuildTestSkillDefinition(
                "skill_deep_sight",
                "Deep Sight");

            bool beforeRegistered = profile.IncreaseProgress(
                skillDefinition.Id,
                10,
                skillRegistry);
            bool registered = skillRegistry.Register(skillDefinition);
            bool afterRegistered = profile.IncreaseProgress(
                skillDefinition.Id,
                10,
                skillRegistry);

            Assert(
                !beforeRegistered && registered && afterRegistered,
                "Mastery progress should validate skill ids through the skill registry.");

            Assert(
                profile.GetRecord(skillDefinition.Id).Progress == 10,
                "Mastery progress should apply only after the skill definition is registered.");
        }

        private static void AssertUnknownSkillMasteryFailsSafely()
        {
            var profile = new MasteryProfile();
            var skillRegistry = BuildTestSkillRegistry();

            bool increased = profile.IncreaseProgress(
                "skill_unknown",
                50,
                skillRegistry);

            Assert(
                !increased,
                "Unknown skill ids should fail safely for mastery progress.");

            Assert(
                profile.Records.Count == 0
                && profile.GetRecord("skill_unknown") == null
                && !profile.IsMastered("skill_unknown"),
                "Unknown skill mastery should not create records or mastered facts.");
        }

        private static void AssertMasterySnapshotCapturesRecords()
        {
            var masteryProfile = BuildMasteryProfileWithProgress();
            var snapshot = new V2ProgressSnapshotService().CreateSnapshot(
                new CodexProfile(),
                new MissionJournalProfile(),
                new UnlockProfile(),
                new ClassProfile(),
                masteryProfile);

            Assert(
                snapshot.MasteryRecords.Count == 2,
                "Snapshot should capture mastery records.");

            Assert(
                snapshot.MasteryRecords[0].SkillId == "skill_deep_sight"
                && snapshot.MasteryRecords[0].Progress == 25
                && snapshot.MasteryRecords[0].Rank == MasteryRank.Practiced,
                "Snapshot should preserve mastery skill id, progress, and rank.");
        }

        private static void AssertMasterySnapshotRestoreRecreatesState()
        {
            var sourceProfile = BuildMasteryProfileWithProgress();
            var service = new V2ProgressSnapshotService();
            var snapshot = service.CreateSnapshot(
                new CodexProfile(),
                new MissionJournalProfile(),
                new UnlockProfile(),
                new ClassProfile(),
                sourceProfile);
            var restoredProfile = new MasteryProfile();

            bool restored = service.RestoreMasteryProfile(
                snapshot,
                restoredProfile,
                BuildTestSkillRegistry());

            Assert(
                restored,
                "Known mastery records should restore successfully.");

            Assert(
                restoredProfile.GetRecord("skill_deep_sight").Progress == 25
                && restoredProfile.IsMastered("skill_balance_ritual"),
                "Restore should recreate mastery progress and mastered facts.");
        }

        private static void AssertMasterySnapshotRestoreValidatesSkillIdsThroughRegistry()
        {
            var snapshot = new V2ProgressSnapshot(
                V2ProgressSnapshotVersions.CurrentSchemaVersion,
                new List<V2CodexEntrySnapshot>(),
                new List<V2MissionLeadSnapshot>(),
                new List<V2ActiveMissionSnapshot>(),
                new List<V2UnlockSnapshot>(),
                null,
                new[]
                {
                    new V2MasterySnapshot(
                        "skill_unknown",
                        MasteryRecord.MasteredProgress,
                        MasteryRank.Mastered)
                });
            var restoredProfile = new MasteryProfile();

            bool restored = new V2ProgressSnapshotService().RestoreMasteryProfile(
                snapshot,
                restoredProfile,
                BuildTestSkillRegistry());

            Assert(
                !restored,
                "Unknown mastery snapshot skill ids should be reported as not fully restored.");

            Assert(
                restoredProfile.Records.Count == 0,
                "Unknown mastery snapshot skill ids should not create mastery records.");
        }

        private static void AssertUnknownSkillMasterySnapshotDoesNotCorruptProfile()
        {
            var profile = new MasteryProfile();
            var skillRegistry = BuildTestSkillRegistry();
            profile.IncreaseProgress("skill_deep_sight", 25, skillRegistry);
            var snapshot = new V2ProgressSnapshot(
                V2ProgressSnapshotVersions.CurrentSchemaVersion,
                new List<V2CodexEntrySnapshot>(),
                new List<V2MissionLeadSnapshot>(),
                new List<V2ActiveMissionSnapshot>(),
                new List<V2UnlockSnapshot>(),
                null,
                new[]
                {
                    new V2MasterySnapshot(
                        "skill_unknown",
                        MasteryRecord.MasteredProgress,
                        MasteryRank.Mastered)
                });

            bool restored = new V2ProgressSnapshotService().RestoreMasteryProfile(
                snapshot,
                profile,
                skillRegistry);

            Assert(
                !restored,
                "Unknown mastery snapshot skill ids should fail safely without corrupting existing state.");

            Assert(
                profile.Records.Count == 1
                && profile.GetRecord("skill_deep_sight").Progress == 25
                && !profile.IsMastered("skill_unknown"),
                "Unknown mastery snapshot skill ids should preserve existing mastery records.");
        }

        private static void AssertOlderJsonWithoutMasteryDataRestoresSafely()
        {
            string json = JsonSerializer.Serialize(new
            {
                SchemaVersion = V2ProgressSnapshotVersions.CurrentSchemaVersion,
                CodexEntries = new object[0],
                MissionLeads = new object[0],
                ActiveMissions = new object[0],
                Unlocks = new object[0],
                ClassProfile = new
                {
                    CurrentClassId = ""
                }
            });
            var snapshot = new V2ProgressSnapshotJsonSerializer().Deserialize(json);
            var profile = new MasteryProfile();

            bool restored = new V2ProgressSnapshotService().RestoreMasteryProfile(
                snapshot,
                profile,
                BuildTestSkillRegistry());

            Assert(
                restored,
                "Older JSON without mastery data should restore safely.");

            Assert(
                snapshot.MasteryRecords.Count == 0
                && profile.Records.Count == 0,
                "Older JSON without mastery data should leave mastery state unchanged.");
        }

        private static void AssertClassAllowedSkillIsUsableByRetentionPolicy()
        {
            var classProfile = new ClassProfile();
            var classRegistry = BuildTestClassRegistry();
            var skillRegistry = BuildTestSkillRegistry();

            classProfile.AssignClassId(ClassId.Tlamatini, classRegistry);

            var result = new MasteredSkillRetentionPolicy().Evaluate(
                "skill_deep_sight",
                classProfile,
                classRegistry,
                new MasteryProfile(),
                skillRegistry);

            Assert(
                result.IsUsable
                && result.AllowedByCurrentClass
                && !result.AllowedByMastery
                && result.BlockReason == MasteredSkillRetentionBlockReason.None,
                "Class-allowed skill should be usable through the retention policy.");
        }

        private static void AssertMasteredCrossClassSkillIsUsableByRetentionPolicy()
        {
            var classProfile = new ClassProfile();
            var classRegistry = BuildTestClassRegistry();
            var skillRegistry = BuildTestSkillRegistry();
            var masteryProfile = new MasteryProfile();

            classProfile.AssignClassId(ClassId.Wanderer, classRegistry);
            masteryProfile.IncreaseProgress(
                "skill_deep_sight",
                MasteryRecord.MasteredProgress,
                skillRegistry);

            var result = new MasteredSkillRetentionPolicy().Evaluate(
                "skill_deep_sight",
                classProfile,
                classRegistry,
                masteryProfile,
                skillRegistry);

            Assert(
                result.IsUsable
                && !result.AllowedByCurrentClass
                && result.AllowedByMastery,
                "Mastered cross-class skill should be usable through mastery facts.");
        }

        private static void AssertNonClassNonMasteredSkillIsBlockedByRetentionPolicy()
        {
            var classProfile = new ClassProfile();
            var classRegistry = BuildTestClassRegistry();
            var skillRegistry = BuildTestSkillRegistry();

            classProfile.AssignClassId(ClassId.Tlamatini, classRegistry);

            var result = new MasteredSkillRetentionPolicy().Evaluate(
                "skill_minor_cleanse",
                classProfile,
                classRegistry,
                new MasteryProfile(),
                skillRegistry);

            Assert(
                !result.IsUsable
                && !result.AllowedByCurrentClass
                && !result.AllowedByMastery
                && result.BlockReason == MasteredSkillRetentionBlockReason.SkillNotAllowed,
                "Non-class non-mastered skill should be blocked by the retention policy.");
        }

        private static void AssertUnknownSkillRetentionFailsSafely()
        {
            var result = new MasteredSkillRetentionPolicy().Evaluate(
                "skill_unknown",
                new ClassProfile(),
                BuildTestClassRegistry(),
                new MasteryProfile(),
                BuildTestSkillRegistry());

            Assert(
                !result.IsUsable
                && !result.AllowedByCurrentClass
                && !result.AllowedByMastery
                && result.BlockReason == MasteredSkillRetentionBlockReason.UnknownSkill,
                "Unknown skill ids should fail safely in the retention policy.");
        }

        private static void AssertMasteredSkillRetentionWorksWithoutCurrentClass()
        {
            var skillRegistry = BuildTestSkillRegistry();
            var masteryProfile = new MasteryProfile();

            masteryProfile.IncreaseProgress(
                "skill_deep_sight",
                MasteryRecord.MasteredProgress,
                skillRegistry);

            var result = new MasteredSkillRetentionPolicy().Evaluate(
                "skill_deep_sight",
                new ClassProfile(),
                BuildTestClassRegistry(),
                masteryProfile,
                skillRegistry);

            Assert(
                result.IsUsable
                && !result.AllowedByCurrentClass
                && result.AllowedByMastery,
                "Mastered registered skill should remain usable even without a current class.");
        }

        private static void AssertCurrentClassSkillIsCombatUsable()
        {
            var classProfile = new ClassProfile();
            var classRegistry = BuildTestClassRegistry();

            classProfile.AssignClassId(ClassId.Tlamatini, classRegistry);

            var result = new CombatSkillAccessService().Evaluate(
                "skill_deep_sight",
                classProfile,
                classRegistry,
                new MasteryProfile(),
                BuildTestSkillRegistry());

            Assert(
                result.IsUsable
                && result.AllowedByCurrentClass
                && !result.AllowedByMastery
                && result.BlockReason == MasteredSkillRetentionBlockReason.None,
                "Current-class skill should be combat-usable.");
        }

        private static void AssertMasteredRetainedSkillIsCombatUsableAfterClassSwitch()
        {
            var classProfile = new ClassProfile();
            var classRegistry = BuildTestClassRegistry();
            var skillRegistry = BuildTestSkillRegistry();
            var masteryProfile = new MasteryProfile();

            classProfile.AssignClassId(ClassId.Tlamatini, classRegistry);
            masteryProfile.IncreaseProgress(
                "skill_deep_sight",
                MasteryRecord.MasteredProgress,
                skillRegistry);
            classProfile.ChangeClassId(ClassId.Wanderer, classRegistry);

            var result = new CombatSkillAccessService().Evaluate(
                "skill_deep_sight",
                classProfile,
                classRegistry,
                masteryProfile,
                skillRegistry);

            Assert(
                result.IsUsable
                && !result.AllowedByCurrentClass
                && result.AllowedByMastery,
                "Mastered retained skill should remain combat-usable after class switch.");
        }

        private static void AssertNonMasteredCrossClassSkillIsBlockedForCombat()
        {
            var classProfile = new ClassProfile();
            var classRegistry = BuildTestClassRegistry();

            classProfile.AssignClassId(ClassId.Tlamatini, classRegistry);

            var result = new CombatSkillAccessService().Evaluate(
                "skill_minor_cleanse",
                classProfile,
                classRegistry,
                new MasteryProfile(),
                BuildTestSkillRegistry());

            Assert(
                !result.IsUsable
                && !result.AllowedByCurrentClass
                && !result.AllowedByMastery
                && result.BlockReason == MasteredSkillRetentionBlockReason.SkillNotAllowed,
                "Non-mastered cross-class skill should be blocked for combat.");
        }

        private static void AssertUnknownCombatSkillAccessFailsSafely()
        {
            var result = new CombatSkillAccessService().Evaluate(
                "skill_unknown",
                new ClassProfile(),
                BuildTestClassRegistry(),
                new MasteryProfile(),
                BuildTestSkillRegistry());

            Assert(
                !result.IsUsable
                && result.SkillId == "skill_unknown"
                && result.BlockReason == MasteredSkillRetentionBlockReason.UnknownSkill,
                "Unknown skill id should fail safely for combat access.");
        }

        private static void AssertCombatSkillAccessMatchesRetentionPolicyFacts()
        {
            var classProfile = new ClassProfile();
            var classRegistry = BuildTestClassRegistry();
            var skillRegistry = BuildTestSkillRegistry();
            var masteryProfile = new MasteryProfile();
            var policy = new MasteredSkillRetentionPolicy();

            classProfile.AssignClassId(ClassId.Tlamatini, classRegistry);

            var policyResult = policy.Evaluate(
                "skill_deep_sight",
                classProfile,
                classRegistry,
                masteryProfile,
                skillRegistry);
            var accessResult = new CombatSkillAccessService(policy).Evaluate(
                "skill_deep_sight",
                classProfile,
                classRegistry,
                masteryProfile,
                skillRegistry);

            Assert(
                accessResult.SkillId == policyResult.SkillId
                && accessResult.IsUsable == policyResult.IsUsable
                && accessResult.AllowedByCurrentClass
                    == policyResult.AllowedByCurrentClass
                && accessResult.AllowedByMastery == policyResult.AllowedByMastery
                && accessResult.BlockReason == policyResult.BlockReason,
                "Combat skill access should expose the exact retention policy facts.");
        }

        private static void AssertValidPlayerCombatSkillUsePassesValidation()
        {
            const string skillId = "skill_validator_player";
            const string classId = "class_validator_player";
            var definition = BuildTestSkillDefinition(skillId, "Validator Player");
            var skillRegistry = new SkillDefinitionRegistry();
            skillRegistry.Register(definition);
            var classRegistry = new ClassDefinitionRegistry();
            classRegistry.Register(new ClassDefinition(
                classId,
                "Validator Player Class",
                new[] { skillId }));
            var classProfile = new ClassProfile();
            classProfile.AssignClassId(classId, classRegistry);

            var result = new CombatSkillUseValidator().Validate(
                TurnRequest.SkillAction(
                    null,
                    null,
                    new V4ValidatorCombatSkill(skillId),
                    new SkillProgress(definition)),
                true,
                new UnlockProfile(),
                classProfile,
                classRegistry,
                new MasteryProfile(),
                skillRegistry);

            Assert(
                result.IsAllowed
                && result.SkillId == skillId
                && result.IsPlayerAction
                && result.V4AccessChecked
                && result.BlockReason == CombatSkillUseBlockReason.None,
                "Valid player skill should pass composed combat skill use validation.");
        }

        private static void AssertLockedV3CombatSkillUseIsBlocked()
        {
            const string skillId = "skill_validator_locked";
            var definition = BuildValidatorSkillDefinition(
                skillId,
                CodexUnlockIds.ForbiddenPracticeCount);

            var result = new CombatSkillUseValidator().Validate(
                TurnRequest.SkillAction(
                    null,
                    null,
                    new V4ValidatorCombatSkill(skillId),
                    new SkillProgress(definition)),
                true,
                new UnlockProfile(),
                null,
                null,
                null,
                null);

            Assert(
                !result.IsAllowed
                && result.V3AccessChecked
                && !result.V4AccessChecked
                && result.BlockReason == CombatSkillUseBlockReason.V3SkillLocked
                && result.V3BlockReason == SkillAccessPolicy.MissingUnlockBlockReason,
                "Locked V3 skill should be blocked with stable validator facts.");
        }

        private static void AssertNonMasteredCrossClassCombatSkillUseIsBlocked()
        {
            const string skillId = "skill_validator_cross_class";
            const string classId = "class_validator_cross_class";
            var definition = BuildTestSkillDefinition(skillId, "Validator Cross Class");
            var skillRegistry = new SkillDefinitionRegistry();
            skillRegistry.Register(definition);
            var classRegistry = new ClassDefinitionRegistry();
            classRegistry.Register(new ClassDefinition(
                classId,
                "Validator Empty Class",
                new string[0]));
            var classProfile = new ClassProfile();
            classProfile.AssignClassId(classId, classRegistry);

            var result = new CombatSkillUseValidator().Validate(
                TurnRequest.SkillAction(
                    null,
                    null,
                    new V4ValidatorCombatSkill(skillId),
                    new SkillProgress(definition)),
                true,
                new UnlockProfile(),
                classProfile,
                classRegistry,
                new MasteryProfile(),
                skillRegistry);

            Assert(
                !result.IsAllowed
                && result.V4AccessChecked
                && result.BlockReason == CombatSkillUseBlockReason.V4SkillBlocked
                && result.V4BlockReason
                    == MasteredSkillRetentionBlockReason.SkillNotAllowed,
                "Non-mastered cross-class skill should be blocked by V4 access validation.");
        }

        private static void AssertMasteredRetainedCombatSkillUsePassesValidation()
        {
            const string skillId = "skill_validator_mastered";
            const string classId = "class_validator_mastered";
            var definition = BuildTestSkillDefinition(skillId, "Validator Mastered");
            var skillRegistry = new SkillDefinitionRegistry();
            skillRegistry.Register(definition);
            var classRegistry = new ClassDefinitionRegistry();
            classRegistry.Register(new ClassDefinition(
                classId,
                "Validator Other Class",
                new string[0]));
            var classProfile = new ClassProfile();
            classProfile.AssignClassId(classId, classRegistry);
            var masteryProfile = new MasteryProfile();
            masteryProfile.IncreaseProgress(
                skillId,
                MasteryRecord.MasteredProgress,
                skillRegistry);

            var result = new CombatSkillUseValidator().Validate(
                TurnRequest.SkillAction(
                    null,
                    null,
                    new V4ValidatorCombatSkill(skillId),
                    new SkillProgress(definition)),
                true,
                new UnlockProfile(),
                classProfile,
                classRegistry,
                masteryProfile,
                skillRegistry);

            Assert(
                result.IsAllowed
                && result.V4AccessChecked
                && result.BlockReason == CombatSkillUseBlockReason.None,
                "Mastered retained skill should pass V4 combat skill use validation.");
        }

        private static void AssertEnemyCombatSkillUsePassesWithoutV4Profiles()
        {
            const string skillId = "skill_validator_enemy";
            var definition = BuildValidatorSkillDefinition(
                skillId,
                CodexUnlockIds.ForbiddenPracticeCount);

            var result = new CombatSkillUseValidator().Validate(
                TurnRequest.SkillAction(
                    null,
                    null,
                    new V4ValidatorCombatSkill(skillId),
                    new SkillProgress(definition)),
                false,
                null,
                null,
                null,
                null,
                null);

            Assert(
                result.IsAllowed
                && !result.IsPlayerAction
                && !result.V3AccessChecked
                && !result.V4AccessChecked,
                "Explicit enemy mode should not require player unlock or V4 profiles.");
        }

        private static void AssertCombatSkillUseIdMismatchIsBlocked()
        {
            var result = new CombatSkillUseValidator().Validate(
                TurnRequest.SkillAction(
                    null,
                    null,
                    new V4ValidatorCombatSkill("skill_validator_runtime"),
                    new SkillProgress(BuildTestSkillDefinition(
                        "skill_validator_progress",
                        "Validator Progress"))),
                true,
                new UnlockProfile(),
                null,
                null,
                null,
                null);

            Assert(
                !result.IsAllowed
                && result.BlockReason
                    == CombatSkillUseBlockReason.SkillProgressIdMismatch,
                "Skill and progress definition id mismatch should be blocked safely.");
        }

        private static void AssertNullCombatSkillUseInputsFailSafely()
        {
            var validator = new CombatSkillUseValidator();
            var missingRequest = validator.Validate(
                null,
                true,
                null,
                null,
                null,
                null,
                null);
            var missingSkill = validator.Validate(
                TurnRequest.SkillAction(null, null, null, null),
                true,
                null,
                null,
                null,
                null,
                null);
            var missingProgress = validator.Validate(
                TurnRequest.SkillAction(
                    null,
                    null,
                    new V4ValidatorCombatSkill("skill_validator_null"),
                    null),
                true,
                null,
                null,
                null,
                null,
                null);

            Assert(
                !missingRequest.IsAllowed
                && missingRequest.BlockReason
                    == CombatSkillUseBlockReason.RequestMissing
                && missingRequest.SkillId == string.Empty
                && !missingSkill.IsAllowed
                && missingSkill.BlockReason
                    == CombatSkillUseBlockReason.SkillMissing
                && !missingProgress.IsAllowed
                && missingProgress.BlockReason
                    == CombatSkillUseBlockReason.SkillProgressMissing,
                "Null combat skill use inputs should return stable failure facts.");
        }

        private static void AssertLegacyValidationContextBypassesProgressionRules()
        {
            const string skillId = "skill_context_legacy";
            var definition = BuildValidatorSkillDefinition(
                skillId,
                CodexUnlockIds.ForbiddenPracticeCount);

            var result = new CombatSkillUseValidator().Validate(
                TurnRequest.SkillAction(
                    null,
                    null,
                    new V4ValidatorCombatSkill(skillId),
                    new SkillProgress(definition)),
                CombatSkillUseValidationContext.LegacyBypass());

            Assert(
                result.IsAllowed
                && result.IsPlayerAction
                && !result.V3AccessChecked
                && !result.V4AccessChecked,
                "Legacy bypass context should allow structurally valid skills without progression profiles.");
        }

        private static void AssertEnemyValidationContextBypassesPlayerProgressionRules()
        {
            const string skillId = "skill_context_enemy";
            var definition = BuildValidatorSkillDefinition(
                skillId,
                CodexUnlockIds.ForbiddenPracticeCount);

            var result = new CombatSkillUseValidator().Validate(
                TurnRequest.SkillAction(
                    null,
                    null,
                    new V4ValidatorCombatSkill(skillId),
                    new SkillProgress(definition)),
                CombatSkillUseValidationContext.Enemy());

            Assert(
                result.IsAllowed
                && !result.IsPlayerAction
                && !result.V3AccessChecked
                && !result.V4AccessChecked,
                "Enemy context should allow structurally valid skills without player progression profiles.");
        }

        private static void AssertPlayerValidationContextRequiresCompleteState()
        {
            const string skillId = "skill_context_player_required";
            var definition = BuildTestSkillDefinition(
                skillId,
                "Context Player Required");
            var context = CombatSkillUseValidationContext.Player(
                null,
                null,
                null,
                null,
                null);

            var result = new CombatSkillUseValidator().Validate(
                TurnRequest.SkillAction(
                    null,
                    null,
                    new V4ValidatorCombatSkill(skillId),
                    new SkillProgress(definition)),
                context);

            Assert(
                context.Mode == CombatSkillUseValidationMode.Player
                && !context.HasCompletePlayerState
                && !result.IsAllowed
                && result.BlockReason
                    == CombatSkillUseBlockReason.PlayerContextIncomplete,
                "Player context should require all progression profiles and registries.");
        }

        private static void AssertPartialPlayerValidationContextFailsSafely()
        {
            const string skillId = "skill_context_player_partial";
            var definition = BuildTestSkillDefinition(
                skillId,
                "Context Player Partial");
            var context = CombatSkillUseValidationContext.Player(
                new UnlockProfile(),
                new ClassProfile(),
                null,
                new ClassDefinitionRegistry(),
                new SkillDefinitionRegistry());

            var result = new CombatSkillUseValidator().Validate(
                TurnRequest.SkillAction(
                    null,
                    null,
                    new V4ValidatorCombatSkill(skillId),
                    new SkillProgress(definition)),
                context);

            Assert(
                !result.IsAllowed
                && result.IsPlayerAction
                && result.BlockReason
                    == CombatSkillUseBlockReason.PlayerContextIncomplete,
                "Partial player validation context should fail safely.");
        }

        private static void AssertCompletePlayerValidationContextAllowsValidAccess()
        {
            const string skillId = "skill_context_player_allowed";
            const string classId = "class_context_player_allowed";
            var definition = BuildTestSkillDefinition(
                skillId,
                "Context Player Allowed");
            var skillRegistry = new SkillDefinitionRegistry();
            skillRegistry.Register(definition);
            var classRegistry = new ClassDefinitionRegistry();
            classRegistry.Register(new ClassDefinition(
                classId,
                "Context Player Allowed Class",
                new[] { skillId }));
            var classProfile = new ClassProfile();
            classProfile.AssignClassId(classId, classRegistry);
            var context = CombatSkillUseValidationContext.Player(
                new UnlockProfile(),
                classProfile,
                new MasteryProfile(),
                classRegistry,
                skillRegistry);

            var result = new CombatSkillUseValidator().Validate(
                TurnRequest.SkillAction(
                    null,
                    null,
                    new V4ValidatorCombatSkill(skillId),
                    new SkillProgress(definition)),
                context);

            Assert(
                context.HasCompletePlayerState
                && result.IsAllowed
                && result.V4AccessChecked
                && result.BlockReason == CombatSkillUseBlockReason.None,
                "Complete player context should allow valid current-class access.");
        }

        private static void AssertCompletePlayerValidationContextBlocksInvalidAccess()
        {
            const string skillId = "skill_context_player_blocked";
            const string classId = "class_context_player_blocked";
            var definition = BuildTestSkillDefinition(
                skillId,
                "Context Player Blocked");
            var skillRegistry = new SkillDefinitionRegistry();
            skillRegistry.Register(definition);
            var classRegistry = new ClassDefinitionRegistry();
            classRegistry.Register(new ClassDefinition(
                classId,
                "Context Player Empty Class",
                new string[0]));
            var classProfile = new ClassProfile();
            classProfile.AssignClassId(classId, classRegistry);
            var context = CombatSkillUseValidationContext.Player(
                new UnlockProfile(),
                classProfile,
                new MasteryProfile(),
                classRegistry,
                skillRegistry);

            var result = new CombatSkillUseValidator().Validate(
                TurnRequest.SkillAction(
                    null,
                    null,
                    new V4ValidatorCombatSkill(skillId),
                    new SkillProgress(definition)),
                context);

            Assert(
                !result.IsAllowed
                && result.V4AccessChecked
                && result.BlockReason == CombatSkillUseBlockReason.V4SkillBlocked
                && result.V4BlockReason
                    == MasteredSkillRetentionBlockReason.SkillNotAllowed,
                "Complete player context should preserve blocked V4 access facts.");
        }

        private static void AssertValidationContextProviderCreatesLegacyBypass()
        {
            var context = new CombatSkillUseValidationContextProvider()
                .CreateLegacyBypass();

            Assert(
                context != null
                && context.Mode == CombatSkillUseValidationMode.LegacyBypass
                && !context.HasCompletePlayerState,
                "Context provider should create an explicit legacy bypass context.");
        }

        private static void AssertValidationContextProviderCreatesEnemy()
        {
            var context = new CombatSkillUseValidationContextProvider()
                .CreateEnemy();

            Assert(
                context != null
                && context.Mode == CombatSkillUseValidationMode.Enemy
                && !context.HasCompletePlayerState,
                "Context provider should create an enemy context without player progression state.");
        }

        private static void AssertValidationContextProviderCreatesPlayer()
        {
            var provider = new CombatSkillUseValidationContextProvider();
            var unlockProfile = new UnlockProfile();
            var classProfile = new ClassProfile();
            var masteryProfile = new MasteryProfile();
            var classRegistry = new ClassDefinitionRegistry();
            var skillRegistry = new SkillDefinitionRegistry();
            CombatSkillUseValidationContext context;

            bool created = provider.TryCreatePlayer(
                unlockProfile,
                classProfile,
                masteryProfile,
                classRegistry,
                skillRegistry,
                out context);

            Assert(
                created
                && context != null
                && context.Mode == CombatSkillUseValidationMode.Player
                && context.HasCompletePlayerState
                && context.UnlockProfile == unlockProfile
                && context.ClassProfile == classProfile
                && context.MasteryProfile == masteryProfile
                && context.ClassRegistry == classRegistry
                && context.SkillRegistry == skillRegistry,
                "Context provider should preserve all required player dependencies.");
        }

        private static void AssertValidationContextProviderRejectsMissingPlayerDependencies()
        {
            var provider = new CombatSkillUseValidationContextProvider();
            CombatSkillUseValidationContext context;

            bool created = provider.TryCreatePlayer(
                new UnlockProfile(),
                new ClassProfile(),
                null,
                new ClassDefinitionRegistry(),
                new SkillDefinitionRegistry(),
                out context);

            Assert(
                !created && context == null,
                "Context provider should fail safely when a player dependency is missing.");
        }

        private static void AssertValidationContextProviderContextsWorkWithValidator()
        {
            const string skillId = "skill_context_provider";
            const string classId = "class_context_provider";
            var definition = BuildTestSkillDefinition(
                skillId,
                "Context Provider Skill");
            var skillRegistry = new SkillDefinitionRegistry();
            skillRegistry.Register(definition);
            var classRegistry = new ClassDefinitionRegistry();
            classRegistry.Register(new ClassDefinition(
                classId,
                "Context Provider Class",
                new[] { skillId }));
            var classProfile = new ClassProfile();
            classProfile.AssignClassId(classId, classRegistry);
            var provider = new CombatSkillUseValidationContextProvider();
            CombatSkillUseValidationContext playerContext;
            provider.TryCreatePlayer(
                new UnlockProfile(),
                classProfile,
                new MasteryProfile(),
                classRegistry,
                skillRegistry,
                out playerContext);
            var request = TurnRequest.SkillAction(
                null,
                null,
                new V4ValidatorCombatSkill(skillId),
                new SkillProgress(definition));
            var validator = new CombatSkillUseValidator();

            var legacyResult = validator.Validate(
                request,
                provider.CreateLegacyBypass());
            var enemyResult = validator.Validate(
                request,
                provider.CreateEnemy());
            var playerResult = validator.Validate(
                request,
                playerContext);

            Assert(
                legacyResult.IsAllowed
                && enemyResult.IsAllowed
                && playerResult.IsAllowed
                && playerResult.V4AccessChecked,
                "Provider contexts should work with combat skill use validation.");
        }

        private static void AssertSkillDefinitionCanBeRegisteredAndResolved()
        {
            var registry = new SkillDefinitionRegistry();
            var definition = BuildTestSkillDefinition(
                "skill_deep_sight",
                "Deep Sight");

            bool registered = registry.Register(definition);
            var resolved = registry.Get("skill_deep_sight");

            Assert(
                registered && registry.Contains("skill_deep_sight"),
                "Skill definition registry should accept valid skill definitions by stable id.");

            Assert(
                resolved == definition && resolved.Name == "Deep Sight",
                "Skill definition registry should resolve the registered definition by id.");
        }

        private static void AssertDuplicateSkillIdHandlingIsSafe()
        {
            var registry = new SkillDefinitionRegistry();
            var firstDefinition = BuildTestSkillDefinition(
                "skill_deep_sight",
                "Deep Sight");
            var duplicateDefinition = BuildTestSkillDefinition(
                "skill_deep_sight",
                "Duplicate Deep Sight");

            bool firstRegistered = registry.Register(firstDefinition);
            bool duplicateRegistered = registry.Register(duplicateDefinition);

            Assert(
                firstRegistered && !duplicateRegistered,
                "Duplicate skill ids should be rejected safely.");

            Assert(
                registry.Get("skill_deep_sight") == firstDefinition,
                "Duplicate skill ids should not replace the original definition.");
        }

        private static void AssertClassAllowedSkillIdsResolveThroughRegistry()
        {
            var skillRegistry = BuildTestSkillRegistry();
            var classDefinition = new ClassDefinition(
                "class_test",
                "Test Class",
                new[]
                {
                    "skill_deep_sight",
                    "skill_unknown"
                });

            var resolvedSkills = classDefinition.ResolveAllowedSkills(skillRegistry);

            Assert(
                resolvedSkills.Count == 1
                && resolvedSkills[0].Id == "skill_deep_sight",
                "Class allowed skill ids should resolve registered skill definitions.");

            Assert(
                classDefinition.AllowsSkill("skill_deep_sight", skillRegistry)
                && !classDefinition.AllowsSkill("skill_unknown", skillRegistry),
                "Class registry-aware skill access should reject unknown allowed skill ids.");
        }

        private static void AssertUnknownSkillIdFailsSafelyThroughRegistry()
        {
            var registry = BuildTestSkillRegistry();

            Assert(
                !registry.Contains("skill_unknown"),
                "Unknown skill ids should not be reported as registered.");

            Assert(
                registry.Get("skill_unknown") == null
                && !registry.Register(BuildTestSkillDefinition("", "Invalid Skill")),
                "Unknown or invalid skill ids should fail safely.");
        }

        private static void AssertSkillTreeNodeCanBeRegisteredAndResolved()
        {
            var skillTree = new SkillTreeDefinition();
            var node = new SkillTreeNodeDefinition(
                "node_deep_sight",
                "skill_deep_sight",
                new string[0]);

            bool registered = skillTree.RegisterNode(
                node,
                BuildTestSkillRegistry());

            Assert(
                registered
                && skillTree.Contains("node_deep_sight")
                && skillTree.Get("node_deep_sight") == node,
                "Skill tree node should register and resolve by stable node id.");
        }

        private static void AssertSkillTreeNodeWithRegisteredSkillIdIsValid()
        {
            var node = new SkillTreeNodeDefinition(
                "node_deep_sight",
                "skill_deep_sight",
                new string[0]);

            Assert(
                SkillTreeDefinition.IsValidNode(
                    node,
                    BuildTestSkillRegistry()),
                "Skill tree node with registered skill id should be valid.");
        }

        private static void AssertSkillTreeNodeWithExistingPrerequisiteRegistersSuccessfully()
        {
            var skillTree = new SkillTreeDefinition();
            var skillRegistry = BuildTestSkillRegistry();
            var rootNode = new SkillTreeNodeDefinition(
                "node_deep_sight",
                "skill_deep_sight",
                new string[0]);
            var dependentNode = new SkillTreeNodeDefinition(
                "node_balance_ritual",
                "skill_balance_ritual",
                new[] { "node_deep_sight" });

            bool rootRegistered = skillTree.RegisterNode(rootNode, skillRegistry);
            bool dependentRegistered = skillTree.RegisterNode(dependentNode, skillRegistry);

            Assert(
                rootRegistered
                && dependentRegistered
                && skillTree.Get("node_balance_ritual") == dependentNode,
                "Skill tree node should register when every prerequisite already exists in the same tree.");
        }

        private static void AssertSkillTreeNodeWithMissingPrerequisiteIsRejected()
        {
            var skillTree = new SkillTreeDefinition();
            var node = new SkillTreeNodeDefinition(
                "node_balance_ritual",
                "skill_balance_ritual",
                new[] { "node_missing" });

            bool registered = skillTree.RegisterNode(
                node,
                BuildTestSkillRegistry());

            Assert(
                !registered
                && !skillTree.Contains("node_balance_ritual"),
                "Skill tree node with a missing prerequisite id should be rejected safely.");
        }

        private static void AssertSkillTreeNodeWithUnknownSkillIdIsRejected()
        {
            var skillTree = new SkillTreeDefinition();
            var node = new SkillTreeNodeDefinition(
                "node_unknown",
                "skill_unknown",
                new string[0]);

            bool registered = skillTree.RegisterNode(
                node,
                BuildTestSkillRegistry());

            Assert(
                !registered
                && !SkillTreeDefinition.IsValidNode(node, BuildTestSkillRegistry())
                && !skillTree.Contains("node_unknown"),
                "Skill tree node with unknown skill id should be rejected safely.");
        }

        private static void AssertSkillTreePrerequisiteBlocksUnlockUntilMet()
        {
            var skillTree = BuildTestSkillTreeDefinition();
            var progress = new SkillTreeProgressProfile();

            progress.DiscoverNode("node_balance_ritual", skillTree);

            var result = new SkillTreeUnlockPolicy().Evaluate(
                "node_balance_ritual",
                skillTree,
                progress,
                BuildTestSkillRegistry());

            Assert(
                !result.CanUnlock
                && result.BlockReason == SkillTreeUnlockBlockReason.PrerequisiteMissing
                && result.MissingPrerequisiteNodeId == "node_deep_sight",
                "Skill tree prerequisite should block unlock until the prerequisite node is unlocked.");
        }

        private static void AssertUnlockedPrerequisiteAllowsNextNode()
        {
            var skillTree = BuildTestSkillTreeDefinition();
            var progress = new SkillTreeProgressProfile();
            var skillRegistry = BuildTestSkillRegistry();

            progress.DiscoverNode("node_deep_sight", skillTree);
            new SkillTreeUnlockApplicationService().Apply(
                "node_deep_sight",
                skillTree,
                progress,
                skillRegistry);
            progress.DiscoverNode("node_balance_ritual", skillTree);

            var result = new SkillTreeUnlockPolicy().Evaluate(
                "node_balance_ritual",
                skillTree,
                progress,
                skillRegistry);

            Assert(
                result.CanUnlock
                && result.BlockReason == SkillTreeUnlockBlockReason.None,
                "Unlocked prerequisite should allow the next skill tree node to unlock.");
        }

        private static void AssertDuplicateSkillTreeNodeIdIsRejectedSafely()
        {
            var skillTree = new SkillTreeDefinition();
            var original = new SkillTreeNodeDefinition(
                "node_deep_sight",
                "skill_deep_sight",
                new string[0]);
            var duplicate = new SkillTreeNodeDefinition(
                "node_deep_sight",
                "skill_balance_ritual",
                new string[0]);

            bool firstRegistered = skillTree.RegisterNode(
                original,
                BuildTestSkillRegistry());
            bool duplicateRegistered = skillTree.RegisterNode(
                duplicate,
                BuildTestSkillRegistry());

            Assert(
                firstRegistered
                && !duplicateRegistered
                && skillTree.Get("node_deep_sight") == original,
                "Duplicate skill tree node ids should be rejected without replacing the original node.");
        }

        private static void AssertValidMultiNodeTreeRemainsUsableByUnlockPolicy()
        {
            var skillTree = BuildTestSkillTreeDefinition();
            var progress = new SkillTreeProgressProfile();
            var skillRegistry = BuildTestSkillRegistry();

            progress.DiscoverNode("node_deep_sight", skillTree);
            new SkillTreeUnlockApplicationService().Apply(
                "node_deep_sight",
                skillTree,
                progress,
                skillRegistry);
            progress.DiscoverNode("node_balance_ritual", skillTree);

            var result = new SkillTreeUnlockPolicy().Evaluate(
                "node_balance_ritual",
                skillTree,
                progress,
                skillRegistry);

            Assert(
                skillTree.Nodes.Count == 2
                && result.CanUnlock
                && result.BlockReason == SkillTreeUnlockBlockReason.None,
                "Valid multi-node tree should remain usable by SkillTreeUnlockPolicy.");
        }

        private static void AssertAllowedSkillTreeNodeUnlockMutatesProgress()
        {
            var skillTree = BuildTestSkillTreeDefinition();
            var progress = new SkillTreeProgressProfile();

            progress.DiscoverNode("node_deep_sight", skillTree);

            var result = new SkillTreeUnlockApplicationService().Apply(
                "node_deep_sight",
                skillTree,
                progress,
                BuildTestSkillRegistry());

            Assert(
                result.Applied
                && result.NodeId == "node_deep_sight"
                && result.BlockReason == SkillTreeUnlockBlockReason.None
                && !result.WasAlreadyUnlocked,
                "Allowed skill tree node application should report the unlock as applied.");

            Assert(
                progress.IsUnlocked("node_deep_sight"),
                "Allowed skill tree node application should mutate progress.");
        }

        private static void AssertBlockedSkillTreePrerequisiteDoesNotMutateProgress()
        {
            var skillTree = BuildTestSkillTreeDefinition();
            var progress = new SkillTreeProgressProfile();

            progress.DiscoverNode("node_balance_ritual", skillTree);

            var result = new SkillTreeUnlockApplicationService().Apply(
                "node_balance_ritual",
                skillTree,
                progress,
                BuildTestSkillRegistry());

            Assert(
                !result.Applied
                && result.BlockReason == SkillTreeUnlockBlockReason.PrerequisiteMissing,
                "Missing prerequisite should block skill tree unlock application.");

            Assert(
                !progress.IsUnlocked("node_balance_ritual"),
                "Blocked prerequisite should not mutate skill tree progress.");
        }

        private static void AssertUndiscoveredSkillTreeNodeDoesNotMutateProgress()
        {
            var skillTree = BuildTestSkillTreeDefinition();
            var progress = new SkillTreeProgressProfile();

            var result = new SkillTreeUnlockApplicationService().Apply(
                "node_deep_sight",
                skillTree,
                progress,
                BuildTestSkillRegistry());

            Assert(
                !result.Applied
                && result.BlockReason == SkillTreeUnlockBlockReason.NodeNotDiscovered,
                "Undiscovered skill tree node should be blocked by the unlock policy.");

            Assert(
                !progress.IsDiscovered("node_deep_sight")
                && !progress.IsUnlocked("node_deep_sight"),
                "Undiscovered skill tree node application should not mutate progress.");
        }

        private static void AssertUnknownSkillTreeNodeApplicationFailsSafely()
        {
            var skillTree = BuildTestSkillTreeDefinition();
            var progress = new SkillTreeProgressProfile();

            var result = new SkillTreeUnlockApplicationService().Apply(
                "node_unknown",
                skillTree,
                progress,
                BuildTestSkillRegistry());

            Assert(
                !result.Applied
                && result.NodeId == "node_unknown"
                && result.BlockReason == SkillTreeUnlockBlockReason.UnknownNode,
                "Unknown skill tree node application should fail safely.");

            Assert(
                progress.DiscoveredNodeIds.Count == 0
                && progress.UnlockedNodeIds.Count == 0,
                "Unknown skill tree node application should not mutate progress.");
        }

        private static void AssertDuplicateSkillTreeNodeUnlockIsSafe()
        {
            var skillTree = BuildTestSkillTreeDefinition();
            var progress = new SkillTreeProgressProfile();
            var applicationService = new SkillTreeUnlockApplicationService();

            progress.DiscoverNode("node_deep_sight", skillTree);

            var firstResult = applicationService.Apply(
                "node_deep_sight",
                skillTree,
                progress,
                BuildTestSkillRegistry());
            int unlockedCountAfterFirstApplication = progress.UnlockedNodeIds.Count;
            var duplicateResult = applicationService.Apply(
                "node_deep_sight",
                skillTree,
                progress,
                BuildTestSkillRegistry());

            Assert(
                firstResult.Applied
                && !duplicateResult.Applied
                && duplicateResult.WasAlreadyUnlocked
                && duplicateResult.BlockReason == SkillTreeUnlockBlockReason.None,
                "Duplicate skill tree unlock application should report an idempotent existing unlock.");

            Assert(
                progress.IsUnlocked("node_deep_sight")
                && progress.UnlockedNodeIds.Count == unlockedCountAfterFirstApplication,
                "Duplicate skill tree unlock application should not duplicate progress state.");
        }

        private static void AssertDirectSkillTreeUnlockMutationIsNotPublic()
        {
            Assert(
                typeof(SkillTreeProgressProfile).GetMethod("UnlockNode") == null
                && typeof(SkillTreeProgressProfile).GetMethod("RecordUnlockedNode") == null,
                "Skill tree progress should not expose a public direct unlock mutation path.");
        }

        private static void AssertDuplicateNodeIdsRemainScopedBySkillTree()
        {
            var firstTree = BuildTestSkillTreeDefinition(SkillTreeId.Tlamatini);
            var secondTree = BuildTestSkillTreeDefinition(SkillTreeId.Wanderer);
            var progress = new SkillTreeProgressProfile();

            bool discovered = progress.DiscoverNode("node_deep_sight", firstTree);

            Assert(
                discovered
                && progress.IsDiscovered("node_deep_sight", firstTree)
                && !progress.IsDiscovered("node_deep_sight", secondTree),
                "Identical node ids should remain isolated by skill tree id.");

            Assert(
                progress.IsDiscovered("node_deep_sight"),
                "Legacy single-node lookup should remain an aggregate compatibility view.");
        }

        private static void AssertUnlockingNodeInOneTreeDoesNotUnlockAnother()
        {
            var firstTree = BuildTestSkillTreeDefinition(SkillTreeId.Tlamatini);
            var secondTree = BuildTestSkillTreeDefinition(SkillTreeId.Wanderer);
            var progress = new SkillTreeProgressProfile();
            var applicationService = new SkillTreeUnlockApplicationService();

            progress.DiscoverNode("node_deep_sight", firstTree);
            var result = applicationService.Apply(
                "node_deep_sight",
                firstTree,
                progress,
                BuildTestSkillRegistry());

            Assert(
                result.Applied
                && progress.IsUnlocked("node_deep_sight", firstTree),
                "Unlock application should record progress in the requested skill tree scope.");

            Assert(
                !progress.IsUnlocked("node_deep_sight", secondTree),
                "Unlocking a node in one skill tree should not unlock the same node id in another tree.");
        }

        private static void AssertClassBindingUsesResolvedSkillTreeProgressScope()
        {
            var classRegistry = BuildTestClassRegistry();
            var bindingRegistry = BuildTestClassSkillTreeBindingRegistryWithBindings();
            var classProfile = new ClassProfile();
            var progress = new SkillTreeProgressProfile();
            var applicationService = new SkillTreeUnlockApplicationService();

            classProfile.AssignClassId(ClassId.Tlamatini, classRegistry);
            var firstTree = bindingRegistry.ResolveCurrentClassSkillTree(classProfile);
            progress.DiscoverNode("node_deep_sight", firstTree);
            var firstResult = applicationService.Apply(
                "node_deep_sight",
                firstTree,
                progress,
                BuildTestSkillRegistry());

            classProfile.ChangeClassId(ClassId.Wanderer, classRegistry);
            var secondTree = bindingRegistry.ResolveCurrentClassSkillTree(classProfile);
            var secondResult = applicationService.Apply(
                "node_deep_sight",
                secondTree,
                progress,
                BuildTestSkillRegistry());

            Assert(
                firstResult.Applied
                && firstTree.Id == SkillTreeId.Tlamatini
                && secondTree.Id == SkillTreeId.Wanderer,
                "Class binding should resolve the skill tree used for scoped progress.");

            Assert(
                !secondResult.Applied
                && secondResult.BlockReason == SkillTreeUnlockBlockReason.NodeNotDiscovered
                && !progress.IsUnlocked("node_deep_sight", secondTree),
                "Changing class should use the resolved tree scope instead of colliding on node id.");
        }

        private static void AssertSnapshotCapturesAndRestoresScopedSkillTreeProgress()
        {
            var bindingRegistry = BuildTestClassSkillTreeBindingRegistry();
            var firstTree = bindingRegistry.GetSkillTree(SkillTreeId.Tlamatini);
            var secondTree = bindingRegistry.GetSkillTree(SkillTreeId.Wanderer);
            var progress = new SkillTreeProgressProfile();
            var applicationService = new SkillTreeUnlockApplicationService();

            progress.DiscoverNode("node_deep_sight", firstTree);
            applicationService.Apply(
                "node_deep_sight",
                firstTree,
                progress,
                BuildTestSkillRegistry());
            progress.DiscoverNode("node_deep_sight", secondTree);

            var service = new V2ProgressSnapshotService();
            var snapshot = service.CreateSnapshot(
                new CodexProfile(),
                new MissionJournalProfile(),
                new UnlockProfile(),
                new ClassProfile(),
                new MasteryProfile(),
                new MentorRelationshipProfile(),
                progress);
            string json = new V2ProgressSnapshotJsonSerializer().Serialize(snapshot);
            var deserialized = new V2ProgressSnapshotJsonSerializer().Deserialize(json);
            var restoredProgress = new SkillTreeProgressProfile();
            bool restored = service.RestoreSkillTreeProgressProfile(
                deserialized,
                restoredProgress,
                bindingRegistry);

            Assert(
                snapshot.SkillTreeProgress.Scopes.Count == 2
                && deserialized.SkillTreeProgress.Scopes.Count == 2,
                "Snapshot JSON should preserve one progress record per stable skill tree id.");

            Assert(
                restored
                && restoredProgress.IsUnlocked("node_deep_sight", firstTree)
                && restoredProgress.IsDiscovered("node_deep_sight", secondTree)
                && !restoredProgress.IsUnlocked("node_deep_sight", secondTree),
                "Scoped snapshot restore should recreate each tree's independent progress.");
        }

        private static void AssertLegacyUnscopedSkillTreeJsonCompatibilityIsExplicit()
        {
            string json = "{"
                + "\"SchemaVersion\":" + V2ProgressSnapshotVersions.CurrentSchemaVersion + ","
                + "\"CodexEntries\":[],"
                + "\"MissionLeads\":[],"
                + "\"ActiveMissions\":[],"
                + "\"Unlocks\":[],"
                + "\"MasteryRecords\":[],"
                + "\"MentorRelationships\":[],"
                + "\"SkillTreeProgress\":{"
                + "\"DiscoveredNodeIds\":[\"node_deep_sight\"],"
                + "\"UnlockedNodeIds\":[\"node_deep_sight\"]"
                + "}"
                + "}";
            var snapshot = new V2ProgressSnapshotJsonSerializer().Deserialize(json);
            var explicitTree = BuildTestSkillTreeDefinition(SkillTreeId.Tlamatini);
            var explicitProgress = new SkillTreeProgressProfile();
            var ambiguousProgress = new SkillTreeProgressProfile();
            var service = new V2ProgressSnapshotService();

            bool explicitRestore = service.RestoreSkillTreeProgressProfile(
                snapshot,
                explicitProgress,
                explicitTree);
            bool ambiguousRestore = service.RestoreSkillTreeProgressProfile(
                snapshot,
                ambiguousProgress,
                BuildTestClassSkillTreeBindingRegistry());

            Assert(
                explicitRestore
                && explicitProgress.IsUnlocked("node_deep_sight", explicitTree),
                "Legacy unscoped JSON should restore into an explicitly supplied skill tree.");

            Assert(
                !ambiguousRestore
                && ambiguousProgress.UnlockedNodeIds.Count == 0,
                "Legacy unscoped JSON should fail safely when multiple target trees make scope ambiguous.");
        }

        private static void AssertValidScopedSnapshotPassesRegistryAwareValidation()
        {
            var skillTreeRegistry = BuildTestClassSkillTreeBindingRegistry();
            var skillTree = skillTreeRegistry.GetSkillTree(SkillTreeId.Tlamatini);
            var progress = BuildSkillTreeProgressWithUnlockedRoot(skillTree);
            var snapshot = new V2ProgressSnapshotService().CreateSnapshot(
                new CodexProfile(),
                new MissionJournalProfile(),
                new UnlockProfile(),
                new ClassProfile(),
                new MasteryProfile(),
                new MentorRelationshipProfile(),
                progress);

            Assert(
                new V2ProgressSnapshotValidationService().IsValid(
                    snapshot,
                    skillTreeRegistry),
                "Registry-aware validation should accept valid scoped skill tree progress.");
        }

        private static void AssertClassCanBindToSkillTree()
        {
            var classRegistry = BuildTestClassRegistry();
            var bindingRegistry = BuildTestClassSkillTreeBindingRegistry();
            var binding = new ClassSkillTreeBinding(
                ClassId.Tlamatini,
                SkillTreeId.Tlamatini);

            bool registered = bindingRegistry.RegisterBinding(
                binding,
                classRegistry);

            Assert(
                registered
                && bindingRegistry.GetBindingForClass(ClassId.Tlamatini) == binding
                && bindingRegistry.ResolveSkillTreeForClass(ClassId.Tlamatini).Id == SkillTreeId.Tlamatini,
                "Class should bind to a registered skill tree by stable ids.");
        }

        private static void AssertDuplicateClassSkillTreeBindingIsRejectedSafely()
        {
            var classRegistry = BuildTestClassRegistry();
            var bindingRegistry = BuildTestClassSkillTreeBindingRegistry();
            var original = new ClassSkillTreeBinding(
                ClassId.Tlamatini,
                SkillTreeId.Tlamatini);
            var duplicate = new ClassSkillTreeBinding(
                ClassId.Tlamatini,
                SkillTreeId.Wanderer);

            bool firstRegistered = bindingRegistry.RegisterBinding(
                original,
                classRegistry);
            bool duplicateRegistered = bindingRegistry.RegisterBinding(
                duplicate,
                classRegistry);

            Assert(
                firstRegistered
                && !duplicateRegistered
                && bindingRegistry.GetBindingForClass(ClassId.Tlamatini) == original
                && bindingRegistry.ResolveSkillTreeForClass(ClassId.Tlamatini).Id == SkillTreeId.Tlamatini,
                "Duplicate class skill tree bindings should be rejected without replacing the original binding.");
        }

        private static void AssertUnknownClassSkillTreeBindingFailsSafely()
        {
            var bindingRegistry = BuildTestClassSkillTreeBindingRegistry();
            var binding = new ClassSkillTreeBinding(
                "class_unknown",
                SkillTreeId.Tlamatini);

            bool registered = bindingRegistry.RegisterBinding(
                binding,
                BuildTestClassRegistry());

            Assert(
                !registered
                && bindingRegistry.GetBindingForClass("class_unknown") == null,
                "Class skill tree binding should reject unknown class ids safely.");
        }

        private static void AssertUnknownSkillTreeBindingFailsSafely()
        {
            var bindingRegistry = new ClassSkillTreeBindingRegistry();
            var binding = new ClassSkillTreeBinding(
                ClassId.Tlamatini,
                "skill_tree_unknown");

            bool registered = bindingRegistry.RegisterBinding(
                binding,
                BuildTestClassRegistry());

            Assert(
                !registered
                && bindingRegistry.GetBindingForClass(ClassId.Tlamatini) == null,
                "Class skill tree binding should reject unknown skill tree ids safely.");
        }

        private static void AssertCurrentClassCanResolveSkillTreeBinding()
        {
            var classRegistry = BuildTestClassRegistry();
            var bindingRegistry = BuildTestClassSkillTreeBindingRegistry();
            var profile = new ClassProfile();

            profile.AssignClassId(ClassId.Tlamatini, classRegistry);
            bindingRegistry.RegisterBinding(
                new ClassSkillTreeBinding(
                    ClassId.Tlamatini,
                    SkillTreeId.Tlamatini),
                classRegistry);

            var currentSkillTree = bindingRegistry.ResolveCurrentClassSkillTree(profile);

            Assert(
                currentSkillTree != null
                && currentSkillTree.Id == SkillTreeId.Tlamatini,
                "Current class should resolve its registered skill tree binding.");
        }

        private static void AssertClassSkillTreeBindingSnapshotCapturesBindings()
        {
            var bindingRegistry = BuildTestClassSkillTreeBindingRegistryWithBindings();
            var snapshot = BuildClassSkillTreeBindingSnapshot(bindingRegistry);

            Assert(
                snapshot.ClassSkillTreeBindings.Count == 2
                && FindClassSkillTreeBindingSnapshot(snapshot, ClassId.Tlamatini).SkillTreeId
                    == SkillTreeId.Tlamatini
                && FindClassSkillTreeBindingSnapshot(snapshot, ClassId.Wanderer).SkillTreeId
                    == SkillTreeId.Wanderer,
                "Snapshot should capture class skill tree bindings by stable ids.");
        }

        private static void AssertClassSkillTreeBindingSnapshotRestoreRecreatesBindings()
        {
            var sourceRegistry = BuildTestClassSkillTreeBindingRegistryWithBindings();
            var sourceSnapshot = BuildClassSkillTreeBindingSnapshot(sourceRegistry);
            string json = new V2ProgressSnapshotJsonSerializer().Serialize(sourceSnapshot);
            var restoredSnapshot = new V2ProgressSnapshotJsonSerializer().Deserialize(json);
            var targetRegistry = BuildTestClassSkillTreeBindingRegistry();

            bool restored = new V2ProgressSnapshotService().RestoreClassSkillTreeBindings(
                restoredSnapshot,
                targetRegistry,
                BuildTestClassRegistry());

            Assert(
                restored
                && targetRegistry.ResolveSkillTreeForClass(ClassId.Tlamatini).Id
                    == SkillTreeId.Tlamatini
                && targetRegistry.ResolveSkillTreeForClass(ClassId.Wanderer).Id
                    == SkillTreeId.Wanderer,
                "Snapshot JSON round-trip should recreate class skill tree bindings.");
        }

        private static void AssertClassSkillTreeBindingSnapshotRestoreValidatesClassIds()
        {
            var snapshot = BuildClassSkillTreeBindingSnapshot(
                new V2ClassSkillTreeBindingSnapshot(
                    "class_unknown",
                    SkillTreeId.Tlamatini));
            var targetRegistry = BuildTestClassSkillTreeBindingRegistry();

            bool restored = new V2ProgressSnapshotService().RestoreClassSkillTreeBindings(
                snapshot,
                targetRegistry,
                BuildTestClassRegistry());

            Assert(
                !restored
                && targetRegistry.Bindings.Count == 0,
                "Class skill tree binding restore should reject class ids missing from ClassDefinitionRegistry.");
        }

        private static void AssertClassSkillTreeBindingSnapshotRestoreValidatesSkillTreeIds()
        {
            var snapshot = BuildClassSkillTreeBindingSnapshot(
                new V2ClassSkillTreeBindingSnapshot(
                    ClassId.Tlamatini,
                    "skill_tree_unknown"));
            var targetRegistry = BuildTestClassSkillTreeBindingRegistry();

            bool restored = new V2ProgressSnapshotService().RestoreClassSkillTreeBindings(
                snapshot,
                targetRegistry,
                BuildTestClassRegistry());

            Assert(
                !restored
                && targetRegistry.Bindings.Count == 0,
                "Class skill tree binding restore should reject unregistered skill tree ids.");
        }

        private static void AssertUnknownClassSkillTreeBindingSnapshotIdsDoNotCorruptExistingBindings()
        {
            var classRegistry = BuildTestClassRegistry();
            var targetRegistry = BuildTestClassSkillTreeBindingRegistry();
            targetRegistry.RegisterBinding(
                new ClassSkillTreeBinding(
                    ClassId.Tlamatini,
                    SkillTreeId.Tlamatini),
                classRegistry);
            var originalBinding = targetRegistry.GetBindingForClass(ClassId.Tlamatini);
            var snapshot = BuildClassSkillTreeBindingSnapshot(
                new V2ClassSkillTreeBindingSnapshot(
                    ClassId.Wanderer,
                    SkillTreeId.Wanderer),
                new V2ClassSkillTreeBindingSnapshot(
                    ClassId.Tlamatini,
                    "skill_tree_unknown"));

            bool restored = new V2ProgressSnapshotService().RestoreClassSkillTreeBindings(
                snapshot,
                targetRegistry,
                classRegistry);

            Assert(
                !restored
                && targetRegistry.GetBindingForClass(ClassId.Tlamatini) == originalBinding
                && targetRegistry.GetBindingForClass(ClassId.Wanderer) == null
                && targetRegistry.Bindings.Count == 1,
                "Unknown binding ids should fail atomically without corrupting existing bindings.");
        }

        private static void AssertOlderJsonWithoutClassSkillTreeBindingsRestoresSafely()
        {
            string json = "{"
                + "\"SchemaVersion\":" + V2ProgressSnapshotVersions.CurrentSchemaVersion + ","
                + "\"CodexEntries\":[],"
                + "\"MissionLeads\":[],"
                + "\"ActiveMissions\":[],"
                + "\"Unlocks\":[],"
                + "\"MasteryRecords\":[],"
                + "\"MentorRelationships\":[]"
                + "}";
            var snapshot = new V2ProgressSnapshotJsonSerializer().Deserialize(json);
            var targetRegistry = BuildTestClassSkillTreeBindingRegistry();

            bool restored = new V2ProgressSnapshotService().RestoreClassSkillTreeBindings(
                snapshot,
                targetRegistry,
                BuildTestClassRegistry());

            Assert(
                restored
                && snapshot.ClassSkillTreeBindings.Count == 0
                && targetRegistry.Bindings.Count == 0,
                "Older JSON without class skill tree binding data should restore safely as empty bindings.");
        }

        private static void AssertSkillTreeSnapshotCapturesDiscoveredAndUnlockedNodes()
        {
            var skillTree = BuildTestSkillTreeDefinition();
            var progress = BuildSkillTreeProgressWithUnlockedRoot(skillTree);

            var snapshot = new V2ProgressSnapshotService().CreateSnapshot(
                new CodexProfile(),
                new MissionJournalProfile(),
                new UnlockProfile(),
                new ClassProfile(),
                new MasteryProfile(),
                new MentorRelationshipProfile(),
                progress);

            Assert(
                snapshot.SkillTreeProgress.DiscoveredNodeIds.Count == 2
                && snapshot.SkillTreeProgress.UnlockedNodeIds.Count == 1,
                "Snapshot should capture discovered and unlocked skill tree node ids.");

            Assert(
                ContainsId(snapshot.SkillTreeProgress.DiscoveredNodeIds, "node_deep_sight")
                && ContainsId(snapshot.SkillTreeProgress.DiscoveredNodeIds, "node_balance_ritual")
                && ContainsId(snapshot.SkillTreeProgress.UnlockedNodeIds, "node_deep_sight"),
                "Snapshot should preserve skill tree progress facts by stable node id.");
        }

        private static void AssertSkillTreeSnapshotRestoreRecreatesProgressState()
        {
            var skillTree = BuildTestSkillTreeDefinition();
            var sourceProgress = BuildSkillTreeProgressWithUnlockedRoot(skillTree);
            var service = new V2ProgressSnapshotService();
            var snapshot = service.CreateSnapshot(
                new CodexProfile(),
                new MissionJournalProfile(),
                new UnlockProfile(),
                new ClassProfile(),
                new MasteryProfile(),
                new MentorRelationshipProfile(),
                sourceProgress);
            var restoredProgress = new SkillTreeProgressProfile();

            bool restored = service.RestoreSkillTreeProgressProfile(
                snapshot,
                restoredProgress,
                skillTree);

            Assert(
                restored,
                "Known skill tree progress snapshot should restore successfully.");

            Assert(
                restoredProgress.IsDiscovered("node_deep_sight")
                && restoredProgress.IsUnlocked("node_deep_sight")
                && restoredProgress.IsDiscovered("node_balance_ritual")
                && !restoredProgress.IsUnlocked("node_balance_ritual"),
                "Restore should recreate discovered and unlocked skill tree progress state.");
        }

        private static void AssertSkillTreeSnapshotRestoreValidatesNodeIdsThroughDefinition()
        {
            var snapshot = BuildUnknownSkillTreeProgressSnapshot();
            var profile = new SkillTreeProgressProfile();

            bool restored = new V2ProgressSnapshotService().RestoreSkillTreeProgressProfile(
                snapshot,
                profile,
                BuildTestSkillTreeDefinition());

            Assert(
                !restored,
                "Skill tree progress restore should reject node ids missing from the skill tree definition.");

            Assert(
                profile.DiscoveredNodeIds.Count == 0
                && profile.UnlockedNodeIds.Count == 0,
                "Rejected skill tree progress restore should not mutate profile state.");
        }

        private static void AssertUnknownSkillTreeSnapshotNodeDoesNotCorruptProfile()
        {
            var skillTree = BuildTestSkillTreeDefinition();
            var profile = BuildSkillTreeProgressWithUnlockedRoot(skillTree);
            var snapshot = BuildUnknownSkillTreeProgressSnapshot();

            bool restored = new V2ProgressSnapshotService().RestoreSkillTreeProgressProfile(
                snapshot,
                profile,
                skillTree);

            Assert(
                !restored,
                "Unknown skill tree node ids should fail safely without corrupting existing progress.");

            Assert(
                profile.IsDiscovered("node_deep_sight")
                && profile.IsUnlocked("node_deep_sight")
                && !profile.IsDiscovered("node_unknown"),
                "Unknown skill tree node ids should preserve existing progress state.");
        }

        private static void AssertOlderJsonWithoutSkillTreeProgressRestoresSafely()
        {
            string json = "{"
                + "\"SchemaVersion\":" + V2ProgressSnapshotVersions.CurrentSchemaVersion + ","
                + "\"CodexEntries\":[],"
                + "\"MissionLeads\":[],"
                + "\"ActiveMissions\":[],"
                + "\"Unlocks\":[],"
                + "\"MasteryRecords\":[],"
                + "\"MentorRelationships\":[]"
                + "}";
            var snapshot = new V2ProgressSnapshotJsonSerializer().Deserialize(json);
            var profile = new SkillTreeProgressProfile();

            bool restored = new V2ProgressSnapshotService().RestoreSkillTreeProgressProfile(
                snapshot,
                profile,
                BuildTestSkillTreeDefinition());

            Assert(
                restored,
                "Older JSON without skill tree progress data should restore safely.");

            Assert(
                snapshot.SkillTreeProgress.DiscoveredNodeIds.Count == 0
                && snapshot.SkillTreeProgress.UnlockedNodeIds.Count == 0
                && profile.DiscoveredNodeIds.Count == 0
                && profile.UnlockedNodeIds.Count == 0,
                "Missing skill tree progress data should restore as empty progress state.");
        }

        private static void AssertV4RestoreCoordinatorRestoresAllProfiles()
        {
            var snapshot = BuildCompleteV4Snapshot(
                ClassId.Tlamatini,
                "skill_deep_sight",
                MentorId.Lyra,
                "node_deep_sight");
            var classProfile = new ClassProfile();
            var masteryProfile = new MasteryProfile();
            var mentorProfile = new MentorRelationshipProfile();
            var skillTreeProgress = new SkillTreeProgressProfile();

            var result = new V4ProgressRestoreCoordinator().Restore(
                snapshot,
                classProfile,
                masteryProfile,
                mentorProfile,
                skillTreeProgress,
                BuildTestClassRegistry(),
                BuildTestSkillRegistry(),
                BuildTestMentorRegistry(),
                BuildTestSkillTreeDefinition());

            Assert(
                result.FullyRestored
                && result.ClassRestored
                && result.MasteryRestored
                && result.MentorRelationshipsRestored
                && result.SkillTreeProgressRestored,
                "V4 restore coordinator should report all subsystem restore success.");

            Assert(
                classProfile.CurrentClassId == ClassId.Tlamatini
                && masteryProfile.GetRecord("skill_deep_sight").Progress == 25
                && mentorProfile.IsDiscovered(MentorId.Lyra)
                && skillTreeProgress.IsUnlocked("node_deep_sight"),
                "V4 restore coordinator should restore class, mastery, mentor, and skill tree state.");
        }

        private static void AssertV4RestoreCoordinatorSuccessHasNoFailureReasons()
        {
            var result = RestoreCompleteV4Snapshot(
                BuildCompleteV4Snapshot(
                    ClassId.Tlamatini,
                    "skill_deep_sight",
                    MentorId.Lyra,
                    "node_deep_sight"));

            Assert(
                result.FullyRestored
                && result.ClassFailureReason == V4ProgressRestoreFailureReason.None
                && result.MasteryFailureReason == V4ProgressRestoreFailureReason.None
                && result.MentorRelationshipsFailureReason == V4ProgressRestoreFailureReason.None
                && result.SkillTreeProgressFailureReason == V4ProgressRestoreFailureReason.None,
                "Successful V4 restore should have no failure reasons.");

            Assert(
                result.ClassFailureMessage == string.Empty
                && result.MasteryFailureMessage == string.Empty
                && result.MentorRelationshipsFailureMessage == string.Empty
                && result.SkillTreeProgressFailureMessage == string.Empty,
                "Successful V4 restore should have no failure messages.");
        }

        private static void AssertV4RestoreCoordinatorReportsClassFailure()
        {
            var result = RestoreCompleteV4Snapshot(
                BuildCompleteV4Snapshot(
                    "class_unknown",
                    "skill_deep_sight",
                    MentorId.Lyra,
                    "node_deep_sight"));

            Assert(
                !result.ClassRestored
                && result.MasteryRestored
                && result.MentorRelationshipsRestored
                && result.SkillTreeProgressRestored,
                "V4 restore coordinator should report class restore failure separately.");

            Assert(
                result.ClassFailureReason == V4ProgressRestoreFailureReason.ValidationFailed
                && result.ClassFailureMessage.Contains("ClassProfile"),
                "V4 restore coordinator should expose class restore failure details.");
        }

        private static void AssertV4RestoreCoordinatorReportsMasteryFailure()
        {
            var result = RestoreCompleteV4Snapshot(
                BuildCompleteV4Snapshot(
                    ClassId.Tlamatini,
                    "skill_unknown",
                    MentorId.Lyra,
                    "node_deep_sight"));

            Assert(
                result.ClassRestored
                && !result.MasteryRestored
                && result.MentorRelationshipsRestored
                && result.SkillTreeProgressRestored,
                "V4 restore coordinator should report mastery restore failure separately.");

            Assert(
                result.MasteryFailureReason == V4ProgressRestoreFailureReason.ValidationFailed
                && result.MasteryFailureMessage.Contains("MasteryProfile"),
                "V4 restore coordinator should expose mastery restore failure details.");
        }

        private static void AssertV4RestoreCoordinatorReportsMentorFailure()
        {
            var result = RestoreCompleteV4Snapshot(
                BuildCompleteV4Snapshot(
                    ClassId.Tlamatini,
                    "skill_deep_sight",
                    "mentor_unknown",
                    "node_deep_sight"));

            Assert(
                result.ClassRestored
                && result.MasteryRestored
                && !result.MentorRelationshipsRestored
                && result.SkillTreeProgressRestored,
                "V4 restore coordinator should report mentor relationship restore failure separately.");

            Assert(
                result.MentorRelationshipsFailureReason == V4ProgressRestoreFailureReason.ValidationFailed
                && result.MentorRelationshipsFailureMessage.Contains("MentorRelationshipProfile"),
                "V4 restore coordinator should expose mentor relationship restore failure details.");
        }

        private static void AssertV4RestoreCoordinatorReportsSkillTreeFailure()
        {
            var result = RestoreCompleteV4Snapshot(
                BuildCompleteV4Snapshot(
                    ClassId.Tlamatini,
                    "skill_deep_sight",
                    MentorId.Lyra,
                    "node_unknown"));

            Assert(
                result.ClassRestored
                && result.MasteryRestored
                && result.MentorRelationshipsRestored
                && !result.SkillTreeProgressRestored,
                "V4 restore coordinator should report skill tree progress restore failure separately.");

            Assert(
                result.SkillTreeProgressFailureReason == V4ProgressRestoreFailureReason.ValidationFailed
                && result.SkillTreeProgressFailureMessage.Contains("SkillTreeProgressProfile"),
                "V4 restore coordinator should expose skill tree progress restore failure details.");
        }

        private static void AssertV4RestoreCoordinatorPartialFailureIsNotFullSuccess()
        {
            var result = RestoreCompleteV4Snapshot(
                BuildCompleteV4Snapshot(
                    "class_unknown",
                    "skill_deep_sight",
                    MentorId.Lyra,
                    "node_unknown"));

            Assert(
                !result.FullyRestored
                && !result.ClassRestored
                && !result.SkillTreeProgressRestored,
                "V4 restore coordinator partial failure should not look like full success.");

            Assert(
                result.ClassFailureReason != V4ProgressRestoreFailureReason.None
                && result.SkillTreeProgressFailureReason != V4ProgressRestoreFailureReason.None
                && result.ClassFailureMessage.Length > 0
                && result.SkillTreeProgressFailureMessage.Length > 0,
                "V4 restore coordinator partial failure should expose enough facts for UI or debug explanations.");
        }

        private static void AssertV4RestoreCoordinatorRestoresClassSkillTreeBindings()
        {
            var snapshot = BuildCompleteV4SnapshotWithBinding(
                ClassId.Tlamatini,
                SkillTreeId.Tlamatini);
            var bindingRegistry = BuildTestClassSkillTreeBindingRegistry();

            var result = RestoreCompleteV4SnapshotWithBindings(
                snapshot,
                bindingRegistry);

            Assert(
                result.FullyRestored
                && result.ClassSkillTreeBindingsRestored
                && bindingRegistry.GetBindingForClass(ClassId.Tlamatini).SkillTreeId
                    == SkillTreeId.Tlamatini,
                "V4 restore coordinator should restore class skill tree bindings successfully.");

            Assert(
                result.ClassSkillTreeBindingsFailureReason == V4ProgressRestoreFailureReason.None
                && result.ClassSkillTreeBindingsFailureMessage == string.Empty,
                "Successful binding restore should expose no binding failure details.");
        }

        private static void AssertV4RestoreCoordinatorReportsClassSkillTreeBindingFailure()
        {
            var result = RestoreCompleteV4SnapshotWithBindings(
                BuildCompleteV4SnapshotWithBinding(
                    ClassId.Tlamatini,
                    "skill_tree_unknown"),
                BuildTestClassSkillTreeBindingRegistry());

            Assert(
                result.ClassRestored
                && result.MasteryRestored
                && result.MentorRelationshipsRestored
                && result.SkillTreeProgressRestored
                && !result.ClassSkillTreeBindingsRestored,
                "V4 restore coordinator should report binding restore failure separately.");

            Assert(
                result.ClassSkillTreeBindingsFailureReason
                    == V4ProgressRestoreFailureReason.ValidationFailed
                && result.ClassSkillTreeBindingsFailureMessage.Contains("ClassSkillTreeBindings"),
                "V4 restore coordinator should expose binding restore failure details.");
        }

        private static void AssertV4RestoreCoordinatorBindingFailureIsNotFullSuccess()
        {
            var result = RestoreCompleteV4SnapshotWithBindings(
                BuildCompleteV4SnapshotWithBinding(
                    "class_unknown",
                    SkillTreeId.Tlamatini),
                BuildTestClassSkillTreeBindingRegistry());

            Assert(
                !result.FullyRestored
                && !result.ClassSkillTreeBindingsRestored,
                "Binding restore failure should prevent V4 coordinator full success.");
        }

        private static void AssertV4RestoreCoordinatorRestoredBindingResolvesCurrentClassSkillTree()
        {
            var snapshot = BuildCompleteV4SnapshotWithBinding(
                ClassId.Tlamatini,
                SkillTreeId.Tlamatini);
            var classProfile = new ClassProfile();
            var bindingRegistry = BuildTestClassSkillTreeBindingRegistry();

            var result = new V4ProgressRestoreCoordinator().Restore(
                snapshot,
                classProfile,
                new MasteryProfile(),
                new MentorRelationshipProfile(),
                new SkillTreeProgressProfile(),
                BuildTestClassRegistry(),
                BuildTestSkillRegistry(),
                BuildTestMentorRegistry(),
                BuildTestSkillTreeDefinition(),
                bindingRegistry);
            var currentSkillTree = bindingRegistry.ResolveCurrentClassSkillTree(classProfile);

            Assert(
                result.FullyRestored
                && currentSkillTree != null
                && currentSkillTree.Id == SkillTreeId.Tlamatini,
                "Successful coordinated restore should resolve the current class skill tree binding.");
        }

        private static void AssertV4RestoreCoordinatorClassPreflightPreventsAllMutation()
        {
            AssertV4PreflightFailurePreventsProfileMutation(
                BuildCompleteV4Snapshot(
                    "class_unknown",
                    "skill_deep_sight",
                    MentorId.Lyra,
                    "node_deep_sight"),
                delegate(V4ProgressRestoreResult result)
                {
                    return !result.ClassRestored;
                },
                "Class preflight failure should prevent all V4 profile mutation.");
        }

        private static void AssertV4RestoreCoordinatorMasteryPreflightPreventsAllMutation()
        {
            AssertV4PreflightFailurePreventsProfileMutation(
                BuildCompleteV4Snapshot(
                    ClassId.Tlamatini,
                    "skill_unknown",
                    MentorId.Lyra,
                    "node_deep_sight"),
                delegate(V4ProgressRestoreResult result)
                {
                    return !result.MasteryRestored;
                },
                "Mastery preflight failure should prevent all V4 profile mutation.");
        }

        private static void AssertV4RestoreCoordinatorMentorPreflightPreventsAllMutation()
        {
            AssertV4PreflightFailurePreventsProfileMutation(
                BuildCompleteV4Snapshot(
                    ClassId.Tlamatini,
                    "skill_deep_sight",
                    "mentor_unknown",
                    "node_deep_sight"),
                delegate(V4ProgressRestoreResult result)
                {
                    return !result.MentorRelationshipsRestored;
                },
                "Mentor preflight failure should prevent all V4 profile mutation.");
        }

        private static void AssertV4RestoreCoordinatorSkillTreePreflightPreventsAllMutation()
        {
            AssertV4PreflightFailurePreventsProfileMutation(
                BuildCompleteV4Snapshot(
                    ClassId.Tlamatini,
                    "skill_deep_sight",
                    MentorId.Lyra,
                    "node_unknown"),
                delegate(V4ProgressRestoreResult result)
                {
                    return !result.SkillTreeProgressRestored;
                },
                "Skill tree preflight failure should prevent all V4 profile mutation.");
        }

        private static void AssertV4RestoreCoordinatorBindingPreflightPreventsAllMutation()
        {
            var snapshot = BuildCompleteV4SnapshotWithBinding(
                ClassId.Tlamatini,
                "skill_tree_unknown");
            var classProfile = new ClassProfile();
            var masteryProfile = new MasteryProfile();
            var mentorProfile = new MentorRelationshipProfile();
            var skillTreeProgress = new SkillTreeProgressProfile();
            var bindingRegistry = BuildTestClassSkillTreeBindingRegistry();

            var result = new V4ProgressRestoreCoordinator().Restore(
                snapshot,
                classProfile,
                masteryProfile,
                mentorProfile,
                skillTreeProgress,
                BuildTestClassRegistry(),
                BuildTestSkillRegistry(),
                BuildTestMentorRegistry(),
                BuildTestSkillTreeDefinition(),
                bindingRegistry);

            Assert(
                !result.ClassSkillTreeBindingsRestored
                && HasNoRestoredV4State(
                    classProfile,
                    masteryProfile,
                    mentorProfile,
                    skillTreeProgress)
                && bindingRegistry.Bindings.Count == 0,
                "Binding preflight failure should prevent all profile and binding mutation.");
        }

        private static void AssertV4RestoreCoordinatorPreflightPreservesFailureDetails()
        {
            var result = RestoreCompleteV4Snapshot(
                BuildCompleteV4Snapshot(
                    ClassId.Tlamatini,
                    "skill_unknown",
                    MentorId.Lyra,
                    "node_deep_sight"));

            Assert(
                result.MasteryFailureReason
                    == V4ProgressRestoreFailureReason.ValidationFailed
                && result.MasteryFailureMessage.Contains("MasteryProfile")
                && result.ClassFailureReason == V4ProgressRestoreFailureReason.None
                && result.MentorRelationshipsFailureReason
                    == V4ProgressRestoreFailureReason.None,
                "Preflight failure should preserve per-subsystem result details.");
        }

        private static void AssertV4PreflightFailurePreventsProfileMutation(
            V2ProgressSnapshot snapshot,
            System.Func<V4ProgressRestoreResult, bool> failedSubsystemAssertion,
            string message)
        {
            var classProfile = new ClassProfile();
            var masteryProfile = new MasteryProfile();
            var mentorProfile = new MentorRelationshipProfile();
            var skillTreeProgress = new SkillTreeProgressProfile();

            var result = new V4ProgressRestoreCoordinator().Restore(
                snapshot,
                classProfile,
                masteryProfile,
                mentorProfile,
                skillTreeProgress,
                BuildTestClassRegistry(),
                BuildTestSkillRegistry(),
                BuildTestMentorRegistry(),
                BuildTestSkillTreeDefinition());

            Assert(
                failedSubsystemAssertion(result)
                && HasNoRestoredV4State(
                    classProfile,
                    masteryProfile,
                    mentorProfile,
                    skillTreeProgress),
                message);
        }

        private static bool HasNoRestoredV4State(
            ClassProfile classProfile,
            MasteryProfile masteryProfile,
            MentorRelationshipProfile mentorProfile,
            SkillTreeProgressProfile skillTreeProgress)
        {
            return string.IsNullOrWhiteSpace(classProfile.CurrentClassId)
                && masteryProfile.Records.Count == 0
                && mentorProfile.DiscoveredMentorIds.Count == 0
                && skillTreeProgress.DiscoveredNodeIds.Count == 0
                && skillTreeProgress.UnlockedNodeIds.Count == 0;
        }

        private static void AssertMentorCanBeDiscovered()
        {
            var profile = new MentorRelationshipProfile();
            var mentor = BuildLyraMentor(true);

            bool discovered = profile.Discover(mentor);

            Assert(
                discovered,
                "Known mentor should be discoverable.");

            Assert(
                profile.IsDiscovered(MentorId.Lyra)
                && profile.IsAvailable(MentorId.Lyra)
                && profile.GetTrust(MentorId.Lyra) == 0,
                "Discovered mentor should expose relationship facts without granting rewards.");
        }

        private static void AssertDuplicateMentorDiscoveryIsIdempotent()
        {
            var profile = new MentorRelationshipProfile();
            var mentor = BuildLyraMentor(true);

            bool firstDiscovery = profile.Discover(mentor);
            bool duplicateDiscovery = profile.Discover(mentor);

            Assert(
                firstDiscovery && !duplicateDiscovery,
                "Duplicate mentor discovery should be idempotent.");

            Assert(
                profile.DiscoveredMentorIds.Count == 1
                && profile.GetTrust(MentorId.Lyra) == 0,
                "Duplicate mentor discovery should not duplicate records or inflate trust.");
        }

        private static void AssertMentorTrustCanIncrease()
        {
            var profile = new MentorRelationshipProfile();
            var mentor = BuildLyraMentor(true);

            profile.Discover(mentor);
            bool increased = profile.IncreaseTrust(mentor, 15);

            Assert(
                increased,
                "Discovered mentor trust should increase.");

            Assert(
                profile.GetTrust(MentorId.Lyra) == 15,
                "Mentor relationship profile should track trust progress.");
        }

        private static void AssertUnavailableMentorIsReportedAsUnavailable()
        {
            var profile = new MentorRelationshipProfile();
            var mentor = BuildAtlasMentor(false);

            bool discovered = profile.Discover(mentor);

            Assert(
                discovered,
                "Unavailable mentor can still be discovered as a relationship fact.");

            Assert(
                profile.IsDiscovered(MentorId.Atlas)
                && !profile.IsAvailable(MentorId.Atlas),
                "Unavailable mentor should be reported as unavailable.");
        }

        private static void AssertUnknownOrNullMentorIsRejectedSafely()
        {
            var profile = new MentorRelationshipProfile();
            var invalidMentor = new MentorDefinition(
                "",
                "Invalid",
                true);

            bool nullDiscovered = profile.Discover(null);
            bool invalidDiscovered = profile.Discover(invalidMentor);
            bool trustIncreased = profile.IncreaseTrust(
                BuildLyraMentor(true),
                10);

            Assert(
                !nullDiscovered && !invalidDiscovered && !trustIncreased,
                "Null, invalid, or undiscovered mentors should be rejected safely.");

            Assert(
                profile.DiscoveredMentorIds.Count == 0
                && !profile.IsDiscovered(MentorId.Lyra)
                && !profile.IsAvailable(MentorId.Lyra),
                "Rejected mentor operations should not mutate relationship state.");
        }

        private static void AssertMentorDefinitionCanBeRegisteredAndResolved()
        {
            var registry = new MentorDefinitionRegistry();
            var mentor = BuildLyraMentor(true);

            bool registered = registry.Register(mentor);
            var resolved = registry.Get(MentorId.Lyra);

            Assert(
                registered && registry.Contains(MentorId.Lyra),
                "Mentor definition registry should accept valid mentors by stable id.");

            Assert(
                resolved == mentor && resolved.Name == "Lyra",
                "Mentor definition registry should resolve registered mentor definitions.");
        }

        private static void AssertDuplicateMentorIdIsRejectedSafely()
        {
            var registry = new MentorDefinitionRegistry();
            var firstMentor = BuildLyraMentor(true);
            var duplicateMentor = new MentorDefinition(
                MentorId.Lyra,
                "Duplicate Lyra",
                false);

            bool firstRegistered = registry.Register(firstMentor);
            bool duplicateRegistered = registry.Register(duplicateMentor);

            Assert(
                firstRegistered && !duplicateRegistered,
                "Duplicate mentor ids should be rejected safely.");

            Assert(
                registry.Get(MentorId.Lyra) == firstMentor,
                "Duplicate mentor ids should not replace the original definition.");
        }

        private static void AssertMentorDiscoveryValidatesThroughRegistry()
        {
            var profile = new MentorRelationshipProfile();
            var registry = BuildTestMentorRegistry();

            bool discovered = profile.Discover(MentorId.Lyra, registry);

            Assert(
                discovered,
                "Mentor discovery should validate through the mentor registry.");

            Assert(
                profile.IsDiscovered(MentorId.Lyra)
                && profile.IsAvailable(MentorId.Lyra),
                "Registry-backed mentor discovery should apply registered mentor facts.");
        }

        private static void AssertMentorTrustIncreaseValidatesThroughRegistry()
        {
            var profile = new MentorRelationshipProfile();
            var registry = BuildTestMentorRegistry();

            profile.Discover(MentorId.Lyra, registry);
            bool increased = profile.IncreaseTrust(
                MentorId.Lyra,
                20,
                registry);

            Assert(
                increased,
                "Mentor trust increase should validate through the mentor registry.");

            Assert(
                profile.GetTrust(MentorId.Lyra) == 20,
                "Registry-backed mentor trust increase should track trust progress.");
        }

        private static void AssertUnknownMentorIdFailsSafelyThroughRegistry()
        {
            var profile = new MentorRelationshipProfile();
            var registry = BuildTestMentorRegistry();

            bool discovered = profile.Discover("mentor_unknown", registry);
            bool increased = profile.IncreaseTrust(
                "mentor_unknown",
                10,
                registry);

            Assert(
                !discovered && !increased,
                "Unknown mentor ids should fail safely through the mentor registry.");

            Assert(
                profile.DiscoveredMentorIds.Count == 0
                && registry.Get("mentor_unknown") == null,
                "Unknown mentor ids should not create relationship state.");
        }

        private static void AssertMentorRelationshipSnapshotCapturesRecords()
        {
            var relationshipProfile = BuildMentorProfileWithRelationships();

            var snapshot = new V2ProgressSnapshotService().CreateSnapshot(
                new CodexProfile(),
                new MissionJournalProfile(),
                new UnlockProfile(),
                new ClassProfile(),
                new MasteryProfile(),
                relationshipProfile);

            Assert(
                snapshot.MentorRelationships.Count == 2,
                "Snapshot should capture discovered mentor relationship records.");

            var lyraSnapshot = FindMentorRelationshipSnapshot(snapshot, MentorId.Lyra);

            Assert(
                lyraSnapshot != null
                && lyraSnapshot.IsDiscovered
                && lyraSnapshot.Trust == 25
                && lyraSnapshot.IsAvailable,
                "Snapshot should capture mentor discovery, trust, and availability facts.");
        }

        private static void AssertMentorRelationshipSnapshotRestoreRecreatesState()
        {
            var sourceProfile = BuildMentorProfileWithRelationships();
            var service = new V2ProgressSnapshotService();
            var snapshot = service.CreateSnapshot(
                new CodexProfile(),
                new MissionJournalProfile(),
                new UnlockProfile(),
                new ClassProfile(),
                new MasteryProfile(),
                sourceProfile);
            var restoredProfile = new MentorRelationshipProfile();

            bool restored = service.RestoreMentorRelationshipProfile(
                snapshot,
                restoredProfile,
                BuildTestMentorRegistry());

            Assert(
                restored,
                "Known mentor relationship snapshot should restore successfully.");

            Assert(
                restoredProfile.IsDiscovered(MentorId.Lyra)
                && restoredProfile.GetTrust(MentorId.Lyra) == 25
                && restoredProfile.IsAvailable(MentorId.Lyra)
                && restoredProfile.IsDiscovered(MentorId.Atlas)
                && restoredProfile.GetTrust(MentorId.Atlas) == 3
                && !restoredProfile.IsAvailable(MentorId.Atlas),
                "Restore should recreate mentor discovery, trust, and availability state.");
        }

        private static void AssertMentorRelationshipSnapshotRestoreValidatesMentorIdsThroughRegistry()
        {
            var snapshot = BuildUnknownMentorRelationshipSnapshot();
            var profile = new MentorRelationshipProfile();

            bool restored = new V2ProgressSnapshotService().RestoreMentorRelationshipProfile(
                snapshot,
                profile,
                BuildTestMentorRegistry());

            Assert(
                !restored,
                "Mentor relationship restore should reject ids missing from the mentor registry.");

            Assert(
                profile.DiscoveredMentorIds.Count == 0,
                "Rejected mentor relationship restore should not mutate profile state.");
        }

        private static void AssertUnknownMentorRelationshipSnapshotDoesNotCorruptProfile()
        {
            var snapshot = BuildUnknownMentorRelationshipSnapshot();
            var profile = BuildTrustedLyraRelationship(10);

            bool restored = new V2ProgressSnapshotService().RestoreMentorRelationshipProfile(
                snapshot,
                profile,
                BuildTestMentorRegistry());

            Assert(
                !restored,
                "Unknown mentor relationship snapshot ids should fail safely without corrupting existing state.");

            Assert(
                profile.IsDiscovered(MentorId.Lyra)
                && profile.GetTrust(MentorId.Lyra) == 10
                && !profile.IsDiscovered("mentor_unknown"),
                "Unknown mentor relationship snapshot ids should preserve existing mentor relationships.");
        }

        private static void AssertOlderJsonWithoutMentorRelationshipDataRestoresSafely()
        {
            string json = "{"
                + "\"SchemaVersion\":" + V2ProgressSnapshotVersions.CurrentSchemaVersion + ","
                + "\"CodexEntries\":[],"
                + "\"MissionLeads\":[],"
                + "\"ActiveMissions\":[],"
                + "\"Unlocks\":[],"
                + "\"MasteryRecords\":[]"
                + "}";
            var snapshot = new V2ProgressSnapshotJsonSerializer().Deserialize(json);
            var profile = new MentorRelationshipProfile();

            bool restored = new V2ProgressSnapshotService().RestoreMentorRelationshipProfile(
                snapshot,
                profile,
                BuildTestMentorRegistry());

            Assert(
                restored,
                "Older JSON without mentor relationship data should restore safely.");

            Assert(
                snapshot.MentorRelationships.Count == 0
                && profile.DiscoveredMentorIds.Count == 0,
                "Missing mentor relationship data should restore as empty mentor relationship state.");
        }

        private static void AssertMentorTeachingAvailableWhenTrustRequirementIsMet()
        {
            var relationshipProfile = BuildTrustedLyraRelationship(25);
            var teaching = BuildLyraDeepSightTeaching(20, 5);

            var result = new MentorTeachingService().Evaluate(
                teaching,
                relationshipProfile,
                BuildTestMentorRegistry(),
                BuildTestSkillRegistry());

            Assert(
                result.IsAvailable,
                "Mentor teaching should be available when mentor is discovered and trust requirement is met.");

            Assert(
                result.CurrentTrust == 25
                && result.RequiredTrust == 20
                && result.BlockReason == MentorTeachingBlockReason.None,
                "Available mentor teaching should expose trust facts and no block reason.");
        }

        private static void AssertMentorTeachingBlockedWhenMentorIsUnknown()
        {
            var relationshipProfile = BuildTrustedLyraRelationship(25);
            var teaching = new MentorTeachingDefinition(
                "mentor_unknown",
                "skill_deep_sight",
                10,
                5);

            var result = new MentorTeachingService().Evaluate(
                teaching,
                relationshipProfile,
                BuildTestMentorRegistry(),
                BuildTestSkillRegistry());

            Assert(
                !result.IsAvailable,
                "Mentor teaching should be blocked when mentor id is unknown.");

            Assert(
                result.BlockReason == MentorTeachingBlockReason.UnknownMentor,
                "Unknown mentor teaching block should report an unknown mentor reason.");
        }

        private static void AssertMentorTeachingBlockedWhenSkillIsUnknown()
        {
            var relationshipProfile = BuildTrustedLyraRelationship(25);
            var teaching = new MentorTeachingDefinition(
                MentorId.Lyra,
                "skill_unknown",
                10,
                5);

            var result = new MentorTeachingService().Evaluate(
                teaching,
                relationshipProfile,
                BuildTestMentorRegistry(),
                BuildTestSkillRegistry());

            Assert(
                !result.IsAvailable,
                "Mentor teaching should be blocked when taught skill id is unknown.");

            Assert(
                result.BlockReason == MentorTeachingBlockReason.UnknownSkill,
                "Unknown skill teaching block should report an unknown skill reason.");
        }

        private static void AssertMentorTeachingBlockedWhenMentorIsNotDiscovered()
        {
            var relationshipProfile = new MentorRelationshipProfile();
            var teaching = BuildLyraDeepSightTeaching(0, 5);

            var result = new MentorTeachingService().Evaluate(
                teaching,
                relationshipProfile,
                BuildTestMentorRegistry(),
                BuildTestSkillRegistry());

            Assert(
                !result.IsAvailable,
                "Mentor teaching should be blocked when mentor is registered but not discovered.");

            Assert(
                result.BlockReason == MentorTeachingBlockReason.MentorNotDiscovered,
                "Undiscovered mentor teaching block should report a mentor-not-discovered reason.");
        }

        private static void AssertMentorTeachingBlockedWhenTrustIsInsufficient()
        {
            var relationshipProfile = BuildTrustedLyraRelationship(5);
            var teaching = BuildLyraDeepSightTeaching(20, 5);

            var result = new MentorTeachingService().Evaluate(
                teaching,
                relationshipProfile,
                BuildTestMentorRegistry(),
                BuildTestSkillRegistry());

            Assert(
                !result.IsAvailable,
                "Mentor teaching should be blocked when trust is insufficient.");

            Assert(
                result.BlockReason == MentorTeachingBlockReason.TrustInsufficient
                && result.CurrentTrust == 5,
                "Insufficient trust block should expose current trust facts.");
        }

        private static void AssertMentorTeachingResultExposesSkillAndMasteryFacts()
        {
            var relationshipProfile = BuildTrustedLyraRelationship(25);
            var teaching = BuildLyraDeepSightTeaching(20, 7);

            var result = new MentorTeachingService().Evaluate(
                teaching,
                relationshipProfile,
                BuildTestMentorRegistry(),
                BuildTestSkillRegistry());

            Assert(
                result.TaughtSkillId == "skill_deep_sight"
                && result.MentorId == MentorId.Lyra,
                "Mentor teaching result should expose mentor and taught skill facts.");

            Assert(
                result.MasteryProgressGranted == 7,
                "Mentor teaching result should expose mastery progress facts without mutating mastery.");
        }

        private static void AssertAvailableMentorTeachingAppliesMasteryProgress()
        {
            var masteryProfile = new MasteryProfile();
            var skillRegistry = BuildTestSkillRegistry();
            var teachingResult = BuildAvailableLyraDeepSightTeachingResult(12);

            var applicationResult = new MentorTeachingApplicationService().Apply(
                teachingResult,
                masteryProfile,
                skillRegistry);

            Assert(
                applicationResult.Applied
                && applicationResult.ProgressApplied == 12
                && applicationResult.BlockReason == MentorTeachingApplicationBlockReason.None,
                "Available mentor teaching should apply mastery progress.");

            Assert(
                masteryProfile.GetRecord("skill_deep_sight").Progress == 12,
                "Mentor teaching application should mutate only mastery progress.");
        }

        private static void AssertZeroProgressMentorTeachingIsNotApplied()
        {
            var masteryProfile = new MasteryProfile();
            var skillRegistry = BuildTestSkillRegistry();
            var teachingResult = BuildAvailableLyraDeepSightTeachingResult(0);

            var applicationResult = new MentorTeachingApplicationService().Apply(
                teachingResult,
                masteryProfile,
                skillRegistry);

            Assert(
                !applicationResult.Applied
                && applicationResult.ProgressApplied == 0
                && applicationResult.BlockReason == MentorTeachingApplicationBlockReason.NoProgressGranted,
                "Zero-progress mentor teaching should not be treated as applied.");

            Assert(
                masteryProfile.Records.Count == 0,
                "Zero-progress mentor teaching should not mutate mastery state.");
        }

        private static void AssertBlockedMentorTeachingDoesNotMutateMastery()
        {
            var masteryProfile = new MasteryProfile();
            var blockedTeaching = new MentorTeachingService().Evaluate(
                BuildLyraDeepSightTeaching(20, 10),
                BuildTrustedLyraRelationship(5),
                BuildTestMentorRegistry(),
                BuildTestSkillRegistry());

            var applicationResult = new MentorTeachingApplicationService().Apply(
                blockedTeaching,
                masteryProfile,
                BuildTestSkillRegistry());

            Assert(
                !applicationResult.Applied
                && applicationResult.BlockReason == MentorTeachingApplicationBlockReason.TeachingBlocked,
                "Blocked mentor teaching should not apply mastery progress.");

            Assert(
                masteryProfile.Records.Count == 0,
                "Blocked mentor teaching should not mutate mastery state.");
        }

        private static void AssertMentorTeachingApplicationUnknownSkillFailsSafely()
        {
            var masteryProfile = new MasteryProfile();
            var teachingResult = new MentorTeachingResult(
                true,
                MentorId.Lyra,
                "skill_unknown",
                0,
                10,
                10,
                MentorTeachingBlockReason.None,
                "");

            var applicationResult = new MentorTeachingApplicationService().Apply(
                teachingResult,
                masteryProfile,
                BuildTestSkillRegistry());

            Assert(
                !applicationResult.Applied
                && applicationResult.BlockReason == MentorTeachingApplicationBlockReason.UnknownSkill,
                "Mentor teaching application should reject unknown skill ids safely.");

            Assert(
                masteryProfile.Records.Count == 0,
                "Unknown skill mentor teaching application should not mutate mastery state.");
        }

        private static void AssertMentorTeachingApplicationCanMasterSkill()
        {
            var masteryProfile = new MasteryProfile();
            var skillRegistry = BuildTestSkillRegistry();
            var teachingResult = BuildAvailableLyraDeepSightTeachingResult(MasteryRecord.MasteredProgress);

            var applicationResult = new MentorTeachingApplicationService().Apply(
                teachingResult,
                masteryProfile,
                skillRegistry);

            Assert(
                applicationResult.Applied
                && applicationResult.ProgressApplied == MasteryRecord.MasteredProgress,
                "Mentor teaching application should apply enough progress to master a skill.");

            Assert(
                masteryProfile.IsMastered("skill_deep_sight"),
                "Applied mentor teaching progress should be able to master a skill.");
        }

        private static void AssertDuplicateMentorTeachingApplicationAfterMasteryIsSafe()
        {
            var masteryProfile = new MasteryProfile();
            var skillRegistry = BuildTestSkillRegistry();
            var teachingResult = BuildAvailableLyraDeepSightTeachingResult(MasteryRecord.MasteredProgress);
            var service = new MentorTeachingApplicationService();

            var firstApplication = service.Apply(
                teachingResult,
                masteryProfile,
                skillRegistry);
            var duplicateApplication = service.Apply(
                teachingResult,
                masteryProfile,
                skillRegistry);

            Assert(
                firstApplication.Applied
                && !duplicateApplication.Applied
                && duplicateApplication.WasAlreadyMastered,
                "Duplicate mentor teaching application after mastery should be safe.");

            Assert(
                duplicateApplication.BlockReason == MentorTeachingApplicationBlockReason.AlreadyMastered
                && masteryProfile.GetRecord("skill_deep_sight").Progress == MasteryRecord.MasteredProgress,
                "Duplicate mentor teaching application should not exceed mastered progress.");
        }

        private static void AssertClassMentorTeachingMasteryWorkflow()
        {
            var skillRegistry = BuildTestSkillRegistry();
            var classRegistry = BuildTestClassRegistry();
            var mentorRegistry = BuildTestMentorRegistry();
            var classProfile = new ClassProfile();
            var mentorProfile = new MentorRelationshipProfile();
            var masteryProfile = new MasteryProfile();
            var teachingDefinition = BuildLyraDeepSightTeaching(20, 15);
            var teachingService = new MentorTeachingService();
            var applicationService = new MentorTeachingApplicationService();

            Assert(
                skillRegistry.Contains("skill_deep_sight"),
                "Integrated workflow should register the taught skill.");

            Assert(
                classRegistry.Contains(ClassId.Tlamatini)
                && classRegistry.Get(ClassId.Tlamatini).AllowsSkill("skill_deep_sight", skillRegistry),
                "Integrated workflow should register a class that resolves the taught skill.");

            Assert(
                classProfile.AssignClassId(ClassId.Tlamatini, classRegistry)
                && classProfile.CurrentClassAllowsSkill("skill_deep_sight", classRegistry, skillRegistry),
                "Integrated workflow should assign a class that allows the taught skill.");

            Assert(
                mentorRegistry.Contains(MentorId.Lyra),
                "Integrated workflow should register the mentor definition.");

            Assert(
                mentorProfile.Discover(MentorId.Lyra, mentorRegistry)
                && mentorProfile.IsDiscovered(MentorId.Lyra),
                "Integrated workflow should discover the mentor through the registry.");

            Assert(
                mentorProfile.IncreaseTrust(MentorId.Lyra, 25, mentorRegistry)
                && mentorProfile.GetTrust(MentorId.Lyra) == 25,
                "Integrated workflow should increase mentor trust through the registry.");

            var teachingResult = teachingService.Evaluate(
                teachingDefinition,
                mentorProfile,
                mentorRegistry,
                skillRegistry);

            Assert(
                teachingResult.IsAvailable
                && teachingResult.TaughtSkillId == "skill_deep_sight"
                && teachingResult.MasteryProgressGranted == 15,
                "Integrated workflow should resolve available mentor teaching facts.");

            var applicationResult = applicationService.Apply(
                teachingResult,
                masteryProfile,
                skillRegistry);

            Assert(
                applicationResult.Applied
                && applicationResult.ProgressApplied == 15,
                "Integrated workflow should apply available teaching to mastery.");

            Assert(
                masteryProfile.GetRecord("skill_deep_sight").Progress == 15,
                "Integrated workflow should change mastery progress.");

            var snapshotService = new V2ProgressSnapshotService();
            var snapshot = snapshotService.CreateSnapshot(
                new CodexProfile(),
                new MissionJournalProfile(),
                new UnlockProfile(),
                classProfile,
                masteryProfile);

            Assert(
                snapshot.ClassProfile != null
                && snapshot.MasteryRecords.Count == 1,
                "Integrated workflow should snapshot class and mastery state.");

            var restoredClassProfile = new ClassProfile();
            var restoredMasteryProfile = new MasteryProfile();
            bool classRestored = snapshotService.RestoreClassProfile(
                snapshot,
                restoredClassProfile,
                classRegistry);
            bool masteryRestored = snapshotService.RestoreMasteryProfile(
                snapshot,
                restoredMasteryProfile,
                skillRegistry);

            Assert(
                classRestored
                && restoredClassProfile.CurrentClassId == ClassId.Tlamatini,
                "Integrated workflow should restore class state through the registry.");

            Assert(
                masteryRestored
                && restoredMasteryProfile.GetRecord("skill_deep_sight").Progress == 15,
                "Integrated workflow should restore mastery state through the registry.");
        }

        private static void AssertMasteredSkillRetentionIntegratedWorkflow()
        {
            const string firstClassId = "class_first_memory";
            const string secondClassId = "class_second_empty";
            const string masteredSkillId = "skill_memory_focus";
            const string unmasteredSkillId = "skill_memory_unmastered";

            var skillRegistry = new SkillDefinitionRegistry();
            skillRegistry.Register(BuildTestSkillDefinition(masteredSkillId, "Memory Focus"));
            skillRegistry.Register(BuildTestSkillDefinition(unmasteredSkillId, "Memory Unmastered"));

            var classRegistry = new ClassDefinitionRegistry();
            classRegistry.Register(new ClassDefinition(
                firstClassId,
                "First Memory",
                new[]
                {
                    masteredSkillId,
                    unmasteredSkillId
                }));
            classRegistry.Register(new ClassDefinition(
                secondClassId,
                "Second Empty",
                new string[0]));

            var classProfile = new ClassProfile();
            var mentorProfile = BuildTrustedLyraRelationship(25);
            var masteryProfile = new MasteryProfile();
            var teachingDefinition = new MentorTeachingDefinition(
                MentorId.Lyra,
                masteredSkillId,
                20,
                MasteryRecord.MasteredProgress);

            Assert(
                skillRegistry.Contains(masteredSkillId)
                && skillRegistry.Contains(unmasteredSkillId),
                "Retention workflow should register two skills.");

            Assert(
                classRegistry.Contains(firstClassId)
                && classRegistry.Contains(secondClassId)
                && classRegistry.Get(firstClassId).AllowsSkill(unmasteredSkillId, skillRegistry)
                && !classRegistry.Get(secondClassId).AllowsSkill(unmasteredSkillId, skillRegistry),
                "Retention workflow should register two classes with different allowed skills.");

            Assert(
                classProfile.AssignClassId(firstClassId, classRegistry)
                && classProfile.CurrentClassAllowsSkill(masteredSkillId, classRegistry, skillRegistry),
                "Retention workflow should assign the first class before mentor teaching.");

            var teachingResult = new MentorTeachingService().Evaluate(
                teachingDefinition,
                mentorProfile,
                BuildTestMentorRegistry(),
                skillRegistry);
            var applicationResult = new MentorTeachingApplicationService().Apply(
                teachingResult,
                masteryProfile,
                skillRegistry);

            Assert(
                teachingResult.IsAvailable
                && applicationResult.Applied
                && masteryProfile.IsMastered(masteredSkillId),
                "Retention workflow should mentor-teach and master the first-class skill.");

            Assert(
                classProfile.ChangeClassId(secondClassId, classRegistry)
                && classProfile.CurrentClassId == secondClassId,
                "Retention workflow should switch to the second class.");

            var retentionPolicy = new MasteredSkillRetentionPolicy();
            var masteredResult = retentionPolicy.Evaluate(
                masteredSkillId,
                classProfile,
                classRegistry,
                masteryProfile,
                skillRegistry);
            var unmasteredResult = retentionPolicy.Evaluate(
                unmasteredSkillId,
                classProfile,
                classRegistry,
                masteryProfile,
                skillRegistry);

            Assert(
                masteredResult.IsUsable
                && !masteredResult.AllowedByCurrentClass
                && masteredResult.AllowedByMastery,
                "Retention workflow should keep mastered first-class skill usable after class switch.");

            Assert(
                !unmasteredResult.IsUsable
                && !unmasteredResult.AllowedByCurrentClass
                && !unmasteredResult.AllowedByMastery,
                "Retention workflow should block non-mastered first-class skill after class switch.");
        }

        private static void AssertCombatFacingSkillAccessIntegratedWorkflow()
        {
            const string firstClassId = "class_combat_memory";
            const string secondClassId = "class_combat_empty";
            const string taughtSkillId = "skill_combat_memory";
            const string unmasteredSkillId = "skill_combat_unmastered";

            var skillRegistry = new SkillDefinitionRegistry();
            bool taughtSkillRegistered = skillRegistry.Register(
                BuildTestSkillDefinition(taughtSkillId, "Combat Memory"));
            bool unmasteredSkillRegistered = skillRegistry.Register(
                BuildTestSkillDefinition(unmasteredSkillId, "Combat Unmastered"));
            var classRegistry = new ClassDefinitionRegistry();
            bool firstClassRegistered = classRegistry.Register(new ClassDefinition(
                firstClassId,
                "Combat Memory Class",
                new[] { taughtSkillId, unmasteredSkillId }));
            bool secondClassRegistered = classRegistry.Register(new ClassDefinition(
                secondClassId,
                "Combat Empty Class",
                new string[0]));
            var classProfile = new ClassProfile();
            var masteryProfile = new MasteryProfile();
            var mentorRegistry = BuildTestMentorRegistry();
            var mentorProfile = new MentorRelationshipProfile();
            var accessService = new CombatSkillAccessService();

            Assert(
                taughtSkillRegistered
                && unmasteredSkillRegistered,
                "Combat access workflow should register two skills.");

            Assert(
                firstClassRegistered
                && secondClassRegistered
                && classRegistry.Get(firstClassId).AllowsSkill(taughtSkillId, skillRegistry)
                && !classRegistry.Get(secondClassId).AllowsSkill(taughtSkillId, skillRegistry),
                "Combat access workflow should register classes with different allowed skills.");

            Assert(
                classProfile.AssignClassId(firstClassId, classRegistry),
                "Combat access workflow should assign the first class.");

            var initialAccess = accessService.Evaluate(
                taughtSkillId,
                classProfile,
                classRegistry,
                masteryProfile,
                skillRegistry);

            Assert(
                initialAccess.IsUsable
                && initialAccess.AllowedByCurrentClass
                && !initialAccess.AllowedByMastery,
                "First-class skill should initially be combat-usable through class access.");

            Assert(
                mentorProfile.Discover(MentorId.Lyra, mentorRegistry)
                && mentorProfile.IncreaseTrust(MentorId.Lyra, 25, mentorRegistry),
                "Combat access workflow should establish sufficient mentor trust.");

            var teachingResult = new MentorTeachingService().Evaluate(
                new MentorTeachingDefinition(
                    MentorId.Lyra,
                    taughtSkillId,
                    20,
                    MasteryRecord.MasteredProgress),
                mentorProfile,
                mentorRegistry,
                skillRegistry);
            var applicationResult = new MentorTeachingApplicationService().Apply(
                teachingResult,
                masteryProfile,
                skillRegistry);

            Assert(
                teachingResult.IsAvailable
                && applicationResult.Applied
                && masteryProfile.IsMastered(taughtSkillId),
                "Combat access workflow should mentor-teach the first-class skill to mastery.");

            Assert(
                classProfile.ChangeClassId(secondClassId, classRegistry),
                "Combat access workflow should switch to the second class.");

            var retainedAccess = accessService.Evaluate(
                taughtSkillId,
                classProfile,
                classRegistry,
                masteryProfile,
                skillRegistry);
            var blockedAccess = accessService.Evaluate(
                unmasteredSkillId,
                classProfile,
                classRegistry,
                masteryProfile,
                skillRegistry);
            var unknownAccess = accessService.Evaluate(
                "skill_unknown",
                classProfile,
                classRegistry,
                masteryProfile,
                skillRegistry);

            Assert(
                retainedAccess.IsUsable
                && !retainedAccess.AllowedByCurrentClass
                && retainedAccess.AllowedByMastery,
                "Mastered first-class skill should remain combat-usable after class switch.");

            Assert(
                !blockedAccess.IsUsable
                && blockedAccess.BlockReason
                    == MasteredSkillRetentionBlockReason.SkillNotAllowed,
                "Non-mastered first-class skill should be blocked after class switch.");

            Assert(
                !unknownAccess.IsUsable
                && unknownAccess.BlockReason
                    == MasteredSkillRetentionBlockReason.UnknownSkill,
                "Unknown skill should be blocked by combat-facing access.");

            var policyResult = new MasteredSkillRetentionPolicy().Evaluate(
                taughtSkillId,
                classProfile,
                classRegistry,
                masteryProfile,
                skillRegistry);

            Assert(
                retainedAccess.SkillId == policyResult.SkillId
                && retainedAccess.IsUsable == policyResult.IsUsable
                && retainedAccess.AllowedByCurrentClass
                    == policyResult.AllowedByCurrentClass
                && retainedAccess.AllowedByMastery == policyResult.AllowedByMastery
                && retainedAccess.BlockReason == policyResult.BlockReason,
                "Combat access workflow facts should match retention policy facts.");
        }

        private static void AssertCombatSkillUseValidatorIntegratedWorkflow()
        {
            const string firstClassId = "class_validator_workflow_first";
            const string secondClassId = "class_validator_workflow_second";
            const string taughtSkillId = "skill_validator_workflow_taught";
            const string unmasteredSkillId = "skill_validator_workflow_unmastered";

            var taughtSkill = BuildTestSkillDefinition(
                taughtSkillId,
                "Validator Workflow Taught");
            var unmasteredSkill = BuildTestSkillDefinition(
                unmasteredSkillId,
                "Validator Workflow Unmastered");
            var skillRegistry = new SkillDefinitionRegistry();
            bool taughtSkillRegistered = skillRegistry.Register(taughtSkill);
            bool unmasteredSkillRegistered = skillRegistry.Register(unmasteredSkill);
            var classRegistry = new ClassDefinitionRegistry();
            bool firstClassRegistered = classRegistry.Register(new ClassDefinition(
                firstClassId,
                "Validator Workflow First",
                new[] { taughtSkillId, unmasteredSkillId }));
            bool secondClassRegistered = classRegistry.Register(new ClassDefinition(
                secondClassId,
                "Validator Workflow Second",
                new string[0]));
            var classProfile = new ClassProfile();
            var masteryProfile = new MasteryProfile();
            var unlockProfile = new UnlockProfile();
            var validator = new CombatSkillUseValidator();

            Assert(
                taughtSkillRegistered && unmasteredSkillRegistered,
                "Validator workflow should register two skills.");

            Assert(
                firstClassRegistered
                && secondClassRegistered
                && classRegistry.Get(firstClassId).AllowsSkill(
                    taughtSkillId,
                    skillRegistry)
                && !classRegistry.Get(secondClassId).AllowsSkill(
                    taughtSkillId,
                    skillRegistry),
                "Validator workflow should register two distinct class access sets.");

            Assert(
                classProfile.AssignClassId(firstClassId, classRegistry),
                "Validator workflow should assign the first class.");

            var initialValidation = validator.Validate(
                TurnRequest.SkillAction(
                    null,
                    null,
                    new V4ValidatorCombatSkill(taughtSkillId),
                    new SkillProgress(taughtSkill)),
                true,
                unlockProfile,
                classProfile,
                classRegistry,
                masteryProfile,
                skillRegistry);

            Assert(
                initialValidation.IsAllowed
                && initialValidation.V4AccessChecked
                && initialValidation.BlockReason
                    == CombatSkillUseBlockReason.None,
                "First-class skill should pass integrated combat skill use validation.");

            var mentorRegistry = BuildTestMentorRegistry();
            var mentorProfile = new MentorRelationshipProfile();
            mentorProfile.Discover(MentorId.Lyra, mentorRegistry);
            mentorProfile.IncreaseTrust(MentorId.Lyra, 25, mentorRegistry);
            var teachingResult = new MentorTeachingService().Evaluate(
                new MentorTeachingDefinition(
                    MentorId.Lyra,
                    taughtSkillId,
                    20,
                    MasteryRecord.MasteredProgress),
                mentorProfile,
                mentorRegistry,
                skillRegistry);
            var applicationResult = new MentorTeachingApplicationService().Apply(
                teachingResult,
                masteryProfile,
                skillRegistry);

            Assert(
                teachingResult.IsAvailable
                && applicationResult.Applied
                && masteryProfile.IsMastered(taughtSkillId),
                "Validator workflow should mentor-teach the first-class skill to mastery.");

            Assert(
                classProfile.ChangeClassId(secondClassId, classRegistry),
                "Validator workflow should switch to the second class.");

            var retainedValidation = validator.Validate(
                TurnRequest.SkillAction(
                    null,
                    null,
                    new V4ValidatorCombatSkill(taughtSkillId),
                    new SkillProgress(taughtSkill)),
                true,
                unlockProfile,
                classProfile,
                classRegistry,
                masteryProfile,
                skillRegistry);
            var blockedValidation = validator.Validate(
                TurnRequest.SkillAction(
                    null,
                    null,
                    new V4ValidatorCombatSkill(unmasteredSkillId),
                    new SkillProgress(unmasteredSkill)),
                true,
                unlockProfile,
                classProfile,
                classRegistry,
                masteryProfile,
                skillRegistry);

            Assert(
                retainedValidation.IsAllowed
                && retainedValidation.V4AccessChecked,
                "Mastered retained skill should pass validation after class switch.");

            Assert(
                !blockedValidation.IsAllowed
                && blockedValidation.BlockReason
                    == CombatSkillUseBlockReason.V4SkillBlocked
                && blockedValidation.V4BlockReason
                    == MasteredSkillRetentionBlockReason.SkillNotAllowed,
                "Non-mastered cross-class skill should be blocked after class switch.");

            var enemyValidation = validator.Validate(
                TurnRequest.SkillAction(
                    null,
                    null,
                    new V4ValidatorCombatSkill(unmasteredSkillId),
                    new SkillProgress(unmasteredSkill)),
                false,
                null,
                null,
                null,
                null,
                null);

            Assert(
                enemyValidation.IsAllowed
                && !enemyValidation.IsPlayerAction
                && !enemyValidation.V3AccessChecked
                && !enemyValidation.V4AccessChecked,
                "Enemy mode should allow a structurally valid skill without V4 profiles.");

            var mismatchValidation = validator.Validate(
                TurnRequest.SkillAction(
                    null,
                    null,
                    new V4ValidatorCombatSkill(taughtSkillId),
                    new SkillProgress(unmasteredSkill)),
                true,
                unlockProfile,
                classProfile,
                classRegistry,
                masteryProfile,
                skillRegistry);

            Assert(
                !mismatchValidation.IsAllowed
                && mismatchValidation.BlockReason
                    == CombatSkillUseBlockReason.SkillProgressIdMismatch,
                "Validator workflow should block mismatched skill and progress ids.");
        }

        private static void AssertCombatSkillUseValidationContextIntegratedWorkflow()
        {
            const string firstClassId = "class_context_workflow_first";
            const string secondClassId = "class_context_workflow_second";
            const string retainedSkillId = "skill_context_workflow_retained";
            const string unmasteredSkillId = "skill_context_workflow_unmastered";

            var retainedSkill = BuildTestSkillDefinition(
                retainedSkillId,
                "Context Workflow Retained");
            var unmasteredSkill = BuildTestSkillDefinition(
                unmasteredSkillId,
                "Context Workflow Unmastered");
            var skillRegistry = new SkillDefinitionRegistry();
            skillRegistry.Register(retainedSkill);
            skillRegistry.Register(unmasteredSkill);
            var classRegistry = new ClassDefinitionRegistry();
            classRegistry.Register(new ClassDefinition(
                firstClassId,
                "Context Workflow First",
                new[] { retainedSkillId, unmasteredSkillId }));
            classRegistry.Register(new ClassDefinition(
                secondClassId,
                "Context Workflow Second",
                new string[0]));
            var classProfile = new ClassProfile();
            classProfile.AssignClassId(firstClassId, classRegistry);
            var masteryProfile = new MasteryProfile();
            var unlockProfile = new UnlockProfile();
            var validator = new CombatSkillUseValidator();
            var retainedRequest = TurnRequest.SkillAction(
                null,
                null,
                new V4ValidatorCombatSkill(retainedSkillId),
                new SkillProgress(retainedSkill));
            var unmasteredRequest = TurnRequest.SkillAction(
                null,
                null,
                new V4ValidatorCombatSkill(unmasteredSkillId),
                new SkillProgress(unmasteredSkill));

            var legacyResult = validator.Validate(
                retainedRequest,
                CombatSkillUseValidationContext.LegacyBypass());

            Assert(
                legacyResult.IsAllowed
                && legacyResult.IsPlayerAction
                && !legacyResult.V3AccessChecked
                && !legacyResult.V4AccessChecked,
                "Context workflow should allow a structurally valid legacy skill without V4 profiles.");

            var enemyResult = validator.Validate(
                unmasteredRequest,
                CombatSkillUseValidationContext.Enemy());

            Assert(
                enemyResult.IsAllowed
                && !enemyResult.IsPlayerAction
                && !enemyResult.V3AccessChecked
                && !enemyResult.V4AccessChecked,
                "Context workflow should allow a structurally valid enemy skill without player profiles.");

            var partialPlayerResult = validator.Validate(
                retainedRequest,
                CombatSkillUseValidationContext.Player(
                    unlockProfile,
                    classProfile,
                    masteryProfile,
                    classRegistry,
                    null));

            Assert(
                !partialPlayerResult.IsAllowed
                && partialPlayerResult.BlockReason
                    == CombatSkillUseBlockReason.PlayerContextIncomplete,
                "Context workflow should reject partial player context.");

            var firstClassPlayerContext = CombatSkillUseValidationContext.Player(
                unlockProfile,
                classProfile,
                masteryProfile,
                classRegistry,
                skillRegistry);
            var firstClassResult = validator.Validate(
                retainedRequest,
                firstClassPlayerContext);

            Assert(
                firstClassResult.IsAllowed
                && firstClassResult.V4AccessChecked,
                "Context workflow should allow a current-class player skill.");

            masteryProfile.IncreaseProgress(
                retainedSkillId,
                MasteryRecord.MasteredProgress,
                skillRegistry);
            classProfile.ChangeClassId(secondClassId, classRegistry);
            var secondClassPlayerContext = CombatSkillUseValidationContext.Player(
                unlockProfile,
                classProfile,
                masteryProfile,
                classRegistry,
                skillRegistry);
            var retainedResult = validator.Validate(
                retainedRequest,
                secondClassPlayerContext);

            Assert(
                retainedResult.IsAllowed
                && retainedResult.V4AccessChecked,
                "Context workflow should allow a mastered retained skill after class switch.");

            var blockedResult = validator.Validate(
                unmasteredRequest,
                secondClassPlayerContext);

            Assert(
                !blockedResult.IsAllowed
                && blockedResult.BlockReason
                    == CombatSkillUseBlockReason.V4SkillBlocked
                && blockedResult.V4BlockReason
                    == MasteredSkillRetentionBlockReason.SkillNotAllowed,
                "Context workflow should reject a non-mastered cross-class skill.");

            var mismatchResult = validator.Validate(
                TurnRequest.SkillAction(
                    null,
                    null,
                    new V4ValidatorCombatSkill(retainedSkillId),
                    new SkillProgress(unmasteredSkill)),
                secondClassPlayerContext);

            Assert(
                !mismatchResult.IsAllowed
                && mismatchResult.BlockReason
                    == CombatSkillUseBlockReason.SkillProgressIdMismatch,
                "Context workflow should reject mismatched skill and progress ids.");
        }

        private static void AssertSkillTreeProgressionIntegratedWorkflow()
        {
            const string rootSkillId = "skill_tree_root";
            const string prerequisiteSkillId = "skill_tree_prerequisite";
            const string dependentSkillId = "skill_tree_dependent";
            const string rootNodeId = "node_tree_root";
            const string prerequisiteNodeId = "node_tree_prerequisite";
            const string dependentNodeId = "node_tree_dependent";

            var skillRegistry = new SkillDefinitionRegistry();
            skillRegistry.Register(BuildTestSkillDefinition(rootSkillId, "Tree Root"));
            skillRegistry.Register(BuildTestSkillDefinition(prerequisiteSkillId, "Tree Prerequisite"));
            skillRegistry.Register(BuildTestSkillDefinition(dependentSkillId, "Tree Dependent"));

            var skillTree = new SkillTreeDefinition();
            skillTree.RegisterNode(
                new SkillTreeNodeDefinition(
                    rootNodeId,
                    rootSkillId,
                    new string[0]),
                skillRegistry);
            skillTree.RegisterNode(
                new SkillTreeNodeDefinition(
                    prerequisiteNodeId,
                    prerequisiteSkillId,
                    new[] { rootNodeId }),
                skillRegistry);
            skillTree.RegisterNode(
                new SkillTreeNodeDefinition(
                    dependentNodeId,
                    dependentSkillId,
                    new[] { prerequisiteNodeId }),
                skillRegistry);

            var progress = new SkillTreeProgressProfile();
            var skillBook = new PlayerSkillBook();
            var masteryProfile = new MasteryProfile();
            var unlockPolicy = new SkillTreeUnlockPolicy();
            var applicationService = new SkillTreeUnlockApplicationService(unlockPolicy);

            Assert(
                skillRegistry.Contains(rootSkillId)
                && skillRegistry.Contains(prerequisiteSkillId)
                && skillRegistry.Contains(dependentSkillId),
                "Skill tree workflow should register skills.");

            Assert(
                skillTree.Contains(rootNodeId)
                && skillTree.Contains(prerequisiteNodeId)
                && skillTree.Contains(dependentNodeId),
                "Skill tree workflow should create a three-node skill tree.");

            Assert(
                progress.DiscoverNode(rootNodeId, skillTree)
                && progress.IsDiscovered(rootNodeId),
                "Skill tree workflow should discover the first node.");

            var rootResult = applicationService.Apply(
                rootNodeId,
                skillTree,
                progress,
                skillRegistry);

            Assert(
                rootResult.Applied
                && progress.IsUnlocked(rootNodeId),
                "Skill tree workflow should unlock the first node through the application service.");

            progress.DiscoverNode(prerequisiteNodeId, skillTree);
            progress.DiscoverNode(dependentNodeId, skillTree);
            var blockedDependent = unlockPolicy.Evaluate(
                dependentNodeId,
                skillTree,
                progress,
                skillRegistry);

            Assert(
                !blockedDependent.CanUnlock
                && blockedDependent.BlockReason == SkillTreeUnlockBlockReason.PrerequisiteMissing
                && blockedDependent.MissingPrerequisiteNodeId == prerequisiteNodeId,
                "Skill tree workflow should block dependent node before prerequisite unlock.");

            var prerequisiteResult = applicationService.Apply(
                prerequisiteNodeId,
                skillTree,
                progress,
                skillRegistry);

            Assert(
                prerequisiteResult.Applied
                && progress.IsUnlocked(prerequisiteNodeId),
                "Skill tree workflow should unlock the prerequisite node through the application service.");

            var dependentResult = applicationService.Apply(
                dependentNodeId,
                skillTree,
                progress,
                skillRegistry);

            Assert(
                dependentResult.Applied
                && progress.IsUnlocked(dependentNodeId),
                "Skill tree workflow should unlock the dependent node through the application service.");

            Assert(
                !skillBook.HasSkill(rootSkillId)
                && !skillBook.HasSkill(prerequisiteSkillId)
                && !skillBook.HasSkill(dependentSkillId),
                "Skill tree workflow should not automatically grant skills.");

            Assert(
                masteryProfile.Records.Count == 0,
                "Skill tree workflow should leave mastery profile unchanged.");
        }

        private static void AssertSkillTreeUnlockApplicationIntegratedWorkflow()
        {
            const string rootSkillId = "skill_application_root";
            const string prerequisiteSkillId = "skill_application_prerequisite";
            const string dependentSkillId = "skill_application_dependent";
            const string rootNodeId = "node_application_root";
            const string prerequisiteNodeId = "node_application_prerequisite";
            const string dependentNodeId = "node_application_dependent";

            var skillRegistry = new SkillDefinitionRegistry();
            bool rootSkillRegistered = skillRegistry.Register(
                BuildTestSkillDefinition(rootSkillId, "Application Root"));
            bool prerequisiteSkillRegistered = skillRegistry.Register(
                BuildTestSkillDefinition(prerequisiteSkillId, "Application Prerequisite"));
            bool dependentSkillRegistered = skillRegistry.Register(
                BuildTestSkillDefinition(dependentSkillId, "Application Dependent"));

            var skillTree = new SkillTreeDefinition();
            bool rootNodeRegistered = skillTree.RegisterNode(
                new SkillTreeNodeDefinition(
                    rootNodeId,
                    rootSkillId,
                    new string[0]),
                skillRegistry);
            bool prerequisiteNodeRegistered = skillTree.RegisterNode(
                new SkillTreeNodeDefinition(
                    prerequisiteNodeId,
                    prerequisiteSkillId,
                    new[] { rootNodeId }),
                skillRegistry);
            bool dependentNodeRegistered = skillTree.RegisterNode(
                new SkillTreeNodeDefinition(
                    dependentNodeId,
                    dependentSkillId,
                    new[] { prerequisiteNodeId }),
                skillRegistry);

            var progress = new SkillTreeProgressProfile();
            var skillBook = new PlayerSkillBook();
            var masteryProfile = new MasteryProfile();
            var applicationService = new SkillTreeUnlockApplicationService(
                new SkillTreeUnlockPolicy());

            Assert(
                rootSkillRegistered
                && prerequisiteSkillRegistered
                && dependentSkillRegistered,
                "Skill tree unlock application workflow should register all skill definitions.");

            Assert(
                rootNodeRegistered
                && prerequisiteNodeRegistered
                && dependentNodeRegistered,
                "Skill tree unlock application workflow should create a valid prerequisite-first tree.");

            Assert(
                progress.DiscoverNode(rootNodeId, skillTree),
                "Skill tree unlock application workflow should discover the root node.");

            var rootResult = applicationService.Apply(
                rootNodeId,
                skillTree,
                progress,
                skillRegistry);

            Assert(
                rootResult.Applied
                && progress.IsUnlocked(rootNodeId),
                "Skill tree unlock application workflow should unlock the root through the application service.");

            Assert(
                progress.DiscoverNode(dependentNodeId, skillTree),
                "Skill tree unlock application workflow should discover the dependent node.");

            var blockedDependentResult = applicationService.Apply(
                dependentNodeId,
                skillTree,
                progress,
                skillRegistry);

            Assert(
                !blockedDependentResult.Applied
                && blockedDependentResult.BlockReason
                    == SkillTreeUnlockBlockReason.PrerequisiteMissing
                && !progress.IsUnlocked(dependentNodeId),
                "Skill tree unlock application workflow should preserve progress when a prerequisite blocks unlock.");

            progress.DiscoverNode(prerequisiteNodeId, skillTree);
            var prerequisiteResult = applicationService.Apply(
                prerequisiteNodeId,
                skillTree,
                progress,
                skillRegistry);

            Assert(
                prerequisiteResult.Applied
                && progress.IsUnlocked(prerequisiteNodeId),
                "Skill tree unlock application workflow should unlock the prerequisite through the application service.");

            var dependentResult = applicationService.Apply(
                dependentNodeId,
                skillTree,
                progress,
                skillRegistry);

            Assert(
                dependentResult.Applied
                && progress.IsUnlocked(dependentNodeId),
                "Skill tree unlock application workflow should unlock the dependent node after its prerequisite.");

            Assert(
                !skillBook.HasSkill(rootSkillId)
                && !skillBook.HasSkill(prerequisiteSkillId)
                && !skillBook.HasSkill(dependentSkillId),
                "Skill tree unlock application workflow should not grant skills automatically.");

            Assert(
                masteryProfile.Records.Count == 0,
                "Skill tree unlock application workflow should leave mastery unchanged.");
        }

        private static void AssertClassSkillTreeBindingIntegratedWorkflow()
        {
            var classRegistry = BuildTestClassRegistry();
            var bindingRegistry = BuildTestClassSkillTreeBindingRegistry();
            var classProfile = new ClassProfile();
            var progress = new SkillTreeProgressProfile();
            var skillBook = new PlayerSkillBook();

            bool firstBindingRegistered = bindingRegistry.RegisterBinding(
                new ClassSkillTreeBinding(
                    ClassId.Tlamatini,
                    SkillTreeId.Tlamatini),
                classRegistry);
            bool secondBindingRegistered = bindingRegistry.RegisterBinding(
                new ClassSkillTreeBinding(
                    ClassId.Wanderer,
                    SkillTreeId.Wanderer),
                classRegistry);

            Assert(
                classRegistry.Contains(ClassId.Tlamatini)
                && classRegistry.Contains(ClassId.Wanderer),
                "Class skill tree binding workflow should register two classes.");

            Assert(
                bindingRegistry.ContainsSkillTree(SkillTreeId.Tlamatini)
                && bindingRegistry.ContainsSkillTree(SkillTreeId.Wanderer),
                "Class skill tree binding workflow should register two stable skill trees.");

            Assert(
                firstBindingRegistered
                && secondBindingRegistered,
                "Class skill tree binding workflow should bind each class to a different skill tree.");

            Assert(
                classProfile.AssignClassId(ClassId.Tlamatini, classRegistry),
                "Class skill tree binding workflow should assign the first class.");

            var firstSkillTree = bindingRegistry.ResolveCurrentClassSkillTree(classProfile);

            Assert(
                firstSkillTree != null
                && firstSkillTree.Id == SkillTreeId.Tlamatini,
                "Class skill tree binding workflow should resolve the first class skill tree.");

            Assert(
                classProfile.ChangeClassId(ClassId.Wanderer, classRegistry),
                "Class skill tree binding workflow should switch to the second class.");

            var secondSkillTree = bindingRegistry.ResolveCurrentClassSkillTree(classProfile);

            Assert(
                secondSkillTree != null
                && secondSkillTree.Id == SkillTreeId.Wanderer,
                "Class skill tree binding workflow should resolve the second class skill tree.");

            Assert(
                progress.DiscoveredNodeIds.Count == 0
                && progress.UnlockedNodeIds.Count == 0,
                "Class skill tree bindings should not mutate skill tree progress.");

            Assert(
                !skillBook.HasSkill("skill_deep_sight")
                && !skillBook.HasSkill("skill_minor_cleanse"),
                "Class skill tree bindings should not automatically grant skills.");
        }

        private static void AssertClassSkillTreeBindingSaveRestoreIntegratedWorkflow()
        {
            var classRegistry = BuildTestClassRegistry();
            var sourceBindingRegistry = BuildTestClassSkillTreeBindingRegistryWithBindings();
            var progress = new SkillTreeProgressProfile();
            var skillBook = new PlayerSkillBook();

            Assert(
                classRegistry.Contains(ClassId.Tlamatini)
                && classRegistry.Contains(ClassId.Wanderer),
                "Binding save workflow should register two classes.");

            Assert(
                sourceBindingRegistry.ContainsSkillTree(SkillTreeId.Tlamatini)
                && sourceBindingRegistry.ContainsSkillTree(SkillTreeId.Wanderer),
                "Binding save workflow should register two stable skill trees.");

            Assert(
                sourceBindingRegistry.GetBindingForClass(ClassId.Tlamatini).SkillTreeId
                    == SkillTreeId.Tlamatini
                && sourceBindingRegistry.GetBindingForClass(ClassId.Wanderer).SkillTreeId
                    == SkillTreeId.Wanderer,
                "Binding save workflow should bind each class to a different skill tree.");

            var snapshot = BuildClassSkillTreeBindingSnapshot(sourceBindingRegistry);

            Assert(
                snapshot.ClassSkillTreeBindings.Count == 2,
                "Binding save workflow should include both bindings in the snapshot.");

            var serializer = new V2ProgressSnapshotJsonSerializer();
            string json = serializer.Serialize(snapshot);
            var deserializedSnapshot = serializer.Deserialize(json);

            Assert(
                deserializedSnapshot.ClassSkillTreeBindings.Count == 2,
                "Binding save workflow should preserve bindings through JSON serialization.");

            var restoredBindingRegistry = BuildTestClassSkillTreeBindingRegistry();
            bool restored = new V2ProgressSnapshotService().RestoreClassSkillTreeBindings(
                deserializedSnapshot,
                restoredBindingRegistry,
                classRegistry);

            Assert(
                restored
                && restoredBindingRegistry.Bindings.Count == 2,
                "Binding save workflow should restore bindings into a fresh registry.");

            var classProfile = new ClassProfile();
            bool firstAssigned = classProfile.AssignClassId(
                ClassId.Tlamatini,
                classRegistry);
            var firstSkillTree = restoredBindingRegistry.ResolveCurrentClassSkillTree(classProfile);

            Assert(
                firstAssigned
                && firstSkillTree != null
                && firstSkillTree.Id == SkillTreeId.Tlamatini,
                "Binding save workflow should resolve the restored first-class skill tree.");

            bool secondAssigned = classProfile.ChangeClassId(
                ClassId.Wanderer,
                classRegistry);
            var secondSkillTree = restoredBindingRegistry.ResolveCurrentClassSkillTree(classProfile);

            Assert(
                secondAssigned
                && secondSkillTree != null
                && secondSkillTree.Id == SkillTreeId.Wanderer,
                "Binding save workflow should resolve the restored second-class skill tree after switching.");

            Assert(
                progress.DiscoveredNodeIds.Count == 0
                && progress.UnlockedNodeIds.Count == 0,
                "Restored bindings should not mutate skill tree progress.");

            Assert(
                !skillBook.HasSkill("skill_deep_sight")
                && !skillBook.HasSkill("skill_minor_cleanse"),
                "Restored bindings should not automatically grant skills.");
        }

        private static void AssertScopedSkillTreeProgressIntegratedWorkflow()
        {
            const string sharedNodeId = "node_deep_sight";
            const string firstTreeOnlyNodeId = "node_balance_ritual";

            var classRegistry = BuildTestClassRegistry();
            var bindingRegistry = BuildTestClassSkillTreeBindingRegistryWithBindings();
            var classProfile = new ClassProfile();
            var progress = new SkillTreeProgressProfile();
            var skillRegistry = BuildTestSkillRegistry();
            var applicationService = new SkillTreeUnlockApplicationService();

            Assert(
                classRegistry.Contains(ClassId.Tlamatini)
                && classRegistry.Contains(ClassId.Wanderer),
                "Scoped progress workflow should register two classes.");

            Assert(
                bindingRegistry.ContainsSkillTree(SkillTreeId.Tlamatini)
                && bindingRegistry.ContainsSkillTree(SkillTreeId.Wanderer)
                && bindingRegistry.GetSkillTree(SkillTreeId.Tlamatini).Contains(sharedNodeId)
                && bindingRegistry.GetSkillTree(SkillTreeId.Wanderer).Contains(sharedNodeId),
                "Scoped progress workflow should register two trees containing the same node id.");

            Assert(
                bindingRegistry.GetBindingForClass(ClassId.Tlamatini).SkillTreeId
                    == SkillTreeId.Tlamatini
                && bindingRegistry.GetBindingForClass(ClassId.Wanderer).SkillTreeId
                    == SkillTreeId.Wanderer,
                "Scoped progress workflow should bind each class to a different tree.");

            Assert(
                classProfile.AssignClassId(ClassId.Tlamatini, classRegistry),
                "Scoped progress workflow should assign the first class.");

            var firstTree = bindingRegistry.ResolveCurrentClassSkillTree(classProfile);

            Assert(
                progress.DiscoverNode(sharedNodeId, firstTree),
                "Scoped progress workflow should discover the shared node in the first tree.");

            var firstUnlock = applicationService.Apply(
                sharedNodeId,
                firstTree,
                progress,
                skillRegistry);

            Assert(
                firstUnlock.Applied
                && progress.IsUnlocked(sharedNodeId, firstTree),
                "Scoped progress workflow should unlock the shared node in the first tree.");

            progress.DiscoverNode(firstTreeOnlyNodeId, firstTree);

            Assert(
                classProfile.ChangeClassId(ClassId.Wanderer, classRegistry),
                "Scoped progress workflow should switch to the second class.");

            var secondTree = bindingRegistry.ResolveCurrentClassSkillTree(classProfile);

            Assert(
                secondTree.Id == SkillTreeId.Wanderer
                && !progress.IsUnlocked(sharedNodeId, secondTree),
                "The shared node id should remain locked in the second tree after class switch.");

            Assert(
                progress.DiscoverNode(sharedNodeId, secondTree),
                "Scoped progress workflow should discover the shared node independently in the second tree.");

            var secondUnlock = applicationService.Apply(
                sharedNodeId,
                secondTree,
                progress,
                skillRegistry);

            Assert(
                secondUnlock.Applied
                && progress.IsUnlocked(sharedNodeId, secondTree),
                "Scoped progress workflow should unlock the shared node independently in the second tree.");

            var snapshotService = new V2ProgressSnapshotService();
            var snapshot = snapshotService.CreateSnapshot(
                new CodexProfile(),
                new MissionJournalProfile(),
                new UnlockProfile(),
                classProfile,
                new MasteryProfile(),
                new MentorRelationshipProfile(),
                progress);
            var serializer = new V2ProgressSnapshotJsonSerializer();
            string json = serializer.Serialize(snapshot);
            var deserializedSnapshot = serializer.Deserialize(json);

            Assert(
                deserializedSnapshot.SkillTreeProgress.Scopes.Count == 2,
                "Scoped progress workflow should preserve both tree scopes through JSON.");

            var restoredRegistry = BuildTestClassSkillTreeBindingRegistry();
            var restoredFirstTree = restoredRegistry.GetSkillTree(SkillTreeId.Tlamatini);
            var restoredSecondTree = restoredRegistry.GetSkillTree(SkillTreeId.Wanderer);
            var restoredProgress = new SkillTreeProgressProfile();
            bool restored = snapshotService.RestoreSkillTreeProgressProfile(
                deserializedSnapshot,
                restoredProgress,
                restoredRegistry);

            Assert(
                restored
                && restoredProgress.IsUnlocked(sharedNodeId, restoredFirstTree)
                && restoredProgress.IsUnlocked(sharedNodeId, restoredSecondTree),
                "Scoped progress workflow should restore the shared unlock independently in both trees.");

            Assert(
                restoredProgress.IsDiscovered(firstTreeOnlyNodeId, restoredFirstTree)
                && !restoredProgress.IsDiscovered(firstTreeOnlyNodeId, restoredSecondTree),
                "Scoped progress workflow restore should preserve facts that belong to only one tree.");
        }

        private static MentorDefinition BuildLyraMentor(bool isAvailable)
        {
            return new MentorDefinition(
                MentorId.Lyra,
                "Lyra",
                isAvailable);
        }

        private static MentorDefinition BuildAtlasMentor(bool isAvailable)
        {
            return new MentorDefinition(
                MentorId.Atlas,
                "Atlas",
                isAvailable);
        }

        private static MentorDefinitionRegistry BuildTestMentorRegistry()
        {
            var registry = new MentorDefinitionRegistry();

            registry.Register(BuildLyraMentor(true));
            registry.Register(BuildAtlasMentor(false));

            return registry;
        }

        private static MentorRelationshipProfile BuildTrustedLyraRelationship(int trust)
        {
            var profile = new MentorRelationshipProfile();
            var registry = BuildTestMentorRegistry();

            profile.Discover(MentorId.Lyra, registry);
            profile.IncreaseTrust(
                MentorId.Lyra,
                trust,
                registry);

            return profile;
        }

        private static MentorRelationshipProfile BuildMentorProfileWithRelationships()
        {
            var profile = new MentorRelationshipProfile();
            var registry = BuildTestMentorRegistry();

            profile.Discover(MentorId.Lyra, registry);
            profile.IncreaseTrust(
                MentorId.Lyra,
                25,
                registry);
            profile.Discover(MentorId.Atlas, registry);
            profile.IncreaseTrust(
                MentorId.Atlas,
                3,
                registry);

            return profile;
        }

        private static V2ProgressSnapshot BuildUnknownMentorRelationshipSnapshot()
        {
            return new V2ProgressSnapshot(
                V2ProgressSnapshotVersions.CurrentSchemaVersion,
                new List<V2CodexEntrySnapshot>(),
                new List<V2MissionLeadSnapshot>(),
                new List<V2ActiveMissionSnapshot>(),
                new List<V2UnlockSnapshot>(),
                null,
                new List<V2MasterySnapshot>(),
                new[]
                {
                    new V2MentorRelationshipSnapshot(
                        "mentor_unknown",
                        true,
                        15,
                        true)
                });
        }

        private static V2MentorRelationshipSnapshot FindMentorRelationshipSnapshot(
            V2ProgressSnapshot snapshot,
            string mentorId)
        {
            foreach (var mentorRelationship in snapshot.MentorRelationships)
            {
                if (mentorRelationship.MentorId == mentorId)
                {
                    return mentorRelationship;
                }
            }

            return null;
        }

        private static MentorTeachingDefinition BuildLyraDeepSightTeaching(
            int requiredTrust,
            int masteryProgressGranted)
        {
            return new MentorTeachingDefinition(
                MentorId.Lyra,
                "skill_deep_sight",
                requiredTrust,
                masteryProgressGranted);
        }

        private static MentorTeachingResult BuildAvailableLyraDeepSightTeachingResult(int masteryProgressGranted)
        {
            return new MentorTeachingService().Evaluate(
                BuildLyraDeepSightTeaching(20, masteryProgressGranted),
                BuildTrustedLyraRelationship(25),
                BuildTestMentorRegistry(),
                BuildTestSkillRegistry());
        }

        private static SkillDefinitionRegistry BuildTestSkillRegistry()
        {
            var registry = new SkillDefinitionRegistry();

            registry.Register(BuildTestSkillDefinition("skill_deep_sight", "Deep Sight"));
            registry.Register(BuildTestSkillDefinition("skill_balance_ritual", "Balance Ritual"));
            registry.Register(BuildTestSkillDefinition("skill_minor_cleanse", "Minor Cleanse"));

            return registry;
        }

        private static SkillTreeDefinition BuildTestSkillTreeDefinition()
        {
            return BuildTestSkillTreeDefinition("");
        }

        private static SkillTreeDefinition BuildTestSkillTreeDefinition(string skillTreeId)
        {
            var skillTree = new SkillTreeDefinition(skillTreeId);
            var skillRegistry = BuildTestSkillRegistry();

            skillTree.RegisterNode(
                new SkillTreeNodeDefinition(
                    "node_deep_sight",
                    "skill_deep_sight",
                    new string[0]),
                skillRegistry);
            skillTree.RegisterNode(
                new SkillTreeNodeDefinition(
                    "node_balance_ritual",
                    "skill_balance_ritual",
                    new[] { "node_deep_sight" }),
                skillRegistry);

            return skillTree;
        }

        private static ClassSkillTreeBindingRegistry BuildTestClassSkillTreeBindingRegistry()
        {
            var registry = new ClassSkillTreeBindingRegistry();

            registry.RegisterSkillTree(BuildTestSkillTreeDefinition(SkillTreeId.Tlamatini));
            registry.RegisterSkillTree(BuildTestSkillTreeDefinition(SkillTreeId.Wanderer));

            return registry;
        }

        private static ClassSkillTreeBindingRegistry BuildTestClassSkillTreeBindingRegistryWithBindings()
        {
            var classRegistry = BuildTestClassRegistry();
            var bindingRegistry = BuildTestClassSkillTreeBindingRegistry();

            bindingRegistry.RegisterBinding(
                new ClassSkillTreeBinding(
                    ClassId.Tlamatini,
                    SkillTreeId.Tlamatini),
                classRegistry);
            bindingRegistry.RegisterBinding(
                new ClassSkillTreeBinding(
                    ClassId.Wanderer,
                    SkillTreeId.Wanderer),
                classRegistry);

            return bindingRegistry;
        }

        private static V2ProgressSnapshot BuildClassSkillTreeBindingSnapshot(
            ClassSkillTreeBindingRegistry bindingRegistry)
        {
            return new V2ProgressSnapshotService().CreateSnapshot(
                new CodexProfile(),
                new MissionJournalProfile(),
                new UnlockProfile(),
                new ClassProfile(),
                new MasteryProfile(),
                new MentorRelationshipProfile(),
                new SkillTreeProgressProfile(),
                bindingRegistry);
        }

        private static V2ProgressSnapshot BuildClassSkillTreeBindingSnapshot(
            params V2ClassSkillTreeBindingSnapshot[] bindings)
        {
            return new V2ProgressSnapshot(
                V2ProgressSnapshotVersions.CurrentSchemaVersion,
                new List<V2CodexEntrySnapshot>(),
                new List<V2MissionLeadSnapshot>(),
                new List<V2ActiveMissionSnapshot>(),
                new List<V2UnlockSnapshot>(),
                null,
                new List<V2MasterySnapshot>(),
                new List<V2MentorRelationshipSnapshot>(),
                new V2SkillTreeProgressSnapshot(
                    new List<string>(),
                    new List<string>()),
                bindings);
        }

        private static V2ClassSkillTreeBindingSnapshot FindClassSkillTreeBindingSnapshot(
            V2ProgressSnapshot snapshot,
            string classId)
        {
            foreach (var binding in snapshot.ClassSkillTreeBindings)
            {
                if (binding.ClassId == classId)
                {
                    return binding;
                }
            }

            return null;
        }

        private static SkillTreeProgressProfile BuildSkillTreeProgressWithUnlockedRoot(
            SkillTreeDefinition skillTree)
        {
            var progress = new SkillTreeProgressProfile();

            progress.DiscoverNode("node_deep_sight", skillTree);
            new SkillTreeUnlockApplicationService().Apply(
                "node_deep_sight",
                skillTree,
                progress,
                BuildTestSkillRegistry());
            progress.DiscoverNode("node_balance_ritual", skillTree);

            return progress;
        }

        private static V2ProgressSnapshot BuildUnknownSkillTreeProgressSnapshot()
        {
            return new V2ProgressSnapshot(
                V2ProgressSnapshotVersions.CurrentSchemaVersion,
                new List<V2CodexEntrySnapshot>(),
                new List<V2MissionLeadSnapshot>(),
                new List<V2ActiveMissionSnapshot>(),
                new List<V2UnlockSnapshot>(),
                null,
                new List<V2MasterySnapshot>(),
                new List<V2MentorRelationshipSnapshot>(),
                new V2SkillTreeProgressSnapshot(
                    new[] { "node_unknown" },
                    new List<string>()));
        }

        private static V2ProgressSnapshot BuildCompleteV4Snapshot(
            string classId,
            string masterySkillId,
            string mentorId,
            string skillTreeNodeId)
        {
            return new V2ProgressSnapshot(
                V2ProgressSnapshotVersions.CurrentSchemaVersion,
                new List<V2CodexEntrySnapshot>(),
                new List<V2MissionLeadSnapshot>(),
                new List<V2ActiveMissionSnapshot>(),
                new List<V2UnlockSnapshot>(),
                new V2ClassProfileSnapshot(classId),
                new[]
                {
                    new V2MasterySnapshot(
                        masterySkillId,
                        25,
                        MasteryRank.Practiced)
                },
                new[]
                {
                    new V2MentorRelationshipSnapshot(
                        mentorId,
                        true,
                        12,
                        true)
                },
                new V2SkillTreeProgressSnapshot(
                    new[] { skillTreeNodeId },
                    new[] { skillTreeNodeId }));
        }

        private static V4ProgressRestoreResult RestoreCompleteV4Snapshot(V2ProgressSnapshot snapshot)
        {
            return new V4ProgressRestoreCoordinator().Restore(
                snapshot,
                new ClassProfile(),
                new MasteryProfile(),
                new MentorRelationshipProfile(),
                new SkillTreeProgressProfile(),
                BuildTestClassRegistry(),
                BuildTestSkillRegistry(),
                BuildTestMentorRegistry(),
                BuildTestSkillTreeDefinition());
        }

        private static V2ProgressSnapshot BuildCompleteV4SnapshotWithBinding(
            string bindingClassId,
            string skillTreeId)
        {
            var snapshot = BuildCompleteV4Snapshot(
                ClassId.Tlamatini,
                "skill_deep_sight",
                MentorId.Lyra,
                "node_deep_sight");

            return new V2ProgressSnapshot(
                snapshot.SchemaVersion,
                snapshot.CodexEntries,
                snapshot.MissionLeads,
                snapshot.ActiveMissions,
                snapshot.Unlocks,
                snapshot.ClassProfile,
                snapshot.MasteryRecords,
                snapshot.MentorRelationships,
                snapshot.SkillTreeProgress,
                new[]
                {
                    new V2ClassSkillTreeBindingSnapshot(
                        bindingClassId,
                        skillTreeId)
                });
        }

        private static V4ProgressRestoreResult RestoreCompleteV4SnapshotWithBindings(
            V2ProgressSnapshot snapshot,
            ClassSkillTreeBindingRegistry bindingRegistry)
        {
            return new V4ProgressRestoreCoordinator().Restore(
                snapshot,
                new ClassProfile(),
                new MasteryProfile(),
                new MentorRelationshipProfile(),
                new SkillTreeProgressProfile(),
                BuildTestClassRegistry(),
                BuildTestSkillRegistry(),
                BuildTestMentorRegistry(),
                BuildTestSkillTreeDefinition(),
                bindingRegistry);
        }

        private static bool ContainsId(
            IReadOnlyList<string> ids,
            string id)
        {
            foreach (var candidateId in ids)
            {
                if (candidateId == id)
                {
                    return true;
                }
            }

            return false;
        }

        private static MasteryProfile BuildMasteryProfileWithProgress()
        {
            var profile = new MasteryProfile();
            var skillRegistry = BuildTestSkillRegistry();

            profile.IncreaseProgress("skill_deep_sight", 25, skillRegistry);
            profile.IncreaseProgress("skill_balance_ritual", MasteryRecord.MasteredProgress, skillRegistry);

            return profile;
        }

        private static SkillDefinition BuildTestSkillDefinition(
            string skillId,
            string name)
        {
            return new SkillDefinition(
                skillId,
                name,
                3,
                new SkillCost(10, 0),
                SkillCategory.Utility,
                SkillTargeting.Self,
                SkillEffectType.Cleanse,
                "");
        }

        private static SkillDefinition BuildValidatorSkillDefinition(
            string skillId,
            string requiredUnlockId)
        {
            return new SkillDefinition(
                skillId,
                "Validator Skill",
                3,
                new SkillCost(10, 0),
                SkillCategory.Combat,
                SkillTargeting.SingleEnemy,
                SkillEffectType.Damage,
                requiredUnlockId);
        }

        private class V4ValidatorCombatSkill : ICombatSkill
        {
            public string Id { get; private set; }

            public string Name
            {
                get { return "Validator Combat Skill"; }
            }

            public V4ValidatorCombatSkill(string id)
            {
                Id = id;
            }

            public void Apply(
                CombatContext context,
                SkillProgress progress)
            {
            }
        }
    }
}
