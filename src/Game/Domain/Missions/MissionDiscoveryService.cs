using System;
using System.Collections.Generic;

namespace Game.Domain.Missions
{
    public class MissionDiscoveryService
    {
        public bool Discover(
            MissionJournalProfile profile,
            MissionTriggerCandidate candidate)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            if (candidate == null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            return profile.DiscoverLead(BuildRecord(candidate));
        }

        public int Discover(
            MissionJournalProfile profile,
            IReadOnlyList<MissionTriggerCandidate> candidates)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            if (candidates == null)
            {
                throw new ArgumentNullException(nameof(candidates));
            }

            int discoveredCount = 0;

            foreach (var candidate in candidates)
            {
                if (Discover(profile, candidate))
                {
                    discoveredCount++;
                }
            }

            return discoveredCount;
        }

        private static MissionLeadRecord BuildRecord(MissionTriggerCandidate candidate)
        {
            return new MissionLeadRecord(
                candidate.TriggerId,
                candidate.TriggerType,
                candidate.Priority,
                candidate.SuggestedMissionFamily,
                candidate.SourceHookId,
                CopyTags(candidate.NarrativeTags));
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
