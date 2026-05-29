using System.Collections.Generic;
using Game.Domain.Missions;

namespace Game.Domain.Progress
{
    public class V2ActiveMissionSnapshot
    {
        public string TriggerId { get; private set; }
        public MissionLifecycleState State { get; private set; }
        public MissionTriggerType TriggerType { get; private set; }
        public int Priority { get; private set; }
        public string SuggestedMissionFamily { get; private set; }
        public string SourceHookId { get; private set; }
        public IReadOnlyList<string> NarrativeTags { get; private set; }

        public V2ActiveMissionSnapshot(
            string triggerId,
            MissionLifecycleState state,
            MissionTriggerType triggerType,
            int priority,
            string suggestedMissionFamily,
            string sourceHookId,
            IReadOnlyList<string> narrativeTags)
        {
            TriggerId = triggerId;
            State = state;
            TriggerType = triggerType;
            Priority = priority;
            SuggestedMissionFamily = suggestedMissionFamily;
            SourceHookId = sourceHookId;
            NarrativeTags = narrativeTags;
        }
    }
}
