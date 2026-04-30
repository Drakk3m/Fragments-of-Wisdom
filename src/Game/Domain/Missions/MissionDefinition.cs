using System.Collections.Generic;

namespace Game.Domain.Missions
{
    public class MissionDefinition
    {
        public string Id { get; private set; }
        public MissionType Type { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public IReadOnlyList<MissionObjectiveDefinition> Objectives { get; private set; }
        public IReadOnlyList<MissionRewardDefinition> Rewards { get; private set; }

        public MissionDefinition(
            string id,
            MissionType type,
            string name,
            string description,
            IReadOnlyList<MissionObjectiveDefinition> objectives,
            IReadOnlyList<MissionRewardDefinition> rewards)
        {
            Id = id;
            Type = type;
            Name = name;
            Description = description;
            Objectives = objectives;
            Rewards = rewards;
        }
    }
}
