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
        scripts = FindObjectOfType<Scripts>();
        // re-assign scripts here, because as this is a singleton the scripts from the last round has literally everything gone        
        SaveSystem.SaveData(scripts, true);
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
        scripts = FindObjectOfType<Scripts>();
        // re-get scripts
        // for (int i = 0; i < items.Count; i++) {
        //     GameObject savedItem = items[i];
        //     // get the item temporarily
        //     savedItem.GetComponent<Item>().scripts = FindObjectOfType<Scripts>();
        //     // re-get scripts of the itme
        //     savedItem.transform.position = new Vector2(-2.75f + i * scripts.itemManager.itemSpacing, scripts.itemManager.itemY);
        //     // create item with correct offset
        //     scripts.itemManager.floorItems.Add(savedItem);
        //     // add the item to the flooritems
        //     savedItem.transform.parent = scripts.itemManager.transform;
        //     // parent the item to the itemmanager
        // }
        scripts.itemManager.CreateItem("arrow", "arrow");
        // create the next level arrow
    }
}