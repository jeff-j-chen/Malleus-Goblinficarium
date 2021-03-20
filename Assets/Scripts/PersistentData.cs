using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PersistentData {
    //test
    public bool easyMode;
    public bool[] unlockedChars;
    public int gamesPlayed;
    public int highestLevel;
    public int highestSub;
    public int successfulRuns;
    public int deaths;
    public int attacksParried;
    public int woundsReceived;
    public int woundsInflicted;
    public int[] woundsInflictedArr;
    public int[] weaponUses;
    public int enemiesSlain;
    public int staminaUsed;
    public int armorBroken;
    public int weaponsSwapped;
    public int scrollsRead;
    public int potionsQuaffed;
    public int foodEaten;
    public int shurikensThrown;
    public int itemsTraded;
    public int diceRerolled;
    public int diceDiscarded;
    public PersistentData() {
        easyMode = false;
        unlockedChars = new bool[4] { true, false, false, false };
        gamesPlayed = 0;
        highestLevel = 1;
        highestSub = 1;
        successfulRuns = 0;
        deaths = 0;
        attacksParried = 0;
        woundsReceived = 0;
        woundsInflicted = 0;
        woundsInflictedArr = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        weaponUses = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 };
        enemiesSlain = 0;
        staminaUsed = 0;
        armorBroken = 0;
        weaponsSwapped = 0;
        scrollsRead = 0;
        potionsQuaffed = 0;
        foodEaten = 0;
        shurikensThrown = 0;
        itemsTraded = 0;
        diceRerolled = 0;
        diceDiscarded = 0;
    }
}
