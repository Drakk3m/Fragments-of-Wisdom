using System.Collections.Generic;

namespace Game.Domain.Player
{
    public class PlayerSkillBook
    {
        private readonly HashSet<string> skills = new HashSet<string>();

        public bool UnlockSkill(string skillId)
        {
            return skills.Add(skillId);
        }

        public bool HasSkill(string skillId)
        {
            return skills.Contains(skillId);
        }
    }
}
