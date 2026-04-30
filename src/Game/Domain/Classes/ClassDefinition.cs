using System.Collections.Generic;

namespace Game.Domain.Classes
{
    public class ClassDefinition
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public IReadOnlyList<string> FixedSkillIds { get; private set; }

        public ClassDefinition(
            string id,
            string name,
            IReadOnlyList<string> fixedSkillIds)
        {
            Id = id;
            Name = name;
            FixedSkillIds = fixedSkillIds;
        }
    }
}
