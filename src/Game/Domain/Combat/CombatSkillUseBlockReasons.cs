namespace Game.Domain.Combat
{
    /// <summary>
    /// Responsibility-based constants for legacy enum values. Keeping them outside the enum
    /// preserves its names, enumeration, parsing, ToString and string-enum serialization.
    /// </summary>
    public static class CombatSkillUseBlockReasons
    {
        public const CombatSkillUseBlockReason UnlockProfileMissing = CombatSkillUseBlockReason.V3UnlockProfileMissing;
        public const CombatSkillUseBlockReason UnlockSkillLocked = CombatSkillUseBlockReason.V3SkillLocked;
        public const CombatSkillUseBlockReason ClassMasteryContextIncomplete = CombatSkillUseBlockReason.V4ContextIncomplete;
        public const CombatSkillUseBlockReason ClassMasterySkillBlocked = CombatSkillUseBlockReason.V4SkillBlocked;
    }
}
