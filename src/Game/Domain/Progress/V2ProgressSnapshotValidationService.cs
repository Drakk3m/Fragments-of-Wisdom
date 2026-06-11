using System;
using System.Collections.Generic;
using Game.Domain.Classes;
using Game.Domain.Skills;

namespace Game.Domain.Progress
{
    public class V2ProgressSnapshotValidationService
    {
        public bool IsValid(V2ProgressSnapshot snapshot)
        {
            return IsValid(snapshot, null, null);
        }

        public bool IsValid(
            V2ProgressSnapshot snapshot,
            ClassDefinitionRegistry classRegistry)
        {
            return IsValid(snapshot, classRegistry, null);
        }

        public bool IsValid(
            V2ProgressSnapshot snapshot,
            SkillDefinitionRegistry skillRegistry)
        {
            return IsValid(snapshot, null, skillRegistry);
        }

        public bool IsValid(
            V2ProgressSnapshot snapshot,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry)
        {
            if (snapshot == null || !snapshot.IsCurrentVersion())
            {
                return false;
            }

            if (snapshot.CodexEntries == null
                || snapshot.MissionLeads == null
                || snapshot.ActiveMissions == null
                || snapshot.Unlocks == null
                || snapshot.MasteryRecords == null
                || snapshot.MentorRelationships == null
                || snapshot.SkillTreeProgress == null
                || snapshot.ClassSkillTreeBindings == null)
            {
                return false;
            }

            if (!HasValidCodexEntries(snapshot)
                || !HasValidMissionLeads(snapshot)
                || !HasValidActiveMissions(snapshot)
                || !HasValidUnlocks(snapshot)
                || !HasValidMasteryRecords(snapshot, skillRegistry)
                || !HasValidMentorRelationships(snapshot)
                || !HasValidSkillTreeProgress(snapshot)
                || !HasValidClassSkillTreeBindings(snapshot))
            {
                return false;
            }

            return HasValidClassProfile(snapshot, classRegistry);
        }

        public void ThrowIfInvalid(V2ProgressSnapshot snapshot)
        {
            ThrowIfInvalid(snapshot, null, null);
        }

        public void ThrowIfInvalid(
            V2ProgressSnapshot snapshot,
            ClassDefinitionRegistry classRegistry)
        {
            ThrowIfInvalid(snapshot, classRegistry, null);
        }

        public void ThrowIfInvalid(
            V2ProgressSnapshot snapshot,
            SkillDefinitionRegistry skillRegistry)
        {
            ThrowIfInvalid(snapshot, null, skillRegistry);
        }

        public void ThrowIfInvalid(
            V2ProgressSnapshot snapshot,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry)
        {
            if (!IsValid(snapshot, classRegistry, skillRegistry))
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

        private static bool HasValidMasteryRecords(
            V2ProgressSnapshot snapshot,
            SkillDefinitionRegistry skillRegistry)
        {
            foreach (var masteryRecord in snapshot.MasteryRecords)
            {
                if (masteryRecord == null || string.IsNullOrWhiteSpace(masteryRecord.SkillId))
                {
                    return false;
                }

                if (skillRegistry != null && !skillRegistry.Contains(masteryRecord.SkillId))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool HasValidMentorRelationships(V2ProgressSnapshot snapshot)
        {
            foreach (var mentorRelationship in snapshot.MentorRelationships)
            {
                if (mentorRelationship == null || string.IsNullOrWhiteSpace(mentorRelationship.MentorId))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool HasValidSkillTreeProgress(V2ProgressSnapshot snapshot)
        {
            if (snapshot.SkillTreeProgress.DiscoveredNodeIds == null
                || snapshot.SkillTreeProgress.UnlockedNodeIds == null)
            {
                return false;
            }

            foreach (var nodeId in snapshot.SkillTreeProgress.DiscoveredNodeIds)
            {
                if (string.IsNullOrWhiteSpace(nodeId))
                {
                    return false;
                }
            }

            foreach (var nodeId in snapshot.SkillTreeProgress.UnlockedNodeIds)
            {
                if (string.IsNullOrWhiteSpace(nodeId))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool HasValidClassSkillTreeBindings(V2ProgressSnapshot snapshot)
        {
            var classIds = new HashSet<string>();

            foreach (var binding in snapshot.ClassSkillTreeBindings)
            {
                if (binding == null
                    || string.IsNullOrWhiteSpace(binding.ClassId)
                    || string.IsNullOrWhiteSpace(binding.SkillTreeId)
                    || !classIds.Add(binding.ClassId))
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
