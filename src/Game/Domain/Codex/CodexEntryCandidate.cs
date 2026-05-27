using System.Collections.Generic;

namespace Game.Domain.Codex
{
    public class CodexEntryCandidate
    {
        public string EntryId { get; private set; }
        public CodexEntryType EntryType { get; private set; }
        public int Priority { get; private set; }
        public string SourceHookId { get; private set; }
        public string SourceTriggerId { get; private set; }
        public string ConceptKey { get; private set; }
        public IReadOnlyList<string> NarrativeTags { get; private set; }

        public CodexEntryCandidate(
            string entryId,
            CodexEntryType entryType,
            int priority,
            string sourceHookId,
            string sourceTriggerId,
            string conceptKey,
            IReadOnlyList<string> narrativeTags)
        {
            EntryId = entryId;
            EntryType = entryType;
            Priority = priority;
            SourceHookId = sourceHookId;
            SourceTriggerId = sourceTriggerId;
            ConceptKey = conceptKey;
            NarrativeTags = narrativeTags;
        }
    }
}
