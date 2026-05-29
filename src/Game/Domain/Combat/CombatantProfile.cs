using System;
using Game.Domain.Combat.Stats;

namespace Game.Domain.Combat
{
    public class CombatantProfile
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public CombatStats Stats { get; private set; }
        public int MaxResource { get; private set; }
        public int CurrentResource { get; private set; }

        public bool IsDefeated
        {
            get { return Stats.CurrentHealth <= 0; }
        }

        public CombatantProfile(
            string id,
            string name,
            CombatStats stats,
            int maxResource)
        {
            Id = id;
            Name = name;
            Stats = stats;
            MaxResource = Math.Max(0, maxResource);
            CurrentResource = MaxResource;

            if (Stats.CurrentHealth <= 0)
            {
                Stats.CurrentHealth = Stats.MaxHealth;
            }
        }

        public bool TrySpendResource(int amount)
        {
            if (amount <= 0)
            {
                return true;
            }

            if (CurrentResource < amount)
            {
                return false;
            }

            CurrentResource -= amount;
            return true;
        }

        public void ReceiveDamage(int amount)
        {
            if (amount > 0)
            {
                Stats.CurrentHealth = Math.Max(0, Stats.CurrentHealth - amount);
            }
        }
    }
}
