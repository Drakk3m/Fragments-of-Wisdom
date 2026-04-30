using Game.Domain.Common;

namespace Game.Domain.Combat.StatusEffects
{
    public class BurnDebuff : BaseDebuff
    {
        private readonly int damagePerTurn;
        private readonly float attackMultiplier;

        public override string Name
        {
            get { return "Burn"; }
        }

        public override DebuffType Type
        {
            get { return DebuffType.DamageOverTime; }
        }

        public BurnDebuff(
            int duration,
            int damagePerTurn,
            float attackMultiplier)
            : base(duration)
        {
            this.damagePerTurn = damagePerTurn;
            this.attackMultiplier = attackMultiplier;
        }

        public override void OnTurnStart(
            CombatEntity target,
            CombatContext context)
        {
            target.ReceiveDamage(damagePerTurn);
            context.EventLog.Add(GameEventType.StatusTick, target.Name + " suffers " + damagePerTurn + " burn damage.");
        }

        public override void OnBeforeAction(CombatContext context)
        {
            context.BaseAttack = (int)(context.BaseAttack * attackMultiplier);
        }
    }
}
