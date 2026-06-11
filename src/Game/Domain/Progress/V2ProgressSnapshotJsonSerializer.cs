using System;
using System.Collections.Generic;
using System.Text.Json;
using Game.Domain.Classes;
using Game.Domain.Codex;
using Game.Domain.Missions;

namespace Game.Domain.Progress
{
    public class V2ProgressSnapshotJsonSerializer
    {
        private readonly V2ProgressSnapshotService snapshotService;
        private readonly JsonSerializerOptions options;

        public V2ProgressSnapshotJsonSerializer()
            : this(new V2ProgressSnapshotService())
        {
        }

        public V2ProgressSnapshotJsonSerializer(V2ProgressSnapshotService snapshotService)
        {
            if (snapshotService == null)
            {
                throw new ArgumentNullException(nameof(snapshotService));
            }

            this.snapshotService = snapshotService;
            options = new JsonSerializerOptions();
        }

        public string Serialize(V2ProgressSnapshot snapshot)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            return JsonSerializer.Serialize(ToDto(snapshot), options);
        }

        public V2ProgressSnapshot Deserialize(string json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var dto = JsonSerializer.Deserialize<SnapshotDto>(json, options);

            if (dto == null)
            {
                throw new InvalidOperationException("V2 progress snapshot JSON did not contain a snapshot.");
            }

            var snapshot = FromDto(dto);

            if (!snapshotService.CanRestore(snapshot))
            {
                throw new InvalidOperationException("Unsupported V2 progress snapshot schema version.");
            }

            return snapshot;
        }

        private static SnapshotDto ToDto(V2ProgressSnapshot snapshot)
        {
            var dto = new SnapshotDto();
            dto.SchemaVersion = snapshot.SchemaVersion;
            dto.CodexEntries = new List<CodexEntryDto>();
            dto.MissionLeads = new List<MissionLeadDto>();
            dto.ActiveMissions = new List<ActiveMissionDto>();
            dto.Unlocks = new List<UnlockDto>();
            dto.MasteryRecords = new List<MasteryDto>();
            dto.MentorRelationships = new List<MentorRelationshipDto>();
            dto.ClassSkillTreeBindings = new List<ClassSkillTreeBindingDto>();
            dto.SkillTreeProgress = new SkillTreeProgressDto
            {
                DiscoveredNodeIds = CopyTags(snapshot.SkillTreeProgress.DiscoveredNodeIds),
                UnlockedNodeIds = CopyTags(snapshot.SkillTreeProgress.UnlockedNodeIds)
            };
            dto.ClassProfile = snapshot.ClassProfile == null
                ? null
                : new ClassProfileDto
                {
                    CurrentClassId = snapshot.ClassProfile.CurrentClassId
                };

            foreach (var entry in snapshot.CodexEntries)
            {
                dto.CodexEntries.Add(new CodexEntryDto
                {
                    EntryId = entry.EntryId,
                    EntryType = entry.EntryType,
                    ConceptKey = entry.ConceptKey,
                    FirstSourceHookId = entry.FirstSourceHookId,
                    FirstSourceTriggerId = entry.FirstSourceTriggerId,
                    NarrativeTags = CopyTags(entry.NarrativeTags)
                });
            }

            foreach (var lead in snapshot.MissionLeads)
            {
                dto.MissionLeads.Add(new MissionLeadDto
                {
                    TriggerId = lead.TriggerId,
                    TriggerType = lead.TriggerType,
                    Priority = lead.Priority,
                    SuggestedMissionFamily = lead.SuggestedMissionFamily,
                    SourceHookId = lead.SourceHookId,
                    NarrativeTags = CopyTags(lead.NarrativeTags)
                });
            }

            foreach (var mission in snapshot.ActiveMissions)
            {
                dto.ActiveMissions.Add(new ActiveMissionDto
                {
                    TriggerId = mission.TriggerId,
                    State = mission.State,
                    TriggerType = mission.TriggerType,
                    Priority = mission.Priority,
                    SuggestedMissionFamily = mission.SuggestedMissionFamily,
                    SourceHookId = mission.SourceHookId,
                    NarrativeTags = CopyTags(mission.NarrativeTags)
                });
            }

            foreach (var unlock in snapshot.Unlocks)
            {
                dto.Unlocks.Add(new UnlockDto
                {
                    UnlockId = unlock.UnlockId,
                    UnlockType = unlock.UnlockType,
                    RequiredEntryType = unlock.RequiredEntryType,
                    RequiredEntryId = unlock.RequiredEntryId,
                    RequiredCount = unlock.RequiredCount,
                    RequiredPercentage = unlock.RequiredPercentage,
                    NarrativeTags = CopyTags(unlock.NarrativeTags)
                });
            }

            foreach (var masteryRecord in snapshot.MasteryRecords)
            {
                dto.MasteryRecords.Add(new MasteryDto
                {
                    SkillId = masteryRecord.SkillId,
                    Progress = masteryRecord.Progress,
                    Rank = masteryRecord.Rank
                });
            }

            foreach (var mentorRelationship in snapshot.MentorRelationships)
            {
                dto.MentorRelationships.Add(new MentorRelationshipDto
                {
                    MentorId = mentorRelationship.MentorId,
                    IsDiscovered = mentorRelationship.IsDiscovered,
                    Trust = mentorRelationship.Trust,
                    IsAvailable = mentorRelationship.IsAvailable
                });
            }

            foreach (var binding in snapshot.ClassSkillTreeBindings)
            {
                dto.ClassSkillTreeBindings.Add(new ClassSkillTreeBindingDto
                {
                    ClassId = binding.ClassId,
                    SkillTreeId = binding.SkillTreeId
                });
            }

            return dto;
        }

        private static V2ProgressSnapshot FromDto(SnapshotDto dto)
        {
            var codexEntries = new List<V2CodexEntrySnapshot>();
            var missionLeads = new List<V2MissionLeadSnapshot>();
            var activeMissions = new List<V2ActiveMissionSnapshot>();
            var unlocks = new List<V2UnlockSnapshot>();
            var masteryRecords = new List<V2MasterySnapshot>();
            var mentorRelationships = new List<V2MentorRelationshipSnapshot>();
            var classSkillTreeBindings = new List<V2ClassSkillTreeBindingSnapshot>();
            var skillTreeProgress = dto.SkillTreeProgress == null
                ? new V2SkillTreeProgressSnapshot(new List<string>(), new List<string>())
                : new V2SkillTreeProgressSnapshot(
                    CopyTags(dto.SkillTreeProgress.DiscoveredNodeIds),
                    CopyTags(dto.SkillTreeProgress.UnlockedNodeIds));

            if (dto.CodexEntries != null)
            {
                foreach (var entry in dto.CodexEntries)
                {
                    codexEntries.Add(new V2CodexEntrySnapshot(
                        entry.EntryId,
                        entry.EntryType,
                        entry.ConceptKey,
                        entry.FirstSourceHookId,
                        entry.FirstSourceTriggerId,
                        CopyTags(entry.NarrativeTags)));
                }
            }

            if (dto.MissionLeads != null)
            {
                foreach (var lead in dto.MissionLeads)
                {
                    missionLeads.Add(new V2MissionLeadSnapshot(
                        lead.TriggerId,
                        lead.TriggerType,
                        lead.Priority,
                        lead.SuggestedMissionFamily,
                        lead.SourceHookId,
                        CopyTags(lead.NarrativeTags)));
                }
            }

            if (dto.ActiveMissions != null)
            {
                foreach (var mission in dto.ActiveMissions)
                {
                    activeMissions.Add(new V2ActiveMissionSnapshot(
                        mission.TriggerId,
                        mission.State,
                        mission.TriggerType,
                        mission.Priority,
                        mission.SuggestedMissionFamily,
                        mission.SourceHookId,
                        CopyTags(mission.NarrativeTags)));
                }
            }

            if (dto.Unlocks != null)
            {
                foreach (var unlock in dto.Unlocks)
                {
                    unlocks.Add(new V2UnlockSnapshot(
                        unlock.UnlockId,
                        unlock.UnlockType,
                        unlock.RequiredEntryType,
                        unlock.RequiredEntryId,
                        unlock.RequiredCount,
                        unlock.RequiredPercentage,
                        CopyTags(unlock.NarrativeTags)));
                }
            }

            if (dto.MasteryRecords != null)
            {
                foreach (var masteryRecord in dto.MasteryRecords)
                {
                    masteryRecords.Add(new V2MasterySnapshot(
                        masteryRecord.SkillId,
                        masteryRecord.Progress,
                        masteryRecord.Rank));
                }
            }

            if (dto.MentorRelationships != null)
            {
                foreach (var mentorRelationship in dto.MentorRelationships)
                {
                    mentorRelationships.Add(new V2MentorRelationshipSnapshot(
                        mentorRelationship.MentorId,
                        mentorRelationship.IsDiscovered,
                        mentorRelationship.Trust,
                        mentorRelationship.IsAvailable));
                }
            }

            if (dto.ClassSkillTreeBindings != null)
            {
                foreach (var binding in dto.ClassSkillTreeBindings)
                {
                    classSkillTreeBindings.Add(new V2ClassSkillTreeBindingSnapshot(
                        binding.ClassId,
                        binding.SkillTreeId));
                }
            }

            return new V2ProgressSnapshot(
                dto.SchemaVersion,
                codexEntries,
                missionLeads,
                activeMissions,
                unlocks,
                dto.ClassProfile == null
                    ? null
                    : new V2ClassProfileSnapshot(dto.ClassProfile.CurrentClassId),
                masteryRecords,
                mentorRelationships,
                skillTreeProgress,
                classSkillTreeBindings);
        }

        private static List<string> CopyTags(IReadOnlyList<string> sourceTags)
        {
            var tags = new List<string>();

            if (sourceTags == null)
            {
                return tags;
            }

            foreach (var sourceTag in sourceTags)
            {
                tags.Add(sourceTag);
            }

            return tags;
        }

        private class SnapshotDto
        {
            public int SchemaVersion { get; set; }
            public List<CodexEntryDto> CodexEntries { get; set; }
            public List<MissionLeadDto> MissionLeads { get; set; }
            public List<ActiveMissionDto> ActiveMissions { get; set; }
            public List<UnlockDto> Unlocks { get; set; }
            public ClassProfileDto ClassProfile { get; set; }
            public List<MasteryDto> MasteryRecords { get; set; }
            public List<MentorRelationshipDto> MentorRelationships { get; set; }
            public SkillTreeProgressDto SkillTreeProgress { get; set; }
            public List<ClassSkillTreeBindingDto> ClassSkillTreeBindings { get; set; }
        }

        private class CodexEntryDto
        {
            public string EntryId { get; set; }
            public CodexEntryType EntryType { get; set; }
            public string ConceptKey { get; set; }
            public string FirstSourceHookId { get; set; }
            public string FirstSourceTriggerId { get; set; }
            public List<string> NarrativeTags { get; set; }
        }

        private class MissionLeadDto
        {
            public string TriggerId { get; set; }
            public MissionTriggerType TriggerType { get; set; }
            public int Priority { get; set; }
            public string SuggestedMissionFamily { get; set; }
            public string SourceHookId { get; set; }
            public List<string> NarrativeTags { get; set; }
        }

        private class ActiveMissionDto
        {
            public string TriggerId { get; set; }
            public MissionLifecycleState State { get; set; }
            public MissionTriggerType TriggerType { get; set; }
            public int Priority { get; set; }
            public string SuggestedMissionFamily { get; set; }
            public string SourceHookId { get; set; }
            public List<string> NarrativeTags { get; set; }
        }

        private class UnlockDto
        {
            public string UnlockId { get; set; }
            public CodexUnlockType UnlockType { get; set; }
            public CodexEntryType? RequiredEntryType { get; set; }
            public string RequiredEntryId { get; set; }
            public int RequiredCount { get; set; }
            public int RequiredPercentage { get; set; }
            public List<string> NarrativeTags { get; set; }
        }

        private class ClassProfileDto
        {
            public string CurrentClassId { get; set; }
        }

        private class MasteryDto
        {
            public string SkillId { get; set; }
            public int Progress { get; set; }
            public MasteryRank Rank { get; set; }
        }

        private class MentorRelationshipDto
        {
            public string MentorId { get; set; }
            public bool IsDiscovered { get; set; }
            public int Trust { get; set; }
            public bool IsAvailable { get; set; }
        }

        private class SkillTreeProgressDto
        {
            public List<string> DiscoveredNodeIds { get; set; }
            public List<string> UnlockedNodeIds { get; set; }
        }

        private class ClassSkillTreeBindingDto
        {
            public string ClassId { get; set; }
            public string SkillTreeId { get; set; }
        }
    }
}
