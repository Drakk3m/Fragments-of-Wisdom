using Game.Domain.Common;

namespace Game.Domain.Combat.StatusEffects
{
    public class PoisonDebuff : BaseDebuff
    {
        private readonly int damagePerTurn;

        public override string Name
        {
            get { return "Poison"; }
        }

        public override DebuffType Type
        {
            get { return DebuffType.DamageOverTime; }
        }

        public PoisonDebuff(
            int duration,
            int damagePerTurn)
            : base(duration)
        {
            this.damagePerTurn = damagePerTurn;
        }

        public override void OnTurnStart(
            CombatEntity target,
            CombatContext context)
        {
            target.ReceiveDamage(damagePerTurn);
            context.EventLog.Add(GameEventType.StatusTick, target.Name + " suffers " + damagePerTurn + " poison damage.");
        }
    }
}
