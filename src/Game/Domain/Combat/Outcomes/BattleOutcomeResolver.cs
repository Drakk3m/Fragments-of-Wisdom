namespace Game.Domain.Combat.Outcomes
{
    public class BattleOutcomeResolver
    {
        public BattleOutcome Resolve(CombatRuntimeContext runtime)
        {
            if (!runtime.Player.IsAlive)
            {
                return new BattleOutcome(BattleWinner.Enemy, runtime.TurnNumber);
            }

            if (!runtime.Enemy.IsAlive)
            {
                return new BattleOutcome(BattleWinner.Player, runtime.TurnNumber);
            }

            return new BattleOutcome(BattleWinner.None, runtime.TurnNumber);
        }
    }
}
