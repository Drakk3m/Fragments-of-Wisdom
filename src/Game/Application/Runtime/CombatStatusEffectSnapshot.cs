using Game.Domain.Combat.StatusEffects;

namespace Game.Application.Runtime
{
    public sealed class CombatStatusEffectSnapshot
    {
        public StatusEffectType Type { get; }
        public int RemainingDuration { get; }
        public int Potency { get; }
        public bool AllowsStacking { get; }
        public bool IsExpired { get; }

        internal CombatStatusEffectSnapshot(StatusEffectInstance effect)
        {
            Type = effect.Type;
            RemainingDuration = effect.RemainingDuration;
            Potency = effect.Potency;
            AllowsStacking = effect.AllowsStacking;
            IsExpired = effect.IsExpired;
        }
    }
}
