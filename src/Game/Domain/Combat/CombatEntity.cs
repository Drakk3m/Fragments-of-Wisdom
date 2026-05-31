using System;
using Game.Domain.Combat.Resources;
using Game.Domain.Combat.Stats;
using Game.Domain.Combat.StatusEffects;

namespace Game.Domain.Combat
{
    public class CombatEntity
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public CombatStats Stats { get; private set; }
        public ResourcePool Resource { get; private set; }
        public DebuffContainer Debuffs { get; private set; }
        public BuffContainer Buffs { get; private set; }
        public StatusEffectCollection StatusEffects { get; private set; }
        public CombatEntityFlags Flags { get; private set; }

        public bool IsAlive
        {
            get { return Stats.CurrentHealth > 0; }
        }

        public CombatEntity(
            string id,
            string name,
            CombatStats stats,
            int maxResource)
        {
            Id = id;
            Name = name;
            Stats = stats;
            Resource = new ResourcePool(maxResource);
            Debuffs = new DebuffContainer();
            Buffs = new BuffContainer();
            StatusEffects = new StatusEffectCollection();
            Flags = new CombatEntityFlags();

            if (Stats.CurrentHealth <= 0)
            {
                Stats.CurrentHealth = Stats.MaxHealth;
            }
        }

        public void ReceiveDamage(int amount)
        {
            if (amount > 0)
            {
                Stats.CurrentHealth = Math.Max(0, Stats.CurrentHealth - amount);
            }
        }

        public void Heal(int amount)
        {
            if (amount > 0)
            {
                Stats.CurrentHealth = Math.Min(Stats.MaxHealth, Stats.CurrentHealth + amount);
            }
        }
    }
}
