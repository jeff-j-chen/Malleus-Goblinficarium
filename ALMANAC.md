# Almanac System

A reference scene where the player reviews all items/weapons they have previously found.
Copied from the CharSelect scene structure.

## Pages

### Page 1 – Weapons (32 entries)
All 16 weapons + their legendary variants, interleaved in weaponNames order.
Grid: 3 full rows of 9 + 1 partial row of 5.

Display order:
  dagger, legendary dagger, flail, legendary flail, hatchet, legendary hatchet,
  mace, legendary mace, maul, legendary maul, montante, legendary montante,
  rapier, legendary rapier, scimitar, legendary scimitar, spear, legendary spear,
  sword, legendary sword, katar, legendary katar, buckler, legendary buckler,
  ham, legendary ham, gladius, legendary gladius, glass sword, legendary glass sword,
  stave, legendary stave

### Page 2 – Items (53 entries)
Every non-weapon collectible item, grouped by category.
Grid: 5 full rows of 9 + 1 partial row of 8.

Display order:
  Row 0: steak, cheese, torch, shuriken, mirror,
          scroll of fury, scroll of haste, scroll of dodge, scroll of leech
  Row 1: scroll of courage, scroll of challenge, scroll of nothing,
          potion of accuracy, potion of speed, potion of strength,
          potion of defense, potion of might, potion of life
  Row 2: potion of nothing,
          necklet of solidity, necklet of rapidity, necklet of strength,
          necklet of defense, arcane necklet, necklet of nothing, necklet of victory,
          charm of the unbroken
  Row 3: charm of the relentless, charm of the aether, charm of the ruthless, crystal shard,
          charm of the riposte, charm of the bulwark, charm of the vindictive,
          charm of the nothing
  Row 4: tarot of the abyss, tarot of the verdant, tarot of the inferno,
          tarot of the glacier, tarot of the dawn, tarot of the nothing,
          armor, ankh, skeleton key
  Row 5: witch hand, campfire, tincture, helm of might, boots of dodge, kapala,
          amulet of resurrection, phylactery

## Navigation
- Left / Right arrows: move within the current row (wraps at row edges)
- Up / Down arrows:    move between rows (clamps at page edges)
- Page switching is UI-button-driven (no keyboard page toggle, for now)

## Discovery tracking
- PersistentData.discoveredWeapons: bool[32], indexed by AlmanacWeaponOrder position
- PersistentData.discoveredItems:   bool[53], indexed by AlmanacItemOrder position
- Items are marked discovered via ItemManager.MarkItemDiscovered() which is called
  from MoveWeaponToInventory / MoveCommonItemToInventory (covers both starter
  and mid-game pickups; does NOT fire on RestoreSavedInventoryDirectly).

## Unknown items
- Shown with the "retry" sprite; itemDesc shows "???"
- Mouse click on an unknown item: Item.Select() returns early with "???" text

## Files changed
- ALMANAC.md               — this file
- PersistentData.cs        — discoveredWeapons / discoveredItems arrays
- ItemManager.cs           — isAlmanac flag, almanac order arrays, MarkItemDiscovered,
                             CreateAlmanacItem, GetAlmanacSpriteName, arrow-key guard
- Almanac.cs               — AlmanacController class (replaces copy of CharacterSelector)

## Checklist
- [x] ALMANAC.md created
- [x] PersistentData: discoveredWeapons / discoveredItems + Normalize
- [x] ItemManager: isAlmanac flag
- [x] ItemManager: arrow-key nav blocked when isAlmanac
- [x] ItemManager: ChangeItemList / UseCurrentItem guard for isAlmanac
- [x] ItemManager: Start() isAlmanac branch (skip default items)
- [x] ItemManager: AlmanacWeaponOrder / AlmanacItemOrder static arrays
- [x] ItemManager: MarkItemDiscovered() method
- [x] ItemManager: call MarkItemDiscovered from MoveWeaponToInventory
- [x] ItemManager: call MarkItemDiscovered from MoveCommonItemToInventory
- [x] ItemManager: CreateAlmanacItem() helper
- [x] Item.cs: early-return guard for "???" in Select()
- [x] Almanac.cs: AlmanacController class written
