using System;

namespace Game.Domain.Combat.Resources
{
    public class ResourcePool
    {
        public int Max { get; private set; }
        public int Current { get; private set; }

        public ResourcePool(int max)
        {
            Max = Math.Max(0, max);
            Current = Max;
        }

        public bool TrySpend(int amount)
        {
            if (amount > Current)
            {
                return false;
            }

            Current -= amount;
            return true;
        }

        public void Regenerate(int amount)
        {
            if (amount > 0)
            {
                Current = Math.Min(Max, Current + amount);
            }
        }
    }
}
