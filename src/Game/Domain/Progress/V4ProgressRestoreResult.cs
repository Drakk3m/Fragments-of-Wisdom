namespace Game.Domain.Progress
{
    /// <summary>Restore operation completion facts, including successful no-ops, not a rollback guarantee.</summary>
    public class V4ProgressRestoreResult
    {
        public bool ClassRestored { get; private set; }
        public bool MasteryRestored { get; private set; }
        public bool MentorRelationshipsRestored { get; private set; }
        public bool SkillTreeProgressRestored { get; private set; }
        public bool ClassSkillTreeBindingsRestored { get; private set; }
        // Coordinator overload participation; legacy public constructors retain their binding convention.
        public bool ClassSkillTreeBindingsRequested { get; private set; }
        public V4ProgressRestoreFailureReason ClassFailureReason { get; private set; }
        public V4ProgressRestoreFailureReason MasteryFailureReason { get; private set; }
        public V4ProgressRestoreFailureReason MentorRelationshipsFailureReason { get; private set; }
        public V4ProgressRestoreFailureReason SkillTreeProgressFailureReason { get; private set; }
        public V4ProgressRestoreFailureReason ClassSkillTreeBindingsFailureReason { get; private set; }
        public string ClassFailureMessage { get; private set; }
        public string MasteryFailureMessage { get; private set; }
        public string MentorRelationshipsFailureMessage { get; private set; }
        public string SkillTreeProgressFailureMessage { get; private set; }
        public string ClassSkillTreeBindingsFailureMessage { get; private set; }

        public bool FullyRestored
        {
            get
            {
                return ClassRestored
                    && MasteryRestored
                    && MentorRelationshipsRestored
                    && SkillTreeProgressRestored
                    && (!ClassSkillTreeBindingsRequested || ClassSkillTreeBindingsRestored);
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
                true)
        {
        }

        public V4ProgressRestoreResult(
            bool classRestored,
            bool masteryRestored,
            bool mentorRelationshipsRestored,
            bool skillTreeProgressRestored,
            bool classSkillTreeBindingsRestored)
            : this(
                classRestored,
                masteryRestored,
                mentorRelationshipsRestored,
                skillTreeProgressRestored,
                classSkillTreeBindingsRestored,
                BuildReason(classRestored),
                BuildReason(masteryRestored),
                BuildReason(mentorRelationshipsRestored),
                BuildReason(skillTreeProgressRestored),
                BuildReason(classSkillTreeBindingsRestored),
                BuildMessage(classRestored, "ClassProfile"),
                BuildMessage(masteryRestored, "MasteryProfile"),
                BuildMessage(mentorRelationshipsRestored, "MentorRelationshipProfile"),
                BuildMessage(skillTreeProgressRestored, "SkillTreeProgressProfile"),
                BuildMessage(classSkillTreeBindingsRestored, "ClassSkillTreeBindings"))
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
            : this(
                classRestored,
                masteryRestored,
                mentorRelationshipsRestored,
                skillTreeProgressRestored,
                true,
                classFailureReason,
                masteryFailureReason,
                mentorRelationshipsFailureReason,
                skillTreeProgressFailureReason,
                V4ProgressRestoreFailureReason.None,
                classFailureMessage,
                masteryFailureMessage,
                mentorRelationshipsFailureMessage,
                skillTreeProgressFailureMessage,
                "")
        {
        }

        public V4ProgressRestoreResult(
            bool classRestored,
            bool masteryRestored,
            bool mentorRelationshipsRestored,
            bool skillTreeProgressRestored,
            bool classSkillTreeBindingsRestored,
            V4ProgressRestoreFailureReason classFailureReason,
            V4ProgressRestoreFailureReason masteryFailureReason,
            V4ProgressRestoreFailureReason mentorRelationshipsFailureReason,
            V4ProgressRestoreFailureReason skillTreeProgressFailureReason,
            V4ProgressRestoreFailureReason classSkillTreeBindingsFailureReason,
            string classFailureMessage,
            string masteryFailureMessage,
            string mentorRelationshipsFailureMessage,
            string skillTreeProgressFailureMessage,
            string classSkillTreeBindingsFailureMessage)
            : this(classRestored, masteryRestored, mentorRelationshipsRestored,
                skillTreeProgressRestored, classSkillTreeBindingsRestored,
                classFailureReason, masteryFailureReason, mentorRelationshipsFailureReason,
                skillTreeProgressFailureReason, classSkillTreeBindingsFailureReason,
                classFailureMessage, masteryFailureMessage, mentorRelationshipsFailureMessage,
                skillTreeProgressFailureMessage, classSkillTreeBindingsFailureMessage, true)
        {
        }

        internal V4ProgressRestoreResult(
            bool classRestored,
            bool masteryRestored,
            bool mentorRelationshipsRestored,
            bool skillTreeProgressRestored,
            bool classSkillTreeBindingsRestored,
            V4ProgressRestoreFailureReason classFailureReason,
            V4ProgressRestoreFailureReason masteryFailureReason,
            V4ProgressRestoreFailureReason mentorRelationshipsFailureReason,
            V4ProgressRestoreFailureReason skillTreeProgressFailureReason,
            V4ProgressRestoreFailureReason classSkillTreeBindingsFailureReason,
            string classFailureMessage,
            string masteryFailureMessage,
            string mentorRelationshipsFailureMessage,
            string skillTreeProgressFailureMessage,
            string classSkillTreeBindingsFailureMessage,
            bool classSkillTreeBindingsRequested)
        {
            ClassSkillTreeBindingsRequested = classSkillTreeBindingsRequested;
            ClassRestored = classRestored;
            MasteryRestored = masteryRestored;
            MentorRelationshipsRestored = mentorRelationshipsRestored;
            SkillTreeProgressRestored = skillTreeProgressRestored;
            ClassSkillTreeBindingsRestored = classSkillTreeBindingsRestored;
            ClassFailureReason = classFailureReason;
            MasteryFailureReason = masteryFailureReason;
            MentorRelationshipsFailureReason = mentorRelationshipsFailureReason;
            SkillTreeProgressFailureReason = skillTreeProgressFailureReason;
            ClassSkillTreeBindingsFailureReason = classSkillTreeBindingsFailureReason;
            ClassFailureMessage = classFailureMessage ?? "";
            MasteryFailureMessage = masteryFailureMessage ?? "";
            MentorRelationshipsFailureMessage = mentorRelationshipsFailureMessage ?? "";
            SkillTreeProgressFailureMessage = skillTreeProgressFailureMessage ?? "";
            ClassSkillTreeBindingsFailureMessage = classSkillTreeBindingsFailureMessage ?? "";
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
