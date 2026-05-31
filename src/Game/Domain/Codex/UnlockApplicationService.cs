using System;
using System.Collections.Generic;
using Game.Domain.Common;

namespace Game.Domain.Codex
{
    public class UnlockApplicationService
    {
        public bool Apply(
            UnlockProfile profile,
            CodexUnlockCandidate candidate)
        {
            return Apply(profile, candidate, null);
        }

        public bool Apply(
            UnlockProfile profile,
            CodexUnlockCandidate candidate,
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

            if (!candidate.IsUnlocked)
            {
                return false;
            }

            bool recorded = profile.RecordUnlock(BuildRecord(candidate));

            if (recorded && domainEvents != null)
            {
                domainEvents.Record(
                    DomainEventType.UnlockRecorded,
                    candidate.UnlockId,
                    "Unlock recorded.",
                    new Dictionary<string, string>
                    {
                        { "unlockType", candidate.UnlockType.ToString() },
                        { "requiredEntryType", candidate.RequiredEntryType.HasValue ? candidate.RequiredEntryType.Value.ToString() : string.Empty },
                        { "requiredEntryId", candidate.RequiredEntryId },
                        { "requiredCount", candidate.RequiredCount.ToString() },
                        { "requiredPercentage", candidate.RequiredPercentage.ToString() }
                    });
            }

            return recorded;
        }

        public int Apply(
            UnlockProfile profile,
            IReadOnlyList<CodexUnlockCandidate> candidates)
        {
            return Apply(profile, candidates, null);
        }

        public int Apply(
            UnlockProfile profile,
            IReadOnlyList<CodexUnlockCandidate> candidates,
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

            int appliedCount = 0;

            foreach (var candidate in candidates)
            {
                if (Apply(profile, candidate, domainEvents))
                {
                    appliedCount++;
                }
            }

            return appliedCount;
        }

        private static UnlockRecord BuildRecord(CodexUnlockCandidate candidate)
        {
            return new UnlockRecord(
                candidate.UnlockId,
                candidate.UnlockType,
                candidate.RequiredEntryType,
                candidate.RequiredEntryId,
                candidate.RequiredCount,
                candidate.RequiredPercentage,
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
