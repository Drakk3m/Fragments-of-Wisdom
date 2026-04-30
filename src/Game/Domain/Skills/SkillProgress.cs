namespace Game.Domain.Skills
{
    public class SkillProgress
    {
        public SkillDefinition Definition { get; private set; }
        public int Level { get; private set; }
        public int Experience { get; private set; }

        public bool IsMastered
        {
            get { return Level >= Definition.MaxLevel; }
        }

        public SkillProgress(SkillDefinition definition)
        {
            Definition = definition;
            Level = 1;
        }

        public void GainExperience(int amount)
        {
            if (IsMastered)
            {
                return;
            }

            Experience += amount;

            if (Experience >= 100)
            {
                Experience = 0;
                Level++;

                if (Level > Definition.MaxLevel)
                {
                    Level = Definition.MaxLevel;
                }
            }
        }

        public int GetBaseCost()
        {
            return Definition.Cost.GetCost(Level);
        }

        public float GetMasteryCostMultiplier()
        {
            return IsMastered ? 0.8f : 1f;
        }

        public float GetMasteryEffectBonus()
        {
            return IsMastered ? 0.15f : 0f;
        }
    }
}
