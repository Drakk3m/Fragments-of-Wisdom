namespace Game.Domain.Combat.Resolvers
{
    public class DamageResult
    {
        public bool Hit { get; private set; }
        public bool Critical { get; private set; }
        public int Damage { get; private set; }

        public DamageResult(
            bool hit,
            bool critical,
            int damage)
        {
            Hit = hit;
            Critical = critical;
            Damage = damage;
        }
    }
}
