using System.Collections.Generic;

namespace Game.Domain.Codex
{
    public class CodexEntryRecord
    {
        public string EntryId { get; private set; }
        public CodexEntryType EntryType { get; private set; }
        public string ConceptKey { get; private set; }
        public string FirstSourceHookId { get; private set; }
        public string FirstSourceTriggerId { get; private set; }
        public IReadOnlyList<string> NarrativeTags { get; private set; }

        public CodexEntryRecord(
            string entryId,
            CodexEntryType entryType,
            string conceptKey,
            string firstSourceHookId,
            string firstSourceTriggerId,
            IReadOnlyList<string> narrativeTags)
        {
            EntryId = entryId;
            EntryType = entryType;
            ConceptKey = conceptKey;
            FirstSourceHookId = firstSourceHookId;
            FirstSourceTriggerId = firstSourceTriggerId;
            NarrativeTags = narrativeTags;
        }
    }
}
