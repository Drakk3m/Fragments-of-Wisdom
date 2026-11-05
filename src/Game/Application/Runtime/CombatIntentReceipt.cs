using Game.Domain.Common;

namespace Game.Application.Runtime
{
    public sealed class CombatIntentReceipt
    {
        public CombatIntentDisposition Disposition { get; }
        public CombatIntentAdmissionReason AdmissionReason { get; }
        public bool AcceptedForExecution { get; }
        public bool InvocationStarted { get; }
        public bool InvocationCompleted { get; }
        public CombatEncounterSnapshot Before { get; }
        public CombatEncounterSnapshot After { get; }
        public CombatEventBatch EventBatch { get; }
        public bool TurnStartedObserved { get; }
        public bool TurnEndedObserved { get; }
        public bool? EncounterIsTerminal { get; }
        // Raw authoritative refill return, not a turn-completion or success verdict.
        public bool? RefillResult { get; }
        public string FaultType { get; }
        public string FaultMessage { get; }
        public string ObservationFailureMessage { get; }

        internal CombatIntentReceipt(
            CombatIntentDisposition disposition, CombatIntentAdmissionReason admissionReason,
            bool invocationStarted, bool invocationCompleted,
            CombatEncounterSnapshot before, CombatEncounterSnapshot after, CombatEventBatch eventBatch,
            bool? refillResult = null, string faultType = null, string faultMessage = null,
            string observationFailureMessage = null)
        {
            Disposition = disposition;
            AdmissionReason = admissionReason;
            AcceptedForExecution = disposition != CombatIntentDisposition.AdmissionRejected;
            InvocationStarted = invocationStarted;
            InvocationCompleted = invocationCompleted;
            Before = before;
            After = after;
            EventBatch = eventBatch;
            RefillResult = refillResult;
            FaultType = faultType;
            FaultMessage = faultMessage;
            ObservationFailureMessage = observationFailureMessage;
            EncounterIsTerminal = after == null ? (bool?)null
                : after.CombatEnded || !after.Player.IsAlive || !after.Enemy.IsAlive;
            foreach (var item in eventBatch.Events)
            {
                if (item.Type == GameEventType.TurnStarted) TurnStartedObserved = true;
                if (item.Type == GameEventType.TurnEnded) TurnEndedObserved = true;
            }
        }
    }
}
