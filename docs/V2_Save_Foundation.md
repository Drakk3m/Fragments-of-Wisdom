# V2 Save Foundation

This document describes the current V2 save and persistence foundation. It covers only the lightweight state profiles and save plumbing that exist today; it does not describe a final save system, save slots, UI, autosave, or Unity scene integration.

## Current Flow

```text
CodexEntryCandidate facts
  -> CodexDiscoveryService
  -> CodexProfile

MissionTriggerCandidate facts
  -> MissionDiscoveryService
  -> MissionJournalProfile

CodexProfile + MissionJournalProfile
  -> DiscoveryOrchestrationService
  -> V2ProgressSnapshotService
  -> V2ProgressSnapshot
  -> V2ProgressSnapshotJsonSerializer
  -> V2ProgressSnapshotFileRepository

UnityV2SavePathProvider
  -> UnityV2SaveService
  -> V2ProgressSnapshotFileRepository
```

## Stateful Profiles

### CodexProfile

`CodexProfile` tracks discovered codex entries. Candidate-backed discoveries are stored as `CodexEntryRecord` values, preserving:

- `EntryId`
- `EntryType`
- `ConceptKey`
- first source hook id
- first source trigger id
- narrative tags

Discovery is idempotent by `EntryId`. Re-applying the same candidate does not duplicate records.

### MissionJournalProfile

`MissionJournalProfile` tracks available mission leads. Mission leads are stored as `MissionLeadRecord` values, preserving:

- `TriggerId`
- `TriggerType`
- `Priority`
- `SuggestedMissionFamily`
- `SourceHookId`
- narrative tags

Discovery is idempotent by `TriggerId`. Re-applying the same trigger candidate does not duplicate records.

## Discovery Services

`CodexDiscoveryService` consumes `CodexEntryCandidate` facts only and applies them to `CodexProfile`.

`MissionDiscoveryService` consumes `MissionTriggerCandidate` facts only and applies them to `MissionJournalProfile`.

`DiscoveryOrchestrationService` composes both discovery services. It applies supplied codex and mission candidate collections to supplied profiles and returns a lightweight summary of newly discovered ids and already-known counts. It does not contain mapping rules and does not generate candidates.

## Snapshot Model

`V2ProgressSnapshot` is the current in-memory DTO for V2 profile persistence. It captures:

- schema version
- codex discovered-entry snapshots
- mission lead snapshots

`V2ProgressSnapshotService` creates snapshots from `CodexProfile` and `MissionJournalProfile`, and restores profiles from a snapshot. Restore mutates only those profiles and remains idempotent because the profiles enforce identity rules.

## Versioning

Current snapshot schema version is centralized in `V2ProgressSnapshotVersions.CurrentSchemaVersion`.

Versioning rules:

- new snapshots use the current schema version
- `V2ProgressSnapshot.IsCurrentVersion()` checks whether a snapshot matches the current schema
- `V2ProgressSnapshotService.CanRestore()` returns true only for current-version snapshots
- restore rejects unsupported versions before mutating target profiles
- no migration support exists yet

## JSON Serialization

`V2ProgressSnapshotJsonSerializer` serializes and deserializes `V2ProgressSnapshot` only.

Serialization rules:

- use built-in `System.Text.Json`
- preserve schema version, codex entries, mission leads, tags, ids, priorities, concept keys, source ids, and mission families
- validate schema version during deserialization using the existing restore/version rules
- malformed JSON fails through `JsonException`
- unsupported schema JSON fails through `InvalidOperationException`

The serializer does not perform file IO, choose paths, manage save slots, or invoke Unity APIs.

## File Repository

`V2ProgressSnapshotFileRepository` owns file IO for V2 snapshot JSON.

Repository rules:

- `Save(path, snapshot)` serializes and writes to the exact path supplied
- `Save` creates the parent directory when needed
- `Load(path)` remains strict for missing files
- `Load` delegates malformed JSON and unsupported schema handling to the JSON serializer

The repository does not know about Unity, save slots, PlayerPrefs, cloud saves, encryption, compression, UI, or gameplay triggers.

## Unity Boundary

Unity-facing code is isolated under `src/Game.Unity`.

`UnityV2SavePathProvider` resolves the V2 save file path using `Application.persistentDataPath`.

`UnityV2SaveService` is a thin facade that composes:

- `IV2SavePathProvider`
- `V2ProgressSnapshotFileRepository`
- `V2ProgressSnapshotService`

The facade exposes:

- `Save(CodexProfile, MissionJournalProfile)`
- `LoadInto(CodexProfile, MissionJournalProfile)`
- `Exists()`

Actual JSON and file persistence remain in the repository and serializer. Unity-specific path resolution remains behind `IV2SavePathProvider`.

## Explicit Non-Goals

The current foundation does not include:

- save slot UI
- autosave
- PlayerPrefs
- `Application.persistentDataPath` in domain code
- encryption
- compression
- cloud saves
- version migrations
- gameplay-triggered saves
- active quest objectives or quest completion
- codex completion percentages
- scene, prefab, or asset changes

## Validation Workflow

Run these commands after changing the V2 save foundation:

```powershell
dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal
dotnet run --project src\Game.SmokeTests\Game.SmokeTests.csproj --no-build
```

Current smoke expectations:

- V1 slice emitted event count remains `38`.
- Codex profile assertions still run.
- Mission journal assertions still run.
- Discovery orchestration assertions still run.
- V2 save snapshot assertions still run.
- V2 save JSON assertions still run.
- V2 save file repository assertions still run.
- Total smoke assertions remain `149` unless a branch intentionally adds or renames smoke coverage and explains why.

Unity-facing files are outside the current .NET solution. Validate them manually in Unity by compiling the `src/Game.Unity` adapter code in a Unity assembly that references the game assembly, then exercising `UnityV2SavePathProvider` and `UnityV2SaveService` in Play Mode.
