using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GameData {
    public bool newGame;
    public int curCharNum;
    public string[] floorItemNames;
    public string[] floorItemTypes;
    public string[] floorItemMods;
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
    public bool isFurious;
    public bool isDodgy;
    public bool isHasty;
    public bool isBloodthirsty;
    public bool isCourageous;
    public int numItemsDroppedForTrade;
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
        resumeItemNames = new string[9];
        resumeItemTypes = new string[9];
        resumeItemMods = new string[9];
        resumeLevel = 1;
        resumeSub = 1;
        resumeAcc = 0;
        resumeSpd = 0;
        resumeDmg = 0;
        resumeDef = 0;
        
        potionAcc = 0; // NOT DONE
        potionSpd = 0; // NOT DONE
        potionDmg = 0; // NOT DONE
        potionDef = 0; // NOT DONE

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

        usedMace = false; // NOT DONE
        usedAnkh = false; // NOT DONE
        usedHelm = false; // NOT DONE
        usedBoots = false; // NOT DONE

        isFurious = false;
        isDodgy = false;
        isHasty = false;
        isBloodthirsty = false;
        isCourageous = false;

        numItemsDroppedForTrade = 0; // NOT DONE
        discardableDieCounter = 0; // NOT DONE

        enemyIsDead = false;
        enemyAcc = 0;
        enemySpd = 0;
        enemyDmg = 0;
        enemyDef = 0;
    }
}
