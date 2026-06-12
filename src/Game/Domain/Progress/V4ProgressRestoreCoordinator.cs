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
            bool classRestored = snapshotService.RestoreClassProfile(
                snapshot,
                classProfile,
                classRegistry);
            bool masteryRestored = snapshotService.RestoreMasteryProfile(
                snapshot,
                masteryProfile,
                skillRegistry);
            bool mentorRelationshipsRestored = snapshotService.RestoreMentorRelationshipProfile(
                snapshot,
                mentorRelationshipProfile,
                mentorRegistry);
            bool skillTreeProgressRestored = restoreClassSkillTreeBindings
                && snapshot.SkillTreeProgress.Scopes.Count > 0
                    ? snapshotService.RestoreSkillTreeProgressProfile(
                        snapshot,
                        skillTreeProgressProfile,
                        classSkillTreeBindingRegistry)
                    : snapshotService.RestoreSkillTreeProgressProfile(
                        snapshot,
                        skillTreeProgressProfile,
                        skillTree);
            bool classSkillTreeBindingsRestored = !restoreClassSkillTreeBindings
                || snapshotService.RestoreClassSkillTreeBindings(
                    snapshot,
                    classSkillTreeBindingRegistry,
                    classRegistry);

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
