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

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void RunV1SmokeTests(GameEventLog log)
        {
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
        }
    }
}

