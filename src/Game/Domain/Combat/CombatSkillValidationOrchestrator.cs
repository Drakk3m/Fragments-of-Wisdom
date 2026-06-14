using Game.Domain.Classes;
using Game.Domain.Codex;
using Game.Domain.Skills;

namespace Game.Domain.Combat
{
    public class CombatSkillValidationOrchestrator
    {
        private readonly CombatActorValidationClassifier classifier;
        private readonly CombatSkillUseValidationContextProvider contextProvider;
        private readonly CombatSkillUseValidator validator;

        public CombatSkillValidationOrchestrator()
            : this(
                new CombatActorValidationClassifier(),
                new CombatSkillUseValidationContextProvider(),
                new CombatSkillUseValidator())
        {
        }

        public CombatSkillValidationOrchestrator(
            CombatActorValidationClassifier classifier,
            CombatSkillUseValidationContextProvider contextProvider,
            CombatSkillUseValidator validator)
        {
            this.classifier = classifier ?? new CombatActorValidationClassifier();
            this.contextProvider = contextProvider
                ?? new CombatSkillUseValidationContextProvider();
            this.validator = validator ?? new CombatSkillUseValidator();
        }

        public CombatSkillValidationOrchestrationResult Validate(
            TurnRequest request,
            CombatEntity actor,
            CombatRuntimeContext runtime,
            bool useLegacyBypass,
            UnlockProfile unlockProfile,
            ClassProfile classProfile,
            MasteryProfile masteryProfile,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry)
        {
            var classification = classifier.Classify(
                actor,
                runtime,
                useLegacyBypass,
                unlockProfile,
                classProfile,
                masteryProfile,
                classRegistry,
                skillRegistry);

            if (!classification.IsClassified
                || !classification.ValidationMode.HasValue)
            {
                return Failed(
                    classification,
                    false,
                    null,
                    CombatSkillValidationOrchestrationFailureStage.Classification);
            }

            CombatSkillUseValidationContext context;

            switch (classification.ValidationMode.Value)
            {
                case CombatSkillUseValidationMode.LegacyBypass:
                    context = contextProvider.CreateLegacyBypass();
                    break;
                case CombatSkillUseValidationMode.Enemy:
                    context = contextProvider.CreateEnemy();
                    break;
                case CombatSkillUseValidationMode.Player:
                    if (!contextProvider.TryCreatePlayer(
                        unlockProfile,
                        classProfile,
                        masteryProfile,
                        classRegistry,
                        skillRegistry,
                        out context))
                    {
                        return Failed(
                            classification,
                            false,
                            classification.ValidationMode,
                            CombatSkillValidationOrchestrationFailureStage.ContextCreation);
                    }

                    break;
                default:
                    return Failed(
                        classification,
                        false,
                        classification.ValidationMode,
                        CombatSkillValidationOrchestrationFailureStage.ContextCreation);
            }

            var validation = validator.Validate(request, context);

            return new CombatSkillValidationOrchestrationResult(
                true,
                validation.IsAllowed,
                true,
                context.Mode,
                classification,
                validation,
                validation.IsAllowed
                    ? CombatSkillValidationOrchestrationFailureStage.None
                    : CombatSkillValidationOrchestrationFailureStage.Validation);
        }

        private static CombatSkillValidationOrchestrationResult Failed(
            CombatActorValidationClassificationResult classification,
            bool contextCreated,
            CombatSkillUseValidationMode? validationMode,
            CombatSkillValidationOrchestrationFailureStage failureStage)
        {
            return new CombatSkillValidationOrchestrationResult(
                false,
                false,
                contextCreated,
                validationMode,
                classification,
                null,
                failureStage);
        }
    }
}
