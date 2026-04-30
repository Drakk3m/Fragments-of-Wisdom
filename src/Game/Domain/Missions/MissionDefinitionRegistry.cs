using System.Collections.Generic;

namespace Game.Domain.Missions
{
    public class MissionDefinitionRegistry
    {
        private readonly Dictionary<string, MissionDefinition> definitions =
            new Dictionary<string, MissionDefinition>();

        public void Register(MissionDefinition definition)
        {
            definitions[definition.Id] = definition;
        }

        public MissionDefinition Get(string missionId)
        {
            return definitions[missionId];
        }
    }
}
