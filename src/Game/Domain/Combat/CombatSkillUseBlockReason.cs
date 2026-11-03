namespace Game.Domain.Combat
{
    public enum CombatSkillUseBlockReason
    {
        None,
        RequestMissing,
        NotSkillAction,
        SkillMissing,
        SkillProgressMissing,
        SkillDefinitionMissing,
        SkillIdMissing,
        SkillProgressIdMismatch,
        ValidationContextMissing,
        PlayerContextIncomplete,
        V3UnlockProfileMissing,
        V3SkillLocked,
        V4ContextIncomplete,
        V4SkillBlocked,
        SkillDefinitionUnregistered,
        SkillDefinitionConflict,
        SkillDefinitionInvalid,
        UnsupportedResonanceOwnership
    }
}
