using System;
using System.Collections.Generic;
using Game.Application.Runtime;

namespace Game.PresentationBoundary.SmokeTests
{
    // Test-only adapter: only one owner and presentation values, never mutable gameplay objects.
    internal sealed class PresentationConsumer
    {
        private readonly CombatEncounterComposition owner;
        private readonly List<CombatEventSnapshot> displayed = new List<CombatEventSnapshot>();
        private readonly List<string> transcript = new List<string>();
        public int Cursor { get; private set; }
        public CombatEncounterSnapshot Snapshot { get; private set; }
        public CombatTurnFlowSnapshot Flow { get; private set; }
        public IReadOnlyList<CombatEventSnapshot> Displayed { get { return displayed.AsReadOnly(); } }
        public IReadOnlyList<string> Transcript { get { return transcript.AsReadOnly(); } }

        public PresentationConsumer(CombatEncounterComposition owner) { this.owner = owner; Observe(); }

        public void Start(bool present = true)
        {
            if (!owner.Start()) throw new InvalidOperationException("Encounter did not start.");
            Observe();
            if (present) Process(owner.ReadStartEvents());
            Record("START");
        }

        public CombatIntentReceipt Submit(CombatIntent intent, bool present = true)
        {
            var receipt = owner.SubmitIntent(intent);
            if (present) Accept(receipt);
            return receipt;
        }

        public CombatIntentReceipt AdvanceEnemy(bool present = true)
        {
            var receipt = owner.AdvanceEnemyTurn();
            if (present) Accept(receipt);
            return receipt;
        }

        public void Accept(CombatIntentReceipt receipt, int failAfter = -1)
        {
            if (receipt.AcceptedForExecution)
            {
                if (Cursor < receipt.EventBatch.StartIndex) Process(owner.ReadEvents(Cursor, receipt.EventBatch.StartIndex));
                Process(receipt.EventBatch, failAfter);
            }
            // A rejection's empty current range is not acknowledgement of pending earlier events.
            Snapshot = receipt.After;
            Flow = receipt.FlowAfter;
            Record(receipt.ActorSide + " " + receipt.Disposition + " " + receipt.AdmissionReason
                + " consumed=" + receipt.OpportunityConsumed + " range=[" + receipt.EventBatch.StartIndex + "," + receipt.EventBatch.EndIndex + ")");
        }

        public void Recover()
        {
            Process(owner.ReadEvents(Cursor));
            Observe();
            Record("RECOVER");
        }

        public void Observe() { Snapshot = owner.GetSnapshot(); Flow = owner.GetTurnFlowSnapshot(); }

        private void Process(CombatEventBatch batch, int failAfter = -1)
        {
            // Stage display work first. A failed batch publishes nothing and acknowledges nothing.
            var pending = new List<CombatEventSnapshot>();
            int next = Cursor;
            foreach (var item in batch.Events)
            {
                if (item.Index < Cursor) continue;
                if (item.Index != next) throw new InvalidOperationException("Presentation event gap.");
                if (pending.Count == failAfter) throw new InvalidOperationException("Simulated presentation processing failure.");
                pending.Add(item);
                next++;
            }
            displayed.AddRange(pending);
            Cursor = next;
        }

        private void Record(string operation)
        {
            transcript.Add(operation + " -> " + Flow.Phase + " cursor=" + Cursor
                + " HP=" + Snapshot?.Player.CurrentHealth + "/" + Snapshot?.Enemy.CurrentHealth
                + " Essence=" + Snapshot?.Player.Essence.Current
                + " Resonance=" + (Snapshot?.PlayerResonance == null ? "absent" : Snapshot.PlayerResonance.Active.Current.ToString())
                + " XP=" + Snapshot?.PreparedSkills[0].Experience);
        }
    }
}
