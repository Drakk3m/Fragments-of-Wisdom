using System.Collections.Generic;
using System.Text.Json;
using Game.Domain.Classes;
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
                    AssertSkillTreeNodeWithUnknownSkillIdIsRejected();
                    AssertSkillTreePrerequisiteBlocksUnlockUntilMet();
                    AssertUnlockedPrerequisiteAllowsNextNode();
                    AssertDuplicateSkillTreeNodeIdIsRejectedSafely();
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
                    AssertSkillTreeProgressionIntegratedWorkflow();
                    AssertClassSkillTreeBindingIntegratedWorkflow();
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

            progress.DiscoverNode("node_deep_sight", skillTree);
            progress.UnlockNode("node_deep_sight", skillTree);
            progress.DiscoverNode("node_balance_ritual", skillTree);

            var result = new SkillTreeUnlockPolicy().Evaluate(
                "node_balance_ritual",
                skillTree,
                progress,
                BuildTestSkillRegistry());

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

            var rootResult = unlockPolicy.Evaluate(
                rootNodeId,
                skillTree,
                progress,
                skillRegistry);

            Assert(
                rootResult.CanUnlock
                && progress.UnlockNode(rootNodeId, skillTree)
                && progress.IsUnlocked(rootNodeId),
                "Skill tree workflow should unlock the first node.");

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

            var prerequisiteResult = unlockPolicy.Evaluate(
                prerequisiteNodeId,
                skillTree,
                progress,
                skillRegistry);

            Assert(
                prerequisiteResult.CanUnlock
                && progress.UnlockNode(prerequisiteNodeId, skillTree)
                && progress.IsUnlocked(prerequisiteNodeId),
                "Skill tree workflow should unlock the prerequisite node.");

            var dependentResult = unlockPolicy.Evaluate(
                dependentNodeId,
                skillTree,
                progress,
                skillRegistry);

            Assert(
                dependentResult.CanUnlock
                && progress.UnlockNode(dependentNodeId, skillTree)
                && progress.IsUnlocked(dependentNodeId),
                "Skill tree workflow should allow and unlock the dependent node after prerequisite unlock.");

            Assert(
                !skillBook.HasSkill(rootSkillId)
                && !skillBook.HasSkill(prerequisiteSkillId)
                && !skillBook.HasSkill(dependentSkillId),
                "Skill tree workflow should not automatically grant skills.");

            Assert(
                masteryProfile.Records.Count == 0,
                "Skill tree workflow should leave mastery profile unchanged.");
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

        private static SkillTreeProgressProfile BuildSkillTreeProgressWithUnlockedRoot(
            SkillTreeDefinition skillTree)
        {
            var progress = new SkillTreeProgressProfile();

            progress.DiscoverNode("node_deep_sight", skillTree);
            progress.UnlockNode("node_deep_sight", skillTree);
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
    }
}
