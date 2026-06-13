using Game.Domain.Classes;

namespace Game.Domain.Combat
{
    public class CombatSkillUseValidationResult
    {
        public bool IsAllowed { get; private set; }
        public string SkillId { get; private set; }
        public bool IsPlayerAction { get; private set; }
        public bool V3AccessChecked { get; private set; }
        public bool V4AccessChecked { get; private set; }
        public CombatSkillUseBlockReason BlockReason { get; private set; }
        public string V3BlockReason { get; private set; }
        public MasteredSkillRetentionBlockReason V4BlockReason { get; private set; }

        public CombatSkillUseValidationResult(
            bool isAllowed,
            string skillId,
            bool isPlayerAction,
            bool v3AccessChecked,
            bool v4AccessChecked,
            CombatSkillUseBlockReason blockReason,
            string v3BlockReason,
            MasteredSkillRetentionBlockReason v4BlockReason)
        {
            IsAllowed = isAllowed;
            SkillId = skillId ?? string.Empty;
            IsPlayerAction = isPlayerAction;
            V3AccessChecked = v3AccessChecked;
            V4AccessChecked = v4AccessChecked;
            BlockReason = blockReason;
            V3BlockReason = v3BlockReason ?? string.Empty;
            V4BlockReason = v4BlockReason;
        }
    }
}
