using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class TombstoneData : MonoBehaviour {
    public int level;
    public int sub;
    public int tempSub;
    Scripts scripts;
    Vector2 offScreen = new Vector2(0f, 15f);

    void Start() {
        scripts = FindObjectOfType<Scripts>();
    }

    public void SetTombstoneData() {
        print("make sure to clear player's saved items and shit here");
        scripts = FindObjectOfType<Scripts>();
        // re-assign scripts here, because as this is a singleton the scripts from the last round has literally everything gone        
        SaveSystem.SaveData(scripts, true);
        scripts.data = SaveSystem.LoadData();
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

    public void SpawnSavedItems() {
        print("make sure to degrade common/rare items!");
        // helm of might -> broken helm
        // boots of dodge -> ruined boots
        // ankh -> shattered ankh
        // kapala -> defiled kapala
        // steak -> rotten steak
        // cheese -> moldy cheese
        // __ weapon -> rusty weapon
        scripts = FindObjectOfType<Scripts>();
        scripts.itemManager.CreateWeaponWithStats(scripts.data.tsItemNames[0], "rusty", scripts.data.tsWeaponAcc, scripts.data.tsWeaponSpd, scripts.data.tsWeaponDmg, scripts.data.tsWeaponSpd);
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
}