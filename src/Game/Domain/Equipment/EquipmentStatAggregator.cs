using Game.Domain.Combat.Stats;

namespace Game.Domain.Equipment
{
    public class EquipmentStatAggregator
    {
        public CombatStats Aggregate(
            CombatStats baseStats,
            EquipmentLoadout loadout)
        {
            return new EquipmentStatModifierService().ResolveEffectiveStats(
                baseStats,
                loadout);
        }
    }
}
