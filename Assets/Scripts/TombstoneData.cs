using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class TombstoneData : MonoBehaviour {
    private Scripts s;

    private void Start() {
        s = FindFirstObjectByType<Scripts>();
    }

    public bool HasUsableResurrectionAmulet() {
        s = FindFirstObjectByType<Scripts>();
        GameObject amuletObject = s.itemManager.GetPlayerItem("amulet of resurrection");
        if (amuletObject == null) { return false; }

        Item amulet = amuletObject.GetComponent<Item>();
        return amulet != null && amulet.itemName != "broken amulet";
    }

    public bool PrepareResurrectionAmulet() {
        s = FindFirstObjectByType<Scripts>();
        GameObject amuletObject = s.itemManager.GetPlayerItem("amulet of resurrection");
        if (amuletObject == null) { return false; }

        Item amulet = amuletObject.GetComponent<Item>();
        if (amulet == null || amulet.itemName == "broken amulet") { return false; }

        s.itemManager.MarkItemUsed("amulet of resurrection");
        BreakResurrectionAmulet(amulet);
        PrepareResurrectionDestination();
        Save.game.showAmuletSurvivalStatusText = true;
        Save.game.pendingAmuletInventoryCleanup = true;
        Save.game.pendingAmuletVisualRestore = true;
        s.itemManager.SaveInventoryItems(true);
        if (s.tutorial == null) { Save.SaveGame(); }
        Save.SavePersistent();
        s.levelManager.lockActions = true;
        return true;
    }

    public bool ConsumeResurrectionAmulet() {
        s = FindFirstObjectByType<Scripts>();
        GameObject amuletObject = s.itemManager.GetPlayerItem("amulet of resurrection");
        if (amuletObject == null) { return false; }

        Item amulet = amuletObject.GetComponent<Item>();
        if (amulet == null || amulet.itemName == "broken amulet") { return false; }

        s.itemManager.MarkItemUsed("amulet of resurrection");
        BreakResurrectionAmulet(amulet);
        Save.game.showAmuletSurvivalStatusText = true;
        Save.game.pendingAmuletInventoryCleanup = true;
        Save.game.pendingAmuletVisualRestore = true;
        s.itemManager.SaveInventoryItems(true);
        if (s.tutorial == null) { Save.SaveGame(); }
        Save.SavePersistent();
        return true;
    }

    public void FinalizePreparedResurrectionInventoryCleanup() {
        s = FindFirstObjectByType<Scripts>();
        if (s == null || !Save.game.pendingAmuletInventoryCleanup) { return; }

        s.itemManager.KeepOnlyWeaponAndBrokenAmulet();
        Save.game.pendingAmuletInventoryCleanup = false;
        s.itemManager.SaveInventoryItems(true);
        if (s.tutorial == null) { Save.SaveGame(); }
        Save.SavePersistent();
    }

    public void BeginPreparedResurrection() {
        StartCoroutine(BeginPreparedResurrectionCoro());
    }

    public void RestorePlayerVisualStateAfterPreparedResurrection() {
        s = FindFirstObjectByType<Scripts>();
        if (s == null || !Save.game.pendingAmuletVisualRestore) { return; }

        s.player.RestoreDefaultVisualState();
        Save.game.pendingAmuletVisualRestore = false;
    }

    public bool HasPreparedTraderItems() {
        return Save.game.floorItemNames != null && Save.game.floorItemNames.Any(itemName => !string.IsNullOrEmpty(itemName));
    }

    /// <summary>
    /// Sets the tombstone data based on the player's current inventory.
    /// </summary>
    public void SetTombstoneData() {
        s = FindFirstObjectByType<Scripts>();
        Save.persistent.tsItemNames = new string[9];
        Save.persistent.tsItemTypes = new string[9];
        Save.persistent.tsItemMods = new string[9];
        // clear data before placing in new stuff
        Item item = s.player.inventory[0].GetComponent<Item>();
        Save.persistent.tsWeaponAcc = item.weaponStats["green"];
        Save.persistent.tsWeaponSpd = item.weaponStats["blue"];
        Save.persistent.tsWeaponDmg = item.weaponStats["red"];
        Save.persistent.tsWeaponDef = item.weaponStats["white"];
        Save.persistent.tsItemNames[0] = ItemManager.GetWeaponBaseName(item.itemName);
        Save.persistent.tsItemTypes[0] = item.itemType;
        Save.persistent.tsItemMods[0] = item.modifier;
        // Save the weapon first
        for (int i = 1; i < s.player.inventory.Count; i++) {
            item = s.player.inventory[i].GetComponent<Item>();
            Save.persistent.tsItemNames[i] = item.itemName;
            Save.persistent.tsItemTypes[i] = item.itemType;
            Save.persistent.tsItemMods[i] = item.modifier;
            // Save everything about the current items
        }
        if (s.levelManager.level == 4 && s.levelManager.sub == 1) { 
            Save.persistent.tsLevel = 3;
            Save.persistent.tsSub = 3;
            // game will crash if we go to 4-1*, easy solution here
        }
        else { 
            // normal level
            if (s.levelManager.sub >= LevelManager.MerchantSub) { Save.persistent.tsSub = 3; }
            // shift back tombstone to come before trader
            else if (s.levelManager.sub == 1 && s.levelManager.level == 1){ 
                Save.persistent.tsSub = -1;
                Save.persistent.tsLevel = -1;
                // dont give tombstones on 1-1
            }
            else { 
                Save.persistent.tsSub = s.levelManager.sub; 
                Save.persistent.tsLevel = s.levelManager.level;
            }
        }
        // assign the level of which the tombstone will appear on
        Save.game.newGame = true;
        // player died, so make the next game a new one
        if (s.tutorial == null) { Save.SaveGame(); }
        for (int i = 0; i < s.itemManager.floorItems.Count; i++){
            s.itemManager.MoveToInventory(0, true, false, false);
            // move all items on the floor to the player's inventory
        }
        foreach (GameObject toBeDeleted in s.player.inventory.ToList()) {
            if (toBeDeleted.GetComponent<Item>().itemName != "retry") {
                toBeDeleted.GetComponent<Item>().Remove(selectNew:false, dontSave:true);
                // remove all items except for the retry button
                // .ToList() is a trick to prevent ienumerator from acting up
            }
        }
        // KEEP IT AS FOREACH, for loop doesn't seem to work!
        GameObject retryButton = s.itemManager.CreateItem("retry");
        // create retry button
        s.itemManager.MoveToInventory(s.itemManager.floorItems.IndexOf(retryButton), true, false, false);
        // move the button explicitly, because it doesn't seem to want to be moved otherwise
        Save.game = new GameData {
            curCharNum = Save.persistent.newCharNum
        };
        // clear all existing player data
        // set the curcharnum to the new one, because it gets set to 0 on new GameData()
        if (s.tutorial == null) { Save.SaveGame(); }
        Save.SavePersistent();
    }

    private void BreakResurrectionAmulet(Item amulet) {
        amulet.itemName = "broken amulet";
        amulet.modifier = "";
        amulet.gameObject.name = amulet.itemName;
    }

    private IEnumerator BeginPreparedResurrectionCoro() {
        yield return new WaitForSeconds(1f);
        s.player.ResetAfterResurrection(false);
        s.levelManager.QueueWarpDestination(Save.game.resumeLevel, Save.game.resumeSub, Save.game.enemyNum, HasPreparedTraderItems());
        s.levelManager.lockActions = false;
        s.levelManager.NextLevel();
    }

    private void PrepareResurrectionDestination() {
        (int level, int sub, int enemyNum, bool useSavedTrader) = GetResurrectionDestination();
        if (useSavedTrader) {
            RestoreLastTraderFloorItems();
        }
        else {
            ClearPreparedResurrectionFloorItems();
        }

        Save.game.newGame = false;
        Save.game.resumeLevel = level;
        Save.game.resumeSub = sub;
        Save.game.enemyNum = enemyNum;
        Save.game.enemyStamina = 0;
        Save.game.enemyAcc = 0;
        Save.game.enemySpd = 0;
        Save.game.enemyDmg = 0;
        Save.game.enemyDef = 0;
        Save.game.enemyTargetIndex = 0;
        Save.game.enemyIsDead = false;
        Save.game.playerWounds = new();
        Save.game.enemyWounds = new();
        Save.game.playerBleedsOutNextRound = false;
        Save.game.enemyBleedsOutNextRound = false;
        Save.game.numItemsDroppedForTrade = 0;
        Save.game.blacksmithHasForged = false;
        Save.game.discardableDieCounter = 0;
        Save.game.usedMace = false;
        Save.game.usedAnkh = false;
        Save.game.usedHelm = false;
        Save.game.usedBoots = false;
        Save.game.pendingMirrorCopy = false;
        Save.game.enemyHasKatarSpeedPenalty = false;
        Save.game.diceNumbers = new();
        Save.game.diceTypes = new();
        Save.game.dicePlayerOrEnemy = new();
        Save.game.diceAttachedToStat = new();
        Save.game.diceRerolled = new();
    }

    private (int level, int sub, int enemyNum, bool useSavedTrader) GetResurrectionDestination() {
        if (s.levelManager.level < 4 && s.levelManager.sub < 4) {
            return (s.levelManager.level, 4, Enemy.MerchantEnemyNum, false);
        }

        bool hasSavedTrader = Save.game.lastTraderLevel > 0
            && Save.game.lastTraderSub > 0
            && Save.game.lastTraderEnemyNum > 0;
        if (hasSavedTrader) {
            return (Save.game.lastTraderLevel, Save.game.lastTraderSub, Save.game.lastTraderEnemyNum, true);
        }

        return (Mathf.Min(s.levelManager.level, 3), 4, Enemy.MerchantEnemyNum, false);
    }

    private void RestoreLastTraderFloorItems() {
        Save.game.floorItemNames = Save.game.lastTraderItemNames.ToArray();
        Save.game.floorItemTypes = Save.game.lastTraderItemTypes.ToArray();
        Save.game.floorItemMods = Save.game.lastTraderItemMods.ToArray();
        Save.game.floorItemAccs = Save.game.lastTraderItemAccs.ToArray();
        Save.game.floorItemSpds = Save.game.lastTraderItemSpds.ToArray();
        Save.game.floorItemDmgs = Save.game.lastTraderItemDmgs.ToArray();
        Save.game.floorItemDefs = Save.game.lastTraderItemDefs.ToArray();
    }

    private void ClearPreparedResurrectionFloorItems() {
        Save.game.floorItemNames = new string[9];
        Save.game.floorItemTypes = new string[9];
        Save.game.floorItemMods = new string[9];
        Save.game.floorItemAccs = new int[9];
        Save.game.floorItemSpds = new int[9];
        Save.game.floorItemDmgs = new int[9];
        Save.game.floorItemDefs = new int[9];
        Save.game.floorAcc = 0;
        Save.game.floorSpd = 0;
        Save.game.floorDmg = 0;
        Save.game.floorDef = 0;
    }

    /// <summary>
    /// Spawn the Saved tombstone items from the player's Save file.
    /// </summary>
    public void SpawnSavedTSItems(bool delay=false) {
        StartCoroutine(SpawnSavedTSItemsCoro(delay));
    }

    /// <summary>
    /// Spawn the Saved floor items from the player's Save file.
    /// </summary>
    public void SpawnSavedFloorItems(bool delay=false) {
        StartCoroutine(SpawnSavedFloorItemsCoro(delay));
    }

    /// <summary>
    /// Spawn the Saved merchant items from the player's Save file.
    /// </summary>
    public void SpawnSavedMerchantItems(bool delay=false) {
        StartCoroutine(SpawnSavedMerchantItemsCoro(delay));
    }

    /// <summary>
    /// Do not call this coroutine, use SpawnSavedTSItems() instead.
    /// </summary>
    private IEnumerator SpawnSavedTSItemsCoro(bool delay) {
        if (delay) { yield return new WaitForSeconds(0.01f); }
        // optional slight delay so that resuming on a tombstone level doesn't mess things up
        s = FindFirstObjectByType<Scripts>();
        string tombstoneWeaponName = ItemManager.NormalizeWeaponSaveName(Save.persistent.tsItemNames[0]);
        s.itemManager.CreateWeaponWithStats(
            tombstoneWeaponName,
            tombstoneWeaponName == "ham" ? "rotten" : "rusty",
            Save.persistent.tsWeaponAcc - 1,
            Save.persistent.tsWeaponSpd - 1,
            Save.persistent.tsWeaponDmg - 1,
            Save.persistent.tsWeaponDef - 1);
        // make the item with worse stats
        List<int> availableItemIndices = new();
        for (int i = 1; i < Save.persistent.tsItemNames.Length; i++) {
            if (!string.IsNullOrEmpty(Save.persistent.tsItemNames[i])) {
                availableItemIndices.Add(i);
            }
        }

        int discardCount = availableItemIndices.Count == 0
            ? 0
            : Random.Range(0, availableItemIndices.Count / 2 + 1);
        List<int> randomizedItemIndices = availableItemIndices.OrderBy(_ => Random.value).ToList();
        int survivorCount = Mathf.Clamp(randomizedItemIndices.Count - discardCount, 0, 7);
        HashSet<int> keptItemIndices = randomizedItemIndices.Take(survivorCount).ToHashSet();

        for (int i = 1; i < Save.persistent.tsItemNames.Length; i++) {
            if (keptItemIndices.Contains(i)) {
                string ruinedItemName = ItemManager.GetRuinedCommonItemName(Save.persistent.tsItemNames[i]);
                s.itemManager.CreateItem(ruinedItemName.Replace(' ', '_'), Save.persistent.tsItemMods[i]);
            }
            // ruin certain items
        }
        s.itemManager.CreateItem("arrow");
        // create the next level arrow
        s.itemManager.SaveFloorItems();
    }

    /// <summary>
    /// Do not call this coroutine, use SpawnSavedFloorItems() instead.
    /// </summary>
    private IEnumerator SpawnSavedFloorItemsCoro(bool delay) { 
        if (delay) { yield return new WaitForSeconds(0.01f); }
        // quick delay so its not super buggy
        s = FindFirstObjectByType<Scripts>();
        for (int i = 0; i < 9; i++) {  
            if (Save.game.floorItemNames[i] != null && Save.game.floorItemNames[i] != "") {
                s.itemManager.CreateSavedFloorItem(
                    Save.game.floorItemNames[i],
                    Save.game.floorItemTypes[i],
                    Save.game.floorItemMods[i],
                    Save.game.floorItemAccs[i],
                    Save.game.floorItemSpds[i],
                    Save.game.floorItemDmgs[i],
                    Save.game.floorItemDefs[i]
                );
            }
        }
        // then put all remaining items onto the floor
    }

    /// <summary>
    /// Do not call this coroutine, use SpawnSavedMerchantItems() instead.
    /// </summary>
    private IEnumerator SpawnSavedMerchantItemsCoro(bool delay) { 
        if (delay) { yield return new WaitForSeconds(0.01f); }
        s = FindFirstObjectByType<Scripts>();
        for (int i = 0; i < 9; i++) {  
            if (Save.game.floorItemNames[i] != null && Save.game.floorItemNames[i] != "") {
                s.itemManager.CreateSavedFloorItem(
                    Save.game.floorItemNames[i],
                    Save.game.floorItemTypes[i],
                    Save.game.floorItemMods[i],
                    Save.game.floorItemAccs[i],
                    Save.game.floorItemSpds[i],
                    Save.game.floorItemDmgs[i],
                    Save.game.floorItemDefs[i]
                );
            }
        }
        // merchant does not have items, so just spawn them in right away
    }
}