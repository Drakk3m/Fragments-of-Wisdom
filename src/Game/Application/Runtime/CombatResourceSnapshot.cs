namespace Game.Application.Runtime
{
    public sealed class CombatResourceSnapshot
    {
        public int Current { get; }
        public int Max { get; }

        internal CombatResourceSnapshot(int current, int max)
        {
            Current = current;
            Max = max;
        }
    }
}
