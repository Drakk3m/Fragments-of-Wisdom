using System.Collections.Generic;
using Game.Domain.Combat.Stats;

namespace Game.Domain.Equipment
{
    public class EquipmentDefinition
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public EquipmentSlot Slot { get; private set; }
        public CombatStats StatBonus { get; private set; }
        public IReadOnlyList<CombatStatModifier> StatModifiers { get; private set; }
        public string TaughtSkillId { get; private set; }

        public EquipmentDefinition(
            string id,
            string name,
            EquipmentSlot slot,
            CombatStats statBonus,
            string taughtSkillId = null)
            : this(
                id,
                name,
                slot,
                BuildModifiers(statBonus),
                taughtSkillId)
        {
        }

        public EquipmentDefinition(
            string id,
            string name,
            EquipmentSlot slot,
            IReadOnlyList<CombatStatModifier> statModifiers,
            string taughtSkillId = null)
        {
            Id = id;
            Name = name;
            Slot = slot;
            StatModifiers = CopyModifiers(statModifiers);
            StatBonus = BuildStatBonus(StatModifiers);
            TaughtSkillId = taughtSkillId;
        }

        private static IReadOnlyList<CombatStatModifier> CopyModifiers(
            IReadOnlyList<CombatStatModifier> sourceModifiers)
        {
            var modifiers = new List<CombatStatModifier>();

            if (sourceModifiers == null)
            {
                return modifiers;
            }

            foreach (var modifier in sourceModifiers)
            {
                if (modifier != null)
                {
                    modifiers.Add(modifier);
                }
            }

            return modifiers;
        }

        private static IReadOnlyList<CombatStatModifier> BuildModifiers(CombatStats statBonus)
        {
            var modifiers = new List<CombatStatModifier>();

            if (statBonus == null)
            {
                return modifiers;
            }

            AddModifier(modifiers, CombatStatType.MaxHealth, statBonus.MaxHealth);
            AddModifier(modifiers, CombatStatType.CurrentHealth, statBonus.CurrentHealth);
            AddModifier(modifiers, CombatStatType.Attack, statBonus.Attack);
            AddModifier(modifiers, CombatStatType.Defense, statBonus.Defense);
            AddModifier(modifiers, CombatStatType.Precision, statBonus.Precision);
            AddModifier(modifiers, CombatStatType.Evasion, statBonus.Evasion);
            AddModifier(modifiers, CombatStatType.CriticalChance, statBonus.CriticalChance);
            AddModifier(modifiers, CombatStatType.CriticalResistance, statBonus.CriticalResistance);
            AddModifier(modifiers, CombatStatType.DebuffChance, statBonus.DebuffChance);
            AddModifier(modifiers, CombatStatType.DebuffResistance, statBonus.DebuffResistance);
            AddModifier(modifiers, CombatStatType.BuffPotency, statBonus.BuffPotency);
            AddModifier(modifiers, CombatStatType.BuffResistance, statBonus.BuffResistance);
            AddModifier(modifiers, CombatStatType.ControlResistance, statBonus.ControlResistance);

            return modifiers;
        }

        private static void AddModifier(
            List<CombatStatModifier> modifiers,
            CombatStatType statType,
            int amount)
        {
            if (amount != 0)
            {
                modifiers.Add(new CombatStatModifier(
                    statType,
                    amount,
                    StatModifierSourceType.Equipment,
                    string.Empty));
            }
        }

        private static CombatStats BuildStatBonus(
            IReadOnlyList<CombatStatModifier> modifiers)
        {
            var stats = new CombatStats();

            foreach (var modifier in modifiers)
            {
                ApplyModifier(stats, modifier);
            }

            return stats;
        }

        private static void ApplyModifier(
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
            }
        }
    }
}
