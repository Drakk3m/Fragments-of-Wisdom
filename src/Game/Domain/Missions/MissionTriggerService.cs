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
            if (hook.HookId == NarrativeHookIds.MissionSharedWisdomFollowup)
            {
                candidates.Add(BuildCandidate(
                    MissionTriggerIds.SharedWisdomFollowup,
                    MissionTriggerType.FollowUp,
                    hook.Priority,
                    MissionFamilies.SharedWisdom,
                    hook));
                return;
            }

            if (hook.HookId == NarrativeHookIds.TransmutationForbiddenPractice)
            {
                candidates.Add(BuildCandidate(
                    MissionTriggerIds.NahualTransmutationDiscovery,
                    MissionTriggerType.Discovery,
                    hook.Priority,
                    MissionFamilies.NahualPath,
                    hook));
                return;
            }

            if (hook.HookId == NarrativeHookIds.DialogueTransmutationBlocked)
            {
                candidates.Add(BuildCandidate(
                    MissionTriggerIds.NahualTransmutationBlockedWarning,
                    MissionTriggerType.Warning,
                    hook.Priority,
                    MissionFamilies.NahualPath,
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
