using System.Collections.Generic;

namespace Game.Domain.Skills
{
    public class SkillDefinitionRegistry
    {
        private readonly Dictionary<string, SkillDefinition> definitionsById = new Dictionary<string, SkillDefinition>();

        public bool Register(SkillDefinition definition)
        {
            if (!IsValidDefinition(definition) || definitionsById.ContainsKey(definition.Id))
            {
                return false;
            }

            definitionsById.Add(definition.Id, definition);
            return true;
        }

        public bool Contains(string skillId)
        {
            return !string.IsNullOrWhiteSpace(skillId) && definitionsById.ContainsKey(skillId);
        }

        public SkillDefinition Get(string skillId)
        {
            SkillDefinition definition;
            return Contains(skillId) && definitionsById.TryGetValue(skillId, out definition)
                ? definition
                : null;
        }

        private static bool IsValidDefinition(SkillDefinition definition)
        {
            return definition != null
                && !string.IsNullOrWhiteSpace(definition.Id)
                && !string.IsNullOrWhiteSpace(definition.Name);
        }
    }
}
