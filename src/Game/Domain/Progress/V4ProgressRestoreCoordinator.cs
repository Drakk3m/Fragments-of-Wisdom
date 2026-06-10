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
            bool skillTreeProgressRestored = snapshotService.RestoreSkillTreeProgressProfile(
                snapshot,
                skillTreeProgressProfile,
                skillTree);

            return new V4ProgressRestoreResult(
                classRestored,
                masteryRestored,
                mentorRelationshipsRestored,
                skillTreeProgressRestored);
        }
    }
}
