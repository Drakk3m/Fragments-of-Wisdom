using System;
using System.Collections.Generic;

namespace Game.Domain.Codex
{
    public class CodexDiscoveryService
    {
        public bool Discover(
            CodexProfile profile,
            CodexEntryCandidate candidate)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            if (candidate == null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            return profile.DiscoverEntry(BuildRecord(candidate));
        }

        public int Discover(
            CodexProfile profile,
            IReadOnlyList<CodexEntryCandidate> candidates)
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

        private static CodexEntryRecord BuildRecord(CodexEntryCandidate candidate)
        {
            return new CodexEntryRecord(
                candidate.EntryId,
                candidate.EntryType,
                candidate.ConceptKey,
                candidate.SourceHookId,
                candidate.SourceTriggerId,
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
