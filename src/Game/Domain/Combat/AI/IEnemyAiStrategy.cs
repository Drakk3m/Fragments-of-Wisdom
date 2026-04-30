namespace Game.Domain.Combat.AI
{
    public interface IEnemyAiStrategy
    {
        TurnRequest ChooseAction(CombatRuntimeContext runtime);
    }
}
