using System;
using System.Collections.Generic;

namespace Game.Domain.Combat.Stats
{
    public class CombatStatModifierResolver
    {
        public CombatStats Resolve(
            CombatStats baseStats,
            IEnumerable<CombatStatModifier> modifiers)
        {
            if (baseStats == null)
            {
                throw new ArgumentNullException(nameof(baseStats));
            }

            var effectiveStats = baseStats.Clone();

            if (modifiers == null)
            {
                return effectiveStats;
            }

            foreach (var modifier in modifiers)
            {
                if (modifier != null)
                {
                    ApplyFlatModifier(effectiveStats, modifier);
                }
            }

            return effectiveStats;
        }

        private static void ApplyFlatModifier(
            CombatStats stats,
            CombatStatModifier modifier)
        {
            switch (modifier.StatType)
            {
                case CombatStatType.MaxHealth:
                    stats.MaxHealth += modifier.FlatAmount;
                    break;
                case CombatStatType.CurrentHealth:
                    stats.CurrentHealth += modifier.FlatAmount;
                    break;
                case CombatStatType.Attack:
                    stats.Attack += modifier.FlatAmount;
                    break;
                case CombatStatType.Defense:
                    stats.Defense += modifier.FlatAmount;
                    break;
                case CombatStatType.Precision:
                    stats.Precision += modifier.FlatAmount;
                    break;
                case CombatStatType.Evasion:
                    stats.Evasion += modifier.FlatAmount;
                    break;
                case CombatStatType.CriticalChance:
                    stats.CriticalChance += modifier.FlatAmount;
                    break;
                case CombatStatType.CriticalResistance:
                    stats.CriticalResistance += modifier.FlatAmount;
                    break;
                case CombatStatType.DebuffChance:
                    stats.DebuffChance += modifier.FlatAmount;
                    break;
                case CombatStatType.DebuffResistance:
                    stats.DebuffResistance += modifier.FlatAmount;
                    break;
                case CombatStatType.BuffPotency:
                    stats.BuffPotency += modifier.FlatAmount;
                    break;
                case CombatStatType.BuffResistance:
                    stats.BuffResistance += modifier.FlatAmount;
                    break;
                case CombatStatType.ControlResistance:
                    stats.ControlResistance += modifier.FlatAmount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(modifier.StatType));
            }
        }
    }
}
