using System.Collections.Generic;

namespace Game.Domain.Classes
{
    public class MentorRelationshipProfile
    {
        private readonly HashSet<string> discoveredMentorIds = new HashSet<string>();
        private readonly Dictionary<string, int> trustByMentorId = new Dictionary<string, int>();
        private readonly Dictionary<string, bool> availabilityByMentorId = new Dictionary<string, bool>();

        public IReadOnlyCollection<string> DiscoveredMentorIds
        {
            get { return discoveredMentorIds; }
        }

        public bool Discover(MentorDefinition mentor)
        {
            if (!IsValidMentor(mentor))
            {
                return false;
            }

            bool discovered = discoveredMentorIds.Add(mentor.Id);

            if (!trustByMentorId.ContainsKey(mentor.Id))
            {
                trustByMentorId.Add(mentor.Id, 0);
            }

            availabilityByMentorId[mentor.Id] = mentor.IsAvailable;
            return discovered;
        }

        public bool Discover(
            string mentorId,
            MentorDefinitionRegistry registry)
        {
            if (registry == null)
            {
                return false;
            }

            return Discover(registry.Get(mentorId));
        }

        public bool IsDiscovered(string mentorId)
        {
            return !string.IsNullOrWhiteSpace(mentorId)
                && discoveredMentorIds.Contains(mentorId);
        }

        public bool IsAvailable(string mentorId)
        {
            bool isAvailable;
            return IsDiscovered(mentorId)
                && availabilityByMentorId.TryGetValue(mentorId, out isAvailable)
                && isAvailable;
        }

        public int GetTrust(string mentorId)
        {
            int trust;
            return !string.IsNullOrWhiteSpace(mentorId)
                && trustByMentorId.TryGetValue(mentorId, out trust)
                    ? trust
                    : 0;
        }

        public bool IncreaseTrust(
            MentorDefinition mentor,
            int amount)
        {
            if (!IsValidMentor(mentor) || amount <= 0 || !IsDiscovered(mentor.Id))
            {
                return false;
            }

            trustByMentorId[mentor.Id] = GetTrust(mentor.Id) + amount;
            availabilityByMentorId[mentor.Id] = mentor.IsAvailable;
            return true;
        }

        public bool IncreaseTrust(
            string mentorId,
            int amount,
            MentorDefinitionRegistry registry)
        {
            if (registry == null)
            {
                return false;
            }

            return IncreaseTrust(
                registry.Get(mentorId),
                amount);
        }

        private static bool IsValidMentor(MentorDefinition mentor)
        {
            return mentor != null && mentor.IsValid();
        }
    }
}
