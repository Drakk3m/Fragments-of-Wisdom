using System.Collections.Generic;
using Game.Domain.Alignment;

namespace Game.Domain.Transmutation
{
    public class TransmutationAccessResult
    {
        public bool CanAccessTransmutation { get; private set; }
        public string BlockReason { get; private set; }
        public KnowledgeAlignment RequiredAlignment { get; private set; }
        public string RequiredTeachingPath { get; private set; }
        public IReadOnlyList<string> NarrativeTags { get; private set; }

        public TransmutationAccessResult(
            bool canAccessTransmutation,
            string blockReason,
            KnowledgeAlignment requiredAlignment,
            string requiredTeachingPath,
            IReadOnlyList<string> narrativeTags)
        {
            CanAccessTransmutation = canAccessTransmutation;
            BlockReason = blockReason;
            RequiredAlignment = requiredAlignment;
            RequiredTeachingPath = requiredTeachingPath;
            NarrativeTags = narrativeTags;
        }
    }
}
