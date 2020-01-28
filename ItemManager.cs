﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System;

public class ItemManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI lootText;
    [SerializeField] public TextMeshProUGUI itemDesc;
    [SerializeField] private GameObject item;
    [SerializeField] public GameObject highlight;
    [SerializeField] public GameObject highlightedItem;
    [SerializeField] private Sprite[] commonItemSprites;
    [SerializeField] private Sprite[] rareItemSprites;
    [SerializeField] private Sprite[] weaponSprites;
    [SerializeField] private Sprite[] otherSprites;
    [SerializeField] public List<GameObject> floorItems;

    public string[] statArr = new string[] { "green", "blue", "red", "white" };
    public string[] statArr1 = new string[] { "accuracy", "speed", "damage", "parry" };
    public Dictionary<string, int> baseWeapon = new Dictionary<string, int>() { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } };
    public Dictionary<string, Dictionary<string, int>> weaponDict = new Dictionary<string, Dictionary<string, int>>() {
        { "dagger", new Dictionary<string, int>()   { { "green", 2 }, { "blue", 4 }, { "red", 0 }, { "white", 0 } } },
        { "flail", new Dictionary<string, int>()    { { "green", 0 }, { "blue", 3 }, { "red", 1 }, { "white", 0 } } },
        { "hatchet", new Dictionary<string, int>()  { { "green", 1 }, { "blue", 2 }, { "red", 2 }, { "white", 1 } } },
        { "mace", new Dictionary<string, int>()     { { "green", 1 }, { "blue", 3 }, { "red", 2 }, { "white", 0 } } },
        { "maul", new Dictionary<string, int>()     { { "green", -1 }, { "blue", -1 }, { "red", 3 }, { "white", 1 } } },
        { "montante", new Dictionary<string, int>() { { "green", 1 }, { "blue", 1 }, { "red", 3 }, { "white", 2 } } },
        { "scimitar", new Dictionary<string, int>() { { "green", 0 }, { "blue", 2 }, { "red", 1 }, { "white", 2 } } },
        { "rapier", new Dictionary<string, int>()   { { "green", 4 }, { "blue", 2 }, { "red", -1 }, { "white", 1 } } },
        { "spear", new Dictionary<string, int>()    { { "green", 2 }, { "blue", -1 }, { "red", 3 }, { "white", 1 } } },
        { "sword", new Dictionary<string, int>()    { { "green", 1 }, { "blue", 2 }, { "red", 1 }, { "white", 2 } } },
    };  
    private string[] weaponNames = new string[] { "dagger", "flail", "hatchet", "mace", "maul", "montante", "scimitar", "rapier", "spear", "sword" };
    public Dictionary<string, Dictionary<string, int>> modifierDict = new Dictionary<string, Dictionary<string, int>>() {
        { "accurate0", new Dictionary<string, int>() { { "green", 1 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } } },
        { "accurate1", new Dictionary<string, int>() { { "green", 2 }, { "blue", -1 }, { "red", 0 }, { "white", 0 } } },
        { "brisk0", new Dictionary<string, int>()    { { "green", 0 }, { "blue", 1 }, { "red", -1 }, { "white", 0 } } },
        { "brisk1", new Dictionary<string, int>()    { { "green", 0 }, { "blue", 1 }, { "red", 0 }, { "white", -1 } } },
        { "blunt0", new Dictionary<string, int>()    { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 1 } } },
        { "blunt1", new Dictionary<string, int>()    { { "green", 0 }, { "blue", 0 }, { "red", -1 }, { "white", 1 } } },
        { "common0", new Dictionary<string, int>()   { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } } },
        { "common1", new Dictionary<string, int>()   { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } } },
        { "common2", new Dictionary<string, int>()   { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } } },
        { "common3", new Dictionary<string, int>()   { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } } },
        { "common4", new Dictionary<string, int>()   { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } } },
        { "common5", new Dictionary<string, int>()   { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } } },
        { "heavy0", new Dictionary<string, int>()    { { "green", 0 }, { "blue", -1 }, { "red", 1 }, { "white", 0 } } },
        { "heavy1", new Dictionary<string, int>()    { { "green", 0 }, { "blue", -1 }, { "red", 0 }, { "white", 1 } } },
        { "nimble0", new Dictionary<string, int>()   { { "green", 0 }, { "blue", 1 }, { "red", 0 }, { "white", -1 } } },
        { "nimble1", new Dictionary<string, int>()   { { "green", 0 }, { "blue", 1 }, { "red", -1 }, { "white", 1 } } },
        { "precise0", new Dictionary<string, int>()  { { "green", 1 }, { "blue", 0 }, { "red", -1 }, { "white", 0 } } },
        { "precise1", new Dictionary<string, int>()  { { "green", 1 }, { "blue", -1 }, { "red", 0 }, { "white", 0 } } },
        { "ruthless0", new Dictionary<string, int>() { { "green", 1 }, { "blue", -1 }, { "red", 0 }, { "white", 1 } } },
        { "ruthless1", new Dictionary<string, int>() { { "green", 0 }, { "blue", -1 }, { "red", 1 }, { "white", 0 } } },
        { "stable0", new Dictionary<string, int>()   { { "green", -1 }, { "blue", 0 }, { "red", 0 }, { "white", 1 } } },
        { "stable1", new Dictionary<string, int>()   { { "green", 0 }, { "blue", -1 }, { "red", 0 }, { "white", 1 } } },
        { "sharp0", new Dictionary<string, int>()    { { "green", 0 }, { "blue", 0 }, { "red", 1 }, { "white", 0 } } },
        { "sharp1", new Dictionary<string, int>()    { { "green", 1 }, { "blue", -1 }, { "red", 1 }, { "white", 0 } } },
    };
    private string[] modifierNames = new string[] { "accurate0", "accurate1", "brisk0", "brisk1", "blunt0", "blunt1", "common0", "common1", "common2", "common3", "common4", "common5", "heavy0", "heavy1", "nimble0", "nimble1", "precise0", "precise1", "ruthless0", "ruthless1", "stable0", "stable1", "sharp0", "sharp1" };

    public Dictionary<string, string> descriptionDict = new Dictionary<string, string>() {
        {"armour", "protects from one hit"}, // finished
        {"arrow", "proceed to the next level"}, // finished
        {"ankh", "force new draft"}, // finished
        {"boots of dodge", "pay 1 stamina to become dodgy"}, // finished
        {"cheese", "3"}, // finished
        {"dagger", "green dice buff damage"}, // finished
        {"flail", "start with red die"}, // finished
        {"hatchet", "enemy can't choose yellow die"},  // finished
        {"helm of might", "pay 3 stamina to gain a yellow die"}, // finished
        {"kapala", "offer an item to become furious"}, // finished
        {"mace", "reroll all dice still to be picked"}, // finished
        {"maul", "any wound is deadly"}, // finished
        {"montante", ""}, // finished
        {"necklet", ""}, // finished
        {"phylactery", "gain leech buff once wounded"}, //
        {"potion", ""}, // finished
        {"rapier", "gain 3 stamina upon kill"}, // finished
        {"scimitar", "discard enemy's die upon parry"},  // finished
        {"scroll", ""}, // finished
        {"shuriken", "discard enemy's die"}, // finished
        {"skeleton key", "escape the fight"}, // finished
        {"spear", "always choose first die"}, // finished
        {"steak", "5"}, // finished
        {"sword", ""}, // finished
        {"torch", "find more loot"} // finished
    };
    public string[] neckletTypes = new string[] { "solidity", "rapidity", "strength", "defense", "arcane", "nothing" };
    public Dictionary<string, int> neckletStats = new Dictionary<string, int>() { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } };
    public Dictionary<string, int> neckletCounter = new Dictionary<string, int>() { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 }, { "arcane", 1 } };
    private string[] scrollTypes = new string[] { "fury", "haste", "dodge", "leech", "courage", "challenge", "nothing" };
    private string[] potionTypes = new string[] { "accuracy", "speed", "strength", "defense", "might", "life", "nothing" };
    private Sprite[] allSprites;
    private float itemSpacing = 1f;
    private float itemY = -5.3f;
    private Scripts scripts;
    public int col = 0;
    public List<GameObject> curList;
    public int discardableDieCounter = 0;

    public bool usedAnkh = false;
    public bool usedHelm = false;
    public bool usedBoots = false;

    public int numItemsDroppedForTrade = 0;

    void Start() {
        allSprites = commonItemSprites.ToArray().Concat(rareItemSprites.ToArray()).Concat(weaponSprites.ToArray()).Concat(otherSprites.ToArray()).ToArray();
        // create a list 
        scripts = FindObjectOfType<Scripts>();
        curList = scripts.player.inventory;
        lootText.text = "";
        CreateWeaponWithStats("sword", "common", 2, 1, 2, 2);
        MoveToInventory(0, true);
        CreateItem("steak", "common");
        MoveToInventory(0, true);
        Select(curList, 0);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
            if (curList == scripts.player.inventory) { Select(floorItems, 0); }
            else if (curList == floorItems) { Select(scripts.player.inventory, 0); }
            else { print("invalid list to select from"); }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            Select(curList, col - 1, false);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            Select(curList, col + 1, false);
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            highlightedItem.GetComponent<Item>().Use();
        }
    } 
    
    public void Select(List<GameObject> itemList, int c, bool forceDifferentSelection=true) {
        if (c <= itemList.Count - 1 && c >= 0) {
            itemList[c].GetComponent<Item>().Select();
            curList = itemList;
            col = c;
        }
        else {
            if (forceDifferentSelection) {
                if (itemList.Count > 1) {
                    itemList[col - 1].GetComponent<Item>().Select();
                    curList = itemList;
                    col--;
                }
                else {
                    scripts.player.inventory[0].GetComponent<Item>().Select();
                    curList = scripts.player.inventory;
                    col = 0;
                }
            }
            else {
                highlightedItem.GetComponent<Item>().Select();
            }
        }
    }

    private void CreateItem(string itemType) {
        GameObject instantiatedItem = Instantiate(item, new Vector2(-2.75f + floorItems.Count * itemSpacing, itemY), Quaternion.identity);
        Sprite sprite = null;
        if (itemType == "weapon") { sprite = weaponSprites[UnityEngine.Random.Range(0, weaponSprites.Length - 1)]; }
        else if (itemType == "common") { sprite = commonItemSprites[UnityEngine.Random.Range(0, commonItemSprites.Length - 1)]; }
        else if (itemType == "rare") { sprite = rareItemSprites[UnityEngine.Random.Range(0, rareItemSprites.Length - 1)]; }
        else { print("bad item type to create"); }
        instantiatedItem.GetComponent<SpriteRenderer>().sprite = sprite;
        instantiatedItem.transform.parent = gameObject.transform;
        instantiatedItem.GetComponent<Item>().itemName = sprite.name.Replace("_", " ");
        instantiatedItem.GetComponent<Item>().itemType = itemType;
        SetItemStatsImmediately(instantiatedItem);
        floorItems.Add(instantiatedItem);
    }

    public void CreateItem(string itemName, string itemType) {
        GameObject instantiatedItem = Instantiate(item, new Vector2(-2.75f + floorItems.Count * itemSpacing, itemY), Quaternion.identity);
        instantiatedItem.GetComponent<SpriteRenderer>().sprite = allSprites[(from a in allSprites select a.name).ToList().IndexOf(itemName)];
        instantiatedItem.transform.parent = gameObject.transform;
        instantiatedItem.GetComponent<Item>().itemName = instantiatedItem.GetComponent<SpriteRenderer>().sprite.name.Replace("_", " ");
        SetItemStatsImmediately(instantiatedItem);
        instantiatedItem.GetComponent<Item>().itemType = itemType;
        floorItems.Add(instantiatedItem);
    }

    private void SetItemStatsImmediately(GameObject instantiatedItem) {
        // if doing this in item.cs the timing is off and it breaks
        if (instantiatedItem.GetComponent<Item>().itemName == "necklet") {
            instantiatedItem.GetComponent<Item>().modifier = neckletTypes[UnityEngine.Random.Range(0, 5)];
        }
        else if (instantiatedItem.GetComponent<Item>().itemName == "scroll") {
            instantiatedItem.GetComponent<Item>().modifier = scrollTypes[UnityEngine.Random.Range(0, scrollTypes.Length)];
        }
        else if (instantiatedItem.GetComponent<Item>().itemName == "potion") {
            instantiatedItem.GetComponent<Item>().modifier = potionTypes[UnityEngine.Random.Range(0, potionTypes.Length)];
        }
    }

    public void CreateRandomWeapon() {
        GameObject instantiatedItem = Instantiate(item, new Vector2(-2.75f + floorItems.Count * itemSpacing, itemY), Quaternion.identity);
        int rand = UnityEngine.Random.Range(0, weaponNames.Length);
        instantiatedItem.GetComponent<SpriteRenderer>().sprite = weaponSprites[rand];
        instantiatedItem.transform.parent = gameObject.transform;
        instantiatedItem.GetComponent<Item>().itemType = "weapon";
        string modifier = modifierNames[UnityEngine.Random.Range(0, modifierNames.Length)];
        instantiatedItem.GetComponent<Item>().modifier = modifier.Substring(0, modifier.Length - 1);
        instantiatedItem.GetComponent<Item>().itemName = instantiatedItem.GetComponent<Item>().modifier + " " + instantiatedItem.GetComponent<SpriteRenderer>().sprite.name.Replace("_", " ");
        baseWeapon = weaponDict[weaponNames[rand]];
        foreach (string key in statArr) { baseWeapon[key] += modifierDict[modifier][key]; }
        instantiatedItem.GetComponent<Item>().weaponStats = baseWeapon;
        floorItems.Add(instantiatedItem);
    }

    public void CreateWeaponWithStats(string weaponName, string modifier, int aim, int spd, int atk, int def) {
        GameObject instantiatedItem = Instantiate(item, new Vector2(-2.75f + floorItems.Count * itemSpacing, itemY), Quaternion.identity);
        Sprite sprite = null;
        sprite = allSprites[(from a in allSprites select a.name).ToList().IndexOf(weaponName)];
        instantiatedItem.GetComponent<SpriteRenderer>().sprite = sprite;
        instantiatedItem.transform.parent = gameObject.transform;
        instantiatedItem.GetComponent<Item>().itemName = modifier + " " + sprite.name.Replace("_", " ");
        instantiatedItem.GetComponent<Item>().itemType = "weapon";
        instantiatedItem.GetComponent<Item>().modifier = modifier;
        baseWeapon["green"] = aim;
        baseWeapon["blue"] = spd;
        baseWeapon["red"] = atk;
        baseWeapon["white"] = def;
        instantiatedItem.GetComponent<Item>().weaponStats = baseWeapon;
        floorItems.Add(instantiatedItem);
    }

    public void MoveToInventory(int index, bool starter=false) {
        if (scripts.player.inventory.Count <= 7) {
            if (!starter) { scripts.soundManager.PlayClip("click"); }
            if (floorItems[index].GetComponent<Item>().itemType == "weapon") { 
                floorItems[index].transform.position = new Vector2(-2.75f, 3.16f);
                scripts.player.stats = floorItems[index].GetComponent<Item>().weaponStats;
                if (!starter) {
                    scripts.turnManager.SetStatusText("you take " + floorItems[index].GetComponent<Item>().itemName.Split(' ')[1]);
                    Destroy(scripts.player.inventory[0]);
                    scripts.player.inventory[0] = floorItems[index]; 
                    scripts.statSummoner.SetDebugInformationFor("player");
                    scripts.statSummoner.SummonStats();
                }
                else {
                    scripts.player.inventory.Add(floorItems[index]);
                }
                floorItems.RemoveAt(0);
                foreach(GameObject item in floorItems) {
                    item.transform.position = new Vector2(item.transform.position.x - 1f, itemY);
                }
                Select(curList, 0);
            }
            else {
                if (floorItems[index].GetComponent<Item>().itemName == "necklet") {
                    if (floorItems[index].GetComponent<Item>().modifier == "solidity") { 
                        neckletStats["green"] += neckletCounter["arcane"]; 
                        neckletCounter["green"]++; 
                    } 
                    else if (floorItems[index].GetComponent<Item>().modifier == "rapidity") { 
                        neckletStats["blue"] += neckletCounter["arcane"];
                        neckletCounter["blue"]++; 
                    } 
                    else if (floorItems[index].GetComponent<Item>().modifier == "strength") { 
                        neckletStats["red"] += neckletCounter["arcane"]; 
                        neckletCounter["red"]++; 
                    } 
                    else if (floorItems[index].GetComponent<Item>().modifier == "defense") { 
                        neckletStats["white"] += neckletCounter["arcane"]; 
                        neckletCounter["white"]++; 
                    }
                    else if (floorItems[index].GetComponent<Item>().modifier == "arcane") {
                        neckletCounter["arcane"]++;    
                        foreach (string stat in statArr) { 
                            neckletStats[stat] = neckletCounter["arcane"] * neckletCounter[stat]; 
                        }
                    } 
                    else if (floorItems[index].GetComponent<Item>().modifier == "nothing") {}
                    else { print("bad modifier"); }
                }
                if (!starter) { scripts.turnManager.SetStatusText("you take " + floorItems[index].GetComponent<Item>().itemName); }
                floorItems[index].transform.position = new Vector2(-2.75f + itemSpacing * scripts.player.inventory.Count, 3.16f);
                scripts.player.inventory.Add(floorItems[index]);
                floorItems.RemoveAt(index);
                for (int i = index; i < floorItems.Count; i++) {
                    floorItems[i].transform.position = new Vector2(floorItems[i].transform.position.x - 1f, itemY);
                }
                Select(curList, index);
            }
        }
    }

    public void SpawnItems() {
        lootText.text = "loot:";
        int spawnCount = 3;
        if (PlayerHas("torch")) { spawnCount = 5; }
        CreateRandomWeapon();
        for (int i = 0; i < UnityEngine.Random.Range(0, spawnCount + scripts.levelManager.level); i++) {
            CreateItem("common");
        }
        if (UnityEngine.Random.Range(0, 10) == 0) { CreateItem("rare"); }
        CreateItem("arrow", "arrow");
    }

    public void SpawnTraderItems() {
        lootText.text = "PLACEHOLDER TEXT";
        for (int i = 0; i < 3; i++) { CreateItem("common"); }
        CreateItem("arrow", "arrow");
    }

    public void HideItems() {
        lootText.text = "";
        Select(scripts.player.inventory, 0);
        foreach (GameObject test in floorItems) {
            test.GetComponent<SpriteRenderer>().sprite = null;
        }
    }
    // HIDE AND DESTROY ARE KEPT SEPARATE, DO NOT MERGE OR DELETE
    public void DestroyItems() {
        foreach (GameObject test in floorItems) {
            Destroy(test);
        }
        floorItems.Clear();
    }

    public bool PlayerHas(string itemName) {
        return (from a in scripts.player.inventory select a.GetComponent<Item>().itemName).Contains(itemName);
    }

    public bool PlayerHasWeapon(string weaponName) {
        return (from a in scripts.player.inventory select a.GetComponent<Item>().itemName).Contains(scripts.player.inventory[0].GetComponent<Item>().itemName.Split(' ')[1]);
    }

    public GameObject GetPlayerItem(string itemName) {
        return scripts.player.inventory[(from a in scripts.player.inventory select a.GetComponent<Item>().itemName).ToList().IndexOf(itemName)];
    }

    public void AttemptFadeTorches() {
        foreach (GameObject item in scripts.player.inventory) {
            if (item.GetComponent<Item>().itemName == "torch") {
                if ($"{scripts.levelManager.level}-{scripts.levelManager.sub}" == item.GetComponent<Item>().modifier) {
                    scripts.turnManager.SetStatusText("your torch runs out");
                    scripts.player.inventory[scripts.player.inventory.IndexOf(item)].GetComponent<Item>().Remove();
                }
            }
        }
    }
}
