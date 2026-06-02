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
            if (skillRegistry == null || !skillRegistry.Contains(skillId))
            {
                return false;
            }

            var record = GetRecord(skillId);

            if (record == null)
            {
                record = new MasteryRecord(skillId);
                records.Add(record);
            }

            return record.AddProgress(amount);
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
    }
}
