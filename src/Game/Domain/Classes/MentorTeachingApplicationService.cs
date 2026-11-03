using Game.Domain.Skills;

namespace Game.Domain.Classes
{
    public class MentorTeachingApplicationService
    {
        public MentorTeachingApplicationResult Apply(
            MentorTeachingResult teachingResult,
            MasteryProfile masteryProfile,
            SkillDefinitionRegistry skillRegistry)
        {
            if (teachingResult == null || !teachingResult.IsAvailable)
            {
                return Blocked(
                    teachingResult == null ? "" : teachingResult.TaughtSkillId,
                    false,
                    MentorTeachingApplicationBlockReason.TeachingBlocked);
            }

            if (masteryProfile == null)
            {
                return Blocked(
                    teachingResult.TaughtSkillId,
                    false,
                    MentorTeachingApplicationBlockReason.MasteryProfileMissing);
            }

            if (skillRegistry == null || !skillRegistry.Contains(teachingResult.TaughtSkillId))
            {
                return Blocked(
                    teachingResult.TaughtSkillId,
                    false,
                    MentorTeachingApplicationBlockReason.UnknownSkill);
            }

            bool wasAlreadyMastered = masteryProfile.IsMastered(teachingResult.TaughtSkillId);

            if (wasAlreadyMastered)
            {
                return Blocked(
                    teachingResult.TaughtSkillId,
                    true,
                    MentorTeachingApplicationBlockReason.AlreadyMastered);
            }

            if (teachingResult.MasteryProgressGranted <= 0)
            {
                return Blocked(
                    teachingResult.TaughtSkillId,
                    false,
                    MentorTeachingApplicationBlockReason.NoProgressGranted);
            }

            var progress = masteryProfile.IncreaseProgressWithResult(
                teachingResult.TaughtSkillId,
                teachingResult.MasteryProgressGranted,
                skillRegistry);

            return new MentorTeachingApplicationResult(
                progress.Applied,
                teachingResult.TaughtSkillId,
                progress.AppliedProgress,
                wasAlreadyMastered,
                progress.Applied
                    ? MentorTeachingApplicationBlockReason.None
                    : MentorTeachingApplicationBlockReason.AlreadyMastered);
        }

        private static MentorTeachingApplicationResult Blocked(
            string skillId,
            bool wasAlreadyMastered,
            MentorTeachingApplicationBlockReason blockReason)
        {
            return new MentorTeachingApplicationResult(
                false,
                skillId,
                0,
                wasAlreadyMastered,
                blockReason);
        }
    }
}
