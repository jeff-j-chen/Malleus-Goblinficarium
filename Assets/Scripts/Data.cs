using UnityEngine;

[System.Serializable]
public class Data {
    // CHARACTER
    public int curCharNum;
    public int newCharNum;

    // TOMBSTONE DATA
    public int tsLevel;
    public int tsSub;
    public string tsWeaponName;
    public int tsWeaponAcc;
    public int tsWeaponSpd;
    public int tsWeaponDmg;
    public int tsWeaponDef;
    public string[] tsItemNames = new string[9];
    public string[] tsItemTypes = new string[9];
    public string[] tsItemMod = new string[9];


    public Data(Scripts scripts, bool setTS) { 
        if (scripts.characterSelector != null) {
            newCharNum = scripts.characterSelector.selectionNum;
        }
        if (scripts.player != null) { 
            Debug.Log("saving charnum!");
            curCharNum = scripts.player.charNum;
            Debug.Log("saved charnum, curCharNum is now {curCharNum}");
            for (int i = 0; i < scripts.player.inventory.Count; i++) {
                // for every item after the weapon
                Item item = scripts.player.inventory[i].GetComponent<Item>();
                // create a temporary store for the item
                if (item.itemName == "torch") {} // don't do anything for the torch, it just disappears
                else if (item.itemType == "weapon") {
                    tsWeaponName = item.itemName.Split(' ')[1];
                    tsWeaponAcc = item.weaponStats["green"] - 1;
                    tsWeaponSpd = item.weaponStats["blue"] - 1;
                    tsWeaponDmg = item.weaponStats["red"] - 1;
                    tsWeaponDef = item.weaponStats["white"] - 1;
                    setItemData(scripts, 0);
                }
                else {
                    setItemData(scripts, i);
                }
            }
            // save tombstone data
            if (setTS) { 
                tsLevel = scripts.levelManager.level;
                tsSub = scripts.levelManager.sub;
            }
        }
    }

    public void resetTSLevel(Scripts scripts) {
        tsLevel = -1;
        tsSub = -1;
        SaveSystem.SaveData(scripts, false);
    }

    private void setItemData(Scripts scripts, int i) {
        tsItemNames[i] = scripts.player.inventory[i].GetComponent<Item>().itemName;
        tsItemTypes[i] = scripts.player.inventory[i].GetComponent<Item>().itemType;
        tsItemMod[i] = scripts.player.inventory[i].GetComponent<Item>().modifier;
    }
}
