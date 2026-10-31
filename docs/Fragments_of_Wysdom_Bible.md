# Fragments of Wysdom — Game Design & Architecture Bible

> **Status:** v2.0 DRAFT — consolidation candidate, not yet approved as sole source of truth.  
> **Repository:** `Drakk3m/Fragments-of-Wisdom`  
> **Rule:** Never infer implementation from a design proposal. Verify code, tests and repository state before marking a feature complete.

## 0. Authority, status and maintenance

This document is intended to become the **single governing design and architecture reference**. Until acceptance, existing living technical documents remain available for traceability.

**Status vocabulary:** `LOCKED DESIGN` (accepted design intent); `IMPLEMENTED — DOCUMENTED` (living technical documentation reports an implementation, pending fresh execution); `PARTIAL` (some foundation exists); `PROPOSED` (not approved for implementation); `DEFERRED` (approved direction, not scheduled); `HISTORICAL` (superseded or contextual); `UNRESOLVED` (requires decision).

**Conflict order:** verified current code and tests establish implementation facts; explicit latest user decisions establish design intent; current living technical docs establish documented architecture; older specs and generators provide history. When these disagree, record the discrepancy instead of silently rewriting either side.

**Project spelling:** **Fragments of Wysdom** is the product title. Repository/solution names using `Wisdom` may remain for compatibility until an explicit rename audit.

## 1. Vision and player experience

**LOCKED DESIGN.** An HD-2D-inspired, strategic RPG about wisdom, cognition, mentorship, memory, mastery, philosophical choice and restoration. The core loop is:

`Explore → Encounter / Fight → Rewards and discoveries → Missions / Codex / Skills / Classes / Cognition → New choices → Explore`

Combat should create discovery, progression or a meaningful next decision, not just bigger numbers. The experience should evoke curiosity, reverence, adventure, melancholy and hope. The world feels like **memories of a civilization that forgot its wisdom**.

**Anti-goals:** mobile gacha, generic fantasy reskin, arbitrary grinding, direct imitation of Octopath or FFVII Remake, and philosophy detached from mechanics.

## 2. World, narrative and cultural research

**LOCKED DESIGN.** Taíno/Caribbean and Mesoamerican visual and cultural references are inspiration for a fictional setting, not interchangeable decorations. Research motifs, architecture, material culture and context before adapting them. Do not turn sacred symbols into generic UI wallpaper. `Nahual` is a **working name**, subject to cultural and narrative review.

**UNRESOLVED:** canonical map, timeline, factions, complete plot, cast, endings, and full Nova biography. Early sample quests, enemies and class names are illustrative unless separately approved.

## 3. Nova, classes, cognition and mastery

**LOCKED DESIGN:** *Classes determine how Nova fights; cognition determines how Nova understands and modifies systems.*

**IMPLEMENTED — DOCUMENTED:** stable class IDs include `Tlamatini` and `Wanderer`; class definitions and registries, skill definitions, skill trees, scoped unlock progress, class-to-tree bindings, mastery profiles, mentor definitions/relationships, teaching evaluation and application, mastered-skill retention across class changes, progression-aware combat validation and runtime/session ownership. See the existing [Player Progression Foundation](PlayerProgression_Foundation.md) for detailed contracts during transition.

Mentor archetypes: **Lyra** (analysis), **Atlas** (stability), **Gepetto** (crafting/system manipulation) and **Spark** (variance/chaos). The latter two are design directions rather than evidence of complete systems.

Teaching concepts: temporary, fixed and equipped; the entire final persistence/equipment-slot UX is not complete. Mentors may require missions to fix teachings permanently. **DEFERRED:** full class roster, Tlamatini tree content, equipment-taught skills, hybrid mentor teachings, cognition UI and expanded mentor systems.

## 4. Combat — canonical current foundation

**IMPLEMENTED — DOCUMENTED:** `TurnBasedCombatEngine` is the **only canonical turn-sequencing authority**. Do not introduce a second `CombatResolver` or parallel action engine. Existing flow:

```text
TurnRequest
  -> TurnBasedCombatEngine
  -> CombatRuntimeContext
  -> CombatContext
  -> DamageResolver
  -> CombatEntity / ResourcePool / ResonanceProfile / GameEventLog
```

The foundation covers basic attacks, skills, hit/evasion, criticals, HP, defeat, resource checks, passive hooks, regeneration and status resolution. Opposed stats: Attack/Defense; Precision/Evasion; Critical Chance/Critical Resistance; Debuff Chance/Debuff Resistance.

**Resonance:** `ActiveResonance` pays for skills; `ResonanceReserve` is stored capacity. A successful refill transfers reserve into active and consumes a turn. Skills never silently consume reserve. Empty reserve and insufficient active resonance must be blocked consistently. Older `Essence`-only combat descriptions are **HISTORICAL**, not permission to create a second resource system.

**Status foundation:** Poison, Burn, SkillSeal, RegenBlock and Regen; advanced stacking, immunity and interactions are **DEFERRED**. Current technical rules and ordering live in [V3 Combat Foundation](V3_Combat_Foundation.md) pending contract-by-contract migration.

### 4.1 Hybrid combat exploration — PROPOSED, not implemented

A recent design conversation proposed keeping **turn-based tactical authority** while moving *basic attacks* from an `Attack` menu to a direct button, with menus reserved for skills, Resonance refill, items, summons and other strategic actions. The intended audience includes fans of action-oriented games such as Ys and tactical RPGs such as Final Fantasy.

**Hypothesis:** direct basic attacks could create an `ACT → BUILD → DECIDE → EXECUTE` rhythm, with slow/pause tactical selection. Classes might have distinct basic-action styles, and enemy tells/counters could reward understanding over button mashing.

**Not yet decided:** whether free movement, dodging, blocking, animation canceling or real-time enemy actions exist; whether basics generate Resonance; how turns are counted; Speed; DOT/Regen timing; menu pause semantics; action frequency; target selection; input/controller support; fairness between motor skill and strategic builds.

**Architecture gate:** do not code or change combat policy before a dedicated `Combat System Redesign v0` compares conservative, hybrid and action-heavy variants, defines a single time/turn authority and event contract, proves no parallel combat engine, and gains explicit design approval. This proposal **does not override** the existing combat foundation.

## 5. Philosophical routes and transmutation

**LOCKED DESIGN:** Shared Wisdom emphasizes mentorship, relationships, knowledge transmitted and community. The alternative **Nahual Path** (working name) explores knowledge extracted or forced from reality; it should be intellectually tempting, dangerous and isolating, **not** a generic evil route.

**PARTIAL — DOCUMENTED:** alignment/progression flags and fact-producing evaluators for teaching acceptance, NPC reactions, vendor restrictions, narrative hooks, mission triggers, Codex candidates and transmutation access. These must not mutate unrelated systems implicitly.

**DEFERRED:** full route content, alternate endings, faction/world consequences and economy balancing. `Transmute Lesser Being` is an early concept: defeated enemies may convert into materials/items with failure, resistance and costs. Wisdom Fragments represent **conceptual knowledge remnants**, not generic collectible crystals. Exact economy is unresolved.

## 6. Missions, narrative and Codex

**IMPLEMENTED — DOCUMENTED:** the V2 narrative foundation favors small evaluators that return facts, tags, hooks and candidate records. Explicit downstream services apply Codex and mission-journal discovery idempotently. No narrative evaluator should directly execute combat, UI, dialogue or reward logic.

**LOCKED DESIGN:** the Codex is where the world reconstructs its memory through Nova. It is more than a wiki: a scholarly, ceremonial, fragmented knowledge instrument. Proposed knowledge domains include world memory, bestiary, classes/masters, skills/teachings, artifacts, regions, factions/philosophies, corrupted knowledge and myths/echoes. Entry stages may progress from unknown to glimpsed, discovered, studied and mastered/reconstructed.

**DEFERRED:** full UI, thresholds, reward bridge, discovery-stage implementation, mission chains, partial objectives and story content. See [V2 Narrative Foundation](V2_Narrative_Foundation.md).

## 7. Town Restoration and the Sanctuary

**LOCKED DESIGN / DEFERRED IMPLEMENTATION:** the **Villa de las Memorias** is a restoration of culture, roots, knowledge and function, **not** a free-placement city builder. Learning/mastering teachings can restore or deepen buildings that eventually host shops, mentors, workshops, cooking, alchemy or transmutation. No town implementation until the visual smoke-demo boundary is designed and audited.

**LOCKED DESIGN / DEFERRED IMPLEMENTATION:** a **Sanctuary / Refuge of Reflection** within the restoration fantasy offers quiet, emotionally meaningful remembrance. An NPC may naturally recall a *small selection of actual significant milestones* from the player's journey. Never fabricate remembered events, turn reflection into XP/achievements, show percentages, or force exhaustive recaps. The Codex remembers the world through Nova; the Sanctuary helps Nova remember the journey. Event inputs, persistence and UI ownership remain **UNRESOLVED**.

## 8. Visual and interface constitution

**LOCKED DESIGN:** HD-2D-inspired pixel characters, layered environments, atmospheric light and depth, with a distinct identity rather than an Octopath copy. Color semantics: jade/turquoise for knowledge, worn gold for mastery, warm stone for history, deep blue for mystery, obsidian/violet for corrupted/forbidden knowledge, restrained red for danger. Shapes: circles and symmetry for order, fractures/broken circles for loss, roots/water for living knowledge.

UI should feel like a ritual/scholarly instrument, not a mobile app. It must remain readable, responsive and controller-friendly. Repeated animations should be brief/skippable. Proposed first visual probes: Codex prototype, combat HUD and environment mood piece. Full UI implementation remains **DEFERRED**.

## 9. Architecture constitution

**LOCKED DESIGN:**
- Pure C# domain and application rules; Unity is an adapter/presentation layer.
- Visual code consumes backend state, results and events; it does not own gameplay rules.
- Combat, progression/cognition, missions, Codex, inventory/equipment, narrative and Unity presentation have explicit boundaries.
- One important public class per file, with narrow exceptions.
- Small, focused, testable services; explicit mutation; no hidden dependencies or parallel engines.
- Preserve Unity `.meta`, GUIDs, scenes, prefabs and serialized references; avoid manual YAML edits.
- Do not regenerate or overwrite the active repository from historical PowerShell snapshots.
- Keep `FoundationVerticalSlice` as a compatibility boundary unless a dedicated branch approves migration.

**IMPLEMENTED — DOCUMENTED:** `LoadedPlayerSession`, session composition/bootstrap and `ProgressionRuntimeDemoBootstrap` establish runtime ownership; registries have appropriate content/session scope; combat objects are per-combat. Full contract details are in [Player Progression Foundation](PlayerProgression_Foundation.md).

## 10. Persistence and compatibility

**IMPLEMENTED — DOCUMENTED:** V2 snapshot DTOs, JSON serializer, file repository, save/restore profiles, Unity save-path adapter, and extensions for class/mentor/mastery/tree facts. Compatibility with existing JSON and stable IDs is mandatory. **DEFERRED:** slots, autosave, cloud and broad persistence migrations.

Version-prefixed persistence APIs have a high rename blast radius. Do not rename them as incidental cleanup. See [V2 Save Foundation](V2_Save_Foundation.md).

## 11. PETTO Mode — engineering workflow

**LOCKED PROCESS:** **Foundation → Workflow Verification → Documentation → Audit → Next Atomic Step.** The audit backlog is the roadmap.

1. Read latest audit/backlog and identify the **earliest unresolved atomic item**.
2. Confirm prior workflow verification and current documentation.
3. Check whether the change would create a parallel system or violate ownership.
4. Plan one purpose per branch; never mix gameplay changes with documentation/tooling cleanup.
5. Implement narrowly; run build, smoke tests, save/restore and compatibility checks where relevant.
6. Record results, update this Bible and audit, then propose the next atomic step.

**Documented guardrails (not freshly rerun here):** legacy slice emits 38 events; combat smoke group has 100 assertions; progression foundation documents 899 total smoke assertions at its latest recorded checkpoint. These are historical documented counts, not proof of the current CI result.

Validation commands documented by the project:

```powershell
dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal
dotnet run --project src\Game.SmokeTests\Game.SmokeTests.csproj --no-build
git diff --check
```

## 12. Current roadmap — subject to repository verification

The [living backlog](TODO_BACKLOG.md) documents:
1. Finish responsibility-based naming cleanup for progress restore and combat-validation diagnostics.
2. Design/audit the visual smoke-demo boundary.
3. Implement only the smallest approved visual adapter consuming existing state/events.
4. Run workflow verification and audit.
5. Revisit Town Restoration only after that boundary is stable.
6. Explore Sanctuary after event/milestone input contracts are designed.
7. Explore hybrid combat **in a separate design-only audit**; do not smuggle it into the current naming/visual track.

**Explicit exclusions from the cleanup branch:** combat policy changes, new rewards/economy, save-format changes, generator-wide rewrite, broad bootstrap migration, Unity asset renames and gameplay refactors.

## 13. Deferred systems and open decisions

**Deferred:** enemy AI expansion; advanced statuses; Atlas prediction; balance simulator; full class/skill content; item-taught skills; mentor hybrids and slots; mission chains and event tracking; Codex UI/completion/rewards; item/equipment registries, affixes, loot, mastery; richer transmutation and alignment consequences; town buildings and Sanctuary implementation.

**Open decisions:** hybrid combat timing/input architecture; final Nahual terminology; full narrative/world bible; equipment/loot economy; Codex completion semantics; Sanctuary event selection and repetition rules.

## 14. Documentation consolidation ledger — draft

| Existing file | Proposed destination | Action before acceptance |
|---|---|---|
| `docs/FOUNDATION_MASTER_SPEC.md` | Chapters 1–6, 9 | Historical ideas migrated; inspect for missed details |
| `docs/V3_Combat_Foundation.md` | Chapter 4 | Retain until all technical contracts migrated |
| `docs/V2_Narrative_Foundation.md` | Chapters 5–6 | Retain until all service boundaries migrated |
| `docs/V2_Save_Foundation.md` | Chapter 10 | Retain until all DTO/compatibility details migrated |
| `docs/PlayerProgression_Foundation.md` | Chapters 3, 9–10 | **High-density:** requires contract-by-contract audit |
| `docs/Foundation_Version_Prefix_Naming_Audit.md` | Chapters 11–12 and historical ledger | Verify completed vs pending rename items |
| `docs/TODO_BACKLOG.md` | Chapters 11–13 | Retain until audit/roadmap migration verified |
| `docs/CODEX_WORKFLOW.md`, `docs/STRUCTURE_NOTES.md` | Chapters 9, 11 | Verify no agent instructions lost |
| `docs/GENERATOR_USAGE.md` | Historical tooling | Check live references before retiring |
| `README.md` | Short project entry point | Rewrite after Bible acceptance |
| `AGENTS.md` | Minimal agent entry point to Bible | **Keep:** agents need discoverable instructions |

**No source documents are deleted in this draft.** The final cleanup requires a traceability checklist, broken-link scan, code/test validation and explicit approval before removal.

## 15. Source traceability and revision policy

The source set is the repository's living docs listed above, earlier visual/Codex concepts and the project's design conversations. The prior consolidated DOCX was a starting point, **not evidence that every old technical contract has been migrated**.

Each new approved design decision should update the relevant chapter and its status; each implementation branch should update the verified state and audit. A mismatch between code and Bible is an **audit finding**, never an excuse for silent code changes.

**Acceptance gate:** confirm complete inventory; review each original file against this Bible; audit code/tests and reference links; approve unresolved design choices; only then designate this file the sole governing document.
