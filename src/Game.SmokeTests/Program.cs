using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Game.Domain.Alignment;
using Game.Domain.Codex;
using Game.Domain.Discovery;
using Game.Domain.Missions;
using Game.Domain.Narrative;
using Game.Domain.Player;
using Game.Domain.Progress;
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
                "Codex progression assertions",
                delegate
                {
                    AssertCodexProgressEmptyProfileSummary();
                    AssertCodexProgressTotalDiscoveredCount();
                    AssertCodexProgressCountByEntryType();
                    AssertCodexProgressDuplicateDiscoveriesDoNotInflateProgress();
                });

            RunSection(
                "Mission journal assertions",
                delegate
                {
                    AssertMissionJournalDiscoversOneLead();
                    AssertMissionJournalDuplicateDiscoveryIsIdempotent();
                    AssertMissionJournalDiscoversMultipleLeads();
                    AssertMissionJournalPreservesNarrativeTagsAndFamily();
                });

            RunSection(
                "Mission lifecycle assertions",
                delegate
                {
                    AssertMissionLeadCanBecomeActiveMission();
                    AssertActiveMissionCanBeCompleted();
                    AssertActiveMissionCanBeFailed();
                    AssertDuplicateMissionPromotionIsSafe();
                    AssertTerminalMissionCannotBeOverwritten();
                });

            RunSection(
                "Discovery orchestration assertions",
                delegate
                {
                    AssertDiscoveryOrchestrationAppliesCodexAndMissionTogether();
                    AssertDiscoveryOrchestrationDuplicateReapplicationIsIdempotent();
                    AssertDiscoveryOrchestrationReportsDiscoveredAndAlreadyKnown();
                });

            RunSection(
                "V2 save snapshot assertions",
                delegate
                {
                    AssertV2SnapshotCapturesCodexEntries();
                    AssertV2SnapshotCapturesMissionLeads();
                    AssertV2SnapshotCapturesActiveMission();
                    AssertV2SnapshotCapturesCompletedMission();
                    AssertV2SnapshotCapturesFailedMission();
                    AssertV2SnapshotRestoreRecreatesEquivalentProfiles();
                    AssertV2SnapshotRestoreRecreatesLifecycleStates();
                    AssertV2SnapshotRestoreIsIdempotent();
                    AssertV2SnapshotLifecycleRestoreIsIdempotent();
                    AssertV2SnapshotUsesCurrentVersion();
                    AssertV2SnapshotCurrentVersionRestores();
                    AssertV2SnapshotUnsupportedVersionIsRejected();
                });

            RunSection(
                "V2 save JSON assertions",
                delegate
                {
                    AssertV2SnapshotSerializesToJson();
                    AssertV2SnapshotDeserializesEquivalentSnapshot();
                    AssertV2SnapshotUnsupportedVersionJsonIsRejected();
                    AssertV2SnapshotMalformedJsonFailsSafely();
                });

            RunSection(
                "V2 save file repository assertions",
                delegate
                {
                    AssertV2SnapshotFileRepositoryRoundtrip();
                    AssertV2SnapshotFileRepositoryCreatesNestedDirectory();
                    AssertV2SnapshotFileRepositoryMissingFileFailsSafely();
                    AssertV2SnapshotFileRepositoryMalformedFileFailsSafely();
                    AssertV2SnapshotFileRepositoryUnsupportedVersionFileFailsSafely();
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

        private static bool ContainsString(
            IReadOnlyList<string> values,
            string value)
        {
            foreach (var existingValue in values)
            {
                if (existingValue == value)
                {
                    return true;
                }
            }

            return false;
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

        private static void AssertCodexProgressEmptyProfileSummary()
        {
            var summary = new CodexProgressService().Summarize(new CodexProfile());

            Assert(
                summary.TotalDiscoveredEntries == 0,
                "Empty codex profile should report zero discovered entries.");

            Assert(
                summary.GetDiscoveredCount(CodexEntryType.Concept) == 0,
                "Empty codex profile should report zero discovered entries by type.");

            Assert(
                !summary.HasDiscoveredEntry(CodexEntryIds.NahualPath),
                "Empty codex profile should not report discovered entry ids.");
        }

        private static void AssertCodexProgressTotalDiscoveredCount()
        {
            var profile = BuildCodexProfileWithThreeEntries();
            var summary = new CodexProgressService().Summarize(profile, 6);

            Assert(
                summary.TotalDiscoveredEntries == 3,
                "Codex progress should report total discovered entries.");

            Assert(
                summary.HasDiscoveredEntry(CodexEntryIds.NahualPath),
                "Codex progress should report discovered entry ids.");

            Assert(
                summary.CompletionPercentage == 50,
                "Codex progress should calculate completion percentage only from externally supplied totals.");
        }

        private static void AssertCodexProgressCountByEntryType()
        {
            var profile = BuildCodexProfileWithThreeEntries();
            var summary = new CodexProgressService().Summarize(profile);

            Assert(
                summary.GetDiscoveredCount(CodexEntryType.Concept) == 1,
                "Codex progress should count discovered concept entries.");

            Assert(
                summary.GetDiscoveredCount(CodexEntryType.Practice) == 1,
                "Codex progress should count discovered practice entries.");

            Assert(
                summary.GetDiscoveredCount(CodexEntryType.Warning) == 1,
                "Codex progress should count discovered warning entries.");
        }

        private static void AssertCodexProgressDuplicateDiscoveriesDoNotInflateProgress()
        {
            var profile = new CodexProfile();
            var candidate = BuildCodexCandidate(
                CodexEntryIds.ForbiddenTransmutation,
                CodexEntryType.Practice,
                CodexConceptKeys.ForbiddenTransmutation,
                NarrativeHookIds.TransmutationForbiddenPractice,
                "",
                NarrativeTags.TransmutationAccessAllowed);

            new CodexDiscoveryService().Discover(profile, candidate);
            new CodexDiscoveryService().Discover(profile, candidate);

            var summary = new CodexProgressService().Summarize(profile);

            Assert(
                summary.TotalDiscoveredEntries == 1,
                "Duplicate codex discoveries should not inflate total progress.");

            Assert(
                summary.GetDiscoveredCount(CodexEntryType.Practice) == 1,
                "Duplicate codex discoveries should not inflate type progress.");
        }

        private static CodexProfile BuildCodexProfileWithThreeEntries()
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
                CodexEntryIds.ForbiddenTransmutation,
                CodexEntryType.Practice,
                CodexConceptKeys.ForbiddenTransmutation,
                NarrativeHookIds.TransmutationForbiddenPractice,
                MissionTriggerIds.NahualTransmutationDiscovery,
                NarrativeTags.TransmutationAccessAllowed));
            candidates.Add(BuildCodexCandidate(
                CodexEntryIds.TransmutationWarning,
                CodexEntryType.Warning,
                CodexConceptKeys.TransmutationWarning,
                NarrativeHookIds.DialogueTransmutationBlocked,
                MissionTriggerIds.NahualTransmutationBlockedWarning,
                NarrativeTags.TransmutationAccessBlocked));

            new CodexDiscoveryService().Discover(profile, candidates);
            return profile;
        }

        private static void AssertMissionJournalDiscoversOneLead()
        {
            var profile = new MissionJournalProfile();
            var candidate = BuildMissionTriggerCandidate(
                MissionTriggerIds.SharedWisdomFollowup,
                MissionTriggerType.FollowUp,
                MissionFamilies.SharedWisdom,
                NarrativeHookIds.MissionSharedWisdomFollowup,
                NarrativeTags.NahualFirstTeachingRejected);

            bool discovered = new MissionDiscoveryService().Discover(profile, candidate);
            var record = profile.GetAvailableLead(MissionTriggerIds.SharedWisdomFollowup);

            Assert(
                discovered,
                "Mission discovery should report a new mission lead.");

            Assert(
                profile.IsLeadAvailable(MissionTriggerIds.SharedWisdomFollowup),
                "Mission journal should track the available mission lead id.");

            Assert(
                record != null && record.TriggerType == MissionTriggerType.FollowUp,
                "Mission journal should preserve the trigger type.");
        }

        private static void AssertMissionJournalDuplicateDiscoveryIsIdempotent()
        {
            var profile = new MissionJournalProfile();
            var candidate = BuildMissionTriggerCandidate(
                MissionTriggerIds.NahualTransmutationDiscovery,
                MissionTriggerType.Discovery,
                MissionFamilies.NahualPath,
                NarrativeHookIds.TransmutationForbiddenPractice,
                NarrativeTags.TransmutationAccessAllowed);

            bool firstDiscovery = new MissionDiscoveryService().Discover(profile, candidate);
            bool duplicateDiscovery = new MissionDiscoveryService().Discover(profile, candidate);

            Assert(
                firstDiscovery,
                "First mission lead discovery should be applied.");

            Assert(
                !duplicateDiscovery,
                "Duplicate mission lead discovery should be idempotent.");

            Assert(
                profile.AvailableLeads.Count == 1,
                "Duplicate mission lead discovery should not add another lead record.");
        }

        private static void AssertMissionJournalDiscoversMultipleLeads()
        {
            var profile = new MissionJournalProfile();
            var candidates = new List<MissionTriggerCandidate>();
            candidates.Add(BuildMissionTriggerCandidate(
                MissionTriggerIds.SharedWisdomFollowup,
                MissionTriggerType.FollowUp,
                MissionFamilies.SharedWisdom,
                NarrativeHookIds.MissionSharedWisdomFollowup,
                NarrativeTags.NahualFirstTeachingRejected));
            candidates.Add(BuildMissionTriggerCandidate(
                MissionTriggerIds.NahualTransmutationBlockedWarning,
                MissionTriggerType.Warning,
                MissionFamilies.NahualPath,
                NarrativeHookIds.DialogueTransmutationBlocked,
                NarrativeTags.TransmutationAccessBlocked));

            int discoveredCount = new MissionDiscoveryService().Discover(profile, candidates);

            Assert(
                discoveredCount == 2,
                "Mission discovery should apply multiple new trigger candidates.");

            Assert(
                profile.IsLeadAvailable(MissionTriggerIds.SharedWisdomFollowup)
                && profile.IsLeadAvailable(MissionTriggerIds.NahualTransmutationBlockedWarning),
                "Mission journal should track multiple available lead ids.");
        }

        private static void AssertMissionJournalPreservesNarrativeTagsAndFamily()
        {
            var profile = new MissionJournalProfile();
            var candidate = BuildMissionTriggerCandidate(
                MissionTriggerIds.NahualTransmutationDiscovery,
                MissionTriggerType.Discovery,
                MissionFamilies.NahualPath,
                NarrativeHookIds.TransmutationForbiddenPractice,
                NarrativeTags.TransmutationAccessAllowed);

            new MissionDiscoveryService().Discover(profile, candidate);
            var record = profile.GetAvailableLead(MissionTriggerIds.NahualTransmutationDiscovery);

            Assert(
                record != null && ContainsTag(record.NarrativeTags, NarrativeTags.TransmutationAccessAllowed),
                "Mission journal should preserve narrative tags on lead records.");

            Assert(
                record != null && record.SuggestedMissionFamily == MissionFamilies.NahualPath,
                "Mission journal should preserve the suggested mission family.");
        }

        private static MissionTriggerCandidate BuildMissionTriggerCandidate(
            string triggerId,
            MissionTriggerType triggerType,
            string suggestedMissionFamily,
            string sourceHookId,
            string narrativeTag)
        {
            var narrativeTags = new List<string>();
            narrativeTags.Add(narrativeTag);

            return new MissionTriggerCandidate(
                triggerId,
                triggerType,
                10,
                suggestedMissionFamily,
                sourceHookId,
                narrativeTags);
        }

        private static void AssertMissionLeadCanBecomeActiveMission()
        {
            var profile = BuildMissionJournalWithLead(
                MissionTriggerIds.SharedWisdomFollowup,
                MissionTriggerType.FollowUp,
                MissionFamilies.SharedWisdom,
                NarrativeHookIds.MissionSharedWisdomFollowup,
                NarrativeTags.NahualFirstTeachingRejected);

            bool promoted = profile.PromoteLeadToActiveMission(MissionTriggerIds.SharedWisdomFollowup);
            var mission = profile.GetActiveMission(MissionTriggerIds.SharedWisdomFollowup);

            Assert(
                promoted,
                "Discovered mission lead should promote to active mission.");

            Assert(
                mission != null && mission.State == MissionLifecycleState.Active,
                "Promoted mission should be active.");

            Assert(
                mission != null
                && mission.SuggestedMissionFamily == MissionFamilies.SharedWisdom
                && ContainsTag(mission.NarrativeTags, NarrativeTags.NahualFirstTeachingRejected),
                "Promoted mission should preserve lead facts.");
        }

        private static void AssertActiveMissionCanBeCompleted()
        {
            var profile = BuildMissionJournalWithLead(
                MissionTriggerIds.NahualTransmutationDiscovery,
                MissionTriggerType.Discovery,
                MissionFamilies.NahualPath,
                NarrativeHookIds.TransmutationForbiddenPractice,
                NarrativeTags.TransmutationAccessAllowed);

            profile.PromoteLeadToActiveMission(MissionTriggerIds.NahualTransmutationDiscovery);
            bool completed = profile.CompleteMission(MissionTriggerIds.NahualTransmutationDiscovery);
            var mission = profile.GetActiveMission(MissionTriggerIds.NahualTransmutationDiscovery);

            Assert(
                completed,
                "Active mission should complete successfully.");

            Assert(
                mission != null && mission.State == MissionLifecycleState.Completed,
                "Completed mission should record completed state.");
        }

        private static void AssertActiveMissionCanBeFailed()
        {
            var profile = BuildMissionJournalWithLead(
                MissionTriggerIds.NahualTransmutationBlockedWarning,
                MissionTriggerType.Warning,
                MissionFamilies.NahualPath,
                NarrativeHookIds.DialogueTransmutationBlocked,
                NarrativeTags.TransmutationAccessBlocked);

            profile.PromoteLeadToActiveMission(MissionTriggerIds.NahualTransmutationBlockedWarning);
            bool failed = profile.FailMission(MissionTriggerIds.NahualTransmutationBlockedWarning);
            var mission = profile.GetActiveMission(MissionTriggerIds.NahualTransmutationBlockedWarning);

            Assert(
                failed,
                "Active mission should fail successfully.");

            Assert(
                mission != null && mission.State == MissionLifecycleState.Failed,
                "Failed mission should record failed state.");
        }

        private static void AssertDuplicateMissionPromotionIsSafe()
        {
            var profile = BuildMissionJournalWithLead(
                MissionTriggerIds.SharedWisdomFollowup,
                MissionTriggerType.FollowUp,
                MissionFamilies.SharedWisdom,
                NarrativeHookIds.MissionSharedWisdomFollowup,
                NarrativeTags.NahualFirstTeachingRejected);

            bool firstPromotion = profile.PromoteLeadToActiveMission(MissionTriggerIds.SharedWisdomFollowup);
            bool duplicatePromotion = profile.PromoteLeadToActiveMission(MissionTriggerIds.SharedWisdomFollowup);

            Assert(
                firstPromotion && duplicatePromotion,
                "Duplicate promotion of an active mission should be safe.");

            Assert(
                profile.ActiveMissions.Count == 1,
                "Duplicate promotion should not create another active mission record.");
        }

        private static void AssertTerminalMissionCannotBeOverwritten()
        {
            var completedProfile = BuildMissionJournalWithLead(
                MissionTriggerIds.SharedWisdomFollowup,
                MissionTriggerType.FollowUp,
                MissionFamilies.SharedWisdom,
                NarrativeHookIds.MissionSharedWisdomFollowup,
                NarrativeTags.NahualFirstTeachingRejected);
            completedProfile.PromoteLeadToActiveMission(MissionTriggerIds.SharedWisdomFollowup);
            completedProfile.CompleteMission(MissionTriggerIds.SharedWisdomFollowup);

            bool failedCompletedMission = completedProfile.FailMission(MissionTriggerIds.SharedWisdomFollowup);
            bool promotedCompletedMission = completedProfile.PromoteLeadToActiveMission(MissionTriggerIds.SharedWisdomFollowup);

            var failedProfile = BuildMissionJournalWithLead(
                MissionTriggerIds.NahualTransmutationBlockedWarning,
                MissionTriggerType.Warning,
                MissionFamilies.NahualPath,
                NarrativeHookIds.DialogueTransmutationBlocked,
                NarrativeTags.TransmutationAccessBlocked);
            failedProfile.PromoteLeadToActiveMission(MissionTriggerIds.NahualTransmutationBlockedWarning);
            failedProfile.FailMission(MissionTriggerIds.NahualTransmutationBlockedWarning);

            bool completedFailedMission = failedProfile.CompleteMission(MissionTriggerIds.NahualTransmutationBlockedWarning);

            Assert(
                !failedCompletedMission && !promotedCompletedMission,
                "Completed mission should not be failed or re-promoted accidentally.");

            Assert(
                !completedFailedMission,
                "Failed mission should not be completed accidentally.");

            Assert(
                completedProfile.GetActiveMission(MissionTriggerIds.SharedWisdomFollowup).State == MissionLifecycleState.Completed
                && failedProfile.GetActiveMission(MissionTriggerIds.NahualTransmutationBlockedWarning).State == MissionLifecycleState.Failed,
                "Terminal mission states should remain stable.");
        }

        private static MissionJournalProfile BuildMissionJournalWithLead(
            string triggerId,
            MissionTriggerType triggerType,
            string suggestedMissionFamily,
            string sourceHookId,
            string narrativeTag)
        {
            var profile = new MissionJournalProfile();
            new MissionDiscoveryService().Discover(
                profile,
                BuildMissionTriggerCandidate(
                    triggerId,
                    triggerType,
                    suggestedMissionFamily,
                    sourceHookId,
                    narrativeTag));
            return profile;
        }

        private static void AssertDiscoveryOrchestrationAppliesCodexAndMissionTogether()
        {
            var codexProfile = new CodexProfile();
            var missionJournalProfile = new MissionJournalProfile();
            var codexCandidates = new List<CodexEntryCandidate>();
            codexCandidates.Add(BuildCodexCandidate(
                CodexEntryIds.NahualPath,
                CodexEntryType.Concept,
                CodexConceptKeys.NahualPath,
                NarrativeHookIds.CodexNahualPath,
                "",
                NarrativeTags.NahualFirstTeachingAccepted));

            var missionCandidates = new List<MissionTriggerCandidate>();
            missionCandidates.Add(BuildMissionTriggerCandidate(
                MissionTriggerIds.SharedWisdomFollowup,
                MissionTriggerType.FollowUp,
                MissionFamilies.SharedWisdom,
                NarrativeHookIds.MissionSharedWisdomFollowup,
                NarrativeTags.NahualFirstTeachingRejected));

            var result = new DiscoveryOrchestrationService().Apply(
                codexProfile,
                missionJournalProfile,
                codexCandidates,
                missionCandidates);

            Assert(
                result.CodexEntriesDiscoveredCount == 1
                && result.MissionLeadsDiscoveredCount == 1,
                "Discovery orchestration should apply codex and mission discoveries together.");

            Assert(
                codexProfile.IsDiscovered(CodexEntryIds.NahualPath)
                && missionJournalProfile.IsLeadAvailable(MissionTriggerIds.SharedWisdomFollowup),
                "Discovery orchestration should mutate only the provided discovery profiles.");

            Assert(
                ContainsString(result.CodexEntryIdsDiscovered, CodexEntryIds.NahualPath)
                && ContainsString(result.MissionLeadTriggerIdsDiscovered, MissionTriggerIds.SharedWisdomFollowup),
                "Discovery orchestration summary should list discovered ids.");
        }

        private static void AssertDiscoveryOrchestrationDuplicateReapplicationIsIdempotent()
        {
            var codexProfile = new CodexProfile();
            var missionJournalProfile = new MissionJournalProfile();
            var codexCandidates = new List<CodexEntryCandidate>();
            codexCandidates.Add(BuildCodexCandidate(
                CodexEntryIds.ForbiddenTransmutation,
                CodexEntryType.Practice,
                CodexConceptKeys.ForbiddenTransmutation,
                NarrativeHookIds.TransmutationForbiddenPractice,
                "",
                NarrativeTags.TransmutationAccessAllowed));

            var missionCandidates = new List<MissionTriggerCandidate>();
            missionCandidates.Add(BuildMissionTriggerCandidate(
                MissionTriggerIds.NahualTransmutationDiscovery,
                MissionTriggerType.Discovery,
                MissionFamilies.NahualPath,
                NarrativeHookIds.TransmutationForbiddenPractice,
                NarrativeTags.TransmutationAccessAllowed));

            var service = new DiscoveryOrchestrationService();
            service.Apply(
                codexProfile,
                missionJournalProfile,
                codexCandidates,
                missionCandidates);

            var result = service.Apply(
                codexProfile,
                missionJournalProfile,
                codexCandidates,
                missionCandidates);

            Assert(
                result.CodexEntriesDiscoveredCount == 0
                && result.MissionLeadsDiscoveredCount == 0,
                "Discovery orchestration duplicate re-application should not rediscover known facts.");

            Assert(
                result.CodexEntriesAlreadyKnownCount == 1
                && result.MissionLeadsAlreadyKnownCount == 1,
                "Discovery orchestration should count already-known duplicate facts.");

            Assert(
                codexProfile.DiscoveredEntries.Count == 1
                && missionJournalProfile.AvailableLeads.Count == 1,
                "Discovery orchestration duplicate re-application should preserve idempotent profile state.");
        }

        private static void AssertDiscoveryOrchestrationReportsDiscoveredAndAlreadyKnown()
        {
            var codexProfile = new CodexProfile();
            var missionJournalProfile = new MissionJournalProfile();
            var codexDiscoveryService = new CodexDiscoveryService();
            var missionDiscoveryService = new MissionDiscoveryService();

            var knownCodexCandidate = BuildCodexCandidate(
                CodexEntryIds.NahualPath,
                CodexEntryType.Concept,
                CodexConceptKeys.NahualPath,
                NarrativeHookIds.CodexNahualPath,
                "",
                NarrativeTags.NahualFirstTeachingAccepted);
            var knownMissionCandidate = BuildMissionTriggerCandidate(
                MissionTriggerIds.SharedWisdomFollowup,
                MissionTriggerType.FollowUp,
                MissionFamilies.SharedWisdom,
                NarrativeHookIds.MissionSharedWisdomFollowup,
                NarrativeTags.NahualFirstTeachingRejected);

            codexDiscoveryService.Discover(codexProfile, knownCodexCandidate);
            missionDiscoveryService.Discover(missionJournalProfile, knownMissionCandidate);

            var codexCandidates = new List<CodexEntryCandidate>();
            codexCandidates.Add(knownCodexCandidate);
            codexCandidates.Add(BuildCodexCandidate(
                CodexEntryIds.TransmutationWarning,
                CodexEntryType.Warning,
                CodexConceptKeys.TransmutationWarning,
                NarrativeHookIds.DialogueTransmutationBlocked,
                MissionTriggerIds.NahualTransmutationBlockedWarning,
                NarrativeTags.TransmutationAccessBlocked));

            var missionCandidates = new List<MissionTriggerCandidate>();
            missionCandidates.Add(knownMissionCandidate);
            missionCandidates.Add(BuildMissionTriggerCandidate(
                MissionTriggerIds.NahualTransmutationBlockedWarning,
                MissionTriggerType.Warning,
                MissionFamilies.NahualPath,
                NarrativeHookIds.DialogueTransmutationBlocked,
                NarrativeTags.TransmutationAccessBlocked));

            var result = new DiscoveryOrchestrationService().Apply(
                codexProfile,
                missionJournalProfile,
                codexCandidates,
                missionCandidates);

            Assert(
                result.CodexEntriesDiscoveredCount == 1
                && result.CodexEntriesAlreadyKnownCount == 1,
                "Discovery orchestration should report discovered and already-known codex entries.");

            Assert(
                result.MissionLeadsDiscoveredCount == 1
                && result.MissionLeadsAlreadyKnownCount == 1,
                "Discovery orchestration should report discovered and already-known mission leads.");

            Assert(
                ContainsString(result.CodexEntryIdsDiscovered, CodexEntryIds.TransmutationWarning)
                && ContainsString(result.MissionLeadTriggerIdsDiscovered, MissionTriggerIds.NahualTransmutationBlockedWarning),
                "Discovery orchestration should report newly discovered ids separately from known ids.");
        }

        private static void AssertV2SnapshotCapturesCodexEntries()
        {
            var codexProfile = new CodexProfile();
            var missionJournalProfile = new MissionJournalProfile();
            new CodexDiscoveryService().Discover(
                codexProfile,
                BuildCodexCandidate(
                    CodexEntryIds.ForbiddenTransmutation,
                    CodexEntryType.Practice,
                    CodexConceptKeys.ForbiddenTransmutation,
                    NarrativeHookIds.TransmutationForbiddenPractice,
                    MissionTriggerIds.NahualTransmutationDiscovery,
                    NarrativeTags.TransmutationAccessAllowed));

            var snapshot = new V2ProgressSnapshotService().CreateSnapshot(
                codexProfile,
                missionJournalProfile);

            Assert(
                snapshot.CodexEntries.Count == 1,
                "V2 snapshot should capture discovered codex entries.");

            Assert(
                snapshot.CodexEntries[0].EntryId == CodexEntryIds.ForbiddenTransmutation
                && snapshot.CodexEntries[0].ConceptKey == CodexConceptKeys.ForbiddenTransmutation,
                "V2 snapshot should preserve codex entry ids and concept keys.");
        }

        private static void AssertV2SnapshotCapturesMissionLeads()
        {
            var codexProfile = new CodexProfile();
            var missionJournalProfile = new MissionJournalProfile();
            new MissionDiscoveryService().Discover(
                missionJournalProfile,
                BuildMissionTriggerCandidate(
                    MissionTriggerIds.NahualTransmutationBlockedWarning,
                    MissionTriggerType.Warning,
                    MissionFamilies.NahualPath,
                    NarrativeHookIds.DialogueTransmutationBlocked,
                    NarrativeTags.TransmutationAccessBlocked));

            var snapshot = new V2ProgressSnapshotService().CreateSnapshot(
                codexProfile,
                missionJournalProfile);

            Assert(
                snapshot.MissionLeads.Count == 1,
                "V2 snapshot should capture available mission leads.");

            Assert(
                snapshot.MissionLeads[0].TriggerId == MissionTriggerIds.NahualTransmutationBlockedWarning
                && snapshot.MissionLeads[0].SuggestedMissionFamily == MissionFamilies.NahualPath,
                "V2 snapshot should preserve mission trigger ids and mission families.");
        }

        private static void AssertV2SnapshotCapturesActiveMission()
        {
            var missionJournalProfile = BuildMissionJournalWithLead(
                MissionTriggerIds.SharedWisdomFollowup,
                MissionTriggerType.FollowUp,
                MissionFamilies.SharedWisdom,
                NarrativeHookIds.MissionSharedWisdomFollowup,
                NarrativeTags.NahualFirstTeachingRejected);
            missionJournalProfile.PromoteLeadToActiveMission(MissionTriggerIds.SharedWisdomFollowup);

            var snapshot = new V2ProgressSnapshotService().CreateSnapshot(
                new CodexProfile(),
                missionJournalProfile);

            Assert(
                snapshot.ActiveMissions.Count == 1,
                "V2 snapshot should capture active missions.");

            Assert(
                snapshot.ActiveMissions[0].State == MissionLifecycleState.Active
                && snapshot.ActiveMissions[0].TriggerId == MissionTriggerIds.SharedWisdomFollowup,
                "V2 snapshot should preserve active mission state and trigger id.");
        }

        private static void AssertV2SnapshotCapturesCompletedMission()
        {
            var missionJournalProfile = BuildMissionJournalWithLead(
                MissionTriggerIds.NahualTransmutationDiscovery,
                MissionTriggerType.Discovery,
                MissionFamilies.NahualPath,
                NarrativeHookIds.TransmutationForbiddenPractice,
                NarrativeTags.TransmutationAccessAllowed);
            missionJournalProfile.PromoteLeadToActiveMission(MissionTriggerIds.NahualTransmutationDiscovery);
            missionJournalProfile.CompleteMission(MissionTriggerIds.NahualTransmutationDiscovery);

            var snapshot = new V2ProgressSnapshotService().CreateSnapshot(
                new CodexProfile(),
                missionJournalProfile);

            Assert(
                snapshot.ActiveMissions.Count == 1,
                "V2 snapshot should capture completed missions.");

            Assert(
                snapshot.ActiveMissions[0].State == MissionLifecycleState.Completed,
                "V2 snapshot should preserve completed mission state.");
        }

        private static void AssertV2SnapshotCapturesFailedMission()
        {
            var missionJournalProfile = BuildMissionJournalWithLead(
                MissionTriggerIds.NahualTransmutationBlockedWarning,
                MissionTriggerType.Warning,
                MissionFamilies.NahualPath,
                NarrativeHookIds.DialogueTransmutationBlocked,
                NarrativeTags.TransmutationAccessBlocked);
            missionJournalProfile.PromoteLeadToActiveMission(MissionTriggerIds.NahualTransmutationBlockedWarning);
            missionJournalProfile.FailMission(MissionTriggerIds.NahualTransmutationBlockedWarning);

            var snapshot = new V2ProgressSnapshotService().CreateSnapshot(
                new CodexProfile(),
                missionJournalProfile);

            Assert(
                snapshot.ActiveMissions.Count == 1,
                "V2 snapshot should capture failed missions.");

            Assert(
                snapshot.ActiveMissions[0].State == MissionLifecycleState.Failed,
                "V2 snapshot should preserve failed mission state.");
        }

        private static void AssertV2SnapshotRestoreRecreatesEquivalentProfiles()
        {
            var sourceCodexProfile = new CodexProfile();
            var sourceMissionJournalProfile = new MissionJournalProfile();
            new CodexDiscoveryService().Discover(
                sourceCodexProfile,
                BuildCodexCandidate(
                    CodexEntryIds.SharedWisdomFollowup,
                    CodexEntryType.FollowUp,
                    CodexConceptKeys.SharedWisdom,
                    NarrativeHookIds.MissionSharedWisdomFollowup,
                    MissionTriggerIds.SharedWisdomFollowup,
                    NarrativeTags.NahualFirstTeachingRejected));
            new MissionDiscoveryService().Discover(
                sourceMissionJournalProfile,
                BuildMissionTriggerCandidate(
                    MissionTriggerIds.SharedWisdomFollowup,
                    MissionTriggerType.FollowUp,
                    MissionFamilies.SharedWisdom,
                    NarrativeHookIds.MissionSharedWisdomFollowup,
                    NarrativeTags.NahualFirstTeachingRejected));

            var service = new V2ProgressSnapshotService();
            var snapshot = service.CreateSnapshot(
                sourceCodexProfile,
                sourceMissionJournalProfile);

            var restoredCodexProfile = new CodexProfile();
            var restoredMissionJournalProfile = new MissionJournalProfile();
            service.Restore(
                snapshot,
                restoredCodexProfile,
                restoredMissionJournalProfile);

            var restoredCodexRecord = restoredCodexProfile.GetDiscoveredEntry(CodexEntryIds.SharedWisdomFollowup);
            var restoredMissionLead = restoredMissionJournalProfile.GetAvailableLead(MissionTriggerIds.SharedWisdomFollowup);

            Assert(
                restoredCodexProfile.IsDiscovered(CodexEntryIds.SharedWisdomFollowup)
                && restoredMissionJournalProfile.IsLeadAvailable(MissionTriggerIds.SharedWisdomFollowup),
                "V2 snapshot restore should recreate discovered codex entries and mission leads.");

            Assert(
                restoredCodexRecord != null
                && restoredCodexRecord.FirstSourceTriggerId == MissionTriggerIds.SharedWisdomFollowup
                && ContainsTag(restoredCodexRecord.NarrativeTags, NarrativeTags.NahualFirstTeachingRejected),
                "V2 snapshot restore should preserve codex source ids and narrative tags.");

            Assert(
                restoredMissionLead != null
                && restoredMissionLead.TriggerType == MissionTriggerType.FollowUp
                && restoredMissionLead.Priority == 10,
                "V2 snapshot restore should preserve mission lead type and priority.");
        }

        private static void AssertV2SnapshotRestoreRecreatesLifecycleStates()
        {
            var sourceMissionJournalProfile = BuildMissionJournalWithLead(
                MissionTriggerIds.NahualTransmutationBlockedWarning,
                MissionTriggerType.Warning,
                MissionFamilies.NahualPath,
                NarrativeHookIds.DialogueTransmutationBlocked,
                NarrativeTags.TransmutationAccessBlocked);
            sourceMissionJournalProfile.PromoteLeadToActiveMission(MissionTriggerIds.NahualTransmutationBlockedWarning);
            sourceMissionJournalProfile.FailMission(MissionTriggerIds.NahualTransmutationBlockedWarning);

            var service = new V2ProgressSnapshotService();
            var snapshot = service.CreateSnapshot(
                new CodexProfile(),
                sourceMissionJournalProfile);

            var restoredMissionJournalProfile = new MissionJournalProfile();
            service.Restore(
                snapshot,
                new CodexProfile(),
                restoredMissionJournalProfile);

            var restoredMission = restoredMissionJournalProfile.GetActiveMission(MissionTriggerIds.NahualTransmutationBlockedWarning);

            Assert(
                restoredMission != null && restoredMission.State == MissionLifecycleState.Failed,
                "V2 snapshot restore should recreate lifecycle state.");

            Assert(
                restoredMission != null
                && restoredMission.TriggerType == MissionTriggerType.Warning
                && restoredMission.Priority == 10
                && restoredMission.SuggestedMissionFamily == MissionFamilies.NahualPath,
                "V2 snapshot restore should preserve active mission facts.");

            Assert(
                restoredMission != null && ContainsTag(restoredMission.NarrativeTags, NarrativeTags.TransmutationAccessBlocked),
                "V2 snapshot restore should preserve active mission narrative tags.");
        }

        private static void AssertV2SnapshotRestoreIsIdempotent()
        {
            var sourceCodexProfile = new CodexProfile();
            var sourceMissionJournalProfile = new MissionJournalProfile();
            new CodexDiscoveryService().Discover(
                sourceCodexProfile,
                BuildCodexCandidate(
                    CodexEntryIds.TransmutationWarning,
                    CodexEntryType.Warning,
                    CodexConceptKeys.TransmutationWarning,
                    NarrativeHookIds.DialogueTransmutationBlocked,
                    MissionTriggerIds.NahualTransmutationBlockedWarning,
                    NarrativeTags.TransmutationAccessBlocked));
            new MissionDiscoveryService().Discover(
                sourceMissionJournalProfile,
                BuildMissionTriggerCandidate(
                    MissionTriggerIds.NahualTransmutationBlockedWarning,
                    MissionTriggerType.Warning,
                    MissionFamilies.NahualPath,
                    NarrativeHookIds.DialogueTransmutationBlocked,
                    NarrativeTags.TransmutationAccessBlocked));

            var service = new V2ProgressSnapshotService();
            var snapshot = service.CreateSnapshot(
                sourceCodexProfile,
                sourceMissionJournalProfile);

            var restoredCodexProfile = new CodexProfile();
            var restoredMissionJournalProfile = new MissionJournalProfile();
            service.Restore(
                snapshot,
                restoredCodexProfile,
                restoredMissionJournalProfile);
            service.Restore(
                snapshot,
                restoredCodexProfile,
                restoredMissionJournalProfile);

            Assert(
                restoredCodexProfile.DiscoveredEntries.Count == 1,
                "V2 snapshot restore should not duplicate codex entries.");

            Assert(
                restoredMissionJournalProfile.AvailableLeads.Count == 1,
                "V2 snapshot restore should not duplicate mission leads.");

            Assert(
                ContainsTag(restoredMissionJournalProfile.AvailableLeads[0].NarrativeTags, NarrativeTags.TransmutationAccessBlocked),
                "V2 snapshot restore should preserve mission lead narrative tags after repeated restore.");
        }

        private static void AssertV2SnapshotLifecycleRestoreIsIdempotent()
        {
            var sourceMissionJournalProfile = BuildMissionJournalWithLead(
                MissionTriggerIds.SharedWisdomFollowup,
                MissionTriggerType.FollowUp,
                MissionFamilies.SharedWisdom,
                NarrativeHookIds.MissionSharedWisdomFollowup,
                NarrativeTags.NahualFirstTeachingRejected);
            sourceMissionJournalProfile.PromoteLeadToActiveMission(MissionTriggerIds.SharedWisdomFollowup);
            sourceMissionJournalProfile.CompleteMission(MissionTriggerIds.SharedWisdomFollowup);

            var service = new V2ProgressSnapshotService();
            var snapshot = service.CreateSnapshot(
                new CodexProfile(),
                sourceMissionJournalProfile);

            var restoredMissionJournalProfile = new MissionJournalProfile();
            service.Restore(
                snapshot,
                new CodexProfile(),
                restoredMissionJournalProfile);
            service.Restore(
                snapshot,
                new CodexProfile(),
                restoredMissionJournalProfile);

            Assert(
                restoredMissionJournalProfile.ActiveMissions.Count == 1,
                "V2 snapshot repeated restore should not duplicate active mission records.");

            Assert(
                restoredMissionJournalProfile.GetActiveMission(MissionTriggerIds.SharedWisdomFollowup).State == MissionLifecycleState.Completed,
                "V2 snapshot repeated restore should preserve terminal lifecycle state.");
        }

        private static void AssertV2SnapshotUsesCurrentVersion()
        {
            var snapshot = new V2ProgressSnapshotService().CreateSnapshot(
                new CodexProfile(),
                new MissionJournalProfile());

            Assert(
                snapshot.SchemaVersion == V2ProgressSnapshotVersions.CurrentSchemaVersion,
                "New V2 snapshots should use the current schema version.");

            Assert(
                snapshot.IsCurrentVersion(),
                "New V2 snapshots should report themselves as current.");
        }

        private static void AssertV2SnapshotCurrentVersionRestores()
        {
            var service = new V2ProgressSnapshotService();
            var snapshot = service.CreateSnapshot(
                new CodexProfile(),
                new MissionJournalProfile());

            Assert(
                service.CanRestore(snapshot),
                "Current V2 snapshot versions should be restorable.");

            service.Restore(
                snapshot,
                new CodexProfile(),
                new MissionJournalProfile());

            Assert(
                true,
                "Current V2 snapshot restore should complete without unsupported-version errors.");
        }

        private static void AssertV2SnapshotUnsupportedVersionIsRejected()
        {
            var unsupportedSnapshot = new V2ProgressSnapshot(
                V2ProgressSnapshotVersions.CurrentSchemaVersion + 1,
                new List<V2CodexEntrySnapshot>(),
                new List<V2MissionLeadSnapshot>());
            var service = new V2ProgressSnapshotService();
            var codexProfile = new CodexProfile();
            var missionJournalProfile = new MissionJournalProfile();
            bool rejected = false;

            try
            {
                service.Restore(
                    unsupportedSnapshot,
                    codexProfile,
                    missionJournalProfile);
            }
            catch (InvalidOperationException)
            {
                rejected = true;
            }

            Assert(
                !service.CanRestore(unsupportedSnapshot),
                "Unsupported V2 snapshot versions should not be restorable.");

            Assert(
                rejected,
                "Unsupported V2 snapshot versions should be rejected explicitly.");

            Assert(
                codexProfile.DiscoveredEntries.Count == 0
                && missionJournalProfile.AvailableLeads.Count == 0,
                "Unsupported V2 snapshot restore should not mutate target profiles.");
        }

        private static void AssertV2SnapshotSerializesToJson()
        {
            var snapshot = BuildSnapshotWithCodexAndMissionLead();
            string json = new V2ProgressSnapshotJsonSerializer().Serialize(snapshot);

            Assert(
                json.Length > 0,
                "V2 snapshot JSON serialization should produce text.");

            Assert(
                json.Contains("SchemaVersion")
                && json.Contains("CodexEntries")
                && json.Contains("MissionLeads"),
                "V2 snapshot JSON should include the snapshot root fields.");
        }

        private static void AssertV2SnapshotDeserializesEquivalentSnapshot()
        {
            var snapshot = BuildSnapshotWithCodexAndMissionLead();
            var serializer = new V2ProgressSnapshotJsonSerializer();
            string json = serializer.Serialize(snapshot);
            var deserialized = serializer.Deserialize(json);

            Assert(
                deserialized.SchemaVersion == V2ProgressSnapshotVersions.CurrentSchemaVersion,
                "V2 snapshot JSON should preserve schema version.");

            Assert(
                deserialized.CodexEntries.Count == 1
                && deserialized.CodexEntries[0].EntryId == CodexEntryIds.ForbiddenTransmutation
                && deserialized.CodexEntries[0].ConceptKey == CodexConceptKeys.ForbiddenTransmutation,
                "V2 snapshot JSON should preserve codex entry ids and concept keys.");

            Assert(
                deserialized.MissionLeads.Count == 1
                && deserialized.MissionLeads[0].TriggerId == MissionTriggerIds.NahualTransmutationDiscovery
                && deserialized.MissionLeads[0].Priority == 10
                && deserialized.MissionLeads[0].SuggestedMissionFamily == MissionFamilies.NahualPath,
                "V2 snapshot JSON should preserve mission lead ids, priorities, and families.");

            Assert(
                ContainsTag(deserialized.CodexEntries[0].NarrativeTags, NarrativeTags.TransmutationAccessAllowed)
                && ContainsTag(deserialized.MissionLeads[0].NarrativeTags, NarrativeTags.TransmutationAccessAllowed),
                "V2 snapshot JSON should preserve narrative tags.");
        }

        private static void AssertV2SnapshotUnsupportedVersionJsonIsRejected()
        {
            string json =
                "{\"SchemaVersion\":999,\"CodexEntries\":[],\"MissionLeads\":[]}";
            bool rejected = false;

            try
            {
                new V2ProgressSnapshotJsonSerializer().Deserialize(json);
            }
            catch (InvalidOperationException)
            {
                rejected = true;
            }

            Assert(
                rejected,
                "V2 snapshot JSON with unsupported schema version should be rejected.");
        }

        private static void AssertV2SnapshotMalformedJsonFailsSafely()
        {
            bool failedSafely = false;

            try
            {
                new V2ProgressSnapshotJsonSerializer().Deserialize("{not valid json");
            }
            catch (JsonException)
            {
                failedSafely = true;
            }

            Assert(
                failedSafely,
                "Malformed V2 snapshot JSON should fail safely.");
        }

        private static V2ProgressSnapshot BuildSnapshotWithCodexAndMissionLead()
        {
            var codexProfile = new CodexProfile();
            var missionJournalProfile = new MissionJournalProfile();
            new CodexDiscoveryService().Discover(
                codexProfile,
                BuildCodexCandidate(
                    CodexEntryIds.ForbiddenTransmutation,
                    CodexEntryType.Practice,
                    CodexConceptKeys.ForbiddenTransmutation,
                    NarrativeHookIds.TransmutationForbiddenPractice,
                    MissionTriggerIds.NahualTransmutationDiscovery,
                    NarrativeTags.TransmutationAccessAllowed));
            new MissionDiscoveryService().Discover(
                missionJournalProfile,
                BuildMissionTriggerCandidate(
                    MissionTriggerIds.NahualTransmutationDiscovery,
                    MissionTriggerType.Discovery,
                    MissionFamilies.NahualPath,
                    NarrativeHookIds.TransmutationForbiddenPractice,
                    NarrativeTags.TransmutationAccessAllowed));

            return new V2ProgressSnapshotService().CreateSnapshot(
                codexProfile,
                missionJournalProfile);
        }

        private static void AssertV2SnapshotFileRepositoryRoundtrip()
        {
            string filePath = BuildTempSnapshotPath();

            try
            {
                var snapshot = BuildSnapshotWithCodexAndMissionLead();
                var repository = new V2ProgressSnapshotFileRepository();

                repository.Save(filePath, snapshot);
                var loaded = repository.Load(filePath);

                Assert(
                    File.Exists(filePath),
                    "V2 snapshot file repository should create the requested file.");

                Assert(
                    loaded.SchemaVersion == V2ProgressSnapshotVersions.CurrentSchemaVersion,
                    "V2 snapshot file repository should preserve schema version.");

                Assert(
                    loaded.CodexEntries.Count == 1
                    && loaded.CodexEntries[0].EntryId == CodexEntryIds.ForbiddenTransmutation,
                    "V2 snapshot file repository should roundtrip codex entries.");

                Assert(
                    loaded.MissionLeads.Count == 1
                    && loaded.MissionLeads[0].TriggerId == MissionTriggerIds.NahualTransmutationDiscovery,
                    "V2 snapshot file repository should roundtrip mission leads.");
            }
            finally
            {
                DeleteTempSnapshotFile(filePath);
            }
        }

        private static void AssertV2SnapshotFileRepositoryMissingFileFailsSafely()
        {
            string filePath = BuildTempSnapshotPath();
            DeleteTempSnapshotFile(filePath);
            bool failedSafely = false;

            try
            {
                new V2ProgressSnapshotFileRepository().Load(filePath);
            }
            catch (FileNotFoundException)
            {
                failedSafely = true;
            }
            finally
            {
                DeleteTempSnapshotFile(filePath);
            }

            Assert(
                failedSafely,
                "Missing V2 snapshot files should fail safely.");
        }

        private static void AssertV2SnapshotFileRepositoryCreatesNestedDirectory()
        {
            string directoryPath = BuildTempSnapshotDirectoryPath();
            string filePath = Path.Combine(
                directoryPath,
                "nested",
                "snapshot.json");

            try
            {
                new V2ProgressSnapshotFileRepository().Save(
                    filePath,
                    BuildSnapshotWithCodexAndMissionLead());

                Assert(
                    Directory.Exists(Path.GetDirectoryName(filePath)),
                    "V2 snapshot file repository should create missing parent directories.");

                Assert(
                    File.Exists(filePath),
                    "V2 snapshot file repository should save inside a newly created nested directory.");
            }
            finally
            {
                DeleteTempSnapshotDirectory(directoryPath);
            }
        }

        private static void AssertV2SnapshotFileRepositoryMalformedFileFailsSafely()
        {
            string filePath = BuildTempSnapshotPath();
            bool failedSafely = false;

            try
            {
                File.WriteAllText(filePath, "{not valid json");
                new V2ProgressSnapshotFileRepository().Load(filePath);
            }
            catch (JsonException)
            {
                failedSafely = true;
            }
            finally
            {
                DeleteTempSnapshotFile(filePath);
            }

            Assert(
                failedSafely,
                "Malformed V2 snapshot files should fail safely.");
        }

        private static void AssertV2SnapshotFileRepositoryUnsupportedVersionFileFailsSafely()
        {
            string filePath = BuildTempSnapshotPath();
            bool failedSafely = false;

            try
            {
                File.WriteAllText(
                    filePath,
                    "{\"SchemaVersion\":999,\"CodexEntries\":[],\"MissionLeads\":[]}");
                new V2ProgressSnapshotFileRepository().Load(filePath);
            }
            catch (InvalidOperationException)
            {
                failedSafely = true;
            }
            finally
            {
                DeleteTempSnapshotFile(filePath);
            }

            Assert(
                failedSafely,
                "Unsupported V2 snapshot file versions should fail safely.");
        }

        private static string BuildTempSnapshotPath()
        {
            return Path.Combine(
                Path.GetTempPath(),
                "fragments-v2-snapshot-" + Guid.NewGuid().ToString("N") + ".json");
        }

        private static string BuildTempSnapshotDirectoryPath()
        {
            return Path.Combine(
                Path.GetTempPath(),
                "fragments-v2-snapshot-" + Guid.NewGuid().ToString("N"));
        }

        private static void DeleteTempSnapshotFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private static void DeleteTempSnapshotDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
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
