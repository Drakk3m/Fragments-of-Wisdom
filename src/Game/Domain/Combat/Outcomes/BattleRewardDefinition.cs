namespace Game.Domain.Combat.Outcomes
{
    public class BattleRewardDefinition
    {
        public string CurrencyId { get; private set; }
        public int CurrencyAmount { get; private set; }
        public int Experience { get; private set; }

        public BattleRewardDefinition(
            string currencyId,
            int currencyAmount,
            int experience)
        {
            CurrencyId = currencyId;
            CurrencyAmount = currencyAmount;
            Experience = experience;
        }
    }
}
