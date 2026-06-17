# TODO Backlog

This is the living backlog for the current foundation track. It should reflect what is true now, not an early wishlist.

## Guardrails

- Preserve the V1 legacy slice guardrail: `38` emitted events.
- Preserve the V3 combat foundation guardrail: `100` assertions.
- Keep foundation work atomic: one branch, one purpose.
- Prefer audit/design slices before broad integration.
- Keep gameplay rules in domain/application code, not Unity presentation code.
- Do not migrate or replace `FoundationVerticalSlice` unless a dedicated branch explicitly approves that scope.
- Do not start visual work until active version-prefixed API names that block the visual boundary are neutralized.
- Visual layers may consume backend state, results, and events; visual layers must not own gameplay rules.
- Every feature branch must update this backlog when it changes the project state.

## Completed / Closed Foundations

- V1 legacy slice remains the compatibility baseline with exactly `38` emitted events.
- V3 combat foundation remains stable with exactly `100` assertions.
- Classes, mastery, mentors, and teaching foundations are implemented and covered by smoke assertions.
- Skill retention is implemented for mastered retained skills.
- Skill trees and scoped skill tree progress are implemented.
- Class-skill tree binding is implemented.
- Progression restore foundation exists through the current restore coordinator/result/failure types.
- Combat validation foundation exists through the skill validator, gateway, optional engine integration, and composition path.
- Progression-aware combat creation exists through the neutral application combat factory path.
- `LoadedPlayerSession` owns one runtime state and creates progression-aware combat engines.
- `PlayerSessionComposition` owns one loaded session and exposes stable runtime state and combat creation.
- `PlayerSessionBootstrap` provides the narrow session bootstrap boundary.
- `ProgressionRuntimeDemoBootstrap` provides the current runtime demo bootstrap path.
- The active runtime/demo bootstrap API rename from `V5RuntimeDemoBootstrap` to `ProgressionRuntimeDemoBootstrap` is complete.
- Runtime/session/bootstrap/demo smoke coverage now lives in `ProgressionRuntimeBootstrapSmokeTests`.
- Active progression foundation documentation now lives in `PlayerProgression_Foundation.md`.

## Current Atomic Track

The current track is naming cleanup before visual smoke demo design.

- Rename progress restore APIs currently prefixed with `V4`.
- Rename combat validation diagnostics currently prefixed with `V3` / `V4`.
- Defer V2 persistence DTO/API renames because the blast radius is high and should be isolated from visual-demo boundary cleanup.

## Immediate Next Branches

1. `codex/rename-progress-restore-api`
   - Rename `V4ProgressRestoreCoordinator`, `V4ProgressRestoreResult`, and `V4ProgressRestoreFailureReason`.
   - Preserve restore semantics.

2. `codex/rename-combat-validation-diagnostics`
   - Rename active `V3` / `V4` combat validation diagnostic facts to unlock/progression terms.
   - Preserve event and validation behavior.

3. `codex/visual-smoke-demo-design-audit`
   - Start only after the visual boundary no longer depends on active V5-named bootstrap APIs.

## Naming Cleanup Backlog

- Completed: `V5RuntimeDemoBootstrap` -> `ProgressionRuntimeDemoBootstrap`.
- Completed: `V4SmokeTests` -> `PlayerProgressionFoundationSmokeTests`.
- Completed: `RunV4SmokeTests` -> `RunPlayerProgressionFoundationSmokeTests`.
- Completed: active V4/V5 smoke runner and assertion group labels -> responsibility-based progression/session/bootstrap labels.
- Completed: runtime/session/bootstrap/demo smoke helpers split into `ProgressionRuntimeBootstrapSmokeTests`.
- Completed: `docs/V4_Classes_Mastery_Foundation.md` -> `docs/PlayerProgression_Foundation.md`.
- `V4ProgressRestoreCoordinator` -> `ProgressRestoreCoordinator` or `PlayerProgressionRestoreCoordinator`.
- `V4ProgressRestoreResult` -> `ProgressRestoreResult` or `PlayerProgressionRestoreResult`.
- `V4ProgressRestoreFailureReason` -> `ProgressRestoreFailureReason` or `PlayerProgressionRestoreFailureReason`.
- `V3AccessChecked` / `V4AccessChecked` -> unlock/progression access terms.
- `V3BlockReason` / `V4BlockReason` -> unlock/progression block reason terms.
- `V3UnlockProfileMissing`, `V3SkillLocked`, `V4ContextIncomplete`, `V4SkillBlocked` -> responsibility-based validation facts.
- Defer broad V2 persistence renames:
  - `IV2SavePathProvider`
  - `UnityV2SavePathProvider`
  - `UnityV2SaveService`
  - `V2ProgressSnapshot*`
  - `V2*Snapshot` DTOs
- Historical references may keep version prefixes when they are branch names, migration notes, audit history, schema compatibility notes, or explicit compatibility guardrails.

## Visual Smoke Demo Backlog

- Run a visual smoke demo design audit before implementation.
- Define the smallest visual boundary that consumes existing backend state, results, and events.
- Use `ProgressionRuntimeDemoBootstrap` as the backend entry point.
- Keep combat, progression, validation, restore, and session ownership rules in backend code.
- Visual layer must not own game rules.
- Visual layer must not construct `TurnBasedCombatEngine` directly.
- Visual layer must not create duplicate runtime state, loaded sessions, or mutable binding registries.
- Add a visual smoke demo foundation only after the design audit approves the boundary.
- Add workflow verification after the visual smoke demo foundation.

## Town Restoration Backlog

Town Restoration is deferred until after the visual smoke demo boundary is designed.

- Town Restoration is not a free placement builder.
- It represents restoring the protagonist's village, roots, culture, and knowledge.
- Buildings appear or are restored when skills and teachings are learned.
- Buildings level up through skill mastery.
- Building functions unlock later, after the restoration foundation is safe.
- No Town Restoration implementation should begin before the visual smoke demo boundary is designed.
- Future Town Restoration work should start with an audit/design branch, not implementation.

## Deferred Gameplay Systems

- Smarter enemy AI.
- Status stacking rules beyond the current foundation.
- Status immunity windows.
- Poison and Burn synergy.
- Atlas prediction API.
- Balance simulator.
- Full Tlamatini tree.
- Additional Mesoamerican-inspired classes.
- Item-taught skills.
- Cognition UI model.
- Active/fixed/equipped teaching persistence beyond the current foundation.
- Lyra and Atlas hybrid skill.
- Gepetto and Spark expansion after validation.
- Teaching slot upgrades.
- Partial objective progress.
- Event-based mission tracking.
- Mission chains and prerequisites.
- Repeatable bounties.
- Story mission examples.
- Codex completion thresholds.
- Codex reward bridge.
- Codex discovery stages.
- Codex UI models.
- Item registry.
- Equipment registry.
- Affixes.
- Loot tables.
- Equipment mastery.

## Technical Debt

- Active production/test/doc names still carry V1/V2/V3/V4/V5 prefixes.
- V2 persistence DTO/API names have high rename blast radius and need a dedicated persistence cleanup branch.
- Current V4/V5 progression assertions are still grouped under the old V4 smoke suite.
- Living progression docs still include active V4 framing.
- Restore API naming still exposes V4 despite being part of the active progression foundation.
- Combat validation diagnostics still expose V3/V4 terminology.
- Snapshot generator profiles and scripts should remain historical unless they become part of the active demo path.
- Backlog updates must become part of normal branch closure to avoid stale planning docs.

## Explicitly Not Now

- Do not perform visual implementation in naming cleanup branches.
- Do not implement Town Restoration before the visual smoke demo boundary is designed.
- Do not perform broad bootstrap migration.
- Do not replace `FoundationVerticalSlice`.
- Do not change save format.
- Do not change combat policy.
- Do not add rewards, unlock bonuses, progression bonuses, or economy changes.
- Do not start new gameplay mechanics while the active naming cleanup track is open.
- Do not do broad refactors under the cover of naming cleanup.
- Do not rename V2 persistence DTO/API names in the first visual-boundary cleanup branch.

## Definition Of Done Update

Every future feature branch must update `TODO_BACKLOG.md` when it:

- completes an item
- defers an item
- creates a new backlog item
- changes priority
- renames an active system
- closes an audit milestone

Backlog updates should stay factual and scoped to the branch. If a branch changes architecture direction, the backlog must say what changed, what moved next, and what is explicitly not being done yet.
