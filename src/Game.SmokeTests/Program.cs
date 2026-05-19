using System;
using System.Collections.Generic;
using Game.Domain.Alignment;
using Game.Domain.Narrative;
using Game.Domain.Player;
using Game.Domain.Transmutation;
using Game.Slice;

namespace Game.SmokeTests
{
    public static class Program
    {
        public static int Main()
        {
            var profile = new PlayerProfile();

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

            AssertDefaultNahualTeachingState(profile);
            AssertAcceptedNahualTeachingState();
            AssertRejectedNahualTeachingState();
            AssertDefaultTransmutationAccess(profile);
            AssertAcceptedTransmutationAccess();
            AssertRejectedTransmutationAccess();

            var log = FoundationVerticalSlice.Run();

            Assert(
                log.Events.Count == 38,
                "Foundation V1 event count changed.");

            if (log.Events.Count == 0)
            {
                Console.Error.WriteLine("Foundation vertical slice produced no events.");
                return 1;
            }

            Console.WriteLine("Alignment foundation smoke test passed.");
            Console.WriteLine("Events: " + log.Events.Count);
            return 0;
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

        private static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
