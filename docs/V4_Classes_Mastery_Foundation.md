# V4 Classes Mastery Foundation

This document describes the first Vertical Slice V4 foundation step for classes, mentors, and mastery.

The current scope is intentionally small: class identity, current class state, class changes, and class-specific skill access facts.

## Current Flow

```text
ClassDefinition
  -> ClassDefinitionRegistry
  -> ClassProfile current class id
  -> registry lookup
  -> allowed skill id facts
```

`UnlockProfile` remains separate. Changing class does not remove learned or unlocked skills.

## Current Types

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

`ClassDefinitionRegistry` owns class definition lookup:

- `Register(classDefinition)`
- `Contains(classId)`
- `Get(classId)`
- `ResolveCurrentClass(profile)`

`ClassProfile` owns only current class id state:

- `CurrentClassId`
- `AssignClass(classDefinition)`
- `ChangeClass(classDefinition)`
- `AssignClassId(classId, registry)`
- `ChangeClassId(classId, registry)`
- `ResolveCurrentClass(registry)`
- `CurrentClassAllowsSkill(skillId, registry)`

`ClassBootstrap` still provides the current Tlamatini definition used by the V1 slice.

## Snapshot Boundary

Class profile persistence extends the existing progress snapshot in a backward-compatible way.

Rules:

- Snapshot stores only `CurrentClassId`.
- Snapshot does not store `ClassDefinition` objects.
- Restore validates the stored id through `ClassDefinitionRegistry`.
- Known class ids restore into `ClassProfile.CurrentClassId`.
- Unknown class ids fail safely and preserve existing class profile state.
- Older JSON without class profile data restores safely and leaves class state unchanged.
- Schema version remains unchanged because missing class data is treated as an empty optional section.

## Rules

- A valid class can be assigned.
- A valid class can replace the current class.
- `ClassProfile` stores only the stable current class id.
- Class definitions are resolved through `ClassDefinitionRegistry`.
- Unknown class ids are rejected safely.
- Null classes are rejected safely.
- Classes with empty ids or names are rejected safely.
- Rejected class changes preserve the current class.
- Class definitions expose allowed skill id facts.
- Class changes do not mutate `UnlockProfile`.
- Learned or unlocked skills remain outside `ClassProfile`.

## Boundaries

Allowed:

- pure C# domain models
- smoke-testable class assignment and class change behavior
- class-specific skill access facts

Not included yet:

- mastery
- mentors
- class trees
- equipment restrictions
- save changes
- UI
- Unity
- combat rewrites
- automatic skill grants or removals

## Smoke Expectations

Current smoke groups:

- V1 Legacy Slice assertions: `2`
- Shared Domain Foundations assertions: `16`
- V2 Narrative / Progression / Persistence assertions: `239`
- V3 Combat / Skills / Resonance / Status / Stats assertions: `100`
- V4 Classes / Mentors / Mastery assertions: `23`
- Total smoke assertions executed: `380`

The V1 slice emitted event count remains `38`.

## Validation

Run:

```powershell
dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal
dotnet run --project src\Game.SmokeTests\Game.SmokeTests.csproj --no-build
git diff --check
```
