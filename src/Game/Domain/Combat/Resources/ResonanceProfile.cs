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
            if (amount <= 0)
            {
                return true;
            }

            int missingActive = Active.Max - Active.Current;
            int refillAmount = Math.Min(amount, missingActive);

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
    }
}
