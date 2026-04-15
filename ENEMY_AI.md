# enemy ai decision tree

this document defines the upgraded enemy ai with four difficulties:

- easy
- normal
- hard
- nightmare

hard and nightmare share the same tactical brain.
nightmare keeps harsher economy/spawn pressure while using the same advanced planner.

---

## 1) design goals and ordering

hard/nightmare priority order:

1. kill the player this round if possible
2. if kill is not possible, prevent or weaken the player counter line
3. if both sides can land hits, convert mutual hit into favorable exchange
4. preserve stamina and flexible yellow value when outcomes are equal
5. deny player future breakpoints during draft and discard effects

normal priority order:

1. preserve legacy feel
2. no stamina-funded face targeting
3. show all enemy stamina/yellow intentions in real time

easy priority order:

1. minimal readable behavior
2. only spend stamina to barely complete attack or defense

---

## 2) difficulty split (final behavior)

### easy

- allowed stamina use: red and white only
- forbidden stamina use: green and blue
- target choice: default target from current aim only
- yellow behavior: no deep tactical reassignment logic
- spend style:
  - if enemy attack is not enough, add only the minimum red to beat player defense
  - if player attack beats enemy defense, add only the minimum white to survive if possible

### normal

- same baseline tactical style as legacy normal
- one strict change: no stamina use for climbing to face
- if enemy already has natural aim >= 7, face remains legal
- plan must be live-visible (no hidden last-second rewrite)
- recalc triggers are immediate for any combat-relevant board change

### hard

- advanced search planner
- yellow dice treated as fully flexible pseudo-stamina
- planner explores many candidate states and picks best scored state
- targets are selected by board outcome, not max target index

### nightmare

- same ai planner as hard
- same decision tree and heuristics
- external pressure (resource/spawn/encounter harshness) remains nightmare-specific

---

## 3) live recalculation triggers

enemy plan recalculates immediately when any of these happen:

- player or enemy stamina added/removed
- yellow reassigned
- any die attached, detached, discarded, rerolled, or value-changed
- item effect changes combat stats (potions, passives, triggered items)
- target index changes
- wound effects alter stats or lock speed interactions

this prevents the "surprise rewrite" problem and keeps enemy intent inspectable.

---

## 4) complete tactical decision tree (hard/nightmare)

notation:

- p = player
- e = enemy
- hit(x->y) means x attack > y defense and aim meets selected target
- kill means face lethal or third wound completion (with immunity checks)
- disable means selected wound removes opponent ability to hit or preserve lethal line

### root node r0: encounter validity

1. if encounter actor is merchant/blacksmith/tombstone -> stop tactical planning
2. if p dead or e dead -> stop tactical planning
3. if planning stage requires fully drafted board and draft incomplete -> defer to draft-only evaluation
4. else proceed to board snapshot

### node r1: board snapshot

capture:

- current stats: aim/speed/attack/defense for p and e
- added stamina by stat for both sides
- attached dice sets by stat for both sides
- yellow dice count and values
- current wound lists for p and e
- item flags: armor, dodgy, maul, spear/legendary speed lock, hatchet/yellow interactions
- declared targets

### node r2: generate candidate state space

1. enumerate yellow assignments (for each yellow die -> one of green/blue/red/white)
2. enumerate stamina distributions (all integer splits across 4 stats, bounded by remaining stamina and restrictions)
3. for each candidate, compute legal target range from resulting aim
4. evaluate each legal target as a full combat exchange

state-space pruning rules:

- drop dominated states (strictly worse attack, defense, speed, and no better target access)
- drop overspend states when a lower spend state has same outcome class
- cap search by using best-known score floor

### node r3: initiative branch

for each candidate state, branch by action order:

1. enemy acts first
2. player acts first
3. tie rules and forced speed locks (knee/spear effects)

#### branch a: enemy acts first

a1. can enemy hit selected target?

- no -> heavy score penalty; keep candidate only if defensive denial still best among bad options
- yes -> continue

a2. does first hit produce immediate kill that is not canceled by armor/dodgy rules?

- yes -> label outcome class `lethal_now`
- no -> continue

a3. if no lethal, does chosen wound disable player counterattack?

- evaluate wound simulation at resulting state:
  - armpits can remove red line
  - hand can collapse white threshold interactions
  - guts can collapse multiple stat lines
  - head can remove pivotal die
  - hip can strip spent stamina and flip thresholds
  - knee can lock future order
- if counter removed -> label `safe_hit`
- else -> label `trade_or_risk`

a4. compute surviving player reply after wound application and update score

#### branch b: player acts first

b1. can player hit in pre-hit state?

- no -> continue to enemy offense optimization
- yes -> simulate received wound first (unless immunity rule blocks)

b2. after taking player wound, can enemy still hit?

- yes -> continue
- no -> label `stopped`

b3. if player can kill and enemy cannot prevent it, choose best trade line (resource-aware)

b4. if survival possible, choose allocation that minimizes death risk first, then maximizes counter-hit quality

### node r4: target scoring branch

for each candidate state and target, compute wound utility in context:

- chest: generally weak, mostly fallback
- guts: best for broad dice/stat collapse
- knee: best for order manipulation
- hip: best vs stamina-heavy plans
- head: best vs pivotal single die plans
- hand: best vs white-dependent defenses
- armpits: best vs red-driven counter lines
- face: best only when it actually closes kill path efficiently

critical rule:

- do not auto-pick highest legal target
- pick target with best outcome class + tie-break score

### node r5: outcome class ordering

sort candidate outcomes by this strict class order:

1. `lethal_now`
2. `prevent_counter_and_survive`
3. `survive_and_force_favorable_trade`
4. `survive_even_exchange`
5. `high_damage_unfavorable_trade`
6. `cannot_hit_but_survive`
7. `losing_line`

within same class, tie-break with:

1. lower stamina spent
2. lower yellow commitment rigidity (more future flexibility)
3. higher denial of player next-turn breakpoint
4. lower exposure to known item counters (armor/dodgy/maul)

### node r6: plan commit

commit selected plan:

1. refund current added enemy stamina
2. clear yellow placements from previous temporary assignment
3. apply new yellow assignment
4. apply stamina split
5. update target index
6. refresh summoned stats and visual positions
7. persist game state

---

## 5) specialized scenario branches (generalized)

### s1: clean lethal exists

- choose cheapest lethal line
- prefer lethal that remains valid after common disruption (head discard, reroll variance)

### s2: only non-face disabling line is correct

- if lower wound removes player counter and face does not, choose lower wound
- examples: guts/armpits/head/hip flipping hit condition

### s3: forced mutual hit

- if preventing player hit is impossible, maximize outcome quality:
  - avoid overspending red
  - preserve stamina for next turn if trade outcome unchanged
  - deny player recovery dice when possible

### s4: player armor active

- first-hit value is reduced
- prioritize surviving counter and setup over speed-for-damage greed

### s5: player dodgy active

- speed value increases sharply when dodge checks depend on order
- if speed cannot be secured, shift to defense and denial instead of overpaying damage

### s6: player maul threat

- any successful player hit is near-lethal
- overvalue defense and counter-disabling wounds

### s7: enemy expects to be wounded first at guts

- project post-guts stat reduction before choosing allocation
- pre-compensate attack/defense/speed to preserve minimum viable line after dice-count reduction

### s8: enemy expects to be wounded first at head

- estimate pivotal die loss on enemy side
- avoid brittle plans that fail if largest contributor is removed

### s9: retroactive disruption window

when player action occurs after initial planning (discard/reroll/late effect):

1. reopen plan window
2. rebuild candidate states from new board
3. re-commit best plan with updated stamina and yellow placement

---

## 6) discard target decision tree

### player-die discard (enemy inflicts head)

1. build candidate list of removable player dice
2. score each die by delta to player line quality:
   - attack threshold damage
   - speed threshold damage
   - aim threshold damage
   - defense threshold damage
3. amplify score if removing die cancels a hit or kill class
4. select highest score die; break ties by higher face-value die

### enemy-die discard resilience (player inflicts head)

1. estimate likely enemy die to be removed (worst-case weighted)
2. avoid plans that collapse under one-die removal if an equally scored robust plan exists

---

## 7) draft decision tree (hard/nightmare)

### d0: evaluate each available die with two futures

for each die:

1. self-value = best future plan improvement if enemy drafts it
2. denial-value = estimated reduction to player best future if enemy denies it
3. flexibility-value = option value of delayed commitment (highest for yellow)

final draft score:

- score = self-value + denial-value + flexibility-value - overcommit_penalty

### d1: overcommit guardrails

if forced mutual hit remains true across best forecasts:

- reduce priority on extra red past needed threshold
- increase denial picks that reduce player certainty next turn

### d2: breakpoint-first policy

prefer smaller die that completes critical breakpoint over larger die that only inflates already-won stat.

breakpoints in descending importance:

1. kill enabling
2. hit enabling (attack > defense)
3. order enabling (speed win/tie lock)
4. target enabling (aim reaches disabling wound)
5. survive enabling (defense threshold)

### d3: yellow draft handling

yellow is treated as a future branch multiplier, not fixed red.
after draft, yellow assignment is chosen by current best combat plan and can change on replans.

---

## 8) normal-mode face constraint tree

normal mode target rule:

1. compute natural aim (stats + fixed dice + yellow assignment)
2. compute stamina-added aim
3. face legal only if natural aim >= 7
4. if stamina is required to reach 7, cap target at max non-face wound

this preserves familiar normal behavior while removing stamina-funded face spikes.

---

## 9) easy-mode micro tree

1. baseline target = default highest non-wounded legal target from current aim
2. if enemy attack <= player defense:
   - spend minimum red needed to make attack > defense, bounded by stamina
3. recompute
4. if player attack > enemy defense:
   - spend minimum white needed so player attack <= enemy defense, bounded by remaining stamina
5. do not spend blue/green stamina
6. do not search alternative yellow allocations

result: readable, low-surprise, threshold-only ai.

---

## 10) robustness rationale

why this tree is robust:

- it is outcome-class driven, not stat-sum greedy
- it evaluates order-of-operations explicitly
- it models wound consequences before finalizing target
- it handles retroactive board changes by replanning, not by stale commitment
- it values denial and flexibility in draft, avoiding common overcommit traps
- it preserves difficulty identity:
  - easy = minimal threshold logic
  - normal = legacy feel with transparent intent and no stamina-face spike
  - hard/nightmare = predictive tactical search

this structure generalizes beyond specific examples because every branch is built on combat primitives:

- can hit?
- can kill?
- can survive reply?
- can disable reply?
- can achieve same class cheaper?

those primitives remain valid across new items, new enemy kits, and future balance patches.