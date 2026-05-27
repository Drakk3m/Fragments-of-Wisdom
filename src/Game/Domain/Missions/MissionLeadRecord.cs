using System.Collections.Generic;

namespace Game.Domain.Missions
{
    public class MissionLeadRecord
    {
        public string TriggerId { get; private set; }
        public MissionTriggerType TriggerType { get; private set; }
        public int Priority { get; private set; }
        public string SuggestedMissionFamily { get; private set; }
        public string SourceHookId { get; private set; }
        public IReadOnlyList<string> NarrativeTags { get; private set; }

        public MissionLeadRecord(
            string triggerId,
            MissionTriggerType triggerType,
            int priority,
            string suggestedMissionFamily,
            string sourceHookId,
            IReadOnlyList<string> narrativeTags)
        {
            TriggerId = triggerId;
            TriggerType = triggerType;
            Priority = priority;
            SuggestedMissionFamily = suggestedMissionFamily;
            SourceHookId = sourceHookId;
            NarrativeTags = narrativeTags;
        }
    }
}
