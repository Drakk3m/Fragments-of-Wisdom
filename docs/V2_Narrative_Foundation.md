# V2 Narrative Foundation

This document describes the current Vertical Slice V2 narrative foundation. It is meant to help future Codex branches and contributors extend the architecture without collapsing domain boundaries or changing the V1 gameplay loop by accident.

For V2 profile persistence, snapshots, JSON, file IO, and Unity save adapters, see [V2_Save_Foundation.md](V2_Save_Foundation.md).

## Current Flow

```text
PlayerProfile
  -> Alignment state
  -> FirstNahualTeachingEvaluator
  -> TransmutationAccessPolicy
  -> NpcReactionService
  -> NarrativeHookService
  -> MissionTriggerService
  -> MissionTriggerCandidateDeduplicator
  -> CodexEntryService
  -> CodexEntryCandidateDeduplicator
```

The flow is fact-based. Each step reads the facts it is allowed to know about, returns a lightweight result, and leaves state mutation to future explicit systems.

## Layer Responsibilities

### PlayerProfile and Alignment

`PlayerProfile` is the source of current player state for this foundation. The V2 narrative path currently reads:

- `CurrentAlignment`
- `AlignmentProgression`
- `AlignmentFlags`

Alignment remains domain state. It should not know about UI, scenes, missions, codex entries, combat effects, dialogue trees, or Unity objects.

### FirstNahualTeachingEvaluator

`FirstNahualTeachingEvaluator` reads `PlayerProfile` alignment state and returns `FirstNahualTeachingResult`.

It decides only whether the first Nahual teaching path is accepted or rejected, and emits narrative tags describing those facts. It does not unlock mechanics, start missions, modify profile state, or drive presentation.

### TransmutationAccessPolicy

`TransmutationAccessPolicy` reads either a `PlayerProfile` or a `FirstNahualTeachingResult` and returns `TransmutationAccessResult`.

It decides whether transmutation access is currently allowed, which alignment or teaching path is required, and why access is blocked when it is blocked. It does not implement transmutation mechanics, item conversion, combat effects, rewards, or UI.

### NpcReactionService

`NpcReactionService` consumes narrative tags and returns `NpcReactionResult`.

It maps existing facts into NPC reaction facts such as tone, trust preview, fear preview, available narrative hooks, and reaction tags. It does not mutate reputation, spawn dialogue, trigger missions, or touch `PlayerProfile`.

### NarrativeHookService

`NarrativeHookService` consumes narrative tags from `FirstNahualTeachingResult`, `TransmutationAccessResult`, `NpcReactionResult`, or a raw tag list and returns `NarrativeHook` facts.

It translates already-known narrative facts into hook candidates for future systems. It does not create mission state, codex entries, dialogue branches, world state, or Unity scene behavior.

### MissionTriggerService

`MissionTriggerService` consumes `NarrativeHook` facts and returns `MissionTriggerCandidate` facts.

It names possible mission trigger candidates for future journal or mission systems. It does not start missions, track objectives, apply rewards, save progress, or alter the V1 slice.

### CodexEntryService

`CodexEntryService` consumes `NarrativeHook` facts and `MissionTriggerCandidate` facts and returns `CodexEntryCandidate` facts.

It names possible encyclopedia/codex entry candidates by entry id, type, priority, source hook or trigger id, concept key, and narrative tags. It does not unlock entries, write to `CodexProfile`, create an encyclopedia database, track completion, grant rewards, save progress, or drive UI.

### Candidate Deduplication

Candidate deduplication is a pure domain cleanup step applied after candidates are generated. It does not change generation rules; it only collapses repeated facts into stable candidate lists.

Current rules:

- `CodexEntryCandidate` values deduplicate by `EntryId`.
- `MissionTriggerCandidate` values deduplicate by `TriggerId`.
- When duplicates exist, the candidate with the highest `Priority` provides the retained candidate fields.
- `NarrativeTags` are merged in first-seen order without duplicates.

## Allowed Dependencies

The dependency direction should stay narrow and domain-first:

- `Game.Domain.Player` may own player state containers.
- `Game.Domain.Alignment` may define alignment values, flags, and progression state.
- `Game.Domain.Narrative` may read alignment/player facts and emit narrative facts.
- `Game.Domain.Transmutation` may depend on narrative facts when deciding access policy.
- `Game.Domain.World` may consume narrative tags to preview NPC reaction facts.
- `Game.Domain.Missions` may consume narrative hooks to preview mission trigger candidates.
- `Game.Domain.Codex` may consume narrative hooks and mission trigger candidates to preview codex entry candidates.
- Smoke tests may compose the full chain to prove the foundation still works.

Domain services may depend on other pure domain services or result objects when that keeps the flow explicit. Prefer constructor injection only when a service genuinely composes another service, as `TransmutationAccessPolicy` does with `FirstNahualTeachingEvaluator`.

## Forbidden Couplings

Do not introduce these couplings into the V2 foundation:

- Domain services depending on Unity `MonoBehaviour`, `GameObject`, scenes, prefabs, assets, or serialized references.
- Narrative evaluators starting missions, adding codex entries, opening UI, triggering combat, granting rewards, or changing inventory.
- NPC reaction services mutating reputation, world state, or `PlayerProfile`.
- Mission trigger candidates becoming active mission tracking or journal state.
- Codex entry candidates becoming active encyclopedia entries, completion state, rewards, persistence, or UI.
- Transmutation access checks implementing actual transmutation mechanics.
- Generated Unity YAML edits for narrative foundation work.
- Giant orchestration MonoBehaviours that contain gameplay rules.
- Hidden dependencies through global state, static mutable registries, scene lookups, or service locators.

## Architectural Philosophy

The V2 foundation exists to create stable facts before full systems are built.

- Keep pure domain first.
- Keep evaluators side-effect free.
- Keep domain code free of Unity dependencies.
- Avoid giant MonoBehaviours.
- Let narrative systems consume facts rather than mutate state.
- Preserve the existing V1 gameplay loop and 38-event smoke assertion.
- Prefer small classes with one clear responsibility.
- Add integration only through explicit adapters when a future Unity or gameplay system is ready.

## Naming Conventions

Use stable, centralized names for cross-system facts:

- Narrative tags live in `NarrativeTags`.
- Narrative hook ids live in `NarrativeHookIds`.
- Suggested downstream systems live in `SuggestedNarrativeSystem`.
- NPC narrative hook ids live in `NpcNarrativeHooks`.
- Transmutation block reasons live in `TransmutationAccessBlockReasons`.
- Mission trigger ids live in `MissionTriggerIds`.
- Mission family names live in `MissionFamilies`.
- Codex entry ids live in `CodexEntryIds`.
- Codex concept/title keys live in `CodexConceptKeys`.

Prefer clear domain names over generic ids. Avoid repeated string literals for tags, hook ids, trigger ids, codex entry ids, concept keys, mission families, suggested systems, or block reasons.

## Narrative Tag Philosophy

Narrative tags are lightweight facts, not commands.

A tag should say what is true, not what a future system must do. For example, a tag can describe that transmutation access is blocked or that the first Nahual teaching was accepted. It should not say that a UI panel must open, a quest must start, or an NPC must speak a specific line.

Tags should be:

- Stable enough for multiple services to consume.
- Small and readable.
- Centralized when reused.
- Safe to pass through result objects.
- Free of presentation, scene, and persistence assumptions.

When adding a tag, add or update smoke coverage for the behavior that consumes it.

## Future Extension Points

### Codex System

Future codex work should consume `CodexEntryCandidate` facts. Codex systems should decide how and when entries are discovered, persisted, displayed, or completed outside the evaluator chain.

### Mission Journal

Future mission journal work should consume `MissionTriggerCandidate` facts. The current service only suggests candidates; a future journal system can decide whether to create, ignore, defer, or merge them.

### Dialogue System

Future dialogue work should consume `NpcReactionResult`, `NarrativeHook`, or narrative tags. Dialogue selection should not be embedded in `NpcReactionService`; that service only previews tone, deltas, hooks, and tags.

### Transmutation Mechanics

Future transmutation mechanics should call `TransmutationAccessPolicy` before doing work. The policy should remain a gate that returns facts. Actual conversion, costs, combat effects, inventory changes, and animation should live in separate systems.

### Reputation and World State

Future reputation or world-state systems may consume `NpcReactionResult` or `NarrativeHook` facts. They should apply durable state changes in explicit services, not inside reaction or hook resolution.

## Validation Workflow

Run these commands after changing the V2 foundation:

```powershell
dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal
dotnet run --project src\Game.SmokeTests\Game.SmokeTests.csproj --no-build
```

Current smoke expectations:

- V1 slice emitted event count remains `38`.
- Alignment assertions still run.
- Nahual evaluator assertions still run.
- Transmutation policy assertions still run.
- NPC reaction assertions still run.
- Narrative hook assertions still run.
- Mission trigger assertions still run.
- Codex entry assertions still run.
- Candidate deduplication assertions still run.
- Codex profile assertions still run.
- Mission journal assertions still run.
- Discovery orchestration assertions still run.
- V2 save snapshot assertions still run.
- V2 save JSON assertions still run.
- V2 save file repository assertions still run.
- V2 foundation flow assertions still run.
- Total smoke assertions remain `149` unless a branch intentionally adds or renames smoke coverage and explains why.

Documentation-only changes should not alter smoke counts, gameplay behavior, Unity assets, scenes, prefabs, or `.meta` files.
