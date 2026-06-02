using System.Collections.Generic;
using System.Text.Json;
using Game.Domain.Classes;
using Game.Domain.Codex;
using Game.Domain.Missions;
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
                "Mastery foundation assertions",
                delegate
                {
                    AssertMasteryProgressIncreases();
                    AssertMasteryCanReachMasteredState();
                    AssertDuplicateMasteryApplicationIsSafe();
                    AssertMasteryProgressValidatesThroughRegistry();
                    AssertUnknownSkillMasteryFailsSafely();
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

        private static SkillDefinitionRegistry BuildTestSkillRegistry()
        {
            var registry = new SkillDefinitionRegistry();

            registry.Register(BuildTestSkillDefinition("skill_deep_sight", "Deep Sight"));
            registry.Register(BuildTestSkillDefinition("skill_balance_ritual", "Balance Ritual"));
            registry.Register(BuildTestSkillDefinition("skill_minor_cleanse", "Minor Cleanse"));

            return registry;
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
