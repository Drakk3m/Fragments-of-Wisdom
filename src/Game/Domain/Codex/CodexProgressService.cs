using System;
using System.Collections.Generic;

namespace Game.Domain.Codex
{
    public class CodexProgressService
    {
        public CodexProgressSummary Summarize(CodexProfile profile)
        {
            return Summarize(profile, 0);
        }

        public CodexProgressSummary Summarize(
            CodexProfile profile,
            int totalKnownEntries)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            var discoveredEntryIds = new List<string>();
            var discoveredCountByType = new Dictionary<CodexEntryType, int>();

            foreach (var entry in profile.DiscoveredEntries)
            {
                discoveredEntryIds.Add(entry.EntryId);

                if (!discoveredCountByType.ContainsKey(entry.EntryType))
                {
                    discoveredCountByType[entry.EntryType] = 0;
                }

                discoveredCountByType[entry.EntryType]++;
            }

            return new CodexProgressSummary(
                discoveredEntryIds,
                discoveredCountByType,
                totalKnownEntries);
        }
    }
}
