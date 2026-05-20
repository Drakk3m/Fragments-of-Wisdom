using System;
using System.Collections.Generic;
using Game.Domain.Narrative;

namespace Game.Domain.Missions
{
    public class MissionTriggerService
    {
        public IReadOnlyList<MissionTriggerCandidate> Resolve(IReadOnlyList<NarrativeHook> hooks)
        {
            if (hooks == null)
            {
                throw new ArgumentNullException(nameof(hooks));
            }

            var candidates = new List<MissionTriggerCandidate>();

            foreach (var hook in hooks)
            {
                AddCandidate(hook, candidates);
            }

            return candidates;
        }

        public IReadOnlyList<MissionTriggerCandidate> Resolve(NarrativeHook hook)
        {
            if (hook == null)
            {
                throw new ArgumentNullException(nameof(hook));
            }

            var hooks = new List<NarrativeHook>();
            hooks.Add(hook);
            return Resolve(hooks);
        }

        private static void AddCandidate(
            NarrativeHook hook,
            List<MissionTriggerCandidate> candidates)
        {
            if (hook.HookId == "hook.mission.shared_wisdom_followup")
            {
                candidates.Add(BuildCandidate(
                    "trigger.shared_wisdom.followup",
                    MissionTriggerType.FollowUp,
                    hook.Priority,
                    "SharedWisdom",
                    hook));
                return;
            }

            if (hook.HookId == "hook.transmutation.forbidden_practice")
            {
                candidates.Add(BuildCandidate(
                    "trigger.nahual.transmutation_discovery",
                    MissionTriggerType.Discovery,
                    hook.Priority,
                    "NahualPath",
                    hook));
                return;
            }

            if (hook.HookId == "hook.dialogue.transmutation_blocked")
            {
                candidates.Add(BuildCandidate(
                    "trigger.nahual.transmutation_blocked_warning",
                    MissionTriggerType.Warning,
                    hook.Priority,
                    "NahualPath",
                    hook));
            }
        }

        private static MissionTriggerCandidate BuildCandidate(
            string triggerId,
            MissionTriggerType triggerType,
            int priority,
            string suggestedMissionFamily,
            NarrativeHook hook)
        {
            return new MissionTriggerCandidate(
                triggerId,
                triggerType,
                priority,
                suggestedMissionFamily,
                hook.HookId,
                CopyTags(hook.SourceTags));
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
    }
}
