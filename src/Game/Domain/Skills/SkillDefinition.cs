namespace Game.Domain.Skills
{
    public class SkillDefinition
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public int MaxLevel { get; private set; }
        public SkillCost Cost { get; private set; }
        public SkillCategory Category { get; private set; }
        public SkillTargeting Targeting { get; private set; }
        public SkillEffectType EffectType { get; private set; }
        public string RequiredUnlockId { get; private set; }

        public SkillDefinition(
            string id,
            string name,
            int maxLevel,
            SkillCost cost)
            : this(
                id,
                name,
                maxLevel,
                cost,
                SkillCategory.Combat,
                SkillTargeting.SingleEnemy,
                SkillEffectType.Damage,
                "")
        {
        }

        public SkillDefinition(
            string id,
            string name,
            int maxLevel,
            SkillCost cost,
            SkillCategory category,
            SkillTargeting targeting,
            SkillEffectType effectType,
            string requiredUnlockId)
        {
            Id = id;
            Name = name;
            MaxLevel = maxLevel;
            Cost = cost;
            Category = category;
            Targeting = targeting;
            EffectType = effectType;
            RequiredUnlockId = requiredUnlockId;
        }
    }
}
