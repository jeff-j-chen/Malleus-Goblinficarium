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
        Save.persistent.tsItemNames = new string[9];
        Save.persistent.tsItemNames = new string[9];
        Save.persistent.tsItemNames = new string[9];
        // clear data before placing in new stuff
        Item item = scripts.player.inventory[0].GetComponent<Item>();
        Save.persistent.tsWeaponAcc = item.weaponStats["green"];
        Save.persistent.tsWeaponSpd = item.weaponStats["blue"];
        Save.persistent.tsWeaponDmg = item.weaponStats["red"];
        Save.persistent.tsWeaponDef = item.weaponStats["white"];
        Save.persistent.tsItemNames[0] = item.itemName.Split(' ')[1];
        Save.persistent.tsItemTypes[0] = item.itemType;
        Save.persistent.tsItemMods[0] = item.modifier;
        // Save the wepon first
        for (int i = 1; i < scripts.player.inventory.Count; i++) {
            item = scripts.player.inventory[i].GetComponent<Item>();
            Save.persistent.tsItemNames[i] = item.itemName;
            Save.persistent.tsItemTypes[i] = item.itemType;
            Save.persistent.tsItemMods[i] = item.modifier;
            // Save everyhting about the current items
        }
        if (scripts.levelManager.level == 4 && scripts.levelManager.sub == 1) { 
            Save.persistent.tsLevel = 3;
            Save.persistent.tsSub = 3;
            // game will crash if we go to 4-1*, easy solution here
        }
        else { 
            // normal level
            if (scripts.levelManager.sub == 4) { Save.persistent.tsSub = 3; }
            // shift back tombstone to come before trader
            else if (scripts.levelManager.sub == 1 && scripts.levelManager.level == 1){ 
                Save.persistent.tsSub = -1;
                Save.persistent.tsLevel = -1;
                // dont give tombstones on 1-1
            }
            else { 
                Save.persistent.tsSub = scripts.levelManager.sub; 
                Save.persistent.tsLevel = scripts.levelManager.level;
            }
        }
        // assign the level of which the tombstone will appear on
        Save.game.newGame = true;
        // player died, so make the next game a new one
        Save.SaveGame();
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
        Save.persistent.deaths++;
        Save.persistent.gamesPlayed++;
        Save.game = new GameData();
        // clear all existing player data
        Save.game.curCharNum = Save.persistent.newCharNum;
        // set the curcharnum to the new one, because it gets set to 0 on new GameData()
        Save.SaveGame();
        Save.SavePersistent();
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
    public IEnumerator SpawnSavedTSItemsCoro(bool delay) {
        if (delay) { yield return new WaitForSeconds(0.01f); }
        // optional slight delay so that resuming on a tombstone level doesn't mess things up
        scripts = FindObjectOfType<Scripts>();
        scripts.itemManager.CreateWeaponWithStats(Save.persistent.tsItemNames[0], "rusty", Save.persistent.tsWeaponAcc - 1, Save.persistent.tsWeaponSpd - 1, Save.persistent.tsWeaponDmg - 1, Save.persistent.tsWeaponDef - 1);
        // make the item with worse stats
        for (int i = 1; i < Save.persistent.tsItemNames.Length; i++) {
            if (Save.persistent.tsItemNames[i] != null && Save.persistent.tsItemNames[i] != "") { 
                GameObject created = scripts.itemManager.CreateItem(Save.persistent.tsItemNames[i].Replace(' ', '_'), Save.persistent.tsItemTypes[i], Save.persistent.tsItemMods[i]);
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
        scripts.itemManager.CreateWeaponWithStats(Save.game.floorItemNames[0], Save.game.floorItemMods[0], Save.game.floorAcc, Save.game.floorSpd, Save.game.floorDmg, Save.game.floorDef);
        // spawn the floor's weapon first
        for (int i = 1; i < 9; i++) {  
            if (Save.game.floorItemNames[i] != null && Save.game.floorItemNames[i] != "") {
                GameObject created = scripts.itemManager.CreateItem(Save.game.floorItemNames[i], Save.game.floorItemTypes[i], Save.game.floorItemMods[i]);
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
            if (Save.game.floorItemNames[i] != null && Save.game.floorItemNames[i] != "") {
                GameObject created = scripts.itemManager.CreateItem(Save.game.floorItemNames[i], Save.game.floorItemTypes[i], Save.game.floorItemMods[i]);
            }
        }
        // merchant does not have items, so just spawn them in right away
    }
}