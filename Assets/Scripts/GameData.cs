using System;
using System.Collections.Generic;
[Serializable]
public class GameData {
    public bool newGame;
    public int curCharNum;
    public string[] floorItemNames;
    public string[] floorItemTypes;
    public string[] floorItemMods;
    public int[] floorItemAccs;
    public int[] floorItemSpds;
    public int[] floorItemDmgs;
    public int[] floorItemDefs;
    public string[] resumeItemNames;
    public string[] resumeItemTypes;
    public string[] resumeItemMods;
    public int resumeLevel;
    public int resumeSub;
    public int resumeAcc;
    public int resumeSpd;
    public int resumeDmg;
    public int resumeDef;
    public int floorAcc;
    public int floorSpd;
    public int floorDmg;
    public int floorDef;
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
    public bool isFurious;
    public bool isDodgy;
    public bool isHasty;
    public bool isBloodthirsty;
    public bool isCourageous;
    public int expendedStamina;
    public int numItemsDroppedForTrade;
    public bool blacksmithHasForged;
    public int discardableDieCounter;
    public bool enemyIsDead;
    public int enemyAcc;
    public int enemySpd;
    public int enemyDmg;
    public int enemyDef;

    public GameData() {
        newGame = true;
        curCharNum = 0;
        floorItemNames = new string[9];
        floorItemTypes = new string[9];
        floorItemMods = new string[9];
        floorItemAccs = new int[9];
        floorItemSpds = new int[9];
        floorItemDmgs = new int[9];
        floorItemDefs = new int[9];
        resumeItemNames = new string[9];
        resumeItemTypes = new string[9];
        resumeItemMods = new string[9];
        resumeLevel = 1;
        resumeSub = 1;
        resumeAcc = 0;
        resumeSpd = 0;
        resumeDmg = 0;
        resumeDef = 0;
        floorAcc = 0;
        floorSpd = 0;
        floorDmg = 0;
        floorDef = 0;
        potionAcc = 0;
        potionSpd = 0;
        potionDmg = 0;
        potionDef = 0;
        playerStamina = 3;
        enemyStamina = 1;
        diceNumbers = new List<int>();
        diceTypes = new List<string>();
        dicePlayerOrEnemy = new List<string>();
        diceAttachedToStat = new List<string>();
        diceRerolled = new List<bool>();
        playerWounds = new List<string>();
        enemyWounds = new List<string>();
        enemyNum = 0;
        usedMace = false;
        usedAnkh = false;
        usedHelm = false;
        usedBoots = false;
        isFurious = false;
        isDodgy = false;
        isHasty = false;
        isBloodthirsty = false;
        isCourageous = false;
        expendedStamina = 0;
        numItemsDroppedForTrade = 0;
        blacksmithHasForged = false;
        discardableDieCounter = 0;
        enemyIsDead = false;
        enemyAcc = 0;
        enemySpd = 0;
        enemyDmg = 0;
        enemyDef = 0;
    }

    public void Normalize() {
        floorItemNames ??= new string[9];
        floorItemTypes ??= new string[9];
        floorItemMods ??= new string[9];
        resumeItemNames ??= new string[9];
        resumeItemTypes ??= new string[9];
        resumeItemMods ??= new string[9];

        if (floorItemAccs == null || floorItemAccs.Length != 9) { floorItemAccs = new int[9]; }
        if (floorItemSpds == null || floorItemSpds.Length != 9) { floorItemSpds = new int[9]; }
        if (floorItemDmgs == null || floorItemDmgs.Length != 9) { floorItemDmgs = new int[9]; }
        if (floorItemDefs == null || floorItemDefs.Length != 9) { floorItemDefs = new int[9]; }

        if (floorItemTypes.Length > 0 && floorItemTypes[0] == "weapon" && floorItemAccs[0] == 0 && floorItemSpds[0] == 0 && floorItemDmgs[0] == 0 && floorItemDefs[0] == 0) {
            floorItemAccs[0] = floorAcc;
            floorItemSpds[0] = floorSpd;
            floorItemDmgs[0] = floorDmg;
            floorItemDefs[0] = floorDef;
        }
    }
}
