using Game.Domain.Combat.Stats;

namespace Game.Domain.Equipment
{
    public class EquipmentStatAggregator
    {
        public CombatStats Aggregate(
            CombatStats baseStats,
            EquipmentLoadout loadout)
        {
            var result = baseStats.Clone();

            foreach (var item in loadout.Equipped.Values)
            {
                result.MaxHealth += item.StatBonus.MaxHealth;
                result.Attack += item.StatBonus.Attack;
                result.Defense += item.StatBonus.Defense;
                result.Precision += item.StatBonus.Precision;
                result.Evasion += item.StatBonus.Evasion;
                result.CriticalChance += item.StatBonus.CriticalChance;
                result.CriticalResistance += item.StatBonus.CriticalResistance;
                result.DebuffChance += item.StatBonus.DebuffChance;
                result.DebuffResistance += item.StatBonus.DebuffResistance;
                result.BuffPotency += item.StatBonus.BuffPotency;
                result.BuffResistance += item.StatBonus.BuffResistance;
                result.ControlResistance += item.StatBonus.ControlResistance;
            }

            return result;
        }
    }
}
