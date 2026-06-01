using System;
using System.Collections.Generic;
using Game.Domain.Combat.Stats;

namespace Game.Domain.Equipment
{
    public class EquipmentStatModifierService
    {
        public IReadOnlyList<CombatStatModifier> GatherModifiers(EquipmentProfile profile)
        {
            var modifiers = new List<CombatStatModifier>();

            if (profile == null)
            {
                return modifiers;
            }

            foreach (var item in profile.Equipped.Values)
            {
                if (item == null)
                {
                    continue;
                }

                foreach (var modifier in item.StatModifiers)
                {
                    modifiers.Add(new CombatStatModifier(
                        modifier.StatType,
                        modifier.FlatAmount,
                        StatModifierSourceType.Equipment,
                        item.Id));
                }
            }

            return modifiers;
        }

        public CombatStats ResolveEffectiveStats(
            CombatStats baseStats,
            EquipmentProfile profile)
        {
            if (baseStats == null)
            {
                throw new ArgumentNullException(nameof(baseStats));
            }

            return new CombatStatModifierResolver().Resolve(
                baseStats,
                GatherModifiers(profile));
        }
    }
}
