using System.Collections.Generic;

namespace Game.Domain.Missions
{
    public class MissionJournalProfile
    {
        private readonly HashSet<string> availableLeadIds = new HashSet<string>();
        private readonly List<MissionLeadRecord> availableLeads = new List<MissionLeadRecord>();
        private readonly List<ActiveMissionRecord> activeMissions = new List<ActiveMissionRecord>();

        public IReadOnlyCollection<string> AvailableLeadIds
        {
            get { return availableLeadIds; }
        }

        public IReadOnlyList<MissionLeadRecord> AvailableLeads
        {
            get { return availableLeads; }
        }

        public IReadOnlyList<ActiveMissionRecord> ActiveMissions
        {
            get { return activeMissions; }
        }

        public bool DiscoverLead(MissionLeadRecord lead)
        {
            if (lead == null)
            {
                return false;
            }

            if (availableLeadIds.Contains(lead.TriggerId))
            {
                return false;
            }

            availableLeadIds.Add(lead.TriggerId);
            availableLeads.Add(lead);
            return true;
        }

        public bool IsLeadAvailable(string triggerId)
        {
            return availableLeadIds.Contains(triggerId);
        }

        public MissionLeadRecord GetAvailableLead(string triggerId)
        {
            foreach (var lead in availableLeads)
            {
                if (lead.TriggerId == triggerId)
                {
                    return lead;
                }
            }

            return null;
        }

        public bool PromoteLeadToActiveMission(string triggerId)
        {
            var lead = GetAvailableLead(triggerId);

            if (lead == null)
            {
                return false;
            }

            var mission = GetActiveMission(triggerId);

            if (mission != null)
            {
                return mission.State == MissionLifecycleState.Active;
            }

            activeMissions.Add(new ActiveMissionRecord(
                lead.TriggerId,
                lead.TriggerType,
                MissionLifecycleState.Active,
                lead.Priority,
                lead.SuggestedMissionFamily,
                lead.SourceHookId,
                CopyTags(lead.NarrativeTags)));

            return true;
        }

        public bool CompleteMission(string triggerId)
        {
            var mission = GetActiveMission(triggerId);

            if (mission == null)
            {
                return false;
            }

            return mission.Complete();
        }

        public bool FailMission(string triggerId)
        {
            var mission = GetActiveMission(triggerId);

            if (mission == null)
            {
                return false;
            }

            return mission.Fail();
        }

        public bool RestoreMission(ActiveMissionRecord mission)
        {
            if (mission == null)
            {
                return false;
            }

            if (GetActiveMission(mission.TriggerId) != null)
            {
                return false;
            }

            activeMissions.Add(new ActiveMissionRecord(
                mission.TriggerId,
                mission.TriggerType,
                mission.State,
                mission.Priority,
                mission.SuggestedMissionFamily,
                mission.SourceHookId,
                CopyTags(mission.NarrativeTags)));

            return true;
        }

        public ActiveMissionRecord GetActiveMission(string triggerId)
        {
            foreach (var mission in activeMissions)
            {
                if (mission.TriggerId == triggerId)
                {
                    return mission;
                }
            }

            return null;
        }

        private static IReadOnlyList<string> CopyTags(IReadOnlyList<string> sourceTags)
        {
            var tags = new List<string>();

            foreach (var sourceTag in sourceTags)
            {
                tags.Add(sourceTag);
            }

            return tags;
        }
    }
}
