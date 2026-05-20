using System.Collections.Generic;

namespace Game.Domain.World
{
    public class NpcReactionResult
    {
        public NpcReactionTone ReactionTone { get; private set; }
        public int TrustDeltaPreview { get; private set; }
        public int FearDeltaPreview { get; private set; }
        public IReadOnlyList<string> AvailableNarrativeHooks { get; private set; }
        public IReadOnlyList<string> NarrativeTags { get; private set; }

        public NpcReactionResult(
            NpcReactionTone reactionTone,
            int trustDeltaPreview,
            int fearDeltaPreview,
            IReadOnlyList<string> availableNarrativeHooks,
            IReadOnlyList<string> narrativeTags)
        {
            ReactionTone = reactionTone;
            TrustDeltaPreview = trustDeltaPreview;
            FearDeltaPreview = fearDeltaPreview;
            AvailableNarrativeHooks = availableNarrativeHooks;
            NarrativeTags = narrativeTags;
        }
    }
}
