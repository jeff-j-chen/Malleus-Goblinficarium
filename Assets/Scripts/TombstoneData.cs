﻿using System.Collections;
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
        // re-assign scripts here, because as this is a singleton the scripts from the last round has literally everything gone        
        if (scripts.levelManager.level == 4 && scripts.levelManager.sub == 1) { 
            scripts.data.tsLevel = 3;
            scripts.data.tsSub = 3;
            // game will crash if we go to 4-1*, easy solution here
        }
        else { 
            if (scripts.levelManager.sub == 4) { scripts.data.tsSub = 3; }
            else { scripts.data.tsSub = scripts.levelManager.sub; }
            scripts.data.tsLevel = scripts.levelManager.level;
        }
        scripts.data.newGame = true;
        scripts.SaveDataToFile();
        for (int i = 0; i < scripts.itemManager.floorItems.Count; i++){
            scripts.itemManager.MoveToInventory(0, true);
            // move all items on the floor to the player's inventory
        }
        foreach (GameObject toBeDeleted in scripts.player.inventory.ToList()) {
            if (toBeDeleted.GetComponent<Item>().itemName != "retry") {
                toBeDeleted.GetComponent<Item>().Remove(selectNew:false);
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
        if (delay) { yield return new WaitForSeconds(0.01f); }
        // optional slight delay so that resuming on a tombstone level doesn't mess things up
        scripts = FindObjectOfType<Scripts>();
        scripts.itemManager.CreateWeaponWithStats(scripts.data.tsItemNames[0], "rusty", scripts.data.tsWeaponAcc - 1, scripts.data.tsWeaponSpd - 1, scripts.data.tsWeaponDmg - 1, scripts.data.tsWeaponDef - 1);
        for (int i = 1; i < 9; i++) {  
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
    }
}