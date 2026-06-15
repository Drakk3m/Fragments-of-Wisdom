using Game.Domain.Classes;
using Game.Domain.Common;
using Game.Domain.Codex;
using Game.Domain.Combat.Passives;
using Game.Domain.Skills;

namespace Game.Domain.Combat
{
    public static class CombatSkillTurnValidationComposition
    {
        public static ICombatSkillTurnValidator CreateValidator(
            UnlockProfile unlockProfile,
            ClassProfile classProfile,
            MasteryProfile masteryProfile,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry)
        {
            return new CombatSkillTurnValidatorGateway(
                unlockProfile,
                classProfile,
                masteryProfile,
                classRegistry,
                skillRegistry);
        }

        public static TurnBasedCombatEngine CreateProgressionAwareEngine(
            IRandomSource random,
            CombatPassivePipeline pipeline,
            UnlockProfile unlockProfile,
            ClassProfile classProfile,
            MasteryProfile masteryProfile,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry)
        {
            return new TurnBasedCombatEngine(
                random,
                pipeline,
                CreateValidator(
                    unlockProfile,
                    classProfile,
                    masteryProfile,
                    classRegistry,
                    skillRegistry));
        }
    }
}
