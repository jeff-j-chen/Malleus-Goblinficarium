using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Data {
    public bool newGame;
    public int curCharNum;
    public int newCharNum;
    public bool easyMode;
    public bool[] unlockedChars;
    public int tsLevel;
    public int tsSub;
    public int tsWeaponAcc;
    public int tsWeaponSpd;
    public int tsWeaponDmg;
    public int tsWeaponDef;
    public string[] tsItemNames;
    public string[] tsItemTypes;
    public string[] tsItemMods;
    public string[] floorItemNames;
    public string[] floorItemTypes;
    public string[] floorItemMods;
    public string[] merchantItemNames;
    public string[] merchantItemTypes;
    public string[] merchantItemMods;
    public string[] resumeItemNames;
    public string[] resumeItemTypes;
    public string[] resumeItemMods;
    public int resumeLevel;
    public int resumeSub;
    public int resumeAcc;
    public int resumeSpd;
    public int resumeDmg;
    public int resumeDef;
    public int potionAcc;
    public int potionSpd;
    public int potionDmg;
    public int potionDef;
    public int playerStamina;
    public int enemyStamina;
    public List<int> diceNumbers;
    public List<string> diceTypes;
    public List<string> dicePlayerOrEnemy;
    public List<string> diceAttachedToStat;
    public List<bool> diceRerolled;
    public List<string> playerWounds;
    public List<string> enemyWounds;
    public int enemyNum;
    public bool usedMace;
    public bool usedAnkh;
    public bool usedHelm;
    public bool usedBoots;
    public int numItemsDroppedForTrade;
    public int discardableDieCounter;
    public bool enemyIsDead;
    public int enemyAcc;
    public int enemySpd;
    public int enemyDmg;
    public int enemyDef;

    public Data() {
        // WHEN TO SAVE THE FOLLOWING DATA:
        // when the player loads in:
        curCharNum = 0; // done
        // after the character makes their seelction:
        newCharNum = 0; // done
        // whenever the player toggles easy:
        easyMode = false; // done
        // after player beats the game: 
        unlockedChars = new bool[4] { true, false, false, false}; // done
        // before the player dies:
        tsLevel = -1; // done
        tsSub = -1; // done
        tsWeaponAcc = 0; // done
        tsWeaponSpd = 0; // done
        tsWeaponDmg = 0; // done
        tsWeaponDef = 0; // done
        tsItemNames = new string[9]; // done
        tsItemTypes = new string[9]; // done
        tsItemMods = new string[9]; // done
        // after the enemy dies, spawning its items:
        floorItemNames = new string[9]; // done
        floorItemTypes = new string[9]; // done
        floorItemMods = new string[9]; // done
        // after the merchant spawns its items:
        merchantItemNames = new string[9]; // done
        merchantItemTypes = new string[9]; // done
        merchantItemMods = new string[9]; // done
        // after a player updates their inventory:
        resumeItemNames = new string[9]; // done
        resumeItemTypes = new string[9]; // done
        resumeItemMods = new string[9]; // done
        // after the player goes to the next level:
        resumeLevel = 0; // done
        resumeSub = 0; // done
        // after the player picks up a new weapon:
        resumeAcc = 0; // done
        resumeSpd = 0; // done
        resumeDmg = 0; // done
        resumeDef = 0; // done
        // after the player drinks a potion:
        potionAcc = 0; // done
        potionSpd = 0; // done
        potionDmg = 0; // done
        potionDef = 0; // done
        // after the player expends stamina, make sure to refund when the player 
        playerStamina = 0; // done
        // turn after the enemy expends stamina:
        enemyStamina = 0; // done
        // after die are generated, and after a die is attached:
        diceNumbers = new List<int>(); // done
        diceTypes = new List<string>(); // done
        dicePlayerOrEnemy = new List<string>(); // done
        diceAttachedToStat = new List<string>(); // done
        // after a die is rerolled:
        diceRerolled = new List<bool>(); // done
        // after the player suffers a wound:
        playerWounds = new List<string>(); // done
        // after the enemy suffers a wound:
        enemyWounds = new List<string>(); // done
        // after the enemy is spawned, and after the devil's cloak is shattered:
        enemyNum = 0; // done
        // whenever the following items are used:
        usedMace = false; // done
        usedAnkh = false; // done
        usedHelm = false; // done
        usedBoots = false; // done
        // after the player drops an item on a merchant level:
        numItemsDroppedForTrade = 0; // done
        // after the player gets more to discard:
        discardableDieCounter = 0; // done
        // after the enemy is spawned or defeated:
        enemyIsDead = false;
        // after the enemy is spawned:
        enemyAcc = 0; // done
        enemySpd = 0; // done
        enemyDmg = 0; // done
        enemyDef = 0; // done
    }
}
