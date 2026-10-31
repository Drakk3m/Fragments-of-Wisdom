# Fragments of Wysdom — Game Design, Creative Vision & Architecture Bible

> **v3.0 — DRAFT for review.** Consolidated from the v2 Bible on `main`, repository technical documents, current source-code inspection and documented design conversations. No gameplay changes.  
> **Interpretation:** the Bible describes *desired experience*, *accepted direction*, *proposed ideas* and *current implementation* separately. Do not mistake a domain class for a finished game feature.  
> **Scope of evidence:** source inspected on `main` (2026-10-30); build, smoke suite, CI and Unity Play Mode were **not run** in this pass.

## Reading guide: two independent status dimensions

Every mechanic has **Design status** (`ACCEPTED`, `PROMISING / TO REFINE`, `OPEN`, `REJECTED`) and **Implementation status** (`CODE FOUNDATION`, `PARTIAL`, `NOT FOUND / NOT VERIFIED`, `CONCEPT ONLY`, `VERIFIED`). `VERIFIED` requires actual execution evidence. A promising conversation idea is **not** a mandate to implement. When a conversation did not settle a detail, record it as `OPEN`. Old version names in code are compatibility identifiers, not proof of active design version.

# Part I — Game Design & Creative Vision

### 1. Vision, player fantasy and emotional pillars

**LOCKED DESIGN.** An HD-2D-inspired, strategic RPG about wisdom, cognition, mentorship, memory, mastery, philosophical choice and restoration. The core loop is:

`Explore → Encounter / Fight → Rewards and discoveries → Missions / Codex / Skills / Classes / Cognition → New choices → Explore`

Combat should create discovery, progression or a meaningful next decision, not just bigger numbers. The experience should evoke curiosity, reverence, adventure, melancholy and hope. The world feels like **memories of a civilization that forgot its wisdom**.

**Anti-goals:** mobile gacha, generic fantasy reskin, arbitrary grinding, direct imitation of Octopath or FFVII Remake, and philosophy detached from mechanics.

### 1.1 Desired player experience

The player should feel that learning changes what Nova can perceive, what Nova can do, whom Nova can trust, and what the world can recover. Combat, exploration, mentorship, Codex and restoration should reinforce one another rather than behave like unrelated minigames.

**Creative north stars:** discovery before checklist completion; knowledge with consequences; mastery earned through practice; community and memory as tangible progress; philosophical ambiguity without simplistic good/evil scores.

### 2. World, culture and narrative identity

**LOCKED DESIGN.** Taíno/Caribbean and Mesoamerican visual and cultural references are inspiration for a fictional setting, not interchangeable decorations. Research motifs, architecture, material culture and context before adapting them. Do not turn sacred symbols into generic UI wallpaper. `Nahual` is a **working name**, subject to cultural and narrative review.

**UNRESOLVED:** canonical map, timeline, factions, complete plot, cast, endings, and full Nova biography. Early sample quests, enemies and class names are illustrative unless separately approved.

### 2.1 Voice, language and naming

**ACCEPTED CREATIVE DIRECTION:** an archaizing English register for names, lore and certain ritual dialogue, intelligible rather than faux-Shakespearean in every menu. `Wysdom` is deliberate archaic styling. Exact localization, orthography and dialogue style guide remain **OPEN**. Preserve existing technical/repository `Wisdom` spellings until a dedicated compatibility decision.

### 2.2 Cultural design guardrail

Taíno/Caribbean petroglyphs, carvings and materials and broader Mesoamerican references require research and differentiation. They are not one culture; avoid mixing sacred iconography carelessly. Fictional symbols should be original, readable and grounded in the setting.

### 3. Art direction, environments and interface

**LOCKED DESIGN:** HD-2D-inspired pixel characters, layered environments, atmospheric light and depth, with a distinct identity rather than an Octopath copy. Color semantics: jade/turquoise for knowledge, worn gold for mastery, warm stone for history, deep blue for mystery, obsidian/violet for corrupted/forbidden knowledge, restrained red for danger. Shapes: circles and symmetry for order, fractures/broken circles for loss, roots/water for living knowledge.

UI should feel like a ritual/scholarly instrument, not a mobile app. It must remain readable, responsive and controller-friendly. Repeated animations should be brief/skippable. Proposed first visual probes: Codex prototype, combat HUD and environment mood piece. Full UI implementation remains **DEFERRED**.

### 3.1 Visual design language — ACCEPTED DIRECTION

- **Rendering:** HD-2D-inspired layering, pixel-art character silhouettes, environmental depth, atmospheric lighting, weather and restrained post-processing; no direct imitation of Octopath assets.
- **Materials:** weathered stone, carved wood, jade, turquoise, obsidian, aged gold, water, roots and organic growth. Ruin and restoration should be visible in the same place over time.
- **Symbols:** petroglyph-inspired glyph grammar, fractured circles, radial geometry, knowledge fragments and subtle luminous pathways; culturally reviewed, not copied from sacred artifacts.
- **Emotional contrast:** ancient monumental spaces vs intimate living villages; uncertainty and melancholy vs warmth and renewal.
- **Interface:** Codex as ceremonial knowledge map, not a flat wiki; tactile readable panels, controller navigation, skippable animation, minimal HUD noise.
- **Practical visual audit:** silhouettes at actual gameplay scale, contrast for accessibility, localization-safe layout, performance on target hardware and Unity serialization stability.

### 3.2 Visual content still OPEN

Canonical character sheets, enemy silhouettes, animation bible, tileset specification, camera rules, screen resolutions, HUD mockups, sound palette, soundtrack direction, accessibility targets and platform targets. None should be presented as already approved assets.

### 4. Combat fantasy, rhythm and tactical economy

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

#### Hybrid combat exploration — PROPOSED, not implemented

A recent design conversation proposed keeping **turn-based tactical authority** while moving *basic attacks* from an `Attack` menu to a direct button, with menus reserved for skills, Resonance refill, items, summons and other strategic actions. The intended audience includes fans of action-oriented games such as Ys and tactical RPGs such as Final Fantasy.

**Hypothesis:** direct basic attacks could create an `ACT → BUILD → DECIDE → EXECUTE` rhythm, with slow/pause tactical selection. Classes might have distinct basic-action styles, and enemy tells/counters could reward understanding over button mashing.

**Not yet decided:** whether free movement, dodging, blocking, animation canceling or real-time enemy actions exist; whether basics generate Resonance; how turns are counted; Speed; DOT/Regen timing; menu pause semantics; action frequency; target selection; input/controller support; fairness between motor skill and strategic builds.

**Architecture gate:** do not code or change combat policy before a dedicated `Combat System Redesign v0` compares conservative, hybrid and action-heavy variants, defines a single time/turn authority and event contract, proves no parallel combat engine, and gains explicit design approval. This proposal **does not override** the existing combat foundation.

### 4.2 Refill as a strategic moment — detailed design record

**ACCEPTED:** active Resonance behaves like MP, reserve tanks provide refill, and refill costs a turn in the current turn-based foundation. Reserve growth *could* derive from character growth, Codex completion and special-skill mastery; exact formulas remain **PROMISING / TO REFINE**.

**PROMISING / TO REFINE:** refill may provide a random buff; an example discussed is “next skill costs less.” A mentor/class skill could unlock `auto-refill` without spending the usual turn. These were explored positively but **have no verified implementation or finalized balance contract**. Decide buff pool, probability, stacking, activation conditions and interaction with the hybrid proposal before coding.

### 4.3 Hybrid combat decision record

Basic attacks on a direct button and tactical menus for skills/items/refill are **PROMISING / TO REFINE**, not approved replacement of the canonical turn engine. A real-time input loop cannot silently create a second authoritative combat system. Compare conservative, hybrid and action-heavy prototypes on timing, enemy fairness, strategic depth and accessibility first.

### 5. Character progression, classes and mentors

**LOCKED DESIGN:** *Classes determine how Nova fights; cognition determines how Nova understands and modifies systems.*

**IMPLEMENTED — DOCUMENTED:** stable class IDs include `Tlamatini` and `Wanderer`; class definitions and registries, skill definitions, skill trees, scoped unlock progress, class-to-tree bindings, mastery profiles, mentor definitions/relationships, teaching evaluation and application, mastered-skill retention across class changes, progression-aware combat validation and runtime/session ownership. See the existing [Player Progression Foundation](PlayerProgression_Foundation.md) for detailed contracts during transition.

Mentor archetypes: **Lyra** (analysis), **Atlas** (stability), **Gepetto** (crafting/system manipulation) and **Spark** (variance/chaos). The latter two are design directions rather than evidence of complete systems.

Teaching concepts: temporary, fixed and equipped; the entire final persistence/equipment-slot UX is not complete. Mentors may require missions to fix teachings permanently. **DEFERRED:** full class roster, Tlamatini tree content, equipment-taught skills, hybrid mentor teachings, cognition UI and expanded mentor systems.

### 5.1 Learning through equipment — PROMISING / TO REFINE

A Final Fantasy IX-like idea: equipment exposes learnable skills; practice/mastery can retain eligible abilities after unequipping and changing classes. Class identity must still matter: not every mastered skill should automatically become usable in every class. The eligibility/retention policies must be explicit. The existing mastery foundation does **not** establish a complete equipment-learning loop.

### 5.2 Mentor commitments and branching

Mentor teachings may begin temporary; a mentor mission or meaningful commitment can “fix” a teaching in Nova. Paths should alter access to skills and world capabilities, not just grant passive numbers. Exact mentors, missions, slot limits and branching consequences are content/design work, not verified completed quests.

### 6. Philosophy, routes, choice and transmutation

**LOCKED DESIGN:** Shared Wisdom emphasizes mentorship, relationships, knowledge transmitted and community. The alternative **Nahual Path** (working name) explores knowledge extracted or forced from reality; it should be intellectually tempting, dangerous and isolating, **not** a generic evil route.

**PARTIAL — DOCUMENTED:** alignment/progression flags and fact-producing evaluators for teaching acceptance, NPC reactions, vendor restrictions, narrative hooks, mission triggers, Codex candidates and transmutation access. These must not mutate unrelated systems implicitly.

**DEFERRED:** full route content, alternate endings, faction/world consequences and economy balancing. `Transmute Lesser Being` is an early concept: defeated enemies may convert into materials/items with failure, resistance and costs. Wisdom Fragments represent **conceptual knowledge remnants**, not generic collectible crystals. Exact economy is unresolved.

### 6.1 Dark route is a design trade-off

The darker path may reduce or remove access to mentor-based permanent skills while opening transmutation/knowledge extraction. This is a **PROMISING DESIGN DIRECTION** that needs explicit economy, reversibility and narrative-cost decisions; do not assert all such restrictions are implemented. The player should understand the allure before facing consequences.

### 7. Codex, discovery and missions

**IMPLEMENTED — DOCUMENTED:** the V2 narrative foundation favors small evaluators that return facts, tags, hooks and candidate records. Explicit downstream services apply Codex and mission-journal discovery idempotently. No narrative evaluator should directly execute combat, UI, dialogue or reward logic.

**LOCKED DESIGN:** the Codex is where the world reconstructs its memory through Nova. It is more than a wiki: a scholarly, ceremonial, fragmented knowledge instrument. Proposed knowledge domains include world memory, bestiary, classes/masters, skills/teachings, artifacts, regions, factions/philosophies, corrupted knowledge and myths/echoes. Entry stages may progress from unknown to glimpsed, discovered, studied and mastered/reconstructed.

**DEFERRED:** full UI, thresholds, reward bridge, discovery-stage implementation, mission chains, partial objectives and story content. See [V2 Narrative Foundation](V2_Narrative_Foundation.md).

### 7.1 Codex visual experience — PROMISING / TO REFINE

The Codex is a **ceremonial map of reconstructed knowledge** with discovery stages, interlinked topics and subtle animated reveals. Its content may later contribute to Resonance reserve growth and town restoration, but those bridges are not verified in code. Quote from the earlier concept: *“The Codex is not where the player stores information. It is where the world remembers itself through the player.”*

### 8. Villa de las Memorias and Sanctuary

**LOCKED DESIGN / DEFERRED IMPLEMENTATION:** the **Villa de las Memorias** is a restoration of culture, roots, knowledge and function, **not** a free-placement city builder. Learning/mastering teachings can restore or deepen buildings that eventually host shops, mentors, workshops, cooking, alchemy or transmutation. No town implementation until the visual smoke-demo boundary is designed and audited.

**LOCKED DESIGN / DEFERRED IMPLEMENTATION:** a **Sanctuary / Refuge of Reflection** within the restoration fantasy offers quiet, emotionally meaningful remembrance. An NPC may naturally recall a *small selection of actual significant milestones* from the player's journey. Never fabricate remembered events, turn reflection into XP/achievements, show percentages, or force exhaustive recaps. The Codex remembers the world through Nova; the Sanctuary helps Nova remember the journey. Event inputs, persistence and UI ownership remain **UNRESOLVED**.

### 8.1 Restoration, not city construction

Buildings are manifestations of recovered cultural capabilities, not free-placement structures. A mastered skill may restore a workshop, mentor house, kitchen or alchemical/transmutation function. **OPEN:** whether one skill maps to one building, a building upgrade or a shared capability family; how quest gates and visual stages work.

### 8.2 Sanctuary as emotional counterweight

**ACCEPTED DIRECTION / DEFERRED:** it recalls a few actual, meaningful player events in natural dialogue. No XP, score, trophy wall, percentages or moral evaluation. **Design maxim:** *“The Sanctuary never evaluates the player. It remembers them.”* It needs a stable milestone/event model before any UI.

### 8.3 Audio, pacing and exploration — OPEN

Music, environmental audio, encounter pacing, exploration layout, travel systems, encounter presentation and interaction affordances require their own design pass. Do not invent decisions from an absence of records.

# Part II — Technical Architecture & Implementation

### 9. Domain-first architecture and ownership

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

### 10. Persistence, save compatibility and adapters

**IMPLEMENTED — DOCUMENTED:** V2 snapshot DTOs, JSON serializer, file repository, save/restore profiles, Unity save-path adapter, and extensions for class/mentor/mastery/tree facts. Compatibility with existing JSON and stable IDs is mandatory. **DEFERRED:** slots, autosave, cloud and broad persistence migrations.

Version-prefixed persistence APIs have a high rename blast radius. Do not rename them as incidental cleanup. See [V2 Save Foundation](V2_Save_Foundation.md).

## 11. Implementation ledger — source-inspected, not runtime-verified

**Status semantics:** `FOUNDATION` = relevant source and contracts found; `PARTIAL` = limited domain support, not end-to-end gameplay; `CONCEPTUAL` = creative direction without verified gameplay implementation; `UNVERIFIED` = insufficient evidence; `VERIFIED` = executed and observed (none newly certified here).

| Area | Design maturity | Code status as of this inspection | Concrete source evidence | Missing or unverified |
|---|---|---|---|---|
| Turn-based combat | Accepted current foundation | **FOUNDATION** | `src/Game/Domain/Combat/TurnBasedCombatEngine.cs`, `src/Game/Slice/FoundationVerticalSlice.cs` | Interactive Unity battle, fresh build/test results |
| Resonance active/reserve | Accepted | **FOUNDATION** | `src/Game/Domain/Combat/Resources/ResonanceProfile.cs`, `ResonanceRefillAction.cs` | Balance, UX, auto-refill, buff effects |
| Refill buffs / auto-refill | Promising | **CONCEPTUAL** | No specific implementation verified | Rules, probability, mentorship integration |
| Hybrid basic attacks | Proposed | **CONCEPTUAL** | Existing engine remains turn-based | Time/turn authority, input, enemies, prototype approval |
| Skill validation | Accepted | **FOUNDATION** | `src/Game/Domain/Combat/CombatSkillUseValidator.cs` | Full content and end-to-end player feedback |
| Classes, skill trees, mastery | Accepted | **FOUNDATION / PARTIAL CONTENT** | `src/Game/Domain/Classes/SkillTreeProgressProfile.cs`, unlock policy/application services; progression smoke tests | Complete roster, UI, balancing |
| Equipment-based skill learning | Promising | **PARTIAL / NOT VERIFIED** | `src/Game/Domain/Equipment/EquipmentDefinition.cs` exists; mastery/retention contracts documented | No verified full equip→learn→master→retain gameplay |
| Mentors and teaching | Accepted foundation | **FOUNDATION / PARTIAL CONTENT** | `src/Game/Domain/Classes/MentorTeachingService.cs` | Quests, full narrative dialogue, final teaching UX |
| Session/bootstrap ownership | Accepted | **FOUNDATION** | `src/Game/Application/Bootstrap/PlayerSessionBootstrap.cs`, `ProgressionRuntimeDemoBootstrap.cs` | Production Unity game-loop integration |
| Narrative and Nahual gate | Accepted foundation | **PARTIAL** | `src/Game/Domain/Transmutation/TransmutationAccessPolicy.cs`, V2 narrative source | Full route consequences and transmutation effects |
| Codex and mission discovery | Accepted foundation | **FOUNDATION / PARTIAL** | `src/Game/Domain/Codex/CodexDiscoveryService.cs`, `DiscoveryOrchestrationService.cs` | Codex UI, full quest system, completion stages |
| Snapshot JSON/save | Accepted foundation | **FOUNDATION** | `src/Game/Domain/Progress/V2ProgressSnapshotJsonSerializer.cs`, file repository, `src/Game.Unity/Persistence/UnityV2SaveService.cs` | Unity Play Mode, save slots, autosave, compatibility run |
| Legacy vertical slice | Historical compatibility | **FOUNDATION** | `FoundationVerticalSlice.Run()`, `src/Game.SmokeTests/SmokeTestRunner.cs` | 38-event count not freshly rerun |
| Villa de las Memorias | Accepted direction | **CONCEPTUAL / DEFERRED** | Backlog references, no dedicated restoration engine verified | Building state, unlocks, persistence, visuals |
| Sanctuary | Accepted direction | **CONCEPTUAL / DEFERRED** | No dedicated Sanctuary class located in code search | Milestone facts, reflection service, UI |
| Full playable game | Vision | **UNVERIFIED / NOT COMPLETE** | Domain foundations and console smoke runner | Playable production loop, content, art/audio, release readiness |

**Audit limitations:** This ledger uses current file fetches and code search, not a recursive repository tree, executed tests, or Unity. A missing search hit is not proof of absence. All `FOUNDATION` claims must be upgraded through workflow verification with file/test evidence. Documented smoke guardrails: **38 legacy events, 100 combat assertions, 899 total assertions at the latest progression documentation checkpoint**; not fresh test results.

## 12. Architecture acceptance and validation gates

- Pure C# domain/application owns gameplay facts and decisions; Unity displays results/events and adapts input/storage.
- Do not add parallel combat, progression, mission or save engines.
- For every implementation claim, record **source path + test path + executed result + known limits**.
- Run `dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal`, `dotnet run --project src/Game.SmokeTests/Game.SmokeTests.csproj --no-build`, `git diff --check`; check Unity adapters manually in Unity.
- Preserve serialization IDs, `.meta` files and V1 slice compatibility; never treat an old generator as current authority.

# Part III — Decisions, Roadmap & Development Governance

### 13. PETTO Mode and audit-driven development

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

### 14. Current audited track

The [living backlog](TODO_BACKLOG.md) documents:
1. Finish responsibility-based naming cleanup for progress restore and combat-validation diagnostics.
2. Design/audit the visual smoke-demo boundary.
3. Implement only the smallest approved visual adapter consuming existing state/events.
4. Run workflow verification and audit.
5. Revisit Town Restoration only after that boundary is stable.
6. Explore Sanctuary after event/milestone input contracts are designed.
7. Explore hybrid combat **in a separate design-only audit**; do not smuggle it into the current naming/visual track.

**Explicit exclusions from the cleanup branch:** combat policy changes, new rewards/economy, save-format changes, generator-wide rewrite, broad bootstrap migration, Unity asset renames and gameplay refactors.

### 15. Deferred and open systems

**Deferred:** enemy AI expansion; advanced statuses; Atlas prediction; balance simulator; full class/skill content; item-taught skills; mentor hybrids and slots; mission chains and event tracking; Codex UI/completion/rewards; item/equipment registries, affixes, loot, mastery; richer transmutation and alignment consequences; town buildings and Sanctuary implementation.

**Open decisions:** hybrid combat timing/input architecture; final Nahual terminology; full narrative/world bible; equipment/loot economy; Codex completion semantics; Sanctuary event selection and repetition rules.

## 16. Decision register — conversations vs code

| Decision / idea | Design classification | Implementation classification | Required next evidence |
|---|---|---|---|
| HD-2D with Taíno/Caribbean motifs and unique visual language | **ACCEPTED DIRECTION** | **CONCEPTUAL VISUAL IDENTITY** | Approved art references, in-engine prototype |
| Strategic Resonance refill consumes a turn | **ACCEPTED** | **FOUNDATION** | Smoke execution and UI proof |
| Refill buff “next skill cheaper” | **PROMISING / TO REFINE** | **CONCEPTUAL** | Written balance rules and tests |
| Auto-refill via mentor/class mastery | **PROMISING / TO REFINE** | **CONCEPTUAL** | Unlock, cost and timing contract |
| Hybrid combat | **PROPOSED** | **CONCEPTUAL** | Dedicated design audit |
| Equipment mastery like FFIX | **PROMISING / TO REFINE** | **PARTIAL FOUNDATION** | Equip/learn/master/retain E2E verification |
| Codex ceremonial knowledge map | **ACCEPTED DIRECTION** | **CONCEPTUAL UI** | UI spec, state model and prototype |
| Villa restoration from knowledge | **ACCEPTED DIRECTION** | **DEFERRED** | Visual smoke boundary, restoration model |
| Sanctuary reflects real milestones | **ACCEPTED DIRECTION** | **DEFERRED** | Milestone fact model and narrative audit |
| Nahual trade-off: lost mentors vs transmutation | **PROMISING / TO REFINE** | **PARTIAL POLICY ONLY** | Consequence design and execution |

### 17. Documentation consolidation ledger

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

### 18. Source provenance and change policy

The source set is the repository's living docs listed above, earlier visual/Codex concepts and the project's design conversations. The prior consolidated DOCX was a starting point, **not evidence that every old technical contract has been migrated**.

Each new approved design decision should update the relevant chapter and its status; each implementation branch should update the verified state and audit. A mismatch between code and Bible is an **audit finding**, never an excuse for silent code changes.

**Acceptance gate:** confirm complete inventory; review each original file against this Bible; audit code/tests and reference links; approve unresolved design choices; only then designate this file the sole governing document.

### 18.1 Provenance classification

- **CODE INSPECTED:** concrete source paths in Chapter 11, from `main` at the time of drafting.
- **REPOSITORY DOCUMENTED:** `docs/TODO_BACKLOG.md`, `docs/PlayerProgression_Foundation.md`, V2 narrative/save and V3 combat foundations.
- **CONVERSATION DESIGN:** visual language, Codex concept, Resonance buff/auto-refill, equipment-learning fantasy, town, Sanctuary, philosophical routes and hybrid-combat exploration. Their status reflects what was discussed, **not an invented implementation claim**.
- **OPEN / NOT VERIFIED:** design decisions or technical facts without enough evidence.

### 18.2 Next documentation audit

1. Compare every section of the dense progression foundation against Chapters 5, 9–12.
2. Cross-check live tests and Unity adapter integration, record exact results.
3. Recover any additional approved concepts from older visual, Codex and vertical-slice documents; mark unverified details `OPEN`.
4. Update `README.md` and `AGENTS.md` only after the new chapter structure is accepted.
5. Retire old documents **only after** a reference scan and user review; never as part of gameplay changes.
