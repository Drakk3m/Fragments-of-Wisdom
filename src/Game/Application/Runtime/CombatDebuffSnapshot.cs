using Game.Domain.Combat.StatusEffects;

namespace Game.Application.Runtime
{
    /// <summary>Legacy debuff facts; RemainingTurns retains its original lifecycle meaning.</summary>
    public sealed class CombatDebuffSnapshot
    {
        public string Name { get; }
        public DebuffType Type { get; }
        public int RemainingTurns { get; }
        public bool IsExpired { get; }

        internal CombatDebuffSnapshot(IDebuff debuff)
        {
            Name = debuff.Name;
            Type = debuff.Type;
            RemainingTurns = debuff.RemainingTurns;
            IsExpired = debuff.IsExpired;
        }
    }
}
