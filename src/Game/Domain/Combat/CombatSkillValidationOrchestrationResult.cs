namespace Game.Domain.Combat
{
    public class CombatSkillValidationOrchestrationResult
    {
        public bool IsOrchestrated { get; private set; }
        public bool IsAllowed { get; private set; }
        public bool ContextCreated { get; private set; }
        public CombatSkillUseValidationMode? ValidationMode { get; private set; }
        public CombatActorValidationClassificationResult Classification { get; private set; }
        public CombatSkillUseValidationResult Validation { get; private set; }
        public CombatSkillValidationOrchestrationFailureStage FailureStage { get; private set; }

        public CombatSkillValidationOrchestrationResult(
            bool isOrchestrated,
            bool isAllowed,
            bool contextCreated,
            CombatSkillUseValidationMode? validationMode,
            CombatActorValidationClassificationResult classification,
            CombatSkillUseValidationResult validation,
            CombatSkillValidationOrchestrationFailureStage failureStage)
        {
            IsOrchestrated = isOrchestrated;
            IsAllowed = isAllowed;
            ContextCreated = contextCreated;
            ValidationMode = validationMode;
            Classification = classification;
            Validation = validation;
            FailureStage = failureStage;
        }
    }
}
