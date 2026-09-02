using System;
using System.Collections.Generic;

/// <summary>
/// Serializable snapshot of the current run. This is the authoritative save payload for combat,
/// inventory, encounter state, delayed item effects, and other mid-run bookkeeping.
/// </summary>
[Serializable]
public class GameData {
    public bool newGame; // true until a run has progressed far enough to count as resumable
    public int curCharNum; // current player character index for the active run
    public string[] floorItemNames; // current floor encounter item names laid out in the scene
    public string[] floorItemTypes; // parallel type array for `floorItemNames`
    public string[] floorItemMods; // parallel modifier array for `floorItemNames`
    public int[] floorItemAccs; // serialized weapon aim bonuses for current floor items
    public int[] floorItemSpds; // serialized weapon speed bonuses for current floor items
    public int[] floorItemDmgs; // serialized weapon damage bonuses for current floor items
    public int[] floorItemDefs; // serialized weapon defense bonuses for current floor items
    public string[] resumeItemNames; // player inventory/loadout item names restored on resume
    public string[] resumeItemTypes; // player inventory/loadout item types restored on resume
    public string[] resumeItemMods; // player inventory/loadout modifiers restored on resume
    public int resumeLevel; // level restored when continuing a run
    public int resumeSub; // subfloor restored when continuing a run
    public int resumeAcc; // player base aim restored on resume
    public int resumeSpd; // player base speed restored on resume
    public int resumeDmg; // player base damage restored on resume
    public int resumeDef; // player base defense restored on resume
    public int floorAcc; // legacy current-floor weapon aim snapshot
    public int floorSpd; // legacy current-floor weapon speed snapshot
    public int floorDmg; // legacy current-floor weapon damage snapshot
    public int floorDef; // legacy current-floor weapon defense snapshot
    public int potionAcc; // potion-stored temporary aim bonus
    public int potionSpd; // potion-stored temporary speed bonus
    public int potionDmg; // potion-stored temporary damage bonus
    public int potionDef; // potion-stored temporary defense bonus
    public int playerStamina; // current player stamina pool
    public int enemyStamina; // current enemy stamina pool
    public List<int> diceNumbers; // serialized die face values for all live dice
    public List<string> diceTypes; // serialized die color names for all live dice
    public List<string> dicePlayerOrEnemy; // serialized die ownership values: none, player, or enemy
    public List<string> diceAttachedToStat; // serialized attached stat row for each live die
    public List<bool> diceRerolled; // whether each saved die has already consumed a reroll
    public List<bool> diceTarotUpgraded; // whether each saved die already received a tarot upgrade
    public List<bool> diceCursedSpawned; // whether each saved die originated from cursed dice effects
    public List<string> playerWounds; // player wounds currently active in the run
    public List<string> enemyWounds; // enemy wounds currently active in the encounter
    public bool playerBleedsOutNextRound; // delayed death flag for player bleedout effects
    public bool enemyBleedsOutNextRound; // delayed death flag for enemy bleedout effects
    public int enemyNum; // active encounter index in `Enemy.enemyArr`
    public bool usedMace; // once-per-round or once-per-encounter mace effect consumed
    public bool usedAnkh; // ankh revive effect consumed
    public bool usedSpellbook; // spellbook use consumed for the current fight/turn state
    public bool usedHelm; // helm of might use consumed
    public bool usedBoots; // boots of dodge use consumed
    public bool isFurious; // player currently has fury status
    public bool isDodgy; // player currently has dodge status
    public bool isHasty; // player currently has haste status
    public bool isBloodthirsty; // player currently has bloodthirst status
    public bool isCourageous; // player currently has courage status
    public bool isDestructive; // player currently has destructive status
    public bool isFortified; // player currently has fortified status
    public bool isEmpowered; // player currently has empowered status
    public int expendedStamina; // stamina already spent this round for effects that care about totals
    public int numItemsDroppedForTrade; // merchant/trade bookkeeping for floor drops
    public bool blacksmithHasForged; // true once the blacksmith already performed its forge action
    public int discardableDieCounter; // number of enemy die discard actions still pending from wounds/effects
    public bool enemyScrollChestActive; // enemy chest scroll effect armed for the next relevant hit
    public bool enemyScrollGutsActive; // enemy guts scroll effect armed for the next relevant hit
    public bool enemyScrollKneeActive; // enemy knee scroll effect armed for the next relevant hit
    public bool enemyScrollHipActive; // enemy hip scroll effect armed for the next relevant hit
    public bool enemyScrollHandActive; // enemy hand scroll effect armed for the next relevant hit
    public bool enemyScrollArmpitsActive; // enemy armpits scroll effect armed for the next relevant hit
    public int enemyWitchHandPenaltyGreen; // temporary enemy green penalty from witch hand-like effects
    public int enemyWitchHandPenaltyBlue; // temporary enemy blue penalty from witch hand-like effects
    public int enemyWitchHandPenaltyRed; // temporary enemy red penalty from witch hand-like effects
    public int enemyWitchHandPenaltyWhite; // temporary enemy white penalty from witch hand-like effects
    public bool enemyIsDead; // whether the active encounter has already been defeated
    public int enemyAcc; // serialized enemy base aim
    public int enemySpd; // serialized enemy base speed
    public int enemyDmg; // serialized enemy base damage
    public int enemyDef; // serialized enemy base defense
    public int enemyTargetIndex; // serialized enemy target wound index
    public string[] lastTraderItemNames; // cached merchant/blacksmith inventory names for revisits/resume
    public string[] lastTraderItemTypes; // cached merchant/blacksmith inventory item types
    public string[] lastTraderItemMods; // cached merchant/blacksmith inventory modifiers
    public int[] lastTraderItemAccs; // cached merchant/blacksmith weapon aim bonuses
    public int[] lastTraderItemSpds; // cached merchant/blacksmith weapon speed bonuses
    public int[] lastTraderItemDmgs; // cached merchant/blacksmith weapon damage bonuses
    public int[] lastTraderItemDefs; // cached merchant/blacksmith weapon defense bonuses
    public int lastTraderLevel; // floor where the cached trader inventory was created
    public int lastTraderSub; // subfloor where the cached trader inventory was created
    public int lastTraderEnemyNum; // encounter index that owned the cached trader inventory
    public bool showAmuletSurvivalStatusText; // delayed UI flag for amulet survival messaging
    public bool pendingAmuletInventoryCleanup; // deferred inventory cleanup after amulet survival triggers
    public bool pendingAmuletVisualRestore; // deferred visual refresh after amulet survival triggers
    public bool enemyHasKatarSpeedPenalty; // enemy currently suffers a katar speed debuff
    public int enemyKatarSpeedPenaltyAmount; // size of the current katar speed debuff
    public int enemyKatarBaseSpeedAfterPenalty; // cached post-penalty base speed for later restoration
    public bool isFirstCombatRoundOfEncounter; // true until the opening combat round has fully resolved
    public bool pendingMirrorCopy; // player is waiting to click a die for the mirror-copy effect
    public bool pendingSpellbookTransmute; // player is waiting to click a die for spellbook transmute
    public string pendingGemTransformColor; // selected gem color waiting for an attached-player-die click
    public bool pendingDeathcapRestore; // delayed deathcap restoration still needs to fire
    public int itemsFoundThisFloor; // number of items found on the current floor
    public int itemsFoundThisFloorLevel; // level where `itemsFoundThisFloor` was counted
    public int itemsFoundThisFloorSub; // subfloor where `itemsFoundThisFloor` was counted
    public bool warhammerStunActive; // current round warhammer stun is active on the enemy
    public bool warhammerStunNextTurn; // warhammer stun has been queued for the next turn
    public int merchantStealAllowanceRemaining; // how many merchant steals are still legal this encounter
    public int pendingLevelStartStaminaBonus; // delayed stamina bonus to apply at the next level start
    public int luckyStatGreen; // stored lucky dice green bonus for the round
    public int luckyStatBlue; // stored lucky dice blue bonus for the round
    public int luckyStatRed; // stored lucky dice red bonus for the round
    public int luckyStatWhite; // stored lucky dice white bonus for the round
    public bool hasLuckyDiceRoundStats; // true once lucky dice bonuses have been rolled for the round
    // charm active bonuses (applied this round, earned last round)
    public int charmActiveBonusGreen; // active green charm bonus applied this round
    public int charmActiveBonusBlue; // active blue charm bonus applied this round
    public int charmActiveBonusRed; // active red charm bonus applied this round
    public int charmActiveBonusWhite; // active white charm bonus applied this round
    // charm bonuses earned this round and applied next round
    public int charmPendingBonusGreen; // pending green charm bonus for next round
    public int charmPendingBonusBlue; // pending blue charm bonus for next round
    public int charmPendingBonusRed; // pending red charm bonus for next round
    public int charmPendingBonusWhite; // pending white charm bonus for next round
    public int[] charmActiveProcCounts; // per-charm proc counts already applied this round
    public int[] charmPendingProcCounts; // per-charm proc counts queued for next round
    // glass sword shatter state (prevents double-shatter)
    public bool glassSwordShattered; // whether the glass sword has already shattered in this run state
    public float sacrificialChaliceCharge; // fractional stored charge for the sacrificial chalice item
    // set to true when the player carries all 5 gem types simultaneously
    public bool isThanos; // gem-set completion flag checked by item systems

    /// <summary>
    /// Initializes a brand-new run snapshot with default values and correctly sized arrays.
    /// </summary>
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
        diceCursedSpawned = new List<bool>();
        playerWounds = new List<string>();
        enemyWounds = new List<string>();
        playerBleedsOutNextRound = false;
        enemyBleedsOutNextRound = false;
        enemyNum = 0;
        usedMace = false;
        usedAnkh = false;
        usedSpellbook = false;
        usedHelm = false;
        usedBoots = false;
        isFurious = false;
        isDodgy = false;
        isHasty = false;
        isBloodthirsty = false;
        isCourageous = false;
        isDestructive = false;
        isFortified = false;
        isEmpowered = false;
        expendedStamina = 0;
        numItemsDroppedForTrade = 0;
        blacksmithHasForged = false;
        discardableDieCounter = 0;
        enemyScrollChestActive = false;
        enemyScrollGutsActive = false;
        enemyScrollKneeActive = false;
        enemyScrollHipActive = false;
        enemyScrollHandActive = false;
        enemyScrollArmpitsActive = false;
        enemyWitchHandPenaltyGreen = 0;
        enemyWitchHandPenaltyBlue = 0;
        enemyWitchHandPenaltyRed = 0;
        enemyWitchHandPenaltyWhite = 0;
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
        enemyKatarSpeedPenaltyAmount = 0;
        enemyKatarBaseSpeedAfterPenalty = 0;
        isFirstCombatRoundOfEncounter = true;
        pendingMirrorCopy = false;
        pendingSpellbookTransmute = false;
        pendingGemTransformColor = "";
        pendingDeathcapRestore = false;
        itemsFoundThisFloor = 0;
        itemsFoundThisFloorLevel = 0;
        itemsFoundThisFloorSub = 0;
        warhammerStunActive = false;
        warhammerStunNextTurn = false;
        merchantStealAllowanceRemaining = 0;
        pendingLevelStartStaminaBonus = 0;
        hasLuckyDiceRoundStats = false;
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

    /// <summary>
    /// Repairs null or legacy save data in-place after loading older files.
    /// </summary>
    public void Normalize() {
        diceNumbers ??= new List<int>();
        diceTypes ??= new List<string>();
        dicePlayerOrEnemy ??= new List<string>();
        diceAttachedToStat ??= new List<string>();
        diceRerolled ??= new List<bool>();
        diceTarotUpgraded ??= new List<bool>();
        diceCursedSpawned ??= new List<bool>();
        playerWounds ??= new List<string>();
        enemyWounds ??= new List<string>();

        floorItemNames ??= new string[9];
        floorItemTypes ??= new string[9];
        floorItemMods ??= new string[9];
        resumeItemNames ??= new string[9];
        resumeItemTypes ??= new string[9];
        resumeItemMods ??= new string[9];

        // tarot/cursed arrays were added after the original dice save lists, so older saves may be shorter
        while (diceTarotUpgraded.Count < diceNumbers.Count) {
            diceTarotUpgraded.Add(false);
        }
        if (diceTarotUpgraded.Count > diceNumbers.Count) {
            diceTarotUpgraded.RemoveRange(diceNumbers.Count, diceTarotUpgraded.Count - diceNumbers.Count);
        }

        while (diceCursedSpawned.Count < diceNumbers.Count) {
            diceCursedSpawned.Add(false);
        }
        if (diceCursedSpawned.Count > diceNumbers.Count) {
            diceCursedSpawned.RemoveRange(diceNumbers.Count, diceCursedSpawned.Count - diceNumbers.Count);
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

        // non-boss living enemies die permanently on the third wound in older saves too, so recover that state here
        if (enemyNum is not Enemy.MerchantEnemyNum and not Enemy.TombstoneEnemyNum and not Enemy.BlacksmithEnemyNum && enemyNum != 2 && enemyWounds.Count >= 3) {
            enemyIsDead = true;
            enemyBleedsOutNextRound = false;
        }

        if (enemyNum is Enemy.MerchantEnemyNum or Enemy.TombstoneEnemyNum or Enemy.BlacksmithEnemyNum || enemyIsDead) {
            // non-combat encounters and dead encounters must not preserve mid-combat delayed effects
            isFirstCombatRoundOfEncounter = false;
            pendingMirrorCopy = false;
            pendingSpellbookTransmute = false;
            pendingGemTransformColor = "";
            pendingDeathcapRestore = false;
            enemyHasKatarSpeedPenalty = false;
            enemyKatarSpeedPenaltyAmount = 0;
            enemyKatarBaseSpeedAfterPenalty = 0;
            enemyScrollChestActive = false;
            enemyScrollGutsActive = false;
            enemyScrollKneeActive = false;
            enemyScrollHipActive = false;
            enemyScrollHandActive = false;
            enemyScrollArmpitsActive = false;
            enemyWitchHandPenaltyGreen = 0;
            enemyWitchHandPenaltyBlue = 0;
            enemyWitchHandPenaltyRed = 0;
            enemyWitchHandPenaltyWhite = 0;
            if (enemyNum != Enemy.MerchantEnemyNum) {
                merchantStealAllowanceRemaining = 0;
            }
        }

        // older saves stored the first floor weapon stats only in the legacy floorAcc/floorSpd/floorDmg/floorDef fields
        if (floorItemTypes.Length > 0 && floorItemTypes[0] == "weapon" && floorItemAccs[0] == 0 && floorItemSpds[0] == 0 && floorItemDmgs[0] == 0 && floorItemDefs[0] == 0) {
            floorItemAccs[0] = floorAcc;
            floorItemSpds[0] = floorSpd;
            floorItemDmgs[0] = floorDmg;
            floorItemDefs[0] = floorDef;
        }
    }
}
