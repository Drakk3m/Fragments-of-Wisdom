using Game.Domain.Skills;

namespace Game.Domain.Classes
{
    public class MasteredSkillRetentionPolicy
    {
        public MasteredSkillRetentionResult Evaluate(
            string skillId,
            ClassProfile classProfile,
            ClassDefinitionRegistry classRegistry,
            MasteryProfile masteryProfile,
            SkillDefinitionRegistry skillRegistry)
        {
            if (skillRegistry == null || !skillRegistry.Contains(skillId))
            {
                return new MasteredSkillRetentionResult(
                    skillId,
                    false,
                    false,
                    false,
                    MasteredSkillRetentionBlockReason.UnknownSkill);
            }

            bool allowedByCurrentClass = classProfile != null
                && classRegistry != null
                && classProfile.CurrentClassAllowsSkill(
                    skillId,
                    classRegistry,
                    skillRegistry);
            bool allowedByMastery = masteryProfile != null
                && masteryProfile.IsMastered(skillId);
            bool isUsable = allowedByCurrentClass || allowedByMastery;

            return new MasteredSkillRetentionResult(
                skillId,
                isUsable,
                allowedByCurrentClass,
                allowedByMastery,
                isUsable
                    ? MasteredSkillRetentionBlockReason.None
                    : MasteredSkillRetentionBlockReason.SkillNotAllowed);
        }
    }
}
