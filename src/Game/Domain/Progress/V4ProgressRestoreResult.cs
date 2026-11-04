namespace Game.Domain.Progress
{
    /// <summary>Compatibility view of canonical restore facts; it owns no restore or result calculation rules.</summary>
    public class V4ProgressRestoreResult
    {
        private readonly ProgressRestoreResult result;

        public bool ClassRestored { get { return result.ClassRestored; } }
        public bool MasteryRestored { get { return result.MasteryRestored; } }
        public bool MentorRelationshipsRestored { get { return result.MentorRelationshipsRestored; } }
        public bool SkillTreeProgressRestored { get { return result.SkillTreeProgressRestored; } }
        public bool ClassSkillTreeBindingsRestored { get { return result.ClassSkillTreeBindingsRestored; } }
        public bool ClassSkillTreeBindingsRequested { get { return result.ClassSkillTreeBindingsRequested; } }
        public V4ProgressRestoreFailureReason ClassFailureReason
        { get { return (V4ProgressRestoreFailureReason)result.ClassFailureReason; } }
        public V4ProgressRestoreFailureReason MasteryFailureReason
        { get { return (V4ProgressRestoreFailureReason)result.MasteryFailureReason; } }
        public V4ProgressRestoreFailureReason MentorRelationshipsFailureReason
        { get { return (V4ProgressRestoreFailureReason)result.MentorRelationshipsFailureReason; } }
        public V4ProgressRestoreFailureReason SkillTreeProgressFailureReason
        { get { return (V4ProgressRestoreFailureReason)result.SkillTreeProgressFailureReason; } }
        public V4ProgressRestoreFailureReason ClassSkillTreeBindingsFailureReason
        { get { return (V4ProgressRestoreFailureReason)result.ClassSkillTreeBindingsFailureReason; } }
        public string ClassFailureMessage { get { return result.ClassFailureMessage; } }
        public string MasteryFailureMessage { get { return result.MasteryFailureMessage; } }
        public string MentorRelationshipsFailureMessage { get { return result.MentorRelationshipsFailureMessage; } }
        public string SkillTreeProgressFailureMessage { get { return result.SkillTreeProgressFailureMessage; } }
        public string ClassSkillTreeBindingsFailureMessage { get { return result.ClassSkillTreeBindingsFailureMessage; } }
        public bool FullyRestored { get { return result.FullyRestored; } }

        public V4ProgressRestoreResult(
            bool classRestored,
            bool masteryRestored,
            bool mentorRelationshipsRestored,
            bool skillTreeProgressRestored)
            : this(new ProgressRestoreResult(classRestored, masteryRestored,
                mentorRelationshipsRestored, skillTreeProgressRestored))
        {
        }

        public V4ProgressRestoreResult(
            bool classRestored,
            bool masteryRestored,
            bool mentorRelationshipsRestored,
            bool skillTreeProgressRestored,
            bool classSkillTreeBindingsRestored)
            : this(new ProgressRestoreResult(classRestored, masteryRestored,
                mentorRelationshipsRestored, skillTreeProgressRestored, classSkillTreeBindingsRestored))
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
            : this(new ProgressRestoreResult(classRestored, masteryRestored,
                mentorRelationshipsRestored, skillTreeProgressRestored,
                (ProgressRestoreFailureReason)classFailureReason,
                (ProgressRestoreFailureReason)masteryFailureReason,
                (ProgressRestoreFailureReason)mentorRelationshipsFailureReason,
                (ProgressRestoreFailureReason)skillTreeProgressFailureReason,
                classFailureMessage, masteryFailureMessage, mentorRelationshipsFailureMessage,
                skillTreeProgressFailureMessage))
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
            : this(new ProgressRestoreResult(classRestored, masteryRestored,
                mentorRelationshipsRestored, skillTreeProgressRestored, classSkillTreeBindingsRestored,
                (ProgressRestoreFailureReason)classFailureReason,
                (ProgressRestoreFailureReason)masteryFailureReason,
                (ProgressRestoreFailureReason)mentorRelationshipsFailureReason,
                (ProgressRestoreFailureReason)skillTreeProgressFailureReason,
                (ProgressRestoreFailureReason)classSkillTreeBindingsFailureReason,
                classFailureMessage, masteryFailureMessage, mentorRelationshipsFailureMessage,
                skillTreeProgressFailureMessage, classSkillTreeBindingsFailureMessage))
        {
        }

        internal V4ProgressRestoreResult(ProgressRestoreResult result)
        {
            this.result = result;
        }
    }
}
