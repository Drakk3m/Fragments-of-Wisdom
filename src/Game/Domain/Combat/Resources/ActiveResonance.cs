namespace Game.Domain.Combat.Resources
{
    public class ActiveResonance
    {
        private readonly ResourcePool pool;

        public int Max
        {
            get { return pool.Max; }
        }

        public int Current
        {
            get { return pool.Current; }
        }

        public ActiveResonance(int max)
        {
            pool = new ResourcePool(max);
        }

        public bool TrySpend(int amount)
        {
            return pool.TrySpend(amount);
        }

        public bool CanSpend(int amount)
        {
            return amount <= 0 || pool.Current >= amount;
        }

        public void Refill(int amount)
        {
            pool.Regenerate(amount);
        }
    }
}
