using System.Collections.Generic;

namespace Game.Domain.Player
{
    public class PlayerWallet
    {
        private readonly Dictionary<string, int> currencies = new Dictionary<string, int>();

        public IReadOnlyDictionary<string, int> Currencies
        {
            get { return currencies; }
        }

        public void AddCurrency(
            string currencyId,
            int amount)
        {
            if (!currencies.ContainsKey(currencyId))
            {
                currencies[currencyId] = 0;
            }

            currencies[currencyId] += amount;
        }
    }
}
