namespace Game.Domain.Combat.Stats
{
    public class CombatStatModifier
    {
        public CombatStatType StatType { get; private set; }
        public int FlatAmount { get; private set; }
        public StatModifierSourceType SourceType { get; private set; }
        public string SourceId { get; private set; }

        public CombatStatModifier(
            CombatStatType statType,
            int flatAmount)
            : this(statType, flatAmount, StatModifierSourceType.Unknown, string.Empty)
        {
        }

        public CombatStatModifier(
            CombatStatType statType,
            int flatAmount,
            StatModifierSourceType sourceType,
            string sourceId)
        {
            StatType = statType;
            FlatAmount = flatAmount;
            SourceType = sourceType;
            SourceId = sourceId ?? string.Empty;
        }
    }
}
