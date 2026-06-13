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

## Unified V4 Restore Coordinator

`V4ProgressRestoreCoordinator` restores V4 state in this defined order:

1. `ClassProfile`
2. `MasteryProfile`
3. `MentorRelationshipProfile`
4. `SkillTreeProgressProfile`
5. class skill tree bindings

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

- combat integration
- skill execution changes
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
- V2 Narrative / Progression / Persistence assertions: `260`
- V3 Combat / Skills / Resonance / Status / Stats assertions: `100`
- V4 Classes / Mentors / Mastery assertions: `256`
- Total smoke assertions executed: `634`

## Validation

Run:

```powershell
dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal
dotnet run --project src\Game.SmokeTests\Game.SmokeTests.csproj --no-build
git diff --check
```
