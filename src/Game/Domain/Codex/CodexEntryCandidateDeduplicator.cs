using System;
using System.Collections.Generic;

namespace Game.Domain.Codex
{
    public class CodexEntryCandidateDeduplicator
    {
        public IReadOnlyList<CodexEntryCandidate> Deduplicate(
            IReadOnlyList<CodexEntryCandidate> candidates)
        {
            if (candidates == null)
            {
                throw new ArgumentNullException(nameof(candidates));
            }

            var results = new List<CodexEntryCandidate>();

            foreach (var candidate in candidates)
            {
                int existingIndex = FindIndex(results, candidate.EntryId);

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
            IReadOnlyList<CodexEntryCandidate> candidates,
            string entryId)
        {
            for (int index = 0; index < candidates.Count; index++)
            {
                if (candidates[index].EntryId == entryId)
                {
                    return index;
                }
            }

            return -1;
        }

        private static CodexEntryCandidate Merge(
            CodexEntryCandidate existing,
            CodexEntryCandidate incoming)
        {
            var preferred = incoming.Priority > existing.Priority
                ? incoming
                : existing;

            return new CodexEntryCandidate(
                preferred.EntryId,
                preferred.EntryType,
                preferred.Priority,
                preferred.SourceHookId,
                preferred.SourceTriggerId,
                preferred.ConceptKey,
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
