- combat exchange rule
  - only allow one player attack status and one enemy attack status per exchange
  - fix the branch logic first
  - do not rely on a generic coroutine-running check to mask duplicate or wrong text

- bleed-out text priority
  - if a side is already due to bleed out this round, resolve that text before parry or miss fallback text

- soft amulet death
  - amulet survival should use the soft player kill path, then restore visuals after the level fader has hidden the scene
  - avoid routing amulet survival through the full tombstone death flow

- encounter stat hiding
  - merchant, blacksmith, tombstone, and dead enemies should all use the same stat-cover rule

- enemy resume drafting
  - opening auto-draft must not fire on a restored board that already has an enemy-attached die
  - fresh dice-set flows can explicitly bypass that guard

- inventory invariant
  - equipped weapon stays in inventory slot 0
  - save and load code assumes that invariant directly