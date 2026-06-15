using Game.Domain.Classes;
using Game.Domain.Codex;
using Game.Domain.Skills;

namespace Game.Domain.Combat
{
    public class CombatSkillTurnValidatorGateway : ICombatSkillTurnValidator
    {
        private readonly CombatSkillValidationOrchestrator orchestrator;
        private readonly bool useLegacyBypass;
        private readonly UnlockProfile unlockProfile;
        private readonly ClassProfile classProfile;
        private readonly MasteryProfile masteryProfile;
        private readonly ClassDefinitionRegistry classRegistry;
        private readonly SkillDefinitionRegistry skillRegistry;

        public CombatSkillTurnValidatorGateway()
            : this(
                new CombatSkillValidationOrchestrator(),
                true,
                null,
                null,
                null,
                null,
                null)
        {
        }

        public CombatSkillTurnValidatorGateway(bool useLegacyBypass)
            : this(
                new CombatSkillValidationOrchestrator(),
                useLegacyBypass,
                null,
                null,
                null,
                null,
                null)
        {
        }

        public CombatSkillTurnValidatorGateway(
            UnlockProfile unlockProfile,
            ClassProfile classProfile,
            MasteryProfile masteryProfile,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry)
            : this(
                new CombatSkillValidationOrchestrator(),
                false,
                unlockProfile,
                classProfile,
                masteryProfile,
                classRegistry,
                skillRegistry)
        {
        }

        public CombatSkillTurnValidatorGateway(
            CombatSkillValidationOrchestrator orchestrator,
            bool useLegacyBypass,
            UnlockProfile unlockProfile,
            ClassProfile classProfile,
            MasteryProfile masteryProfile,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry)
        {
            this.orchestrator = orchestrator
                ?? new CombatSkillValidationOrchestrator();
            this.useLegacyBypass = useLegacyBypass;
            this.unlockProfile = unlockProfile;
            this.classProfile = classProfile;
            this.masteryProfile = masteryProfile;
            this.classRegistry = classRegistry;
            this.skillRegistry = skillRegistry;
        }

        public CombatSkillTurnValidationResult Validate(
            TurnRequest request,
            CombatRuntimeContext runtime)
        {
            var result = orchestrator.Validate(
                request,
                request == null ? null : request.Attacker,
                runtime,
                useLegacyBypass,
                unlockProfile,
                classProfile,
                masteryProfile,
                classRegistry,
                skillRegistry);

            var classificationFailureReason = result.Classification == null
                ? CombatActorValidationClassificationFailureReason.ActorMissing
                : result.Classification.FailureReason;
            var blockReason = result.Validation == null
                ? CombatSkillUseBlockReason.None
                : result.Validation.BlockReason;
            var v3BlockReason = result.Validation == null
                ? string.Empty
                : result.Validation.V3BlockReason;
            var v4BlockReason = result.Validation == null
                ? MasteredSkillRetentionBlockReason.None
                : result.Validation.V4BlockReason;

            return new CombatSkillTurnValidationResult(
                result.IsAllowed,
                result.ValidationMode,
                result.FailureStage,
                classificationFailureReason,
                blockReason,
                v3BlockReason,
                v4BlockReason);
        }
    }
}
