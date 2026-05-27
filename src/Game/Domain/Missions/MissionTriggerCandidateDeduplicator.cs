using System;
using System.Collections.Generic;

namespace Game.Domain.Missions
{
    public class MissionTriggerCandidateDeduplicator
    {
        public IReadOnlyList<MissionTriggerCandidate> Deduplicate(
            IReadOnlyList<MissionTriggerCandidate> candidates)
        {
            if (candidates == null)
            {
                throw new ArgumentNullException(nameof(candidates));
            }

            var results = new List<MissionTriggerCandidate>();

            foreach (var candidate in candidates)
            {
                int existingIndex = FindIndex(results, candidate.TriggerId);

                if (existingIndex < 0)
                {
                    results.Add(candidate);
                    continue;
                }

                results[existingIndex] = Merge(results[existingIndex], candidate);
            }

            return results;
        }

        private static int FindIndex(
            IReadOnlyList<MissionTriggerCandidate> candidates,
            string triggerId)
        {
            for (int index = 0; index < candidates.Count; index++)
            {
                if (candidates[index].TriggerId == triggerId)
                {
                    return index;
                }
            }

            return -1;
        }

        private static MissionTriggerCandidate Merge(
            MissionTriggerCandidate existing,
            MissionTriggerCandidate incoming)
        {
            var preferred = incoming.Priority > existing.Priority
                ? incoming
                : existing;

            return new MissionTriggerCandidate(
                preferred.TriggerId,
                preferred.TriggerType,
                preferred.Priority,
                preferred.SuggestedMissionFamily,
                preferred.SourceHookId,
                MergeTags(existing.NarrativeTags, incoming.NarrativeTags));
        }

        private static IReadOnlyList<string> MergeTags(
            IReadOnlyList<string> existingTags,
            IReadOnlyList<string> incomingTags)
        {
            var tags = new List<string>();

            AddUniqueTags(tags, existingTags);
            AddUniqueTags(tags, incomingTags);

            return tags;
        }

        private static void AddUniqueTags(
            List<string> tags,
            IReadOnlyList<string> sourceTags)
        {
            foreach (var sourceTag in sourceTags)
            {
                if (!Contains(tags, sourceTag))
                {
                    tags.Add(sourceTag);
                }
            }
        }

        private static bool Contains(
            IReadOnlyList<string> tags,
            string tag)
        {
            foreach (var existingTag in tags)
            {
                if (existingTag == tag)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
