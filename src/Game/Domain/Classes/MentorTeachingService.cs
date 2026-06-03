using Game.Domain.Skills;

namespace Game.Domain.Classes
{
    public class MentorTeachingService
    {
        public const string InvalidTeaching = "invalid_teaching";
        public const string UnknownMentor = "unknown_mentor";
        public const string UnknownSkill = "unknown_skill";
        public const string TrustInsufficient = "trust_insufficient";

        public MentorTeachingResult Evaluate(
            MentorTeachingDefinition teaching,
            MentorRelationshipProfile relationshipProfile,
            MentorDefinitionRegistry mentorRegistry,
            SkillDefinitionRegistry skillRegistry)
        {
            if (teaching == null || !teaching.IsValid())
            {
                return Blocked(
                    teaching,
                    0,
                    InvalidTeaching);
            }

            if (mentorRegistry == null || !mentorRegistry.Contains(teaching.MentorId))
            {
                return Blocked(
                    teaching,
                    0,
                    UnknownMentor);
            }

            if (skillRegistry == null || !skillRegistry.Contains(teaching.TaughtSkillId))
            {
                return Blocked(
                    teaching,
                    0,
                    UnknownSkill);
            }

            int currentTrust = relationshipProfile == null
                ? 0
                : relationshipProfile.GetTrust(teaching.MentorId);

            if (relationshipProfile == null
                || !relationshipProfile.IsDiscovered(teaching.MentorId)
                || currentTrust < teaching.RequiredTrust)
            {
                return Blocked(
                    teaching,
                    currentTrust,
                    TrustInsufficient);
            }

            return new MentorTeachingResult(
                true,
                teaching.MentorId,
                teaching.TaughtSkillId,
                teaching.RequiredTrust,
                currentTrust,
                teaching.MasteryProgressGranted,
                "");
        }

        private static MentorTeachingResult Blocked(
            MentorTeachingDefinition teaching,
            int currentTrust,
            string blockReason)
        {
            return new MentorTeachingResult(
                false,
                teaching == null ? "" : teaching.MentorId,
                teaching == null ? "" : teaching.TaughtSkillId,
                teaching == null ? 0 : teaching.RequiredTrust,
                currentTrust,
                teaching == null ? 0 : teaching.MasteryProgressGranted,
                blockReason);
        }
    }
}
