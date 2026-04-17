# enemy ai (alias)

Authoritative combat and planner documentation for this project lives in **`ENEMY_AI.md`** (underscore in the filename). Update that file when changing enemy AI or shared combat primitives; this file exists so references to `ENEMYAI.md` still resolve in the repo.

## recent deltas (mirror)

- combined **queued bleed-out** + **third-wound kill** on the same attack: only **`Kill(..., bleedOut: true)`** sets the bleed-out status line (no second line from `PlayerAttacks` / `EnemyAttacks`).
- **maul**: default wound hit line; neck-style bleed-out is applied only through `ApplyInjuriesDuringMove(..., neckStyleBleedOut: true)` on a new wound.
- **charm of the inevitable**: when the enemy acts first, the player gets **+1 attack immediately** for their counterattack that round.
