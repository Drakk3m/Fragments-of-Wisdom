# AGENTS.md

## Project Identity

Fragments of Wysdom is an HD-2D inspired, turn-based RPG about combat, cognition, mentorship, memory, and philosophical progression.

Current priority is Vertical Slice V2 foundation. Do not expand into full-game implementation unless the user explicitly asks for that scope.

## Coding Rules

- Keep the architecture modular and domain-first.
- Prefer small, focused classes with clear ownership.
- Use one important public class per file.
- Do not create giant MonoBehaviours.
- Do not move gameplay rules into Unity presentation scripts.
- Keep gameplay behavior testable outside Unity where possible.
- Avoid large rewrites unless a smaller incremental change cannot safely solve the problem.

## Unity Rules

- Preserve `.meta` files.
- Preserve existing scenes, prefabs, ScriptableObjects, and serialized references.
- Do not rename or move Unity assets casually; asset GUID stability matters.
- Do not edit generated Unity YAML by hand unless the user specifically asks and the change is narrowly understood.
- When Unity validation is needed, describe the manual editor steps clearly after coding.

## Git Rules

- Work in small branches.
- Make small commits with one clear purpose.
- Do not mix gameplay changes with project organization, documentation, or tooling changes.
- Before committing, inspect `git status` and summarize the exact scope.
- Never revert user changes unless the user explicitly asks.

## Architecture Goals

These systems must remain separated:

- Combat
- Cognition and progression
- Missions
- Codex / encyclopedia
- Inventory and equipment
- Narrative systems
- Unity presentation and adapters

Systems may communicate through explicit services, data models, events, and adapters. Avoid hidden dependencies between gameplay domains.

## Before Coding

Before making code changes:

1. Inspect the repository structure.
2. Check the current git branch and working tree status.
3. Identify the relevant systems and files.
4. Produce a short plan.
5. Keep the plan incremental and compatible with Vertical Slice V2 foundation.

## After Coding

After making changes, summarize:

- Every changed file and why it changed.
- Any behavior risks or architectural risks.
- Tests or checks that were run.
- Manual Unity validation steps, especially for scenes, prefabs, ScriptableObjects, serialized references, and `.meta` files.

## Workflow Verification Rule

After completing any significant mechanic:

- Do not continue immediately with the next feature.
- Create a verification iteration.
- Validate:
  - Build.
  - Smoke tests.
  - Assertion counts.
  - Save/restore when state exists.
  - Compatibility with older snapshots.
  - Absence of parallel systems.
  - Respect for architectural boundaries.
  - Documentation updates.
  - At least one integrated E2E flow.

Suggested branch format:

`codex/<mechanic>-workflow-verification`

Goal:

Detect technical debt and integration problems early before continuing to expand the backend.
