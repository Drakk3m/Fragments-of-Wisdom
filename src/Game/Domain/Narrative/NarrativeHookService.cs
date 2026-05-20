using System;
using System.Collections.Generic;
using Game.Domain.Transmutation;
using Game.Domain.World;

namespace Game.Domain.Narrative
{
    public class NarrativeHookService
    {
        public IReadOnlyList<NarrativeHook> Resolve(FirstNahualTeachingResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return Resolve(result.NarrativeTags);
        }

        public IReadOnlyList<NarrativeHook> Resolve(TransmutationAccessResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return Resolve(result.NarrativeTags);
        }

        public IReadOnlyList<NarrativeHook> Resolve(NpcReactionResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return Resolve(result.NarrativeTags);
        }

        public IReadOnlyList<NarrativeHook> Resolve(IReadOnlyList<string> narrativeTags)
        {
            if (narrativeTags == null)
            {
                throw new ArgumentNullException(nameof(narrativeTags));
            }

            var hooks = new List<NarrativeHook>();

            if (HasTag(narrativeTags, NarrativeTags.TransmutationAccessAllowed))
            {
                hooks.Add(BuildHook(
                    NarrativeHookIds.TransmutationForbiddenPractice,
                    NarrativeHookType.World,
                    90,
                    narrativeTags,
                    SuggestedNarrativeSystem.World));
            }

            if (HasTag(narrativeTags, NarrativeTags.NpcReactionWary))
            {
                hooks.Add(BuildHook(
                    NarrativeHookIds.DialogueNpcWaryForbiddenPractice,
                    NarrativeHookType.Dialogue,
                    80,
                    narrativeTags,
                    SuggestedNarrativeSystem.Dialogue));
            }

            if (HasTag(narrativeTags, NarrativeTags.NahualFirstTeachingAccepted))
            {
                hooks.Add(BuildHook(
                    NarrativeHookIds.CodexNahualPath,
                    NarrativeHookType.Codex,
                    70,
                    narrativeTags,
                    SuggestedNarrativeSystem.Codex));
            }

            if (HasTag(narrativeTags, NarrativeTags.NahualFirstTeachingRejected))
            {
                hooks.Add(BuildHook(
                    NarrativeHookIds.MissionSharedWisdomFollowup,
                    NarrativeHookType.Mission,
                    60,
                    narrativeTags,
                    SuggestedNarrativeSystem.Missions));
            }

            if (HasTag(narrativeTags, NarrativeTags.TransmutationAccessBlocked))
            {
                hooks.Add(BuildHook(
                    NarrativeHookIds.DialogueTransmutationBlocked,
                    NarrativeHookType.Dialogue,
                    50,
                    narrativeTags,
                    SuggestedNarrativeSystem.Dialogue));
            }

            return hooks;
        }

        private static NarrativeHook BuildHook(
            string hookId,
            NarrativeHookType hookType,
            int priority,
            IReadOnlyList<string> sourceTags,
            string suggestedSystem)
        {
            return new NarrativeHook(
                hookId,
                hookType,
                priority,
                CopyTags(sourceTags),
                suggestedSystem);
        }

        private static IReadOnlyList<string> CopyTags(IReadOnlyList<string> sourceTags)
        {
            var tags = new List<string>();

            foreach (var sourceTag in sourceTags)
            {
                tags.Add(sourceTag);
            }

            return tags;
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
