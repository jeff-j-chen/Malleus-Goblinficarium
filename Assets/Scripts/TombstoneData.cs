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
        scripts.data.tsItemNames = new string[9];
        scripts.data.tsItemNames = new string[9];
        scripts.data.tsItemNames = new string[9];
        // clear data before placing in new stuff
        Item item = scripts.player.inventory[0].GetComponent<Item>();
        scripts.data.tsWeaponAcc = item.weaponStats["green"];
        scripts.data.tsWeaponSpd = item.weaponStats["blue"];
        scripts.data.tsWeaponDmg = item.weaponStats["red"];
        scripts.data.tsWeaponDef = item.weaponStats["white"];
        scripts.data.tsItemNames[0] = item.itemName.Split(' ')[1];
        scripts.data.tsItemTypes[0] = item.itemType;
        scripts.data.tsItemMods[0] = item.modifier;
        for (int i = 1; i < scripts.player.inventory.Count; i++) {
            item = scripts.player.inventory[i].GetComponent<Item>();
            scripts.data.tsItemNames[i] = item.itemName;
            scripts.data.tsItemTypes[i] = item.itemType;
            scripts.data.tsItemMods[i] = item.modifier;
        }
        // set the tombstone data to whatever the player currently has
        scripts.SaveDataToFile();
        if (scripts.levelManager.level == 4 && scripts.levelManager.sub == 1) { 
            scripts.data.tsLevel = 3;
            scripts.data.tsSub = 3;
            // game will crash if we go to 4-1*, easy solution here
        }
        else { 
            if (scripts.levelManager.sub == 4) { scripts.data.tsSub = 3; }
            else if (scripts.levelManager.sub == 1 && scripts.levelManager.level == 1){ 
                scripts.data.tsSub = -1;
                scripts.data.tsLevel = -1;
            }
            else { 
                scripts.data.tsSub = scripts.levelManager.sub; 
                scripts.data.tsLevel = scripts.levelManager.level;
            }
        }
        // assign the level of which the tombstone will appear on
        scripts.data.newGame = true;
        // player died, so make the next game a new one
        scripts.SaveDataToFile();
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
        scripts.itemManager.CreateWeaponWithStats(scripts.data.tsItemNames[0], "rusty", scripts.data.tsWeaponAcc - 1, scripts.data.tsWeaponSpd - 1, scripts.data.tsWeaponDmg - 1, scripts.data.tsWeaponDef - 1);
        // make the item with worse stats
        for (int i = 1; i < scripts.data.tsItemNames.Length; i++) {
            if (scripts.data.tsItemNames[i] != null && scripts.data.tsItemNames[i] != "") { 
                GameObject created = scripts.itemManager.CreateItem(scripts.data.tsItemNames[i].Replace(' ', '_'), scripts.data.tsItemTypes[i], scripts.data.tsItemMods[i]);
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
            if (scripts.data.merchantItemNames[i] != null && scripts.data.merchantItemNames[i] != "") { 
                print($"creating a {scripts.data.merchantItemNames[i]}");
                GameObject created = scripts.itemManager.CreateItem(scripts.data.merchantItemNames[i], scripts.data.merchantItemTypes[i], scripts.data.merchantItemMods[i]);
            }
        }
        // just spawn in the goods 
    }
}