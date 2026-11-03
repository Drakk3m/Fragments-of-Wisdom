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
        private static void RunSharedDomainSmokeTests()
        {
            RunSection(
                "AUD-11 resource pool invariant assertions",
                RunResourcePoolSpendSmokeTests);

            RunSection(
                "Domain event foundation assertions",
                delegate
                {
                    AssertDomainEventsCanBeRecordedAndRead();
                    AssertDomainEventsDoNotMutateStatefulProfiles();
                    AssertDomainEventsCanCarryMetadataFacts();
                    AssertCodexDiscoveryEmitsDomainEvent();
                    AssertDuplicateCodexDiscoveryEmitsNoDuplicateDomainEvent();
                    AssertUnlockApplicationEmitsDomainEvent();
                    AssertLockedAndDuplicateUnlocksEmitNoDomainEvent();
                    AssertCodexAndUnlockStateRemainCanonicalWithEvents();
                });
        }
        private static void AssertDomainEventsCanBeRecordedAndRead()
        {
            var eventLog = new DomainEventLog();

            eventLog.Record(
                DomainEventType.CodexEntryDiscovered,
                CodexEntryIds.NahualPath,
                "Codex entry discovered.");
            eventLog.Record(
                DomainEventType.MissionLeadDiscovered,
                MissionTriggerIds.SharedWisdomFollowup,
                "Mission lead discovered.");

            Assert(
                eventLog.Events.Count == 2,
                "Domain event log should record lightweight facts.");

            Assert(
                eventLog.Events[0].Type == DomainEventType.CodexEntryDiscovered
                && eventLog.Events[1].SubjectId == MissionTriggerIds.SharedWisdomFollowup,
                "Domain events should preserve type and subject facts for readers.");
        }

        private static void AssertDomainEventsDoNotMutateStatefulProfiles()
        {
            var eventLog = new DomainEventLog();
            var codexProfile = new CodexProfile();
            var missionJournal = new MissionJournalProfile();
            var unlockProfile = new UnlockProfile();

            eventLog.Record(
                DomainEventType.CodexEntryDiscovered,
                CodexEntryIds.ForbiddenTransmutation,
                "Codex discovery event recorded.");
            eventLog.Record(
                DomainEventType.MissionLeadDiscovered,
                MissionTriggerIds.NahualTransmutationDiscovery,
                "Mission lead event recorded.");
            eventLog.Record(
                DomainEventType.UnlockRecorded,
                CodexUnlockIds.ForbiddenPracticeCount,
                "Unlock event recorded.");

            Assert(
                eventLog.Events.Count == 3,
                "Domain events should be recorded independently of stateful profiles.");

            Assert(
                codexProfile.DiscoveredEntries.Count == 0
                && missionJournal.AvailableLeads.Count == 0
                && unlockProfile.Unlocks.Count == 0,
                "Recording domain events should not mutate canonical stateful profiles.");
        }

        private static void AssertDomainEventsCanCarryMetadataFacts()
        {
            var eventLog = new DomainEventLog();

            eventLog.Record(
                DomainEventType.CombatantDamaged,
                "combatant.player",
                "Combatant damaged.",
                new Dictionary<string, string>
                {
                    { "amount", "12" },
                    { "source", "poison" }
                });
            eventLog.Record(
                DomainEventType.StatusApplied,
                "combatant.player",
                "Status applied.",
                new Dictionary<string, string>
                {
                    { "status", StatusEffectType.Poison.ToString() }
                });

            Assert(
                eventLog.Events[0].Metadata["amount"] == "12"
                && eventLog.Events[0].Metadata["source"] == "poison",
                "Domain events should preserve optional metadata facts.");

            Assert(
                eventLog.Events[1].Type == DomainEventType.StatusApplied
                && eventLog.Events[1].Metadata["status"] == "Poison",
                "Domain events should support status-applied facts without applying gameplay state.");
        }

        private static void AssertCodexDiscoveryEmitsDomainEvent()
        {
            var profile = new CodexProfile();
            var eventLog = new DomainEventLog();
            var candidate = BuildCodexCandidate(
                CodexEntryIds.NahualPath,
                CodexEntryType.Concept,
                CodexConceptKeys.NahualPath,
                NarrativeHookIds.CodexNahualPath,
                "",
                NarrativeTags.NahualFirstTeachingAccepted);

            bool discovered = new CodexDiscoveryService().Discover(profile, candidate, eventLog);

            Assert(
                discovered && eventLog.Events.Count == 1,
                "New codex discovery should emit one domain event.");

            Assert(
                eventLog.Events[0].Type == DomainEventType.CodexEntryDiscovered
                && eventLog.Events[0].SubjectId == CodexEntryIds.NahualPath
                && eventLog.Events[0].Metadata["conceptKey"] == CodexConceptKeys.NahualPath,
                "Codex discovery event should preserve codex entry facts.");
        }

        private static void AssertDuplicateCodexDiscoveryEmitsNoDuplicateDomainEvent()
        {
            var profile = new CodexProfile();
            var eventLog = new DomainEventLog();
            var candidate = BuildCodexCandidate(
                CodexEntryIds.ForbiddenTransmutation,
                CodexEntryType.Practice,
                CodexConceptKeys.ForbiddenTransmutation,
                NarrativeHookIds.TransmutationForbiddenPractice,
                MissionTriggerIds.NahualTransmutationDiscovery,
                NarrativeTags.TransmutationAccessAllowed);
            var service = new CodexDiscoveryService();

            bool firstDiscovery = service.Discover(profile, candidate, eventLog);
            bool duplicateDiscovery = service.Discover(profile, candidate, eventLog);

            Assert(
                firstDiscovery && !duplicateDiscovery,
                "Duplicate codex discovery should remain idempotent with event logging.");

            Assert(
                eventLog.Events.Count == 1 && profile.DiscoveredEntries.Count == 1,
                "Duplicate codex discovery should not emit a duplicate event or record.");
        }

        private static void AssertUnlockApplicationEmitsDomainEvent()
        {
            var profile = new UnlockProfile();
            var eventLog = new DomainEventLog();
            var candidate = BuildUnlockedCandidate(
                CodexUnlockIds.ForbiddenPracticeCount,
                CodexUnlockType.EntryTypeCount,
                CodexEntryType.Practice,
                "",
                1,
                0,
                NarrativeTags.TransmutationAccessAllowed);

            bool applied = new UnlockApplicationService().Apply(profile, candidate, eventLog);

            Assert(
                applied && eventLog.Events.Count == 1,
                "New unlock application should emit one domain event.");

            Assert(
                eventLog.Events[0].Type == DomainEventType.UnlockRecorded
                && eventLog.Events[0].SubjectId == CodexUnlockIds.ForbiddenPracticeCount
                && eventLog.Events[0].Metadata["requiredEntryType"] == "Practice",
                "Unlock recorded event should preserve unlock requirement facts.");
        }

        private static void AssertLockedAndDuplicateUnlocksEmitNoDomainEvent()
        {
            var profile = new UnlockProfile();
            var eventLog = new DomainEventLog();
            var lockedCandidate = new CodexUnlockCandidate(
                CodexUnlockIds.ForbiddenPracticeCount,
                CodexUnlockType.EntryTypeCount,
                CodexEntryType.Practice,
                "",
                1,
                0,
                false,
                new List<string>());
            var unlockedCandidate = BuildUnlockedCandidate(
                CodexUnlockIds.NahualPathDiscovered,
                CodexUnlockType.EntryDiscovery,
                null,
                CodexEntryIds.NahualPath,
                0,
                0,
                NarrativeTags.NahualFirstTeachingAccepted);
            var service = new UnlockApplicationService();

            bool lockedApplied = service.Apply(profile, lockedCandidate, eventLog);
            bool firstUnlocked = service.Apply(profile, unlockedCandidate, eventLog);
            bool duplicateUnlocked = service.Apply(profile, unlockedCandidate, eventLog);

            Assert(
                !lockedApplied && firstUnlocked && !duplicateUnlocked,
                "Locked and duplicate unlock candidates should not apply.");

            Assert(
                eventLog.Events.Count == 1 && profile.Unlocks.Count == 1,
                "Locked and duplicate unlock candidates should not emit extra events.");
        }

        private static void AssertCodexAndUnlockStateRemainCanonicalWithEvents()
        {
            var codexProfile = new CodexProfile();
            var unlockProfile = new UnlockProfile();
            var eventLog = new DomainEventLog();
            var codexCandidate = BuildCodexCandidate(
                CodexEntryIds.SharedWisdomFollowup,
                CodexEntryType.FollowUp,
                CodexConceptKeys.SharedWisdom,
                NarrativeHookIds.MissionSharedWisdomFollowup,
                MissionTriggerIds.SharedWisdomFollowup,
                NarrativeTags.NahualFirstTeachingRejected);
            var unlockCandidate = BuildUnlockedCandidate(
                CodexUnlockIds.NahualPathDiscovered,
                CodexUnlockType.EntryDiscovery,
                null,
                CodexEntryIds.NahualPath,
                0,
                0,
                NarrativeTags.NahualFirstTeachingAccepted);

            new CodexDiscoveryService().Discover(codexProfile, codexCandidate, eventLog);
            new UnlockApplicationService().Apply(unlockProfile, unlockCandidate, eventLog);

            Assert(
                codexProfile.IsDiscovered(CodexEntryIds.SharedWisdomFollowup)
                && unlockProfile.HasUnlock(CodexUnlockIds.NahualPathDiscovered),
                "Profiles should remain the canonical source of codex and unlock state.");

            Assert(
                eventLog.Events.Count == 2,
                "Domain events should mirror accepted operations without becoming state.");
        }
    }
}

