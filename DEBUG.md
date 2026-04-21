# debug scenarios

note for future agents
- keep this exact format
- for each case, trace the actual `if` conditions the chosen path uses in `EnemyAI.cs`
- write each condition as `if: <condition> -> true/false` and then explain why
- when comparing a bad line against a good line, explicitly state which gate flips and why
- keep two blank lines between stat lines so markdown preview separates them cleanly
- write dice as `g5`, `b1`, `r6`, `w2`, `y4`
- write stamina as `1 s`, `2 s`

---

## case 0 - bad observed line

PLAYER: 7 / 3 / 7 / 2

accuracy: 7 = (2 + g5)


speed: 3 = (2 + b1)


damage: 7 = (1 + r6)


parry: 2 = (2)


targeting: armpits


ENEMY: 9 / 2 / 7 / 3

accuracy: 9 = (1 + y4 + y3 + y1)


speed: 2 = (2)


damage: 3 = (2 + 1 s)


parry: 1 = (1)


stamina: 1 remaining


targeting: head


`EnemyAI.cs TRACE RESULT:`

if: `canUseStamina = !enemyHipWound || enemyIsLich` -> true
- the enemy is not hip-wounded, so the planner is allowed to search stamina splits

if: `maxAim = enemyBaseAim + fixedGreen + yellowGreen + staminaGreen` -> `1 + 0 + 8 + 0 = 9`
- this plan makes targets `0..7` legal because aim is above neck threshold

if: `enemyActsFirst = EnemySpeedLockedHigh || (!PlayerSpeedLockedHigh && E.spd > P.spd)` -> false
- `2 > 3` is false, so the enemy does not go first

if: `playerCanHitBefore = PlayerAim >= 0 && PlayerAtt > EnemyDef` -> true
- `7 >= 0` and `7 > 1`

if: `playerDamagesBefore = PlayerHitDamagesEnemy(...)` -> true
- the player can hit, the target is armpits, and armpits is not already wounded

if: `playerKillsBefore = PlayerWouldKillEnemy(...)` -> false
- the player is not targeting neck and this is not a third-wound kill state

if: `enemyCanHitBefore = EnemyAim >= 0 && EnemyAtt > PlayerDef` -> true
- `9 >= 0` and `3 > 2`

if: `enemyDamagesBefore = EnemyHitAppliesWound(..., enemyActsFirst)` -> true at precheck
- the raw hit is valid and the target is legal
- this does not save the plan because the enemy still acts second

if: `enemyKillsBefore = EnemyWouldKillPlayer(...) && enemyDamagesBefore` -> false
- the target is head, not neck, and this is not a third-wound kill state

if: `enemyActsFirst` branch -> false, so branch B is taken
- player acts first branch

if: `playerDamagesBefore && !EnemyIsLich` -> true
- the player wounds first, so `ApplyWoundToEnemy(armpits)` is executed

if inside `ApplyWoundToEnemy`: `case "armpits"` -> taken
- enemy red dice are removed before the enemy attacks
- this is the key failure point of the bad line

if: `enemyCanHitAfterPlayer = !playerKillsBefore && afterPlayerHit.EnemyAim >= 0 && afterPlayerHit.EnemyAtt > afterPlayerHit.PlayerDef` -> false
- after the armpits wound, the enemy loses its red contribution
- the planned `3 > 2` line no longer survives the player’s first action

if: `enemyDamagesAfterPlayer` -> false
- no surviving hit remains after the player’s wound

if: `enemyKillsAfterPlayer` -> false
- no hit means no kill

if: `evaluation.EnemyDamagesPlayer = enemyDamagesAfterPlayer` -> false
- this bad line fails gate 2

if: `evaluation.EnemyAvoidsDamage = !playerDamagesBefore` -> false
- the player definitely damaged the enemy first

if: `evaluation.EnemyAvoidsKill = !playerKillsBefore` -> true
- the player did not kill, but only wound

why this line is wrong
- it over-invests aim far past head threshold
- it never clears the speed breakpoint
- because it fails the speed breakpoint, the player’s armpits wound lands first
- once armpits lands first, the enemy’s damage line collapses
- all that extra accuracy changes no winning gate

result
- player wounds first at armpits
- enemy attack fails
- player keeps the r6

---

## case 1 - correct minimum-resource head line against one r6

PLAYER: 7 / 3 / 7 / 2

accuracy: 7 = (2 + g5)


speed: 3 = (2 + b1)


damage: 7 = (1 + r6)


parry: 2 = (2)


targeting: armpits


ENEMY: 5 / 5 / 3 / 1

accuracy: 5 = (1 + y4)


speed: 5 = (2 + y3)


damage: 3 = (2 + y1)


parry: 1 = (1)


stamina: 2 remaining


targeting: head


`EnemyAI.cs TRACE RESULT:`

if: `canUseStamina = !enemyHipWound || enemyIsLich` -> true
- full search is allowed

if: `maxAim = 1 + 4 + 0 = 5` -> true for targets `0..5`
- head is legal because target index 4 is within max aim 5
- neck is not legal because 5 is below 7

if: `enemyActsFirst = EnemySpeedLockedHigh || (!PlayerSpeedLockedHigh && E.spd > P.spd)` -> true
- `5 > 3` is true, so branch A is taken

if: `playerCanHitBefore = PlayerAim >= 0 && PlayerAtt > EnemyDef` -> true
- `7 >= 0` and `7 > 1`

if: `playerDamagesBefore = PlayerHitDamagesEnemy(...)` -> true
- if allowed to reply, the player would wound at armpits

if: `playerKillsBefore = PlayerWouldKillEnemy(...)` -> false
- not neck and not a third-wound kill state

if: `enemyCanHitBefore = EnemyAim >= 0 && EnemyAtt > PlayerDef` -> true
- `5 >= 0` and `3 > 2`

if: `enemyDamagesBefore = EnemyHitAppliesWound(..., enemyActsFirst)` -> true
- no armor, no dodgy block, and head is not already wounded

if: `enemyKillsBefore = EnemyWouldKillPlayer(...) && enemyDamagesBefore` -> false
- head does not kill on its own here

if: `enemyActsFirst` branch -> true, so branch A is taken

if: `enemyDamagesBefore` -> true
- `ApplyWoundToPlayer(head)` is executed before the player can attack

if inside `ApplyWoundToPlayer`: `case "head"` -> taken
- `ApplyBestPlayerDiscard()` runs

if inside discard scoring: removing `r6` breaks the player hit line -> true
- the player drops from `7` damage to `1`
- that makes `1 > 1` false against enemy parry
- this is why head is correct against a single `r6`

if: `playerCanHitAfter = afterEnemyHit.PlayerAim >= 0 && afterEnemyHit.PlayerAtt > afterEnemyHit.EnemyDef` -> false
- `7 >= 0` is still true, but `1 > 1` is false

if: `playerDamagesAfter` -> false
- no surviving hit

if: `playerKillsAfter` -> false
- no surviving hit means no kill

if: `evaluation.EnemyDamagesPlayer = enemyDamagesBefore` -> true
- gate 2 is satisfied

if: `evaluation.EnemyAvoidsDamage = !playerDamagesAfter` -> true
- gate 4 is satisfied

if: `evaluation.EnemyAvoidsKill = !playerKillsAfter` -> true
- gate 3 is satisfied

if: `evaluation.RemovesPlayerBestDie` -> true
- head removed one attached player die, and it was the best one

if: `IsBetterAdvancedEvaluation(candidate, badAccuracyDumpPlan)` -> true
- both lines can reach a wound target, but this line flips the important gate by acting first
- this line gets `EnemyDamagesPlayer = true` and `EnemyAvoidsDamage = true`
- the bad line lost gate 2 after the player’s first strike
- this line also has lower overspend and lower resource waste

why this line is correct
- y4 is the smallest die that reaches head from base aim 1
- y3 is the smallest remaining die that clears speed `> 3`
- y1 is the smallest remaining die that clears damage `> 2`
- no stamina is required, so it is conserved

result
- enemy goes first
- enemy hits head
- player loses `r6`
- player counterattack fails

---

## case 2 - player has three r1 dice, so armpits beats head

PLAYER: 7 / 3 / 7 / 2

accuracy: 7 = (2 + g5)


speed: 3 = (2 + b1)


damage: 4 = (1 + r1 + r1 + r1)


parry: 2 = (2)


targeting: armpits


enemy: 6 / 5 / 3 / 1

accuracy: 6 = (1 + y4 + 1 s)


speed: 5 = (2 + y3)


damage: 3 = (2 + y1)


parry: 1 = (1)


stamina: 1 remaining


targeting: armpits


enemyai trace

if: `canUseStamina` -> true
- stamina search is available

if: `maxAim = 1 + 4 + 1 = 6` -> true for targets `0..6`
- armpits is legal because it needs index 6
- neck is not legal because 6 is still below 7

if: `enemyActsFirst` -> true
- `5 > 3`

if: `playerCanHitBefore` -> true
- `7 >= 0` and `4 > 1`

if: `playerDamagesBefore` -> true
- the player would damage if allowed to act

if: `enemyCanHitBefore` -> true
- `6 >= 0` and `3 > 2`

if: `enemyDamagesBefore` -> true
- armpits is legal and the hit lands

if: `enemyKillsBefore` -> false
- armpits is not neck and not a third-wound kill state

if: `enemyActsFirst` branch -> true
- branch A is taken again

if: `enemyDamagesBefore` -> true
- `ApplyWoundToPlayer(armpits)` runs first

if inside `ApplyWoundToPlayer`: `case "armpits"` -> taken
- all player red dice are removed

if: `playerCanHitAfter` -> false
- player damage falls from `4` to `1`
- `1 > 1` is false against enemy parry 1

if: `playerDamagesAfter` -> false
- no surviving hit

if: `evaluation.RemovesPlayerRed` -> true
- this is the key gate that head could not satisfy here

if comparing against a head plan: `RemovesPlayerBestDie` for head -> true, but `RemovesPlayerRed` -> false
- head removes only one `r1`
- player would still have damage `3`
- `3 > 1` would still hit the enemy
- armpits therefore wins the comparison because it flips the player damage result while head does not

if: `evaluation.EnemyDamagesPlayer` -> true
- enemy still lands the wound

if: `evaluation.EnemyAvoidsDamage` -> true
- the player no longer has a hit after armpits removes all red dice

why this line is correct
- y4 alone only reaches aim 5, which is enough for head but not enough for armpits
- adding `1 s` to aim reaches the exact armpits breakpoint at 6
- that stamina spend is necessary because it changes the outcome gate from player still damages to player no longer damages
- this is an allowed stamina spend because it changes the result, not just the number

result
- enemy goes first
- enemy hits armpits
- all player red dice are removed
- player counterattack fails

---

## case 3 - if 2 stamina are committed to accuracy, neck becomes the best fatal line

PLAYER: 7 / 3 / 7 / 2

accuracy: 7 = (2 + g5)


speed: 3 = (2 + b1)


damage: 7 = (1 + r6)


parry: 2 = (2)


targeting: armpits


enemy: 7 / 5 / 3 / 1

accuracy: 7 = (1 + y4 + 2 s)


speed: 5 = (2 + y3)


damage: 3 = (2 + y1)


parry: 1 = (1)


stamina: 0 remaining


targeting: neck


enemyai trace

if: `canUseStamina` -> true

if: `maxAim = 1 + 4 + 2 = 7` -> true for targets `0..7`
- neck becomes legal exactly at 7

if: `enemyActsFirst` -> true
- `5 > 3`

if: `enemyCanHitBefore` -> true
- `7 >= 0` and `3 > 2`

if: `enemyDamagesBefore` -> true
- neck is legal and the hit lands

if: `enemyKillsBefore = EnemyWouldKillPlayer(neck) && enemyDamagesBefore` -> true
- current planner shorthand treats neck as a fatal line once accuracy reaches 7 and the hit lands
- the actual combat death resolves next round through bleed-out, not immediately

if: `enemyActsFirst` branch -> true
- branch A is taken

if: `evaluation.EnemyKills` -> true
- gate 1 is satisfied, which outranks every non-kill line

if comparing against head or armpits lines: `candidate.EnemyKills != current.EnemyKills` -> true
- the neck fatal line wins immediately in `IsBetterAdvancedEvaluation`
- no lower tiebreak matters after gate 1 flips true

if: stamina spend is necessary -> true
- without `2 s` on aim, enemy aim is only 5
- target neck would be illegal
- the stamina is justified because it upgrades the plan from wound to outright kill

- in runtime terms, it upgrades the plan from a normal wound to guaranteed bleed-out pressure

why this line is correct
- y4 is still the best green die because it gets closest to neck
- y3 still must go to speed to preserve first move
- y1 still must go to damage to clear parry
- the extra `2 s` is the exact amount needed to make neck legal
- because this creates `EnemyKills = true`, this line is better than any stamina-saving non-kill line

result
- enemy goes first
- enemy hits neck
- player survives the round but is on a guaranteed bleed-out line next round

---

## summary of the gate changes that matter

bad observed line
- `EnemyActsFirst = false`
- `EnemyDamagesPlayer = false` after the player’s first strike
- extra aim is wasted

correct head line vs one `r6`
- `EnemyActsFirst = true`
- `EnemyDamagesPlayer = true`
- `EnemyAvoidsDamage = true`
- `RemovesPlayerBestDie = true`

correct armpits line vs three `r1`
- `EnemyActsFirst = true`
- `EnemyDamagesPlayer = true`
- `EnemyAvoidsDamage = true`
- `RemovesPlayerRed = true`

correct neck line when 2 stamina are available
- `EnemyActsFirst = true`
- `EnemyDamagesPlayer = true`
- `EnemyKills = true`
- gate 1 ends the comparison immediately

---

## case 4 - guts-wounded gog should not spend `1 s` on blue for chest

PLAYER: 7 / 4 / 8 / 8

accuracy: 7 = (1 + g6)


speed: 4 = (4)


damage: 8 = (3 + r5)


parry: 8 = (3 + w5)


stamina: 2 remaining


wounds: none


targeting: neck


available dice: none


ENEMY: 2 / 5 / 2 / 4

accuracy: 2 = (2)


speed: 5 = (3 + b1 + 1 s)


damage: 2 = (2)


parry: 4 = (4)


stamina: 0 remaining


wounds: guts


targeting: chest


`EnemyAI.cs TRACE RESULT:`

if: `enemyActsFirst = EnemySpeedLockedHigh || (!PlayerSpeedLockedHigh && E.spd > P.spd)` -> true
- `5 > 4` is true, so the blue spend does flip initiative

if: `playerCanHitBefore = PlayerAim >= 0 && PlayerAtt > EnemyDef` -> true
- `7 >= 0` and `8 > 4`

if: `playerKillsBefore = PlayerWouldKillEnemy(...)` -> true
- the player is targeting neck at aim 7, so the current planner treats this as a fatal line

if: `enemyCanHitBefore = EnemyAim >= 0 && EnemyAtt > PlayerDef` -> false
- `2 >= 0` is true, but `2 > 8` is false
- this is the key point: the enemy cannot wound at all

if: `enemyDamagesBefore = EnemyHitAppliesWound(..., enemyActsFirst)` -> false
- no hit means no wound, regardless of moving first

if: `enemyKillsBefore = EnemyWouldKillPlayer(...) && enemyDamagesBefore` -> false
- chest is not neck, and there is no landed hit anyway

if: `enemyActsFirst` branch -> true
- branch A is taken, but it still cannot create a wound because `enemyDamagesBefore` is false

if: `evaluation.EnemyDamagesPlayer = enemyDamagesBefore` -> false
- gate 2 stays false

if: `evaluation.EnemyAvoidsKill = !playerKillsAfter` -> false
- the player still has the neck fatal line after the enemy's non-hit

if: `evaluation.EnemyAvoidsDamage = !playerDamagesAfter` -> false
- the player still damages and kills

if: `evaluation.UsesChestOnHighValuePlayerDice = enemyTarget == "chest" && evaluation.EnemyDamagesPlayer && PlayerHasHighValueDice(state)` -> false
- the player does have high-value dice, but chest is only meaningful if the chest wound actually lands
- because `EnemyDamagesPlayer` is false, chest gets no gate credit here

if: `evaluation.UsesChestAsLastDitchGamble = enemyTarget == "chest" && evaluation.EnemyDamagesPlayer && ...` -> false
- same reason: no landed wound means no chest effect

if: `IsTrulyFutileAdvancedEvaluation(candidate)` -> true
- the plan spends stamina
- it satisfies none of the meaningful gates
- and the chest gates no longer mask that futility

why the old line was wrong
- the planner was allowing chest preference gates to turn on even when the enemy could not land a chest wound
- once that happened, spending `1 s` on blue could look superficially "better" only because it added `EnemyActsFirst`
- but acting first without any attack line does not change the outcome against the player's neck fatal line

why the fixed line is correct
- chest preference now requires an actual landed chest wound
- since Gog cannot beat player parry 8 here, chest has zero gate value
- the blue spend is therefore truly futile and gets rejected

result
- the enemy should not spend `1 s` on blue here
- if no better damaging line exists, it should keep that stamina

---

## case 5 - guts-wounded gog should take `r6`, not `y1`

PLAYER: 1 / 7 / 3 / 3

accuracy: 1 = (1)


speed: 7 = (4 + b3)


damage: 3 = (3)


parry: 3 = (3)


stamina: 2 remaining


wounds: none


targeting: chest


available dice: r1, r6, w3, w3


enemy: Gog


ENEMY: 2 / 4 / 2 / 4

accuracy: 2 = (2)


speed: 4 = (3 + y1)


damage: 2 = (2)


parry: 4 = (4)


stamina: 0 remaining


wounds: guts


targeting: knee


`EnemyAI.cs TRACE RESULT:`

if: `effectiveEnemyValue(y1) = max(0, 1 - 1)` -> `0`
- Gog is guts-wounded, so any drafted die loses 1 pip on attach
- a drafted `y1` therefore fades out immediately and has zero self-value

if: `GetDraftPreviewEvaluation(y1, anyStat)` -> current-board evaluation only
- because the effective drafted value is `0`, the preview must treat `y1` as adding nothing to Gog's own plan

if: `enemyCanHitBefore` on the current board -> false
- current enemy attack is `2`
- player parry is `3`
- `2 > 3` is false, so Gog has no damaging line before taking a new die

if: `BestPlan(y1).EnemyDamagesPlayer` -> false
- the yellow 1 does not survive guts, so it cannot create a hit threshold

if: `BestPlan(y1).EnemyAvoidsDamage` -> true
- player damage is `3` and enemy parry is `4`
- the player already cannot damage Gog, so the yellow 1 adds nothing new here

if: `effectiveEnemyValue(r6) = max(0, 6 - 1)` -> `5`
- the drafted `r6` becomes `r5` after the guts penalty
- it still contributes real attack value

if: `enemyAtt after r6 = 2 + 5 = 7` -> true
- `7 > 3` against player parry, so the red die creates a live wound line

if: `enemyActsFirst` after r6 -> false
- speed stays `3`, so Gog still acts second against player speed `7`
- that does not matter here because the player cannot damage Gog anyway

if: `playerCanHitBefore = PlayerAim >= 0 && PlayerAtt > EnemyDef` -> false
- `1 >= 0` is true, but `3 > 4` is false
- the player has no hit line to punish Gog for acting second

if: `enemyActsFirst` branch -> false, so branch B is taken
- player acts first branch

if: `playerDamagesBefore && !EnemyIsLich` -> false
- no player wound is applied before Gog attacks

if: `evaluation.EnemyAvoidsDamage = !playerDamagesBefore` -> true
- Gog already keeps gate 4 because the player cannot hit

if: `evaluation.EnemyDamagesPlayer = enemyCanHitAfterPlayer` -> true for `r6`
- after taking `r6`, Gog can still hit for `7 > 3`
- this flips gate 2, which the `y1` line never reaches

if: `IsBetterAdvancedEvaluation(BestPlan(r6), BestPlan(y1))` -> true
- both lines keep `EnemyAvoidsDamage = true`
- only the `r6` line also gets `EnemyDamagesPlayer = true`
- gate 2 beats any tie on lower criteria

why the old line was wrong
- draft preview was scoring enemy self-value from the raw die face, not the post-guts attached value
- that let a drafted `y1` look like flexible value even though it should immediately vanish

why the fixed line is correct
- draft preview now evaluates enemy-side attach penalties first
- under guts, `y1` is treated as zero self-value
- `r6` still becomes a real `r5`, which creates the only actual damage line

result
- Gog should take `r6`
- `y1` should lose because it creates no new gate at all

---

## case 6 - goblin should take `y5`, not a low-value non-kill die

PLAYER: 3 / 3 / 8 / 3

accuracy: 3 = (3)


speed: 3 = (3)


damage: 8 = (2 + r6)


parry: 3 = (3)


stamina: 3 remaining


wounds: none


targeting: chest


available dice: y5, r5, w1, w4


enemy: Goblin


ENEMY: 2 / 3 / 5 / 2

accuracy: 2 = (2)


speed: 3 = (3)


damage: 5 = (3 + r2)


parry: 2 = (2)


stamina: 2 remaining


wounds: none


targeting: knee


`EnemyAI.cs TRACE RESULT:`

if: `GetDraftPreviewEvaluation(y5, green)` runs a full post-pick advanced preview -> true
- draft choice must evaluate the real plan after the pick, not just the raw stat bump with zero stamina
- this is the key bug path that was flattening strong draft choices

if: `enemyAim after y5->green = 2 + 5 = 7` -> true
- neck becomes legal immediately

if: `playerCanHitBefore = PlayerAim >= 0 && PlayerAtt > EnemyDef` -> true
- `3 >= 0` and `8 > 2`
- the player can wound first, but chest is not a kill line

if: `playerKillsBefore = PlayerWouldKillEnemy(...)` -> false
- the player is targeting chest, not neck, and Goblin has no third-wound death set up

if: `enemyCanHitAfterPlayer = !playerKillsBefore && afterPlayerHit.EnemyAim >= 0 && afterPlayerHit.EnemyAtt > afterPlayerHit.PlayerDef` -> true
- after the player's chest wound, enemy stats still support `5 > 3`
- chest does not remove the enemy attack line in the preview model

if: `enemyKillsAfterPlayer = EnemyWouldKillPlayer(neck) && enemyDamagesAfterPlayer` -> true
- with aim `7`, neck becomes a fatal bleed-out line in planner shorthand
- this flips gate 1 for the `y5` line

if: `BestPlan(y5).EnemyKills` -> true
- any draft candidate with gate 1 must beat any non-kill draft candidate

if: `BestPlan(r5).EnemyKills` -> false
- `r5` improves damage, but it does not make neck legal
- it stays a wound line, not a kill line

if: `BestPlan(w4).EnemyKills` -> false
- extra defense may help survival later, but it does not open a kill line here

if: `IsBetterAdvancedEvaluation(BestPlan(y5), BestPlan(r5 or w4 or w1))` -> true
- `EnemyKills = true` on `y5`
- gate 1 beats every non-kill alternative immediately

why the old line was wrong
- draft preview was too shallow and could flatten post-pick value into a near-current-board snapshot
- when that happened, obviously stronger dice were not being credited for the real plan they enabled

why the fixed line is correct
- the draft preview now runs the real post-pick advanced search, including stamina options
- that lets `y5` claim the neck fatal line it actually opens
- once `EnemyKills` is visible in the preview, `y5` must win the draft comparison

result
- Goblin should take `y5`
- any non-kill pickup should lose to that neck fatal line

---

## case 7 - skeleton must not let draft overspend hide player denial

PLAYER: 3 / 3 / 8 / 1

accuracy: 3 = (3)


speed: 3 = (3)


damage: 8 = (3 + y5)


parry: 1 = (1)


stamina: 1 remaining


wounds: none


targeting: chest


available dice: g4, r4, r6, w2


enemy: Skeleton


ENEMY: 2 / 2 / 6 / 2

accuracy: 2 = (2)


speed: 2 = (2)


damage: 6 = (2 + r4)


parry: 2 = (2)


stamina: 2 remaining


wounds: hip


targeting: knee


`EnemyAI.cs TRACE RESULT:`

if: `canUseStamina = !enemyHipWound || enemyIsLich` -> false
- Skeleton is hip-wounded, so the draft preview must not assume any future stamina rescue

if: `IsBetterDraftPlanPreview(BestPlan(r4), BestPlan(r6))` -> false
- both red picks keep the same strategic gates in this spot
- neither pick changes initiative, survival, or the player's chest line yet

if: `DeniesPlayerKill(r4) != DeniesPlayerKill(r6)` -> false
- neither red pick removes a binary player kill line here

if: `DeniesPlayerDamage(r4) != DeniesPlayerDamage(r6)` -> false
- the player still damages either way

if: `PlayerDenialScore(r6) > PlayerDenialScore(r4)` -> true
- leaving `r6` to the player is strictly worse than leaving `r4`
- this is the real tie-break that matters once the enemy has already decided it wants red

if in the old code: `IsBetterAdvancedEvaluation(BestPlan(r4), BestPlan(r6))` -> true
- this was the bug
- the full advanced comparator reached stamina and overspend tie-breaks too early
- `r4` looked "cleaner" only because it produced less `ResourceOverspend`
- that let a smaller red die beat a larger red die before player-denial value was considered

if: `candidate.DieValue != current.DieValue` -> reached only after denial ties now
- once the strategic plan and denial gates are tied, the larger same-color die wins

why the old line was wrong
- the draft comparator was letting post-pick overspend break the tie before it asked which die was safer to deny from the player
- in a hip-wound state, that was especially wrong because the enemy cannot spend later stamina to make up for the lost draft value

why the fixed line is correct
- draft comparison now resolves the strategic plan first, then denial, then denied player value
- only after those are tied may overspend/resource tie-breaks matter
- that means `r6` beats `r4` in this scenario for the right reason, not because of a hardcoded same-color rule

result
- Skeleton should take `r6`
- `r4` must no longer win just because its preview plan wastes fewer points on paper

---

## case 8 - enemy-first chest must pause for the rescue reroll

PLAYER: 3 / 3 / 4 / 1

accuracy: 3 = (3)


speed: 3 = (3)


damage: 4 = (r4)


parry: 1 = (1)


wounds: chest


targeting: chest


ENEMY: some live white-side die >= 3 exists

accuracy: unchanged


speed: enemy already acted first


damage: irrelevant to the rescue step


parry: 2 before reroll


`TurnManager.cs TRACE RESULT:`

if: `toMove == "player"` in `RoundTwo()` -> true
- the enemy already attacked first, so the player reply is about to begin

if: `HandleEnemyCounterattackRescuesBeforePlayerAttack()` -> entered
- this is now the single pre-reply rescue window

if: `s.player.woundList.Contains("chest")` -> true
- the player was just hit in the chest, so enemy chest rescue is live

if: `PlayerWouldDamageEnemyNow()` -> true
- the pending player reply is `4 > 2`
- there is a real threat to rescue against

if: `EnemyHasChestRescueReroll()` -> true
- at least one enemy die with face `>= 3` has a reroll branch that can stop the player damage line

if inside `TryGetBestEnemyChestRescueDie`: `playerKillsNow && !PlayerWouldKillEnemyWithRerolledDieValue(...)` -> false
- this case is only about breaking damage, not breaking a kill line

if inside `TryGetBestEnemyChestRescueDie`: `!PlayerWouldDamageEnemyWithRerolledDieValue(dice, rerolledValue)` -> true for some reroll outcome
- the rescue window is justified because a reroll can turn the reply into a parry

if: `while (PlayerWouldDamageEnemyNow() && TryGetBestEnemyChestRescueDie(...))` -> true
- turn flow halts here until the reroll sequence finishes

why the old line was wrong
- the enemy-first chest interaction was not guaranteed to get its own explicit rescue pause before the player counterattack
- that made the turn flow brittle in exactly the spot where chest is supposed to matter

why the fixed line is correct
- the pre-counterattack rescue window now runs before `PlayerAttacks()`
- the rerolls resolve from live stats, then the player reply is checked again

result
- enemy-first chest now pauses the turn
- the enemy gets its chest reroll chance before the player's counterattack resolves

---

## case 9 - enemy-first head must discard before the player reply

PLAYER: 3 / 3 / 5 / 1

accuracy: 3 = (3)


speed: 3 = (3)


damage: 5 = (y5)


parry: 1 = (1)


wounds: head


targeting: chest


ENEMY: 3 / 4 / 4 / 3

accuracy: 3 = (3)


speed: 4 = (4)


damage: 4 = (4)


parry: 3 = (3)


`TurnManager.cs TRACE RESULT:`

if: `enemyAttackedFirst` -> true
- the enemy already landed the head wound before the player reply begins

if: `pendingEnemyHeadCounterDiscard = enemyAttackedFirst && s.enemy.target.text == "head"` -> true
- the discard is queued for the rescue window instead of firing invisibly in the background

if inside `ApplyInjuriesDuringMoveCoro`: `injury == "head" && appliedTo == "player" && !pendingEnemyHeadCounterDiscard` -> false
- this prevents a hidden early discard
- the discard is deferred so the turn manager can halt cleanly first

if: `HandleEnemyCounterattackRescuesBeforePlayerAttack()` -> entered
- this runs before `PlayerAttacks()`

if: `pendingEnemyHeadCounterDiscard` -> true
- the rescue window resolves the discard first

if inside `EnemyAI.GetBestPlayerDieToDiscard(...)`: `BreaksDamage` on `y5` -> true
- removing `y5` drops the player's attack below enemy parry `3`
- that makes the yellow die the correct discard target

if: `playerCanHitAfterDiscard = PlayerAtt > EnemyDef` -> false
- after the discard, the reply no longer clears defense

why the old line was wrong
- head discard could resolve off to the side instead of becoming an explicit stop in the enemy-first counterattack flow
- that made the important save happen less clearly than chest rerolls do

why the fixed line is correct
- the head discard now uses the same pre-counterattack rescue window as chest
- the turn pauses, removes the best player die, then resumes from the updated board

result
- enemy-first head now halts the turn, discards the saving die, and only then allows the player reply to continue

---

## case 10 - enemy-first defense search must include charm-buffed counterattacks

PLAYER: 3 / 3 / 4 / 2

accuracy: 3 = (3)


speed: 3 = (3)


damage: 4 = (4)


parry: 2 = (2)


wounds: none


charms: inevitable + vindictive


targeting: chest


ENEMY: 4 / 4 / 3 / 2

accuracy: 4 = (4)


speed: 4 = (4)


damage: 3 = (3)


parry: 2 = (2)


stamina: 4 remaining


targeting: head


`EnemyAI.cs TRACE RESULT:`

if: `enemyActsFirst = EnemySpeedLockedHigh || (!PlayerSpeedLockedHigh && E.spd > P.spd)` -> true
- `4 > 3` is true, so the enemy acts first

if: `BuildDefenseSpendOptions(..., GetExactDefenseSpendNeeded(...), GetEnemyFirstCounterattackDefenseSpendNeeded(...))` -> includes the larger white candidate
- the old search only considered the raw pre-hit defense threshold
- that missed enemy-first reply buffs from immediate charms

if: `GetEnemyFirstCounterattackDefenseSpendNeeded(...)` -> greater than `GetExactDefenseSpendNeeded(...)`
- inevitable always adds red after an enemy-first swing
- vindictive adds more red if the hit becomes a real wound
- the defense search now keeps the higher of the clean-state and enemy-first counterattack thresholds

if: `whiteSpend` reaches that higher threshold -> true
- the enemy is now allowed to spend enough white stamina to survive the charm-buffed reply

if: `evaluation.EnemyAvoidsDamage = !playerDamagesAfter` -> true on the defended line
- this is the gate the old search was missing because it never generated the needed white-spend candidate

why the old line was wrong
- the advanced search only offered white stamina values for the current visible player attack
- when enemy-first charms increased the player's round-two red, the ai could have spare stamina but never test the actual safe parry total

why the fixed line is correct
- white search now includes the enemy-first immediate-response threshold as well
- if the player reply can be fully defended with stamina, that candidate now exists and can win gate 4

result
- the enemy now spends white stamina when that extra defense is what actually blocks the charm-buffed counterattack

---

## case 11 - forced hit trades may spend green for neck but not for lower wounds

PLAYER: 4 / 5 / 6 / 2

accuracy: 4 = (4)


speed: 5 = (5)


damage: 6 = (6)


parry: 2 = (2)


targeting: chest


ENEMY: 5 / 4 / 3 / 1

accuracy: 5 = (5)


speed: 4 = (4)


damage: 3 = (3)


parry: 1 = (1)


stamina: 2 remaining


targeting options: head with `1 s` green, neck with `2 s` green


`EnemyAI.cs TRACE RESULT:`

if: `playerActsFirst` and `enemyDamagesAfterPlayer` -> true
- the player still wounds first
- the enemy still has a live strike after that
- this is a guaranteed hit trade state

if: `staminaPlan["green"] > 0 && targetIndex < 7 && !evaluation.EnemyKills && evaluation.EnemyDamagesPlayer && !evaluation.EnemyAvoidsDamage` -> true on the non-neck line
- extra green was spent
- the target stayed below neck
- both sides still land damage
- the spend did not create a fatal line

if: `evaluation.UsesAimStaminaForNonFatalTrade` -> true
- this marks the non-neck green-spend trade as strategically futile

if: `IsTrulyFutileAdvancedEvaluation(candidate)` -> true
- the non-neck forced-trade aim spend is discarded before final comparison

if: `targetIndex == 7` and the extra green makes neck legal -> true on the neck line
- the same forced-trade exception does NOT fire here because the green created a fatal line

if: `evaluation.EnemyKills` -> true on the neck line
- gate 1 wins immediately against any non-kill trade

why the old line was wrong
- the ai could burn green stamina on head / armpits even when the round still ended as a simple hit exchange
- that spend did not justify the permanent stamina loss unless it actually converted the trade into a fatal line

why the fixed line is correct
- in a forced trade, extra green stamina is now reserved for fatal conversion only
- if neck is reachable, the ai may and should spend for it
- if only lower wounds are reachable and the hit exchange remains guaranteed, that green spend is rejected

result
- forced hit trades now use green stamina only for neck-level fatal conversion, not for lower non-fatal wound upgrades

---

## case 12 - enemy-first defense search must not let wound-only charm spikes hide reachable parry defense

PLAYER: 3 / 3 / 5 / 11

accuracy: 3 = (3)


speed: 3 = (3)


damage: 5 = (2 + r3)


parry: 11 = (3 + w3 + y5)


stamina: 3 remaining


wounds: none


targeting: guts


ENEMY: 2 / 7 / 5 / 4

accuracy: 2 = (2)


speed: 7 = (2 + b5)


damage: 5 = (2 + r3)


parry: 4 = (2 + w2)


stamina: 6 remaining


targeting: knee


`EnemyAI.cs TRACE RESULT:`

if: `enemyActsFirst = EnemySpeedLockedHigh || (!PlayerSpeedLockedHigh && E.spd > P.spd)` -> true
- `7 > 3` is true, so the enemy acts first

if: `enemyCanHitBefore = EnemyAim >= 0 && EnemyAtt > playerDefenseAgainstEnemy` -> false
- `5 > 11` is false, so this is an enemy-first parry / miss line, not a wound line

if: `GetExactDefenseSpendNeeded(snapshot.PlayerAim, snapshot.PlayerAtt, baseDef)` -> `1`
- the visible player reply is `5` into enemy parry `4`
- even with no immediate charm response, the enemy must at least test `white +1`

if: `GetEnemyFirstParryCounterattackDefenseSpendNeeded(...)` -> evaluated separately
- this models only the enemy-first parry reply branch
- if the player has `inevitable` / `riposte`, this threshold can be higher than `1`
- that higher parry-only threshold must stay in the search as its own white candidate

if: `GetEnemyFirstWoundCounterattackDefenseSpendNeeded(...)` -> evaluated separately
- this models the enemy-first wound reply branch
- vindictive / crystal shard / glass sword only matter on that branch
- in this board state the enemy does not have a live hit, so a wound-only spike must not replace the reachable parry threshold

if in the old code: `whiteOptions = BuildSpendOptions(..., 0, baseNeed, max(parryNeed, woundNeed))` -> wrong
- a larger wound-only threshold could overwrite the smaller reachable parry threshold
- if that wound threshold was above remaining stamina, the search would test `0` and the base visible defense only, but skip the actual parry-response defense amount the enemy could afford

if in the fixed code: `whiteOptions = BuildSpendOptions(..., 0, baseNeed, parryNeed, woundNeed)` -> correct
- the enemy now keeps both enemy-first counterattack thresholds instead of collapsing them into one max value
- reachable parry-defense spends remain legal even when the wound branch is larger or impossible

if: `evaluation.EnemyAvoidsDamage = !playerDamagesAfter` -> true on the defended white candidate
- once the reachable parry threshold is tested, the enemy can select the line that survives the player's reply

why the old line was wrong
- the defense search was using one merged enemy-first counterattack threshold
- that merged value could be dominated by a wound-only charm response the current line never actually triggered
- when that happened, the enemy skipped the reachable white spend that would have defended the real parry reply

why the fixed line is correct
- base defense, parry-response defense, and wound-response defense are now searched separately
- enemy-first no-hit lines can still spend stamina to block the player's actual counterattack
- wound-only charm spikes no longer hide affordable defense lines

result
- the enemy now tests and can choose the reachable white-spend defense line in this scenario instead of refusing to defend

---

## case 13 - hip-wounded gog must not let preview cleanup beat denying `y6`

PLAYER: 0 / 4 / 13 / 0

accuracy: 0 = (0)


speed: 4 = (4)


damage: 13 = (7 + y6)


parry: 0 = (0)


stamina: 0 remaining


wounds: none


targeting: chest


available dice: y6, g2, b4, w4


enemy: Gog


ENEMY: 3 / 2 / 9 / 2

accuracy: 3 = (3)


speed: 2 = (2)


damage: 9 = (4 + r5)


parry: 2 = (2)


stamina: 3 remaining


wounds: hip


targeting: hip


`EnemyAI.cs TRACE RESULT:`

if: `canUseStamina = !enemyHipWound || enemyIsLich` -> false
- Gog is hip-wounded, so the draft preview cannot assume any future stamina rescue

if: `IsBetterDraftPlanPreview(BestPlan(r5), BestPlan(y6))` -> false
- the observed bad draft was the earlier `r5` over `y6` choice that produced this live board
- at the real gate level, neither pick wins by kill / survival / disable alone in a way that should erase denial value

if: `DeniesPlayerKill(r5) != DeniesPlayerKill(y6)` -> false
- neither draft removes a binary player kill line here

if: `DeniesPlayerDamage(r5) != DeniesPlayerDamage(y6)` -> false
- the player still keeps a damage line either way

if: `PlayerDenialScore(y6) > PlayerDenialScore(r5)` -> true
- denying a `y6` removes far more player flexibility than denying a narrower `r5`
- with Gog hip-wounded, that denial matters even more because Gog cannot repair a weak draft later with stamina

if in the old code: `IsBetterAdvancedEvaluation(BestPlan(r5), BestPlan(y6))` -> reached before denied-player value
- this was the bug path
- the full advanced comparator fell through to overspend / resource cleanup too early
- that let the narrower red line look cleaner on paper before the draft asked which die was safer to deny from the player

why the old line was wrong
- hip wound removes Gog's future stamina escape hatch
- once the strategic preview is tied, denying the player a `y6` is more important than keeping a slightly tidier preview plan
- `r5` should not beat `y6` just because the resulting paper plan wastes fewer points

why the fixed line is correct
- the draft comparator now resolves strategic preview gates first, then denial, then denied player value and die strength
- only after those ties are exhausted may full overspend / resource tie-breaks decide the draft
- that keeps `y6` above the narrower `r5` line here

result
- Gog should take `y6`
- paper-cleaner preview waste must no longer beat the stronger denial pick

---

## case 14 - goblin must respect player yellow reassign defense and take `r6`

PLAYER: 0 / 4 / 14 / 0

accuracy: 0 = (0)


speed: 4 = (4)


damage: 14 = (8 + y6)


parry: 0 = (0)


stamina: 0 remaining


wounds: none


targeting: chest


available dice: g4, b1, r6, w1


enemy: Goblin


ENEMY: 2 / 10 / 2 / 2

accuracy: 2 = (2)


speed: 10 = (4 + b6)


damage: 2 = (2)


parry: 2 = (2)


stamina: 4 remaining


wounds: none


targeting: knee


`EnemyAI.cs TRACE RESULT:`

if: `enemyActsFirst = EnemySpeedLockedHigh || (!PlayerSpeedLockedHigh && E.spd > P.spd)` -> true
- `10 > 4` is true, so Goblin already attacks first before taking a new die

if in the old preview: `playerDef` was read only from the current live board -> `0`
- the preview treated the player as if the attached `y6` were locked on red
- that flattened many draft choices into the same immediate hit line because `2 > 0` was already true

if: `GetPlayerYellowReassignmentPreviewOptions()` includes moving the player's `y6` from red to white -> true in the fixed preview
- the player can still reposition attached yellow dice during draft
- the worst-case reply must therefore test the player at `parry = 6`, not only `parry = 0`

if after player yellow reassignment: `enemyAtt without r6 > playerDef` -> false
- the non-`r6` lines no longer force damage once the player moves `y6` to white

if after player yellow reassignment: `enemyAtt with r6 > playerDef` -> true
- `2 + 6 > 6`
- only `r6` preserves the real enemy damage line against the player's flexible defense reply

if: `BestPlan(r6).EnemyDamagesPlayer` -> true under the worst-case player reply
- the red pickup still forces a hit after the yellow move

if: `BestPlan(g4 or b1 or w1).EnemyDamagesPlayer` -> false under the same worst-case player reply
- those picks let the player slide the `y6` into parry and erase the hit

why the old line was wrong
- draft preview only modeled the player's future picked die, not the player's existing movable yellow dice
- that made the preview underrate forced-hit red pickups whenever the player could simply retarget yellow into defense

why the fixed line is correct
- worst-case draft reply preview now enumerates current player yellow reassignments before judging the enemy pick
- that exposes the real defense line the player can create
- once that defense is modeled, `r6` is the only pick that still keeps gate 2

result
- Goblin should take `r6`
- the enemy must now account for player yellow defense reassignments during draft preview

---

## case 15 - gog must take `w6` when tiny yellow only matches the line by burning stamina

PLAYER: 2 / 1 / 4 / 3


accuracy: 2 = (2)


speed: 1 = (1)


damage: 4 = (4)


parry: 3 = (3)


stamina: 3 remaining


wounds: none


targeting: chest


available dice: y1, r4, w6


enemy: Gog


ENEMY: 3 / 0 / 2 / 5


accuracy: 3 = (3)


speed: 0 = (0)


damage: 2 = (2)


parry: 5 = (5)


stamina: 2 remaining


wounds: none


targeting: hip


`EnemyAI.cs TRACE RESULT:`

if: `IsBetterDraftPlanPreview(BestPlan(w6), BestPlan(y1))` -> false
- the full real preview gates tie here
- neither pick wins on kill / survive / damage gates alone

if: `IsBetterDraftPlanPreview(BestPlan(y1), BestPlan(w6))` -> false
- the tiny yellow does not create a stronger real preview than the white die

if: `IsBetterDraftOutcomePreview(BestPlan(w6), BestPlan(y1))` -> false
- the coarse kill / damage / survive bucket also ties

if: `candidate.DeniesPlayerKill != current.DeniesPlayerKill` -> false
- neither pick uniquely blocks an immediate player kill line

if: `candidate.DeniesPlayerDamage != current.DeniesPlayerDamage` -> false
- neither pick uniquely flips the raw player-damage gate before deeper tie-breaks

if in the fixed comparator: `candidateSpentStamina != currentSpentStamina` -> true for `w6` vs `y1`
- the `w6` line reaches the same real preview result with less stamina
- the `y1` line only keeps up because the planner can spend permanent stamina later

if in the old comparator: `candidate.DeniesPlayerGoFirst != current.DeniesPlayerGoFirst` -> false
- this soft denial flag did not separate the lines

if in the old comparator: `candidate.DeniesPlayerTarget != current.DeniesPlayerTarget` -> false
- this soft denial flag also tied

if in the old comparator: `!Mathf.Approximately(candidate.PlayerDenialScore, current.PlayerDenialScore)` -> true for `y1`
- `GetPlayerDieDesireScore(...)` gives yellow a flat desirability bump
- that let the weak yellow look like the better denial pick even though it was the less efficient self-securing line

if in the fixed comparator: `candidateSpentStamina < currentSpentStamina` -> true for `w6`
- `w6` now wins before soft denial scoring is allowed to decide the draft

why the old line was wrong
- the draft comparator treated soft player-denial value as more important than how much stamina the enemy had to burn to reproduce the same preview line
- that let a tiny yellow beat a die that directly secured the same outcome more cleanly
- in this board, Gog could talk itself into taking `y1` and leaving both `r4` and `w6` behind

why the fixed line is correct
- once the real preview gates tie, lower `BestPlan.SpentStamina` now resolves the draft before softer denial heuristics
- that keeps direct self-securing dice above weak yellow nuisance value
- `w6` therefore beats `y1` here, while still preserving the existing high-level preview ordering

result
- Gog should take `w6`
- soft yellow denial must not beat the same real draft line when it costs more stamina to maintain

## case 16 - gog must take `w6` over blue nuisance value when white secures the real line

PLAYER: 2 / 2 / 5 / 6


accuracy: 2 = (2)


speed: 2 = (2)


damage: 5 = (-1 + y6)


parry: 6 = (6)


stamina: 0 remaining


wounds: none


targeting: chest


available dice: g6, b2, b1, w6


enemy: Gog


ENEMY: 5 / 8 / 5 / 4


accuracy: 5 = (5)


speed: 8 = (4 + b4)


damage: 5 = (5)


parry: 4 = (4)


stamina: 3 remaining


wounds: none


targeting: hand


`EnemyAI.cs TRACE RESULT:`

if: `IsBetterDraftPlanPreview(BestPlan(w6), BestPlan(b2))` -> false
- the high-level real preview gates can tie here
- both picks can still reach a deny-and-hit line once Gog spends the remaining stamina well

if: `IsBetterDraftPlanPreview(BestPlan(b2), BestPlan(w6))` -> false
- the blue die does not create a stronger real post-pick plan than the white die

if: `IsBetterDraftOutcomePreview(BestPlan(w6), BestPlan(b2))` -> false
- the coarse kill / damage / survive bucket also ties

if: `candidate.DeniesPlayerKill != current.DeniesPlayerKill` -> false
- neither die uniquely flips an immediate player kill line here

if: `candidate.DeniesPlayerDamage != current.DeniesPlayerDamage` -> false
- neither die wins solely on the hard player-damage denial flag

if in the old comparator: `!Mathf.Approximately(candidate.PlayerDenialScore, current.PlayerDenialScore)` -> true for `b2`
- `GetPlayerDieDesireScore(...)` heavily values blue because the player likes speed
- that soft nuisance score could beat the white die before the enemy's own direct draft benefit was allowed to decide the tie

if in the fixed comparator: `!Mathf.Approximately(candidate.ProgressScore, current.ProgressScore)` or later `candidate.DieValue != current.DieValue` -> true for `w6`
- `w6` directly closes the enemy's defense gap and is the larger self-securing die
- that self-benefit now resolves the tie before the softer denied-player-value score

why the old line was wrong
- the draft comparator let a soft "the player wants this blue" score outrank a die that directly improved Gog's own real attack / defense plan
- that made blue nuisance value look better than the stronger white setup die even though Gog still needed a real self-securing line

why the fixed line is correct
- once the real preview and hard denial gates tie, the comparator now prefers direct enemy progress and stronger self-value before falling back to `PlayerDenialScore`
- this keeps blue annoyance picks from beating a die that actually secures the better draft line for Gog

result
- Gog should take `w6`
- soft player desire for blue must not beat the stronger self-benefiting white draft line

## case 17 - kobold must take `r2` when utility dice only create the line by later stamina

PLAYER: 2 / 2 / 1 / 2


accuracy: 2 = (2)


speed: 2 = (2)


damage: 1 = (-1 + y2)


parry: 2 = (2)


stamina: 2 remaining


wounds: none


targeting: chest


available dice: b5, b6, r2


enemy: Kobold


ENEMY: 6 / 4 / 2 / 11


accuracy: 6 = (4 + g2)


speed: 4 = (4)


damage: 2 = (2)


parry: 11 = (5 + w6)


stamina: 3 remaining


wounds: none


targeting: armpits


`EnemyAI.cs TRACE RESULT:`

if: `playerCanHitBefore = PlayerAim >= 0 && PlayerAtt > EnemyDef` -> false
- `2 >= 0` is true, but `1 > 11` is false
- the player already cannot damage the enemy, so soft denial value is low here

if: `enemyCanHitBefore = EnemyAim >= 0 && EnemyAtt > PlayerDef` -> false
- `6 >= 0` is true, but `2 > 2` is false
- Kobold does not currently have a wound line

if: `BestPlan(r2).EnemyDamagesPlayer` -> true
- taking `r2` immediately raises enemy attack from `2` to `4`
- `4 > 2` turns on the hit threshold with no extra stamina required

if: `BestPlan(b5 or b6).EnemyDamagesPlayer` -> true only after later red stamina
- the blue utility pick still needs Kobold to burn stamina into red before any wound can land
- it does not directly complete the missing hit threshold by itself

if: `candidateSpentStamina != currentSpentStamina` -> true for `r2` vs utility blue lines whenever the real preview ties
- `r2` reaches the same real post-pick line more cheaply

if in the fixed comparator: `candidate.CompletesHitBreakpoint != current.CompletesHitBreakpoint` -> true for `r2`
- the red die directly turns on the first real attack breakpoint
- that breakpoint now resolves the draft before softer utility / denial cleanup can interfere

if: `candidate.DeniesPlayerGoFirst != current.DeniesPlayerGoFirst` -> false as a useful separator
- Kobold already acts first at `4 > 2`
- extra blue does not create a new action-order gate here

why the old line was wrong
- a utility die could survive long enough in the draft comparator to beat the direct hit-enabling red die, even though the enemy had no live attack line yet
- that forced the enemy to rely on later stamina spending for a wound it could have secured immediately by drafting red

why the fixed line is correct
- once the real preview gates and hard denial flags tie, direct breakpoint completion now beats softer utility considerations
- when the enemy cannot yet hit, the die that actually turns on the hit threshold must win over dice that only help indirectly

result
- Kobold should take `r2`
- utility dice must not beat a direct hit-enabling draft when the enemy has no live wound line

## case 18 - kobold rebuild must still take `r2` over utility blue

PLAYER: 2 / 2 / 1 / 2


accuracy: 2 = (2)


speed: 2 = (2)


damage: 1 = (-1 + y2)


parry: 2 = (2)


stamina: 2 remaining


wounds: none


targeting: chest


available dice: b5, b6, r2


enemy: Kobold


ENEMY: 6 / 4 / 2 / 11


accuracy: 6 = (4 + g2)


speed: 4 = (4)


damage: 2 = (2)


parry: 11 = (5 + w6)


stamina: 3 remaining


wounds: none


targeting: armpits


`EnemyAI.cs TRACE RESULT:`

if: `playerCanHitBefore = PlayerAim >= 0 && PlayerAtt > EnemyDef` -> false
- `2 >= 0` is true, but `1 > 11` is false
- the player still cannot damage Kobold here, so denial value should not drive the draft

if: `enemyCanHitBefore = EnemyAim >= 0 && EnemyAtt > PlayerDef` -> false
- `6 >= 0` is true, but `2 > 2` is false
- Kobold starts with no live wound line

if: `BestPlan(r2).EnemyDamagesPlayer` -> true
- drafting `r2` raises enemy attack from `2` to `4`
- `4 > 2` immediately turns on the hit threshold

if: `BestPlan(b5)` or `BestPlan(b6)` damages only after later stamina help -> true
- blue can still participate in a later plan, but it does not directly create the missing wound line
- the enemy would be choosing an indirect setup die over a direct winning die

if: `candidateSpentStamina != currentSpentStamina` -> true for `r2` against the utility lines whenever the real preview ties
- `r2` reaches the same strategic line with less permanent stamina

if in the fixed comparator: `candidate.CompletesHitBreakpoint != current.CompletesHitBreakpoint` -> true for `r2`
- the direct hit-enabling breakpoint now resolves the draft before softer utility cleanup

if: `candidate.DeniesPlayerGoFirst != current.DeniesPlayerGoFirst` -> false as a deciding gate
- Kobold already acts first at `4 > 2`
- more blue does not create a new meaningful order change

why the old line was wrong
- the hard/nightmare draft comparator could let soft utility value linger too long even when the enemy still lacked a live hit line
- that made Kobold pass up `r2` and rely on later stamina for something the draft itself could already secure

why the fixed line is correct
- once real preview gates and hard denial tie, direct breakpoint completion now wins first
- this keeps hard/nightmare enemies from taking utility dice over the die that actually enables the wound line

result
- Kobold should take `r2`
- case 17 and case 18 both stay protected by the same hard/nightmare comparator rule

## case 19 - kobold must take the highest yellow once yellow is the winning color

PLAYER: 2 / 2 / -1 / 2


accuracy: 2 = (2)


speed: 2 = (2)


damage: -1 = (-1)


parry: 2 = (2)


stamina: 69 remaining


wounds: none


targeting: chest


available dice: y5, g4, b6, r6, w3


enemy: Kobold


ENEMY: 4 / 3 / 6 / 7


accuracy: 4 = (4)


speed: 3 = (3)


damage: 6 = (2 + y4)


parry: 7 = (7)


stamina: 4 remaining


wounds: none


targeting: head


`EnemyAI.cs TRACE RESULT:`

if: `playerCanHitBefore = PlayerAim >= 0 && PlayerAtt > EnemyDef` -> false
- `2 >= 0` is true, but `-1 > 7` is false
- the player cannot threaten Kobold here, so this is mostly a self-value draft question

if: `enemyCanHitBefore = EnemyAim >= 0 && EnemyAtt > PlayerDef` -> true
- `4 >= 0` is true and `6 > 2` is true
- Kobold already has a live wound line before drafting

if two candidate picks are both yellow and `IsBetterDraftPlanPreview(...)` plus `IsBetterDraftOutcomePreview(...)` tie -> possible
- same-color candidates can reach the same strategic preview bucket
- the old comparator could then drift into later cleanup and occasionally keep the lower yellow

if in the fixed comparator: `candidate.DieType == current.DieType && candidate.DieValue != current.DieValue` -> true for higher yellow
- once both candidates are yellow and the strategic preview ties, the higher face value wins immediately
- a lower yellow can no longer survive by looking slightly cleaner on later utility cleanup

if in the second safety layer: `ChooseAdvancedDraftDie(...)` returns the highest face die matching the winning color -> true
- even if an earlier comparison path ever settled on yellow without distinguishing `y5` from a lower yellow, the final selection upgrades to the highest yellow still available
- this gives a color-level guarantee on top of the comparator fix

why the old line was wrong
- same-color candidates could occasionally fall through to later cleanup where lower overspend or similar noise kept the smaller die
- that allowed the ai to pick a weaker die even after effectively deciding that the color itself was correct

why the fixed line is correct
- hard/nightmare draft choice now has two defenses
- first, same-color ties resolve to the higher face value early in the comparator
- second, the final hard/nightmare picker upgrades any chosen color to the highest die of that color still on the board

result
- if Kobold chooses yellow here, it must take `y5`
- once hard/nightmare commits to a color, it must never leave a higher-value die of that same color behind
