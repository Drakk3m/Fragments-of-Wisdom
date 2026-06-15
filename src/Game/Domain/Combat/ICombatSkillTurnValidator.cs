namespace Game.Domain.Combat
{
    public interface ICombatSkillTurnValidator
    {
        CombatSkillTurnValidationResult Validate(
            TurnRequest request,
            CombatRuntimeContext runtime);
    }
}
