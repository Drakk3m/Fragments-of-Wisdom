namespace Game.Domain.Classes
{
    public class MentorTeachingResult
    {
        public bool IsAvailable { get; private set; }
        public string MentorId { get; private set; }
        public string TaughtSkillId { get; private set; }
        public int RequiredTrust { get; private set; }
        public int CurrentTrust { get; private set; }
        public int MasteryProgressGranted { get; private set; }
        public string BlockReason { get; private set; }

        public MentorTeachingResult(
            bool isAvailable,
            string mentorId,
            string taughtSkillId,
            int requiredTrust,
            int currentTrust,
            int masteryProgressGranted,
            string blockReason)
        {
            IsAvailable = isAvailable;
            MentorId = mentorId;
            TaughtSkillId = taughtSkillId;
            RequiredTrust = requiredTrust;
            CurrentTrust = currentTrust;
            MasteryProgressGranted = masteryProgressGranted;
            BlockReason = blockReason;
        }
    }
}
