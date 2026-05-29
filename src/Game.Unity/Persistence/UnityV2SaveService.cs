using System;
using System.IO;
using Game.Application.Persistence;
using Game.Domain.Codex;
using Game.Domain.Missions;
using Game.Domain.Progress;

namespace Game.Unity.Persistence
{
    public class UnityV2SaveService
    {
        private readonly IV2SavePathProvider savePathProvider;
        private readonly V2ProgressSnapshotFileRepository repository;
        private readonly V2ProgressSnapshotService snapshotService;

        public UnityV2SaveService(IV2SavePathProvider savePathProvider)
            : this(
                savePathProvider,
                new V2ProgressSnapshotFileRepository(),
                new V2ProgressSnapshotService())
        {
        }

        public UnityV2SaveService(
            IV2SavePathProvider savePathProvider,
            V2ProgressSnapshotFileRepository repository,
            V2ProgressSnapshotService snapshotService)
        {
            if (savePathProvider == null)
            {
                throw new ArgumentNullException(nameof(savePathProvider));
            }

            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (snapshotService == null)
            {
                throw new ArgumentNullException(nameof(snapshotService));
            }

            this.savePathProvider = savePathProvider;
            this.repository = repository;
            this.snapshotService = snapshotService;
        }

        public void Save(
            CodexProfile codexProfile,
            MissionJournalProfile missionJournalProfile)
        {
            repository.Save(
                savePathProvider.GetV2ProgressSnapshotPath(),
                snapshotService.CreateSnapshot(
                    codexProfile,
                    missionJournalProfile));
        }

        public void LoadInto(
            CodexProfile codexProfile,
            MissionJournalProfile missionJournalProfile)
        {
            snapshotService.Restore(
                repository.Load(savePathProvider.GetV2ProgressSnapshotPath()),
                codexProfile,
                missionJournalProfile);
        }

        public bool Exists()
        {
            return File.Exists(savePathProvider.GetV2ProgressSnapshotPath());
        }
    }
}
