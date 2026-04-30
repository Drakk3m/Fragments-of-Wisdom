namespace Game.Domain.Skills
{
    public class SkillCost
    {
        public int BaseCost { get; private set; }
        public int CostPerLevel { get; private set; }

        public SkillCost(
            int baseCost,
            int costPerLevel)
        {
            BaseCost = baseCost;
            CostPerLevel = costPerLevel;
        }

        public int GetCost(int skillLevel)
        {
            return BaseCost + (CostPerLevel * skillLevel);
        }
    }
}
