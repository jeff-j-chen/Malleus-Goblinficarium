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
        level = scripts.levelManager.level;
        sub = scripts.levelManager.sub;
        Item item = scripts.player.inventory[0].GetComponent<Item>();
        GameObject created = scripts.itemManager.CreateWeaponWithStats(item.itemName.Split(' ')[1], item.modifier, item.weaponStats["green"] - 1, item.weaponStats["blue"] - 1, item.weaponStats["red"] - 1, item.weaponStats["white"] - 1);
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
            scripts.itemManager.floorItems.Remove(created);
            created.transform.parent = transform;
            created.transform.position = offScreen;
            items.Add(created);
            // check for items to make them unusable
        }
        foreach (GameObject toBeDeleted in scripts.player.inventory.ToList()) {
            toBeDeleted.GetComponent<Item>().Remove(selectNew:false);
            // remove all items from the player's inventory
            // .ToList() is a trick to prevent ienumerator froma acting up
        }
    }
}
