using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public const int MerchantSub = 4;
    public const int BlacksmithSub = 5;
    [SerializeField] private GameObject levelBox;
    [SerializeField] private GameObject loadingCircle;
    [SerializeField] private TextMeshProUGUI levelTransText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] public int level;
    [SerializeField] public int sub;
    [SerializeField] public bool lockActions = false;
    [SerializeField] private string characters = "";
    [SerializeField] private string thinCharacters = "";
    private readonly Vector3 onScreen = new Vector2(0.0502f, -1.533f);
    private readonly Vector3 offScreen = new Vector2(-20f, 15f);
    private readonly float[] balanced = { 5f, 15f, 15f, 5f };
    private readonly float[] fast =     { 5f, 33f, 5f, 5f };
    private readonly float[] damage =   { 15f, 8f, 28f, 8f };
    private readonly float[] defense =  { 5f, 5f, 5f, 38f };
    private readonly float[] mix =      { 5f, -10f, 23f, 23f };
    private readonly Dictionary<string, float[]> levelStats = new() {
        // add on the stats and iterate (add) through with random variance, divide, then round to get final stats
        //                    aim, spd, atk, def, var,   bal/fas/dmg/def/mix
        { "11", new[] { 10f, 10f, 10f, 10f, 0f,          10f, 0f, 0f, 0f, 0f } },
        { "12", new[] { 10f, 10f, 10f, 10f, 0f,          4f, 2f, 2f, 1f, 1f  } },
        { "13", new[] { 10f, 10f, 10f, 10f, 0.75f,       3f, 2f, 2f, 2f, 1f  } },
        { "21", new[] { 10f, 10f, 10f, 10f, 1f,          3f, 2f, 2f, 2f, 1f  } },
        { "22", new[] { 10f, 15f, 10f, 10f, 1f,          1f, 1f, 1f, 6f, 1f  } },
        { "23", new[] { 10f, 10f, 11f, 11f, 1.25f,       2f, 2f, 2f, 2f, 2f  } },
        { "31", new[] { 10f, 10f, 12f, 12f, 1.25f,       1f, 3f, 2f, 2f, 2f  } },
        { "32", new[] { 12f, 15f, 14f, 14f, 1.5f,        0f, 2f, 3f, 2f, 3f  } },
        { "33", new[] { 15f, 15f, 15f, 15f, 3f,          2f, 2f, 2f, 2f, 2f  } }
        // something to make it more probable that genstats will gen more difficult enemies later
    };
    private Coroutine transGlitchCoro;
    private Coroutine debugGlitchCoro;
    private SpriteRenderer boxSR;
    private Color temp;
    private Scripts s;
    private bool hasQueuedWarpDestination;
    private int queuedWarpLevel;
    private int queuedWarpSub;
    private int queuedWarpEnemyNum;
    private bool queuedWarpUseSavedTrader;

    public bool ShouldForceBlacksmithSpawn() {
        return sub == BlacksmithSub;
    }

    private static bool IsVendorSub(int currentSub) {
        return currentSub == MerchantSub || currentSub == BlacksmithSub;
    }

    private static string GetLevelLabel(int currentLevel, int currentSub) {
        return $"level {currentLevel}-{currentSub}";
    }

    private void QueuePendingArrivalBonuses() {
        if (s == null || s.itemManager == null) { return; }

        Save.game.pendingLevelStartStaminaBonus = s.itemManager.GetHamLevelStartBonus();
    }

    private IEnumerator ApplyPendingArrivalBonuses() {
        while (s == null || s.player == null || s.turnManager == null || s.soundManager == null) {
            yield return null;
        }

        if (Save.game.pendingLevelStartStaminaBonus <= 0) { yield break; }

        yield return s.delays[0.25f];
        int staminaBonus = Save.game.pendingLevelStartStaminaBonus;
        Save.game.pendingLevelStartStaminaBonus = 0;
        s.soundManager.PlayClip("eat");
        s.turnManager.ChangeStaminaOf("player", staminaBonus);
        if (s.tutorial == null) { Save.SaveGame(); }
    }

    private void Start() {
        s = FindFirstObjectByType<Scripts>();
        levelText.gameObject.SetActive(PlayerPrefs.GetString(s.DEBUG_KEY) == "on");
        // show the level text only if debug is on
        level = Save.game.resumeLevel;
        sub = Save.game.resumeSub;
        // if going to bugtest devil, do it through gamesave, not here!
        boxSR = levelBox.GetComponent<SpriteRenderer>();
        // get the spriterenderer for the box that covers the screen when the next level is being loaded
        temp = boxSR.color;
        temp.a = 0f;
        boxSR.color = temp;
        // make it transparent
        levelBox.transform.position = offScreen;
        loadingCircle.transform.position = offScreen;
        // make the black box and the loading circle go off the screen
        lockActions = false;
        // make sure actions aren't locked
        if (s.tutorial == null) {
            if (IsVendorSub(sub)) { levelText.text = $"({GetLevelLabel(level, sub)})"; }
            else if (sub == Save.persistent.tsSub && level == Save.persistent.tsLevel && !(sub == 1 && level == 1))
            { levelText.text = $"(level {level}-{sub}*)"; }
            else if (level == 4 && sub == 1) { StartCoroutine(GlitchyDebugText()); }
            else if (s.enemy.enemyName.text == "Lich") { levelText.text = "(level ???)"; }
            else { levelText.text = $"(level {level}-{sub})"; }
        }
        else { levelText.text = ""; }
        StartCoroutine(ApplyPendingArrivalBonuses());
        
    }

    // private void Update() { 
    //     if (Input.GetKeyDown(KeyCode.Space)) { 
    //         NextLevel();
    //     }
    // }

    /// <summary>
    /// Generate the stats for an enemy.
    /// </summary>
    public float[] GenStats(string lichOrDevilOrNormal = "normal") {
        if (lichOrDevilOrNormal != "normal") {
            if (lichOrDevilOrNormal == "lich") {
                if (DifficultyHelper.IsNightmare(Save.persistent.gameDifficulty) || DifficultyHelper.IsHard(Save.persistent.gameDifficulty)) {
                    return new[] { 3f, 3f, 3f, 3f };
                }
                else { 
                    return new[] { 2f, 2f, 2f, 2f };
                }
            }
            else if (lichOrDevilOrNormal == "devil") {
                if (DifficultyHelper.IsNightmare(Save.persistent.gameDifficulty) || DifficultyHelper.IsHard(Save.persistent.gameDifficulty)) {
                    return new[] { 3f, 3f, 3f, 3f };
                }
                else { 
                    return new[] { 2f, 2f, 2f, 2f };
                }
            }
        }
        if (sub >= MerchantSub) { return new[] { 0f, 0f, 0f, 0f }; }
        // given key is not present in the dictionary for sub-4s, instantly return blank
        float[] stats = levelStats[level + sub.ToString()];
        // based on the level and sub, get the stats from the level
        float[] totalStats = new float[4];
        float[] baseStats = level switch {
            // create empty arrays of floats to store the stats in
            1 => GenBaseStats(stats, balanced),
            2 => GenBaseStats(stats, damage),
            3 => GenBaseStats(stats, fast),
            _ => null
        };
        // generate the stats for the enemy based on level 
        for (int i = 0; i < 4; i++) {
            // for every stat, the set the stats to be the combination of level stats (from dictionary), the base stats (from function), and a slight amount of RNG 
            totalStats[i] = Mathf.Round((stats[i] + baseStats[i] + UnityEngine.Random.Range(0f, stats[4]))/10f);
            if (DifficultyHelper.IsNightmare(Save.persistent.gameDifficulty)) {
                totalStats[i] += UnityEngine.Random.Range(0, s.levelManager.level);
            }
        }
        if (DifficultyHelper.IsNightmare(Save.persistent.gameDifficulty)) {
            if (s.levelManager.level == 1 && s.levelManager.sub == 1) {
                totalStats[1] = 0;
                // game is slightly playable by settings speed of 1st enemy to 0 always
            }
            else {
                // all nightmare enemies get speed boost of 1
                totalStats[1] += 1;
            }
        }
        return totalStats;
        // return the total stats
    }

    /// <summary>
    /// Generate the base stats of an enemy.
    /// </summary>
    private float[] GenBaseStats(float[] stats, float[] normal) {
        float sum = stats[5] + stats[6] + stats[7] + stats[8] + stats[9];
        // get the sum of the present chances
        const double tolerance = 0.01f;
        if (Math.Abs(sum - 10f) > tolerance) { 
            // print("not 10f"); 
            return balanced; 
        }
        // something went wrong while setting up the dictionary, so notify me and return (so compiler is happy)
        int rand = UnityEngine.Random.Range(1, 11);
        // get a random number from 1-10
        float[] chances = { stats[5], stats[5] + stats[6], stats[5] + stats[6] + stats[7], stats[5] + stats[6] + stats[7] + stats[8], 10f };
        // create a float array of the different chances
        if (rand >= 0f && rand < chances[0])          { return balanced; }
        if (rand >= chances[0] && rand < chances[1])  { return fast; }
        if (rand >= chances[1] && rand < chances[2])  { return damage; }
        if (rand >= chances[2] && rand < chances[3])  { return defense; }
        if (rand >= chances[3] && rand < chances[4])  { return mix; }
        return normal;
        // return a stat preset based on the chances and the random number
    }

    /// <summary>
    /// Send the player to the next level.
    /// </summary>
    public void NextLevel(bool isLich = false) { StartCoroutine(NextLevelCoroutine(isLich)); }

    public void QueueWarpDestination(int targetLevel, int targetSub, int targetEnemyNum, bool useSavedTrader = false) {
        hasQueuedWarpDestination = true;
        queuedWarpLevel = targetLevel;
        queuedWarpSub = targetSub;
        queuedWarpEnemyNum = targetEnemyNum;
        queuedWarpUseSavedTrader = useSavedTrader;
    }

    public void ClearSavedEnemyCombatStateForEscape() {
        Save.game.enemyStamina = 0;
        Save.game.enemyAcc = 0;
        Save.game.enemySpd = 0;
        Save.game.enemyDmg = 0;
        Save.game.enemyDef = 0;
        Save.game.enemyTargetIndex = 0;
        Save.game.enemyWounds = new();
        Save.game.enemyBleedsOutNextRound = false;
        Save.game.enemyHasKatarSpeedPenalty = false;
        Save.game.discardableDieCounter = 0;
        Save.game.enemyIsDead = false;
        Save.game.isFirstCombatRoundOfEncounter = false;
        Save.game.diceNumbers = new();
        Save.game.diceTypes = new();
        Save.game.dicePlayerOrEnemy = new();
        Save.game.diceAttachedToStat = new();
        Save.game.diceRerolled = new();
        Save.game.diceTarotUpgraded = new();

        if (s == null) { s = FindFirstObjectByType<Scripts>(); }
        if (s?.enemy != null) {
            s.enemy.woundList.Clear();
            s.enemy.stamina = 0;
            s.enemy.targetIndex = 0;
            s.enemy.staminaCounter.text = "0";
        }
        if (s?.statSummoner != null) {
            foreach (string stat in s.itemManager.statArr) {
                s.statSummoner.addedEnemyStamina[stat] = 0;
                s.statSummoner.addedEnemyDice[stat].Clear();
            }
        }
        EnemyAI.InvalidateCachedPlan();
    }

    public void ShowTransferDestination(int targetLevel, int targetSub, int targetEnemyNum) {
        if (boxSR == null) { return; }

        Color tempColor = boxSR.color;
        tempColor.a = 1f;
        boxSR.color = tempColor;
        levelBox.transform.position = onScreen;
        loadingCircle.transform.position = onScreen;
        levelTransText.text = GetTransferDestinationText(targetLevel, targetSub, targetEnemyNum);
        if (s != null && s.soundManager != null) { s.soundManager.PlayClip("next"); }
    }

    private string GetTransferDestinationText(int targetLevel, int targetSub, int targetEnemyNum) {
        if (targetEnemyNum == 2) {
            return "level ???";
        }

        return GetLevelLabel(targetLevel, targetSub);
    }


    /// <summary>
    /// Do not call this coroutine, use NextLevel() instead.
    /// </summary>
    private IEnumerator NextLevelCoroutine(bool isLich=false) {
        if (!lockActions) {
            lockActions = true;
            bool shouldAdvanceTorchCounter = s != null
                && s.enemy != null
                && s.enemy.enemyName.text is not "Merchant" and not "Blacksmith" and not "Tombstone";
            Color tempColor = boxSR.color;
            tempColor.a = 0f;
            boxSR.color = tempColor;
            // hide the level loading box
            s = FindFirstObjectByType<Scripts>();
            // re-get s
            s.soundManager.PlayClip("next");
            // play sound clip
            QueuePendingArrivalBonuses();
            if (level == 4 && sub == 1 && !hasQueuedWarpDestination) {
                if (debugGlitchCoro != null) { StopCoroutine(debugGlitchCoro); }
                // going to next level after having defeated devil
                if (Save.game.curCharNum != 3 && !Save.persistent.unlockedChars[Save.game.curCharNum + 1]) { 
                    // unlock the next character and let them know if it has not yet been unlocked
                    StartCoroutine(IndicateUnlock());
                    Save.persistent.unlockedChars[Save.game.curCharNum + 1] = true;
                    Save.SavePersistent();
                }
                else {
                    Initiate.Fade("Credits", Color.black, 2.5f);
                }
                Save.persistent.IncrementSuccessfulRuns(Save.persistent.gameDifficulty);
                Save.SavePersistent();
                Save.game = new GameData();
                if (s.tutorial == null) { Save.SaveGame(); }
                // for some reason file.delete doesn't want to work here
                yield break;
            }
            s.turnManager.ClearVariablesAfterRound();
            // remove variables before going to next level
            if (s.statSummoner != null) {
                s.statSummoner.ResetDiceAndStamina();
            }
            foreach (GameObject dice in s.diceSummoner.existingDice) {
                StartCoroutine(dice.GetComponent<Dice>().FadeOut(false));
            }
            // fade out all die (die are only faded out upon kill normally)
            // yield return s.delays[1.5f]; // uncomment for tombstone tests
            if (isLich || level == 3 && sub == BlacksmithSub) { 
                s.music.FadeVolume("LaBossa");
                // if spawning lich or devil, fade to boss music
            }
            else if (sub == 3 && s.enemy.enemyName.text != "Tombstone") { 
                s.music.FadeVolume("Smoke");
                // if spawning merchant, fade to smoke, as long as we are not fading from tombstone
            }
            else { 
                if (s.enemy.enemyName.text == "Lich" || s.enemy.enemyName.text == "Blacksmith" || sub == BlacksmithSub) { 
                    // leaving lich or blacksmith, so change back to main
                    s.music.FadeVolume("Through");
                }
                else { s.music.FadeVolume(); }
            }
            string toSpawn;
            for (int i = 0; i < 15; i++) {
                yield return s.delays[0.033f];
                tempColor.a += 1f/15f;
                boxSR.color = tempColor;
            }
            // fade in the box
            loadingCircle.transform.position = onScreen;
            // make the loading thing go on screen
            if (Save.game.pendingAmuletVisualRestore) {
                s.tombstoneData.RestorePlayerVisualStateAfterPreparedResurrection();
            }
            if (Save.game.pendingAmuletInventoryCleanup) {
                s.tombstoneData.FinalizePreparedResurrectionInventoryCleanup();
            }
            if (!hasQueuedWarpDestination || !queuedWarpUseSavedTrader) {
                Save.game.floorItemNames = new string[9];
                Save.game.floorItemTypes = new string[9];
                Save.game.floorItemMods = new string[9];
                Save.game.floorItemAccs = new int[9];
                Save.game.floorItemSpds = new int[9];
                Save.game.floorItemDmgs = new int[9];
                Save.game.floorItemDefs = new int[9];
                Save.game.floorAcc = 0;
                Save.game.floorSpd = 0;
                Save.game.floorDmg = 0;
                Save.game.floorDef = 0;
            }
            Save.game.blacksmithHasForged = false;
            // clear merchant and floor items when going to the next level
            if (!isLich) {
                if (hasQueuedWarpDestination) {
                    level = queuedWarpLevel;
                    sub = queuedWarpSub;
                    Save.game.enemyNum = queuedWarpEnemyNum;
                }
                else {
                    // if spawning a normal enemy
                    sub++;
                    // increment the sub counter
                    if (sub > BlacksmithSub) { sub = 1; level++; }
                    // increment level and reset sub if we passed sub 4
                    if (level > Save.persistent.highestLevel || level == Save.persistent.highestLevel && sub > Save.persistent.highestSub) {
                        Save.persistent.highestLevel = level;
                        Save.persistent.highestSub = sub;
                    }
                    if (s.enemy.enemyName.text == "Tombstone") {
                        Save.persistent.tsLevel = -1;
                        Save.persistent.tsSub = -1;
                        Save.persistent.tsWeaponAcc = -1;
                        Save.persistent.tsWeaponSpd = -1;
                        Save.persistent.tsWeaponDmg = -1;
                        Save.persistent.tsWeaponDef = -1;
                        Save.persistent.tsItemNames = new string[9];
                        Save.persistent.tsItemNames = new string[9];
                        Save.persistent.tsItemNames = new string[9];
                        sub--;
                        // make tombstone inaccessible
                    }
                }
                // going on to the next level (as opposed to next sub, so make sure to set the variables up correctly)
                if (!hasQueuedWarpDestination && sub == Save.persistent.tsSub && level == Save.persistent.tsLevel && !(sub == 1 && level == 1)) {
                    // spawn tombstone if we are on the correct level and not on 1-1
                    toSpawn = "tombstone";
                    // level matches which level to add to
                    levelTransText.text = $"level {level}-{sub}*";
                    levelText.text = $"(level {level}-{sub}*)";
                    // decrement sub (because we went up 1 level but aren't going to fight anything)
                    s.enemy.SpawnNewEnemy(8, true);
                    // spawn the tombstone
                }
                else if (queuedWarpEnemyNum == Enemy.BlacksmithEnemyNum || !hasQueuedWarpDestination && ShouldForceBlacksmithSpawn()) {
                    toSpawn = "blacksmith";
                    s.enemy.SpawnNewEnemy(Enemy.BlacksmithEnemyNum, true);
                    s.turnManager.RefreshBlackBoxVisibility();
                    levelTransText.text = GetTransferDestinationText(level, sub, Enemy.BlacksmithEnemyNum);
                    levelText.text = $"({GetTransferDestinationText(level, sub, Enemy.BlacksmithEnemyNum)})";
                }
                else if (sub == MerchantSub || hasQueuedWarpDestination && queuedWarpEnemyNum == Enemy.MerchantEnemyNum) { 
                    toSpawn = "merchant";
                    s.enemy.SpawnNewEnemy(Enemy.MerchantEnemyNum, true);
                    // create the trader enemy
                    s.turnManager.RefreshBlackBoxVisibility();
                    // summon trader if necessary
                    levelTransText.text = GetLevelLabel(level, sub);
                    levelText.text = $"({GetLevelLabel(level, sub)})";
                    // set the correct level loading text
                }
                else if (level == 4 && sub == 1) { 
                    toSpawn = "devil";
                    // spawn the devil if on the correct level
                    transGlitchCoro = StartCoroutine(GlitchyLevelText());
                    s.enemy.SpawnNewEnemy(0, true); 
                    debugGlitchCoro = StartCoroutine(GlitchyDebugText());
                }
                else { 
                    toSpawn = "normal";
                    s.enemy.SpawnNewEnemy(UnityEngine.Random.Range(3, 7), true); 
                    levelTransText.text = $"level {level}-{sub}";
                    levelText.text = $"(level {level}-{sub})";
                }
                // normal level, so notify the player accordingly and spawn basic enemy
            }
            else { 
                toSpawn = "lich";
                levelTransText.text = "level ???";
                levelText.text = "(level ???)";
                // going to the lich level, so notify player
                s.enemy.SpawnNewEnemy(2, true);
            }
            // always spawn new enemy if going to next level
            s.itemManager.DestroyItems();
            // remove all items from the floor
            // s.player.GetComponent<Animation>().Rewind();
            s.enemy.GetComponent<Animator>().Rebind();
            s.enemy.GetComponent<Animator>().Update(0f);
            s.player.GetComponent<Animator>().Rebind();
            s.player.GetComponent<Animator>().Update(0f);
            s.terrain.Rebind();
            s.terrain.Update(0f);
            yield return s.delays[1.5f];
            if (transGlitchCoro != null) { StopCoroutine(transGlitchCoro); }
            // wait 1.5s
            s.statSummoner.SummonStats();
            s.statSummoner.SetDebugInformationFor("enemy");
            // summon the stats and update the debug information
            levelTransText.text = "";
            loadingCircle.transform.position = offScreen;
            levelBox.transform.position = offScreen;
            // clear the loading text and move the box offscreen
            Save.game.numItemsDroppedForTrade = 0;
            // clear the number of items player has dropped
            if (s.tutorial == null) { Save.SaveGame(); }
            if (toSpawn == "tombstone") { 
                // going to tombstone, spawn spawn items
                s.itemManager.lootText.text = "loot:";
                s.tombstoneData.SpawnSavedTSItems();
            }
            else if (toSpawn == "merchant") {
                // going to trader, so spawn trader items
                if (hasQueuedWarpDestination && queuedWarpUseSavedTrader) { s.tombstoneData.SpawnSavedMerchantItems(); }
                else { s.itemManager.SpawnMerchantItems(); }
            }
            else if (toSpawn == "blacksmith") {
                if (hasQueuedWarpDestination && queuedWarpUseSavedTrader) { s.tombstoneData.SpawnSavedMerchantItems(); }
                else { s.itemManager.SpawnBlacksmithItems(); }
            }
            else {
                // not going to the trader or tombstone
                s.diceSummoner.SummonDice(false, true);
                s.turnManager.RefreshBlackBoxVisibility();
                // summon die and make sure the enemy's stats can be seen
            }
            if (toSpawn is "normal" or "devil" or "lich") { s.itemManager.BeginNewEncounterWeaponState(rerollLuckyDice:true); }
            else { s.itemManager.EndEncounterWeaponState(); }
            // can spawn the items here because we have a deletion queue rather than just deleting all
            s.player.targetIndex = 0;
            s.turnManager.SetTargetOf("player");
            // make it so that the player auto targets chest, rather than their previous attack
            for (int i = 0; i < 15; i++) {
                yield return s.delays[0.033f];
                tempColor.a -= 1f/15f;
                boxSR.color = tempColor;
            }
            if (Save.game.showAmuletSurvivalStatusText) {
                Save.game.showAmuletSurvivalStatusText = false;
                s.turnManager.SetStatusText("you live another day", true);
            }
            else if (toSpawn == "tombstone") { s.turnManager.SetStatusText("you come across a humble tombstone", true); }
            else if (toSpawn == "lich") { s.turnManager.SetStatusText("impervious, he seems to be immune to wound effects", true); }
            else if (toSpawn == "devil") { s.turnManager.SetStatusText("dice of slain heroes rattle around his neck", true); }
            else if (toSpawn == "blacksmith") { s.turnManager.SetStatusText("forge one stat", true); }
            // fade the level box back out
            s.itemManager.AttemptFadeTorches(shouldAdvanceTorchCounter);
            StartCoroutine(ApplyPendingArrivalBonuses());
            // try to remove torches
            // spawn the items so the player can interact with them, after the items are destroyed
            s.turnManager.DetermineMove(false, true);
            // determine who moves
            lockActions = false;
            Save.game.resumeSub = s.levelManager.sub;
            Save.game.resumeLevel = s.levelManager.level;
            hasQueuedWarpDestination = false;
            queuedWarpUseSavedTrader = false;
        }
        Save.SavePersistent();
        if (s.tutorial == null) { Save.SaveGame(); }
    }

    private char R() { 
        // return a random character
        return characters[UnityEngine.Random.Range(0, characters.Length)];
    }
    private char T() { 
        // return a random thin character
        return thinCharacters[UnityEngine.Random.Range(0, thinCharacters.Length)];
    }

    private IEnumerator IndicateUnlock() {
        // fade in the black box, let the player know that they unlocked a new character, then go to credits
        Color tempColor = boxSR.color;
        tempColor.a = 0f;
        boxSR.color = tempColor;
        for (int i = 0; i < 15; i++) {
            yield return s.delays[0.033f];
            tempColor.a += 1f/15f;
            boxSR.color = tempColor;
        }
        loadingCircle.transform.position = onScreen;
        levelTransText.text = "New character unlocked!";
        yield return s.delays[2f];
        levelTransText.text = "";
        loadingCircle.transform.position = offScreen;
        Initiate.Fade("Credits", Color.black, 4f);
    }

    /// <summary>
    /// Create the glitchy effect that the text has when going to 4-1 (Devil).
    /// </summary>
    private IEnumerator GlitchyLevelText() {
        for (int i = 0; i < 12; i++) {
            levelTransText.text = $"level {R()}-{R()}";
            yield return s.delays[0.033f];
            // 12 times, 'level z-w'
        }
        for (int i = 0; i < 12; i++) {
            levelTransText.text = $"lev{R()} {R()}{R()}{R()}";
            yield return s.delays[0.033f];
            // 12 times, 'levnU p0y'
        }
        for (int i = 0; i < 12; i++) {
            levelTransText.text = $"{R()}{R()}{R()}{R()} {R()}{R()}{R()}";
            yield return s.delays[0.05f];
            // 12 times, 'OQI"k Hl9'
        }
    }

    /// <summary>
    /// Make the level indicator for 4-1 (Devil) to be glitchy, but not as much.
    /// </summary>
    private IEnumerator GlitchyDebugText() {
        while (true){ 
            levelText.text = $"level {T()}-{T()}";
            yield return s.delays[0.05f];
            // forever until interrupted, 'level i-!'
        }
    }
}
