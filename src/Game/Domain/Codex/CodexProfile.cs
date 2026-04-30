using System.Collections.Generic;

namespace Game.Domain.Codex
{
    public class CodexProfile
    {
        private readonly HashSet<string> discovered = new HashSet<string>();
        private readonly HashSet<string> completed = new HashSet<string>();

        public void DiscoverEntry(string entryId)
        {
            discovered.Add(entryId);
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
    }
}
