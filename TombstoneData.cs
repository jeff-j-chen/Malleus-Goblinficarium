using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class TombstoneData : MonoBehaviour
{
    public List<GameObject> items;
    public int level;
    public int sub;
    Scripts scripts;
    Vector2 offScreen = new Vector2(0f, 15f);

    void Start()
    {
        scripts = FindObjectOfType<Scripts>();
        SetUpSingleton();
        // make sure this object persists
    }
    
    private void SetUpSingleton() {
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
        else {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SetTombstoneData() {
        scripts = FindObjectOfType<Scripts>();
        // re-assign scripts here, because as this is a singleton the scripts from the last round has literally everything gone
        foreach (GameObject remove in items.ToList()) {
            items.Remove(remove);
            Destroy(remove);
        }
        // clear all existing items
        level = scripts.levelManager.level;
        sub = scripts.levelManager.sub;
        // set level according to th
        if (level == 1 && sub == 1) {
            level = -1;
            sub = -1;
        }
        // save level data only if the player got past 1-1
        Item item = scripts.player.inventory[0].GetComponent<Item>();
        GameObject created = scripts.itemManager.CreateWeaponWithStats(item.itemName.Split(' ')[1], "rusty", item.weaponStats["green"] - 1, item.weaponStats["blue"] - 1, item.weaponStats["red"] - 1, item.weaponStats["white"] - 1);
        // create weapon with reduced stats
        scripts.itemManager.floorItems.Remove(created);
        // remove from floor items
        created.transform.parent = transform;
        // child it to this (so not deleted when reloading scene)
        created.transform.position = offScreen;
        // move the item offscreen
        items.Add(created);
        // add the item to the list
        for (int i = 1; i < scripts.player.inventory.Count; i++) {
            // for every item after the weapon
            item = scripts.player.inventory[i].GetComponent<Item>();
            created = scripts.itemManager.CreateItem(item.itemName, item.itemType);
            // recreate the according item (replace so it goes from 'helm of might' -> 'helm_of_might', so we can use it)
            scripts.itemManager.floorItems.Remove(created);
            // remove it from the floor
            created.transform.parent = transform;
            // child it
            created.transform.position = offScreen;
            // move it offscreen
            items.Add(created);
            // add it to the stored array
            // check for items to make them unusable
        }
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
        GameObject retryButton = scripts.itemManager.CreateItem("retry", "retry");
        // create retry button
        scripts.itemManager.MoveToInventory(scripts.itemManager.floorItems.IndexOf(retryButton), true);
        // move the button explicitly, because it doesn't seem to want to be moved otherwise
    }

    public void SpawnSavedItems() {
        print("spawning in the tombstone items");
        foreach (GameObject savedItem in items) {
            savedItem.transform.position = new Vector2(-2.75f + (scripts.itemManager.floorItems.Count - 1) * scripts.itemManager.itemSpacing, scripts.itemManager.itemY);
            scripts.itemManager.floorItems.Add(savedItem);
            savedItem.transform.parent = scripts.itemManager.transform;
        }
        scripts.itemManager.CreateItem("arrow", "arrow");
    }
}