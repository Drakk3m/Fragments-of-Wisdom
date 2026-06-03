using System;
using Game.Domain.Classes;

namespace Game.Domain.Progress
{
    public class V2ProgressSnapshotValidationService
    {
        public bool IsValid(V2ProgressSnapshot snapshot)
        {
            return IsValid(snapshot, null);
        }

        public bool IsValid(
            V2ProgressSnapshot snapshot,
            ClassDefinitionRegistry classRegistry)
        {
            if (snapshot == null || !snapshot.IsCurrentVersion())
            {
                return false;
            }

            if (snapshot.CodexEntries == null
                || snapshot.MissionLeads == null
                || snapshot.ActiveMissions == null
                || snapshot.Unlocks == null
                || snapshot.MasteryRecords == null)
            {
                return false;
            }

            if (!HasValidCodexEntries(snapshot)
                || !HasValidMissionLeads(snapshot)
                || !HasValidActiveMissions(snapshot)
                || !HasValidUnlocks(snapshot)
                || !HasValidMasteryRecords(snapshot))
            {
                return false;
            }

            return HasValidClassProfile(snapshot, classRegistry);
        }

        public void ThrowIfInvalid(V2ProgressSnapshot snapshot)
        {
            ThrowIfInvalid(snapshot, null);
        }

        public void ThrowIfInvalid(
            V2ProgressSnapshot snapshot,
            ClassDefinitionRegistry classRegistry)
        {
            if (!IsValid(snapshot, classRegistry))
            {
                throw new InvalidOperationException("V2 progress snapshot failed semantic validation.");
            }
        }

        private static bool HasValidCodexEntries(V2ProgressSnapshot snapshot)
        {
            foreach (var entry in snapshot.CodexEntries)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.EntryId))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool HasValidMissionLeads(V2ProgressSnapshot snapshot)
        {
            foreach (var lead in snapshot.MissionLeads)
            {
                if (lead == null || string.IsNullOrWhiteSpace(lead.TriggerId))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool HasValidActiveMissions(V2ProgressSnapshot snapshot)
        {
            foreach (var mission in snapshot.ActiveMissions)
            {
                if (mission == null || string.IsNullOrWhiteSpace(mission.TriggerId))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool HasValidUnlocks(V2ProgressSnapshot snapshot)
        {
            foreach (var unlock in snapshot.Unlocks)
            {
                if (unlock == null || string.IsNullOrWhiteSpace(unlock.UnlockId))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool HasValidMasteryRecords(V2ProgressSnapshot snapshot)
        {
            foreach (var masteryRecord in snapshot.MasteryRecords)
            {
                if (masteryRecord == null || string.IsNullOrWhiteSpace(masteryRecord.SkillId))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool HasValidClassProfile(
            V2ProgressSnapshot snapshot,
            ClassDefinitionRegistry classRegistry)
        {
            if (snapshot.ClassProfile == null
                || string.IsNullOrWhiteSpace(snapshot.ClassProfile.CurrentClassId))
            {
                return true;
            }

            return classRegistry == null
                || classRegistry.Contains(snapshot.ClassProfile.CurrentClassId);
        }
    }
}
