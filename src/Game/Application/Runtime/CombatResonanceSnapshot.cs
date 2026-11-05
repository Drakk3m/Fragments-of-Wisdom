using Game.Domain.Combat.Resources;

namespace Game.Application.Runtime
{
    public sealed class CombatResonanceSnapshot
    {
        public CombatResourceSnapshot Active { get; }
        public CombatResourceSnapshot Reserve { get; }

        internal CombatResonanceSnapshot(ResonanceProfile profile)
        {
            Active = new CombatResourceSnapshot(profile.Active.Current, profile.Active.Max);
            Reserve = new CombatResourceSnapshot(profile.Reserve.Current, profile.Reserve.Max);
        }
    }
}
