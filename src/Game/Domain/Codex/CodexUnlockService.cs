using System;
using System.Collections.Generic;

namespace Game.Domain.Codex
{
    public class CodexUnlockService
    {
        public IReadOnlyList<CodexUnlockCandidate> Resolve(CodexProgressSummary summary)
        {
            if (summary == null)
            {
                throw new ArgumentNullException(nameof(summary));
            }

            var candidates = new List<CodexUnlockCandidate>();

            candidates.Add(new CodexUnlockCandidate(
                CodexUnlockIds.NahualPathDiscovered,
                CodexUnlockType.EntryDiscovery,
                null,
                CodexEntryIds.NahualPath,
                0,
                0,
                summary.HasDiscoveredEntry(CodexEntryIds.NahualPath),
                EmptyTags()));

            candidates.Add(new CodexUnlockCandidate(
                CodexUnlockIds.ForbiddenPracticeCount,
                CodexUnlockType.EntryTypeCount,
                CodexEntryType.Practice,
                "",
                1,
                0,
                summary.GetDiscoveredCount(CodexEntryType.Practice) >= 1,
                EmptyTags()));

            candidates.Add(new CodexUnlockCandidate(
                CodexUnlockIds.HalfCatalogueDiscovered,
                CodexUnlockType.CompletionPercentage,
                null,
                "",
                0,
                50,
                summary.CompletionPercentage >= 50,
                EmptyTags()));

            return candidates;
        }

        private static IReadOnlyList<string> EmptyTags()
        {
            return new List<string>();
        }
    }
}
