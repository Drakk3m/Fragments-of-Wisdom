using System;

namespace Game.Domain.Combat.Resources
{
    public class ResonanceProfile
    {
        public ActiveResonance Active { get; private set; }
        public ResonanceReserve Reserve { get; private set; }

        public ResonanceProfile(
            ActiveResonance active,
            ResonanceReserve reserve)
        {
            if (active == null)
            {
                throw new ArgumentNullException(nameof(active));
            }

            if (reserve == null)
            {
                throw new ArgumentNullException(nameof(reserve));
            }

            Active = active;
            Reserve = reserve;
        }

        public bool TrySpendActive(int amount)
        {
            return Active.TrySpend(amount);
        }

        public bool CanSpendActive(int amount)
        {
            return Active.CanSpend(amount);
        }

        public bool TryRefillFromReserve(int amount)
        {
            int refillAmount = GetRefillAmount(amount);

            if (refillAmount <= 0)
            {
                return true;
            }

            if (!Reserve.TryConsume(refillAmount))
            {
                return false;
            }

            Active.Refill(refillAmount);
            return true;
        }

        // Rejected transfers must not start a turn or run its status/debuff hooks.
        internal bool CanRefillFromReserve(int amount)
        {
            return Reserve.Current >= GetRefillAmount(amount);
        }

        private int GetRefillAmount(int amount)
        {
            return amount <= 0 ? 0 : Math.Min(amount, Active.Max - Active.Current);
        }
    }
}
