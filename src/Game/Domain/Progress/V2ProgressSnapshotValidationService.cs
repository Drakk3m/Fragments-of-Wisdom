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
            return IsValid(snapshot, null, null, null);
        }

        public bool IsValid(
            V2ProgressSnapshot snapshot,
            ClassDefinitionRegistry classRegistry)
        {
            return IsValid(snapshot, classRegistry, null, null);
        }

        public bool IsValid(
            V2ProgressSnapshot snapshot,
            SkillDefinitionRegistry skillRegistry)
        {
            return IsValid(snapshot, null, skillRegistry, null);
        }

        public bool IsValid(
            V2ProgressSnapshot snapshot,
            ClassSkillTreeBindingRegistry skillTreeRegistry)
        {
            return IsValid(snapshot, null, null, skillTreeRegistry);
        }

        public bool IsValid(
            V2ProgressSnapshot snapshot,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry)
        {
            return IsValid(snapshot, classRegistry, skillRegistry, null);
        }

        public bool IsValid(
            V2ProgressSnapshot snapshot,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry,
            ClassSkillTreeBindingRegistry skillTreeRegistry)
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
                || !HasValidSkillTreeProgress(snapshot.SkillTreeProgress, skillTreeRegistry)
                || !HasValidClassSkillTreeBindings(snapshot))
            {
                return false;
            }

            return HasValidClassProfile(snapshot, classRegistry);
        }

        public void ThrowIfInvalid(V2ProgressSnapshot snapshot)
        {
            ThrowIfInvalid(snapshot, null, null, null);
        }

        public void ThrowIfInvalid(
            V2ProgressSnapshot snapshot,
            ClassDefinitionRegistry classRegistry)
        {
            ThrowIfInvalid(snapshot, classRegistry, null, null);
        }

        public void ThrowIfInvalid(
            V2ProgressSnapshot snapshot,
            SkillDefinitionRegistry skillRegistry)
        {
            ThrowIfInvalid(snapshot, null, skillRegistry, null);
        }

        public void ThrowIfInvalid(
            V2ProgressSnapshot snapshot,
            ClassSkillTreeBindingRegistry skillTreeRegistry)
        {
            ThrowIfInvalid(snapshot, null, null, skillTreeRegistry);
        }

        public void ThrowIfInvalid(
            V2ProgressSnapshot snapshot,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry)
        {
            ThrowIfInvalid(snapshot, classRegistry, skillRegistry, null);
        }

        public void ThrowIfInvalid(
            V2ProgressSnapshot snapshot,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry,
            ClassSkillTreeBindingRegistry skillTreeRegistry)
        {
            if (!IsValid(snapshot, classRegistry, skillRegistry, skillTreeRegistry))
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

        internal bool IsValidSkillTreeProgressForRestore(
            V2SkillTreeProgressSnapshot progress,
            SkillTreeDefinition skillTree,
            ClassSkillTreeBindingRegistry scopedTreeRegistry)
        {
            if (!HasValidSkillTreeProgress(progress, scopedTreeRegistry))
            {
                return false;
            }

            // Scoped binding restores use registered definitions; legacy restores use the explicit tree.
            if (progress.Scopes.Count > 0 && scopedTreeRegistry != null)
            {
                return true;
            }

            if (skillTree == null)
            {
                throw new ArgumentNullException(nameof(skillTree));
            }

            if (progress.Scopes.Count > 0)
            {
                if (string.IsNullOrWhiteSpace(skillTree.Id)
                    || progress.Scopes.Count != 1
                    || progress.Scopes[0].SkillTreeId != skillTree.Id)
                {
                    return false;
                }

                var scope = progress.Scopes[0];
                return HasValidSkillTreeNodes(scope.DiscoveredNodeIds, scope.UnlockedNodeIds, skillTree);
            }

            return HasValidSkillTreeNodes(progress.DiscoveredNodeIds, progress.UnlockedNodeIds, skillTree);
        }

        private static bool HasValidSkillTreeProgress(
            V2SkillTreeProgressSnapshot progress,
            ClassSkillTreeBindingRegistry skillTreeRegistry)
        {
            if (progress == null
                || progress.DiscoveredNodeIds == null
                || progress.UnlockedNodeIds == null
                || progress.Scopes == null)
            {
                return false;
            }

            bool hasLegacyProgress = progress.DiscoveredNodeIds.Count > 0
                || progress.UnlockedNodeIds.Count > 0;

            if (hasLegacyProgress && progress.Scopes.Count > 0)
            {
                return false;
            }

            foreach (var nodeId in progress.DiscoveredNodeIds)
            {
                if (string.IsNullOrWhiteSpace(nodeId))
                {
                    return false;
                }
            }

            foreach (var nodeId in progress.UnlockedNodeIds)
            {
                if (string.IsNullOrWhiteSpace(nodeId))
                {
                    return false;
                }
            }

            var skillTreeIds = new HashSet<string>();

            foreach (var scope in progress.Scopes)
            {
                if (scope == null
                    || string.IsNullOrWhiteSpace(scope.SkillTreeId)
                    || scope.DiscoveredNodeIds == null
                    || scope.UnlockedNodeIds == null
                    || !skillTreeIds.Add(scope.SkillTreeId))
                {
                    return false;
                }

                foreach (var nodeId in scope.DiscoveredNodeIds)
                {
                    if (string.IsNullOrWhiteSpace(nodeId))
                    {
                        return false;
                    }
                }

                foreach (var nodeId in scope.UnlockedNodeIds)
                {
                    if (string.IsNullOrWhiteSpace(nodeId))
                    {
                        return false;
                    }
                }

                if (skillTreeRegistry != null
                    && !HasValidSkillTreeProgressScope(scope, skillTreeRegistry))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool HasValidSkillTreeProgressScope(
            V2SkillTreeProgressScopeSnapshot scope,
            ClassSkillTreeBindingRegistry skillTreeRegistry)
        {
            var skillTree = skillTreeRegistry.GetSkillTree(scope.SkillTreeId);

            return HasValidSkillTreeNodes(scope.DiscoveredNodeIds, scope.UnlockedNodeIds, skillTree);
        }

        private static bool HasValidSkillTreeNodes(
            IReadOnlyList<string> discoveredIds,
            IReadOnlyList<string> unlockedIds,
            SkillTreeDefinition skillTree)
        {
            if (skillTree == null)
            {
                return false;
            }

            var discoveredNodeIds = new HashSet<string>();

            foreach (var nodeId in discoveredIds)
            {
                if (!skillTree.Contains(nodeId))
                {
                    return false;
                }

                discoveredNodeIds.Add(nodeId);
            }

            var unlockedNodeIds = new HashSet<string>();

            foreach (var nodeId in unlockedIds)
            {
                if (!skillTree.Contains(nodeId)
                    || !discoveredNodeIds.Contains(nodeId))
                {
                    return false;
                }

                unlockedNodeIds.Add(nodeId);
            }

            foreach (var nodeId in unlockedNodeIds)
            {
                var node = skillTree.Get(nodeId);

                foreach (var requiredNodeId in node.RequiredNodeIds)
                {
                    if (!unlockedNodeIds.Contains(requiredNodeId))
                    {
                        return false;
                    }
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
