# V4 Classes, Mentors, Mastery Foundation

This document describes the current Vertical Slice V4 foundation for classes,
mentors, mastery, mentor teaching, persistence, mastered skill retention, and
skill tree progression with class-specific skill tree binding facts.

The scope remains intentionally small and domain-first. V4 stores and evaluates
facts; it does not execute combat skills, grant rewards, drive UI, or introduce
Unity behavior.

## Current Flow

```text
SkillDefinition
  -> SkillDefinitionRegistry
  -> ClassDefinition allowed skill ids
  -> ClassDefinitionRegistry
  -> ClassProfile current class id
  -> SkillTreeId stable skill tree ids
  -> SkillTreeDefinition id/node/prerequisite facts
  -> ClassSkillTreeBinding class-to-skill-tree facts
  -> ClassSkillTreeBindingRegistry
  -> SkillTreeProgressProfile discovered/unlocked facts
  -> SkillTreeUnlockPolicy
  -> MasteryProfile mastered skill facts
  -> MasteredSkillRetentionPolicy
```

```text
MentorDefinition
  -> MentorDefinitionRegistry
  -> MentorRelationshipProfile discovery/trust/availability facts
  -> MentorTeachingService teaching facts
  -> MentorTeachingApplicationService explicit MasteryProfile progress
```

`UnlockProfile`, combat execution, UI, and Unity presentation remain separate.

## Class Foundation

`ClassId` centralizes stable class ids.

Current ids:

- `ClassId.Tlamatini`
- `ClassId.Wanderer`

`ClassDefinition` describes a class:

- `Id`
- `Name`
- `FixedSkillIds`
- `AllowedSkillIds`
- `AllowsSkill(skillId)`
- `AllowsSkill(skillId, skillRegistry)`
- `ResolveAllowedSkills(skillRegistry)`

`ClassDefinitionRegistry` owns class definition lookup:

- `Register(classDefinition)`
- `Contains(classId)`
- `Get(classId)`
- `ResolveCurrentClass(profile)`

Registry rules:

- valid class definitions register by stable id
- null, empty id, and empty name are rejected safely
- duplicate class ids are rejected safely
- duplicate class ids do not replace the original definition

`ClassProfile` owns only current class id state:

- `CurrentClassId`
- `AssignClass(classDefinition)`
- `ChangeClass(classDefinition)`
- `AssignClassId(classId, registry)`
- `ChangeClassId(classId, registry)`
- `ResolveCurrentClass(registry)`
- `CurrentClassAllowsSkill(skillId, registry)`
- `CurrentClassAllowsSkill(skillId, classRegistry, skillRegistry)`
- `ResolveCurrentClassAllowedSkills(classRegistry, skillRegistry)`

Class changes do not mutate unlocks, mastery, mentors, combat state, inventory,
or skill definitions.

## Skill Definition Registry

`SkillDefinitionRegistry` is the stable source of truth for skill ids used by
class, skill tree nodes, mastery, mentor teaching, retention, and snapshot
validation.

Rules:

- valid skill definitions register by stable id
- null, empty id, and empty name are rejected safely
- duplicate skill ids are rejected safely
- unknown skill ids fail safely when a registry-aware operation is used

## Skill Tree Progression

`SkillTreeId` centralizes stable skill tree ids.

Current ids:

- `SkillTreeId.Tlamatini`
- `SkillTreeId.Wanderer`

`SkillTreeNodeDefinition` describes a skill tree node:

- `Id`
- `SkillId`
- `RequiredNodeIds`

Node rules:

- node ids are stable ids
- skill ids must resolve through `SkillDefinitionRegistry`
- missing prerequisite lists are treated as empty
- prerequisite ids are copied as stable node id facts

`SkillTreeDefinition` owns a stable skill tree id and node lookup:

- `Id`
- `RegisterNode(node, skillRegistry)`
- `Contains(nodeId)`
- `Get(nodeId)`
- `IsValidNode(node, skillRegistry)`

Registry rules:

- stable skill tree ids are optional for standalone node/progression tests
- stable skill tree ids are required when registering a tree for class binding
- valid nodes register by stable node id
- prerequisite nodes must already exist in the same `SkillTreeDefinition`
- nodes must be registered in prerequisite-first order
- forward prerequisite references are rejected safely
- self-references are rejected because the node does not exist before its own
  registration
- cycles cannot be introduced through incremental registration because every
  dependency must already exist
- missing prerequisite ids reject the node without mutating the tree
- null or empty node ids are rejected safely
- nodes with unknown skill ids are rejected safely
- duplicate node ids are rejected safely
- duplicate node ids do not replace the original node

`SkillTreeProgressProfile` owns progression facts:

- discovered node ids scoped by `SkillTreeId`
- unlocked node ids scoped by `SkillTreeId`
- `DiscoverNode(nodeId, skillTree)`
- `IsDiscovered(nodeId)`
- `IsDiscovered(nodeId, skillTree)`
- `IsUnlocked(nodeId)`
- `IsUnlocked(nodeId, skillTree)`

Progress rules:

- discovery validates node ids through `SkillTreeDefinition`
- policy and application queries use the supplied `SkillTreeDefinition.Id`
- identical node ids in different trees do not share progress
- legacy node-only queries remain aggregate compatibility views
- public unlock mutation is applied through `SkillTreeUnlockApplicationService`
- the profile's low-level unlock recorder is internal for application and
  snapshot restoration only
- unlock records node facts only
- unlock does not grant skills automatically
- unlock does not mutate mastery

`SkillTreeUnlockPolicy` evaluates whether a node can be unlocked.

It returns `SkillTreeUnlockResult` facts:

- `NodeId`
- `SkillId`
- `CanUnlock`
- `IsDiscovered`
- `IsAlreadyUnlocked`
- `MissingPrerequisiteNodeId`
- `BlockReason`

Block reasons:

- `None`
- `UnknownNode`
- `UnknownSkill`
- `NodeNotDiscovered`
- `PrerequisiteMissing`

Unlock policy rules:

- unknown nodes fail safely
- node skill ids are validated through `SkillDefinitionRegistry`
- undiscovered nodes are blocked
- prerequisite node ids must already be unlocked
- unlocked prerequisites allow dependent nodes to become unlockable
- the policy returns facts only and does not grant skills

`SkillTreeUnlockApplicationService` is the public policy-enforced mutation
path. It evaluates `SkillTreeUnlockPolicy` before recording an unlock and does
not mutate progress when the policy blocks the node.

## Class Skill Tree Binding

`ClassSkillTreeBinding` stores a class-to-skill-tree fact:

- `ClassId`
- `SkillTreeId`

`ClassSkillTreeBindingRegistry` owns skill tree registration and class binding
lookup:

- `RegisterSkillTree(skillTree)`
- `RegisterBinding(binding, classRegistry)`
- `ContainsSkillTree(skillTreeId)`
- `GetSkillTree(skillTreeId)`
- `GetBindingForClass(classId)`
- `ResolveSkillTreeForClass(classId)`
- `ResolveCurrentClassSkillTree(profile)`

Binding rules:

- class ids validate through `ClassDefinitionRegistry`
- skill tree ids validate through registered `SkillTreeDefinition` values
- null bindings, empty class ids, and empty skill tree ids are rejected safely
- skill trees with empty ids are rejected by the binding registry
- duplicate class bindings are rejected safely
- duplicate class bindings do not replace the original binding
- current class skill tree resolution reads `ClassProfile.CurrentClassId`
- binding returns facts only
- binding does not grant skills automatically
- binding does not mutate `SkillTreeProgressProfile`

## Class Skill Tree Binding Snapshot And Restore

Class skill tree binding persistence extends `V2ProgressSnapshot` without a
schema version bump.

Each binding snapshot stores:

- `ClassId`
- `SkillTreeId`

Restore rules:

- class ids validate through `ClassDefinitionRegistry`
- skill tree ids validate through trees registered in
  `ClassSkillTreeBindingRegistry`
- the complete binding collection is validated before mutation
- unknown ids, duplicate class ids, or conflicting existing bindings fail
  safely
- invalid binding collections do not partially mutate the target registry
- matching existing bindings restore idempotently
- older JSON without binding data restores safely as an empty collection

## Skill Tree Snapshot And Save Behavior

Skill tree progress persistence extends `V2ProgressSnapshot` in a
backward-compatible way.

Scoped snapshot records store:

- `SkillTreeId`
- `DiscoveredNodeIds`
- `UnlockedNodeIds`

Legacy unscoped discovered/unlocked lists remain readable for older JSON.

Compatibility policy:

- legacy-only progress is valid
- scoped-only progress is valid
- empty legacy lists may accompany scoped progress
- snapshots containing both non-empty legacy progress and scoped progress are
  rejected as ambiguous
- older JSON without skill tree progress remains valid as empty progress

Restore rules:

- scoped skill tree ids validate through registered `SkillTreeDefinition`
  values
- scoped node ids validate against their matching tree
- multi-tree restore validates all scopes before mutation
- legacy unscoped progress restores when an explicit target tree is supplied
- registry-based legacy restore requires exactly one target tree
- ambiguous legacy progress with multiple target trees fails without mutation
- unknown node ids fail safely and do not mutate the target profile
- older JSON without skill tree progress data restores safely as empty progress
  state
- schema version remains unchanged because missing skill tree progress data is
  optional

`V2ProgressSnapshotValidationService` rejects null skill tree progress sections,
null discovered/unlocked node id lists, empty node ids, empty scoped skill tree
ids, and duplicate skill tree scopes.

Registry-aware validation additionally uses registered
`SkillTreeDefinition` values to require:

- every scoped `SkillTreeId` resolves
- every discovered and unlocked node id exists in its scoped tree
- every unlocked node is also discovered
- every prerequisite of an unlocked node is also unlocked

Validation without a skill tree registry remains structural-only for backward
compatibility.

## Mastery Foundation

`MasteryRecord` stores skill mastery facts:

- `SkillId`
- `Progress`
- `Rank`
- `IsMastered`

`MasteryRank` is derived from progress:

- `Unranked`
- `Practiced`
- `Mastered`

`MasteryProfile` owns mastery records:

- `Records`
- `IncreaseProgress(skillId, amount, skillRegistry)`
- `GetRecord(skillId)`
- `IsMastered(skillId)`
- `RestoreRecord(skillId, progress, skillRegistry)`

Rules:

- progress only applies for registered skill ids
- zero or negative progress does not increase mastery
- mastered progress is capped at `MasteryRecord.MasteredProgress`
- duplicate progress after mastery is safe
- mastery remains facts only

## Mastery Snapshot And Save Behavior

Mastery persistence extends `V2ProgressSnapshot` in a backward-compatible way.

Snapshot stores:

- `SkillId`
- `Progress`
- derived `Rank`

Restore rules:

- restore validates skill ids through `SkillDefinitionRegistry`
- known skill ids restore into `MasteryProfile`
- unknown skill ids fail safely and do not create mastery records
- older JSON without mastery data restores safely as empty mastery state
- schema version remains unchanged because missing mastery data is optional

`V2ProgressSnapshotValidationService` also supports registry-aware mastery
validation. When a `SkillDefinitionRegistry` is supplied, mastery snapshot skill
ids must resolve through that registry.

## Mentor Foundation

`MentorId` centralizes stable mentor ids.

Current ids:

- `MentorId.Lyra`
- `MentorId.Atlas`

`MentorDefinition` describes a mentor:

- `Id`
- `Name`
- `IsAvailable`
- `IsValid()`

`MentorDefinitionRegistry` owns mentor definition lookup:

- `Register(mentor)`
- `Contains(mentorId)`
- `Get(mentorId)`

Registry rules:

- valid mentor definitions register by stable id
- null, empty id, and empty name are rejected safely
- duplicate mentor ids are rejected safely
- duplicate mentor ids do not replace the original definition

`MentorRelationshipProfile` owns relationship facts:

- discovered mentor ids
- trust/progress per mentor id
- availability fact per discovered mentor id

Relationship rules:

- discovery validates mentor ids through `MentorDefinitionRegistry`
- duplicate discovery is idempotent
- trust increases only for discovered, registered mentors
- unavailable mentors are reported as unavailable
- unknown mentor ids fail safely

## Mentor Relationship Snapshot And Save Behavior

Mentor relationship persistence extends `V2ProgressSnapshot` in a
backward-compatible way.

Snapshot stores:

- `MentorId`
- `IsDiscovered`
- `Trust`
- `IsAvailable`

Restore rules:

- restore validates mentor ids through `MentorDefinitionRegistry`
- known mentor ids restore discovered, trust, and availability facts
- unknown mentor ids fail safely and do not create relationship state
- older JSON without mentor relationship data restores safely as empty mentor
  relationship state
- schema version remains unchanged because missing mentor relationship data is
  optional

`V2ProgressSnapshotValidationService` rejects null mentor relationship
collections and mentor relationship records with empty mentor ids.

## Mentor Teaching

`MentorTeachingDefinition` describes a teaching fact:

- `MentorId`
- `TaughtSkillId`
- `RequiredTrust`
- `MasteryProgressGranted`

`MentorTeachingService` evaluates availability. It validates:

- mentor id through `MentorDefinitionRegistry`
- taught skill id through `SkillDefinitionRegistry`
- mentor discovery through `MentorRelationshipProfile`
- required trust through `MentorRelationshipProfile`

It returns `MentorTeachingResult` facts only:

- availability
- mentor id
- taught skill id
- required trust
- current trust
- mastery progress granted
- stable block reason
- optional block message

Teaching does not grant skills, mutate mastery, start missions, affect combat, or
drive UI.

`MentorTeachingApplicationService` explicitly consumes an available
`MentorTeachingResult` and applies its mastery progress to `MasteryProfile`.

Application rules:

- blocked teaching does not mutate mastery
- unknown skill ids fail safely
- missing mastery profile fails safely
- zero or negative mastery progress is not applied
- already-mastered skills are safe and are not over-applied
- successful application mutates only `MasteryProfile`

## Mastered Skill Retention

`MasteredSkillRetentionPolicy` evaluates whether a registered skill is usable
after class changes.

A skill is usable if:

- the current class allows it, or
- it is mastered in `MasteryProfile`

The policy returns facts only:

- `SkillId`
- `IsUsable`
- `AllowedByCurrentClass`
- `AllowedByMastery`
- stable block reason

Rules:

- skill ids must resolve through `SkillDefinitionRegistry`
- class access is resolved through `ClassProfile` and `ClassDefinitionRegistry`
- mastery access is resolved through `MasteryProfile`
- no class is required when mastery allows the skill
- the policy does not mutate `ClassProfile` or `MasteryProfile`
- the policy does not execute skills or integrate with combat

## Combat-Facing Skill Access

`CombatSkillAccessService` is a read-only adapter from combat-facing callers to
`MasteredSkillRetentionPolicy`.

It returns `CombatSkillAccessResult` facts:

- `SkillId`
- `IsUsable`
- `AllowedByCurrentClass`
- `AllowedByMastery`
- `BlockReason`

Access rules:

- current-class skills are usable
- mastered retained skills remain usable after a class switch
- non-mastered cross-class skills are blocked
- unknown skill ids fail safely
- output facts are copied directly from `MasteredSkillRetentionPolicy`
- no profile, registry, combat engine, or skill execution state is mutated
- `TurnBasedCombatEngine` does not consume the service yet

## Combat Skill Use Validation

`CombatSkillUseValidator` composes structural request validation with the
existing V3 `SkillAccessPolicy` and V4 `CombatSkillAccessService`.

`CombatSkillUseValidationContext` provides explicit validation modes:

- `LegacyBypass` validates request structure and matching skill ids while
  bypassing V3/V4 progression rules
- `Enemy` validates request structure and matching skill ids without requiring
  player progression profiles
- `Player` requires `UnlockProfile`, `ClassProfile`, `MasteryProfile`,
  `ClassDefinitionRegistry`, and `SkillDefinitionRegistry`

Static context factories create each mode. Partial player context fails with
the stable `PlayerContextIncomplete` block reason. A missing context fails with
`ValidationContextMissing`.

`CombatSkillUseValidationContextProvider` centralizes context creation:

- `CreateLegacyBypass` creates only the explicit legacy mode
- `CreateEnemy` creates enemy mode without player progression dependencies
- `TryCreatePlayer` requires all five player profiles and registries
- missing player dependencies return `false` with a null context
- failed player creation never falls back to legacy or enemy mode
- produced contexts are directly consumable by `CombatSkillUseValidator`

`CombatActorValidationClassifier` determines the validation mode for a combat
actor without creating contexts or mutating combat state.

Classification rules:

- an explicitly requested legacy path classifies as `LegacyBypass`
- the runtime enemy classifies as `Enemy` without player dependencies
- the runtime player classifies as `Player` only when all five player
  dependencies are present
- null runtime, null actor, incomplete player dependencies, and actors outside
  the runtime fail with stable classification reasons
- failed classification returns `Unknown` with no validation mode
- classification uses actor identity from `CombatRuntimeContext`
- `TurnBasedCombatEngine` does not consume the classifier directly; the
  configured gateway owns classification

`CombatSkillValidationOrchestrator` composes the production validation flow:

1. classify the actor
2. create the matching validation context
3. run `CombatSkillUseValidator`
4. return combined classification and validation facts

`CombatSkillValidationOrchestrationResult` exposes whether orchestration
reached validation, whether the skill is allowed, whether a context was
created, the selected mode, the original classification, the validator result,
and a stable failure stage.

Failure behavior:

- classification failures preserve the classifier reason and do not create a
  context
- context creation failures preserve the selected classification mode
- blocked skills complete orchestration but report the `Validation` failure
  stage and preserve V3/V4 validator reasons
- successful validation reports no failure stage
- the orchestrator is read-only and does not emit events or execute combat
- `TurnBasedCombatEngine` does not consume the orchestrator directly; the
  configured gateway owns orchestration

`ICombatSkillTurnValidator` is the engine-facing validation boundary. It
exposes only:

```csharp
CombatSkillTurnValidationResult Validate(
    TurnRequest request,
    CombatRuntimeContext runtime);
```

`CombatSkillTurnValidatorGateway` implements that boundary and owns the
configuration needed by `CombatSkillValidationOrchestrator`. The default
constructor selects explicit `LegacyBypass`. Strict runtime classification can
be configured without player dependencies for enemy actions, while the player
constructor receives the complete unlock, class, mastery, and registry state
once at composition time.

`CombatSkillTurnValidationResult` flattens the engine-relevant facts:

- whether the skill is allowed
- selected validation mode
- orchestration failure stage
- actor classification failure reason
- skill-use block reason
- underlying V3 and V4 block reasons

The gateway derives the actor from `TurnRequest.Attacker`. Engine-facing
callers do not receive or pass the orchestrator, classifier, context provider,
validator, profiles, or registries per turn. The gateway remains read-only and
does not emit events, advance turns, spend resources, apply skills, grant
experience, or modify combat state.

`TurnBasedCombatEngine` accepts `ICombatSkillTurnValidator` through an optional
three-argument constructor overload. The existing two-argument constructor
remains unchanged for callers and delegates with no validator, preserving the
legacy/pass-through behavior and event sequence.

For skill actions, the engine calls the optional validator immediately before
the existing `SkillSeal` check. Allowed validation continues through the
existing skill path without adding events. Blocked validation:

- emits exactly one `ActionBlocked` event with a stable enum-based reason
- ends and consumes the turn through the existing `EndTurn` path
- does not continue into `SkillSeal`
- does not calculate or spend Essence
- does not calculate or spend Resonance
- does not execute the skill
- does not grant skill experience
- does not emit `ResourceNotEnough`

Basic attacks do not invoke combat skill turn validation.

Custom `ICombatSkillTurnValidator` implementations must return a non-null
`CombatSkillTurnValidationResult`. If a custom validator returns null, the
engine fails closed before `SkillSeal` and resource processing. It emits one
`ActionBlocked` event with the stable `NullValidationResult` reason, consumes
the turn through `EndTurn`, and does not spend resources, grant experience,
execute skill effects, or emit `ResourceNotEnough`.

`CombatSkillTurnValidationComposition` owns production construction for the
validation path without moving dependency graphs into the combat engine.

It exposes:

```csharp
ICombatSkillTurnValidator CreateValidator(
    UnlockProfile unlockProfile,
    ClassProfile classProfile,
    MasteryProfile masteryProfile,
    ClassDefinitionRegistry classRegistry,
    SkillDefinitionRegistry skillRegistry);

TurnBasedCombatEngine CreateV4EnabledEngine(
    IRandomSource random,
    CombatPassivePipeline pipeline,
    UnlockProfile unlockProfile,
    ClassProfile classProfile,
    MasteryProfile masteryProfile,
    ClassDefinitionRegistry classRegistry,
    SkillDefinitionRegistry skillRegistry);
```

`CreateValidator` constructs the existing
`CombatSkillTurnValidatorGateway`. `CreateV4EnabledEngine` passes that
validator through the existing optional engine constructor. The composition
helper does not duplicate classification, context creation, validation, or
engine behavior.

Player validation requires all five player dependencies. Missing dependencies
remain configured on the gateway and fail player actions closed through the
existing classifier. Enemy actions still classify and validate without player
progression dependencies. Legacy callers continue constructing
`TurnBasedCombatEngine` with its existing two-argument constructor and remain
pass-through.

## Combat Validation Bootstrap Ownership Audit

The current production-like entry point is `FoundationVerticalSlice`. It owns a
short-lived `PlayerProfile`, combat entities, passive pipeline,
`TurnBasedCombatEngine`, and `CombatRuntimeContext`. It intentionally creates
the engine through the legacy two-argument constructor and remains the V1
event-count compatibility path.

The slice is not yet a safe V4 composition root:

- `PlayerProfile` owns the legacy `PlayerClassCollection`, but not the V4
  `UnlockProfile`, `ClassProfile`, `MasteryProfile`,
  `MentorRelationshipProfile`, or `SkillTreeProgressProfile`
- V4 restore APIs mutate caller-supplied profile instances in place
- `ClassBootstrap` can build class definitions, but there is no complete
  production bootstrap for the skill, mentor, skill-tree, and binding
  registries
- slice combat skills are currently defined locally instead of being resolved
  from one authoritative `SkillDefinitionRegistry`
- creating fresh V4 profiles or registries per combat would disconnect
  validation from restored and mutated progression state

The recommended owner is a future application/session-scoped V4 runtime state,
not `TurnBasedCombatEngine`, `CombatRuntimeContext`, or a static domain
singleton. That owner should:

- create each mutable V4 profile exactly once per loaded player session
- create each definition registry exactly once per application content
  session
- restore snapshots into those same live profile instances before combat
- provide the same profile and registry references to progression services,
  save/snapshot services, and `CreateV4EnabledEngine`
- keep combat entities, turn order, passive pipelines, resonance, and the
  combat engine scoped to an individual combat

`CombatSkillTurnValidationComposition` is sufficient for final engine
assembly, but it does not own or build the dependency graph. Calling it is safe
only after an application owner can supply canonical live profiles and
registries.

Accidental legacy validation remains possible anywhere callers use the
two-argument engine constructor. This is required for V1 compatibility, so the
future V4 application path should centralize engine creation and avoid direct
constructor use without changing the legacy slice.

The earliest safe integration point is the future application-level
per-combat creation method, after session state has been created or restored
and the canonical registries are available. `CreateV4EnabledEngine` should be
called there immediately before constructing the matching
`CombatRuntimeContext`.

Unresolved risks before integration:

- legacy `PlayerClassCollection` and V4 `ClassProfile` can drift unless one
  application owner defines their roles
- independently rebuilt registries can contain different definition sets
- replacing profile instances after gateway construction would leave the
  gateway reading stale state
- locally constructed skill definitions can diverge from the canonical skill
  registry
- no dedicated post-composition combat-validation workflow-verification branch
  exists yet, although focused smoke coverage exists for gateway, engine, and
  composition behavior

Recommended next atomic step:

- add an application-layer V4 runtime-state ownership foundation that holds the
  live mutable profiles and canonical registries without migrating combat call
  sites
- follow it with a dedicated combat-validation workflow-verification iteration
  before wiring any production/bootstrap engine creation

Validation rules:

- the request, skill, progress, and progress definition must exist
- `ICombatSkill.Id` must match `SkillProgress.Definition.Id`
- player skills with a required unlock are checked through
  `SkillAccessPolicy`
- V4 access is checked only when a complete class/mastery registry context is
  supplied
- partial V4 context is rejected explicitly
- enemy mode skips player unlock and class/mastery checks
- enemy requests still require valid skill/progress facts and matching ids

`CombatSkillUseValidationResult` returns stable facts including:

- `IsAllowed`
- `SkillId`
- `IsPlayerAction`
- `V3AccessChecked`
- `V4AccessChecked`
- `CombatSkillUseBlockReason`
- underlying V3 and V4 block reasons when applicable

The validator is read-only. It does not emit events, spend resources, advance
turns, apply skills, grant experience, or modify combat state.
`TurnBasedCombatEngine` consumes validation only through
`ICombatSkillTurnValidator`.

The previous parameter-based validator overload remains compatible. New engine
integration should use the context-based overload so legacy, player, and enemy
behavior cannot be inferred from null parameters.

## Unified V4 Restore Coordinator

`V4ProgressRestoreCoordinator` restores V4 state in this defined order:

1. `ClassProfile`
2. `MasteryProfile`
3. `MentorRelationshipProfile`
4. `SkillTreeProgressProfile`
5. class skill tree bindings

Before mutating target state, the coordinator runs the same individual restore
operations against temporary `ClassProfile`, `MasteryProfile`,
`MentorRelationshipProfile`, and `SkillTreeProgressProfile` instances. Binding
restore feasibility is checked against a temporary
`ClassSkillTreeBindingRegistry`.

Preflight rules:

- class, mastery, mentor, skill tree progress, and binding feasibility are all
  evaluated
- any failed subsystem returns before target mutation begins
- successful preflight applies the existing individual restore operations in
  the documented order
- per-subsystem success facts, failure reasons, and messages remain available
- individual restore methods remain unchanged and independently callable
- snapshot DTOs, JSON shape, and schema version remain unchanged

The binding-aware coordinator overload receives:

- `ClassDefinitionRegistry`
- `SkillDefinitionRegistry`
- `MentorDefinitionRegistry`
- `SkillTreeDefinition`
- `ClassSkillTreeBindingRegistry` containing registered skill tree definitions

Existing coordinator calls remain compatible. The original overload restores
the original four subsystems and treats binding restore as not required. The
binding-aware overload restores bindings through the existing
`V2ProgressSnapshotService` behavior.

`V4ProgressRestoreResult` exposes per-subsystem facts, including:

- `ClassRestored`
- `MasteryRestored`
- `MentorRelationshipsRestored`
- `SkillTreeProgressRestored`
- `ClassSkillTreeBindingsRestored`
- `FullyRestored`

Binding restore also exposes structured failure details:

- `ClassSkillTreeBindingsFailureReason`
- `ClassSkillTreeBindingsFailureMessage`

All subsystems expose stable `V4ProgressRestoreFailureReason` values and
human-readable messages. Any failed subsystem, including binding restore,
causes `FullyRestored` to be false.

## Integrated Workflow Verification

V4 smoke tests include integrated workflow coverage for:

- class + mentor + teaching + mastery
- class/mastery snapshot and restore
- mastered skill retention after class switch
- combat-facing skill access after mentor teaching and class switch
- combat skill use validation across teaching and class switching
- combat skill use validation context modes across class switching
- combat skill use validation context provider across class switching
- combat actor classification through context creation and validation
- combat skill validation orchestration across class switching and failures
- skill tree progression
- class-specific skill tree binding
- class skill tree binding snapshot, JSON round-trip, and restore
- scoped skill tree progress across class switching and JSON restore

The mastered skill retention workflow verifies:

- two skills are registered
- two classes with different allowed skills are registered
- the first class is assigned
- a mentor teaches a first-class skill
- teaching is applied until the skill is mastered
- the profile switches to a second class
- the mastered first-class skill remains usable through
  `MasteredSkillRetentionPolicy`
- a non-mastered first-class skill is not usable after the class switch

The combat-facing skill access workflow verifies:

- two skills and two classes with different allowed skills are registered
- the first class is assigned and its skill is combat-usable
- a mentor teaches that skill to mastery
- the profile switches to the second class
- the mastered first-class skill remains combat-usable
- the non-mastered first-class skill and an unknown skill are blocked
- `CombatSkillAccessService` facts match `MasteredSkillRetentionPolicy`
- no combat execution behavior is changed

The combat skill use validator workflow verifies:

- two skills and two classes are registered
- the first class is assigned
- a first-class skill passes composed request validation
- mentor teaching masters that skill
- the profile switches to the second class
- the mastered retained skill still passes validation
- a non-mastered cross-class skill is blocked
- enemy mode accepts a structurally valid skill without V4 profiles
- mismatched skill and progress ids are blocked
- `TurnBasedCombatEngine` is not invoked or changed

The combat skill use validation context workflow verifies:

- `LegacyBypass` accepts a structurally valid skill without V4 profiles
- `Enemy` accepts a structurally valid skill without player profiles
- `Player` rejects partial context
- `Player` accepts a current-class skill
- `Player` accepts a mastered retained skill after a class switch
- `Player` rejects a non-mastered cross-class skill
- `Player` rejects mismatched skill and progress ids
- `TurnBasedCombatEngine` is not invoked or changed

The combat skill use validation context provider workflow verifies:

- the provider creates explicit legacy bypass and enemy contexts
- both non-player-progression contexts validate structurally valid skills
- incomplete player dependencies are refused without fallback
- complete player dependencies produce a valid player context
- the player context validates a current-class skill
- the same player context observes mastered retention after a class switch
- the same player context blocks a non-mastered cross-class skill
- `TurnBasedCombatEngine` is not invoked or changed

The combat actor classification workflow verifies:

- explicit legacy classification maps to a legacy context accepted by the
  validator
- the runtime enemy maps to an enemy context without player profiles
- a complete runtime player classifies as `Player`
- incomplete player dependencies fail classification safely
- an actor outside the runtime fails classification safely
- the classified player mode requests the matching context from the provider
- the produced player context is accepted by `CombatSkillUseValidator`
- `TurnBasedCombatEngine` is not invoked or changed

The combat skill validation orchestrator workflow verifies:

- legacy and enemy actors complete orchestration successfully
- a player current-class skill completes orchestration successfully
- a mastered retained skill remains valid after a class switch
- a non-mastered cross-class skill fails at the `Validation` stage
- incomplete player dependencies fail at the `Classification` stage
- a foreign actor fails safely at the `Classification` stage
- mismatched skill and progress ids fail at the `Validation` stage
- successful orchestration reports no failure stage
- `TurnBasedCombatEngine` is not invoked or changed

The combat skill turn validator gateway workflow verifies:

- legacy mode allows a structurally valid skill
- enemy mode allows a structurally valid skill without player dependencies
- player mode allows a current-class skill
- player mode allows a mastered retained skill
- player mode blocks a non-mastered cross-class skill
- missing player dependencies return clear classification failure facts
- the engine-facing interface accepts only the turn request and runtime

The combat skill turn validator engine integration verifies:

- the existing two-argument constructor preserves legacy execution
- an explicit null validator produces the same event types
- allowed validation preserves skill execution, costs, and experience
- blocked validation emits exactly one `ActionBlocked`
- blocked validation consumes the turn
- blocked validation spends neither Essence nor Resonance
- blocked validation grants no experience and executes no skill effects
- blocked validation does not emit `ResourceNotEnough`
- validation runs before and preempts `SkillSeal`
- a null custom validator result fails closed with `NullValidationResult`
- null-result blocking preserves turn, resource, experience, and effect rules
- null-result blocking preempts `SkillSeal`

The combat skill turn validation composition verifies:

- the factory creates a non-null `ICombatSkillTurnValidator`
- the explicit V4 engine path allows current-class player skills
- legacy engine construction remains pass-through
- missing player dependencies fail closed
- enemy actions require no player progression dependencies
- mastered retained skills remain allowed
- non-mastered cross-class skills are blocked
- blocked composed validation emits one `ActionBlocked` and consumes the turn
- blocked composed validation spends no Essence or Resonance
- blocked composed validation grants no experience or skill effects

The skill tree workflow verifies:

- skills are registered
- a three-node skill tree is created
- the first node is discovered
- the first node is unlocked
- a dependent node is blocked before its prerequisite unlock
- the prerequisite node unlocks
- the dependent node becomes unlockable
- the dependent node unlocks
- no skills are automatically granted
- `MasteryProfile` remains unchanged

The class skill tree binding workflow verifies:

- two classes are registered
- two stable skill trees are registered
- each class binds to a different skill tree
- the first class is assigned
- the first class skill tree resolves
- the profile switches to the second class
- the second class skill tree resolves
- bindings do not mutate `SkillTreeProgressProfile`
- bindings do not grant skills automatically

The binding save workflow additionally verifies:

- both bindings are captured in the progress snapshot
- bindings survive JSON serialization and deserialization

The scoped skill tree progress workflow verifies:

- two classes bind to different trees containing the same node id
- unlocking the shared node in the first tree does not unlock it in the second
- the shared node can be discovered and unlocked independently after switching
  classes
- both scopes survive snapshot creation and JSON round-trip
- restore preserves shared unlocks and tree-specific discovery facts
- bindings restore into a fresh registry
- restored bindings resolve after assigning and switching classes
- restored bindings do not mutate skill tree progress
- restored bindings do not grant skills automatically

## Boundaries

Allowed:

- pure C# domain models
- registry-backed validation
- smoke-testable facts and services
- snapshot DTOs and restore helpers
- explicit service calls for state mutation

Not included:

- application/bootstrap integration
- changes to skill effect behavior
- automatic skill unlock grants
- mentor rewards
- mentor missions
- dialogue
- UI
- Unity
- generator-wide rewrites

## Smoke Expectations

Current smoke groups:

- V1 Legacy Slice assertions: `2`
- V1 emitted event count: `38`
- Shared Domain Foundations assertions: `16`
- V2 Narrative / Progression / Persistence assertions: `264`
- V3 Combat / Skills / Resonance / Status / Stats assertions: `100`
- V4 Classes / Mentors / Mastery assertions: `388`
- Total smoke assertions executed: `770`

## Validation

Run:

```powershell
dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal
dotnet run --project src\Game.SmokeTests\Game.SmokeTests.csproj --no-build
git diff --check
```
