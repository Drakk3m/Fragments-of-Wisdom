using System.Text.Json.Serialization;
using Game.Domain.Classes;

namespace Game.Domain.Combat
{
    public class CombatSkillTurnValidationResult
    {
        public bool IsAllowed { get; private set; }
        public CombatSkillUseValidationMode? ValidationMode { get; private set; }
        public CombatSkillValidationOrchestrationFailureStage FailureStage { get; private set; }
        public CombatActorValidationClassificationFailureReason ClassificationFailureReason { get; private set; }
        public CombatSkillUseBlockReason BlockReason { get; private set; }
        public string V3BlockReason { get; private set; }
        public MasteredSkillRetentionBlockReason V4BlockReason { get; private set; }

        // Keep legacy JSON fields and named constructor arguments without duplicate stored facts.
        [JsonIgnore]
        public string UnlockBlockReason { get { return V3BlockReason; } }

        [JsonIgnore]
        public MasteredSkillRetentionBlockReason ClassMasteryBlockReason { get { return V4BlockReason; } }

        public CombatSkillTurnValidationResult(
            bool isAllowed,
            CombatSkillUseValidationMode? validationMode,
            CombatSkillValidationOrchestrationFailureStage failureStage,
            CombatActorValidationClassificationFailureReason classificationFailureReason,
            CombatSkillUseBlockReason blockReason,
            string v3BlockReason,
            MasteredSkillRetentionBlockReason v4BlockReason)
        {
            IsAllowed = isAllowed;
            ValidationMode = validationMode;
            FailureStage = failureStage;
            ClassificationFailureReason = classificationFailureReason;
            BlockReason = blockReason;
            V3BlockReason = v3BlockReason ?? string.Empty;
            V4BlockReason = v4BlockReason;
        }
    }
}
