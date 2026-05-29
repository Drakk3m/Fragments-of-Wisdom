using System;
using System.IO;

namespace Game.Domain.Progress
{
    public class V2ProgressSnapshotFileRepository
    {
        private readonly V2ProgressSnapshotJsonSerializer serializer;

        public V2ProgressSnapshotFileRepository()
            : this(new V2ProgressSnapshotJsonSerializer())
        {
        }

        public V2ProgressSnapshotFileRepository(V2ProgressSnapshotJsonSerializer serializer)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            this.serializer = serializer;
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

            string directoryPath = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrEmpty(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllText(filePath, serializer.Serialize(snapshot));
        }

        public V2ProgressSnapshot Load(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("V2 progress snapshot file path is required.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("V2 progress snapshot file was not found.", filePath);
            }

            return serializer.Deserialize(File.ReadAllText(filePath));
        }
    }
}
