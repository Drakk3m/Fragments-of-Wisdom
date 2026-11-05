using Game.Domain.Skills;

namespace Game.Application.Runtime
{
    public sealed class PreparedSkillSnapshot
    {
        public string SkillId { get; }
        public string Name { get; }
        public SkillCategory Category { get; }
        public SkillTargeting Targeting { get; }
        public int MaxLevel { get; }
        public int Level { get; }
        public int Experience { get; }
        // XP/level mastery only, not MasteryProfile's cross-class retention fact.
        public bool IsSkillProgressMastered { get; }
        // Metadata only: neither value is a final payable cost or an affordability claim.
        public int BaseEssenceCost { get; }
        public int EssenceCostPerLevel { get; }
        public int ResonanceCost { get; }

        internal PreparedSkillSnapshot(SkillDefinition canonical, SkillProgress progress)
        {
            SkillId = canonical.Id;
            Name = canonical.Name;
            Category = canonical.Category;
            Targeting = canonical.Targeting;
            MaxLevel = canonical.MaxLevel;
            Level = progress.Level;
            Experience = progress.Experience;
            IsSkillProgressMastered = progress.IsMastered;
            BaseEssenceCost = canonical.Cost.BaseCost;
            EssenceCostPerLevel = canonical.Cost.CostPerLevel;
            ResonanceCost = canonical.ResonanceCost;
        }
    }
}
