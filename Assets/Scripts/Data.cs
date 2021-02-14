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
    public string[] tsItemMods = new string[9];


    public Data(Scripts scripts, bool setTS) { 
        if (scripts.characterSelector != null) {
            curCharNum = scripts.data.curCharNum;
            newCharNum = scripts.characterSelector.selectionNum;
            easyMode = scripts.characterSelector.easy;
        }
        if (scripts.player != null) { 
            unlockedChars = scripts.data.unlockedChars;
            curCharNum = scripts.player.charNum;
            newCharNum = scripts.player.charNum;
            easyMode = scripts.data.easyMode;
            for (int i = 0; i < scripts.player.inventory.Count; i++) {
                // for every item after the weapon
                Item item = scripts.player.inventory[i].GetComponent<Item>();
                // create a temporary store for the item
                if (item.itemName == "torch") {} // don't do anything for the torch, it just disappears
                else if (item.itemType == "weapon") {
                    tsWeaponAcc = item.weaponStats["green"] - 1;
                    tsWeaponSpd = item.weaponStats["blue"] - 1;
                    tsWeaponDmg = item.weaponStats["red"] - 1;
                    tsWeaponDef = item.weaponStats["white"] - 1;
                    tsItemNames[0] = scripts.player.inventory[0].GetComponent<Item>().itemName.Split(' ')[1];
                    tsItemTypes[0] = "weapon";
                    tsItemMods[i] = scripts.player.inventory[i].GetComponent<Item>().modifier.Split(' ')[0];
                }
                else {
                    tsItemNames[i] = scripts.player.inventory[i].GetComponent<Item>().itemName;
                    tsItemTypes[i] = scripts.player.inventory[i].GetComponent<Item>().itemType;
                    tsItemMods[i] = scripts.player.inventory[i].GetComponent<Item>().modifier;
                }
            }
            // save tombstone data
            if (setTS) { 
                tsLevel = scripts.levelManager.level;
                tsSub = scripts.levelManager.sub;
            }
            else { 
                tsLevel = scripts.data.tsLevel;
                tsSub = scripts.data.tsSub;
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
