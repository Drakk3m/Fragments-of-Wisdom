using Game.Domain.Classes;
using Game.Domain.Codex;
using Game.Domain.Skills;

namespace Game.Domain.Combat
{
    public class CombatSkillUseValidationContextProvider
    {
        public CombatSkillUseValidationContext CreateLegacyBypass()
        {
            return CombatSkillUseValidationContext.LegacyBypass();
        }

        public CombatSkillUseValidationContext CreateEnemy()
        {
            return CombatSkillUseValidationContext.Enemy();
        }

        public bool TryCreatePlayer(
            UnlockProfile unlockProfile,
            ClassProfile classProfile,
            MasteryProfile masteryProfile,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry,
            out CombatSkillUseValidationContext context)
        {
            if (unlockProfile == null
                || classProfile == null
                || masteryProfile == null
                || classRegistry == null
                || skillRegistry == null)
            {
                context = null;
                return false;
            }

            context = CombatSkillUseValidationContext.Player(
                unlockProfile,
                classProfile,
                masteryProfile,
                classRegistry,
                skillRegistry);
            return true;
        }
    }
}
