using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.StatusEffects;

namespace Game.Domain.Skills
{
    public class DestinyCorrectionSkill : ICombatSkill
    {
        public string Id
        {
            get { return "skill_destiny_correction"; }
        }

        public string Name
        {
            get { return "CorrecciÃ³n del Destino"; }
        }

        public void Apply(
            CombatContext context,
            SkillProgress progress)
        {
            context.DamageMultiplier += 0.05f;
            context.RequestDebuff(new PoisonDebuff(3, 4), 0.65f + progress.GetMasteryEffectBonus());
            context.EventLog.Add(GameEventType.SkillUsed, context.Attacker.Name + " uses CorrecciÃ³n del Destino.");
        }
    }
}
