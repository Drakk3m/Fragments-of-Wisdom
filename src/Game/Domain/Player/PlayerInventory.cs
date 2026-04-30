using System.Collections.Generic;

namespace Game.Domain.Player
{
    public class PlayerInventory
    {
        private readonly Dictionary<string, int> items = new Dictionary<string, int>();

        public IReadOnlyDictionary<string, int> Items
        {
            get { return items; }
        }

        public void AddItem(
            string itemId,
            int amount)
        {
            if (!items.ContainsKey(itemId))
            {
                items[itemId] = 0;
            }

            items[itemId] += amount;
        }
    }
}
