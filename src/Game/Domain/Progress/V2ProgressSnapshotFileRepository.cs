using System;
using System.IO;

namespace Game.Domain.Progress
{
    public class V2ProgressSnapshotFileRepository
    {
        private readonly V2ProgressSnapshotJsonSerializer serializer;
        private readonly V2ProgressSnapshotValidationService validationService;

        public V2ProgressSnapshotFileRepository()
            : this(
                new V2ProgressSnapshotJsonSerializer(),
                new V2ProgressSnapshotValidationService())
        {
        }

        public V2ProgressSnapshotFileRepository(V2ProgressSnapshotJsonSerializer serializer)
            : this(
                serializer,
                new V2ProgressSnapshotValidationService())
        {
        }

        public V2ProgressSnapshotFileRepository(
            V2ProgressSnapshotJsonSerializer serializer,
            V2ProgressSnapshotValidationService validationService)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (validationService == null)
            {
                throw new ArgumentNullException(nameof(validationService));
            }

            this.serializer = serializer;
            this.validationService = validationService;
        }

        public void Save(
            string filePath,
            V2ProgressSnapshot snapshot)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("V2 progress snapshot file path is required.", nameof(filePath));
            }

            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            validationService.ThrowIfInvalid(snapshot);

            string directoryPath = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrEmpty(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string tempFilePath = BuildTempFilePath(filePath);

            try
            {
                File.WriteAllText(tempFilePath, serializer.Serialize(snapshot));

                if (File.Exists(filePath))
                {
                    File.Replace(tempFilePath, filePath, BuildBackupFilePath(filePath), true);
                }
                else
                {
                    File.Move(tempFilePath, filePath);
                }
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        public V2ProgressSnapshot Load(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("V2 progress snapshot file path is required.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                string backupFilePath = BuildBackupFilePath(filePath);

                if (File.Exists(backupFilePath))
                {
                    return LoadStrict(backupFilePath);
                }

                throw new FileNotFoundException("V2 progress snapshot file was not found.", filePath);
            }

            try
            {
                return LoadStrict(filePath);
            }
            catch (Exception) when (File.Exists(BuildBackupFilePath(filePath)))
            {
                return LoadStrict(BuildBackupFilePath(filePath));
            }
        }

        private V2ProgressSnapshot LoadStrict(string filePath)
        {
            var snapshot = serializer.Deserialize(File.ReadAllText(filePath));
            validationService.ThrowIfInvalid(snapshot);
            return snapshot;
        }

        public static string BuildBackupFilePath(string filePath)
        {
            return filePath + ".bak";
        }

        private static string BuildTempFilePath(string filePath)
        {
            return filePath + ".tmp";
        }
    }
}
