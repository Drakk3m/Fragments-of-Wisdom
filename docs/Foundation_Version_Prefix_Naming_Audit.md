# Foundation Version Prefix Naming Audit

Branch: `codex/foundation-version-prefix-naming-audit`

## Purpose

This audit classifies active `V1` / `V2` / `V3` / `V4` / `V5`-prefixed names before visual work begins. The target rule is:

- active production APIs, test suites, assertion groups, and living architecture docs should use responsibility-based names
- version prefixes may remain in branch names, migration notes, audit history, schema compatibility notes, and explicitly historical references
- this branch records the rename plan only; it does not perform renames

## Must Rename

These names are active production or production-facing API names. They should be renamed through small dedicated branches.

| Current name | Location | Recommended name | Notes |
| --- | --- | --- | --- |
| `V5RuntimeDemoBootstrap` | `src/Game/Application/Bootstrap/V5RuntimeDemoBootstrap.cs` | `ProgressionRuntimeDemoBootstrap` | Active demo/runtime path and the earliest visual-work blocker. |
| `V4ProgressRestoreCoordinator` | `src/Game/Domain/Progress/V4ProgressRestoreCoordinator.cs` | `ProgressRestoreCoordinator` or `PlayerProgressionRestoreCoordinator` | Active restore/progression coordinator. |
| `V4ProgressRestoreResult` | `src/Game/Domain/Progress/V4ProgressRestoreResult.cs` | `ProgressRestoreResult` or `PlayerProgressionRestoreResult` | Active result type. |
| `V4ProgressRestoreFailureReason` | `src/Game/Domain/Progress/V4ProgressRestoreFailureReason.cs` | `ProgressRestoreFailureReason` or `PlayerProgressionRestoreFailureReason` | Active failure type. |
| `IV2SavePathProvider` | `src/Game/Application/Persistence/IV2SavePathProvider.cs` | `ISavePathProvider` or `IProgressSavePathProvider` | Active application persistence API. |
| `UnityV2SavePathProvider` | `src/Game.Unity/Persistence/UnityV2SavePathProvider.cs` | `UnitySavePathProvider` or `UnityProgressSavePathProvider` | Active Unity adapter. |
| `UnityV2SaveService` | `src/Game.Unity/Persistence/UnityV2SaveService.cs` | `UnitySaveService` or `UnityProgressSaveService` | Active Unity adapter. |
| `V2ProgressSnapshotService` | `src/Game/Domain/Progress/V2ProgressSnapshotService.cs` | `ProgressSnapshotService` | Active save/restore service. |
| `V2ProgressSnapshotJsonSerializer` | `src/Game/Domain/Progress/V2ProgressSnapshotJsonSerializer.cs` | `ProgressSnapshotJsonSerializer` | Active serializer. |
| `V2ProgressSnapshotFileRepository` | `src/Game/Domain/Progress/V2ProgressSnapshotFileRepository.cs` | `ProgressSnapshotFileRepository` | Active repository. |
| `V2ProgressSnapshotValidationService` | `src/Game/Domain/Progress/V2ProgressSnapshotValidationService.cs` | `ProgressSnapshotValidationService` | Active validation service. |
| `V2ProgressSnapshotVersions` | `src/Game/Domain/Progress/V2ProgressSnapshotVersions.cs` | `ProgressSnapshotSchemaVersions` | Keep the internal schema version value, but neutralize the type name. |
| `V2*Snapshot` DTO types | `src/Game/Domain/Progress/V2*Snapshot.cs` | remove `V2` prefix, for example `ProgressSnapshot`, `ClassProfileSnapshot`, `MasterySnapshot` | High blast radius because these are persistence contracts and test fixtures. Rename in a dedicated persistence branch. |

## Should Rename

These names are active test organization, assertion grouping, or living documentation names. They do not change runtime behavior, but they keep versioned framing alive in the current workflow.

| Current name | Location | Recommended name | Notes |
| --- | --- | --- | --- |
| `V4SmokeTests` | `src/Game.SmokeTests/V4SmokeTests.cs` | `ProgressionFoundationSmokeTests` or `FoundationProgressionSmokeTests` | Current V4/V5 progression foundation suite. |
| `RunV4SmokeTests` | `src/Game.SmokeTests/V4SmokeTests.cs`, `SmokeTestRunner.cs` | `RunProgressionFoundationSmokeTests` | Preserve assertion count reporting while renaming the group. |
| `V1SmokeTests` | `src/Game.SmokeTests/V1SmokeTests.cs` | `LegacySliceSmokeTests` | Active legacy guardrail, but the file/class can be neutral. |
| `V2SmokeTests` | `src/Game.SmokeTests/V2SmokeTests.cs` | `NarrativePersistenceSmokeTests` | Active save/narrative/progression guardrail. |
| `V3SmokeTests` | `src/Game.SmokeTests/V3SmokeTests.cs` | `CombatFoundationSmokeTests` | Active combat foundation guardrail. |
| Smoke runner labels such as `V1 Legacy Slice`, `V2 Narrative / Progression / Persistence`, `V3 Combat / Skills / Resonance / Status / Stats`, `V4 Classes / Mentors / Mastery` | `src/Game.SmokeTests/SmokeTestRunner.cs` | `Legacy Slice`, `Narrative / Progression / Persistence`, `Combat / Skills / Resonance / Status / Stats`, `Player Progression / Combat Validation` | Keep count semantics while neutralizing active output. |
| Assertion group labels containing `V4` or `V5` | `src/Game.SmokeTests/V4SmokeTests.cs` | progression/session/bootstrap responsibility names | Rename with the smoke suite to avoid count-report churn in multiple branches. |
| `docs/V4_Classes_Mastery_Foundation.md` | docs | `PlayerProgression_CombatValidation_Foundation.md` | Smallest safe doc rename; split later only if the document becomes too broad. |
| `docs/V3_Combat_Foundation.md` | docs | `Combat_Foundation.md` | Active living combat doc. |
| `docs/V2_Save_Foundation.md` | docs | `Save_Persistence_Foundation.md` | Active persistence doc. |
| `docs/V2_Narrative_Foundation.md` | docs | `Narrative_Progression_Foundation.md` | Active narrative/progression doc. |

## Deferred Low-Risk Internal Diagnostics

These are active internal facts or enum values, but they are lower risk to defer because they are diagnostic policy labels rather than public composition entry points. They should still be renamed before the naming cleanup is fully closed.

| Current name | Location | Recommended name |
| --- | --- | --- |
| `V3AccessChecked` | `CombatSkillUseValidationResult` | `UnlockAccessChecked` or `LegacyUnlockAccessChecked` |
| `V4AccessChecked` | `CombatSkillUseValidationResult` | `ProgressionAccessChecked` |
| `V3BlockReason` | `CombatSkillUseValidationResult`, `CombatSkillTurnValidationResult` | `UnlockBlockReason` |
| `V4BlockReason` | `CombatSkillUseValidationResult`, `CombatSkillTurnValidationResult` | `ProgressionBlockReason` |
| `V3UnlockProfileMissing` | `CombatSkillUseBlockReason` | `UnlockProfileMissing` |
| `V3SkillLocked` | `CombatSkillUseBlockReason` | `SkillLocked` |
| `V4ContextIncomplete` | `CombatSkillUseBlockReason` | `ProgressionContextIncomplete` |
| `V4SkillBlocked` | `CombatSkillUseBlockReason` | `ProgressionSkillBlocked` |

## Historical References To Keep

The following version-prefixed references are acceptable when clearly historical or compatibility-scoped:

- branch names in audit notes and changelog-style history
- migration notes that explain how a feature moved from one foundation slice to another
- explicit count guardrails such as "V1 remains 38 events" when documenting historical compatibility
- schema compatibility notes that mention an old save schema by version
- snapshot generator profile names such as `VerticalSliceV1` / `VerticalSliceV2` when they intentionally reproduce historical slice output
- script names that invoke historical snapshot generation, unless they become part of the new active demo path

## Recommended Rename Sequence

1. `codex/rename-runtime-demo-bootstrap-api`
   - Rename `V5RuntimeDemoBootstrap` to `ProgressionRuntimeDemoBootstrap`.
   - Update usages, smoke assertions, and docs.
   - This is the smallest branch that unblocks visual smoke demo design.

2. `codex/rename-progression-foundation-smoke-suite`
   - Rename `V4SmokeTests` and `RunV4SmokeTests`.
   - Neutralize active V4/V5 assertion group labels.
   - Preserve V1 event count, V3 assertion count, grouped progression count, and total assertion count.

3. `codex/rename-progression-foundation-docs`
   - Rename `V4_Classes_Mastery_Foundation.md` to `PlayerProgression_CombatValidation_Foundation.md`.
   - Update active headings and links.
   - Keep audit-history version names clearly marked as history.

4. `codex/rename-progress-restore-api`
   - Rename `V4ProgressRestoreCoordinator`, `V4ProgressRestoreResult`, and `V4ProgressRestoreFailureReason`.
   - Update tests/docs without changing restore behavior.

5. `codex/rename-combat-validation-diagnostics`
   - Rename `V3*` / `V4*` combat validation diagnostic properties and enum values to unlock/progression terms.
   - Keep event payload semantics stable.

6. `codex/rename-legacy-combat-and-narrative-smoke-suites`
   - Rename `V1SmokeTests`, `V2SmokeTests`, `V3SmokeTests`, their runner methods, and active group labels.
   - Keep historical count references in docs and final reports.

7. `codex/rename-progress-persistence-api`
   - Rename `V2` persistence services, repository, serializer, validation service, path providers, Unity adapters, and snapshot DTO types.
   - This has the largest blast radius and should be isolated from the demo/bootstrap naming cleanup.

## Blast Radius Summary

- `V5RuntimeDemoBootstrap`: low. One production class plus focused tests/docs.
- `V4SmokeTests`: medium. Test file/class/method names, runner labels, assertion output strings, docs.
- `V4_Classes_Mastery_Foundation.md`: medium. File rename plus links and active headings.
- `V4ProgressRestore*`: medium. Production files, type references, tests, docs.
- Combat validation diagnostic facts: medium. Production result models, validators, smoke assertions, docs.
- `V1` / `V2` / `V3` smoke suite names: medium. Test organization and count reporting only.
- `V2` persistence APIs and DTOs: high. Production persistence surface, Unity adapters, tests, docs, and schema compatibility language.

## Audit Decision

Naming consistency is a real blocker for visual smoke demo work because the active demo entry point still exposes `V5` in its production-facing API. The earliest safe atomic branch is:

`codex/rename-runtime-demo-bootstrap-api`

That branch should rename only `V5RuntimeDemoBootstrap` to `ProgressionRuntimeDemoBootstrap`, update direct usages and documentation, and rerun the full smoke suite before any visual design begins.
