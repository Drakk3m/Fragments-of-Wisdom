using System;
using System.Collections.Generic;
using Game.Domain.Common;

namespace Game.Domain.Codex
{
    public class CodexDiscoveryService
    {
        public bool Discover(
            CodexProfile profile,
            CodexEntryCandidate candidate)
        {
            return Discover(profile, candidate, null);
        }

        public bool Discover(
            CodexProfile profile,
            CodexEntryCandidate candidate,
            DomainEventLog domainEvents)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            if (candidate == null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            bool discovered = profile.DiscoverEntry(BuildRecord(candidate));

            if (discovered && domainEvents != null)
            {
                domainEvents.Record(
                    DomainEventType.CodexEntryDiscovered,
                    candidate.EntryId,
                    "Codex entry discovered.",
                    new Dictionary<string, string>
                    {
                        { "entryType", candidate.EntryType.ToString() },
                        { "conceptKey", candidate.ConceptKey },
                        { "sourceHookId", candidate.SourceHookId },
                        { "sourceTriggerId", candidate.SourceTriggerId }
                    });
            }

            return discovered;
        }

        public int Discover(
            CodexProfile profile,
            IReadOnlyList<CodexEntryCandidate> candidates)
        {
            return Discover(profile, candidates, null);
        }

        public int Discover(
            CodexProfile profile,
            IReadOnlyList<CodexEntryCandidate> candidates,
            DomainEventLog domainEvents)
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
                if (Discover(profile, candidate, domainEvents))
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
