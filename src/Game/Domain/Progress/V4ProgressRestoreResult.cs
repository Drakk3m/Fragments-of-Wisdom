namespace Game.Domain.Progress
{
    public class V4ProgressRestoreResult
    {
        public bool ClassRestored { get; private set; }
        public bool MasteryRestored { get; private set; }
        public bool MentorRelationshipsRestored { get; private set; }
        public bool SkillTreeProgressRestored { get; private set; }

        public bool FullyRestored
        {
            get
            {
                return ClassRestored
                    && MasteryRestored
                    && MentorRelationshipsRestored
                    && SkillTreeProgressRestored;
            }
        }

        public V4ProgressRestoreResult(
            bool classRestored,
            bool masteryRestored,
            bool mentorRelationshipsRestored,
            bool skillTreeProgressRestored)
        {
            ClassRestored = classRestored;
            MasteryRestored = masteryRestored;
            MentorRelationshipsRestored = mentorRelationshipsRestored;
            SkillTreeProgressRestored = skillTreeProgressRestored;
        }
    }
}
