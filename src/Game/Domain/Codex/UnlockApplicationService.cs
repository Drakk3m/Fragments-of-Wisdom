using System;
using System.Collections.Generic;

namespace Game.Domain.Codex
{
    public class UnlockApplicationService
    {
        public bool Apply(
            UnlockProfile profile,
            CodexUnlockCandidate candidate)
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

            return profile.RecordUnlock(BuildRecord(candidate));
        }

        public int Apply(
            UnlockProfile profile,
            IReadOnlyList<CodexUnlockCandidate> candidates)
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
                if (Apply(profile, candidate))
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
