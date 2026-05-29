using System.Collections.Generic;

namespace Game.Domain.Missions
{
    public class ActiveMissionRecord
    {
        public string TriggerId { get; private set; }
        public MissionTriggerType TriggerType { get; private set; }
        public MissionLifecycleState State { get; private set; }
        public int Priority { get; private set; }
        public string SuggestedMissionFamily { get; private set; }
        public string SourceHookId { get; private set; }
        public IReadOnlyList<string> NarrativeTags { get; private set; }

        public ActiveMissionRecord(
            string triggerId,
            MissionTriggerType triggerType,
            MissionLifecycleState state,
            int priority,
            string suggestedMissionFamily,
            string sourceHookId,
            IReadOnlyList<string> narrativeTags)
        {
            TriggerId = triggerId;
            TriggerType = triggerType;
            State = state;
            Priority = priority;
            SuggestedMissionFamily = suggestedMissionFamily;
            SourceHookId = sourceHookId;
            NarrativeTags = narrativeTags;
        }

        public bool Complete()
        {
            if (State == MissionLifecycleState.Completed)
            {
                return true;
            }

            if (State == MissionLifecycleState.Failed)
            {
                return false;
            }

            State = MissionLifecycleState.Completed;
            return true;
        }

        public bool Fail()
        {
            if (State == MissionLifecycleState.Failed)
            {
                return true;
            }

            if (State == MissionLifecycleState.Completed)
            {
                return false;
            }

            State = MissionLifecycleState.Failed;
            return true;
        }
    }
}
