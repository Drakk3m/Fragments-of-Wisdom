using Game.Domain.Skills;

namespace Game.Domain.Classes
{
    public class MentorTeachingService
    {
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
                    MentorTeachingBlockReason.InvalidTeaching,
                    "Mentor teaching definition is invalid.");
            }

            if (mentorRegistry == null || !mentorRegistry.Contains(teaching.MentorId))
            {
                return Blocked(
                    teaching,
                    0,
                    MentorTeachingBlockReason.UnknownMentor,
                    "Mentor id is not registered.");
            }

            if (skillRegistry == null || !skillRegistry.Contains(teaching.TaughtSkillId))
            {
                return Blocked(
                    teaching,
                    0,
                    MentorTeachingBlockReason.UnknownSkill,
                    "Taught skill id is not registered.");
            }

            int currentTrust = relationshipProfile == null
                ? 0
                : relationshipProfile.GetTrust(teaching.MentorId);

            if (relationshipProfile == null
                || !relationshipProfile.IsDiscovered(teaching.MentorId))
            {
                return Blocked(
                    teaching,
                    currentTrust,
                    MentorTeachingBlockReason.MentorNotDiscovered,
                    "Mentor has not been discovered.");
            }

            if (currentTrust < teaching.RequiredTrust)
            {
                return Blocked(
                    teaching,
                    currentTrust,
                    MentorTeachingBlockReason.TrustInsufficient,
                    "Mentor trust requirement is not met.");
            }

            return new MentorTeachingResult(
                true,
                teaching.MentorId,
                teaching.TaughtSkillId,
                teaching.RequiredTrust,
                currentTrust,
                teaching.MasteryProgressGranted,
                MentorTeachingBlockReason.None,
                "");
        }

        private static MentorTeachingResult Blocked(
            MentorTeachingDefinition teaching,
            int currentTrust,
            MentorTeachingBlockReason blockReason,
            string blockMessage)
        {
            return new MentorTeachingResult(
                false,
                teaching == null ? "" : teaching.MentorId,
                teaching == null ? "" : teaching.TaughtSkillId,
                teaching == null ? 0 : teaching.RequiredTrust,
                currentTrust,
                teaching == null ? 0 : teaching.MasteryProgressGranted,
                blockReason,
                blockMessage);
        }
    }
}
