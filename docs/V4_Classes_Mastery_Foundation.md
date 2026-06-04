# V4 Classes, Mentors, Mastery Foundation

This document describes the current Vertical Slice V4 foundation for classes,
mentors, mastery, mentor teaching, persistence, mastered skill retention, and
workflow verification.

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

`UnlockProfile`, combat execution, UI, Unity presentation, and skill trees remain
separate.

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
class, mastery, mentor teaching, retention, and snapshot validation.

Rules:

- valid skill definitions register by stable id
- null, empty id, and empty name are rejected safely
- duplicate skill ids are rejected safely
- unknown skill ids fail safely when a registry-aware operation is used

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

## Integrated Workflow Verification

V4 smoke tests include integrated workflow coverage for:

- class + mentor + teaching + mastery
- class/mastery snapshot and restore
- mastered skill retention after class switch

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
- class skill trees beyond existing definition facts
- UI
- Unity
- generator-wide rewrites

## Smoke Expectations

Current smoke groups:

- V1 Legacy Slice assertions: `2`
- V1 emitted event count: `38`
- Shared Domain Foundations assertions: `16`
- V2 Narrative / Progression / Persistence assertions: `252`
- V3 Combat / Skills / Resonance / Status / Stats assertions: `100`
- V4 Classes / Mentors / Mastery assertions: `133`
- Total smoke assertions executed: `503`

## Validation

Run:

```powershell
dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal
dotnet run --project src\Game.SmokeTests\Game.SmokeTests.csproj --no-build
git diff --check
```
