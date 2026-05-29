namespace Game.Domain.Combat
{
    public class CombatTurnResult
    {
        public bool ActionAccepted { get; private set; }
        public string BlockReason { get; private set; }
        public bool Hit { get; private set; }
        public bool Critical { get; private set; }
        public int Damage { get; private set; }
        public bool TargetDefeated { get; private set; }

        public CombatTurnResult(
            bool actionAccepted,
            string blockReason,
            bool hit,
            bool critical,
            int damage,
            bool targetDefeated)
        {
            ActionAccepted = actionAccepted;
            BlockReason = blockReason;
            Hit = hit;
            Critical = critical;
            Damage = damage;
            TargetDefeated = targetDefeated;
        }
    }
}
