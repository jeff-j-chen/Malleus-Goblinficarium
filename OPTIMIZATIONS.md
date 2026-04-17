# enemy ai optimization draft

this file tracks concrete ideas for reducing enemy ai recalculation cost without changing the behavior specified in ENEMY_AI.md.

all optimizations here assume the decision tree stays identical unless explicitly marked as a design change.

---

## current baseline

recent fixes already landed:
- advanced-plan caching keyed by a full combat-state hash
- target recomputation suppressed while `ApplyPlan()` is mutating the board
- extra post-plan enemy debug refresh removed from `RunEnemyCalculations()`
- profiler summary now reports cache effectiveness
- debug text refresh is now separated from immediate target recomputation
- real combat-state changes now use an explicit deferred refresh path via `SetCombatDebugInformationFor()`
- advanced planning now snapshots board facts once per build instead of rereading live state per candidate
- advanced planning now has a zero-resource fast path when only target choice can vary
- profiler logging is now sampled so development logs do not flood every plan build
- resume-from-save dice restoration now batches enemy-plan refreshes until all saved dice are placed
- fresh level setup now refuses to plan until at least one live die exists, preventing trader-transition pre-spends

observed timings after those fixes:
- cold advanced build: `20.586ms` with `yellow=1` and `candidates=32`
- later rebuild: `3.475ms` with cache reuse beginning to help
- heavier case: `11.360ms` with `yellow=4`, `candidates=96`, `cache=3/6`

current read:
- the previous recursion loop was real and the new cache helps
- remaining stalls are now split between two buckets:
  1. planner work on true cache misses
  2. planner frequency when many board mutations happen back-to-back

memory note:
- there is no unbounded hash table here
- the advanced cache is a single-entry cache: one `int` key plus one cached `Plan`
- temporary `HashCode` values and per-build scratch objects are reclaimed normally after each plan build
- long-session growth is therefore not expected from "more hashes accumulating"
- the only values that grow for the whole session are profiler counters like total runs / hits / misses, which are constant-space numeric fields

---

## implementation status

### done now

- idea 1 is now implemented in a safe first pass
- `StatSummoner.SetDebugInformationFor()` is pure ui/debug work again
- combat mutations that genuinely affect planning now opt into deferred replanning through `SetCombatDebugInformationFor()`
- this removes the old hidden `TargetBest()` side effect from debug-only refreshes such as stat display updates and item-preview flows
- advanced-plan candidates now reuse a build-local snapshot of player stats, enemy base stats, dice summaries, and item flags
- advanced mode now skips the full yellow/stamina search entirely when there are no yellow dice and no usable stamina allocation to explore
- development profiling now keeps `LastAdvancedPlanProfileSummary` current but only logs sampled or slow builds
- resume loading now suppresses per-die replans and flushes one deferred refresh after the saved dice batch completes
- fresh fights now skip enemy planning entirely until round dice exist, so menu saves cannot capture bogus pre-pick stamina commitments

### still worth doing later

- wider adoption of explicit combat refresh calls in more mutation sites if profiling still shows avoidable rebuilds
- stronger batching/dirty-flag control for bursts that span multiple frames
- planner-body reductions once refresh frequency is low enough that cache misses dominate

---

## newly drafted ideas

### 16) keep the cache strictly bounded

**idea**
- keep advanced-plan caching single-entry or otherwise explicitly capped
- never let plan caching turn into an unbounded dictionary of old combat states

**why it should help**
- this avoids the exact long-session memory worry about hashes accumulating for the whole run

**validity**: very high  
**timesave**: low  
**risk**: low

**status**
- already true in the current implementation

---

### 17) build-local planner snapshot

**idea**
- flatten the current board into a compact snapshot once per advanced-plan build
- evaluate candidates from that snapshot instead of rereading live stats, dice totals, wounds, and item flags every time

**why it should help**
- repeated `SumOfStat()` calls, repeated fixed-die scans, and repeated item/wound checks were all pure setup overhead inside hot loops

**validity**: very high  
**timesave**: medium  
**risk**: low to medium

**status**
- implemented

---

### 18) zero-resource advanced fast path

**idea**
- if advanced mode has no yellow dice and no usable stamina split to enumerate, skip directly to target comparison only

**why it should help**
- the generic search scaffolding is wasted in cases where only target choice can change the outcome

**validity**: very high  
**timesave**: medium  
**risk**: low

**status**
- implemented

---

### 19) sampled profiler logging

**idea**
- keep profiler summaries accurate every run, but only emit logs on sampled runs or slow builds

**why it should help**
- editor logging itself can become part of the hitch once plan refreshes are frequent

**validity**: high  
**timesave**: low to medium in editor  
**risk**: low

**status**
- implemented

---

## ranking key

- validity: how likely the idea is to help in this codebase without changing ai behavior
- timesave: rough savings if implemented well
- risk: chance of bugs, behavior drift, or maintenance cost

scale:
- very high
- high
- medium
- low

---

## brainstormed ideas

### 1) separate debug text refresh from target/planner refresh

**status**
- implemented in first-pass form
- debug refresh no longer directly calls `TargetBest()`
- real combat updates now use explicit deferred refresh calls instead of hidden synchronous replans

**idea**
- stop calling `s.enemy.TargetBest()` from every `SetDebugInformationFor()` call
- split the method into:
  - debug text / layout update only
  - enemy intent refresh only when combat state is actually dirty

**why it should help**
- current stack traces still show planner work coming from debug refresh paths
- debug text changes are cheap ui work, but they currently drag full ai recomputation along with them

**validity**: very high  
**timesave**: very high  
**risk**: low

**implementation sketch**
- add a pure `RefreshDebugTextFor()` path in `StatSummoner`
- move `TargetBest()` to explicit callers that truly changed combat state
- keep one shared helper for places that genuinely need both

**notes**
- this is the cleanest next step if stalls still correlate with `StatSummoner.SetDebugInformationFor()`
- remaining work here is coverage refinement, not the core split itself

---

### 2) batch enemy replans behind a dirty flag

**idea**
- when dice, stamina, items, or wounds change several times in a row, mark enemy intent dirty instead of rebuilding immediately each time
- flush exactly once at the end of the mutation burst, end of frame, or end of coroutine step

**why it should help**
- the current system is very reactive, which is correct for visibility, but repeated mutations during setup or fade coroutines can cause several near-identical replans in a row

**validity**: very high  
**timesave**: very high  
**risk**: medium

**implementation sketch**
- add `enemyPlanDirty`
- add `RequestEnemyPlanRefresh()`
- resolve the refresh once in a controlled place such as `LateUpdate`, a turn-manager checkpoint, or after the current mutation batch completes

**notes**
- this directly addresses the user’s own suspicion that remaining cost may be more about refresh frequency than planner body cost

---

### 3) replace full-state hashing with cheap version counters

**idea**
- instead of rebuilding the advanced-plan cache key from full stats, wounds, item flags, stamina, and dice lists on every `BuildPlan()` call, maintain version numbers that increment when relevant state changes

**why it should help**
- `CreateAdvancedPlanCacheKey()` walks dictionaries, wound lists, and dice lists every time
- if planner calls become frequent, cache-key construction becomes a tax even on hits

**validity**: high  
**timesave**: medium  
**risk**: medium

**implementation sketch**
- keep counters such as `combatStateVersion`, `enemyBoardVersion`, `playerBoardVersion`, `itemVersion`
- compose the cache key from those counters plus difficulty and target selection
- increment versions only at authoritative mutation points

**notes**
- this helps most once repeated refreshes are already under control

---

### 4) pool planner scratch data to remove hot-path allocations

**idea**
- reuse the dictionaries, arrays, lists, and hash sets allocated inside `BuildAdvancedPlan()`
- avoid repeated `new()` work for `yellowTotals`, `yellowCounts`, `staminaPlan`, `bestStaminaPlan`, `bestYellowAssignments`, and `visitedYellowStates`

**why it should help**
- the planner allocates several short-lived containers per build
- even if raw cpu time is acceptable, allocations can add gc spikes and editor hitching

**validity**: high  
**timesave**: medium  
**risk**: medium

**implementation sketch**
- create a reusable private scratch container owned by `EnemyAI`
- clear and refill arrays instead of allocating new dictionaries/lists
- consider fixed-size arrays for stats to make pooling cleaner

**notes**
- this is especially attractive if profiler shows gc allocs near `EnemyAI.BuildAdvancedPlan`

---

### 5) replace string-keyed stat dictionaries with indexed arrays or enums

**idea**
- use a fixed stat index (`green=0`, `blue=1`, `red=2`, `white=3`) everywhere in the planner instead of `Dictionary<string, int>` and repeated string lookups

**why it should help**
- the planner is full of tiny hot-path dictionary and string operations
- the stat space is fixed and tiny, so arrays are a much better fit

**validity**: high  
**timesave**: medium to high  
**risk**: medium to high

**implementation sketch**
- add a `StatId` enum or rely on the existing index mapping
- convert planner-only containers first
- leave public gameplay APIs string-based if needed, but translate once at the boundary

**notes**
- this is a substantial refactor, but it is the most direct way to make the planner more data-oriented

---

### 6) collapse equivalent yellow assignments earlier

**idea**
- enumerate unique yellow total states directly instead of recursing through each yellow die as if die identity mattered
- if two different assignment orders produce the same per-stat totals and counts, skip the duplicate before the deep search

**why it should help**
- a `visitedYellowStates` set already exists, which means duplicate states are known to happen
- generating duplicates and deduplicating later still burns recursion and bookkeeping time

**validity**: high  
**timesave**: high in yellow-heavy fights  
**risk**: medium

**implementation sketch**
- treat yellow dice as a multiset by face value
- generate combinations of totals/counts instead of per-die permutations
- preserve assignment reconstruction only for the winning state

**notes**
- this likely matters most in the `yellow=4` and above scenarios that are still taking double-digit milliseconds

---

### 7) pre-prune targets before full candidate evaluation

**idea**
- skip targets that cannot possibly beat the current best candidate under the spec
- examples:
  - skip face when armor blocks the only payoff
  - skip chest unless one of the two chest gates can actually become true
  - skip already-wounded non-face targets when they cannot enable any gate

**why it should help**
- target count is small, but each target drives a full evaluation path with state creation and wound simulation
- several targets are often obviously dead on arrival in a given board state

**validity**: high  
**timesave**: medium  
**risk**: medium

**implementation sketch**
- add a fast precheck layer before `EvaluateAdvancedPlanCandidate()`
- only allow a target through when at least one early gate is still realistically reachable

**notes**
- this must be proven carefully against sections 8 and 9 of ENEMY_AI.md so no legal winner is skipped

---

### 8) build a shared base simulation state once per yellow/stamina split

**idea**
- create one base `SimState` for the current yellow assignment and stamina split, then derive target-specific evaluations from that base instead of fully rebuilding the state per target

**why it should help**
- most of the simulated numbers are identical across the target loop
- only target-dependent wound application and target-specific gates differ

**validity**: high  
**timesave**: medium  
**risk**: medium

**implementation sketch**
- split `CreateSimulationState()` into:
  - base stat assembly
  - per-target evaluation from the already-built state
- use a cheap clone or a pair of mutable scratch states for branch a / branch b

**notes**
- this is a natural follow-up after buffer pooling

---

### 9) cache post-wound branch results inside one build

**idea**
- memoize expensive branch outcomes such as:
  - player-before-hit facts
  - player-after-enemy-hit facts for each enemy target
  - enemy-after-player-hit facts for the player’s declared target

**why it should help**
- inside one build, many candidate evaluations repeat the same logical checks with the same stat totals
- the player’s declared target is fixed across the whole search, which makes some branch results especially cacheable

**validity**: medium  
**timesave**: medium  
**risk**: medium

**implementation sketch**
- keep a small per-build memo keyed by a compact state signature plus target index
- cache outcomes such as `playerKillsBefore`, `playerDamagesBefore`, and post-wound follow-up gates

**notes**
- useful if profiler shows `EvaluateAdvancedPlanCandidate()` dominating after call frequency is reduced

---

### 10) branch-and-bound pruning based on current best gate vector

**idea**
- once a strong best candidate exists, stop exploring search branches that cannot possibly beat it on the earliest deciding gates

**why it should help**
- the comparator is lexicographic
- if the current branch can no longer reach a gate that the best plan already wins on, the rest of that subtree is wasted work

**validity**: medium  
**timesave**: high  
**risk**: high

**implementation sketch**
- compute optimistic upper bounds for a partial branch
- if even the optimistic outcome loses at the earliest differing gate, prune immediately

**notes**
- this can save a lot, but it is the easiest place to introduce subtle correctness bugs
- only worth doing after easier structural wins are taken

---

### 11) add simple fast paths for trivial advanced states

**idea**
- short-circuit common cases before the full advanced search starts
- examples:
  - no yellow dice and zero usable stamina
  - speed lock makes blue irrelevant for all candidates
  - player armor or dodgy makes offensive-first branches collapse to defense/survival only

**why it should help**
- the full advanced planner is overkill for some board states whose legal choices are already heavily constrained by the spec

**validity**: high  
**timesave**: medium  
**risk**: low to medium

**implementation sketch**
- add a handful of safe preflight checks in `BuildAdvancedPlan()`
- if the check proves only one reduced search space remains, jump straight there

**notes**
- this should be kept narrow and proof-based, not a second planner that drifts from the main one

---

### 12) precompute bitmasks for wounds, items, and target reachability

**idea**
- replace repeated `Contains()` checks and repeated target legality checks with compact booleans or bitmasks stored once per build

**why it should help**
- the planner repeatedly asks questions like whether a wound exists, whether speed is locked, and which targets are reachable
- these are tiny costs, but they sit in very hot code

**validity**: high  
**timesave**: low to medium  
**risk**: low

**implementation sketch**
- build a planner context struct with precomputed booleans for wounds/items
- keep target reach thresholds as ints or a mask

**notes**
- this pairs well with idea 8 and is easy to justify

---

### 13) suppress or sample development logging for planner profiling

**idea**
- do not `Debug.Log()` every advanced-plan profile line in development builds during stress testing
- either sample every nth run or expose the summary without logging each run

**why it should help**
- editor logging can be surprisingly expensive and can distort the very timing being measured
- current profiling output is useful, but always-on logging can hide the real remaining cost

**validity**: high  
**timesave**: low to medium in editor, near zero in release  
**risk**: low

**implementation sketch**
- keep `LastAdvancedPlanProfileSummary`
- add a toggle or sample rate for the log call itself

**notes**
- this is not a gameplay optimization, but it can make profiling cleaner and the editor feel less hitchy

---

### 14) move planner math toward pure data and away from scene objects

**idea**
- snapshot all required combat data into a compact immutable context before searching, so the planner never touches live scene structures during the hot loop

**why it should help**
- scene-object access and dictionary/list traversals are mixed throughout setup work
- a compact snapshot makes caching, pooling, and testing easier

**validity**: high  
**timesave**: medium  
**risk**: medium

**implementation sketch**
- create a `PlannerContext` struct with all stats, wounds, items, dice summaries, and lock flags already flattened
- let `BuildAdvancedPlan()` and `EvaluateAdvancedPlanCandidate()` consume only that snapshot

**notes**
- this is a strong foundation refactor that makes several other ideas safer

---

### 15) burst/job-system rewrite of the planner

**idea**
- move the search into burst-friendly data structs and run it as a job

**why it might help**
- in theory this could cut raw candidate evaluation time substantially

**validity**: low to medium  
**timesave**: potentially high  
**risk**: very high

**implementation sketch**
- only realistic after the planner is fully data-oriented and detached from unity object access

**notes**
- this is not a good first optimization
- the codebase is still far more likely to benefit from fewer replans and less allocation before needing jobs

---

## best next bets

if the goal is maximum gain for minimum risk, the strongest next sequence is:

1. separate debug refresh from `TargetBest()`
2. batch enemy replans behind a dirty flag
3. pool planner scratch containers
4. replace planner-local dictionaries with arrays / enum indices
5. collapse equivalent yellow assignments earlier

that order attacks the likely remaining costs in the order they are most likely to matter:
- too many planner invocations
- too much per-invocation allocation and setup work
- too much combinatorial search in yellow-heavy states

---

## guardrails

any optimization must preserve the behavior described in ENEMY_AI.md, especially:
- gate ordering remains lexicographic
- no stamina-funded normal-mode face targeting
- hard/nightmare search still finds the same best plan
- chest remains a conditional fallback rather than a default target
- cache or batching must not let the displayed enemy intent go stale beyond the accepted refresh window
