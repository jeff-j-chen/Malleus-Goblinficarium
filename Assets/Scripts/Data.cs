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
    public string[] tsItemNames;


    public Data(Scripts scripts) { 
        newCharNum = scripts.characterSelector.selectionNum;
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
            }
            else {
                tsItemNames[i] = scripts.player.inventory[i].GetComponent<Item>().itemName;
            }
        }
    }
}
