using System.Collections.Generic;
using Game.Domain.Codex;

namespace Game.Domain.Progress
{
    public class V2UnlockSnapshot
    {
        public string UnlockId { get; private set; }
        public CodexUnlockType UnlockType { get; private set; }
        public CodexEntryType? RequiredEntryType { get; private set; }
        public string RequiredEntryId { get; private set; }
        public int RequiredCount { get; private set; }
        public int RequiredPercentage { get; private set; }
        public IReadOnlyList<string> NarrativeTags { get; private set; }

        public V2UnlockSnapshot(
            string unlockId,
            CodexUnlockType unlockType,
            CodexEntryType? requiredEntryType,
            string requiredEntryId,
            int requiredCount,
            int requiredPercentage,
            IReadOnlyList<string> narrativeTags)
        {
            UnlockId = unlockId;
            UnlockType = unlockType;
            RequiredEntryType = requiredEntryType;
            RequiredEntryId = requiredEntryId;
            RequiredCount = requiredCount;
            RequiredPercentage = requiredPercentage;
            NarrativeTags = narrativeTags;
        }
    }
}
