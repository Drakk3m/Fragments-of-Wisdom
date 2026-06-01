using System.Collections.Generic;

namespace Game.Domain.Equipment
{
    public class EquipmentProfile
    {
        private readonly Dictionary<EquipmentSlot, EquipmentDefinition> equipped =
            new Dictionary<EquipmentSlot, EquipmentDefinition>();

        public IReadOnlyDictionary<EquipmentSlot, EquipmentDefinition> Equipped
        {
            get { return equipped; }
        }

        public bool Equip(EquipmentDefinition item)
        {
            if (item == null)
            {
                return false;
            }

            equipped[item.Slot] = item;
            return true;
        }

        public bool Unequip(EquipmentSlot slot)
        {
            return equipped.Remove(slot);
        }

        public EquipmentDefinition GetEquipped(EquipmentSlot slot)
        {
            EquipmentDefinition item;
            return equipped.TryGetValue(slot, out item) ? item : null;
        }
    }
}
