using Game.Domain.Classes;
using Game.Domain.Codex;
using Game.Domain.Skills;

namespace Game.Domain.Combat
{
    public class CombatSkillUseValidationContext
    {
        public CombatSkillUseValidationMode Mode { get; private set; }
        public UnlockProfile UnlockProfile { get; private set; }
        public ClassProfile ClassProfile { get; private set; }
        public MasteryProfile MasteryProfile { get; private set; }
        public ClassDefinitionRegistry ClassRegistry { get; private set; }
        public SkillDefinitionRegistry SkillRegistry { get; private set; }

        private CombatSkillUseValidationContext(
            CombatSkillUseValidationMode mode,
            UnlockProfile unlockProfile,
            ClassProfile classProfile,
            MasteryProfile masteryProfile,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry)
        {
            Mode = mode;
            UnlockProfile = unlockProfile;
            ClassProfile = classProfile;
            MasteryProfile = masteryProfile;
            ClassRegistry = classRegistry;
            SkillRegistry = skillRegistry;
        }

        public static CombatSkillUseValidationContext LegacyBypass()
        {
            return new CombatSkillUseValidationContext(
                CombatSkillUseValidationMode.LegacyBypass,
                null,
                null,
                null,
                null,
                null);
        }

        public static CombatSkillUseValidationContext Enemy()
        {
            return new CombatSkillUseValidationContext(
                CombatSkillUseValidationMode.Enemy,
                null,
                null,
                null,
                null,
                null);
        }

        public static CombatSkillUseValidationContext Player(
            UnlockProfile unlockProfile,
            ClassProfile classProfile,
            MasteryProfile masteryProfile,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry)
        {
            return new CombatSkillUseValidationContext(
                CombatSkillUseValidationMode.Player,
                unlockProfile,
                classProfile,
                masteryProfile,
                classRegistry,
                skillRegistry);
        }

        public bool HasCompletePlayerState
        {
            get
            {
                return UnlockProfile != null
                    && ClassProfile != null
                    && MasteryProfile != null
                    && ClassRegistry != null
                    && SkillRegistry != null;
            }
        }
    }
}
