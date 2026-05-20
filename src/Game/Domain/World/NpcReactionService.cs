using System;
using System.Collections.Generic;
using Game.Domain.Narrative;

namespace Game.Domain.World
{
    public class NpcReactionService
    {
        public NpcReactionResult Resolve(IReadOnlyList<string> narrativeTags)
        {
            return Resolve(new NpcReactionContext(narrativeTags));
        }

        public NpcReactionResult Resolve(NpcReactionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (HasTag(context.NarrativeTags, NarrativeTags.TransmutationAccessAllowed))
            {
                return BuildResult(
                    context.NarrativeTags,
                    NpcReactionTone.Wary,
                    -2,
                    2,
                    NpcNarrativeHooks.ForbiddenPractice,
                    NarrativeTags.NpcReactionWary);
            }

            if (HasTag(context.NarrativeTags, NarrativeTags.NahualFirstTeachingAccepted))
            {
                return BuildResult(
                    context.NarrativeTags,
                    NpcReactionTone.Cautious,
                    -1,
                    1,
                    NpcNarrativeHooks.NahualPath,
                    NarrativeTags.NpcReactionCautious);
            }

            if (HasTag(context.NarrativeTags, NarrativeTags.NahualFirstTeachingRejected))
            {
                return BuildResult(
                    context.NarrativeTags,
                    NpcReactionTone.Reassured,
                    1,
                    0,
                    NpcNarrativeHooks.SharedWisdom,
                    NarrativeTags.NpcReactionReassured);
            }

            return BuildResult(
                context.NarrativeTags,
                NpcReactionTone.Neutral,
                0,
                0,
                NpcNarrativeHooks.None,
                NarrativeTags.NpcReactionNeutral);
        }

        private static NpcReactionResult BuildResult(
            IReadOnlyList<string> sourceTags,
            NpcReactionTone tone,
            int trustDeltaPreview,
            int fearDeltaPreview,
            string hook,
            string reactionTag)
        {
            var hooks = new List<string>();
            hooks.Add(hook);

            var tags = new List<string>();

            foreach (var sourceTag in sourceTags)
            {
                tags.Add(sourceTag);
            }

            tags.Add(reactionTag);

            return new NpcReactionResult(
                tone,
                trustDeltaPreview,
                fearDeltaPreview,
                hooks,
                tags);
        }

        private static bool HasTag(
            IReadOnlyList<string> narrativeTags,
            string tag)
        {
            foreach (var narrativeTag in narrativeTags)
            {
                if (narrativeTag == tag)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
