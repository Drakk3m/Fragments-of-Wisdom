namespace Game.Domain.Combat.Outcomes
{
    public class BattleOutcome
    {
        public BattleWinner Winner { get; private set; }
        public int TurnCount { get; private set; }

        public BattleOutcome(
            BattleWinner winner,
            int turnCount)
        {
            Winner = winner;
            TurnCount = turnCount;
        }
    }
}
