using System;
using System.Collections.Generic;
using Game.Domain.Missions;
using Game.Domain.Narrative;

namespace Game.Domain.Codex
{
    public class CodexEntryService
    {
        public IReadOnlyList<CodexEntryCandidate> Resolve(
            IReadOnlyList<NarrativeHook> hooks,
            IReadOnlyList<MissionTriggerCandidate> missionTriggerCandidates)
        {
            if (hooks == null)
            {
                throw new ArgumentNullException(nameof(hooks));
            }

            if (missionTriggerCandidates == null)
            {
                throw new ArgumentNullException(nameof(missionTriggerCandidates));
            }

            var candidates = new List<CodexEntryCandidate>();

            AddHookCandidates(hooks, candidates);
            AddMissionTriggerCandidates(missionTriggerCandidates, candidates);

            return candidates;
        }

        public IReadOnlyList<CodexEntryCandidate> ResolveHooks(
            IReadOnlyList<NarrativeHook> hooks)
        {
            if (hooks == null)
            {
                throw new ArgumentNullException(nameof(hooks));
            }

            var candidates = new List<CodexEntryCandidate>();
            AddHookCandidates(hooks, candidates);
            return candidates;
        }

        public IReadOnlyList<CodexEntryCandidate> ResolveMissionTriggers(
            IReadOnlyList<MissionTriggerCandidate> missionTriggerCandidates)
        {
            if (missionTriggerCandidates == null)
            {
                throw new ArgumentNullException(nameof(missionTriggerCandidates));
            }

            var candidates = new List<CodexEntryCandidate>();
            AddMissionTriggerCandidates(missionTriggerCandidates, candidates);
            return candidates;
        }

        private static void AddHookCandidates(
            IReadOnlyList<NarrativeHook> hooks,
            List<CodexEntryCandidate> candidates)
        {
            foreach (var hook in hooks)
            {
                AddHookCandidate(hook, candidates);
            }
        }

        private static void AddMissionTriggerCandidates(
            IReadOnlyList<MissionTriggerCandidate> missionTriggerCandidates,
            List<CodexEntryCandidate> candidates)
        {
            foreach (var missionTriggerCandidate in missionTriggerCandidates)
            {
                AddMissionTriggerCandidate(missionTriggerCandidate, candidates);
            }
        }

        private static void AddHookCandidate(
            NarrativeHook hook,
            List<CodexEntryCandidate> candidates)
        {
            if (hook.HookId == NarrativeHookIds.CodexNahualPath)
            {
                candidates.Add(BuildFromHook(
                    CodexEntryIds.NahualPath,
                    CodexEntryType.Concept,
                    CodexConceptKeys.NahualPath,
                    hook));
                return;
            }

            if (hook.HookId == NarrativeHookIds.TransmutationForbiddenPractice)
            {
                candidates.Add(BuildFromHook(
                    CodexEntryIds.ForbiddenTransmutation,
                    CodexEntryType.Practice,
                    CodexConceptKeys.ForbiddenTransmutation,
                    hook));
                return;
            }

            if (hook.HookId == NarrativeHookIds.DialogueTransmutationBlocked)
            {
                candidates.Add(BuildFromHook(
                    CodexEntryIds.TransmutationWarning,
                    CodexEntryType.Warning,
                    CodexConceptKeys.TransmutationWarning,
                    hook));
            }
        }

        private static void AddMissionTriggerCandidate(
            MissionTriggerCandidate missionTriggerCandidate,
            List<CodexEntryCandidate> candidates)
        {
            if (missionTriggerCandidate.TriggerId == MissionTriggerIds.SharedWisdomFollowup)
            {
                candidates.Add(BuildFromMissionTrigger(
                    CodexEntryIds.SharedWisdomFollowup,
                    CodexEntryType.FollowUp,
                    CodexConceptKeys.SharedWisdom,
                    missionTriggerCandidate));
                return;
            }

            if (missionTriggerCandidate.TriggerId == MissionTriggerIds.NahualTransmutationDiscovery)
            {
                candidates.Add(BuildFromMissionTrigger(
                    CodexEntryIds.ForbiddenTransmutation,
                    CodexEntryType.Practice,
                    CodexConceptKeys.ForbiddenTransmutation,
                    missionTriggerCandidate));
                return;
            }

            if (missionTriggerCandidate.TriggerId == MissionTriggerIds.NahualTransmutationBlockedWarning)
            {
                candidates.Add(BuildFromMissionTrigger(
                    CodexEntryIds.TransmutationWarning,
                    CodexEntryType.Warning,
                    CodexConceptKeys.TransmutationWarning,
                    missionTriggerCandidate));
            }
        }

        private static CodexEntryCandidate BuildFromHook(
            string entryId,
            CodexEntryType entryType,
            string conceptKey,
            NarrativeHook hook)
        {
            return new CodexEntryCandidate(
                entryId,
                entryType,
                hook.Priority,
                hook.HookId,
                "",
                conceptKey,
                CopyTags(hook.SourceTags));
        }

        private static CodexEntryCandidate BuildFromMissionTrigger(
            string entryId,
            CodexEntryType entryType,
            string conceptKey,
            MissionTriggerCandidate missionTriggerCandidate)
        {
            return new CodexEntryCandidate(
                entryId,
                entryType,
                missionTriggerCandidate.Priority,
                missionTriggerCandidate.SourceHookId,
                missionTriggerCandidate.TriggerId,
                conceptKey,
                CopyTags(missionTriggerCandidate.NarrativeTags));
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
