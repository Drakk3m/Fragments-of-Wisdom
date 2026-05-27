using System;
using System.Collections.Generic;
using Game.Domain.Codex;
using Game.Domain.Missions;

namespace Game.Domain.Discovery
{
    public class DiscoveryOrchestrationService
    {
        private readonly CodexDiscoveryService codexDiscoveryService;
        private readonly MissionDiscoveryService missionDiscoveryService;

        public DiscoveryOrchestrationService()
            : this(new CodexDiscoveryService(), new MissionDiscoveryService())
        {
        }

        public DiscoveryOrchestrationService(
            CodexDiscoveryService codexDiscoveryService,
            MissionDiscoveryService missionDiscoveryService)
        {
            if (codexDiscoveryService == null)
            {
                throw new ArgumentNullException(nameof(codexDiscoveryService));
            }

            if (missionDiscoveryService == null)
            {
                throw new ArgumentNullException(nameof(missionDiscoveryService));
            }

            this.codexDiscoveryService = codexDiscoveryService;
            this.missionDiscoveryService = missionDiscoveryService;
        }

        public DiscoveryOrchestrationResult Apply(
            CodexProfile codexProfile,
            MissionJournalProfile missionJournalProfile,
            IReadOnlyList<CodexEntryCandidate> codexEntryCandidates,
            IReadOnlyList<MissionTriggerCandidate> missionTriggerCandidates)
        {
            if (codexProfile == null)
            {
                throw new ArgumentNullException(nameof(codexProfile));
            }

            if (missionJournalProfile == null)
            {
                throw new ArgumentNullException(nameof(missionJournalProfile));
            }

            if (codexEntryCandidates == null)
            {
                throw new ArgumentNullException(nameof(codexEntryCandidates));
            }

            if (missionTriggerCandidates == null)
            {
                throw new ArgumentNullException(nameof(missionTriggerCandidates));
            }

            var codexEntryIdsDiscovered = new List<string>();
            var missionLeadTriggerIdsDiscovered = new List<string>();
            int codexEntriesAlreadyKnownCount = 0;
            int missionLeadsAlreadyKnownCount = 0;

            foreach (var candidate in codexEntryCandidates)
            {
                if (codexDiscoveryService.Discover(codexProfile, candidate))
                {
                    codexEntryIdsDiscovered.Add(candidate.EntryId);
                }
                else
                {
                    codexEntriesAlreadyKnownCount++;
                }
            }

            foreach (var candidate in missionTriggerCandidates)
            {
                if (missionDiscoveryService.Discover(missionJournalProfile, candidate))
                {
                    missionLeadTriggerIdsDiscovered.Add(candidate.TriggerId);
                }
                else
                {
                    missionLeadsAlreadyKnownCount++;
                }
            }

            return new DiscoveryOrchestrationResult(
                codexEntryIdsDiscovered,
                missionLeadTriggerIdsDiscovered,
                codexEntriesAlreadyKnownCount,
                missionLeadsAlreadyKnownCount);
        }
    }
}
