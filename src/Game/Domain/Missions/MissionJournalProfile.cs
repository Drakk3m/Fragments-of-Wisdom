using System.Collections.Generic;

namespace Game.Domain.Missions
{
    public class MissionJournalProfile
    {
        private readonly HashSet<string> availableLeadIds = new HashSet<string>();
        private readonly List<MissionLeadRecord> availableLeads = new List<MissionLeadRecord>();

        public IReadOnlyCollection<string> AvailableLeadIds
        {
            get { return availableLeadIds; }
        }

        public IReadOnlyList<MissionLeadRecord> AvailableLeads
        {
            get { return availableLeads; }
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
    }
}
