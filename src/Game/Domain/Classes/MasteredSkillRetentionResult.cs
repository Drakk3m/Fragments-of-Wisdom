namespace Game.Domain.Classes
{
    public class MasteredSkillRetentionResult
    {
        public string SkillId { get; private set; }
        public bool IsUsable { get; private set; }
        public bool AllowedByCurrentClass { get; private set; }
        public bool AllowedByMastery { get; private set; }
        public MasteredSkillRetentionBlockReason BlockReason { get; private set; }

        public MasteredSkillRetentionResult(
            string skillId,
            bool isUsable,
            bool allowedByCurrentClass,
            bool allowedByMastery,
            MasteredSkillRetentionBlockReason blockReason)
        {
            SkillId = skillId;
            IsUsable = isUsable;
            AllowedByCurrentClass = allowedByCurrentClass;
            AllowedByMastery = allowedByMastery;
            BlockReason = blockReason;
        }
    }
}
