namespace Game.Domain.Skills
{
    public class SkillDefinition
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public int MaxLevel { get; private set; }
        public SkillCost Cost { get; private set; }

        public SkillDefinition(
            string id,
            string name,
            int maxLevel,
            SkillCost cost)
        {
            Id = id;
            Name = name;
            MaxLevel = maxLevel;
            Cost = cost;
        }
    }
}
