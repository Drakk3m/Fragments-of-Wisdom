using System.Collections.Generic;
using Game.Domain.Codex;

namespace Game.Domain.Progress
{
    public class V2CodexEntrySnapshot
    {
        public string EntryId { get; private set; }
        public CodexEntryType EntryType { get; private set; }
        public string ConceptKey { get; private set; }
        public string FirstSourceHookId { get; private set; }
        public string FirstSourceTriggerId { get; private set; }
        public IReadOnlyList<string> NarrativeTags { get; private set; }

        public V2CodexEntrySnapshot(
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
