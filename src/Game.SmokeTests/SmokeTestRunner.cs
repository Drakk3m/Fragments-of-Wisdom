using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Game.Domain.Alignment;
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.Passives;
using Game.Domain.Combat.Resources;
using Game.Domain.Combat.Stats;
using Game.Domain.Combat.StatusEffects;
using Game.Domain.Codex;
using Game.Domain.Discovery;
using Game.Domain.Missions;
using Game.Domain.Narrative;
using Game.Domain.Player;
using Game.Domain.Progress;
using Game.Domain.Skills;
using Game.Domain.Transmutation;
using Game.Domain.World;
using Game.Slice;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static int assertionsExecuted;

        public static int Main()
        {
            var profile = new PlayerProfile();

            Console.WriteLine("Fragments of Wisdom smoke validation");
            Console.WriteLine();

            var log = FoundationVerticalSlice.Run();

            int v1Assertions = RunGroup(
                "V1 Legacy Slice",
                delegate
                {
                    RunV1SmokeTests(log);
                });

            int sharedAssertions = RunGroup(
                "Shared Domain Foundations",
                delegate
                {
                    RunSharedDomainSmokeTests();
                });

            int v3Assertions = RunGroup(
                "V3 Combat / Skills / Resonance / Status / Stats",
                delegate
                {
                    RunV3SmokeTests();
                });

            int v2Assertions = RunGroup(
                "V2 Narrative / Progression / Persistence",
                delegate
                {
                    RunV2SmokeTests(profile);
                });

            Console.WriteLine();
            Console.WriteLine("Smoke assertion summary");
            Console.WriteLine("  V1 Legacy Slice assertions: " + v1Assertions);
            Console.WriteLine("  Shared Domain Foundations assertions: " + sharedAssertions);
            Console.WriteLine("  V2 Narrative / Progression / Persistence assertions: " + v2Assertions);
            Console.WriteLine("  V3 Combat / Skills / Resonance / Status / Stats assertions: " + v3Assertions);
            Console.WriteLine("Total smoke assertions executed: " + assertionsExecuted);
            return 0;
        }
        private static int RunGroup(
            string name,
            Action sections)
        {
            int before = assertionsExecuted;

            Console.WriteLine(name);
            Console.WriteLine(new string('-', name.Length));
            sections();

            int groupAssertions = assertionsExecuted - before;
            Console.WriteLine(name + " assertions: " + groupAssertions);
            Console.WriteLine();
            return groupAssertions;
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

