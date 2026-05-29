using System.Collections.Generic;

namespace Game.Domain.Codex
{
    public class CodexProgressSummary
    {
        private readonly IReadOnlyList<string> discoveredEntryIds;
        private readonly IReadOnlyDictionary<CodexEntryType, int> discoveredCountByType;

        public int TotalDiscoveredEntries
        {
            get { return discoveredEntryIds.Count; }
        }

        public int TotalKnownEntries { get; private set; }

        public int CompletionPercentage
        {
            get
            {
                if (TotalKnownEntries <= 0)
                {
                    return 0;
                }

                return TotalDiscoveredEntries * 100 / TotalKnownEntries;
            }
        }

        public IReadOnlyList<string> DiscoveredEntryIds
        {
            get { return discoveredEntryIds; }
        }

        public IReadOnlyDictionary<CodexEntryType, int> DiscoveredCountByType
        {
            get { return discoveredCountByType; }
        }

        public CodexProgressSummary(
            IReadOnlyList<string> discoveredEntryIds,
            IReadOnlyDictionary<CodexEntryType, int> discoveredCountByType,
            int totalKnownEntries)
        {
            this.discoveredEntryIds = discoveredEntryIds;
            this.discoveredCountByType = discoveredCountByType;
            TotalKnownEntries = totalKnownEntries;
        }

        public bool HasDiscoveredEntry(string entryId)
        {
            foreach (var discoveredEntryId in discoveredEntryIds)
            {
                if (discoveredEntryId == entryId)
                {
                    return true;
                }
            }

            return false;
        }

        public int GetDiscoveredCount(CodexEntryType entryType)
        {
            if (discoveredCountByType.ContainsKey(entryType))
            {
                return discoveredCountByType[entryType];
            }

            return 0;
        }
    }
}
