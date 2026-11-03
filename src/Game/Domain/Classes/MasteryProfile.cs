using System.Collections.Generic;
using Game.Domain.Skills;

namespace Game.Domain.Classes
{
    public class MasteryProfile
    {
        private readonly List<MasteryRecord> records = new List<MasteryRecord>();

        public IReadOnlyList<MasteryRecord> Records
        {
            get { return records; }
        }

        public bool IncreaseProgress(
            string skillId,
            int amount,
            SkillDefinitionRegistry skillRegistry)
        {
            // Compatibility only: the legacy bool API creates an empty record on a
            // registered first nonpositive grant. The detailed API never does this.
            if (amount <= 0 && skillRegistry != null && skillRegistry.Contains(skillId))
            {
                GetOrCreateRecord(skillId);
            }

            return IncreaseProgressWithResult(skillId, amount, skillRegistry).Applied;
        }

        public MasteryProgressResult IncreaseProgressWithResult(
            string skillId,
            int amount,
            SkillDefinitionRegistry skillRegistry)
        {
            if (skillRegistry == null || !skillRegistry.Contains(skillId) || amount <= 0)
            {
                return MasteryProgressResult.Rejected(amount);
            }

            return GetOrCreateRecord(skillId).AddProgressWithResult(amount);
        }

        private MasteryRecord GetOrCreateRecord(string skillId)
        {
            var record = GetRecord(skillId);

            if (record == null)
            {
                record = new MasteryRecord(skillId);
                records.Add(record);
            }

            return record;
        }

        public MasteryRecord GetRecord(string skillId)
        {
            if (string.IsNullOrWhiteSpace(skillId))
            {
                return null;
            }

            foreach (var record in records)
            {
                if (record.SkillId == skillId)
                {
                    return record;
                }
            }

            return null;
        }

        public bool IsMastered(string skillId)
        {
            var record = GetRecord(skillId);
            return record != null && record.IsMastered;
        }

        public bool RestoreRecord(
            string skillId,
            int progress,
            SkillDefinitionRegistry skillRegistry)
        {
            if (skillRegistry == null || !skillRegistry.Contains(skillId))
            {
                return false;
            }

            var record = GetRecord(skillId);

            if (record == null)
            {
                records.Add(new MasteryRecord(skillId, progress));
                return true;
            }

            record.SetProgress(progress);
            return true;
        }
    }
}
