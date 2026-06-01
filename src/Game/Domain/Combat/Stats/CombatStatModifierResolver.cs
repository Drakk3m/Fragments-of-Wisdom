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

            ClampEffectiveStats(effectiveStats);
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

        private static void ClampEffectiveStats(CombatStats stats)
        {
            stats.MaxHealth = Math.Max(1, stats.MaxHealth);
            stats.CurrentHealth = Math.Min(
                stats.MaxHealth,
                Math.Max(0, stats.CurrentHealth));

            stats.Attack = Math.Max(0, stats.Attack);
            stats.Defense = Math.Max(0, stats.Defense);
            stats.Precision = Math.Max(0, stats.Precision);
            stats.Evasion = Math.Max(0, stats.Evasion);
            stats.CriticalChance = Math.Max(0, stats.CriticalChance);
            stats.CriticalResistance = Math.Max(0, stats.CriticalResistance);
            stats.CriticalDamageMultiplier = Math.Max(1f, stats.CriticalDamageMultiplier);
            stats.DebuffChance = Math.Max(0, stats.DebuffChance);
            stats.DebuffResistance = Math.Max(0, stats.DebuffResistance);
            stats.BuffPotency = Math.Max(0, stats.BuffPotency);
            stats.BuffResistance = Math.Max(0, stats.BuffResistance);
            stats.ControlResistance = Math.Max(0, stats.ControlResistance);
        }
    }
}
