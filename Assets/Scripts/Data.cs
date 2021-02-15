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
    public string[] tsItemNames = new string[9];
    public string[] tsItemTypes = new string[9];
    public string[] tsItemMods  = new string[9];

    // CONTINUED GAME
    public int resumeLevel;
    public int resumeSub;
    public int resumeAcc;
    public int resumeSpd;
    public int resumeDmg;
    public int resumeDef;
    public int resumeStamina;
    public string[] resumeItemNames = new string[9];
    public string[] resumeItemTypes = new string[9];
    public string[] resumeItemMods  = new string[9];
    // SAVE DICE AND ENEMY STATS, ENEMY NAME HERE, P/E WOUNDS


    public Data(Scripts scripts, bool setTS) {
        unlockedChars = scripts.data.unlockedChars;
        if (scripts.characterSelector != null) {
            curCharNum = scripts.data.curCharNum;
            newCharNum = scripts.characterSelector.selectionNum;
            easyMode = scripts.characterSelector.easy;
        }
        if (scripts.player != null) { 
            curCharNum = scripts.player.charNum;
            newCharNum = scripts.player.charNum;
            easyMode = scripts.data.easyMode;
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
                    tsItemMods[0]  = resumeItemMods[0]  = scripts.player.inventory[i].GetComponent<Item>().modifier.Split(' ')[0];
                }
                else {
                    tsItemNames[i] = resumeItemNames[0] = scripts.player.inventory[i].GetComponent<Item>().itemName;
                    tsItemTypes[i] = resumeItemTypes[0] = scripts.player.inventory[i].GetComponent<Item>().itemType;
                    tsItemMods[i]  = resumeItemMods[0]  = scripts.player.inventory[i].GetComponent<Item>().modifier;
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
                    tsLevel = scripts.levelManager.level;
                    tsSub = scripts.levelManager.sub;
                }
                resumeAcc = resumeSpd = resumeDmg = resumeDef = 0;
                resumeItemNames = new string[9];
                resumeItemTypes = new string[9];
                resumeItemMods  = new string[9];
                // setting tombstone data, so clear all resume data
            }
            else { 
                tsLevel = scripts.data.tsLevel;
                tsSub = scripts.data.tsSub;
                resumeLevel = scripts.data.resumeLevel;
                resumeSub = scripts.data.resumeSub;
                resumeStamina = scripts.data.resumeStamina;
                // KEEP THIS HERE BECAUSE IT DOESN'T SAVE FOR SOME FUCKING REASON
            }
        }
    }

    public void resetTSLevel(Scripts scripts) {
        tsLevel = -1;
        tsSub = -1;
        SaveSystem.SaveData(scripts, false);
        scripts.data = SaveSystem.LoadData();
    }
}
