namespace Game.Domain.Classes
{
    public class SkillTreeUnlockResult
    {
        public string NodeId { get; private set; }
        public string SkillId { get; private set; }
        public bool CanUnlock { get; private set; }
        public bool IsDiscovered { get; private set; }
        public bool IsAlreadyUnlocked { get; private set; }
        public string MissingPrerequisiteNodeId { get; private set; }
        public SkillTreeUnlockBlockReason BlockReason { get; private set; }

        public SkillTreeUnlockResult(
            string nodeId,
            string skillId,
            bool canUnlock,
            bool isDiscovered,
            bool isAlreadyUnlocked,
            string missingPrerequisiteNodeId,
            SkillTreeUnlockBlockReason blockReason)
        {
            NodeId = nodeId;
            SkillId = skillId;
            CanUnlock = canUnlock;
            IsDiscovered = isDiscovered;
            IsAlreadyUnlocked = isAlreadyUnlocked;
            MissingPrerequisiteNodeId = missingPrerequisiteNodeId;
            BlockReason = blockReason;
        }
    }
}
