using Game.Domain.Combat.Stats;

namespace Game.Domain.Equipment
{
    public class EquipmentDefinition
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public EquipmentSlot Slot { get; private set; }
        public CombatStats StatBonus { get; private set; }
        public string TaughtSkillId { get; private set; }

        public EquipmentDefinition(
            string id,
            string name,
            EquipmentSlot slot,
            CombatStats statBonus,
            string taughtSkillId = null)
        {
            Id = id;
            Name = name;
            Slot = slot;
            StatBonus = statBonus;
            TaughtSkillId = taughtSkillId;
        }
    }
}
