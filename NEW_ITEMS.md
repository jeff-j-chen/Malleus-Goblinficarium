# new items planning

## april 2026 follow-up checklist

### crystal shard

- [x] replace `charm of the crystalline` with standalone `crystal shard`
- [x] restore the passive bonus to `+2 attack`
- [x] make one shard break on a real wound
- [x] remove crystalline from charm roll tables, charm text, and charm-ai assumptions

### charm of the vindictive

- [x] change vindictive to grant `+2 attack` per trigger
- [x] keep charm-of-the-arcane and `stave` scaling working correctly
- [x] update runtime text and enemy-planning assumptions

### charm of the bulwark

- [x] grant the `+1 parry` immediately when the player declares attack second
- [x] keep `charm of the inevitable` unchanged
- [x] update runtime text and enemy-planning assumptions

### lucky dice

- [x] change the round roll to random `-1` or `+1` for every stat
- [x] keep stat totals readable outside live combat
- [x] suppress lucky-dice stat shifts when the enemy is dead or the room is merchant/blacksmith
- [x] update runtime text and enemy-planning assumptions where relevant

## april 2026 active checklist

### charm normalization pass

- [x] normalize all scalable charm buffs to `+1`
- [x] update charm descriptions to match the normalized `+1` values
- [x] keep `exalted` as a non-scaling one-hit shield
- [x] make crystalline passive/shatter math use the normalized `+1` base

### charm of the arcane

- [x] add `charm of the arcane` to charm roll tables and almanac ordering
- [x] add description text: `all charms are more effective`
- [x] make each copy increase every non-arcane scalable charm by `+1`
- [x] make multiple copies stack
- [x] keep `arcane`, `exalted`, and `nothing` unaffected by charm-arcane scaling

### tarot of the arcane

- [x] add `tarot of the arcane` to tarot roll tables and almanac ordering
- [x] add description text: `all tarots are more effective`
- [x] make each copy add `+1` more upgrade to every non-arcane tarot color
- [x] keep die upgrades capped at `6`
- [x] make multiple copies stack

### stave rework

- [x] change `stave` description to `all necklets and charms are more effective`
- [x] make `stave` still count as one extra arcane necklet
- [x] make `stave` also count as one extra charm of the arcane
- [x] update enemy-planning docs and runtime logic for the new scaling

## implementation map

| file | what changes |
|---|---|
| `GameData.cs` | charm active/pending bonus fields, glass sword shard state |
| `ItemManager.cs` | `charmPassiveStats`, `charmActiveBonus`, `charmPendingBonus`, charm/tarot counter logic, passive inventory recalculation for necklets + `stave`, glass sword/stave/gauntlets in stat dicts, `GetWeaponBaseName()` helper, typed item description dict entries |
| `StatSummoner.cs` | add `charmPassiveStats + charmActiveBonus` to `SumOfStat`, `RawSumOfStat`, `OutermostPlayerX` |
| `TurnManager.cs` | `enemyAttackedFirst` flag; charm trigger sites in `RoundOne`, `EnemyAttacks`, `PlayerAttacks`, `RoundTwo`, `ClearVariablesAfterRound`; glass sword shard logic in `EnemyAttacks` |
| `Item.cs` | charm/tarot description display in `Select()`; charm/tarot pickup and drop status text; passive-inventory refresh on removal |
| `Dice.cs` | `IncreaseDiceValue()` helper for tarot upgrades |
| `HighlightCalculator.cs` | apply tarot upgrades when draft dice are taken |
| `DiceSummoner.cs` | apply tarot upgrades to player-generated dice |
| `PersistentData.cs` / `Statistics.cs` | expand tracked weapon list for `glass sword`, `stave`, and `gauntlets` |

---

## shared infra

## multi-charm behavior

- yes: multiple charms already work simultaneously
- different charm modifiers coexist because each trigger checks the full inventory
- duplicate copies of the same charm stack because each copy contributes its own fixed effect
- shattering charms are consumed one at a time per hit, which matches the intended one-charm-per-trigger behavior

---

## latest balance and content pass

### april 2026 bugfix pass

- [x] attack resolution defers bleed-out / combined fatal presentation to `Kill(..., bleedOut: true)` only, so the same killing attack never runs two `SetStatusText` bleed-out lines (attack paths no longer pre-empt `Kill`)
- [x] thief's armband now turns the merchant's free take into `you steal ...` and only that free merchant take uses the steal wording
- [x] sacrificial chalice now stores charge per copy, stacks across multiple chalices, converts paired `+0.5` gains into a live extra point, and drops back correctly when one chalice is removed
- [x] unstable spellbook can transmute any clicked die during draft or combat (no “finish draft first” gate); enemy chest/head follow-up actions still take priority on eligible enemy dice unless head has no discard quota or the chest die was already rerolled
- [x] maul keeps the normal wound status text while applying neck-style bleed-out only through `ApplyInjuriesDuringMove(..., neckStyleBleedOut: true)` on a new wound (removed extra `SetBleedOutNextRound` that could double-book bleed state)
- [x] enemy ai now treats knee wounds as a fully locked initiative state, so blue dice and blue stamina are devalued whenever either side has a knee injury, with double-knee still resolving player-first
- [x] saved charm active and pending bonuses are restored again after the player finishes rebuilding their saved combat state, preventing load-time startup code from wiping their live effect
- [x] campfire, tincture, and potion of life now refuse to fire when the player has no wounds and report `you're healthy`
- [x] potion of life now uses the same wound-refresh animation timing as tincture, with the heal blip landing on the fade
- [x] successful run count now saves immediately when a run ends instead of being lost during the credits transition
- [x] almanac page turns now play the same click feedback as character select page turns
- [x] **lucky dice**: each combat round (including the first after `BeginNewEncounterWeaponState` and each `ClearVariablesAfterRound` via `EndEncounterWeaponState`), roll per copy in inventory: one random stat `-1`, another random stat `+2` (same stat allowed); every stat’s **total** after the shift is floored at `-1`
- [x] **charm of the inevitable**: when the enemy wins speed and attacks first in `RoundOne`, grant `+1` attack immediately (`ActivateCharmTriggerImmediately`) for the player’s reply that round

### gauntlets
description: `always go first`

- [x] add base stats `2 / -1 / 1 / -1`
- [x] add legendary stats `3 / -1 / 3 / -1`
- [x] make `gauntlets` always choose the first draft die
- [x] make `gauntlets` always attack first
- [x] add `gauntlets` to almanac and tracked weapon arrays

### charm save and restore

- [x] restore charm gameplay to per-copy fixed effects instead of flattening to one copy
- [x] keep saved scalar charm bonuses authoritative on load
- [x] refresh player combat stats after restoring inventory so saved charm bonuses immediately apply

### ham tombstone exception

- [x] tombstone `ham` now becomes `rotten ham`
- [x] `rotten ham` shows `gain 0 stamina next level`
- [x] weapon creation now prefers indexed weapon sprites so tombstone ham keeps its proper sprite

### glass sword timing

- [x] shatter reflow still waits for the `cloak` timing window
- [x] shattered stats now fully refresh combat stats and dice at that same moment
- [x] `shattered glass sword` description stays empty

### tarot and necklet text

- [x] necklets now always describe their own `+1` bonus
- [x] tarot descriptions no longer encode current stack count
- [x] gameplay stacking still works per copy

### maul

- [x] maul no longer kills instantly on hit
- [x] any successful maul hit now causes neck-style bleedout on the next round
- [x] maul hit text stays on the default `you hit ..., damaging ...` wording while the wound still queues bleed-out like a neck hit

### thief's armband

- [x] the merchant free take now uses `you steal ...`
- [x] paid trader pickups still use the normal `you take ...` text

### unstable spellbook

- [x] spellbook targeting now works on unattached draft dice as well as attached dice
- [x] spellbook can be activated while draft dice remain unattached (transmute any die anywhere once stamina is paid)
- [x] enemy chest rerolls and enemy-die discard windows resolve before spellbook transmute on eligible enemy dice

### sacrificial chalice

- [x] each chalice now tracks its own charge in inventory/save data
- [x] total chalice bonus is the floor of all chalice charges added together (e.g. two at `1.5` → `+3` to stats, not two separate floors of `1`)
- [x] each wound advances **every** copy by the same step (`+1` then `+0.5`…), so two chalices after one wound both read `+1` and together grant `+2`
- [x] legacy `Save.game.sacrificialChaliceCharge` with no per-item modifiers splits **evenly** across all chalices (not “fill first chalice to 3”); unequal per-copy modifiers are averaged and snapped back in sync
- [x] dropping one chalice removes only that chalice's stored contribution

---

## follow-up items

### stave
description: `all necklets are more effective`

- [x] add base stats `2 / 2 / 0 / 2`
- [x] add legendary stats `3 / 3 / 0 / 3`
- [x] add `stave` and `legendary stave` descriptions
- [x] make wielded `stave` count as an additional effective arcane necklet
- [x] recalculate necklet bonuses on weapon swap and inventory restore
- [x] add `stave` to weapon tracking arrays

### legendary stave
description: `all necklets are more effective`

- [x] add legendary stat block
- [x] share the same passive necklet-amplifying behavior as `stave`
- [x] add description entry
- [x] include in weapon tracking arrays

### charm of the nothing
description: `does nothing`

- [x] add `nothing` to `charmTypes`
- [x] add description text in `Item.cs`
- [x] add description dict entry
- [x] leave all gameplay hooks as a no-op

### tarot of the abyss
description: `blue dice are more effective`

- [x] add tarot modifier entry: `abyss`
- [x] upgrade taken blue dice by +1 per matching tarot, up to 6
- [x] add description text and pickup/drop strings
- [x] apply to player-generated blue dice too

### tarot of the verdant
description: `green dice are more effective`

- [x] add tarot modifier entry: `verdant`
- [x] upgrade taken green dice by +1 per matching tarot, up to 6
- [x] add description text and pickup/drop strings
- [x] apply to player-generated green dice too

### tarot of the inferno
description: `red dice are more effective`

- [x] add tarot modifier entry: `inferno`
- [x] upgrade taken red dice by +1 per matching tarot, up to 6
- [x] add description text and pickup/drop strings
- [x] apply to player-generated red dice too

### tarot of the glacier
description: `white dice are more effective`

- [x] add tarot modifier entry: `glacier`
- [x] upgrade taken white dice by +1 per matching tarot, up to 6
- [x] add description text and pickup/drop strings
- [x] apply to player-generated white dice too

### tarot of the dawn
description: `yellow dice are more effective`

- [x] add tarot modifier entry: `dawn`
- [x] upgrade taken yellow dice by +1 per matching tarot, up to 6
- [x] add description text and pickup/drop strings
- [x] apply to player-generated yellow dice too

### tarot of the nothing
description: `does nothing`

- [x] add tarot modifier entry: `nothing`
- [x] add description text and pickup/drop strings
- [x] leave gameplay as a no-op

### in `ItemManager`

```cs
// always-on stat bonus while charm is in inventory (crystalline: +2 atk per count)
public readonly Dictionary<string, int> charmPassiveStats = new() {
    { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 }
};
// stat bonus active this round (earned last round, applies now; cleared at round clear)
public Dictionary<string, int> charmActiveBonus = new() {
    { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 }
};
// stat bonus pending (earned this round, activates next round; cleared at round clear)
public Dictionary<string, int> charmPendingBonus = new() {
    { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 }
};

public void UpdateCharmPassiveStats() {
    charmPassiveStats["red"] = GetCharmCount("crystalline") * 2;
    // add other always-on charms here as needed
}
public bool PlayerHasCharm(string modifier) {
    return s.player.inventory.Any(a => {
        Item it = a.GetComponent<Item>();
        return it.itemName == "charm" && it.modifier == modifier;
    });
}
public int GetCharmCount(string modifier) {
    return s.player.inventory.Count(a => {
        Item it = a.GetComponent<Item>();
        return it.itemName == "charm" && it.modifier == modifier;
    });
}
```

### `SetItemStatsImmediately` addition
add "charm" modifier selection from `charmTypes`:
```cs
else if (instantiatedItem.GetComponent<Item>().itemName == "charm") {
    instantiatedItem.GetComponent<Item>().modifier = charmTypes[Random.Range(0, charmTypes.Length)];
}
```

### in `StatSummoner.SumOfStat` (player branch)
```cs
int sum = s.player.stats[stat] + s.player.potionStats[stat] + addedPlayerStamina[stat]
    + s.itemManager.neckletStats[stat]
    + s.itemManager.charmPassiveStats[stat]
    + s.itemManager.charmActiveBonus[stat]
    + GetEncounterWeaponStatBonus(stat);
```
same addition applies to `RawSumOfStat` and `OutermostPlayerX`.

### pickup / drop handling
in `MoveCommonItemToInventory` (via `ApplyNeckletEffects` or a new `ApplyCharmEffects`):
```cs
// on pickup of a charm
UpdateCharmPassiveStats();
StartCoroutine(UpdateUIAfterDelay());
```
in `Item.Remove()`, after the necklet block:
```cs
if (removedItem.itemName == "charm") {
    // passive stat recalculation (crystalline breaks reduce passive atk)
    s.itemManager.UpdateCharmPassiveStats();
    s.statSummoner.SummonStats();
    s.statSummoner.SetCombatDebugInformationFor("player");
}
```

### save / restore
in `GameData.cs`:
```cs
public int charmActiveBonusRed;
public int charmActiveBonusBlue;
public int charmActiveBonusGreen;
public int charmActiveBonusWhite;
public int charmPendingBonusRed;
public int charmPendingBonusBlue;
public int charmPendingBonusGreen;
public int charmPendingBonusWhite;
```
save at `ClearVariablesAfterRound`; restore in `ItemManager.GiveStarterItems` (continuation path) or in a new `RestoreCharmState()` call.

### `charmTypes` array
```cs
public string[] charmTypes = {
    "unbroken", "relentless", "aether", "ruthless",
    "crystalline", "exalted", "riposte", "bulwark", "vindictive"
};
```

---

## `TurnManager` additions

### new fields
```cs
private bool enemyAttackedFirst = false;   // set each RoundOne
private bool playerAttackedFirst = false;  // convenience inverse
```

### RoundOne modifications
```cs
public void RoundOne() {
    // ... existing setup ...
    enemyAttackedFirst = playerSpd < enemySpd;
    playerAttackedFirst = !enemyAttackedFirst;

    if (playerSpd >= enemySpd) {
        // CHARM: aether — player went first
        if (s.itemManager.PlayerHasCharm("aether"))
            s.itemManager.charmPendingBonus["blue"] += s.itemManager.GetCharmCount("aether");

        // CHARM: bulwark — does NOT trigger (player went first)
        if (!PlayerAttacks()) { ... }
        ...
    } else {
        // CHARM: bulwark — player attacks second
        if (s.itemManager.PlayerHasCharm("bulwark"))
            s.itemManager.charmPendingBonus["white"] += s.itemManager.GetCharmCount("bulwark");

        // CHARM: aether — does NOT trigger
        if (!EnemyAttacks()) { ... }
        ...
    }
}
```

### `EnemyAttacks()` modifications

**in the parry else-branch (enemy missed, player defended):**
```cs
// CHARM: unbroken — queue +parry next round
if (s.itemManager.PlayerHasCharm("unbroken"))
    s.itemManager.charmPendingBonus["white"] += s.itemManager.GetCharmCount("unbroken");

// CHARM: riposte
if (s.itemManager.PlayerHasCharm("riposte")) {
    if (enemyAttackedFirst) {
        // enemy went first → player goes second → apply immediately before player's RoundTwo attack
        s.itemManager.charmActiveBonus["red"] += s.itemManager.GetCharmCount("riposte");
        s.statSummoner.SummonStats();
    } else {
        // player went first → parry happens in RoundTwo → apply next round
        s.itemManager.charmPendingBonus["red"] += s.itemManager.GetCharmCount("riposte");
    }
}
```

**in the hit path, BEFORE wound is added (armor/exalted block):**
```cs
// CHARM: exalted — behaves like armor but says "your charm shatters" and plays cloak
bool exalted = !armor && s.itemManager.PlayerHasCharm("exalted");
if (exalted) {
    armor = true;  // reuse armor flag to block wound + skip animation
    SetStatusText($"{s.enemy.enemyName.text.ToLower()} hits you... your charm shatters");
    // remove ONE exalted charm
    StartCoroutine(RemoveCharmAfterDelay("exalted"));
    s.itemManager.Select(s.player.inventory, 0, playAudio: false);
}
// existing armor check follows unchanged
```

**sound plumbing for exalted vs armor:**
add `string hitSoundOverride = null` as optional parameter on `DoStuffForAttack`. When `exalted=true` but `armor=false-original`, pass `hitSoundOverride: "cloak"` and `armor: true` so animation is suppressed but sound is "cloak" not "armor".

actually simpler: introduce `bool charmShatter = false` alongside `armor`. When charmShatter=true:
- plays "cloak" in the sound block (instead of "hit")
- skips hit animation (same as armor)
- skips playing "armor" in the animation branch

in `DoStuffForAttack` signature: add `bool charmShatter = false`.

**in the wound-add block (after armor/exalted checks, player did take a hit):**
```cs
// CHARM: crystalline — no wound block, but plays cloak + breaks
bool crystallineShatters = s.itemManager.PlayerHasCharm("crystalline");
// (sound + removal handled by charmShatter in DoStuffForAttack)

// CHARM: vindictive — grant +3 atk
if (s.itemManager.PlayerHasCharm("vindictive") && !s.player.woundList.Contains(s.enemy.target.text)) {
    int bonus = s.itemManager.GetCharmCount("vindictive") * 3;
    if (enemyAttackedFirst) {
        // enemy went first → player still gets to attack this round → apply immediately
        s.itemManager.charmActiveBonus["red"] += bonus;
        s.statSummoner.SummonStats();
    } else {
        // enemy went second → round is over → queue for next round
        s.itemManager.charmPendingBonus["red"] += bonus;
    }
}
// glass sword check (see glass sword section below)
```

### `PlayerAttacks()` modifications

**when wound is added to enemy (after `s.enemy.woundList.Add(...)`):**
```cs
// CHARM: relentless — wound enemy → +2 atk next round
if (s.itemManager.PlayerHasCharm("relentless") && s.enemy.spawnNum != 0)
    s.itemManager.charmPendingBonus["red"] += s.itemManager.GetCharmCount("relentless") * 2;
```

**at the top of PlayerAttacks(), after InitializeVariables:**
```cs
// CHARM: ruthless — targeting neck this attack? grant +3 acc next round
if (s.itemManager.PlayerHasCharm("ruthless") && targetArr[Mathf.Clamp(s.player.targetIndex, 0, targetArr.Length-1)] == "neck")
    s.itemManager.charmPendingBonus["green"] += s.itemManager.GetCharmCount("ruthless") * 3;
```

### `ClearVariablesAfterRound()` addition (at the end)
```cs
// apply pending charm bonuses → become active next round
// keep always-on passive stats; just replace round-keyed bonuses
s.itemManager.charmActiveBonus["green"] = s.itemManager.charmPendingBonus["green"];
s.itemManager.charmActiveBonus["blue"]  = s.itemManager.charmPendingBonus["blue"];
s.itemManager.charmActiveBonus["red"]   = s.itemManager.charmPendingBonus["red"];
s.itemManager.charmActiveBonus["white"] = s.itemManager.charmPendingBonus["white"];
s.itemManager.charmPendingBonus["green"] = 0;
s.itemManager.charmPendingBonus["blue"]  = 0;
s.itemManager.charmPendingBonus["red"]   = 0;
s.itemManager.charmPendingBonus["white"] = 0;
s.itemManager.UpdateCharmPassiveStats();
// save charm state
Save.game.charmActiveBonusGreen = s.itemManager.charmActiveBonus["green"];
Save.game.charmActiveBonusBlue  = s.itemManager.charmActiveBonus["blue"];
Save.game.charmActiveBonusRed   = s.itemManager.charmActiveBonus["red"];
Save.game.charmActiveBonusWhite = s.itemManager.charmActiveBonus["white"];
```

---

## per-charm checklists

### charm of the unbroken (+1 parry on parry)
- [x] `charmTypes` entry: `"unbroken"`
- [x] description dict: `{ "charm of the unbroken", "parry to gain +1 parry" }`
- [x] `EnemyAttacks()` parry branch: `charmPendingBonus["white"] += GetCharmCount("unbroken")`
- [x] triggers regardless of who went first (timing: next round either way)
- [x] `ClearVariablesAfterRound` flush (covered by shared infra)
- [x] `GameData` save fields (covered)
- [x] `Item.Select()` description display
- [x] `SetPickupStatusText` "you take charm of the unbroken"
- [x] `Remove()` cleanup: `UpdateCharmPassiveStats()`

### charm of the relentless (+2 attack on enemy wound)
- [x] `charmTypes` entry: `"relentless"`
- [x] description: `"wound to gain +2 attack"`
- [x] `PlayerAttacks()`, wound-add path: `charmPendingBonus["red"] += count * 2`
- [x] only when `s.enemy.spawnNum != 0` (don't trigger vs cloaked devil)
- [x] next-round bonus either way (user spec: "if player went first, mark and add after roundtwo")
- [x] `ClearVariablesAfterRound` flush
- [x] `Item.Select()` description
- [x] pickup/drop infra

### charm of the aether (+1 speed if attack first)
- [x] `charmTypes` entry: `"aether"`
- [x] description: `"attack first to gain +1 speed"`
- [x] `RoundOne()`, before player-first branch: `charmPendingBonus["blue"] += count`
- [x] condition: `playerSpd >= enemySpd`
- [x] `ClearVariablesAfterRound` flush
- [x] `Item.Select()` description
- [x] pickup/drop infra

### charm of the ruthless (+3 accuracy when targeting neck)
- [x] `charmTypes` entry: `"ruthless"`
- [x] description: `"attack the neck to gain +3 accuracy"`
- [x] `PlayerAttacks()`, after `InitializeVariables`: check if neck target → `charmPendingBonus["green"] += count * 3`
- [x] triggers on the targeting, not just on hit
- [x] `ClearVariablesAfterRound` flush
- [x] `Item.Select()` description
- [x] pickup/drop infra

### charm of the crystalline (+2 attack, shatters on hit)
- [x] `charmTypes` entry: `"crystalline"`
- [x] description: `"shatters if wounded"` (wait: "+2 attack but breaks when wounded")
- [x] `UpdateCharmPassiveStats()`: `charmPassiveStats["red"] = GetCharmCount("crystalline") * 2`
- [x] called on pickup and `Remove()`
- [x] `EnemyAttacks()` hit path: if player has crystalline and wound would be added (not dodgy, not exalted-blocked): play "cloak" sound (via `charmShatter`), call `RemoveCharmAfterDelay("crystalline")`
  - wound STILL applies — crystalline only overrides sound and removes itself
  - no status text change
- [x] `DoStuffForAttack` gets `bool charmShatter` param: if true, play "cloak" not "hit", but still run hit animation
- [x] `Remove()` will call `UpdateCharmPassiveStats()` which decrements the +2

### charm of the exalted (blocks one hit, "your charm shatters")
- [x] `charmTypes` entry: `"exalted"`
- [x] description: `"protects from one hit"`
- [x] `EnemyAttacks()` hit path: checked AFTER armor, BEFORE wound. If present, set `exalted=true`, status text = `"...your charm shatters"`, call `RemoveCharmAfterDelay("exalted")`, pass `charmShatter=true` to `DoStuffForAttack` so "cloak" plays, wound is BLOCKED (same as armor)
- [x] sound: "cloak" (not "armor")
- [x] animation: none (same as armor — skips PlayHitAnimation)
- [x] `Item.Select()` description

### charm of riposte (+1 attack on parry)
- [x] `charmTypes` entry: `"riposte"`
- [x] description: `"parry to gain +1 attack"`
- [x] `EnemyAttacks()` parry branch:
  - enemy went first (parry in RoundOne) → `charmActiveBonus["red"] += count` + `SummonStats()`
  - player went first (parry in RoundTwo) → `charmPendingBonus["red"] += count`
- [x] determined by `enemyAttackedFirst` flag
- [x] `ClearVariablesAfterRound` flush
- [x] `Item.Select()` description
- [x] pickup/drop infra

### charm of the bulwark (+1 defense when attacking second)
- [x] `charmTypes` entry: `"bulwark"`
- [x] description: `"attack second to gain +1 defense"`
- [x] `RoundOne()`, enemy-first branch: `charmPendingBonus["white"] += count`
- [x] condition: `playerSpd < enemySpd`
- [x] `ClearVariablesAfterRound` flush
- [x] `Item.Select()` description
- [x] pickup/drop infra

### charm of the vindictive (+3 attack when wounded)
- [x] `charmTypes` entry: `"vindictive"`
- [x] description: `"wound to gain +3 attack"` (or "+3 attack when wounded")
- [x] `EnemyAttacks()` wound-add path (wound was actually added, not dodgy/armor/exalted):
  - enemy went first (`enemyAttackedFirst=true`) → `charmActiveBonus["red"] += count*3` + `SummonStats()`
  - enemy went second → `charmPendingBonus["red"] += count*3`
- [x] trigger: only if wound was actually added (not blocked by armor/exalted)
- [x] `ClearVariablesAfterRound` flush
- [x] `Item.Select()` description
- [x] pickup/drop infra

---

## glass sword

### overview
- weapon name: `"glass sword"` (two words — breaks `.Split(' ')[1]` patterns)
- normal stats: green=0, blue=3, red=6, white=0
- legendary stats: green=0, blue=3, red=8, white=0
- description: `"shatters if wounded"`
- on player hit (wound added): play "cloak", downgrade stats to 0/1/1/0 permanently; no text
- sprite: `"glass_sword"` (needs adding in Unity inspector for both `weaponSprites` and `otherSprites` or similar)

### data additions
```cs
// in weaponNames:
"glass sword" added at end

// in weaponStatDict:
{ "glass sword", { "green",0, "blue",3, "red",6, "white",0 } }

// in legendaryStatDict:
{ "glass sword", { "green",0, "blue",3, "red",8, "white",0 } }

// in descriptionDict:
{ "glass sword",           "shatters if wounded" }
{ "legendary glass sword", "shatters if wounded" }
```

### `NormalizeItemSpriteName` addition
```cs
"glass sword"  => "glass_sword",
"glass_sword"  => "glass_sword",
```

### `GetWeaponBaseName` helper
since all `.Split(' ')[1]` calls break for "common glass sword" (gives "glass" instead of "glass sword"):
```cs
private static string GetWeaponBaseName(string fullItemName) {
    string[] parts = fullItemName.Split(' ');
    return parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : fullItemName;
}
```

### all call sites that need `GetWeaponBaseName` instead of `.Split(' ')[1]`
| location | old | new |
|---|---|---|
| `ItemManager.PlayerHasWeapon` | `Split(' ')[1]` | `GetWeaponBaseName(...)` |
| `ItemManager.SaveInventoryItems` | `Split(' ')[1]` for weapon name | `GetWeaponBaseName(...)` |
| `ItemManager.SaveFloorItems` | `Split(' ')[1]` | `GetWeaponBaseName(...)` |
| `ItemManager.StoreCurrentFloorItemsAsLastTrader` | `Split(' ')[1]` | `GetWeaponBaseName(...)` |
| `TombstoneData.SetTombstoneData` | `Split(' ')[1]` | `ItemManager.GetWeaponBaseName(...)` |
| `CharacterSelector` nightmare rename | `Split(' ')[1]` | `ItemManager.GetWeaponBaseName(...)` |
| `ItemManager.CreateRandomWeapon` | `modifier + " " + weaponNames[rand]` | `modifier + " " + weaponNames[rand]` (already fine — produces "common glass sword") |
| `ItemManager.ForgePlayerWeapon` | `weapon.itemName.Split(' ')[1]` | `GetWeaponBaseName(weapon.itemName)` |
| `ItemManager.MoveWeaponToInventory` | status text Split | `GetWeaponBaseName(...)` |
| `Item.Select()` | `itemName.Split(' ')[1]` (weapon description lookup) | `GetWeaponBaseName(itemName)` |
| `TurnManager.RoundOne` | `.Split(' ')[1]` for weaponUses tracking | `s.itemManager.GetWeaponBaseName(...)` |

### shattering in `EnemyAttacks()`
when player is hit (not dodgy, not blocked by armor/exalted) and wound actually added:
```cs
// glass sword shatters silently
if (s.itemManager.PlayerHasWeapon("glass sword") && !Save.game.glassSwordShattered) {
    Save.game.glassSwordShattered = true;
    StartCoroutine(ShatterGlassSwordAfterDelay());
    // sound is "cloak" — same mechanism as crystalline charmShatter flag
}
```

`ShatterGlassSwordAfterDelay`:
```cs
private IEnumerator ShatterGlassSwordAfterDelay() {
    yield return s.delays[0.45f];
    Item weapon = s.player.inventory[0].GetComponent<Item>();
    weapon.weaponStats["green"] = 0;
    weapon.weaponStats["blue"]  = 1;
    weapon.weaponStats["red"]   = 1;
    weapon.weaponStats["white"] = 0;
    s.player.stats = weapon.weaponStats;
    Save.game.resumeAcc = 0;
    Save.game.resumeSpd = 1;
    Save.game.resumeDmg = 1;
    Save.game.resumeDef = 0;
    s.statSummoner.SummonStats();
    s.statSummoner.SetCombatDebugInformationFor("player");
    s.itemManager.SaveInventoryItems();
}
```

`GameData` field: `public bool glassSwordShattered;` — reset to false when picking up a new glass sword (on `MoveWeaponToInventory` effectively). Actually this should be weapon-specific; we track it permanently on the save since the downgraded stats are saved directly to `resumeAcc/Spd/Dmg/Def`. So `glassSwordShattered` can be reset when a new weapon is taken or a new encounter starts.

actually: since the downgraded stats are stored in `resumeSpd/Dmg` etc. already, we don't need a separate field — the stats ARE the state. We just need to not shatter twice. Add `bool glassSwordShattered` to GameData, then reset it only when the player takes a new weapon or forges the current one.

---

## notes for unity editor setup

- add sprite `"glass_sword"` to `weaponSprites` array at the same index as `"glass sword"` in `weaponNames`
- add sprite `"charm"` to `itemSprites` array (single sprite shared by all charm modifier variants)
- charm should appear in item drop table: add `{ "charm", 4 }` to `itemDropDict`

---

## save tutorial JSON notice

the `tutorialJson` string in `Save.cs` is a hardcoded JSON literal. the new `GameData` fields will be missing from it and will default to `0`/`false`, which is fine. no action needed.

---

## edge cases / interaction notes

- **lich immunity**: lich is immune to most wound effects. charm of relentless should still trigger if lich is wounded (player inflicted wound), but confirm `s.enemy.spawnNum != 0` check still passes for lich (lich is not spawnNum 0 or 1, so yes).
- **maul instant kill**: maul kills instantly via `ApplyInjuriesDuringMove`, so `PlayerAttacks()` returns true early. charm of relentless should still fire — both add call sites are sequenced correctly since relentless is added when wound is added (before the return).
- **exalted vs armor**: if player has both armor and exalted, armor takes priority (checked first). exalted is only checked if `!armor`.
- **crystalline + exalted**: if exalted blocks the hit, crystalline does NOT shatter (no wound was added).
- **glass sword + armor**: armor blocks the hit, glass sword does NOT shatter.
- **glass sword + crystalline**: both trigger on same wound; both sound as "cloak" (one sound played), glass sword shatters, crystalline shatters.
- **glass sword shard re-shatter**: once shattered (stats 0/1/1/0), `glassSwordShattered=true` prevents re-triggering.
- **nightmare mode weapon swap**: glass sword behaves normally during nightmare mode restrictions.
- **forge**: forging a glass sword resets its modifier and itemName. `glassSwordShattered` should also reset on `ForgePlayerWeapon` call (or any upgrade). However, `glassSwordShattered` tracks the current sword's break state. If forged, treat as new — add reset to `ForgePlayerWeapon()`.

---

## sound summary

| event | sound |
|---|---|
| charm of exalted blocks a hit | "cloak" (replaces "hit") |
| charm of crystalline shatters on hit | "cloak" (replaces "hit"), wound still plays |
| glass sword shatters on hit | "cloak" (replaces "hit"), wound still plays |
| normal player hit | "hit" |
| armor blocks a hit | "armor" (during animation phase) |
| normal parry | "parry" |

---

## implementation

### GameData.cs checklist
- [x] add `charmActiveBonusGreen/Blue/Red/White` int fields
- [x] add `charmPendingBonusGreen/Blue/Red/White` int fields so mid-round saves preserve queued next-round bonuses
- [x] add `glassSwordShattered` bool field
- [x] update constructor for new fields (default 0/false)
- [x] update `Normalize()` if needed

### ItemManager.cs checklist
- [x] add `charmPassiveStats`, `charmActiveBonus`, `charmPendingBonus` dicts
- [x] add `charmTypes` array
- [x] add `PlayerHasCharm`, `GetCharmCount` helpers
- [x] add `UpdateCharmPassiveStats` method
- [x] static helper `GetWeaponBaseName`; make `PlayerHasWeapon` use it
- [x] fix `SaveInventoryItems` Split site
- [x] fix `SaveFloorItems` Split site
- [x] fix `StoreCurrentFloorItemsAsLastTrader` Split site
- [x] fix `MoveWeaponToInventory` display text Split site
- [x] fix `ForgePlayerWeapon` itemName rebuild Split site
- [x] add glass sword to `weaponNames`
- [x] add glass sword to `weaponStatDict` and `legendaryStatDict`
- [x] add glass sword entries to `descriptionDict`
- [x] add `NormalizeItemSpriteName` entries for "glass sword"
- [x] add "charm" to `itemDropDict`
- [x] add charm modifier in `SetItemStatsImmediately`
- [x] add "charm" description entries to `descriptionDict`
- [x] add `ApplyCharmEffects` call (like `ApplyNeckletEffects`) in `MoveCommonItemToInventory`
- [x] restore charm active bonus from Save in continuation path
- [x] reset `glassSwordShattered` when taking a new weapon or forging the current weapon

### StatSummoner.cs checklist
- [x] `SumOfStat` player path: add `charmPassiveStats + charmActiveBonus`
- [x] `RawSumOfStat` player path: same
- [x] `OutermostPlayerX`: add same terms

### TurnManager.cs checklist
- [x] add `enemyAttackedFirst` and `playerAttackedFirst` bool fields
- [x] `RoundOne`: set flags; charm aether trigger (player-first); charm bulwark trigger (enemy-first)
- [x] `EnemyAttacks()`: parry → unbroken pending, riposte (immediate or pending); hit → exalted (block+shatter), crystalline (shatter+cloak), vindictive (immediate or pending), glass sword (shatter+cloak)
- [x] `PlayerAttacks()`: ruthless (targeting neck), relentless (wound added)
- [x] `ClearVariablesAfterRound`: flush pending→active, clear pending, call `UpdateCharmPassiveStats`, save to GameData
- [x] add `RemoveCharmAfterDelay(string modifier)` coroutine
- [x] add `ShatterGlassSwordAfterDelay()` coroutine
- [x] `DoStuffForAttack`: add `bool charmShatter = false` param; if charmShatter, play "cloak" not "hit", but still run animation; skip "armor" sound override
- [x] ForgePlayerWeapon reset `glassSwordShattered`

### Item.cs checklist
- [x] `Select()` weapon branch: fix `itemName.Split(' ')[1]` → `GetWeaponBaseName(itemName)` for desc lookup
- [x] `Select()` weapon branch: fix `itemName.Split(' ')[1]` for modifier=="legendary" detection (already correct since it checks `[0]`, not `[1]`)
- [x] `Select()` common branch: add `"charm"` case with sub-switch on `modifier`
- [x] `SetPickupStatusText()`: add "charm" handling like "necklet" (shows "you take charm of X")
- [x] `Remove()`: add charm counter cleanup block (calls `UpdateCharmPassiveStats`)
- [x] `UseCommonCoro()`: charms are passive — no active use (like necklets). Add note or empty case to prevent errors if somehow Used.

### TurnManager.RoundOne weapon tracking
- [x] fix `Array.IndexOf(s.itemManager.weaponNames, itemName.Split(' ')[1])` to use `GetWeaponBaseName`

---

## CONFLICTS

- **tarot of the glacier vs the class passive "all white dice are set to 1"**  
    resolved: the class passive wins. tarot of the glacier does not upgrade white dice for that character, so white dice remain locked at `1`.

- **auto-heal character vs wound-triggered break/retaliation effects**  
    resolved: the hit still counts as being wounded for `charm of the vindictive`, `charm of the crystalline`, and `glass sword`, even if the character's auto-heal immediately clears the wound afterward.

- **resurrection amulet destination after neck bleed-out**  
    resolved: resurrection amulet returns the player to the trader with only their weapon and the broken amulet. this is the intended documented behavior, including after neck bleed-out.

---

## new request - april 2026

### implementation map additions

| file | what changes |
|---|---|
| `ItemManager.cs` | add `glaive` + `claymore` weapon data, new passive item descriptions/drop entries, chalice/cornucopia/claymore helper logic, vendor theft allowance, dynamic weapon preview helpers |
| `TurnManager.cs` | glaive second-wound kill logic, sacrificial chalice trigger timing on enemy hits, spellbook transmute reflow / enemy-plan refresh hook |
| `Item.cs` | unstable spellbook use flow, passive item descriptions, cornucopia-aware food use values, floor weapon preview uses dynamic claymore stats |
| `Dice.cs` | resolve pending unstable spellbook targeting, retint / transmute selected dice |
| `StatSummoner.cs` | include sacrificial chalice + claymore dynamic bonuses in player stat sums and stat-shape layout |
| `GameData.cs` | save chalice charge + pending spellbook targeting state |
| `PersistentData.cs` / `Statistics.cs` | expand tracked weapon arrays for `glaive` + `claymore` |
| `Save.cs` | no tutorial json changes needed; new save fields can default |

### glaive
description: `two wounds are deadly`

- [x] add base stats `2 / 0 / 4 / 1`
- [x] add legendary stats `2 / 0 / 6 / 1`
- [x] add `glaive` + `legendary glaive` descriptions
- [x] add `glaive` to weapon arrays / tracked stats / almanac order
- [x] make a fresh second wound instantly kill, as if it were the third wound
- [x] update kill-preview helpers so enemy plan / reroll previews respect `glaive`

### sacrificial chalice
description: `it thirsts...\n+0`

- [x] add item description entry with dynamic `+x` text
- [x] add save field for current chalice charge (`0.0` to `3.0`)
- [x] on the first enemy-inflicted wound, raise charge to `+1` and add `+1` to every stat
- [x] on later enemy-inflicted wounds, add `+0.5` charge each time, capped at `+3`
- [x] only whole points affect stats and visuals outside the description
- [x] trigger the stat gain at hit-sound timing, with no status text
- [x] change description to `it's full...\n+3` at max charge
- [x] add chalice passive bonus into player stat sums / shapes / preview layout

### unstable spellbook
description: `transmute a die`

- [x] add description + drop entry
- [x] only allow use after the draft is complete
- [x] play `zap` on use
- [x] consume the item and enter a pending-target state saved in `GameData`
- [x] allow clicking either a player die or an enemy die while pending
- [x] randomize both the selected die's color and its value
- [x] reattach the die to the correct stat, reflow dice, refresh stats, and rerun enemy planning
- [x] clear the pending state after resolution / when combat context ends

### claymore
description: `stronger with stamina`

- [x] add base stats `1 / 1 / 3 / 1`
- [x] add legendary stats `1 / 1 / 3 / 1`
- [x] add `claymore` + `legendary claymore` descriptions
- [x] add `claymore` to weapon arrays / tracked stats / almanac order
- [x] base effect: every `5` stamina grants `+1 speed` and `+1 damage`
- [x] legendary effect: every `4` stamina grants `+1 speed` and `+1 damage`
- [x] apply the bonus dynamically in real combat stats without mutating the saved base weapon block
- [x] make floor / inventory preview show the claymore values the player would currently get

### thief's armband
description: `take what's yours`

- [x] add description + drop entry + almanac tracking
- [x] grant `1` free merchant/blacksmith take per encounter via vendor trade-credit setup
- [x] keep the free-take state save-safe across reloads in vendor encounters

### cornucopia
description: `food gives more stamina`

- [x] add description + drop entry + almanac tracking
- [x] make `cheese` / `steak` restore `+2` more stamina while owned
- [x] stack correctly with character 1's food bonus
- [x] make `moldy cheese` / `rotten steak` restore `+1` while owned
- [x] give `ham` / `legendary ham` an additional `+1` level-start stamina while owned
- [x] update dynamic item descriptions so shown stamina values match gameplay

### unity editor setup notes

- [ ] add weapon sprites / almanac sprites for `glaive` and `claymore`
- [ ] add item sprites for `sacrificial chalice`, `unstable spellbook`, `thief's armband`, and `cornucopia`

## follow-up request - april 2026

### implementation map additions

| file | what changes |
|---|---|
| `ItemManager.cs` | add `crossbow` weapon data, update reusable spellbook text, keep `thief's armband` canonical, restrict merchant allowance, and restore charm state from proc-count saves first |
| `TurnManager.cs` | make crossbow ignore enemy parry while still missing on low accuracy, update attack previews, and replace the standard player-hit text with the chalice blood-drink line when applicable |
| `EnemyAI.cs` | stop overvaluing white defense against crossbow while still denying the player's key dice and kill lines |
| `Item.cs` | make `unstable spellbook` reusable with a stamina cost and stop vendor drops from feeding `kapala` |
| `StatSummoner.cs` | render `gladius` opening attack as diamond stats |
| `PersistentData.cs` / `Statistics.cs` | expand tracked weapon arrays for `crossbow` |

### crossbow
description: `ignore enemy parry`

- [x] add base stats `-1 / -1 / -1 / -1`
- [x] add legendary stats `1 / -1 / 0 / -1`
- [x] add `crossbow` + `legendary crossbow` descriptions
- [x] add `crossbow` to weapon arrays / tracked stats / almanac order
- [x] make crossbow hits ignore enemy parry when attack is above `0`
- [x] keep the default miss result when accuracy is below `0`
- [x] keep the default weak-attack result when attack is not above `0`
- [x] update enemy ai to favor pyrrhic-victory lines instead of futile white-defense stacking

### thief's armband follow-up

- [x] keep the in-game name exactly `thief's armband`
- [x] limit the free-take allowance to merchant encounters only
- [x] ensure dropped-item trade credit increments correctly

### charm save restore

- [x] stop saved charm state from being overwritten while loading
- [x] rebuild charm bonuses from saved proc counts whenever that data exists

### vendor drop handling

- [x] prevent merchant / blacksmith drops from triggering `kapala`
- [x] use trader-facing drop text while keeping vendor ui and trade-credit state in sync

### gladius

- [x] show the opening `+2` attack as diamonds

### sacrificial chalice follow-up

- [x] replace the normal player-hit status text with `xx hits you... the chalice drinks your blood!` when applicable
- [x] keep combat text limited to one player-side and one enemy-side message per round

### unstable spellbook follow-up
description: `pay 3 stamina to transmute a die`

- [x] stop consuming the spellbook on use
- [x] spend stamina when the spellbook is activated
- [x] keep the pending transmute state save-safe
- [x] update the description to match the new reusable behavior
