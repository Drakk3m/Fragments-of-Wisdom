using System.Collections.Generic;

namespace Game.Domain.Equipment
{
    public class EquipmentLoadout
    {
        private readonly Dictionary<EquipmentSlot, EquipmentDefinition> equipped =
            new Dictionary<EquipmentSlot, EquipmentDefinition>();

        public IReadOnlyDictionary<EquipmentSlot, EquipmentDefinition> Equipped
        {
            get { return equipped; }
        }

        public void Equip(EquipmentDefinition item)
        {
            equipped[item.Slot] = item;
        }
    }
}
