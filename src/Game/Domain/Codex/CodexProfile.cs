using System.Collections.Generic;

namespace Game.Domain.Codex
{
    public class CodexProfile
    {
        private readonly HashSet<string> discovered = new HashSet<string>();
        private readonly HashSet<string> completed = new HashSet<string>();
        private readonly List<CodexEntryRecord> discoveredEntries = new List<CodexEntryRecord>();

        public IReadOnlyCollection<string> DiscoveredEntryIds
        {
            get { return discovered; }
        }

        public IReadOnlyList<CodexEntryRecord> DiscoveredEntries
        {
            get { return discoveredEntries; }
        }

        public void DiscoverEntry(string entryId)
        {
            discovered.Add(entryId);
        }

        public bool DiscoverEntry(CodexEntryRecord entry)
        {
            if (entry == null)
            {
                return false;
            }

            if (discovered.Contains(entry.EntryId))
            {
                return false;
            }

            discovered.Add(entry.EntryId);
            discoveredEntries.Add(entry);
            return true;
        }

        public void CompleteEntry(string entryId)
        {
            discovered.Add(entryId);
            completed.Add(entryId);
        }

        public bool IsDiscovered(string entryId)
        {
            return discovered.Contains(entryId);
        }

        public bool IsCompleted(string entryId)
        {
            return completed.Contains(entryId);
        }

        public CodexEntryRecord GetDiscoveredEntry(string entryId)
        {
            foreach (var entry in discoveredEntries)
            {
                if (entry.EntryId == entryId)
                {
                    return entry;
                }
            }

            return null;
        }
    }
}
