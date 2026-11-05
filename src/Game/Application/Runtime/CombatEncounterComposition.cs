using System;
using System.Collections.Generic;
using Game.Domain.Combat;
using Game.Domain.Combat.Resources;
using Game.Domain.Player;

namespace Game.Application.Runtime
{
    /// <summary>
    /// Single-threaded encounter owner. Mutable dependencies stay inside the application
    /// boundary; detached reads do not execute actions or persist cross-encounter state.
    /// </summary>
    public sealed class CombatEncounterComposition
    {
        private readonly CombatEncounterPreparation preparation;
        private readonly Dictionary<string, PreparedCombatSkillBinding> skillsById;
        private CombatEncounterState state = CombatEncounterState.Prepared;
        private bool commandInFlight;
        private bool startAttempted;
        private int startEventIndex;
        private int startEventEndIndex;

        internal LoadedPlayerSession Session { get; }
        internal PlayerProgressionRuntimeState Progression { get { return Session.RuntimeState; } }
        internal PlayerProfile PlayerProfile { get; }
        internal CombatRuntimeContext Runtime { get; }
        internal TurnBasedCombatEngine Engine { get; }

        public Exception StartFailure { get; private set; }
        public Exception ExecutionFailure { get; private set; }

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
            if (state != CombatEncounterState.Prepared || commandInFlight) return false;

            // Set before validation/hooks so a reentrant start cannot run a second time.
            state = CombatEncounterState.Starting;
            startAttempted = true;
            startEventIndex = Runtime.EventLog.Events.Count;
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
            finally
            {
                startEventEndIndex = Runtime.EventLog.Events.Count;
            }
        }

        /// <summary>
        /// Copies currently observable state without executing gameplay. Call between
        /// mutations on the owning thread; observation during start hooks is rejected.
        /// </summary>
        public CombatEncounterSnapshot GetSnapshot()
        {
            EnsureObservationReady();
            return CaptureSnapshot();
        }

        public CombatIntentReceipt SubmitIntent(CombatIntent intent)
        {
            if (commandInFlight || state == CombatEncounterState.Starting)
                return RejectIntent(CombatIntentAdmissionReason.CommandInFlight, false);

            commandInFlight = true;
            try
            {
                var reason = CheckAdmission(intent);
                if (reason != CombatIntentAdmissionReason.None) return RejectIntent(reason, true);

                int startIndex = Runtime.EventLog.Events.Count;
                CombatEncounterSnapshot before = null;
                bool invoked = false;
                bool completed = false;
                bool? refillResult = null;
                try
                {
                    before = CaptureSnapshot();
                    if (intent.Type == CombatIntentType.ResonanceRefill)
                    {
                        invoked = true;
                        refillResult = new ResonanceRefillAction().Execute(Runtime.ResonanceProfile, Runtime, intent.RefillAmount);
                    }
                    else
                    {
                        var binding = intent.Type == CombatIntentType.PreparedSkill ? GetSkillBinding(intent.SkillId) : null;
                        var request = binding == null
                            ? TurnRequest.BasicAttack(Runtime.Player, Runtime.Enemy)
                            : TurnRequest.SkillAction(Runtime.Player, Runtime.Enemy, binding.Skill, binding.Progress);
                        invoked = true;
                        Engine.ExecuteTurn(Runtime, request);
                    }
                    completed = true;
                    var after = CaptureSnapshot();
                    return new CombatIntentReceipt(CombatIntentDisposition.Completed, CombatIntentAdmissionReason.None,
                        invoked, completed, before, after, CopyEvents(startIndex, Runtime.EventLog.Events.Count), refillResult);
                }
                catch (Exception exception)
                {
                    ExecutionFailure = exception;
                    state = CombatEncounterState.Faulted;
                    string observationFailure;
                    var after = TryCaptureSnapshot(out observationFailure);
                    return new CombatIntentReceipt(CombatIntentDisposition.Faulted, CombatIntentAdmissionReason.None,
                        invoked, completed, before, after, CopyEvents(startIndex, Runtime.EventLog.Events.Count), refillResult,
                        exception.GetType().FullName, exception.Message, observationFailure);
                }
            }
            finally
            {
                commandInFlight = false;
            }
        }

        /// <summary>
        /// Pure replayable read. Process each index once and advance your per-owner cursor
        /// to EndIndex; requesting an earlier range intentionally replays observations only.
        /// </summary>
        public CombatEventBatch ReadEvents(int startIndex, int? endIndex = null)
        {
            EnsureObservationReady();
            int count = Runtime.EventLog.Events.Count;
            int end = endIndex ?? count;
            if (startIndex < 0 || startIndex > count) throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (end < startIndex || end > count) throw new ArgumentOutOfRangeException(nameof(endIndex));
            return CopyEvents(startIndex, end);
        }

        public CombatEventBatch ReadStartEvents()
        {
            EnsureObservationReady();
            if (!startAttempted) throw new InvalidOperationException("The encounter has not attempted start.");
            return CopyEvents(startEventIndex, startEventEndIndex);
        }

        private CombatIntentAdmissionReason CheckAdmission(CombatIntent intent)
        {
            if (state == CombatEncounterState.Faulted) return CombatIntentAdmissionReason.EncounterFaulted;
            if (state != CombatEncounterState.Started) return CombatIntentAdmissionReason.EncounterNotStarted;
            if (State == CombatEncounterState.Terminal) return CombatIntentAdmissionReason.EncounterTerminal;
            if (intent == null) return CombatIntentAdmissionReason.NullIntent;
            switch (intent.Type)
            {
                case CombatIntentType.BasicAttack:
                    return CombatIntentAdmissionReason.None;
                case CombatIntentType.PreparedSkill:
                    if (string.IsNullOrWhiteSpace(intent.SkillId)) return CombatIntentAdmissionReason.InvalidSkillId;
                    return GetSkillBinding(intent.SkillId) == null ? CombatIntentAdmissionReason.SkillNotPrepared : CombatIntentAdmissionReason.None;
                case CombatIntentType.ResonanceRefill:
                    return Runtime.ResonanceProfile == null ? CombatIntentAdmissionReason.ResonanceUnavailable : CombatIntentAdmissionReason.None;
                default:
                    return CombatIntentAdmissionReason.UnknownIntentType;
            }
        }

        private CombatIntentReceipt RejectIntent(CombatIntentAdmissionReason reason, bool capture)
        {
            int cursor = Runtime.EventLog.Events.Count;
            string observationFailure = null;
            var snapshot = capture ? TryCaptureSnapshot(out observationFailure) : null;
            return new CombatIntentReceipt(CombatIntentDisposition.AdmissionRejected, reason, false, false,
                snapshot, snapshot, CopyEvents(cursor, cursor), observationFailureMessage: observationFailure);
        }

        private CombatEncounterSnapshot CaptureSnapshot()
        {
            return new CombatEncounterSnapshot(State, Runtime, preparation.Skills, Progression.SkillRegistry);
        }

        private CombatEncounterSnapshot TryCaptureSnapshot(out string failure)
        {
            try { failure = null; return CaptureSnapshot(); }
            catch (Exception exception) { failure = exception.Message; return null; }
        }

        private CombatEventBatch CopyEvents(int startIndex, int endIndex)
        {
            return new CombatEventBatch(startIndex, endIndex, Runtime.EventLog.Events);
        }

        private void EnsureObservationReady()
        {
            if (state == CombatEncounterState.Starting || commandInFlight)
                throw new InvalidOperationException("Cannot observe an encounter while an operation is in flight.");
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
