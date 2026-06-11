namespace Game.Domain.Progress
{
    public class V4ProgressRestoreResult
    {
        public bool ClassRestored { get; private set; }
        public bool MasteryRestored { get; private set; }
        public bool MentorRelationshipsRestored { get; private set; }
        public bool SkillTreeProgressRestored { get; private set; }
        public V4ProgressRestoreFailureReason ClassFailureReason { get; private set; }
        public V4ProgressRestoreFailureReason MasteryFailureReason { get; private set; }
        public V4ProgressRestoreFailureReason MentorRelationshipsFailureReason { get; private set; }
        public V4ProgressRestoreFailureReason SkillTreeProgressFailureReason { get; private set; }
        public string ClassFailureMessage { get; private set; }
        public string MasteryFailureMessage { get; private set; }
        public string MentorRelationshipsFailureMessage { get; private set; }
        public string SkillTreeProgressFailureMessage { get; private set; }

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
            : this(
                classRestored,
                masteryRestored,
                mentorRelationshipsRestored,
                skillTreeProgressRestored,
                BuildReason(classRestored),
                BuildReason(masteryRestored),
                BuildReason(mentorRelationshipsRestored),
                BuildReason(skillTreeProgressRestored),
                BuildMessage(classRestored, "ClassProfile"),
                BuildMessage(masteryRestored, "MasteryProfile"),
                BuildMessage(mentorRelationshipsRestored, "MentorRelationshipProfile"),
                BuildMessage(skillTreeProgressRestored, "SkillTreeProgressProfile"))
        {
        }

        public V4ProgressRestoreResult(
            bool classRestored,
            bool masteryRestored,
            bool mentorRelationshipsRestored,
            bool skillTreeProgressRestored,
            V4ProgressRestoreFailureReason classFailureReason,
            V4ProgressRestoreFailureReason masteryFailureReason,
            V4ProgressRestoreFailureReason mentorRelationshipsFailureReason,
            V4ProgressRestoreFailureReason skillTreeProgressFailureReason,
            string classFailureMessage,
            string masteryFailureMessage,
            string mentorRelationshipsFailureMessage,
            string skillTreeProgressFailureMessage)
        {
            ClassRestored = classRestored;
            MasteryRestored = masteryRestored;
            MentorRelationshipsRestored = mentorRelationshipsRestored;
            SkillTreeProgressRestored = skillTreeProgressRestored;
            ClassFailureReason = classFailureReason;
            MasteryFailureReason = masteryFailureReason;
            MentorRelationshipsFailureReason = mentorRelationshipsFailureReason;
            SkillTreeProgressFailureReason = skillTreeProgressFailureReason;
            ClassFailureMessage = classFailureMessage ?? "";
            MasteryFailureMessage = masteryFailureMessage ?? "";
            MentorRelationshipsFailureMessage = mentorRelationshipsFailureMessage ?? "";
            SkillTreeProgressFailureMessage = skillTreeProgressFailureMessage ?? "";
        }

        private static V4ProgressRestoreFailureReason BuildReason(bool restored)
        {
            return restored
                ? V4ProgressRestoreFailureReason.None
                : V4ProgressRestoreFailureReason.ValidationFailed;
        }

        private static string BuildMessage(
            bool restored,
            string subsystemName)
        {
            return restored
                ? ""
                : subsystemName + " failed registry or snapshot validation during restore.";
        }
    }
}
