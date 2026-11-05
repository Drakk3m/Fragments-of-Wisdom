namespace Game.Application.Runtime
{
    public enum CombatTurnFlowPhase
    {
        NotStarted,
        AwaitingPlayer,
        ResolvingPlayer,
        AwaitingEnemy,
        ResolvingEnemy,
        Terminal,
        Faulted
    }
}
