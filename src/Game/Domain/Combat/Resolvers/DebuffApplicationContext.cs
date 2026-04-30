namespace Game.Domain.Combat.Resolvers
{
    public class DebuffApplicationContext
    {
        public CombatEntity Source { get; private set; }
        public CombatEntity Target { get; private set; }
        public float BaseChance { get; set; }
        public float BonusChance { get; set; }

        public float FinalChance
        {
            get { return BaseChance + BonusChance; }
        }

        public DebuffApplicationContext(
            CombatEntity source,
            CombatEntity target,
            float baseChance)
        {
            Source = source;
            Target = target;
            BaseChance = baseChance;
        }
    }
}
