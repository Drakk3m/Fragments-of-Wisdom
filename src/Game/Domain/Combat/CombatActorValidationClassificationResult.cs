namespace Game.Domain.Combat
{
    public class CombatActorValidationClassificationResult
    {
        public bool IsClassified { get; private set; }
        public CombatActorValidationKind Kind { get; private set; }
        public CombatSkillUseValidationMode? ValidationMode { get; private set; }
        public CombatActorValidationClassificationFailureReason FailureReason { get; private set; }

        public CombatActorValidationClassificationResult(
            bool isClassified,
            CombatActorValidationKind kind,
            CombatSkillUseValidationMode? validationMode,
            CombatActorValidationClassificationFailureReason failureReason)
        {
            IsClassified = isClassified;
            Kind = kind;
            ValidationMode = validationMode;
            FailureReason = failureReason;
        }
    }
}
