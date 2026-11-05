namespace Game.Application.Runtime
{
    /// <summary>Scalar-only scheduling observation; safe to read even from an executing hook.</summary>
    public sealed class CombatTurnFlowSnapshot
    {
        public CombatTurnFlowPhase Phase { get; }
        public CombatActorSide? EligibleActor { get; }
        public bool IsTerminal { get; }
        public bool IsFaulted { get; }

        internal CombatTurnFlowSnapshot(CombatTurnFlowPhase phase)
        {
            Phase = phase;
            EligibleActor = phase == CombatTurnFlowPhase.AwaitingPlayer ? CombatActorSide.Player
                : phase == CombatTurnFlowPhase.AwaitingEnemy ? CombatActorSide.Enemy : (CombatActorSide?)null;
            IsTerminal = phase == CombatTurnFlowPhase.Terminal;
            IsFaulted = phase == CombatTurnFlowPhase.Faulted;
        }
    }
}
