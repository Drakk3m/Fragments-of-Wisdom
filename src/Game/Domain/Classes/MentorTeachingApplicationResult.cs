namespace Game.Domain.Classes
{
    public class MentorTeachingApplicationResult
    {
        public bool Applied { get; private set; }
        public string SkillId { get; private set; }
        public int ProgressApplied { get; private set; }
        public bool WasAlreadyMastered { get; private set; }
        public MentorTeachingApplicationBlockReason BlockReason { get; private set; }

        public MentorTeachingApplicationResult(
            bool applied,
            string skillId,
            int progressApplied,
            bool wasAlreadyMastered,
            MentorTeachingApplicationBlockReason blockReason)
        {
            Applied = applied;
            SkillId = skillId;
            ProgressApplied = progressApplied;
            WasAlreadyMastered = wasAlreadyMastered;
            BlockReason = blockReason;
        }
    }
}
