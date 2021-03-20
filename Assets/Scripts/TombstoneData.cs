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

    public void SetTombstoneData() {
        print("make sure to clear player's saved items and stuff here");
        scripts = FindObjectOfType<Scripts>();
        scripts.gameData.tsItemNames = new string[9];
        scripts.gameData.tsItemNames = new string[9];
        scripts.gameData.tsItemNames = new string[9];
        // clear data before placing in new stuff
        Item item = scripts.player.inventory[0].GetComponent<Item>();
        scripts.gameData.tsWeaponAcc = item.weaponStats["green"];
        scripts.gameData.tsWeaponSpd = item.weaponStats["blue"];
        scripts.gameData.tsWeaponDmg = item.weaponStats["red"];
        scripts.gameData.tsWeaponDef = item.weaponStats["white"];
        scripts.gameData.tsItemNames[0] = item.itemName.Split(' ')[1];
        scripts.gameData.tsItemTypes[0] = item.itemType;
        scripts.gameData.tsItemMods[0] = item.modifier;
        for (int i = 1; i < scripts.player.inventory.Count; i++) {
            item = scripts.player.inventory[i].GetComponent<Item>();
            scripts.gameData.tsItemNames[i] = item.itemName;
            scripts.gameData.tsItemTypes[i] = item.itemType;
            scripts.gameData.tsItemMods[i] = item.modifier;
        }
        // set the tombstone data to whatever the player currently has
        if (scripts.levelManager.level == 4 && scripts.levelManager.sub == 1) { 
            scripts.gameData.tsLevel = 3;
            scripts.gameData.tsSub = 3;
            // game will crash if we go to 4-1*, easy solution here
        }
        else { 
            if (scripts.levelManager.sub == 4) { scripts.gameData.tsSub = 3; }
            else if (scripts.levelManager.sub == 1 && scripts.levelManager.level == 1){ 
                scripts.gameData.tsSub = -1;
                scripts.gameData.tsLevel = -1;
            }
            else { 
                scripts.gameData.tsSub = scripts.levelManager.sub; 
                scripts.gameData.tsLevel = scripts.levelManager.level;
            }
        }
        // assign the level of which the tombstone will appear on
        scripts.gameData.newGame = true;
        // player died, so make the next game a new one
        scripts.SaveGameData();
        for (int i = 0; i < scripts.itemManager.floorItems.Count; i++){
            scripts.itemManager.MoveToInventory(0, true);
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
        scripts.itemManager.MoveToInventory(scripts.itemManager.floorItems.IndexOf(retryButton), true);
        // move the button explicitly, because it doesn't seem to want to be moved otherwise
        scripts.persistentData.deaths++;
        scripts.persistentData.gamesPlayed++;
        scripts.SavePersistentData();
    }

    public void SpawnSavedTSItems(bool delay=false) {
        StartCoroutine(SpawnSavedTSItemsCoro(delay));
    }

    public void SpawnSavedMerchantItems(bool delay=false) {
        StartCoroutine(SpawnSavedMerchantItemsCoro(delay));
    }

    public IEnumerator SpawnSavedTSItemsCoro(bool delay) { 
        print("spawning saved ts items!");
        if (delay) { yield return new WaitForSeconds(0.01f); }
        // optional slight delay so that resuming on a tombstone level doesn't mess things up
        scripts = FindObjectOfType<Scripts>();
        scripts.itemManager.CreateWeaponWithStats(scripts.gameData.tsItemNames[0], "rusty", scripts.gameData.tsWeaponAcc - 1, scripts.gameData.tsWeaponSpd - 1, scripts.gameData.tsWeaponDmg - 1, scripts.gameData.tsWeaponDef - 1);
        // make the item with worse stats
        for (int i = 1; i < scripts.gameData.tsItemNames.Length; i++) {
            if (scripts.gameData.tsItemNames[i] != null && scripts.gameData.tsItemNames[i] != "") { 
                GameObject created = scripts.itemManager.CreateItem(scripts.gameData.tsItemNames[i].Replace(' ', '_'), scripts.gameData.tsItemTypes[i], scripts.gameData.tsItemMods[i]);
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
            // decrease 
        }
        scripts.itemManager.CreateItem("arrow", "arrow");
        // create the next level arrow
    }

    public IEnumerator SpawnSavedMerchantItemsCoro(bool delay) { 
        if (delay) { yield return new WaitForSeconds(0.01f); }
        scripts = FindObjectOfType<Scripts>();
        for (int i = 0; i < 9; i++) {  
            if (scripts.gameData.merchantItemNames[i] != null && scripts.gameData.merchantItemNames[i] != "") { 
                print($"creating a {scripts.gameData.merchantItemNames[i]}");
                GameObject created = scripts.itemManager.CreateItem(scripts.gameData.merchantItemNames[i], scripts.gameData.merchantItemTypes[i], scripts.gameData.merchantItemMods[i]);
            }
        }
        // just spawn in the goods 
    }
}