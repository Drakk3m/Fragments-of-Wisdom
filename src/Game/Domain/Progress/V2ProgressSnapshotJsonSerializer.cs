using System;
using System.Collections.Generic;
using System.Text.Json;
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

            return dto;
        }

        private static V2ProgressSnapshot FromDto(SnapshotDto dto)
        {
            var codexEntries = new List<V2CodexEntrySnapshot>();
            var missionLeads = new List<V2MissionLeadSnapshot>();

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

            return new V2ProgressSnapshot(
                dto.SchemaVersion,
                codexEntries,
                missionLeads);
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
    }
}
