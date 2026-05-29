namespace Game.Domain.Combat.Resources
{
    public class ResonanceReserve
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

        public ResonanceReserve(int max)
        {
            pool = new ResourcePool(max);
        }

        public bool TryConsume(int amount)
        {
            return pool.TrySpend(amount);
        }
    }
}
