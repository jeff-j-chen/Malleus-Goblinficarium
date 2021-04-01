using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class TombstoneData : MonoBehaviour {
    Scripts scripts;

    void Start() {
        scripts = FindObjectOfType<Scripts>();
    }

    /// <summary>
    /// Sets the tombstone data based on the player's current inventory.
    /// </summary>
    public void SetTombstoneData() {
        scripts = FindObjectOfType<Scripts>();
        scripts.persistentData.tsItemNames = new string[9];
        scripts.persistentData.tsItemNames = new string[9];
        scripts.persistentData.tsItemNames = new string[9];
        // clear data before placing in new stuff
        Item item = scripts.player.inventory[0].GetComponent<Item>();
        scripts.persistentData.tsWeaponAcc = item.weaponStats["green"];
        scripts.persistentData.tsWeaponSpd = item.weaponStats["blue"];
        scripts.persistentData.tsWeaponDmg = item.weaponStats["red"];
        scripts.persistentData.tsWeaponDef = item.weaponStats["white"];
        scripts.persistentData.tsItemNames[0] = item.itemName.Split(' ')[1];
        scripts.persistentData.tsItemTypes[0] = item.itemType;
        scripts.persistentData.tsItemMods[0] = item.modifier;
        // save the wepon first
        for (int i = 1; i < scripts.player.inventory.Count; i++) {
            item = scripts.player.inventory[i].GetComponent<Item>();
            scripts.persistentData.tsItemNames[i] = item.itemName;
            scripts.persistentData.tsItemTypes[i] = item.itemType;
            scripts.persistentData.tsItemMods[i] = item.modifier;
            // save everyhting about the current items
        }
        if (scripts.levelManager.level == 4 && scripts.levelManager.sub == 1) { 
            scripts.persistentData.tsLevel = 3;
            scripts.persistentData.tsSub = 3;
            // game will crash if we go to 4-1*, easy solution here
        }
        else { 
            // normal level
            if (scripts.levelManager.sub == 4) { scripts.persistentData.tsSub = 3; }
            // shift back tombstone to come before trader
            else if (scripts.levelManager.sub == 1 && scripts.levelManager.level == 1){ 
                scripts.persistentData.tsSub = -1;
                scripts.persistentData.tsLevel = -1;
                // dont give tombstones on 1-1
            }
            else { 
                scripts.persistentData.tsSub = scripts.levelManager.sub; 
                scripts.persistentData.tsLevel = scripts.levelManager.level;
            }
        }
        // assign the level of which the tombstone will appear on
        scripts.gameData.newGame = true;
        // player died, so make the next game a new one
        scripts.SaveGameData();
        for (int i = 0; i < scripts.itemManager.floorItems.Count; i++){
            scripts.itemManager.MoveToInventory(0, true, false, false);
            // move all items on the floor to the player's inventory
        }
        foreach (GameObject toBeDeleted in scripts.player.inventory.ToList()) {
            if (toBeDeleted.GetComponent<Item>().itemName != "retry") {
                toBeDeleted.GetComponent<Item>().Remove(selectNew:false, dontSave:true);
                // remove all items except for the retry button
                // .ToList() is a trick to prevent ienumerator from acting up
            }
        }
        // KEEP IT AS FOREACH, for loop doesn't seem to work!
        GameObject retryButton = scripts.itemManager.CreateItem("retry", "retry");
        // create retry button
        scripts.itemManager.MoveToInventory(scripts.itemManager.floorItems.IndexOf(retryButton), true, false, false);
        // move the button explicitly, because it doesn't seem to want to be moved otherwise
        scripts.persistentData.deaths++;
        scripts.persistentData.gamesPlayed++;
        scripts.gameData = new GameData();
        // clear all existing player data
        scripts.gameData.curCharNum = scripts.persistentData.newCharNum;
        // set the curcharnum to the new one, because it gets set to 0 on new GameData()
        scripts.SaveGameData();
        scripts.SavePersistentData();
    }

    /// <summary>
    /// Spawn the saved tombstone items from the player's save file.
    /// </summary>
    public void SpawnSavedTSItems(bool delay=false) {
        StartCoroutine(SpawnSavedTSItemsCoro(delay));
    }

    /// <summary>
    /// Spawn the saved floor items from the player's save file.
    /// </summary>
    public void SpawnSavedFloorItems(bool delay=false) {
        StartCoroutine(SpawnSavedFloorItemsCoro(delay));
    }

    /// <summary>
    /// Spawn the saved merchant items from the player's save file.
    /// </summary>
    public void SpawnSavedMerchantItems(bool delay=false) {
        StartCoroutine(SpawnSavedMerchantItemsCoro(delay));
    }

    /// <summary>
    /// Do not call this coroutine, use SpawnSavedTSItems() instead.
    /// </summary>
    public IEnumerator SpawnSavedTSItemsCoro(bool delay) {
        if (delay) { yield return new WaitForSeconds(0.01f); }
        // optional slight delay so that resuming on a tombstone level doesn't mess things up
        scripts = FindObjectOfType<Scripts>();
        scripts.itemManager.CreateWeaponWithStats(scripts.persistentData.tsItemNames[0], "rusty", scripts.persistentData.tsWeaponAcc - 1, scripts.persistentData.tsWeaponSpd - 1, scripts.persistentData.tsWeaponDmg - 1, scripts.persistentData.tsWeaponDef - 1);
        // make the item with worse stats
        for (int i = 1; i < scripts.persistentData.tsItemNames.Length; i++) {
            if (scripts.persistentData.tsItemNames[i] != null && scripts.persistentData.tsItemNames[i] != "") { 
                GameObject created = scripts.itemManager.CreateItem(scripts.persistentData.tsItemNames[i].Replace(' ', '_'), scripts.persistentData.tsItemTypes[i], scripts.persistentData.tsItemMods[i]);
                switch (created.GetComponent<Item>().itemName) { 
                    case "helm of might":
                        created.GetComponent<Item>().itemName = "broken helm"; break;
                    case "boots of dodge":
                        created.GetComponent<Item>().itemName = "ruined boots"; break;
                    case "ankh":
                        created.GetComponent<Item>().itemName = "shattered ankh"; break;
                    case "kapala":
                        created.GetComponent<Item>().itemName = "defiled kapala"; break;
                    case "steak":
                        created.GetComponent<Item>().itemName = "rotten steak"; break;
                    case "cheese":
                        created.GetComponent<Item>().itemName = "moldy cheese"; break;
                }
            }
            // ruin certain items
        }
        scripts.itemManager.CreateItem("arrow", "arrow");
        // create the next level arrow
        scripts.itemManager.SaveTombstoneItems();
    }

    /// <summary>
    /// Do not call this coroutine, use SpawnSavedFloorItems() instead.
    /// </summary>
    public IEnumerator SpawnSavedFloorItemsCoro(bool delay) { 
        if (delay) { yield return new WaitForSeconds(0.01f); }
        // quick delay so its not super buggy
        scripts = FindObjectOfType<Scripts>();
        scripts.itemManager.CreateWeaponWithStats(scripts.gameData.floorItemNames[0], scripts.gameData.floorItemMods[0], scripts.gameData.floorAcc, scripts.gameData.floorSpd, scripts.gameData.floorDmg, scripts.gameData.floorDef);
        // spawn the floor's weapon first
        for (int i = 1; i < 9; i++) {  
            if (scripts.gameData.floorItemNames[i] != null && scripts.gameData.floorItemNames[i] != "") {
                GameObject created = scripts.itemManager.CreateItem(scripts.gameData.floorItemNames[i], scripts.gameData.floorItemTypes[i], scripts.gameData.floorItemMods[i]);
            }
        }
        // then put all remaining items onto the floor
    }

    /// <summary>
    /// Do not call this coroutine, use SpawnSavedMerchantItems() instead.
    /// </summary>
    public IEnumerator SpawnSavedMerchantItemsCoro(bool delay) { 
        if (delay) { yield return new WaitForSeconds(0.01f); }
        scripts = FindObjectOfType<Scripts>();
        for (int i = 0; i < 9; i++) {  
            if (scripts.gameData.floorItemNames[i] != null && scripts.gameData.floorItemNames[i] != "") {
                GameObject created = scripts.itemManager.CreateItem(scripts.gameData.floorItemNames[i], scripts.gameData.floorItemTypes[i], scripts.gameData.floorItemMods[i]);
            }
        }
        // merchant does not have items, so just spawn them in right away
    }
}