using System;
using Game.Domain.Classes;
using Game.Domain.Skills;

namespace Game.Domain.Progress
{
    public class V4ProgressRestoreCoordinator
    {
        private readonly V2ProgressSnapshotService snapshotService;

        public V4ProgressRestoreCoordinator()
            : this(new V2ProgressSnapshotService())
        {
        }

        public V4ProgressRestoreCoordinator(V2ProgressSnapshotService snapshotService)
        {
            if (snapshotService == null)
            {
                throw new ArgumentNullException(nameof(snapshotService));
            }

            this.snapshotService = snapshotService;
        }

        public V4ProgressRestoreResult Restore(
            V2ProgressSnapshot snapshot,
            ClassProfile classProfile,
            MasteryProfile masteryProfile,
            MentorRelationshipProfile mentorRelationshipProfile,
            SkillTreeProgressProfile skillTreeProgressProfile,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry,
            MentorDefinitionRegistry mentorRegistry,
            SkillTreeDefinition skillTree)
        {
            return RestoreInternal(
                snapshot,
                classProfile,
                masteryProfile,
                mentorRelationshipProfile,
                skillTreeProgressProfile,
                classRegistry,
                skillRegistry,
                mentorRegistry,
                skillTree,
                null,
                false);
        }

        public V4ProgressRestoreResult Restore(
            V2ProgressSnapshot snapshot,
            ClassProfile classProfile,
            MasteryProfile masteryProfile,
            MentorRelationshipProfile mentorRelationshipProfile,
            SkillTreeProgressProfile skillTreeProgressProfile,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry,
            MentorDefinitionRegistry mentorRegistry,
            SkillTreeDefinition skillTree,
            ClassSkillTreeBindingRegistry classSkillTreeBindingRegistry)
        {
            if (classSkillTreeBindingRegistry == null)
            {
                throw new ArgumentNullException(nameof(classSkillTreeBindingRegistry));
            }

            return RestoreInternal(
                snapshot,
                classProfile,
                masteryProfile,
                mentorRelationshipProfile,
                skillTreeProgressProfile,
                classRegistry,
                skillRegistry,
                mentorRegistry,
                skillTree,
                classSkillTreeBindingRegistry,
                true);
        }

        private V4ProgressRestoreResult RestoreInternal(
            V2ProgressSnapshot snapshot,
            ClassProfile classProfile,
            MasteryProfile masteryProfile,
            MentorRelationshipProfile mentorRelationshipProfile,
            SkillTreeProgressProfile skillTreeProgressProfile,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry,
            MentorDefinitionRegistry mentorRegistry,
            SkillTreeDefinition skillTree,
            ClassSkillTreeBindingRegistry classSkillTreeBindingRegistry,
            bool restoreClassSkillTreeBindings)
        {
            // Temporary-profile preflight cannot validate the real destination references.
            if (classProfile == null)
            {
                throw new ArgumentNullException(nameof(classProfile));
            }

            if (masteryProfile == null)
            {
                throw new ArgumentNullException(nameof(masteryProfile));
            }

            if (mentorRelationshipProfile == null)
            {
                throw new ArgumentNullException(nameof(mentorRelationshipProfile));
            }

            if (skillTreeProgressProfile == null)
            {
                throw new ArgumentNullException(nameof(skillTreeProgressProfile));
            }

            var preflightBindingRegistry = restoreClassSkillTreeBindings
                ? BuildPreflightBindingRegistry(
                    classSkillTreeBindingRegistry,
                    classRegistry)
                : null;
            bool classRestored = snapshotService.RestoreClassProfile(
                snapshot,
                new ClassProfile(),
                classRegistry);
            bool masteryRestored = snapshotService.RestoreMasteryProfile(
                snapshot,
                new MasteryProfile(),
                skillRegistry);
            bool mentorRelationshipsRestored = snapshotService.RestoreMentorRelationshipProfile(
                snapshot,
                new MentorRelationshipProfile(),
                mentorRegistry);
            bool skillTreeProgressRestored = restoreClassSkillTreeBindings
                && snapshot.SkillTreeProgress.Scopes.Count > 0
                    ? snapshotService.RestoreSkillTreeProgressProfile(
                        snapshot,
                        new SkillTreeProgressProfile(),
                        classSkillTreeBindingRegistry)
                    : snapshotService.RestoreSkillTreeProgressProfile(
                        snapshot,
                        new SkillTreeProgressProfile(),
                        skillTree);
            bool classSkillTreeBindingsRestored = !restoreClassSkillTreeBindings
                || snapshotService.RestoreClassSkillTreeBindings(
                    snapshot,
                    preflightBindingRegistry,
                    classRegistry);

            var preflightResult = BuildResult(
                classRestored,
                masteryRestored,
                mentorRelationshipsRestored,
                skillTreeProgressRestored,
                classSkillTreeBindingsRestored);

            if (!preflightResult.FullyRestored)
            {
                return preflightResult;
            }

            snapshotService.RestoreClassProfile(snapshot, classProfile, classRegistry);
            snapshotService.RestoreMasteryProfile(snapshot, masteryProfile, skillRegistry);
            snapshotService.RestoreMentorRelationshipProfile(
                snapshot,
                mentorRelationshipProfile,
                mentorRegistry);

            if (restoreClassSkillTreeBindings
                && snapshot.SkillTreeProgress.Scopes.Count > 0)
            {
                snapshotService.RestoreSkillTreeProgressProfile(
                    snapshot,
                    skillTreeProgressProfile,
                    classSkillTreeBindingRegistry);
            }
            else
            {
                snapshotService.RestoreSkillTreeProgressProfile(
                    snapshot,
                    skillTreeProgressProfile,
                    skillTree);
            }

            if (restoreClassSkillTreeBindings)
            {
                snapshotService.RestoreClassSkillTreeBindings(
                    snapshot,
                    classSkillTreeBindingRegistry,
                    classRegistry);
            }

            return preflightResult;
        }

        private static ClassSkillTreeBindingRegistry BuildPreflightBindingRegistry(
            ClassSkillTreeBindingRegistry sourceRegistry,
            ClassDefinitionRegistry classRegistry)
        {
            var preflightRegistry = new ClassSkillTreeBindingRegistry();

            foreach (var skillTree in sourceRegistry.SkillTrees.Values)
            {
                preflightRegistry.RegisterSkillTree(skillTree);
            }

            foreach (var binding in sourceRegistry.Bindings.Values)
            {
                preflightRegistry.RegisterBinding(binding, classRegistry);
            }

            return preflightRegistry;
        }

        private static V4ProgressRestoreResult BuildResult(
            bool classRestored,
            bool masteryRestored,
            bool mentorRelationshipsRestored,
            bool skillTreeProgressRestored,
            bool classSkillTreeBindingsRestored)
        {
            return new V4ProgressRestoreResult(
                classRestored,
                masteryRestored,
                mentorRelationshipsRestored,
                skillTreeProgressRestored,
                classSkillTreeBindingsRestored,
                BuildReason(classRestored),
                BuildReason(masteryRestored),
                BuildReason(mentorRelationshipsRestored),
                BuildReason(skillTreeProgressRestored),
                BuildReason(classSkillTreeBindingsRestored),
                BuildMessage(classRestored, "ClassProfile"),
                BuildMessage(masteryRestored, "MasteryProfile"),
                BuildMessage(mentorRelationshipsRestored, "MentorRelationshipProfile"),
                BuildMessage(skillTreeProgressRestored, "SkillTreeProgressProfile"),
                BuildMessage(classSkillTreeBindingsRestored, "ClassSkillTreeBindings"));
        }

        private static V4ProgressRestoreFailureReason BuildReason(bool restored)
        {
            return restored
                ? V4ProgressRestoreFailureReason.None
                : V4ProgressRestoreFailureReason.ValidationFailed;
        }

        private static string BuildMessage(
            bool restored,
            string subsystemName)
        {
            return restored
                ? ""
                : subsystemName + " failed registry or snapshot validation during restore.";
        }
    }
}
