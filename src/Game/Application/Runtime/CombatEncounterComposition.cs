using System;
using System.Collections.Generic;
using Game.Domain.Combat;
using Game.Domain.Player;

namespace Game.Application.Runtime
{
    /// <summary>
    /// Single-threaded encounter owner. Mutable dependencies stay inside the application
    /// boundary; no actions, presentation projections or cross-encounter persistence here.
    /// </summary>
    public sealed class CombatEncounterComposition
    {
        private readonly CombatEncounterPreparation preparation;
        private readonly Dictionary<string, PreparedCombatSkillBinding> skillsById;
        private CombatEncounterState state = CombatEncounterState.Prepared;

        internal LoadedPlayerSession Session { get; }
        internal PlayerProgressionRuntimeState Progression { get { return Session.RuntimeState; } }
        internal PlayerProfile PlayerProfile { get; }
        internal CombatRuntimeContext Runtime { get; }
        internal TurnBasedCombatEngine Engine { get; }

        public Exception StartFailure { get; private set; }

        public CombatEncounterState State
        {
            get
            {
                return state == CombatEncounterState.Started
                    && (Runtime.CombatEnded || !Runtime.Player.IsAlive || !Runtime.Enemy.IsAlive)
                    ? CombatEncounterState.Terminal
                    : state;
            }
        }

        public CombatEncounterComposition(
            LoadedPlayerSession session,
            PlayerProfile playerProfile,
            CombatEncounterPreparation preparation)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
            PlayerProfile = playerProfile ?? throw new ArgumentNullException(nameof(playerProfile));
            this.preparation = preparation ?? throw new ArgumentNullException(nameof(preparation));
            ValidatePreparation();

            skillsById = new Dictionary<string, PreparedCombatSkillBinding>(StringComparer.Ordinal);
            foreach (var binding in preparation.Skills) skillsById.Add(binding.SkillId, binding);

            preparation.Claim();
            Engine = Session.CreateCombatEngine(preparation.Random, preparation.Passives);
            Runtime = new CombatRuntimeContext(
                preparation.Player, preparation.Enemy, preparation.EventLog,
                PlayerProfile, preparation.Resonance);
        }

        /// <summary>
        /// Returns true only for the first successful start. A thrown start is terminal
        /// for this owner; its original exception is retained and rethrown, without rollback.
        /// </summary>
        public bool Start()
        {
            if (state != CombatEncounterState.Prepared) return false;

            // Set before validation/hooks so a reentrant start cannot run a second time.
            state = CombatEncounterState.Starting;
            try
            {
                ValidatePreparation();
                Engine.StartCombat(Runtime);
                state = CombatEncounterState.Started;
                return true;
            }
            catch (Exception exception)
            {
                StartFailure = exception;
                state = CombatEncounterState.Faulted;
                throw;
            }
        }

        internal PreparedCombatSkillBinding GetSkillBinding(string skillId)
        {
            PreparedCombatSkillBinding binding;
            return !string.IsNullOrWhiteSpace(skillId) && skillsById.TryGetValue(skillId, out binding)
                ? binding
                : null;
        }

        private void ValidatePreparation()
        {
            if (Progression == null
                || Progression.ClassRegistry == null || Progression.SkillRegistry == null
                || Progression.MentorRegistry == null || Progression.ClassSkillTreeBindingRegistry == null)
            {
                throw new ArgumentException("An encounter requires the existing session's registries.");
            }

            preparation.Validate();
            foreach (var binding in preparation.Skills) binding.Validate(Progression.SkillRegistry);
        }
    }
}
