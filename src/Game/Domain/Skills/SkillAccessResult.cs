namespace Game.Domain.Skills
{
    public class SkillAccessResult
    {
        public bool IsAvailable { get; private set; }
        public string BlockReason { get; private set; }
        public string RequiredUnlockId { get; private set; }

        public SkillAccessResult(
            bool isAvailable,
            string blockReason,
            string requiredUnlockId)
        {
            IsAvailable = isAvailable;
            BlockReason = blockReason;
            RequiredUnlockId = requiredUnlockId;
        }
    }
}
