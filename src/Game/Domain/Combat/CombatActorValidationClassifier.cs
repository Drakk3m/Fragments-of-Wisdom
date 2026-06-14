using Game.Domain.Classes;
using Game.Domain.Codex;
using Game.Domain.Skills;

namespace Game.Domain.Combat
{
    public class CombatActorValidationClassifier
    {
        public CombatActorValidationClassificationResult Classify(
            CombatEntity actor,
            CombatRuntimeContext runtime,
            bool useLegacyBypass,
            UnlockProfile unlockProfile,
            ClassProfile classProfile,
            MasteryProfile masteryProfile,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry)
        {
            if (useLegacyBypass)
            {
                return Classified(
                    CombatActorValidationKind.LegacyBypass,
                    CombatSkillUseValidationMode.LegacyBypass);
            }

            if (runtime == null)
            {
                return Failed(
                    CombatActorValidationClassificationFailureReason.RuntimeMissing);
            }

            if (actor == null)
            {
                return Failed(
                    CombatActorValidationClassificationFailureReason.ActorMissing);
            }

            if (actor == runtime.Enemy)
            {
                return Classified(
                    CombatActorValidationKind.Enemy,
                    CombatSkillUseValidationMode.Enemy);
            }

            if (actor == runtime.Player)
            {
                if (unlockProfile == null
                    || classProfile == null
                    || masteryProfile == null
                    || classRegistry == null
                    || skillRegistry == null)
                {
                    return Failed(
                        CombatActorValidationClassificationFailureReason
                            .IncompletePlayerDependencies);
                }

                return Classified(
                    CombatActorValidationKind.Player,
                    CombatSkillUseValidationMode.Player);
            }

            return Failed(
                CombatActorValidationClassificationFailureReason.UnknownActor);
        }

        private static CombatActorValidationClassificationResult Classified(
            CombatActorValidationKind kind,
            CombatSkillUseValidationMode mode)
        {
            return new CombatActorValidationClassificationResult(
                true,
                kind,
                mode,
                CombatActorValidationClassificationFailureReason.None);
        }

        private static CombatActorValidationClassificationResult Failed(
            CombatActorValidationClassificationFailureReason failureReason)
        {
            return new CombatActorValidationClassificationResult(
                false,
                CombatActorValidationKind.Unknown,
                null,
                failureReason);
        }
    }
}
