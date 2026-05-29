using System.IO;
using Game.Application.Persistence;
using UnityEngine;

namespace Game.Unity.Persistence
{
    public class UnityV2SavePathProvider : IV2SavePathProvider
    {
        private const string FileName = "v2-progress-snapshot.json";

        public string GetV2ProgressSnapshotPath()
        {
            return Path.Combine(
                Application.persistentDataPath,
                FileName);
        }
    }
}
