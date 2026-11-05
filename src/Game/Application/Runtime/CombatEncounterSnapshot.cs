using System.Collections.Generic;
using Game.Domain.Combat;
using Game.Domain.Skills;

namespace Game.Application.Runtime
{
    /// <summary>Detached observation, not a save DTO or an action-availability decision.</summary>
    public sealed class CombatEncounterSnapshot
    {
        public CombatEncounterState State { get; }
        public int TurnNumber { get; }
        public bool CombatEnded { get; }
        public CombatantSnapshot Player { get; }
        public CombatantSnapshot Enemy { get; }
        public bool HasPlayerResonance { get; }
        public CombatResonanceSnapshot PlayerResonance { get; }
        public IReadOnlyList<PreparedSkillSnapshot> PreparedSkills { get; }

        internal CombatEncounterSnapshot(
            CombatEncounterState state,
            CombatRuntimeContext runtime,
            IReadOnlyList<PreparedCombatSkillBinding> bindings,
            SkillDefinitionRegistry skillRegistry)
        {
            State = state;
            TurnNumber = runtime.TurnNumber;
            CombatEnded = runtime.CombatEnded;
            Player = new CombatantSnapshot(runtime.Player);
            Enemy = new CombatantSnapshot(runtime.Enemy);
            HasPlayerResonance = runtime.ResonanceProfile != null;
            PlayerResonance = HasPlayerResonance ? new CombatResonanceSnapshot(runtime.ResonanceProfile) : null;

            var skills = new List<PreparedSkillSnapshot>();
            foreach (var binding in bindings)
            {
                skills.Add(new PreparedSkillSnapshot(skillRegistry.Get(binding.SkillId), binding.Progress));
            }
            PreparedSkills = skills.AsReadOnly();
        }
    }
}
