- [x] blacksmith stat hide now uses the same encounter-wide visibility rule as merchant and tombstone encounters
  - added a shared `RefreshEnemyStatVisibility()` path in `TurnManager` so vendor encounters consistently keep enemy stats covered

- [x] skeleton key escape no longer lets the next enemy inherit saved draft state or stamina
  - `LevelManager.ClearSavedEnemyCombatStateForEscape()` now clears enemy stamina, wounds, targeting, cached AI plan data, and saved dice attachment state before moving on

- [x] bleed-out text now wins over parry text when the enemy is already due to bleed out this round
  - `PlayerAttacks()` now filters that case before parry handling and uses the bleed-out attack presentation instead of the parry branch

- [x] combat status text remains limited to one player-attack message and one enemy-attack message per exchange
  - kept the per-side status setters and tightened the attack-branch filtering so bleed-out cases are resolved before fallback text like `he parries`
  - amulet survival continues to use the soft player death visual path instead of the full tombstone death flow

- [x] rusty weapons still count toward weapon usage statistics
  - verified the usage path records the equipped weapon through canonical-name normalization so rusty variants resolve to their base weapon entry

- [x] negative stat squares disappear correctly when stamina lifts a negative stat back to zero or above
  - `StatSummoner.GenerateForStat()` now renders negative squares from the post-stamina total rather than the raw pre-stamina total

- [x] resume no longer lets the enemy auto-draft an extra die if one was already drafted before saving
  - `DetermineMove()` now skips the opening auto-draft when the restored board already contains an enemy-attached die
  - fresh dice-set flows explicitly opt out of that resume-only guard