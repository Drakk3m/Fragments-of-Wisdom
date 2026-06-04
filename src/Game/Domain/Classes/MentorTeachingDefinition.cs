namespace Game.Domain.Classes
{
    public class MentorTeachingDefinition
    {
        public string MentorId { get; private set; }
        public string TaughtSkillId { get; private set; }
        public int RequiredTrust { get; private set; }
        public int MasteryProgressGranted { get; private set; }

        public MentorTeachingDefinition(
            string mentorId,
            string taughtSkillId,
            int requiredTrust,
            int masteryProgressGranted)
        {
            MentorId = mentorId;
            TaughtSkillId = taughtSkillId;
            RequiredTrust = requiredTrust;
            MasteryProgressGranted = masteryProgressGranted;
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(MentorId)
                && !string.IsNullOrWhiteSpace(TaughtSkillId)
                && RequiredTrust >= 0
                && MasteryProgressGranted >= 0;
        }
    }
}
