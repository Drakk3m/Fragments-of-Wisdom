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

            if (HasTag(narrativeTags, "transmutation.access.allowed"))
            {
                hooks.Add(BuildHook(
                    "hook.transmutation.forbidden_practice",
                    NarrativeHookType.World,
                    90,
                    narrativeTags,
                    "World"));
            }

            if (HasTag(narrativeTags, "npc.reaction.wary"))
            {
                hooks.Add(BuildHook(
                    "hook.dialogue.npc_wary_forbidden_practice",
                    NarrativeHookType.Dialogue,
                    80,
                    narrativeTags,
                    "Dialogue"));
            }

            if (HasTag(narrativeTags, "nahual.first_teaching.accepted"))
            {
                hooks.Add(BuildHook(
                    "hook.codex.nahual_path",
                    NarrativeHookType.Codex,
                    70,
                    narrativeTags,
                    "Codex"));
            }

            if (HasTag(narrativeTags, "nahual.first_teaching.rejected"))
            {
                hooks.Add(BuildHook(
                    "hook.mission.shared_wisdom_followup",
                    NarrativeHookType.Mission,
                    60,
                    narrativeTags,
                    "Missions"));
            }

            if (HasTag(narrativeTags, "transmutation.access.blocked"))
            {
                hooks.Add(BuildHook(
                    "hook.dialogue.transmutation_blocked",
                    NarrativeHookType.Dialogue,
                    50,
                    narrativeTags,
                    "Dialogue"));
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
