using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.StatusEffects;

namespace Game.Domain.Skills
{
    public class BalanceRitualSkill : ICombatSkill
    {
        public string Id
        {
            get { return "skill_balance_ritual"; }
        }

        public string Name
        {
            get { return "Ritual de Balance"; }
        }

        public void Apply(
            CombatContext context,
            SkillProgress progress)
        {
            context.DamageMultiplier += 0.15f + progress.GetMasteryEffectBonus();
            context.RequestDebuff(new BurnDebuff(2, 5, 0.9f), 0.45f);
            context.EventLog.Add(GameEventType.SkillUsed, context.Attacker.Name + " uses Ritual de Balance.");
        }
    }
}
