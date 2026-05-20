using System;
using System.Collections.Generic;

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

            if (HasTag(context.NarrativeTags, "transmutation.access.allowed"))
            {
                return BuildResult(
                    context.NarrativeTags,
                    NpcReactionTone.Wary,
                    -2,
                    2,
                    "npc.hook.forbidden_practice",
                    "npc.reaction.wary");
            }

            if (HasTag(context.NarrativeTags, "nahual.first_teaching.accepted"))
            {
                return BuildResult(
                    context.NarrativeTags,
                    NpcReactionTone.Cautious,
                    -1,
                    1,
                    "npc.hook.nahual_path",
                    "npc.reaction.cautious");
            }

            if (HasTag(context.NarrativeTags, "nahual.first_teaching.rejected"))
            {
                return BuildResult(
                    context.NarrativeTags,
                    NpcReactionTone.Reassured,
                    1,
                    0,
                    "npc.hook.shared_wisdom",
                    "npc.reaction.reassured");
            }

            return BuildResult(
                context.NarrativeTags,
                NpcReactionTone.Neutral,
                0,
                0,
                "npc.hook.none",
                "npc.reaction.neutral");
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
