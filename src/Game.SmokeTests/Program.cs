using System;
using Game.Domain.Alignment;
using Game.Domain.Player;
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

            var log = FoundationVerticalSlice.Run();

            Assert(
                log.Events.Count == 38,
                "Foundation V1 event count changed.");

            Console.WriteLine("Alignment foundation smoke test passed.");
            Console.WriteLine("Events: " + log.Events.Count);
            return 0;
        }

        private static void Assert(
            bool condition,
            string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
