using System;
using System.Collections.Generic;
using Game.Domain.Alignment;
using Game.Domain.Codex;
using Game.Domain.Missions;
using Game.Domain.Narrative;
using Game.Domain.Player;
using Game.Domain.Transmutation;
using Game.Domain.World;
using Game.Slice;

namespace Game.SmokeTests
{
    public static class Program
    {
        private static int assertionsExecuted;

        public static int Main()
        {
            var profile = new PlayerProfile();

            Console.WriteLine("Fragments of Wisdom smoke validation");
            Console.WriteLine();

            var log = FoundationVerticalSlice.Run();

            RunSection(
                "V1 slice emitted event count",
                delegate
                {
                    Assert(
                        log.Events.Count == 38,
                        "Foundation V1 event count changed.");

                    Assert(
                        log.Events.Count != 0,
                        "Foundation vertical slice produced no events.");

                    Console.WriteLine("  Events emitted: " + log.Events.Count);
                    Console.WriteLine("  Expected events: 38");
                });

            RunSection(
                "Alignment assertions",
                delegate
                {
                    Assert(
                        profile.CurrentAlignment == KnowledgeAlignment.SharedWisdom,
                        "Default alignment should be SharedWisdom.");

                    Assert(
                        profile.AlignmentProgression.GetProgress(KnowledgeAlignment.SharedWisdom) == 0,
                        "Default SharedWisdom progress should be zero.");

                    Assert(
                        profile.AlignmentProgression.GetProgress(KnowledgeAlignment.NahualPath) == 0,
                        "Default NahualPath progress should be zero.");

                    Assert(
                        profile.AlignmentFlags.Count == 0,
                        "Default alignment flags should be empty.");
                });

            RunSection(
                "Nahual evaluator assertions",
                delegate
                {
                    AssertDefaultNahualTeachingState(profile);
                    AssertAcceptedNahualTeachingState();
                    AssertRejectedNahualTeachingState();
                });

            RunSection(
                "Transmutation policy assertions",
                delegate
                {
                    AssertDefaultTransmutationAccess(profile);
                    AssertAcceptedTransmutationAccess();
                    AssertRejectedTransmutationAccess();
                });

            RunSection(
                "NPC reaction assertions",
                delegate
                {
                    AssertNeutralNpcReaction(profile);
                    AssertAcceptedNahualNpcReaction();
                    AssertAllowedTransmutationNpcReaction();
                    AssertRejectedNahualNpcReaction();
                });

            RunSection(
                "Narrative hook assertions",
                delegate
                {
                    AssertAcceptedTeachingNarrativeHooks();
                    AssertAllowedTransmutationNarrativeHooks();
                    AssertWaryNpcReactionNarrativeHooks();
                    AssertRejectedTeachingNarrativeHooks();
                });

            RunSection(
                "Mission trigger assertions",
                delegate
                {
                    AssertSharedWisdomMissionTriggerCandidate();
                    AssertForbiddenPracticeMissionTriggerCandidate();
                    AssertBlockedTransmutationMissionTriggerCandidate();
                });

            RunSection(
                "Codex entry assertions",
                delegate
                {
                    AssertNahualPathCodexEntryCandidate();
                    AssertForbiddenPracticeCodexEntryCandidate();
                    AssertSharedWisdomCodexEntryCandidate();
                    AssertBlockedTransmutationCodexEntryCandidate();
                });

            RunSection(
                "Candidate deduplication assertions",
                delegate
                {
                    AssertCodexEntryCandidateDeduplication();
                    AssertMissionTriggerCandidateDeduplication();
                    AssertIntegratedCodexEntryCandidateDeduplication();
                });

            RunSection(
                "Codex profile assertions",
                delegate
                {
                    AssertCodexProfileDiscoversOneEntry();
                    AssertCodexProfileDuplicateDiscoveryIsIdempotent();
                    AssertCodexProfileDiscoversMultipleEntries();
                    AssertCodexProfilePreservesNarrativeTags();
                });

            RunSection(
                "V2 foundation flow assertions",
                delegate
                {
                    AssertDefaultSharedWisdomFlow();
                    AssertAcceptedForbiddenTeachingFlow();
                    AssertRejectedBlockedTransmutationFlow();
                });

            Console.WriteLine();
            Console.WriteLine("Total smoke assertions executed: " + assertionsExecuted);
            return 0;
        }

        private static void RunSection(
            string name,
            Action assertions)
        {
            int before = assertionsExecuted;

            Console.WriteLine(name);
            assertions();
            Console.WriteLine("  Assertions passed: " + (assertionsExecuted - before));
            Console.WriteLine();
        }

        private static void AssertDefaultNahualTeachingState(PlayerProfile profile)
        {
            var result = new FirstNahualTeachingEvaluator().Evaluate(profile);

            Assert(
                !result.AcceptedTeachingPath,
                "Default profile should not have accepted the first Nahual teaching.");

            Assert(
                !result.RejectedTeachingPath,
                "Default profile should not have rejected the first Nahual teaching.");
        }

        private static void AssertAcceptedNahualTeachingState()
        {
            var profile = new PlayerProfile();
            profile.SetAlignment(KnowledgeAlignment.NahualPath);
            profile.AddAlignmentProgress(KnowledgeAlignment.NahualPath, 1);
            profile.AddAlignmentFlag(AlignmentFlag.DiscoveredNahualPath);
            profile.AddAlignmentFlag(AlignmentFlag.AcceptedFirstNahualTeaching);

            var result = new FirstNahualTeachingEvaluator().Evaluate(profile);

            Assert(
                result.AcceptedTeachingPath,
                "Nahual-aligned profile should describe the accepted teaching path.");

            Assert(
                !result.RejectedTeachingPath,
                "Accepted Nahual teaching path should not also be rejected.");

            Assert(
                ContainsTag(result, NarrativeTags.NahualFirstTeachingAccepted),
                "Accepted result should include an accepted narrative tag.");
        }

        private static void AssertRejectedNahualTeachingState()
        {
            var profile = new PlayerProfile();
            profile.AddAlignmentProgress(KnowledgeAlignment.SharedWisdom, 1);
            profile.AddAlignmentFlag(AlignmentFlag.DiscoveredNahualPath);

            var result = new FirstNahualTeachingEvaluator().Evaluate(profile);

            Assert(
                !result.AcceptedTeachingPath,
                "Shared Wisdom profile should not describe the accepted teaching path.");

            Assert(
                result.RejectedTeachingPath,
                "Shared Wisdom profile with discovery progress should describe the rejected teaching path.");

            Assert(
                ContainsTag(result, NarrativeTags.NahualFirstTeachingRejected),
                "Rejected result should include a rejected narrative tag.");
        }

        private static bool ContainsTag(
            FirstNahualTeachingResult result,
            string tag)
        {
            return ContainsTag(result.NarrativeTags, tag);
        }

        private static bool ContainsTag(
            TransmutationAccessResult result,
            string tag)
        {
            return ContainsTag(result.NarrativeTags, tag);
        }

        private static bool ContainsTag(
            NpcReactionResult result,
            string tag)
        {
            return ContainsTag(result.NarrativeTags, tag);
        }

        private static bool ContainsTag(
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

        private static bool ContainsHook(
            NpcReactionResult result,
            string hook)
        {
            foreach (var availableHook in result.AvailableNarrativeHooks)
            {
                if (availableHook == hook)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsHook(
            IReadOnlyList<NarrativeHook> hooks,
            string hookId)
        {
            foreach (var hook in hooks)
            {
                if (hook.HookId == hookId)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsTrigger(
            IReadOnlyList<MissionTriggerCandidate> candidates,
            string triggerId)
        {
            foreach (var candidate in candidates)
            {
                if (candidate.TriggerId == triggerId)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsEntry(
            IReadOnlyList<CodexEntryCandidate> candidates,
            string entryId)
        {
            foreach (var candidate in candidates)
            {
                if (candidate.EntryId == entryId)
                {
                    return true;
                }
            }

            return false;
        }

        private static int CountEntries(
            IReadOnlyList<CodexEntryCandidate> candidates,
            string entryId)
        {
            int count = 0;

            foreach (var candidate in candidates)
            {
                if (candidate.EntryId == entryId)
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountTriggers(
            IReadOnlyList<MissionTriggerCandidate> candidates,
            string triggerId)
        {
            int count = 0;

            foreach (var candidate in candidates)
            {
                if (candidate.TriggerId == triggerId)
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountTags(
            IReadOnlyList<string> narrativeTags,
            string tag)
        {
            int count = 0;

            foreach (var narrativeTag in narrativeTags)
            {
                if (narrativeTag == tag)
                {
                    count++;
                }
            }

            return count;
        }

        private static void AssertDefaultTransmutationAccess(PlayerProfile profile)
        {
            var result = new TransmutationAccessPolicy().Evaluate(profile);

            Assert(
                !result.CanAccessTransmutation,
                "Default profile should not access transmutation.");

            Assert(
                result.RequiredAlignment == KnowledgeAlignment.NahualPath,
                "Blocked transmutation should require NahualPath alignment.");

            Assert(
                result.RequiredTeachingPath == NarrativeTags.NahualFirstTeachingAccepted,
                "Blocked transmutation should require accepted first Nahual teaching path.");
        }

        private static void AssertAcceptedTransmutationAccess()
        {
            var profile = new PlayerProfile();
            profile.SetAlignment(KnowledgeAlignment.NahualPath);
            profile.AddAlignmentProgress(KnowledgeAlignment.NahualPath, 1);
            profile.AddAlignmentFlag(AlignmentFlag.AcceptedFirstNahualTeaching);

            var result = new TransmutationAccessPolicy().Evaluate(profile);

            Assert(
                result.CanAccessTransmutation,
                "Accepted Nahual teaching path should access transmutation.");

            Assert(
                result.BlockReason == "",
                "Allowed transmutation access should not include a block reason.");

            Assert(
                ContainsTag(result, NarrativeTags.TransmutationAccessAllowed),
                "Allowed transmutation access should include an allowed narrative tag.");
        }

        private static void AssertRejectedTransmutationAccess()
        {
            var profile = new PlayerProfile();
            profile.AddAlignmentProgress(KnowledgeAlignment.SharedWisdom, 1);
            profile.AddAlignmentFlag(AlignmentFlag.DiscoveredNahualPath);

            var result = new TransmutationAccessPolicy().Evaluate(profile);

            Assert(
                !result.CanAccessTransmutation,
                "Rejected Nahual teaching path should not access transmutation.");

            Assert(
                result.BlockReason != "",
                "Blocked transmutation access should include a block reason.");

            Assert(
                ContainsTag(result, NarrativeTags.TransmutationAccessBlocked),
                "Blocked transmutation access should include a blocked narrative tag.");
        }

        private static void AssertNeutralNpcReaction(PlayerProfile profile)
        {
            var teachingResult = new FirstNahualTeachingEvaluator().Evaluate(profile);
            var result = new NpcReactionService().Resolve(teachingResult.NarrativeTags);

            Assert(
                result.ReactionTone == NpcReactionTone.Neutral,
                "Default narrative tags should produce neutral NPC reaction tone.");

            Assert(
                result.TrustDeltaPreview == 0,
                "Default NPC reaction should not preview trust changes.");

            Assert(
                result.FearDeltaPreview == 0,
                "Default NPC reaction should not preview fear changes.");
        }

        private static void AssertAcceptedNahualNpcReaction()
        {
            var profile = new PlayerProfile();
            profile.SetAlignment(KnowledgeAlignment.NahualPath);
            profile.AddAlignmentFlag(AlignmentFlag.AcceptedFirstNahualTeaching);

            var teachingResult = new FirstNahualTeachingEvaluator().Evaluate(profile);
            var result = new NpcReactionService().Resolve(teachingResult.NarrativeTags);

            Assert(
                result.ReactionTone == NpcReactionTone.Cautious,
                "Accepted Nahual teaching tags should produce cautious NPC reaction tone.");

            Assert(
                ContainsHook(result, NpcNarrativeHooks.NahualPath),
                "Accepted Nahual teaching reaction should expose the Nahual path hook.");
        }

        private static void AssertAllowedTransmutationNpcReaction()
        {
            var profile = new PlayerProfile();
            profile.SetAlignment(KnowledgeAlignment.NahualPath);
            profile.AddAlignmentFlag(AlignmentFlag.AcceptedFirstNahualTeaching);

            var accessResult = new TransmutationAccessPolicy().Evaluate(profile);
            var result = new NpcReactionService().Resolve(accessResult.NarrativeTags);

            Assert(
                result.ReactionTone == NpcReactionTone.Wary,
                "Allowed transmutation tags should produce wary NPC reaction tone.");

            Assert(
                result.TrustDeltaPreview < 0,
                "Allowed transmutation reaction should preview trust loss.");

            Assert(
                result.FearDeltaPreview > 0,
                "Allowed transmutation reaction should preview fear gain.");

            Assert(
                ContainsHook(result, NpcNarrativeHooks.ForbiddenPractice),
                "Allowed transmutation reaction should expose the forbidden practice hook.");
        }

        private static void AssertRejectedNahualNpcReaction()
        {
            var profile = new PlayerProfile();
            profile.AddAlignmentProgress(KnowledgeAlignment.SharedWisdom, 1);
            profile.AddAlignmentFlag(AlignmentFlag.DiscoveredNahualPath);

            var teachingResult = new FirstNahualTeachingEvaluator().Evaluate(profile);
            var result = new NpcReactionService().Resolve(teachingResult.NarrativeTags);

            Assert(
                result.ReactionTone == NpcReactionTone.Reassured,
                "Rejected Nahual teaching tags should produce reassured NPC reaction tone.");

            Assert(
                result.TrustDeltaPreview > 0,
                "Rejected Nahual teaching reaction should preview trust gain.");
        }

        private static void AssertAcceptedTeachingNarrativeHooks()
        {
            var profile = new PlayerProfile();
            profile.SetAlignment(KnowledgeAlignment.NahualPath);
            profile.AddAlignmentFlag(AlignmentFlag.AcceptedFirstNahualTeaching);

            var teachingResult = new FirstNahualTeachingEvaluator().Evaluate(profile);
            var hooks = new NarrativeHookService().Resolve(teachingResult);

            Assert(
                ContainsHook(hooks, NarrativeHookIds.CodexNahualPath),
                "Accepted Nahual teaching should produce the Nahual path codex hook.");

            Assert(
                hooks[0].SourceTags.Count > 0,
                "Narrative hooks should preserve source tags.");
        }

        private static void AssertAllowedTransmutationNarrativeHooks()
        {
            var profile = new PlayerProfile();
            profile.SetAlignment(KnowledgeAlignment.NahualPath);
            profile.AddAlignmentFlag(AlignmentFlag.AcceptedFirstNahualTeaching);

            var accessResult = new TransmutationAccessPolicy().Evaluate(profile);
            var hooks = new NarrativeHookService().Resolve(accessResult);

            Assert(
                ContainsHook(hooks, NarrativeHookIds.TransmutationForbiddenPractice),
                "Allowed transmutation should produce a forbidden practice world hook.");

            Assert(
                ContainsHook(hooks, NarrativeHookIds.CodexNahualPath),
                "Allowed transmutation should preserve the accepted Nahual codex hook.");
        }

        private static void AssertWaryNpcReactionNarrativeHooks()
        {
            var profile = new PlayerProfile();
            profile.SetAlignment(KnowledgeAlignment.NahualPath);
            profile.AddAlignmentFlag(AlignmentFlag.AcceptedFirstNahualTeaching);

            var accessResult = new TransmutationAccessPolicy().Evaluate(profile);
            var reactionResult = new NpcReactionService().Resolve(accessResult.NarrativeTags);
            var hooks = new NarrativeHookService().Resolve(reactionResult);

            Assert(
                ContainsHook(hooks, NarrativeHookIds.DialogueNpcWaryForbiddenPractice),
                "Wary NPC reaction should produce a wary dialogue hook.");

            Assert(
                hooks[0].Priority >= hooks[1].Priority,
                "Narrative hooks should be emitted in priority order for the current foundation rules.");
        }

        private static void AssertRejectedTeachingNarrativeHooks()
        {
            var profile = new PlayerProfile();
            profile.AddAlignmentProgress(KnowledgeAlignment.SharedWisdom, 1);
            profile.AddAlignmentFlag(AlignmentFlag.DiscoveredNahualPath);

            var teachingResult = new FirstNahualTeachingEvaluator().Evaluate(profile);
            var accessResult = new TransmutationAccessPolicy().Evaluate(teachingResult);
            var hooks = new NarrativeHookService().Resolve(accessResult);

            Assert(
                ContainsHook(hooks, NarrativeHookIds.MissionSharedWisdomFollowup),
                "Rejected Nahual teaching should produce a Shared Wisdom mission hook.");

            Assert(
                ContainsHook(hooks, NarrativeHookIds.DialogueTransmutationBlocked),
                "Blocked transmutation should produce a blocked-transmutation dialogue hook.");
        }

        private static void AssertSharedWisdomMissionTriggerCandidate()
        {
            var profile = new PlayerProfile();
            profile.AddAlignmentProgress(KnowledgeAlignment.SharedWisdom, 1);
            profile.AddAlignmentFlag(AlignmentFlag.DiscoveredNahualPath);

            var teachingResult = new FirstNahualTeachingEvaluator().Evaluate(profile);
            var accessResult = new TransmutationAccessPolicy().Evaluate(teachingResult);
            var hooks = new NarrativeHookService().Resolve(accessResult);
            var candidates = new MissionTriggerService().Resolve(hooks);

            Assert(
                ContainsTrigger(candidates, MissionTriggerIds.SharedWisdomFollowup),
                "Shared Wisdom hook should produce a Shared Wisdom follow-up trigger candidate.");

            Assert(
                candidates[0].SuggestedMissionFamily == MissionFamilies.SharedWisdom,
                "Shared Wisdom trigger should suggest the SharedWisdom mission family.");
        }

        private static void AssertForbiddenPracticeMissionTriggerCandidate()
        {
            var profile = new PlayerProfile();
            profile.SetAlignment(KnowledgeAlignment.NahualPath);
            profile.AddAlignmentFlag(AlignmentFlag.AcceptedFirstNahualTeaching);

            var accessResult = new TransmutationAccessPolicy().Evaluate(profile);
            var hooks = new NarrativeHookService().Resolve(accessResult);
            var candidates = new MissionTriggerService().Resolve(hooks);

            Assert(
                ContainsTrigger(candidates, MissionTriggerIds.NahualTransmutationDiscovery),
                "Forbidden practice hook should produce a Nahual transmutation discovery trigger candidate.");

            Assert(
                candidates[0].SourceHookId == NarrativeHookIds.TransmutationForbiddenPractice,
                "Forbidden practice trigger should preserve the source hook id.");
        }

        private static void AssertBlockedTransmutationMissionTriggerCandidate()
        {
            var profile = new PlayerProfile();
            var accessResult = new TransmutationAccessPolicy().Evaluate(profile);
            var hooks = new NarrativeHookService().Resolve(accessResult);
            var candidates = new MissionTriggerService().Resolve(hooks);

            Assert(
                ContainsTrigger(candidates, MissionTriggerIds.NahualTransmutationBlockedWarning),
                "Blocked transmutation hook should produce a blocked-transmutation warning trigger candidate.");

            Assert(
                candidates[0].NarrativeTags.Count > 0,
                "Mission trigger candidates should preserve narrative tags.");
        }

        private static void AssertNahualPathCodexEntryCandidate()
        {
            var profile = new PlayerProfile();
            profile.SetAlignment(KnowledgeAlignment.NahualPath);
            profile.AddAlignmentFlag(AlignmentFlag.AcceptedFirstNahualTeaching);

            var teachingResult = new FirstNahualTeachingEvaluator().Evaluate(profile);
            var hooks = new NarrativeHookService().Resolve(teachingResult);
            var candidates = new CodexEntryService().ResolveHooks(hooks);

            Assert(
                ContainsEntry(candidates, CodexEntryIds.NahualPath),
                "Accepted Nahual teaching codex hook should produce a Nahual path codex entry candidate.");

            Assert(
                candidates[0].SourceHookId == NarrativeHookIds.CodexNahualPath,
                "Nahual path codex entry candidate should preserve the source hook id.");
        }

        private static void AssertForbiddenPracticeCodexEntryCandidate()
        {
            var profile = new PlayerProfile();
            profile.SetAlignment(KnowledgeAlignment.NahualPath);
            profile.AddAlignmentFlag(AlignmentFlag.AcceptedFirstNahualTeaching);

            var accessResult = new TransmutationAccessPolicy().Evaluate(profile);
            var hooks = new NarrativeHookService().Resolve(accessResult);
            var candidates = new CodexEntryService().ResolveHooks(hooks);

            Assert(
                ContainsEntry(candidates, CodexEntryIds.ForbiddenTransmutation),
                "Forbidden practice hook should produce a forbidden transmutation codex entry candidate.");

            Assert(
                candidates[0].EntryType == CodexEntryType.Practice,
                "Forbidden transmutation codex entry candidate should be a practice entry.");
        }

        private static void AssertSharedWisdomCodexEntryCandidate()
        {
            var profile = new PlayerProfile();
            profile.AddAlignmentProgress(KnowledgeAlignment.SharedWisdom, 1);
            profile.AddAlignmentFlag(AlignmentFlag.DiscoveredNahualPath);

            var teachingResult = new FirstNahualTeachingEvaluator().Evaluate(profile);
            var accessResult = new TransmutationAccessPolicy().Evaluate(teachingResult);
            var hooks = new NarrativeHookService().Resolve(accessResult);
            var missionTriggerCandidates = new MissionTriggerService().Resolve(hooks);
            var candidates = new CodexEntryService().ResolveMissionTriggers(missionTriggerCandidates);

            Assert(
                ContainsEntry(candidates, CodexEntryIds.SharedWisdomFollowup),
                "Shared Wisdom mission trigger should produce a Shared Wisdom codex entry candidate.");

            Assert(
                candidates[0].SourceTriggerId == MissionTriggerIds.SharedWisdomFollowup,
                "Shared Wisdom codex entry candidate should preserve the source trigger id.");
        }

        private static void AssertBlockedTransmutationCodexEntryCandidate()
        {
            var profile = new PlayerProfile();
            var accessResult = new TransmutationAccessPolicy().Evaluate(profile);
            var hooks = new NarrativeHookService().Resolve(accessResult);
            var missionTriggerCandidates = new MissionTriggerService().Resolve(hooks);
            var candidates = new CodexEntryService().Resolve(hooks, missionTriggerCandidates);

            Assert(
                ContainsEntry(candidates, CodexEntryIds.TransmutationWarning),
                "Blocked transmutation facts should produce a transmutation warning codex entry candidate.");

            Assert(
                candidates[0].NarrativeTags.Count > 0,
                "Codex entry candidates should preserve narrative tags.");
        }

        private static void AssertCodexEntryCandidateDeduplication()
        {
            var candidates = new List<CodexEntryCandidate>();
            candidates.Add(new CodexEntryCandidate(
                CodexEntryIds.ForbiddenTransmutation,
                CodexEntryType.Practice,
                10,
                NarrativeHookIds.TransmutationForbiddenPractice,
                "",
                CodexConceptKeys.ForbiddenTransmutation,
                new List<string>
                {
                    NarrativeTags.NahualFirstTeachingAccepted,
                    NarrativeTags.TransmutationAccessAllowed
                }));
            candidates.Add(new CodexEntryCandidate(
                CodexEntryIds.ForbiddenTransmutation,
                CodexEntryType.Practice,
                90,
                "",
                MissionTriggerIds.NahualTransmutationDiscovery,
                CodexConceptKeys.ForbiddenTransmutation,
                new List<string>
                {
                    NarrativeTags.TransmutationAccessAllowed,
                    NarrativeTags.NpcReactionWary
                }));

            var result = new CodexEntryCandidateDeduplicator().Deduplicate(candidates);

            Assert(
                result.Count == 1,
                "Duplicate codex entry candidates should collapse into one result.");

            Assert(
                result[0].Priority == 90,
                "Deduplicated codex entry candidate should preserve the highest priority.");

            Assert(
                result[0].SourceTriggerId == MissionTriggerIds.NahualTransmutationDiscovery,
                "Deduplicated codex entry candidate should preserve fields from the highest-priority source.");

            Assert(
                CountTags(result[0].NarrativeTags, NarrativeTags.TransmutationAccessAllowed) == 1,
                "Deduplicated codex entry candidate should merge narrative tags without duplicates.");
        }

        private static void AssertMissionTriggerCandidateDeduplication()
        {
            var candidates = new List<MissionTriggerCandidate>();
            candidates.Add(new MissionTriggerCandidate(
                MissionTriggerIds.NahualTransmutationDiscovery,
                MissionTriggerType.Discovery,
                30,
                MissionFamilies.NahualPath,
                NarrativeHookIds.TransmutationForbiddenPractice,
                new List<string>
                {
                    NarrativeTags.TransmutationAccessAllowed,
                    NarrativeTags.NahualFirstTeachingAccepted
                }));
            candidates.Add(new MissionTriggerCandidate(
                MissionTriggerIds.NahualTransmutationDiscovery,
                MissionTriggerType.Discovery,
                80,
                MissionFamilies.NahualPath,
                NarrativeHookIds.TransmutationForbiddenPractice,
                new List<string>
                {
                    NarrativeTags.TransmutationAccessAllowed,
                    NarrativeTags.NpcReactionWary
                }));

            var result = new MissionTriggerCandidateDeduplicator().Deduplicate(candidates);

            Assert(
                result.Count == 1,
                "Duplicate mission trigger candidates should collapse into one result.");

            Assert(
                result[0].Priority == 80,
                "Deduplicated mission trigger candidate should preserve the highest priority.");

            Assert(
                CountTags(result[0].NarrativeTags, NarrativeTags.TransmutationAccessAllowed) == 1,
                "Deduplicated mission trigger candidate should merge narrative tags without duplicates.");
        }

        private static void AssertIntegratedCodexEntryCandidateDeduplication()
        {
            var profile = new PlayerProfile();
            var accessResult = new TransmutationAccessPolicy().Evaluate(profile);
            var hooks = new NarrativeHookService().Resolve(accessResult);
            var missionTriggerCandidates = new MissionTriggerService().Resolve(hooks);
            var candidates = new CodexEntryService().Resolve(hooks, missionTriggerCandidates);

            Assert(
                CountEntries(candidates, CodexEntryIds.TransmutationWarning) == 1,
                "Codex entry service should collapse duplicate transmutation warning candidates.");

            Assert(
                CountTriggers(missionTriggerCandidates, MissionTriggerIds.NahualTransmutationBlockedWarning) == 1,
                "Mission trigger service should keep blocked transmutation trigger candidates stable.");
        }

        private static void AssertCodexProfileDiscoversOneEntry()
        {
            var profile = new CodexProfile();
            var candidate = BuildCodexCandidate(
                CodexEntryIds.NahualPath,
                CodexEntryType.Concept,
                CodexConceptKeys.NahualPath,
                NarrativeHookIds.CodexNahualPath,
                "",
                NarrativeTags.NahualFirstTeachingAccepted);

            bool discovered = new CodexDiscoveryService().Discover(profile, candidate);
            var record = profile.GetDiscoveredEntry(CodexEntryIds.NahualPath);

            Assert(
                discovered,
                "Codex discovery should report a new entry discovery.");

            Assert(
                profile.IsDiscovered(CodexEntryIds.NahualPath),
                "Codex profile should track the discovered entry id.");

            Assert(
                record != null && record.ConceptKey == CodexConceptKeys.NahualPath,
                "Codex profile should preserve the discovered entry concept key.");
        }

        private static void AssertCodexProfileDuplicateDiscoveryIsIdempotent()
        {
            var profile = new CodexProfile();
            var candidate = BuildCodexCandidate(
                CodexEntryIds.ForbiddenTransmutation,
                CodexEntryType.Practice,
                CodexConceptKeys.ForbiddenTransmutation,
                NarrativeHookIds.TransmutationForbiddenPractice,
                "",
                NarrativeTags.TransmutationAccessAllowed);

            bool firstDiscovery = new CodexDiscoveryService().Discover(profile, candidate);
            bool duplicateDiscovery = new CodexDiscoveryService().Discover(profile, candidate);

            Assert(
                firstDiscovery,
                "First codex discovery should be applied.");

            Assert(
                !duplicateDiscovery,
                "Duplicate codex discovery should be idempotent.");

            Assert(
                profile.DiscoveredEntries.Count == 1,
                "Duplicate codex discovery should not add another entry record.");
        }

        private static void AssertCodexProfileDiscoversMultipleEntries()
        {
            var profile = new CodexProfile();
            var candidates = new List<CodexEntryCandidate>();
            candidates.Add(BuildCodexCandidate(
                CodexEntryIds.NahualPath,
                CodexEntryType.Concept,
                CodexConceptKeys.NahualPath,
                NarrativeHookIds.CodexNahualPath,
                "",
                NarrativeTags.NahualFirstTeachingAccepted));
            candidates.Add(BuildCodexCandidate(
                CodexEntryIds.TransmutationWarning,
                CodexEntryType.Warning,
                CodexConceptKeys.TransmutationWarning,
                NarrativeHookIds.DialogueTransmutationBlocked,
                MissionTriggerIds.NahualTransmutationBlockedWarning,
                NarrativeTags.TransmutationAccessBlocked));

            int discoveredCount = new CodexDiscoveryService().Discover(profile, candidates);

            Assert(
                discoveredCount == 2,
                "Codex discovery should apply multiple new entry candidates.");

            Assert(
                profile.IsDiscovered(CodexEntryIds.NahualPath)
                && profile.IsDiscovered(CodexEntryIds.TransmutationWarning),
                "Codex profile should track multiple discovered entry ids.");
        }

        private static void AssertCodexProfilePreservesNarrativeTags()
        {
            var profile = new CodexProfile();
            var candidate = BuildCodexCandidate(
                CodexEntryIds.SharedWisdomFollowup,
                CodexEntryType.FollowUp,
                CodexConceptKeys.SharedWisdom,
                NarrativeHookIds.MissionSharedWisdomFollowup,
                MissionTriggerIds.SharedWisdomFollowup,
                NarrativeTags.NahualFirstTeachingRejected);

            new CodexDiscoveryService().Discover(profile, candidate);
            var record = profile.GetDiscoveredEntry(CodexEntryIds.SharedWisdomFollowup);

            Assert(
                record != null && ContainsTag(record.NarrativeTags, NarrativeTags.NahualFirstTeachingRejected),
                "Codex profile should preserve narrative tags on discovery records.");

            Assert(
                record != null && record.FirstSourceTriggerId == MissionTriggerIds.SharedWisdomFollowup,
                "Codex profile should preserve the first discovery source trigger.");
        }

        private static CodexEntryCandidate BuildCodexCandidate(
            string entryId,
            CodexEntryType entryType,
            string conceptKey,
            string sourceHookId,
            string sourceTriggerId,
            string narrativeTag)
        {
            var narrativeTags = new List<string>();
            narrativeTags.Add(narrativeTag);

            return new CodexEntryCandidate(
                entryId,
                entryType,
                10,
                sourceHookId,
                sourceTriggerId,
                conceptKey,
                narrativeTags);
        }

        private static void AssertDefaultSharedWisdomFlow()
        {
            Console.WriteLine("  Flow scenario: default SharedWisdom player");

            var profile = new PlayerProfile();
            var teachingResult = new FirstNahualTeachingEvaluator().Evaluate(profile);
            var accessResult = new TransmutationAccessPolicy().Evaluate(teachingResult);
            var reactionResult = new NpcReactionService().Resolve(accessResult.NarrativeTags);
            var hooks = new NarrativeHookService().Resolve(reactionResult);
            var candidates = new MissionTriggerService().Resolve(hooks);

            Assert(
                profile.CurrentAlignment == KnowledgeAlignment.SharedWisdom,
                "Default flow should begin from SharedWisdom alignment.");

            Assert(
                !teachingResult.AcceptedTeachingPath,
                "Default flow should not accept the first Nahual teaching.");

            Assert(
                !teachingResult.RejectedTeachingPath,
                "Default flow should not reject an undiscovered Nahual teaching.");

            Assert(
                !accessResult.CanAccessTransmutation,
                "Default flow should block transmutation access.");

            Assert(
                reactionResult.ReactionTone == NpcReactionTone.Neutral,
                "Default flow should produce neutral NPC reaction.");

            Assert(
                hooks.Count == 1,
                "Default flow should preserve the blocked-transmutation dialogue hook only.");

            Assert(
                ContainsTrigger(candidates, MissionTriggerIds.NahualTransmutationBlockedWarning),
                "Default flow should produce a blocked transmutation warning trigger.");
        }

        private static void AssertAcceptedForbiddenTeachingFlow()
        {
            Console.WriteLine("  Flow scenario: accepted forbidden/Nahual teaching path");

            var profile = new PlayerProfile();
            profile.SetAlignment(KnowledgeAlignment.NahualPath);
            profile.AddAlignmentProgress(KnowledgeAlignment.NahualPath, 1);
            profile.AddAlignmentFlag(AlignmentFlag.DiscoveredNahualPath);
            profile.AddAlignmentFlag(AlignmentFlag.AcceptedFirstNahualTeaching);

            var teachingResult = new FirstNahualTeachingEvaluator().Evaluate(profile);
            var accessResult = new TransmutationAccessPolicy().Evaluate(teachingResult);
            var reactionResult = new NpcReactionService().Resolve(accessResult.NarrativeTags);
            var hooks = new NarrativeHookService().Resolve(reactionResult);
            var candidates = new MissionTriggerService().Resolve(hooks);

            Assert(
                teachingResult.AcceptedTeachingPath,
                "Accepted forbidden teaching flow should accept the Nahual teaching path.");

            Assert(
                accessResult.CanAccessTransmutation,
                "Accepted forbidden teaching flow should allow transmutation access.");

            Assert(
                ContainsTag(accessResult, NarrativeTags.TransmutationAccessAllowed),
                "Accepted forbidden teaching flow should carry the allowed transmutation tag.");

            Assert(
                reactionResult.ReactionTone == NpcReactionTone.Wary,
                "Accepted forbidden teaching flow should produce wary NPC reaction.");

            Assert(
                ContainsHook(reactionResult, NpcNarrativeHooks.ForbiddenPractice),
                "Accepted forbidden teaching flow should expose the forbidden practice NPC hook.");

            Assert(
                ContainsHook(hooks, NarrativeHookIds.TransmutationForbiddenPractice),
                "Accepted forbidden teaching flow should produce the forbidden practice narrative hook.");

            Assert(
                ContainsTrigger(candidates, MissionTriggerIds.NahualTransmutationDiscovery),
                "Accepted forbidden teaching flow should produce a transmutation discovery trigger.");
        }

        private static void AssertRejectedBlockedTransmutationFlow()
        {
            Console.WriteLine("  Flow scenario: rejected/blocked transmutation path");

            var profile = new PlayerProfile();
            profile.AddAlignmentProgress(KnowledgeAlignment.SharedWisdom, 1);
            profile.AddAlignmentFlag(AlignmentFlag.DiscoveredNahualPath);

            var teachingResult = new FirstNahualTeachingEvaluator().Evaluate(profile);
            var accessResult = new TransmutationAccessPolicy().Evaluate(teachingResult);
            var reactionResult = new NpcReactionService().Resolve(accessResult.NarrativeTags);
            var hooks = new NarrativeHookService().Resolve(reactionResult);
            var candidates = new MissionTriggerService().Resolve(hooks);

            Assert(
                teachingResult.RejectedTeachingPath,
                "Rejected flow should mark the first Nahual teaching as rejected.");

            Assert(
                !accessResult.CanAccessTransmutation,
                "Rejected flow should block transmutation access.");

            Assert(
                ContainsTag(accessResult, NarrativeTags.TransmutationAccessBlocked),
                "Rejected flow should carry the blocked transmutation tag.");

            Assert(
                reactionResult.ReactionTone == NpcReactionTone.Reassured,
                "Rejected flow should produce reassured NPC reaction.");

            Assert(
                ContainsHook(reactionResult, NpcNarrativeHooks.SharedWisdom),
                "Rejected flow should expose the Shared Wisdom NPC hook.");

            Assert(
                ContainsHook(hooks, NarrativeHookIds.MissionSharedWisdomFollowup),
                "Rejected flow should produce the Shared Wisdom follow-up narrative hook.");

            Assert(
                ContainsTrigger(candidates, MissionTriggerIds.SharedWisdomFollowup),
                "Rejected flow should produce the Shared Wisdom follow-up trigger.");
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }

            assertionsExecuted++;
        }
    }
}
