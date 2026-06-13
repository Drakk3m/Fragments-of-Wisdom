using Game.Domain.Classes;
using Game.Domain.Skills;

namespace Game.Domain.Combat
{
    public class CombatSkillAccessService
    {
        private readonly MasteredSkillRetentionPolicy retentionPolicy;

        public CombatSkillAccessService()
            : this(new MasteredSkillRetentionPolicy())
        {
        }

        public CombatSkillAccessService(MasteredSkillRetentionPolicy retentionPolicy)
        {
            this.retentionPolicy = retentionPolicy ?? new MasteredSkillRetentionPolicy();
        }

        public CombatSkillAccessResult Evaluate(
            string skillId,
            ClassProfile classProfile,
            ClassDefinitionRegistry classRegistry,
            MasteryProfile masteryProfile,
            SkillDefinitionRegistry skillRegistry)
        {
            var retentionResult = retentionPolicy.Evaluate(
                skillId,
                classProfile,
                classRegistry,
                masteryProfile,
                skillRegistry);

            return new CombatSkillAccessResult(
                retentionResult.SkillId,
                retentionResult.IsUsable,
                retentionResult.AllowedByCurrentClass,
                retentionResult.AllowedByMastery,
                retentionResult.BlockReason);
        }
    }
}
