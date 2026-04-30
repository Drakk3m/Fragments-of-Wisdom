using System;
using Game.Domain.Common;

namespace Game.Domain.Combat.Resolvers
{
    public class DamageResolver
    {
        private readonly IRandomSource random;

        public DamageResolver(IRandomSource random)
        {
            this.random = random;
        }

        public DamageResult Resolve(CombatContext context)
        {
            float hitChance = ProbabilityCalculator.CalculateChance(
                context.Attacker.Stats.Precision,
                context.Defender.Stats.Evasion,
                0.75f + context.HitChanceModifier,
                0.005f,
                0.1f,
                0.95f);

            bool hit = random.NextDouble() <= hitChance;

            if (!hit)
            {
                return new DamageResult(false, false, 0);
            }

            int attack = Math.Max(1, context.BaseAttack + context.FlatDamageBonus);
            int defense = Math.Max(0, context.BaseDefense);
            int damage = Math.Max(1, (int)(attack * ((float)attack / (attack + defense))));
            damage = Math.Max(1, (int)(damage * context.DamageMultiplier));

            float critChance = ProbabilityCalculator.CalculateChance(
                context.Attacker.Stats.CriticalChance,
                context.Defender.Stats.CriticalResistance,
                0.10f + context.CriticalChanceModifier,
                0.003f,
                0.02f,
                0.65f);

            bool critical = random.NextDouble() <= critChance;

            if (critical)
            {
                damage = Math.Max(1, (int)(damage * context.Attacker.Stats.CriticalDamageMultiplier));
            }

            return new DamageResult(true, critical, damage);
        }
    }
}
