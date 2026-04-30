using System;
using Game.Domain.Common;

namespace Game.Domain.Combat
{
    public static class ProbabilityCalculator
    {
        public static float CalculateChance(
            int attackerStat,
            int defenderStat,
            float baseChance,
            float factor,
            float min,
            float max)
        {
            return Math.Max(min, Math.Min(max, baseChance + ((attackerStat - defenderStat) * factor)));
        }

        public static bool Roll(
            IRandomSource random,
            int attackerStat,
            int defenderStat,
            float baseChance,
            float factor,
            float min,
            float max)
        {
            return random.NextDouble() <= CalculateChance(
                attackerStat,
                defenderStat,
                baseChance,
                factor,
                min,
                max);
        }
    }
}
