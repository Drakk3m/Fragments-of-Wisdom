namespace Game.Domain.Classes
{
    public class SkillTreeUnlockApplicationResult
    {
        public bool Applied { get; private set; }
        public string NodeId { get; private set; }
        public SkillTreeUnlockBlockReason BlockReason { get; private set; }
        public bool WasAlreadyUnlocked { get; private set; }

        public SkillTreeUnlockApplicationResult(
            bool applied,
            string nodeId,
            SkillTreeUnlockBlockReason blockReason,
            bool wasAlreadyUnlocked)
        {
            Applied = applied;
            NodeId = nodeId;
            BlockReason = blockReason;
            WasAlreadyUnlocked = wasAlreadyUnlocked;
        }
    }
}
