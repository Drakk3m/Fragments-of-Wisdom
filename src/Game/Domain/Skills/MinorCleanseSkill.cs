using Game.Domain.Common;
using Game.Domain.Combat;

namespace Game.Domain.Skills
{
    public class MinorCleanseSkill : ICombatSkill
    {
        public string Id
        {
            get { return "skill_minor_cleanse"; }
        }

        public string Name
        {
            get { return "RefutaciÃ³n Menor"; }
        }

        public void Apply(
            CombatContext context,
            SkillProgress progress)
        {
            context.RequestCleanse(CleanseType.Partial);
            context.EventLog.Add(GameEventType.SkillUsed, context.Attacker.Name + " uses RefutaciÃ³n Menor.");
        }
    }
}
