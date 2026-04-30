using Game.Domain.Common;
using Game.Domain.Combat;

namespace Game.Domain.Skills
{
    public class DeepSightSkill : ICombatSkill
    {
        public string Id
        {
            get { return "skill_deep_sight"; }
        }

        public string Name
        {
            get { return "Mirada Profunda"; }
        }

        public void Apply(
            CombatContext context,
            SkillProgress progress)
        {
            context.RevealOneEnemyTrait = true;
            context.EventLog.Add(GameEventType.SkillUsed, context.Attacker.Name + " uses Mirada Profunda.");
        }
    }
}
