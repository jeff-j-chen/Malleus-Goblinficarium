# findings

## fixed in this pass

- `Assets/Scripts/ItemManager.cs`
  - tarot-driven die upgrades now keep the enemy plan refresh batched until the die finishes settling
  - this removes the redundant enemy recalculation that used to happen once on die attach and again after the tarot bonus increased the die value

## documentation and comment gaps

- `Assets/Scripts/CharacterSelector.cs` around the difficulty toggle and starter-item flow
  - comments and summaries still refer to an old "easy mode" toggle even though the code now cycles four difficulties
  - rename the wording to "difficulty" and summarize the actual hide/show rules per difficulty

- `Assets/Scripts/Player.cs` around startup, target movement, and restart handling
  - several core lifecycle methods near the top of the class still lack summaries while later combat methods are documented
  - add short summaries for startup, target movement, suicide, and tutorial-specific restrictions

- `Assets/Scripts/Scripts.cs`
  - the bootstrap object has no class-level summary and only sparse method documentation
  - document initialization order, scene wiring, cached delays, and the delayed-save behavior

- `Assets/Scripts/DifficultyHelper.cs`
  - the helper is clean but under-documented for how difficulty migration and policy helpers are intended to be used
  - add class and public method summaries so future rule changes stay centralized

## inefficiencies and discrepancies

- `Assets/Scripts/Save.cs`
  - `Save.SaveGame()` does a fresh `FindFirstObjectByType<Scripts>()` and a full file write on every call
  - this is invoked from many gameplay paths, so save cost scales with combat churn
  - streamline by caching the scene reference and batching or debouncing writes at action boundaries

- `Assets/Scripts/SoundManager.cs`
  - `PlayClip(string)` relies on `Array.IndexOf(...)` with direct array indexing and no missing-key guard
  - an unknown sound name can become an invalid `-1` access
  - replace the parallel arrays with a startup dictionary and log a warning for unknown keys

- `Assets/Scripts/Music.cs`
  - `musicPieceNames` is written to in `Awake()` without being allocated there, unlike `SoundManager`
  - this currently depends on inspector setup being correct
  - initialize the array in code or replace the name-array lookup with a dictionary

- `Assets/Scripts/Music.cs`
  - base volume is inconsistent: startup uses `0.4f` while fade routines restore to `0.5f`
  - music gets permanently louder after the first fade cycle
  - centralize the base volume in one constant or serialized field

- `Assets/Scripts/Enemy.cs`, `Assets/Scripts/ItemManager.cs`, `Assets/Scripts/CharacterSelector.cs`
  - difficulty-dependent behavior is still expressed through scattered inline checks
  - some code paths group hard and nightmare together while others special-case nightmare only
  - move these policies behind intent-based helpers in `DifficultyHelper`

## streamlining opportunities

- `Assets/Scripts/ItemManager.cs`
  - item creation is spread across several flows for random drops, named items, trader items, forge results, and weapons
  - shared instantiate, sprite, naming, parenting, and registration logic is repeated
  - consolidate through one floor-item creation helper plus small post-processing hooks

- `Assets/Scripts/CharacterSelector.cs`
  - starter loadouts are hard-coded in a large switch with later nightmare overrides layered on top
  - rebalance and expansion will stay tedious while data is embedded in control flow
  - move starter kits into a data table or small config structure

- `Assets/Scripts/ItemManager.cs`
  - there is a dead branch around floor-item saving where both sides still call `SaveFloorItems()`
  - remove the branch or restore the intended special handling explicitly

- `Assets/Scripts/ItemManager.cs`
  - inventory helpers still use LINQ-heavy lookups and exception-style fallback for simple item presence queries
  - these are hot helpers used across combat and item systems
  - replace them with direct loops or a lightweight cached lookup keyed by item name and modifier

## already in solid shape

- `Assets/Scripts/EnemyAI.cs`
  - planner and cache logic is already documented better than most of the project

- `Assets/Scripts/AlmanacController.cs`
  - concise comments and clear method boundaries make the UI flow easy to follow

- `Assets/Scripts/ItemManager.cs`
  - despite its size, much of the public item flow already has better coverage than the surrounding scripts
