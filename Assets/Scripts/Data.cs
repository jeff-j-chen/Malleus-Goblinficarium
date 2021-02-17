using UnityEngine;

[System.Serializable]
public class Data {
    // CHARACTER
    public int curCharNum;
    public int newCharNum;
    public bool easyMode;
    public bool[] unlockedChars;

    // TOMBSTONE DATA
    public int tsLevel;
    public int tsSub;
    public int tsWeaponAcc;
    public int tsWeaponSpd;
    public int tsWeaponDmg;
    public int tsWeaponDef;
    public string[] tsItemNames;
    public string[] tsItemTypes;
    public string[] tsItemMods;

    // MERCHANT ITEMS
    public string[] merchantItemNames;
    public string[] merchantItemTypes;
    public string[] merchantItemMods;

    // CONTINUED GAME
    public int resumeLevel;
    public int resumeSub;
    public int resumeAcc;
    public int resumeSpd;
    public int resumeDmg;
    public int resumeDef;
    public int resumeStamina;
    public string[] resumeItemNames;
    public string[] resumeItemTypes;
    public string[] resumeItemMods;
    // SAVE DICE AND ENEMY STATS, ENEMY NAME HERE, P/E WOUNDS


    public Data(Scripts scripts, bool setTS) {
        // copy all data first, then change it, so we don't get fucked around
        Data oldData = SaveSystem.LoadData();
        // curCharNum = oldData.curCharNum;
        // newCharNum = oldData.newCharNum;
        // easyMode = oldData.easyMode;
        // unlockedChars = oldData.unlockedChars;
        // tsLevel = oldData.tsLevel;
        // tsSub = oldData.tsSub;
        // tsWeaponAcc = oldData.tsWeaponAcc;
        // tsWeaponSpd = oldData.tsWeaponSpd;
        // tsWeaponDmg = oldData.tsWeaponDmg;
        // tsWeaponDef = oldData.tsWeaponDef;
        // tsItemNames = oldData.tsItemNames;
        // tsItemTypes = oldData.tsItemTypes;
        // tsItemMods  = oldData.tsItemMods;
        // merchantItemNames = oldData.merchantItemNames;
        // merchantItemTypes = oldData.merchantItemTypes;
        // merchantItemMods  = oldData.merchantItemMods;
        // resumeLevel = oldData.resumeLevel;
        // resumeSub = oldData.resumeSub;
        // resumeAcc = oldData.resumeAcc;
        // resumeSpd = oldData.resumeSpd;
        // resumeDmg = oldData.resumeDmg;
        // resumeDef = oldData.resumeDef;
        // resumeStamina = oldData.resumeStamina;
        // resumeItemNames = oldData.resumeItemNames;
        // resumeItemTypes = oldData.resumeItemTypes;
        // resumeItemMods  = oldData.resumeItemMods;
        easyMode = true;
        unlockedChars = new bool[4] {true, false, false, false};
        tsLevel = 99;
        tsSub = 0;
        tsWeaponAcc = 0;
        tsWeaponSpd = 0;
        tsWeaponDmg = 0;
        tsWeaponDef = 0;
        tsItemNames = new string[9];
        tsItemTypes = new string[9];
        tsItemMods  = new string[9];
        merchantItemNames = new string[9];
        merchantItemTypes = new string[9];
        merchantItemMods  = new string[9];
        resumeLevel = 0;
        resumeSub = 0;
        resumeAcc = 0;
        resumeSpd = 0;
        resumeDmg = 99;
        resumeDef = 0;
        resumeStamina = 0;
        resumeItemNames = new string[9];
        resumeItemTypes = new string[9];
        resumeItemMods  = new string[9];
        if (setTS) { Debug.Log("setting the tombstone!"); }
        if (scripts.characterSelector != null) {
            curCharNum = oldData.curCharNum;
            newCharNum = scripts.characterSelector.selectionNum;
            easyMode = scripts.characterSelector.easy;
        }
        if (scripts.player != null) { 
            curCharNum = scripts.player.charNum;
            newCharNum = scripts.player.charNum;
            easyMode = oldData.easyMode;
            resumeLevel = scripts.levelManager.level;
            resumeSub = scripts.levelManager.sub;
            resumeStamina = scripts.player.stamina;
            for (int i = 0; i < scripts.player.inventory.Count; i++) {
                // for every item after the weapon
                Item item = scripts.player.inventory[i].GetComponent<Item>();
                // create a temporary store for the item
                if (item.itemName == "torch") {} // don't do anything for the torch, it just disappears
                else if (item.itemType == "weapon") {
                    tsWeaponAcc = resumeAcc = item.weaponStats["green"];
                    tsWeaponSpd = resumeSpd = item.weaponStats["blue"];
                    tsWeaponDmg = resumeDmg = item.weaponStats["red"];
                    tsWeaponDef = resumeDef = item.weaponStats["white"];
                    tsItemNames[0] = resumeItemNames[0] = scripts.player.inventory[0].GetComponent<Item>().itemName.Split(' ')[1];
                    tsItemTypes[0] = resumeItemTypes[0] = "weapon";
                    tsItemMods[0]  = resumeItemMods[0]  = scripts.player.inventory[0].GetComponent<Item>().modifier.Split(' ')[0];
                }
                else {
                    tsItemNames[i] = resumeItemNames[i] = scripts.player.inventory[i].GetComponent<Item>().itemName;
                    tsItemTypes[i] = resumeItemTypes[i] = scripts.player.inventory[i].GetComponent<Item>().itemType;
                    tsItemMods[i]  = resumeItemMods[i]  = scripts.player.inventory[i].GetComponent<Item>().modifier;
                }
            }
            // save tombstone data
            if (setTS) { 
                if (scripts.levelManager.level == 4 && scripts.levelManager.sub == 1) { 
                    tsLevel = 3;
                    tsSub = 3;
                    // game will crash if we go to 4-1*, easy solution here
                }
                else { 
                    if (scripts.levelManager.sub == 4) { tsSub = 3; }
                    else { tsSub = scripts.levelManager.sub; }
                    tsLevel = scripts.levelManager.level;
                }
                resumeAcc = resumeSpd = resumeDmg = resumeDef = 0;
                resumeItemNames = new string[9];
                resumeItemTypes = new string[9];
                resumeItemMods  = new string[9];
                // setting tombstone data, so clear all resume data
            }
            else { 
                tsLevel = oldData.tsLevel;
                tsSub = oldData.tsSub;
                resumeLevel = oldData.resumeLevel;
                resumeSub = oldData.resumeSub;
                resumeStamina = oldData.resumeStamina;
                // KEEP THIS HERE BECAUSE IT DOESN'T SAVE FOR SOME REASON
            }
            if (scripts.levelManager.sub == 4) { 
                Debug.Log("saving merchant wares!");
                bool arrowFound = false;
                for (int i = 0; i < 9; i++) {
                    if (!arrowFound) { 
                        Item item = scripts.itemManager.floorItems[i].GetComponent<Item>();
                        Debug.Log($"saved {scripts.itemManager.floorItems[i].GetComponent<Item>().itemName}");
                        merchantItemNames[i] = scripts.itemManager.floorItems[i].GetComponent<Item>().itemName;
                        merchantItemTypes[i] = scripts.itemManager.floorItems[i].GetComponent<Item>().itemType;
                        merchantItemMods[i] = scripts.itemManager.floorItems[i].GetComponent<Item>().modifier;
                        if (merchantItemNames[i] == "arrow") { arrowFound = true;Debug.Log("found an arrow!"); }
                    }
                    else {
                        Debug.Log($"clearing {i}");
                        merchantItemNames[i] = "";
                        merchantItemTypes[i] = "";
                        merchantItemMods[i] = "";
                    }
                }
            }
        }
    }

    public void ClearMerchantWares(Scripts scripts) { 
        merchantItemNames = new string[9];
        merchantItemTypes = new string[9];
        merchantItemMods  = new string[9];
        SaveSystem.SaveData(scripts, false);
        scripts.data = SaveSystem.LoadData();
    }

    public void resetTSLevel(Scripts scripts) {
        tsLevel = -1;
        tsSub = -1;
        SaveSystem.SaveData(scripts, false);
        scripts.data = SaveSystem.LoadData();
    }
}
