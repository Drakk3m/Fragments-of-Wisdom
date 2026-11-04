using Game.Domain.Classes;
using Game.Domain.Skills;

namespace Game.Domain.Progress
{
    /// <summary>Compatibility entry point. New callers should use ProgressRestoreCoordinator.</summary>
    public class V4ProgressRestoreCoordinator
    {
        private readonly ProgressRestoreCoordinator coordinator;

        public V4ProgressRestoreCoordinator()
            : this(new V2ProgressSnapshotService())
        {
        }

        public V4ProgressRestoreCoordinator(V2ProgressSnapshotService snapshotService)
        {
            coordinator = new ProgressRestoreCoordinator(snapshotService);
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
            return new V4ProgressRestoreResult(coordinator.Restore(snapshot, classProfile, masteryProfile,
                mentorRelationshipProfile, skillTreeProgressProfile, classRegistry, skillRegistry,
                mentorRegistry, skillTree));
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
            return new V4ProgressRestoreResult(coordinator.Restore(snapshot, classProfile, masteryProfile,
                mentorRelationshipProfile, skillTreeProgressProfile, classRegistry, skillRegistry,
                mentorRegistry, skillTree, classSkillTreeBindingRegistry));
        }
    }
}
