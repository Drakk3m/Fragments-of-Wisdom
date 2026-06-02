using System.Collections.Generic;

namespace Game.Domain.Classes
{
    public class ClassDefinition
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public IReadOnlyList<string> FixedSkillIds { get; private set; }
        public IReadOnlyList<string> AllowedSkillIds
        {
            get { return FixedSkillIds; }
        }

        public ClassDefinition(
            string id,
            string name,
            IReadOnlyList<string> fixedSkillIds)
        {
            Id = id;
            Name = name;
            FixedSkillIds = CopySkillIds(fixedSkillIds);
        }

        public bool AllowsSkill(string skillId)
        {
            if (string.IsNullOrEmpty(skillId))
            {
                return false;
            }

            foreach (var allowedSkillId in AllowedSkillIds)
            {
                if (allowedSkillId == skillId)
                {
                    return true;
                }
            }

            return false;
        }

        private static IReadOnlyList<string> CopySkillIds(IReadOnlyList<string> sourceSkillIds)
        {
            var skillIds = new List<string>();

            if (sourceSkillIds == null)
            {
                return skillIds;
            }

            foreach (var skillId in sourceSkillIds)
            {
                if (!string.IsNullOrEmpty(skillId))
                {
                    skillIds.Add(skillId);
                }
            }

            return skillIds;
        }
    }
}
