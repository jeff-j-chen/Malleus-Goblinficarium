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
    public List<bool> diceTarotUpgraded;
    public List<string> playerWounds;
    public List<string> enemyWounds;
    public bool playerBleedsOutNextRound;
    public bool enemyBleedsOutNextRound;
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
    public int enemyTargetIndex;
    public string[] lastTraderItemNames;
    public string[] lastTraderItemTypes;
    public string[] lastTraderItemMods;
    public int[] lastTraderItemAccs;
    public int[] lastTraderItemSpds;
    public int[] lastTraderItemDmgs;
    public int[] lastTraderItemDefs;
    public int lastTraderLevel;
    public int lastTraderSub;
    public int lastTraderEnemyNum;
    public bool showAmuletSurvivalStatusText;
    public bool pendingAmuletInventoryCleanup;
    public bool pendingAmuletVisualRestore;
    public bool enemyHasKatarSpeedPenalty;
    public bool isFirstCombatRoundOfEncounter;
    public bool pendingMirrorCopy;
    public bool pendingSpellbookTransmute;
    public int merchantStealAllowanceRemaining;
    public int pendingLevelStartStaminaBonus;
    public int luckyStatGreen;
    public int luckyStatBlue;
    public int luckyStatRed;
    public int luckyStatWhite;
    // charm active bonuses (applied this round, earned last round)
    public int charmActiveBonusGreen;
    public int charmActiveBonusBlue;
    public int charmActiveBonusRed;
    public int charmActiveBonusWhite;
    // charm bonuses earned this round and applied next round
    public int charmPendingBonusGreen;
    public int charmPendingBonusBlue;
    public int charmPendingBonusRed;
    public int charmPendingBonusWhite;
    public int[] charmActiveProcCounts;
    public int[] charmPendingProcCounts;
    // glass sword shatter state (prevents double-shatter)
    public bool glassSwordShattered;
    public float sacrificialChaliceCharge;

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
        diceTarotUpgraded = new List<bool>();
        playerWounds = new List<string>();
        enemyWounds = new List<string>();
        playerBleedsOutNextRound = false;
        enemyBleedsOutNextRound = false;
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
        enemyTargetIndex = 0;
        lastTraderItemNames = new string[9];
        lastTraderItemTypes = new string[9];
        lastTraderItemMods = new string[9];
        lastTraderItemAccs = new int[9];
        lastTraderItemSpds = new int[9];
        lastTraderItemDmgs = new int[9];
        lastTraderItemDefs = new int[9];
        lastTraderLevel = -1;
        lastTraderSub = -1;
        lastTraderEnemyNum = -1;
        showAmuletSurvivalStatusText = false;
        pendingAmuletInventoryCleanup = false;
        pendingAmuletVisualRestore = false;
        enemyHasKatarSpeedPenalty = false;
        isFirstCombatRoundOfEncounter = true;
        pendingMirrorCopy = false;
        pendingSpellbookTransmute = false;
        merchantStealAllowanceRemaining = 0;
        pendingLevelStartStaminaBonus = 0;
        charmActiveBonusGreen = 0;
        charmActiveBonusBlue  = 0;
        charmActiveBonusRed   = 0;
        charmActiveBonusWhite = 0;
        charmPendingBonusGreen = 0;
        charmPendingBonusBlue  = 0;
        charmPendingBonusRed   = 0;
        charmPendingBonusWhite = 0;
        charmActiveProcCounts = new int[11];
        charmPendingProcCounts = new int[11];
        glassSwordShattered = false;
        sacrificialChaliceCharge = 0f;
    }

    public void Normalize() {
        diceNumbers ??= new List<int>();
        diceTypes ??= new List<string>();
        dicePlayerOrEnemy ??= new List<string>();
        diceAttachedToStat ??= new List<string>();
        diceRerolled ??= new List<bool>();
        diceTarotUpgraded ??= new List<bool>();
        playerWounds ??= new List<string>();
        enemyWounds ??= new List<string>();

        floorItemNames ??= new string[9];
        floorItemTypes ??= new string[9];
        floorItemMods ??= new string[9];
        resumeItemNames ??= new string[9];
        resumeItemTypes ??= new string[9];
        resumeItemMods ??= new string[9];

        while (diceTarotUpgraded.Count < diceNumbers.Count) {
            diceTarotUpgraded.Add(false);
        }
        if (diceTarotUpgraded.Count > diceNumbers.Count) {
            diceTarotUpgraded.RemoveRange(diceNumbers.Count, diceTarotUpgraded.Count - diceNumbers.Count);
        }

        if (floorItemAccs == null || floorItemAccs.Length != 9) { floorItemAccs = new int[9]; }
        if (floorItemSpds == null || floorItemSpds.Length != 9) { floorItemSpds = new int[9]; }
        if (floorItemDmgs == null || floorItemDmgs.Length != 9) { floorItemDmgs = new int[9]; }
        if (floorItemDefs == null || floorItemDefs.Length != 9) { floorItemDefs = new int[9]; }
        if (lastTraderItemNames == null || lastTraderItemNames.Length != 9) { lastTraderItemNames = new string[9]; }
        if (lastTraderItemTypes == null || lastTraderItemTypes.Length != 9) { lastTraderItemTypes = new string[9]; }
        if (lastTraderItemMods == null || lastTraderItemMods.Length != 9) { lastTraderItemMods = new string[9]; }
        if (lastTraderItemAccs == null || lastTraderItemAccs.Length != 9) { lastTraderItemAccs = new int[9]; }
        if (lastTraderItemSpds == null || lastTraderItemSpds.Length != 9) { lastTraderItemSpds = new int[9]; }
        if (lastTraderItemDmgs == null || lastTraderItemDmgs.Length != 9) { lastTraderItemDmgs = new int[9]; }
        if (lastTraderItemDefs == null || lastTraderItemDefs.Length != 9) { lastTraderItemDefs = new int[9]; }
        if (charmActiveProcCounts == null || charmActiveProcCounts.Length != 11) { charmActiveProcCounts = new int[11]; }
        if (charmPendingProcCounts == null || charmPendingProcCounts.Length != 11) { charmPendingProcCounts = new int[11]; }

        if (enemyNum is Enemy.MerchantEnemyNum or Enemy.TombstoneEnemyNum or Enemy.BlacksmithEnemyNum || enemyIsDead) {
            isFirstCombatRoundOfEncounter = false;
            pendingMirrorCopy = false;
            pendingSpellbookTransmute = false;
            enemyHasKatarSpeedPenalty = false;
            if (enemyNum != Enemy.MerchantEnemyNum) {
                merchantStealAllowanceRemaining = 0;
            }
        }

        if (floorItemTypes.Length > 0 && floorItemTypes[0] == "weapon" && floorItemAccs[0] == 0 && floorItemSpds[0] == 0 && floorItemDmgs[0] == 0 && floorItemDefs[0] == 0) {
            floorItemAccs[0] = floorAcc;
            floorItemSpds[0] = floorSpd;
            floorItemDmgs[0] = floorDmg;
            floorItemDefs[0] = floorDef;
        }
    }
}
