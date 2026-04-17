# enemy ai decision tree

this document defines the enemy ai across four difficulties and serves as the authoritative
specification for expected planning behavior. it should be read alongside EnemyAI.cs.

difficulties: easy / normal / hard / nightmare

hard and nightmare share the same tactical brain (BuildAdvancedPlan).
hard reveals that plan live during the draft phase.
nightmare keeps harsher economy/spawn pressure, but now holds the plan back until attack time
and reveals it with a short click-by-click animation.

---

## 0) game combat primitives (ground truth)

pulled from TurnManager.cs. these are the only rules the AI reasons about.

**initiative:**
- player goes first when `playerSpd >= enemySpd` (player wins ties)
- enemy goes first ONLY when `enemySpd > playerSpd` (strictly greater)
- spear+legendary: player always acts first regardless of speed
- knee wound on ENEMY → player speed locked high (player always goes first)
- knee wound on PLAYER → enemy speed locked high (enemy always goes first)

**hit conditions:**
- any attack lands when `attacker_attack > defender_defense` (strictly greater)
- a defense holds when `defender_defense >= attacker_attack`
- accuracy (green) must be >= 0 to land any hit; negative aim = automatic miss
- accuracy must be >= 7 to target neck

**kill conditions:**
- neck hit causes bleed out next round (Lich is immune to neck bleed-out)
- any third wound kills (re-wounding an existing site does NOT count)
- maul kills on any successful hit unconditionally

**planner shorthand used in EnemyAI.cs:**
- `PlayerWouldKillEnemy` and `EnemyWouldKillPlayer` are fatal-line helpers, not strict same-round death checks
- a landed neck hit is folded into that fatal bucket because the resulting bleed-out is guaranteed unless separately prevented
- gate names like `EnemyKills` and `BreaksPlayerKill` therefore mean "creates or prevents a fatal line" inside the planner

**wound effects (immediate, applied the same round):**
- chest: player can reroll enemy dice >= 3 face value (once per die); forces enemy rerolls
- guts: every attached die on the wounded side loses 1 pip
- knee: speed of the injured side is locked lower permanently
- hip: ALL stamina the wounded side has spent this round is refunded and reset to zero
- head: opponent discards one of the wounded side's attached dice (they pick their best)
- hand: ALL white dice on the wounded side are removed permanently
- armpits: ALL red dice on the wounded side are removed permanently
- neck: wounded side bleeds out next round

**stamina rules:**
- stamina is finite and persists across rounds (it does NOT reset each round)
- committed stamina is consumed even if the wound never lands
- enemy hip wound blocks ALL stamina use (unless Lich)
- Lich stamina refreshes to baseline at the start of each round
- after any round ends, addedEnemyStamina is cleared and Save.game.enemyStamina is
  resynced to base enemy.stamina — no allocation ever lingers past the turn

**items that affect planning:**
- armor: absorbs one hit; the hit produces no wound; armor then breaks
- exalted: behaves like armor for one hit; the hit produces no wound; the charm then breaks, and enemy planning must NOT cash in any same-hit wound followup gates from that blocked attack
- dodgy: if the player is still dodgy when the enemy attack resolves, the hit is dodged; if enemy goes first, dodgy is cleared before that hit
- maul: any successful player hit = kill; changes threat calculation entirely
- hatchet: enemy yellow dice fade out on attach (yellow self-value becomes zero)
- spear+legendary: player always acts first; blue investment has zero return
- crystal shard: when the player takes a real wound, one crystal shard breaks immediately and the player loses 2 red before any same-round counterattack; this is NOT charm-arcane-scaled
- bulwark: if the enemy will attack first, the player gains +1 white per effective bulwark immediately before that hit is checked, so enemy-first hit math must use the boosted defense threshold
- inevitable: if the enemy attacks first, the player's round-two counterattack gains +1 red per effective inevitable immediately after the enemy swing resolves, even on a parry; this is charm-arcane / stave scaled and must be modeled before the player's reply
- glass sword: when the player takes a real wound, an unshattered glass sword immediately becomes 0 / 1 / 1 / 0 before any same-round counterattack
- riposte: if the enemy goes first and is parried, the player's round-two counterattack gets +1 red per riposte immediately, scaled by charm-of-the-arcane / stave effectiveness
- vindictive / charm of the vindictive: if the enemy goes first and inflicts a real wound, the player's round-two counterattack gets +2 red per effective copy immediately, scaled by charm-of-the-arcane / stave effectiveness
- scimitar: if the enemy goes first and is parried, the player may discard enemy die value before their counterattack; the planner models this as immediate enemy-die loss before the reply
- charm of the arcane: every non-arcane scalable charm copy is multiplied by the shared arcane effectiveness; `stave` adds one extra charm-arcane layer in the same way it already boosts necklets
- tarot of the arcane: every non-arcane tarot copy adds an extra +1 upgrade layer to matching drafted dice, still capped at 6
- lucky dice: lucky-dice bonuses only exist in live fightable combat; merchant / blacksmith / dead-enemy states clear them so the planner and the player both see true weapon stats outside combat
- witch hand / necklets / charm active bonuses: these are already reflected in live summed stats or die values, so the planner snapshot sees them directly
- tarots / tarot of the arcane: live attached dice already include the bonus; draft previews explicitly add the capped tarot upgrade when judging player denial

---

## 1) design goals and priority ordering

### hard / nightmare gate ordering (IsBetterAdvancedEvaluation)

candidates are compared left-to-right. first differing gate wins. no floating-point values.

```
 1  EnemyKills                       the enemy creates a fatal line (third wound or guaranteed neck bleed-out)
 2  EnemyDamagesPlayer               the enemy inflicts any wound
 3  EnemyAvoidsKill                  the enemy survives without being killed
 4  EnemyAvoidsDamage                the enemy takes no wound at all
 5  BreaksPlayerKill                 prevents a player kill that would otherwise land
 6  BreaksPlayerDamage               prevents any player wound
 7  BreaksPlayerProtection           breaks armor or exalted without incorrectly simulating a wound through it
 8  StripsPlayerStamina              causes player to lose all added stamina (hip wound)
 9  BreaksPlayerSpeed                denies player their speed advantage (knee wound)
10  RemovesPlayerRed                 removes all player red dice (armpits wound)
11  RemovesPlayerBestDie             discards player's best die (head wound)
12  RemovesPlayerWhite               removes all player white dice (hand wound)
13  BreaksPlayerTarget               prevents player from reaching their declared target
14  UsesChestOnHighValuePlayerDice   target chest when player has high-value dice (denial)
15  UsesChestAsLastDitchGamble       target chest as desperation when damage is unavoidable
16  EnemyActsFirst                   enemy moves before player this round
17  SpentStamina   (lower is better) same outcome reached with less stamina
18  TotalOverspend (lower is better) less excess beyond each threshold boundary
19  ResourceOverspend (lower is better) less unused surplus in already-cleared stats
20  TargetIndex       (lower is better) prefer lower target as final tiebreaker
```

### hard / nightmare draft tie handling

- draft previews still compare the high-level outcome gates first
- if two draft picks reach the same real preview outcome, the ai now compares the full preview plan before raw progress scores
- this prevents "futile parry" drafting where a defensive die that still needs extra stamina can beat a cleaner pyrrhic-hit die that already secures the same damage line
- planner snapshots also cache the live triggered charm attack bonuses, so mid-turn charm activations do not reuse stale hard/nightmare plans

### normal priority order

1. preserve the familiar feel; no degenerate surprises
2. no stamina-funded neck targeting (natural aim must already be >= 7)
3. enemy intent stays hidden until the attack begins
4. threshold-only spending — never waste stamina above the exact breakpoint

### easy priority order

1. minimal threshold arithmetic only
2. readable, predictable behavior that teaches the player the rules
3. never spend blue or green stamina

---

## 2) difficulty split (final behavior)

### easy

- allowed stamina: red (to just beat playerDef) and white (to just beat playerAtt)
- forbidden stamina: green, blue
- target choice: highest reachable non-wounded slot from natural aim
- yellow behavior: no reassignment; yellow always goes to red by default
- futility check: if `enemyAtt + remainingStamina <= playerDef`, skip red spend
- futility check: if `playerAtt + remainingStamina <= enemyDef`, skip white spend

### normal

- constraint: neck requires natural aim >= 7 (base stats + fixed dice + yellow; NO stamina)
- threshold-only spending: exact amount per transition
  - red spend  = playerDef - enemyAtt + 1
  - blue spend = playerSpd - enemySpd + 1
  - white spend = playerAtt - enemyDef
  - green spend = forbidden in normal mode for neck access
  - no compound spending (two thresholds simultaneously)
- plan is computed only when the attack begins; it is not shown during draft

### hard

- full candidate search via BuildAdvancedPlan
- all yellow dice assigned to any stat; all integer stamina splits enumerated
- all targets 0..maxLegalTarget evaluated for each (yellow, stamina) pair
- white stamina search now includes the live pre-hit defense threshold plus separate enemy-first parry-reply and enemy-first wound-reply defense thresholds, so an impossible wound-only charm spike cannot hide a reachable parry-only defense spend
- if a round is still a guaranteed hit trade after spending green stamina, non-fatal aim spending is discarded; in that state green stamina is only justified when it upgrades the line into a fatal target, normally neck
- candidates compared gate-by-gate via IsBetterAdvancedEvaluation (section 1)
- no floating-point scoring; no blended utility values
- truly futile stamina plans are discarded before comparison (section 4)
- perfect plan (zero stamina, zero overspend, enemy kills) short-circuits the search
- the current live plan stays visible during the draft phase

### nightmare

- identical search planner to hard
- no changes to the decision tree
- the plan is hidden during draft and revealed only when the attack begins
- reveal order is: stamina pips, then yellow-die moves, then target shifts
- each reveal step plays `click0`, performs one action, then waits `0.05s`
- environmental difference only (encounter harshness, spawn density, enemy stats)

---

## 3) master planning flow

```
  START: BuildAdvancedPlan
         |
         v
  CanPlan?
  |-- NO (Merchant / Blacksmith / Tombstone / dead) --> STOP
  |
  +-- YES
         |
         v
  hip wound active AND not Lich?
  |-- YES --> canUseStamina = false  (yellow search only, no stamina splits)
  |
  +-- NO  --> canUseStamina = true   (full search)
         |
         v
  for each yellow assignment (green / blue / red / white per die):
    for each stamina split across green / blue / red / white:
      for each targetIndex 0..maxLegalTarget:
        |
        v
        candidate = EvaluateAdvancedPlanCandidate(...)
        |
        +-- IsTrulyFutileAdvancedEvaluation(candidate)? --> skip; next iteration
        |
        +-- IsBetterAdvancedEvaluation(candidate, best)? --> best = candidate
        |
        +-- IsPerfectAdvancedEvaluation(best)? --> exit all loops immediately
  |
  v
  ApplyPlan(best)
```

---

## 4) stamina futility rules

stamina is permanent. spending it on goals that change nothing is a strategy-level error.

### hard-mode futility filter (IsTrulyFutileAdvancedEvaluation)

a plan that spends stamina is skipped entirely if ALL of the following gates are false:

```
  EnemyKills
  EnemyDamagesPlayer
  EnemyAvoidsKill
  EnemyAvoidsDamage
  BreaksPlayerKill
  BreaksPlayerDamage
  BreaksPlayerProtection
  StripsPlayerStamina
  BreaksPlayerSpeed
  RemovesPlayerRed
  RemovesPlayerWhite
  RemovesPlayerBestDie
  BreaksPlayerTarget
  UsesChestOnHighValuePlayerDice
  UsesChestAsLastDitchGamble
```

AND TotalOverspend >= SpentStamina.
(spending 3 stamina to produce 3 points of excess produces no outcome change; skip it)

### per-stat futility (applies at all difficulties before any spend)

RED -- skip if any of:
  - enemyAtt is already > playerDef (hit already confirmed, more red does nothing)
  - enemyAtt + all remaining stamina <= playerDef (threshold is unreachable)
  - player acts first and targets armpits (red dice will be removed before enemy attacks)

WHITE -- skip if any of:
  - enemyDef is already >= playerAtt (defense already confirmed)
  - playerAtt + remaining stamina < enemyDef + 1 (gap is uncloseable)
  - when enemy acts first, defense candidate search must cover the base reply, the parry-triggered reply, and the wound-triggered reply separately before deciding white spend is impossible

BLUE -- skip if any of:
  - enemy already acts first (speed advantage already present)
  - gaining speed does not change any outcome gate this round
  - enemy knee wound locked player first (speed cannot be reclaimed)

GREEN -- skip if any of:
  - current aim already reaches the highest-value wound available given attack
  - the only new target accessible is neck and player has armor (armor blocks the neck wound)
  - if the final candidate is still a guaranteed hit trade and the extra green does not create a fatal line, treat that green spend as futile

---

## 5) live recalculation triggers

easy and hard keep the enemy plan live once drafting is complete.
normal and nightmare keep the enemy plan hidden until the attack begins.

easy still uses the simple threshold planner. the only visibility change is that once no
unattached dice remain, the player can see the enemy's current stamina commitment and target
while adjusting their own board state.

the plan is rebuilt and recommitted whenever any of the following happen outside of combat:

- player or enemy stamina added or removed
- a yellow die is reassigned
- any die is attached, detached, discarded, rerolled, or has its value changed
- an item effect alters combat stats
- the player's target index changes
- a wound effect alters a stat or locks a speed interaction

handled by RefreshEnemyPlanIfNeeded in TurnManager.cs.
prevents stale plans on easy and hard, where the enemy intent is intentionally visible.

planning is also skipped while a fresh fight has no live dice in play yet.
this prevents pre-pick stamina commits during level-transition setup.

---

## 6) breakpoint table

a breakpoint is the threshold that changes an outcome gate.
prefer completing one precisely over exceeding it.

  breakpoint            condition                          significance
  -----------------------------------------------------------------------
  hit enabled           enemyAtt > playerDef               enemy inflicts a wound
  fatal neck line       enemyAim >= 7 AND hit enabled      planner treats guaranteed bleed-out as fatal
  kill via third wound  playerWoundCount == 2 AND hit      three-wound win
  order flip            enemySpd > playerSpd               acts first; all first-strike value
  aim for armpits       enemyAim >= 6                      removes all player red dice
  aim for head          enemyAim >= 4                      discards player's best die
  aim for hip           enemyAim >= 3                      strips all player stamina
  aim for knee          enemyAim >= 2                      locks player speed lower permanently
  survive counter       enemyDef >= playerAtt              takes no wound

same gate result on all candidates -> prefer fewer stamina spent (gate 16). savings compound.

---

## 7) candidate evaluation tree (hard / nightmare)

what EvaluateAdvancedPlanCandidate computes for every (yellow, stamina, target) triple.

```
  INPUTS: targetIndex, yellowTotals, yellowCounts, staminaPlan
          |
          v
  Build SimState from current board + yellow assignments + stamina spend

  enemyActsFirst = EnemySpeedLockedHigh
                   OR (!PlayerSpeedLockedHigh AND E.spd > P.spd)

  enemyCanHit   = E.aim >= 0 AND E.att > P.def
  playerCanHit  = P.aim >= 0 AND P.att > E.def
  playerDamages = PlayerHitDamagesEnemy(state, playerTarget, playerCanHit)
  playerKills   = PlayerWouldKillEnemy (state, playerTarget, playerCanHit)

  ===================================================================
  BRANCH A: enemyActsFirst = true
  ===================================================================

  enemyCanHit?
  |-- NO --> EnemyKills = false, EnemyDamagesPlayer = false
  |          (all offence gates remain false)
  |
  +-- YES
         |
         v
  EnemyHitAppliesWound?
  |-- armor active?   --> wound skipped; armor breaks; EnemyDamagesPlayer = false
  |-- dodgy active?   --> not applicable here; enemy-first clears dodgy before the hit
  |
  +-- wound lands:
         |
         v
  EnemyWouldKillPlayer?  (fatal line: neck@aim7 OR third wound)
  |-- YES --> EnemyKills = true
  |
  +-- NO or blocked
         |
         v
  EnemyDamagesPlayer = true  (wound lands, even if not a kill)
         |
         v
  ApplyWoundToPlayer(simulatedState, target):
    guts    --> P.att and P.aim reduced by redDiceCount
    knee    --> BreaksPlayerSpeed = was player going first?
    hip     --> StripsPlayerStamina = did player have added stamina?
    head    --> RemovesPlayerBestDie = did best die exist?
    hand    --> RemovesPlayerWhite   = did white dice exist?
    armpits --> RemovesPlayerRed     = did red dice exist?
    chest   --> see section 8
    neck    --> player bleeds out next round
         |
         v
  check player reply from post-wound state:
    playerStillHits  = P.att(wounded) > E.def AND P.aim(wounded) >= 0
    playerStillKills = PlayerWouldKillEnemy(wounded state)

    !playerStillKills AND was killing before  --> BreaksPlayerKill = true
    !playerStillHits  AND was hitting before  --> BreaksPlayerDamage = true
    !playerStillKills (regardless)            --> EnemyAvoidsKill = true
    !playerStillHits  (regardless)            --> EnemyAvoidsDamage = true

    player declared target unreachable now?   --> BreaksPlayerTarget = true

  ===================================================================
  BRANCH B: enemyActsFirst = false (player acts first)
  ===================================================================

  playerDamages AND !EnemyIsLich?
  |-- YES --> ApplyWoundToEnemy(simulatedState, playerTarget)
  |            |
  |            v
  |           check enemy function post-wound:
  |             enemyStillHits  = E.att(wounded) > P.def AND E.aim(wounded) >= 0
  |             enemyStillKills = EnemyWouldKillPlayer(wounded state)
  |
  |           EnemyAvoidsKill   = !playerKills
  |           EnemyAvoidsDamage = !playerDamages
  |           EnemyKills        = enemyStillKills
  |           EnemyDamagesPlayer = enemyStillHits
  |
  |           (secondary wound-removal gates are false in branch B;
  |            those gates only apply when enemy wounds the player)
  |
  +-- NO  --> player cannot damage enemy
               EnemyAvoidsKill   = true
               EnemyAvoidsDamage = true
               EnemyKills        = EnemyWouldKillPlayer(clean state)
               EnemyDamagesPlayer = enemyCanHit

  UsesChestAsLastDitchGamble computed here (branch B) -- see section 8

  ===================================================================
  OVERSPEND CALCULATION (both branches)
  ===================================================================

  RedOverspend   = GetAttackOverspend  (playerDef, enemyAtt, staminaPlan["red"])
  BlueOverspend  = GetSpeedOverspend   (playerSpd, enemySpd, staminaPlan["blue"])
  GreenOverspend = GetAimOverspend     (targetIndex, enemyAim, staminaPlan["green"])
  WhiteOverspend = GetDefenseOverspend (playerAtt,  enemyDef, staminaPlan["white"])

  SpentStamina  = sum(staminaPlan.Values)
  TotalOverspend = RedOverspend + BlueOverspend + GreenOverspend + WhiteOverspend
```

---

## 8) chest target gates

chest is not a default preference. it becomes valid under exactly two conditions.

```
  PlayerHasHighValueDice? (any stat dice sum >= 5  OR  total dice count >= 4)
  |
  +-- YES --> UsesChestOnHighValuePlayerDice = true  (enemy targets chest as denial)
  |
  +-- NO  --> UsesChestOnHighValuePlayerDice = false

  prerequisite: the chest hit must actually land
  - if the enemy cannot wound the player at all, both chest gates stay false
  - chest is never allowed to justify futile speed or aim spending by itself

  UsesChestAsLastDitchGamble = true when ALL of:
    - target is chest
    - EnemyKills         = false  (no fatal line created)
    - EnemyAvoidsDamage  = false  (will take a hit)
    - playerDamagesBefore = true  (player will wound first)
    - PlayerHasHighValueDice = true
    - RemovesPlayerRed        = false  (no better removal available)
    - RemovesPlayerWhite      = false
    - StripsPlayerStamina     = false
    - RemovesPlayerBestDie    = false

  rationale: chest forces enemy rerolls on dice >= 3 face value.
  when a hit is unavoidable and no disabling wound is reachable, chest at least
  creates the chance that the enemy's high-value dice will be disrupted before combat.

  chest ranks 13th and 14th in gate ordering:
    - preferred over acting first (gate 15)
    - never overrides any plan that disables, harms, or avoids the player
```

---

## 9) candidate comparison tree (IsBetterAdvancedEvaluation)

```
  candidate vs. current best -- first differing gate decides:

  [1]  EnemyKills?                        candidate wins if candidate=true, current=false
  [2]  EnemyDamagesPlayer?
  [3]  EnemyAvoidsKill?
  [4]  EnemyAvoidsDamage?
  [5]  BreaksPlayerKill?
  [6]  BreaksPlayerDamage?
  [7]  BreaksPlayerProtection?
  [8]  StripsPlayerStamina?
  [9]  BreaksPlayerSpeed?
  [10] RemovesPlayerRed?
  [11] RemovesPlayerBestDie?
  [12] RemovesPlayerWhite?
  [13] BreaksPlayerTarget?
  [14] UsesChestOnHighValuePlayerDice?
  [15] UsesChestAsLastDitchGamble?
  [16] EnemyActsFirst?
  [17] SpentStamina    candidate wins if candidate.SpentStamina < current.SpentStamina
  [18] TotalOverspend  candidate wins if candidate.TotalOverspend < current.TotalOverspend
  [19] ResourceOverspend candidate wins if candidate.ResourceOverspend < current.ResourceOverspend
  [20] TargetIndex      candidate wins if candidate.TargetIndex < current.TargetIndex

  if all 20 gates match: neither is better; keep current.

  SHORT-CIRCUIT (IsPerfectAdvancedEvaluation):
  EnemyKills=true AND SpentStamina=0 AND TotalOverspend=0 --> stop searching immediately
```

---

## 10) specialized scenario branches

### s1: clean kill exists

```
  EnemyKills = true, SpentStamina = 0, TotalOverspend = 0?
  +-- YES --> perfect; skip all remaining candidates (IsPerfectAdvancedEvaluation)

  EnemyKills = true but requires stamina?
  +-- YES --> continue search; a cheaper fatal line or yellow-based fatal line may still exist
              prefer yellow assignment over stamina spend (preserves stamina for later)
```

### s2: player armor active

```
  armor absorbs the first hit → EnemyDamagesPlayer = false for all E-first plans

  implication: gates 1-2 (EnemyKills, EnemyDamagesPlayer) are false for blocked-hit candidates
               gates 3-4 (survive) still decide first, then gate 7 rewards exact-threshold hits that crack the protection

  neck wound? --> armor blocks the hit entirely (TurnManager.cs checks armor before wound)
                  no bleed-out flag is applied when armor is active

  priority shift:
    - white (defense): closing defense gap is higher return than speed
    - EnemyAvoidsDamage (gate 4) is the new primary goal
    - once survival is tied, BreaksPlayerProtection (gate 7) rewards spending just enough to break armor / exalted
    - blue (speed) has low return; the protection absorbs the wound from going first
```

### s3: player dodgy active

```
  dodgy only matters if the enemy attack happens second
  EnemyHitAppliesWound = false when !enemyActsFirst AND dodgy

  implication: enemy-going-second candidates can lose their wound entirely
               blue gains value because flipping to enemy-first also clears dodgy before the hit

  priority shift:
    - white (defense): survive the player's attack that WILL land
    - EnemyAvoidsDamage (gate 4) becomes the primary goal
    - green: disabling wound aim (armpits, hip, head) if player doesn't dodgy and
             enemy can find a candidate where E goes second but still hits first via counter
```

### s4: player maul threat

```
  maul: any successful player hit = kill
  PlayerWouldKillEnemy = true whenever playerCanHit

  implication: EnemyAvoidsKill (gate 3) is nearly always false unless:
    - enemyDef >= playerAtt (player can't hit)
    - enemy acts first AND kills first (gate 1 wins before kill check)

  priority:
    1. EnemyKills first (gate 1) -- go offensive if there's any path to kill
    2. EnemyAvoidsDamage / EnemyAvoidsKill (gates 4, 3) via white spend
    3. if impossible: armpits (RemovesPlayerRed, gate 10), hip (StripsPlayerStamina, gate 8)
```

### s5: forced mutual hit

```
  ALL candidates have: EnemyDamagesPlayer=true AND EnemyAvoidsKill=false

  gates 1-4 are equal for all candidates; gates 5-14 decide the winner:

  BreaksPlayerKill (5)
    > BreaksPlayerDamage (6)
    > BreaksPlayerProtection (7)
    > StripsPlayerStamina (8)
    > BreaksPlayerSpeed (9)
    > RemovesPlayerRed (10)
    > RemovesPlayerBestDie (11)
    > RemovesPlayerWhite (12)
    > BreaksPlayerTarget (13)
    > chest gates (14, 15)
    > EnemyActsFirst (16)
    > SpentStamina lower (17)
```

### s6: player targets guts, player acts first

```
  guts removes 1 pip from every enemy attached die before enemy attacks
  modeled by ApplyWoundToEnemy in branch B (playerActsFirst path)

  compensate: draft or spend extra to ensure E.att_postGuts > P.def
    if P goes first AND P.aim >= 1 AND P.att > E.def:
      effective E.att after guts = E.att - E.redDiceCount
    ensure this value still > P.def for EnemyDamagesPlayer = true

  if compensation is infeasible: EnemyDamagesPlayer = false for all candidates;
  accept it and maximize survival gates (3, 4)
```

### s7: player targets head, player acts first

```
  head discards enemy best die before enemy attacks
  modeled by ApplyBestEnemyDiscard in branch B simulation

  brittle plan: entire outcome depends on one high-value die being present
    if that die is discarded, EnemyDamagesPlayer flips false

  prefer: spread value across multiple dice
          seek plans where removing the single best die still satisfies gate 2
```

---

## 11) discard decision tree

### 11a: enemy chooses which player die to discard (head wound on player)

```
  for each candidate player die, compute LiveDiscardEvaluation:

  BreaksKill?       removing this die causes playerKills  to flip false
  BreaksDamage?     removing this die causes playerCanHit to flip false
  BreaksGoFirst?    removing this die causes P.spd < E.spd
  BreaksTarget?     removing this die causes P.aim < playerTargetIndex
  RestoresDefense?  removing this die causes P.att <= E.def
  IsYellow?         die type is yellow (flexible; loss hurts multiple future options)
  DieValue          face value of the die

  comparison order (IsBetterLiveDiscardChoice):

  [1] BreaksKill?       -- candidate wins if true, current false
  [2] BreaksDamage?
  [3] BreaksGoFirst?
  [4] BreaksTarget?
  [5] RestoresDefense?
  [6] IsYellow?
  [7] DieValue          -- candidate wins if candidate.DieValue > current.DieValue

  discard the die that wins this comparison against all others.
```

### 11b: enemy-die discard resilience (head wound on enemy)

```
  player will choose symmetrically; enemy planning anticipates this via
  ApplyBestEnemyDiscard called in branch B simulation.

  robust plan = gate 2 (EnemyDamagesPlayer) survives losing the highest-impact enemy die

  if plan is fragile (losing that die breaks gate 2):
    --> continue searching for a plan that doesn't depend on one die
    --> if none exists: accept fragile plan and proceed
```

---

## 12) draft decision tree (hard / nightmare)

### d0: draft pick comparison (IsBetterDraftChoice)

```
  for each available die d, compute EvaluateAdvancedDraftChoice(s, d):

  BestPlan           = best AdvancedPlanEvaluation when d is attached to enemy
  DeniesPlayerKill   = player loses kill capability if enemy takes d instead of player
  DeniesPlayerDamage = player loses hit capability if enemy takes d instead of player
  DeniesPlayerGoFirst = player loses speed advantage if enemy takes d
  DeniesPlayerTarget  = player loses aim for declared target if enemy takes d
  LosesValueToHatchet = d is yellow AND player has hatchet (enemy yellow fades; zero self-value)
  IsYellow            = die type is yellow
  DieValue            = face value

  comparison (IsBetterDraftChoice):

  [1] IsBetterAdvancedEvaluation(BestPlan)?   gate-compare resulting plans first
  [2] DeniesPlayerKill?
  [3] DeniesPlayerDamage?
  [4] ProgressScore         prefer picks that close unmet enemy attack/defense/speed/aim gaps
  [5] DeniesPlayerGoFirst?
  [6] DeniesPlayerTarget?
  [7] LosesValueToHatchet?  (prefer die that does NOT lose value to hatchet)
  [8] same-value yellow tie if face values match and hatchet is not active
  [9] DieValue              (higher wins)

  implementation note:
  - `BestPlan` must be a real post-pick advanced search result
  - it must include the enemy's remaining stamina search after the draft pick
  - a shallow zero-stamina preview is not sufficient for hard/nightmare draft choice
  - `ProgressScore` is an internal tie-break heuristic used when no immediate gate flips exist yet
  - it rewards progress toward unmet enemy thresholds, especially survival when the enemy has no live hit line
```

### d1: overcommit detection

```
  mutualHitLocked = E.att > P.def  AND  P.att > E.def
    AND no die in pool can flip either side's threshold

  mutualHitLocked = true:
  |-- stop taking red beyond the hit threshold (zero gate effect above the breakpoint)
  |-- shift priority to: armpits / hip / head (disruption gates 9, 7, 10)
  +-- prioritize: blue die (gate 15 matters in mutual-hit) and green (wound aim)

  mutualHitLocked = false:
  +-- follow breakpoint-first policy (d2)
```

### d2: breakpoint-first policy

```
  prefer a die that completes a critical threshold over a die that inflates an already-won stat:

  1. fatal-line enabling: E.att + die > P.def  AND  E.aim >= 7  --> neck bleed-out line opens
  2. hit enabling:     E.att + die > P.def  (first time)     --> any wound possible
  3. order enabling:   E.spd + die > P.spd  (first time)     --> acts first
  4. armpits enabling: E.aim + die >= 6                      --> removes P red
  5. head enabling:    E.aim + die >= 4                      --> discards P best die
  6. survive enabling: E.def + die >= P.att                  --> takes no wound

  a small die completing breakpoint 1 beats a large die inflating an already-won stat.

  example:
    E.aim = 6 (neck not yet accessible WITHOUT this die)
    green-1 has zero aim-breakpoint value
    red-3 that completes the hit threshold beats the green-1
```

### d3: yellow handling in draft

```
  yellow breaks ties against same-sized non-yellow dice unless hatchet is active

  hatchet exception:
    LosesValueToHatchet = true drops yellow below non-yellow at d0 step 7
    but yellow STILL denies the player a large die (DeniesPlayer* gates still apply)
```

### d4: guts wound compensation in draft

```
  guts wound: all drafted dice arrive at value - 1 (DecreaseDiceValue called on attach)
  if the drafted die is a `1`, it fades out immediately and contributes zero self-value
  draft preview must score that die as zero for the enemy's own plan

  to reach E.att = X: draft a die with face value X + 1 (pre-decrement)

  if breakpoints become unreachable after the guts decrement:
    stop allocating red; shift to disruption (green aim for armpits/hip/head)
```

### d5: item-aware draft adjustments

```
  armor active:
    gate 2 (EnemyDamagesPlayer) is false when E goes first → blue has low return
    shift to white (defense) or green (wound routing via lower targets)

  dodgy active:
    gate 2 is false for E-second plans where dodgy is still active
    shift to blue if initiative can flip; otherwise shift to white / disabling aim

  maul active:
    gate 3 (EnemyAvoidsKill) = false whenever player can hit → existential threat
    blue and white are top priority (go first or close defense gap)
    red matters only to confirm the hit, not to exceed it

  hatchet active:
    yellow self-value = zero (fades out on attach)
    LosesValueToHatchet = true; non-yellow preferred for self-value
    denial value from yellow still applies (DeniesPlayer* gates)

  spear+legendary:
    player always acts first; EnemyActsFirst gate = false for ALL candidates
    blue has zero gate return; skip blue entirely
    invest in red (attack) and white (defense) only
```

---

## 13) normal mode neck constraint

```
  naturalAim = E.base_green
             + sum(fixed green dice)
             + sum(yellow dice assigned to green)
             (stamina added to green is NOT counted)

  naturalAim >= 7?
  |-- YES --> neck is a legal target; stamina may be spent on other stats freely
  +-- NO  --> neck is illegal; stamina cannot push aim to 7

  spending order (threshold-exact, no excess):

  1. if E.att <= P.def:
       spend = min(P.def - E.att + 1, remainingStamina) --> add to red

  2. if E.att > P.def AND P.spd >= E.spd AND P.att > E.def:
       spend = min(P.spd - E.spd + 1, remainingStamina) --> add to blue

  3. if P.att > E.def AND P.aim >= 0:
       spend = min(P.att - E.def, remainingStamina) --> add to white

  4. do NOT spend on green
       naturalAim alone decides whether neck is legal in normal mode

  never spend more than the exact threshold amount per stat.
```

---

## 14) easy mode micro tree

```
  1. target = highest reachable non-wounded slot from natural aim (no stamina)

  2. red futility check:
     if E.att > P.def           --> skip (hit already confirmed)
     if E.att + stamina <= P.def --> skip (threshold unreachable)

  3. red spend:
     needed = P.def - E.att + 1
     spend  = min(needed, remainingStamina)
     add to red; update effective E.att

  4. white futility check:
     if E.def >= P.att          --> skip (defense already confirmed)
     if P.att > E.def + stamina --> skip (gap uncloseable)
     if P.aim < 0               --> skip (player cannot hit anyway)

  5. white spend:
     needed = P.att - E.def
     spend  = min(needed, remainingStamina)
     add to white

  6. do NOT spend blue or green
  7. do NOT reassign yellow; yellow defaults to red
```

---

## 15) plan commit sequence (ApplyPlan)

```
  1. refund all currently-added enemy stamina --> restore enemy.stamina
  2. clear all yellow dice from addedEnemyDice
  3. assign each yellow die to its stat in plan.YellowAssignments
  4. set addedEnemyStamina values from plan.Stamina
  5. deduct total stamina cost from enemy.stamina
  6. set enemy.targetIndex to plan.TargetIndex
  7. call SummonStats() to recompute displayed stats
  8. call RepositionAllDice() to update visual positions
  9. call SetTargetOf("enemy") to update target display
  10. persist to Save.game.enemyStamina

  ROUND-END CLEANUP (StatSummoner.ResetDiceAndStamina):
    addedEnemyStamina zeroed
    Save.game.enemyStamina = s.enemy.stamina   (base only; no allocation lingers)
    Save.game.playerStamina = s.player.stamina (base only)
    stamina counter UI updated for both sides
    no committed stamina ever persists past the turn
```

---

## 16) robustness analysis

**gate-driven, not stat-sum greedy:**
  every candidate is measured by what it achieves (kills, wounds, avoids, disables).
  raw stat totals only appear as tiebreakers (gates 16-18). a candidate that adds
  3 red stamina when the hit is already confirmed wins nothing at gates 1-15 and
  loses gate 16 (more stamina spent). it will never win.

**order-of-operations is explicit:**
  the initiative branch models actual TurnManager.cs resolution:
  first mover's wound applies before the second mover attacks.
  the evaluation sees post-wound stats, not pre-wound assumptions.

**wound consequences are simulated before target selection:**
  ApplyWoundToPlayer and ApplyWoundToEnemy are called on cloned states.
  the gate values reflect the post-wound board.

**truly futile stamina is discarded before comparison:**
  IsTrulyFutileAdvancedEvaluation removes plans that spend stamina but satisfy zero gates.
  the enemy never commits resources to actions that change nothing.

**chest is a conditional fallback, not a default:**
  UsesChestOnHighValuePlayerDice and UsesChestAsLastDitchGamble are only true under
  specific conditions. chest ranks below all disabling wounds and below acting first.
  it only wins when no better option exists and the player has dice worth disrupting.

**retroactive replanning handles mid-round disruption:**
  RefreshEnemyPlanIfNeeded fires on board changes.
  on easy and hard, the visible plan is rebuilt from actual state, not stale memory.

**difficulty identity is preserved:**
  - easy:          threshold arithmetic only; teaches the combat rules; live stamina plan once drafting is done
  - normal:        legacy feel; hidden intent; no stamina-funded neck spike
  - hard:          full gate search; visible live plan during draft
  - nightmare:     full gate search; hidden plan with attack-time reveal animation

**primitives generalize to new content:**
  every decision reduces to: can hit? can kill? survive reply? disable reply? cheapest path?
  new items expand the flag set in SimState without restructuring the tree.

**stamina is a long-term resource:**
  the futility filter enforces hard stops before the comparator runs.
  gate 16 prefers fewer stamina spent for any equal outcome.
  the result: stamina is only ever spent when it produces a gate change.

---

## 17) planner performance status

this document is the behavior spec first, but the current planner also has a few
performance guardrails that are now part of the implementation contract unless a
future optimization replaces them safely.

**currently implemented safeguards:**
- advanced-plan caching keyed by full combat-state hash
- target recomputation suppressed while `ApplyPlan` is mutating board state
- extra post-plan enemy debug refresh removed from `RunEnemyCalculations`
- profiler summary includes cache effectiveness as `cache=hits/total`
- debug text refresh no longer calls `TargetBest` implicitly
- combat-state ui updates use an explicit deferred refresh path instead of synchronous replans hidden inside debug helpers
- advanced planning snapshots board facts once per build instead of rereading live state for every candidate
- advanced mode has a zero-resource fast path when only target choice can vary
- development logging for planner profiling is sampled so every build does not spam the console
- resume-from-save dice restoration batches combat refresh requests until all saved dice are placed
- planning is skipped while a new fight has no dice in play yet, so trader transitions cannot pre-spend stamina before drafting starts

**current observed timings after those fixes:**
- cold advanced build can still land around `20.586ms` on a `yellow=1`, `candidates=32` case
- warmed rebuilds can drop near `3.475ms` once cache reuse starts helping
- heavier `yellow=4`, `candidates=96` cases still land around `11.360ms`

**current interpretation:**
- the previous recursive replan loop was real and is now reduced
- cache reuse proves some repeated replans are being avoided
- debug-only ui refreshes are no longer allowed to trigger planner work by accident
- remaining stalls are now more likely to come from either:
  - true cache misses inside `BuildAdvancedPlan`
  - too many valid refresh triggers firing in quick succession

**memory note:**
- hashes do not accumulate across the run in an unbounded container
- the live advanced cache stores only one cached plan and one cache key at a time
- profiler counters grow numerically across the session, but they remain constant-space fields

**optimization policy:**
- preserve sections 1-16 exactly unless this spec is updated first
- prefer fewer planner invocations before attempting risky search rewrites
- profile both `EnemyAI.BuildAdvancedPlan` and refresh call frequency
- only hard keeps enemy intent visible; batching must not leave that hard-mode view stale beyond the accepted refresh window

the active optimization backlog and implementation ideas live in `OPTIMIZATIONS.md`.

---

## 18) regression scenarios

these cases are now part of the behavior contract.

### r1: same-color draft ties must still deny the better player die

if two draft candidates lead to the same strategic post-pick plan, the enemy must not let
`SpentStamina`, `TotalOverspend`, or `ResourceOverspend` decide the draft before considering
how much value is being left to the player.

example shape:
- skeleton is hip-wounded, so future stamina is unusable
- both `r4` and `r6` keep the same immediate outcome gates for the enemy
- a full `IsBetterAdvancedEvaluation(BestPlan)` tie would incorrectly prefer `r4` only because
  the resulting plan has less overspend
- that is wrong because it leaves `r6` for the player, which is strictly higher denial loss

draft policy update:
- compare post-pick plan outcome gates first
- then compare denial gates (`DeniesPlayerKill`, `DeniesPlayerDamage`, etc)
- then compare the value denied to the player
- only after those are tied may overspend/resource tie-breaks decide the draft

result: if the enemy is already taking a red die here, it must take `r6`, not `r4`

### r2: enemy-first chest must open a rescue window before the player reply

when the enemy attacks first and inflicts `chest` on the player, the combat flow must pause
before the player counterattacks and allow the enemy to reroll its own dice >= 3 exactly like
the existing chest rescue logic.

requirements:
- this rescue window only matters if the player's pending counterattack still damages the enemy
- the reroll search should prefer outcomes that break a kill line before outcomes that only break damage
- after the rerolls finish, the turn resumes and the player attack is re-evaluated from live stats

result: enemy-first chest can legitimately convert a guaranteed player hit into a parry if a
white or yellow-to-white reroll finds a safe defense line

### r3: enemy-first head must discard before the player reply

when the enemy attacks first and inflicts `head` on the player, the head discard must resolve in
an explicit pre-counterattack pause before the player attacks.

requirements:
- the discard is deferred into the same rescue window used for chest-time counterattack prevention
- the discard still happens in the same round; it is only delayed so the turn flow clearly halts,
  applies the removal, and then resumes with updated player stats
- the chosen die uses the live head-discard comparator (`BreaksKill`, `BreaksDamage`,
  `BreaksGoFirst`, `BreaksTarget`, `RestoresDefense`, `IsYellow`, `DieValue`)

result: if discarding the player's `y5` is what stops the reply, the turn manager must perform
that discard before `PlayerAttacks()` is resolved
