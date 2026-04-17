# itemmanager + item rewrite plan

## goals

- remove duplicated floor-item creation code from random drops, named item spawns, trader stock, forge arrows, saved floor items, and weapon generation
- stop depending on a separate `weaponNames` list that must stay in the same order as `weaponSprites`
- replace hot inventory helpers that currently use LINQ or `try/catch` for normal control flow
- keep all current gameplay behavior, save behavior, tutorial behavior, almanac behavior, and vendor behavior intact

## current constraints to preserve

these are not accidents; the rewrite has to keep them working

- `Item.itemName` is currently doing three jobs at once: gameplay identity, save/load identity, and display text seed
	- example: weapons are stored as `"legendary sword"`, `"forged mace"`, or `"shattered glass sword"`
	- because of that, several helpers strip modifiers back off with `GetWeaponBaseName()` before doing logic
- item instances need their core fields filled immediately after `Instantiate()`
	- some flows rely on the object being fully initialized before `Start()` runs
	- that is why `SetItemStatsImmediately()` exists today
- player inventory slot `0` is the equipped weapon slot
	- many combat, save, and AI helpers assume this directly
- trader and blacksmith stock are still ordinary floor items
	- they just have stricter post-processing and different pickup rules
- `forge` is a pseudo-item, not a normal inventory item
	- it needs common-item visuals and floor registration, but custom use behavior
- some sprite names are aliases, not direct item names
	- `glass_sword` -> `glass sword`
	- `glass_sword_shattered` -> `shattered glass sword`
	- other non-weapon aliases already exist too, such as `thiefs_armband`
- almanac ordering must stay stable even if runtime weapon lookup stops using a hard-coded ordered weapon-name array
	- page order and discovery indices are a separate concern from sprite lookup

## rewrite strategy

### phase 1 - introduce a single floor-item creation pipeline

- [ ] add one private helper that owns all shared floor-item instantiation work
- [ ] make every current floor-spawn path go through it
- [ ] keep small hooks for path-specific behavior instead of re-duplicating setup

### proposed shape

use one internal request object or method signature that carries all creation data needed by the different flows

example responsibilities for the central helper:

- choose spawn position from `itemX`, `itemY`, `itemSpacing`, `floorItems.Count`, and `negativeOffset`
- instantiate the prefab once
- assign parent
- assign sprite
- assign `itemName`, `itemType`, `modifier`
- assign `weaponStats` when relevant
- optionally run a post-process callback
- optionally register in `floorItems`
- return the created `GameObject`

example hooks that stay small and explicit:

- `AssignRandomCommonModifier(Item item)`
- `RollRandomWeaponData(string forcedWeaponName = null)`
- `EnsureTraderSafeModifier(Item item)`
- `ConfigureForgeArrow(Item item, string stat)`
- `ConfigureAlmanacDisplayItem(...)`

### why this shape

- the repeated logic is all mechanical and should live in one place
- the differences between flows are mostly data selection, not object construction
- post-process hooks let the code stay readable without reintroducing branching soup into one giant method

## floor creation flows to collapse

all of these should become thin wrappers over the shared helper

- [ ] `CreateRandomItem()`
- [ ] `CreateItem(string itemName, int negativeOffset = 0)`
- [ ] `CreateItem(string itemName, string modifier, int negativeOffset = 0)`
- [ ] `CreateRandomWeapon()`
- [ ] `CreateWeaponWithStats()`
- [ ] `CreateSmithUpgradeArrow()`
- [ ] `CreateSavedFloorItem()`
- [ ] `CreateRandomItemForTrader()`
- [ ] `CreateGuaranteedTraderHealingItem()`
- [ ] `CreateAlmanacItem()` only for the instantiate/setup portion

## weapon-name delivery rewrite

### problem

right now runtime weapon selection depends on two parallel structures:

- `CanonicalWeaponNames`
- `weaponSprites`

that coupling is fragile because the code assumes both lists stay in identical order forever.

### target model

- [ ] derive runtime weapon names from the actual sprite names in `weaponSprites`
- [ ] normalize sprite names into canonical weapon base names
- [ ] build a lookup map once during startup
- [ ] use that map for sprite lookup and random weapon selection

### proposed implementation

build a weapon catalog at startup, something conceptually like this:

- `List<string> weaponBaseNames`
- `Dictionary<string, Sprite> weaponSpriteByBaseName`
- `Dictionary<string, string> weaponBaseNameBySpriteName`

normalization rules:

- `dagger` -> `dagger`
- `glass_sword` -> `glass sword`
- `glass_sword_shattered` -> not part of random weapon base generation, but still registered as a sprite alias

### important boundary

the rewrite should separate two concerns that are currently mixed together:

1. runtime weapon identity and sprite lookup
2. stable almanac/save ordering

runtime lookup should come from sprite names.

stable ordered content, like `AlmanacWeaponOrder`, can stay explicit if its indices must not move.

### concrete rewrite steps

- [ ] remove serialized/runtime dependence on `weaponNames`
- [ ] replace `IndexOfWeaponName()` internals so they query the runtime catalog or an explicit set instead of a parallel array
- [ ] rewrite `GetWeaponSprite()` to use `weaponSpriteByBaseName`
- [ ] rewrite random weapon selection to choose from derived `weaponBaseNames`
- [ ] keep a small alias helper for the shattered glass sword special case
- [ ] validate that every weapon in `weaponStatDict` and `legendaryStatDict` resolves to a sprite-backed weapon name

### why this is safer

- sprite order stops mattering
- adding a new weapon becomes one content step instead of two synchronized list edits
- mismatches become detectable during startup with validation warnings instead of becoming silent bad drops

## inventory lookup rewrite

### problem

these helpers are called constantly across combat, AI, turn evaluation, and item use:

- `PlayerHas()`
- `PlayerHasWeapon()`
- `PlayerHasCharm()`
- `GetCharmCount()`
- `PlayerHasTarot()`
- `GetTarotCount()`
- `GetPlayerItem()`
- `GetEquippedWeapon()`

today they rely on LINQ scans, repeated `GetComponent<Item>()`, or `try/catch` fallback for ordinary misses.

### target model

- [ ] replace hot-path LINQ with direct loops immediately
- [ ] add a lightweight inventory cache that can be rebuilt after any inventory mutation
- [ ] key the cache by item name and, where needed, modifier
- [ ] keep `inventory[0]` as the source of truth for the equipped weapon

### cache shape

keep this simple. no complicated invalidation logic.

recommended cache data:

- `Dictionary<string, int> inventoryItemCounts`
- `Dictionary<string, GameObject> firstInventoryItemByName`
- `Dictionary<string, int> charmCountsByModifier`
- `Dictionary<string, int> tarotCountsByModifier`
- cached `Item equippedWeapon`
- cached `string equippedWeaponBaseName`
- cached `bool equippedWeaponIsLegendary`

### rebuild points

rebuild once after any operation that changes inventory contents or the equipped weapon:

- [ ] pickup from floor
- [ ] drop/remove item
- [ ] weapon exchange
- [ ] starter loadout grant
- [ ] direct restore from save
- [ ] cleanup helpers like `KeepOnlyWeaponAndBrokenAmulet()`
- [ ] any future item destruction path that can remove an inventory entry

### helper rewrite plan

- [ ] `PlayerHas()` -> direct cached count check
- [ ] `GetPlayerItem()` -> cached first item lookup
- [ ] `PlayerHasCharm()` / `GetCharmCount()` -> modifier count lookup
- [ ] `PlayerHasTarot()` / `GetTarotCount()` -> modifier count lookup
- [ ] `PlayerHasWeapon()` -> compare requested base name to cached equipped weapon base name
- [ ] `PlayerHasLegendary()` -> cached bool
- [ ] `GetEquippedWeapon()` -> direct slot-0 validation with no string-splitting fallback logic

### why this is enough

- inventory is tiny, so even direct loops are cheap
- the cache is still worthwhile because these helpers are called from many systems every combat frame and plan evaluation
- a rebuild-on-mutation model is easy to reason about and hard to desync if all mutations go through a few known methods

## data normalization cleanup

- [ ] centralize sprite-name aliases in one helper
- [ ] centralize display-name generation in one helper
- [ ] centralize weapon full-name creation in one helper
- [ ] centralize weapon base-name extraction in one helper that does not allocate more than needed

### naming helpers to add or tighten

- `NormalizeSpriteLookupName(string rawName)`
- `GetWeaponBaseName(string fullItemName)`
- `BuildWeaponFullName(string baseName, string modifier)`
- `TryGetWeaponBaseNameFromSprite(Sprite sprite, out string baseName)`
- `GetItemDisplayName(Item item)` or keep current display helpers but make them depend on normalized canonical naming

### important rule

only one place should know how to convert between:

- sprite name
- base gameplay name
- full saved weapon name
- display name

that avoids the current spread of `Replace("_", " ")`, `Split(' ')`, and special-case branches.

## weapon stat generation cleanup

- [ ] separate weapon roll logic from item instantiation
- [ ] keep one helper that returns complete rolled weapon data before object creation
- [ ] use the same path for random weapon drops and any future preview or simulation work

### recommended split

- `RollWeaponDefinition(string forcedWeaponBaseName = null)` returns
	- base name
	- modifier
	- rolled stats
	- sprite
	- final full item name

then the shared creation helper only consumes the finished definition.

### why

- the current `CreateRandomWeapon()` mixes RNG, data rules, object creation, naming, sprite lookup, and floor registration
- splitting these makes the weapon logic testable without spawning GameObjects

## item.cs cleanup plan

`Item.cs` should stay focused on interaction and item use, not identity reconstruction.

- [ ] replace ad hoc name formatting in status text paths with shared naming helpers from `ItemManager`
- [ ] stop depending on repeated inventory scans after remove/pickup if the manager can rebuild/update its cache centrally
- [ ] preserve all current use restrictions for vendors, tombstones, tutorials, and combat lock state

### places to tighten in `Item.cs`

- drop text generation
- blacksmith pickup text
- any `GetComponent<Item>()` chains that can be reduced after central helper cleanup
- post-remove refresh flow so cache rebuild happens in one known place

## behavior preservation checklist

the rewrite is only done if these still work exactly as they do now

### spawning and setup

- [ ] random common drops still roll the same tables and same modifier pools
- [ ] trader stock still avoids `nothing` for scrolls and potions where intended
- [ ] guaranteed trader healing slot still follows current weighted rules
- [ ] blacksmith forge arrows still spawn as `forge` pseudo-items with the chosen stat modifier
- [ ] saved floor items still restore with the same names, modifiers, and weapon stats
- [ ] almanac display items still show correct sprite, name, and default stats

### combat identity

- [ ] `PlayerHasWeapon()` still means equipped weapon only
- [ ] `PlayerHasLegendary()` still reflects the equipped weapon modifier
- [ ] glass sword shatter handling still uses the shattered sprite and special save behavior
- [ ] claymore, gladius, ham, spear, gauntlets, katar, glaive, and sword checks still read the correct weapon base name

### inventory and saves

- [ ] inventory slot `0` remains the equipped weapon slot
- [ ] save serialization still writes weapon base name plus modifier separately
- [ ] resume load still survives legacy saved names
- [ ] discovery tracking still resolves the same almanac full names
- [ ] removing or adding items always refreshes passive effects and cached lookup state

### ui and text

- [ ] item descriptions remain unchanged
- [ ] pickup/drop text remains unchanged
- [ ] vendor UI selection rules remain unchanged
- [ ] almanac page order remains unchanged

## validation plan

- [ ] add startup validation for mismatches between sprite-derived weapons and stat dictionaries
- [ ] log warnings if a weapon stat entry has no sprite
- [ ] log warnings if a weapon sprite has no base stat entry
- [ ] verify glass sword and shattered glass sword aliases explicitly

## implementation order

recommended order so the rewrite stays safe and reviewable

1. add weapon sprite/name catalog helpers and validation
2. add the shared floor-item creation helper
3. migrate common-item creation paths
4. migrate weapon creation paths
5. add inventory cache + rebuild method
6. switch hot helpers over to loops/cache
7. simplify `Item.cs` status text and removal paths to use the new helpers
8. run through gameplay regression checks

## regression checklist

- [ ] new game starter loadouts
- [ ] normal enemy loot drops
- [ ] merchant stock and trade flow
- [ ] blacksmith stock, weapon exchange, forge arrow, and post-forge naming
- [ ] challenge scroll and skeleton key floor transitions
- [ ] almanac pages, especially weapon page ordering
- [ ] save/load on normal items, forged weapons, legendary weapons, and shattered glass sword
- [ ] kapala offering, charm shatter, torch fade, and broken amulet persistence

## non-goals for this pass

- do not redesign item balance tables
- do not change saved data format unless required for compatibility wrapping
- do not change almanac discovery indexing
- do not move gameplay rules out of `Item.cs` unless needed to support the helper/cache rewrite

## end state

after the rewrite:

- item spawning should have one construction path
- weapon lookup should come from sprite-derived names instead of synchronized parallel arrays
- hot inventory queries should be cheap and explicit
- naming rules should live in one place
- current gameplay should remain behaviorally identical
