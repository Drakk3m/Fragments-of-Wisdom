# Codex Workflow

Use Codex as an incremental engineering partner for Fragments of Wysdom, not as a bulk generator.

## Safe Request Pattern

1. Ask Codex to inspect the repo first.
2. Ask for a short plan before code changes.
3. Keep each task narrow: one system, one bug, one refactor, or one document.
4. Prefer small branches and small commits.
5. Ask Codex to summarize every changed file before you commit.

## Good Tasks

- Add project scaffolding or tests.
- Implement one small Vertical Slice V2 foundation piece.
- Add a focused domain service.
- Extract a small class from an oversized file.
- Review Unity asset impact before touching scenes or prefabs.

## Risky Tasks

- Regenerate the whole project over existing source.
- Rewrite combat, progression, or mission systems in one pass.
- Rename Unity assets without preserving `.meta` files.
- Edit scenes, prefabs, or ScriptableObjects without manual Unity validation.
- Mix gameplay behavior changes with tooling or documentation changes.

## After Codex Changes

Review:

- `git status`
- The diff for every changed file
- Any new or modified Unity `.meta` files
- Any scene, prefab, or ScriptableObject reference changes
- The test/build output

Then manually validate in Unity when assets are involved:

- Open the changed scene.
- Check the Console for errors.
- Inspect changed prefabs and ScriptableObjects.
- Enter Play Mode for the affected flow.
- Confirm serialized references are intact.
