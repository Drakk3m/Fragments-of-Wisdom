using System.Collections.Generic;

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
            IReadOnlyList<string> knownSkillIds)
        {
            if (!IsKnownSkill(skillId, knownSkillIds))
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

        private static bool IsKnownSkill(
            string skillId,
            IReadOnlyList<string> knownSkillIds)
        {
            if (string.IsNullOrWhiteSpace(skillId) || knownSkillIds == null)
            {
                return false;
            }

            foreach (var knownSkillId in knownSkillIds)
            {
                if (knownSkillId == skillId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
