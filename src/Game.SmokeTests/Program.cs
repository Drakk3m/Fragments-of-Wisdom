using System;
using System.Collections.Generic;
using Game.Domain.Alignment;
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
                ContainsTag(result, "nahual.first_teaching.accepted"),
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
                ContainsTag(result, "nahual.first_teaching.rejected"),
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
                result.RequiredTeachingPath == "nahual.first_teaching.accepted",
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
                ContainsTag(result, "transmutation.access.allowed"),
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
                ContainsTag(result, "transmutation.access.blocked"),
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
                ContainsHook(result, "npc.hook.nahual_path"),
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
                ContainsHook(result, "npc.hook.forbidden_practice"),
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

        private static void AssertDefaultSharedWisdomFlow()
        {
            Console.WriteLine("  Flow scenario: default SharedWisdom player");

            var profile = new PlayerProfile();
            var teachingResult = new FirstNahualTeachingEvaluator().Evaluate(profile);
            var accessResult = new TransmutationAccessPolicy().Evaluate(teachingResult);
            var reactionResult = new NpcReactionService().Resolve(accessResult.NarrativeTags);

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

            Assert(
                teachingResult.AcceptedTeachingPath,
                "Accepted forbidden teaching flow should accept the Nahual teaching path.");

            Assert(
                accessResult.CanAccessTransmutation,
                "Accepted forbidden teaching flow should allow transmutation access.");

            Assert(
                ContainsTag(accessResult, "transmutation.access.allowed"),
                "Accepted forbidden teaching flow should carry the allowed transmutation tag.");

            Assert(
                reactionResult.ReactionTone == NpcReactionTone.Wary,
                "Accepted forbidden teaching flow should produce wary NPC reaction.");

            Assert(
                ContainsHook(reactionResult, "npc.hook.forbidden_practice"),
                "Accepted forbidden teaching flow should expose the forbidden practice NPC hook.");
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

            Assert(
                teachingResult.RejectedTeachingPath,
                "Rejected flow should mark the first Nahual teaching as rejected.");

            Assert(
                !accessResult.CanAccessTransmutation,
                "Rejected flow should block transmutation access.");

            Assert(
                ContainsTag(accessResult, "transmutation.access.blocked"),
                "Rejected flow should carry the blocked transmutation tag.");

            Assert(
                reactionResult.ReactionTone == NpcReactionTone.Reassured,
                "Rejected flow should produce reassured NPC reaction.");

            Assert(
                ContainsHook(reactionResult, "npc.hook.shared_wisdom"),
                "Rejected flow should expose the Shared Wisdom NPC hook.");
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
