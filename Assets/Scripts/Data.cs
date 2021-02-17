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


    public Data() {
        easyMode = false;
        unlockedChars = new bool[4] {true, false, false, false};
        tsLevel = 0;
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
    }
}
