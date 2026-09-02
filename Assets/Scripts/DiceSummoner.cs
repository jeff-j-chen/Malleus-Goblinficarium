using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
public class DiceSummoner : MonoBehaviour
{
    [SerializeField] private GameObject diceBase; // colored background sprite for each die
    [SerializeField] public GameObject[] numArr; // face prefabs indexed 0-5 (faces 1-6)
    public bool breakOutOfScimitarParryLoop = false; // set to true to escape an infinite scimitar parry loop
    private Scripts s;
    public List<GameObject> existingDice = new(); // all die GameObjects currently alive on the board
    public float yCoord = -5.51f; // y position of the unattached draft row
    private readonly float[] desktopXCoords = { -2.75f, -1.65f, -0.55f, 0.55f, 1.65f, 2.75f };
    private readonly float[] mobileXCoords = { -2.75f*1.49f, -1.65f*1.49f, -0.55f*1.49f, 0.55f*1.49f, 1.65f*1.49f, 2.75f*1.49f };
    private float[] xCoords; // set at Start to whichever of the above matches the platform
    private readonly List<Color> generatedTypes = new(); // scratch pool built by GenerateDiceTypes(), consumed during the initial six spawn
    // lastNum/lastType/lastStat: face value, color, and stat of the die carried over by scroll of courage; lastNum == -1 means no saved die
    public int lastNum;
    public string lastType;
    public string lastStat;
    public readonly Vector3 desktopDiceScale = new(1f, 1f, 1f);
    public readonly Vector3 mobileDiceScale = new(1.499f, 1.499f, 1f); // 1.50f causes strange visual bugs on the dice, so 1.499f it is
    private Vector3 diceScale; // resolved at Start based on mobileMode
    // draft-spawn bookkeeping: allows SaveDiceValues to wait until all bonus dice have fully spawned
    private int pendingDraftSpawnEffects; // count of TrackDraftSpawnEffect coroutines still running
    private bool draftSpawnEffectsInProgress; // true while any tracked spawn coroutine is active
    private bool pendingDraftSaveAfterSpawn; // if true, triggers SaveDiceValues once all tracked spawns finish
    
    
    private void Start() {
        s = FindFirstObjectByType<Scripts>();
        if (s.mobileMode) {
            xCoords = mobileXCoords;
            diceScale = mobileDiceScale;
        }
        else {
            xCoords = desktopXCoords;
            diceScale = desktopDiceScale;
        }
    }

    // private void Update() {
    //     if (Input.GetKeyDown(KeyCode.Space)) {
    //         // testing purposes only, use to refresh this given set of dice
    //         s.statSummoner.ResetDiceAndStamina();
    //         SummonDice(false, true);
    //         s.statSummoner.SummonStats();
    //     }
    // }

    /// <summary>
    /// Entry point for dice summoning; delegates to SummonAfterFade().
    /// </summary>
    /// <param name="initialSummon">true on first combat load, skips the between-round delay</param>
    /// <param name="newSet">true generates a fresh draft; false rebuilds the board from save data</param>
    public void SummonDice(bool initialSummon, bool newSet) {
        StartCoroutine(SummonAfterFade(initialSummon, newSet));
    }

    /// <summary>
    /// Returns true when all bonus-die spawn coroutines have finished.
    /// Polled by TurnManager to gate player input during the draft phase.
    /// </summary>
    /// <returns>true if no tracked spawn coroutines are still running</returns>
    public bool IsDraftSpawnComplete() {
        return !draftSpawnEffectsInProgress && pendingDraftSpawnEffects <= 0;
    }

    /// <summary>
    /// Wraps a bonus-die spawn coroutine so its running state is tracked globally.
    /// Use this instead of StartCoroutine for any spawn that must complete before the
    /// dice save or player input is allowed.
    /// </summary>
    /// <param name="routine">coroutine to run (e.g. SpawnFlailDice, SpawnDevilDice)</param>
    private void TrackDraftSpawnEffect(IEnumerator routine) {
        if (routine == null) { return; }
        pendingDraftSpawnEffects++;
        draftSpawnEffectsInProgress = true;
        StartCoroutine(TrackDraftSpawnEffectCoro(routine));
    }

    private IEnumerator TrackDraftSpawnEffectCoro(IEnumerator routine) {
        try {
            yield return StartCoroutine(routine);
        }
        finally {
            // decrement; when it hits 0 all bonus-die spawns are done
            pendingDraftSpawnEffects = Mathf.Max(0, pendingDraftSpawnEffects - 1);
            if (pendingDraftSpawnEffects == 0) {
                draftSpawnEffectsInProgress = false;
                if (pendingDraftSaveAfterSpawn) {
                    // a save was requested while spawns were still in flight; do it now
                    pendingDraftSaveAfterSpawn = false;
                    SaveDiceValues(0.05f);
                }
            }
        }
    }

    /// <summary>
    /// Do not call this coroutine directly; use SummonDice() instead.
    /// Full dice-spawning logic:
    ///   newSet=true  - generates a fresh six-die draft, then fires any bonus-die spawns
    ///                  (flail, cursed dice, char bonuses, devil, courage) via TrackDraftSpawnEffect
    ///   newSet=false - rebuilds the board from Save.game (e.g. after a scene reload)
    /// </summary>
    /// <param name="initialSummon">suppresses the 0.25s pre-spawn delay used between rounds</param>
    /// <param name="newSet">true generates a fresh draft; false restores dice from save data</param>
    private IEnumerator SummonAfterFade(bool initialSummon, bool newSet) {
        if (newSet) {
            pendingDraftSaveAfterSpawn = false;
            pendingDraftSpawnEffects = 0;
            draftSpawnEffectsInProgress = false;
            if (s.turnManager.dieSavedFromLastRound != null) { 
                Dice fromLastRound = s.turnManager.dieSavedFromLastRound.GetComponent<Dice>();
                lastNum = fromLastRound.diceNum;
                lastType = fromLastRound.diceType;
                lastStat = fromLastRound.statAddedTo;
                // need to store them in primitives because the dice and its info will be destroyed
            }
            else { lastNum = -1; } // -1 is a sentinel: no die was saved from last round
            // need to Save the die before the delay and summon it afterwards for some reason
            if (!initialSummon) {
                // delay if necessary
                yield return s.delays[0.25f];
            }
            existingDice.Clear();
            // clear the list so we have a fresh array
            if (s.tutorial != null && s.tutorial.ConsumeQueuedParryDraft()) {
                for (int i = 0; i < 6; i++) {
                    yield return s.delays[0.025f];
                    GenerateSingleDie(i + 1, "white", "none", null, i, initialSix:true);
                }
            }
            else {
                GenerateDiceTypes();
                for (int i = 0; i < 6; i++) {
                    yield return s.delays[0.025f];
                    GenerateSingleDie(Random.Range(1, 7), null, "none", null, i, initialSix:true);
                    // generate the 6 base die for every round
                }
            }
            if (s.itemManager.PlayerHasWeapon("flail")) {
                TrackDraftSpawnEffect(SpawnFlailDice());
            }
            if (s.itemManager.ShouldSpawnCursedDiceAtDraftStart()) {
                TrackDraftSpawnEffect(SpawnCursedDice());
            }
            if (Save.game.curCharNum == 1) {
                TrackDraftSpawnEffect(SpawnCharOneDice());
            }
            if (s.itemManager.PlayerHasWeapon("hatchet") && s.itemManager.PlayerHasLegendary()) {
                TrackDraftSpawnEffect(SpawnHatchetDice());
            }
            if (s.enemy != null && s.enemy.enemyName.text == "Devil") {
                TrackDraftSpawnEffect(SpawnDevilDice());
            }
            if (lastNum != -1) {
                TrackDraftSpawnEffect(SpawnCourageDice());
            }

            // if all bonus spawns finished synchronously, save now; otherwise defer until they complete
            if (IsDraftSpawnComplete()) {
                SaveDiceValues(0.35f);
            }
            else {
                pendingDraftSaveAfterSpawn = true;
            }
        }
        else { 
            pendingDraftSaveAfterSpawn = false;
            pendingDraftSpawnEffects = 0;
            draftSpawnEffectsInProgress = false;
            existingDice.Clear();
            int initialSpawnCount = 0;
            s.turnManager.BeginEnemyPlanRefreshBatch();
            try {
                for (int i = 0; i < Save.game.diceTypes.Count; i++) {
                    yield return s.delays[0.05f];
                    // for every die
                    bool tarotUpgradeAlreadyApplied = Save.game.diceTarotUpgraded != null
                        && i < Save.game.diceTarotUpgraded.Count
                        && Save.game.diceTarotUpgraded[i];
                    bool cursedDiceSpawned = Save.game.diceCursedSpawned != null
                        && i < Save.game.diceCursedSpawned.Count
                        && Save.game.diceCursedSpawned[i];
                    if (Save.game.dicePlayerOrEnemy[i] == "none") {
                        // if its not attached, its part of the 6 pickup-able
                        Dice createdDie = GenerateSingleDie(
                            Save.game.diceNumbers[i],
                            Save.game.diceTypes[i],
                            "none",
                            Save.game.diceAttachedToStat[i],
                            initialSpawnCount,
                            initialSix:true
                        );
                        createdDie.tarotUpgradeApplied = tarotUpgradeAlreadyApplied;
                        createdDie.spawnedByCursedDice = cursedDiceSpawned;
                        // create the die
                        initialSpawnCount++;
                        // increment the counter (used in generation to calculate offset)
                    }
                    else {
                        // else its a die attached by some other means (e.g. flail, devil)
                        Dice createdDie = GenerateSingleDie(
                            Save.game.diceNumbers[i],
                            Save.game.diceTypes[i],
                            Save.game.dicePlayerOrEnemy[i],
                            Save.game.diceAttachedToStat[i],
                            initialSpawnCount,
                            initialSix:true
                        );
                        createdDie.tarotUpgradeApplied = tarotUpgradeAlreadyApplied;
                        createdDie.spawnedByCursedDice = cursedDiceSpawned;
                        if (createdDie.isAttached && createdDie.isOnPlayerOrEnemy == "player" && !createdDie.tarotUpgradeApplied) {
                            s.itemManager.TryUpgradeTakenDieWithTarot(createdDie, 0.05f);
                        }
                        // so create it and attach directly
                    }
                }
            }
            finally {
                s.turnManager.EndEnemyPlanRefreshBatch();
            }
            SaveDiceValues(0.35f);
        } 
    }

    /// <summary>
    /// Grants 1 red die to the player (or 2 when wielding a legendary flail).
    /// The die is pre-attached to the player's red stat.
    /// </summary>
    private IEnumerator SpawnFlailDice() {
        yield return s.delays[0.2f];
        if (s.itemManager.PlayerHasLegendary()) {
            // give the player two red die if wielding a legendary flail, else one
            GenerateSingleDie(Random.Range(1, 7), "red", "player", "red", initialSix:true, isFromMight:true);
            yield return s.delays[0.1f];
            GenerateSingleDie(Random.Range(1, 7), "red", "player", "red", initialSix:true, isFromMight:true);
        }
        else {
            GenerateSingleDie(Random.Range(1, 7), "red", "player", "red", initialSix:true, isFromMight:true);
        }
    }

    /// <summary>
    /// Grants a yellow die to the player at draft start for character slot 1 (the second character).
    /// The die is pre-attached to the player's red stat row.
    /// </summary>
    private IEnumerator SpawnCharOneDice() {
        yield return s.delays[0.2f];
        // if player character #2 (maul armor helm), give player yellow die
        GenerateSingleDie(Random.Range(1, 7), "yellow", "player", "red", initialSix:true, isFromMight:true);
    }

    /// <summary>
    /// Grants a yellow die to the player at draft start when wielding a legendary hatchet.
    /// The die is pre-attached to the player's red stat row.
    /// </summary>
    private IEnumerator SpawnHatchetDice() {
        yield return s.delays[0.2f];
        // legendary hatchet lets player start out with yellow die
        GenerateSingleDie(Random.Range(1, 7), "yellow", "player", "red", initialSix:true, isFromMight:true);
    }

    /// <summary>
    /// Spawns cursed yellow dice (from the cursed dice item) and attaches them to the player.
    /// The count is determined by GetCursedDiceSpawnCount(). Each created die is flagged
    /// spawnedByCursedDice so other systems can identify and handle it specially.
    /// </summary>
    private IEnumerator SpawnCursedDice() {
        yield return s.delays[0.2f];

        int spawnCount = s.itemManager.GetCursedDiceSpawnCount();
        for (int i = 0; i < spawnCount; i++) {
            Dice created = GenerateSingleDie(Random.Range(1, 7), "yellow", "player", "red", initialSix:true, isFromMight:true);
            created.spawnedByCursedDice = true;
            if (i < spawnCount - 1) {
                yield return s.delays[0.1f];
            }
        }
    }

    /// <summary>
    /// Populates the Devil enemy's dice at draft start: one die per stat type (from statArr),
    /// then additional yellow dice scaled by difficulty and level. Wound-affected dice are
    /// faded or reduced immediately after spawning.
    /// </summary>
    private IEnumerator SpawnDevilDice() {
        yield return s.delays[0.2f];
        foreach (string typeToGen in s.itemManager.statArr) {
            // generate a die for every stat
            yield return s.delays[0.05f];
            Dice created = GenerateSingleDie(Random.Range(1,7), typeToGen, "enemy", typeToGen, initialSix:true);
            // attach it to the devil
            if (typeToGen == "red" && (s.enemy.woundList.Contains("armpits") || s.itemManager.EnemyHasTemporaryArmpitsInjury())
                || typeToGen == "white" && (s.enemy.woundList.Contains("hand") || s.itemManager.EnemyHasTemporaryHandInjury())) {
                StartCoroutine(created.FadeOut(true));
            }
            else if ((s.enemy.woundList.Contains("guts") || s.itemManager.EnemyHasTemporaryGutsInjury()) && s.enemy.enemyName.text != "Lich") {
                StartCoroutine(created.DecreaseDiceValue(false));
            }
            // devil doesn't get to take its starting red and white if its wounded there
        }

        int yellowDiceCount = (DifficultyHelper.IsHard(Save.persistent.gameDifficulty) || DifficultyHelper.IsNightmare(Save.persistent.gameDifficulty)) ? 1 : 0;
        yellowDiceCount += Mathf.Max(0, s.levelManager.level - 4);
        for (int i = 0; i < yellowDiceCount; i++) {
            yield return s.delays[0.1f];
            if (s.levelManager.level == 4) { 
                GenerateSingleDie(Random.Range(1, 4), "yellow", "enemy", "red", initialSix:true);
            }
            else { 
                GenerateSingleDie(Random.Range(1, 6), "yellow", "enemy", "red", initialSix:true);
            }
        }
    }

    /// <summary>
    /// Restores the die saved from the previous round by scroll of courage.
    /// lastNum/lastType/lastStat are captured into primitives before the old dice are
    /// destroyed, so they survive safely into this coroutine.
    /// </summary>
    private IEnumerator SpawnCourageDice() {
        yield return s.delays[0.2f];
        // re-create the saved die and pre-attach it to the player
        GenerateSingleDie(lastNum, lastType, "player", lastStat, initialSix:true, isFromMight:true);
    }

    /// <summary>
    /// Applies the player's active wound penalties to a die that was just granted and
    /// attached to the player (i.e. not picked from the draft row by the player).
    /// Wound effects: chest rerolls high values (>=4), armpits/hand wounds discard
    /// red/white dice, guts decreases die value, char #3 forces white dice to 1.
    /// Also attempts a tarot upgrade after all wound effects are applied.
    /// </summary>
    /// <param name="dice">the newly-attached player die to evaluate</param>
    private void ApplyGrantedPlayerDieWoundEffects(Dice dice) {
        if (dice == null || s == null || s.player == null || !dice.isAttached || dice.isOnPlayerOrEnemy != "player") {
            return;
        }

        bool chestReroll = s.player.woundList.Contains("chest") && dice.diceNum >= 4;
        if (chestReroll) {
            StartCoroutine(dice.RerollAnimation());
        }

        bool shouldDiscardForWound = (dice.diceType == "red" && s.player.woundList.Contains("armpits"))
            || (dice.diceType == "white" && s.player.woundList.Contains("hand"));
        if (shouldDiscardForWound) {
            StartCoroutine(dice.FadeOut());
            return;
        }

        if (s.player.woundList.Contains("guts")) {
            StartCoroutine(dice.DecreaseDiceValue(false));
        }

        if (dice.diceType == "white" && Save.game.curCharNum == 2) {
            dice.SetToOne();
        }

        s.itemManager.TryUpgradeTakenDieWithTarot(dice, chestReroll ? 1.5f : 0.05f);
    }

    /// <summary>
    /// Creates a copy of a die and immediately attaches it to the player.
    /// Called from Dice.cs when a pending mirror-copy effect (pendingMirrorCopy) is resolved.
    /// </summary>
    /// <param name="diceNum">face value of the die to duplicate</param>
    /// <param name="diceType">color of the die; yellow dice attach to the red stat row</param>
    /// <returns>the newly created and attached Dice component</returns>
    public Dice DuplicateDieToPlayer(int diceNum, string diceType) {
        string statToAttachTo = diceType == "yellow" ? "red" : diceType;
        Dice createdDie = GenerateSingleDie(diceNum, diceType, "player", statToAttachTo, initialSix:true, isFromMight:true);
        return createdDie;
    }
    

    /// <summary>
    /// Instantiates and configures one die GameObject, adds it to existingDice, and
    /// optionally attaches it to a player or enemy stat row.
    /// </summary>
    /// <param name="diceNum">face value 1-6</param>
    /// <param name="diceType">color name ("red", "white", "yellow", etc.); null draws from the
    /// pre-generated pool (generatedTypes[i]), used for the initial six</param>
    /// <param name="attachToPlayerOrEnemy">"none" places it in the draft row; "player"/"enemy"
    /// attaches it directly to the specified stat</param>
    /// <param name="statToAttachTo">color name of the stat row to attach to; ignored when "none"</param>
    /// <param name="i">slot index for positioning unattached dice in the draft row</param>
    /// <param name="initialSix">when true, skips the per-die SaveDiceValues call to avoid
    /// redundant saves during batch spawn</param>
    /// <param name="isFromMight">when true, calls ApplyGrantedPlayerDieWoundEffects after attaching</param>
    /// <returns>the Dice component on the newly created die GameObject</returns>
    public Dice GenerateSingleDie(int diceNum, string diceType=null, string attachToPlayerOrEnemy="none", string statToAttachTo=null, int i=0, bool initialSix=false,bool isFromMight=false) {
        Vector2 instantiationPos = attachToPlayerOrEnemy switch {
            // reference variable for the die's attribute
            "none" => new Vector2(xCoords[i], yCoord),
            // add to the bottom row with correct offset if not attaching
            "player" => new Vector2(s.statSummoner.OutermostPlayerX(statToAttachTo), s.statSummoner.yCoords[Array.IndexOf(Colors.colorNameArr, statToAttachTo)] - 0.01f),
            "enemy" => new Vector2(s.statSummoner.OutermostEnemyX(statToAttachTo) - s.statSummoner.diceOffset, s.statSummoner.yCoords[Array.IndexOf(Colors.colorNameArr, statToAttachTo)] - 0.01f),
            _ => new Vector2(0, 0)
        };
        // if diceType is null, draw color from the pre-generated pool by slot index; otherwise look it up by name
        int diceColorIndex = diceType == null ? Array.IndexOf(Colors.colorArr, generatedTypes[i]) : Array.IndexOf(Colors.colorNameArr, diceType);
        // else create one of the specified type
        GameObject number = Instantiate(numArr[diceNum - 1], instantiationPos, Quaternion.identity);
        GameObject indivBase = Instantiate(diceBase, instantiationPos, Quaternion.identity);
        // create gameobjects
        indivBase.transform.parent = number.transform;
        number.transform.parent = transform;
        // parent the base to the number and the number to this (the manager)
        number.transform.localScale = diceScale;
        // scale the number based on whether we are playing in mobile mode or not (base is auto scaled with parent)
        number.GetComponent<Dice>().diceNum = diceNum;
        number.GetComponent<Dice>().diceType = Colors.colorNameArr[diceColorIndex];
        number.GetComponent<Dice>().instantiationPos = instantiationPos;
        // set the necessary attributes
        if (attachToPlayerOrEnemy == "player")  {
            // if attaching to player
            s.statSummoner.AddDiceToPlayer(statToAttachTo, number.GetComponent<Dice>());
            // add it to the array
            number.GetComponent<Dice>().statAddedTo = statToAttachTo;
            if (diceType != "yellow") { number.GetComponent<Dice>().moveable = false; }
            number.GetComponent<Dice>().isAttached = true;
            number.GetComponent<Dice>().isOnPlayerOrEnemy = "player";
            // set necessary attributes
        }
        else if (attachToPlayerOrEnemy == "enemy")  {
            // if attaching to enemy
            s.statSummoner.AddDiceToEnemy(statToAttachTo, number.GetComponent<Dice>());
            // add it to the array
            number.GetComponent<Dice>().moveable = false;
            number.GetComponent<Dice>().statAddedTo = statToAttachTo;
            number.GetComponent<Dice>().isOnPlayerOrEnemy = "enemy";
            number.GetComponent<Dice>().isAttached = true;
            // set necessary attributes
        }
        number.GetComponent<SpriteRenderer>().sortingOrder = 1;
        //set the correct sorting order (so no weird visual glitches)
        if (Colors.colorArr[diceColorIndex] == Colors.white || Colors.colorArr[diceColorIndex] == Colors.yellow) {
            // give the number the correct color relative to the base (e.g. black with yellow or white with red)
            number.GetComponent<SpriteRenderer>().color = Color.black;
        }
        indivBase.GetComponent<SpriteRenderer>().color = Colors.colorArr[diceColorIndex];
        // set the color of the base
        // fade in the die
        existingDice.Add(number);
        // isFromMight means this die was granted by an ability/item (not player-picked),
        // so wound penalties must be applied to it immediately
        if (attachToPlayerOrEnemy == "player" && isFromMight)  {
            ApplyGrantedPlayerDieWoundEffects(number.GetComponent<Dice>());
        }
        // add it to the array of existing dice so that functions can be performed on all die at once
        if (attachToPlayerOrEnemy == "player") { s.statSummoner.SetCombatDebugInformationFor("player"); }
        else if (attachToPlayerOrEnemy == "enemy") { s.statSummoner.SetCombatDebugInformationFor("enemy"); }
        // set the necessary debug information
        if (!initialSix) { SaveDiceValues(); } // skip during batch spawn; caller handles the single save
        return number.GetComponent<Dice>();
    }

    /// <summary>
    /// Serializes all current dice state into Save.game and writes to disk.
    /// </summary>
    /// <param name="waitTime">seconds to wait before serializing; this delay is REQUIRED —
    /// state is not fully settled until the frame has finished processing</param>
    /// 
    public void SaveDiceValues(float waitTime=0.1f) { 
        StartCoroutine(SaveDiceValuesCoro(waitTime));
    }

    /// <summary>
    /// Do not call this coroutine directly; use SaveDiceValues() instead.
    /// </summary>
    /// <param name="waitTime">seconds to delay before writing; mirrors the SaveDiceValues param</param>
    private IEnumerator SaveDiceValuesCoro(float waitTime) { 
        yield return s.delays[waitTime];
        // KEEP THIS DELAY HERE, WITHOUT IT THE DICE WILL NOT SAVE PROPERLY!!!
        Save.game.diceNumbers.Clear();
        Save.game.diceTypes.Clear();
        Save.game.diceAttachedToStat.Clear();
        Save.game.dicePlayerOrEnemy.Clear();
        Save.game.diceRerolled.Clear();
        Save.game.diceTarotUpgraded.Clear();
        Save.game.diceCursedSpawned.Clear();
        // make sure to clear everything before saving new data
        foreach (GameObject g in existingDice) {
            // for every existing dice
            Dice dice = g.GetComponent<Dice>();
            Save.game.diceNumbers.Add(dice.diceNum);
            Save.game.diceTypes.Add(dice.diceType);
            Save.game.diceAttachedToStat.Add(dice.statAddedTo);
            Save.game.dicePlayerOrEnemy.Add(dice.isOnPlayerOrEnemy);
            Save.game.diceRerolled.Add(dice.isRerolled);
            Save.game.diceTarotUpgraded.Add(dice.tarotUpgradeApplied);
            Save.game.diceCursedSpawned.Add(dice.spawnedByCursedDice);
            // add its info to the info 
        }
        if (s.tutorial == null) { Save.SaveGame(); }
        // make sure to Save it
    }

    /// <summary>
    /// Fills generatedTypes with 6 random colors matching the game's distribution.
    /// Starts with 3 yellow + 3 each of red/white/green/blue (15 total), removes 1 green,
    /// then randomly discards 8 more to reach 6. The result is consumed slot-by-slot
    /// by GenerateSingleDie() during the initial six-die spawn.
    /// </summary>
    private void GenerateDiceTypes() {
        generatedTypes.Clear();
        // clear the list so we can start with a new one
        for (int a = 0; a < 3; a++) {
            // create 3 yellow dice
            generatedTypes.Add(Colors.colorArr[4]);
        }
        for (int b = 0; b < 4; b++) {
            // for each dice type that is not yellow
            for (int c = 0; c < 3; c++) {
                // create 3 dice 
                generatedTypes.Add(Colors.colorArr[b]);
            }
        }
        generatedTypes.RemoveAt(4);
        // remove a green
        for (int d = 0; d < 8; d++) {
            // remove dice needed to get just 6
            generatedTypes.RemoveAt(Random.Range(0, generatedTypes.Count));
        }
        // this generates a set of die identical to malleus die generation, as far as i can tell
    }

    /// <summary>
    /// Converts every die currently attached to the player into a yellow die.
    /// Called when the player becomes furious (scroll of fury or kapala offering).
    /// Also marks each converted die as moveable so it can be freely redistributed.
    /// </summary>
    public void MakeAllAttachedYellow() {
        foreach (GameObject dice in existingDice) {
            // for every die that exists
            if (dice.GetComponent<Dice>().isAttached && dice.GetComponent<Dice>().isOnPlayerOrEnemy == "player") {
                // if the die is attached to the player
                dice.GetComponent<Dice>().GetComponent<SpriteRenderer>().color = Color.black;
                dice.GetComponent<Dice>().transform.GetChild(0).GetComponent<SpriteRenderer>().color = Colors.yellow;
                dice.GetComponent<Dice>().diceType = Colors.colorNameArr[4];
                // make the die yellow
                dice.GetComponent<Dice>().moveable = true;
                // allow for moving the die around
            }
        }
        SaveDiceValues();
    }

    /// <summary>
    /// Returns the count of dice still in the draft row (not attached to any stat).
    /// TurnManager checks this is >= 6 before allowing the player to end their turn.
    /// </summary>
    /// <returns>number of unattached dice currently in existingDice</returns>
    public int CountUnattachedDice() {
        return existingDice.Count(curObject => curObject.GetComponent<Dice>().isAttached == false);
    }

    /// <summary>
    /// Returns true if any die on the board is currently playing its roll animation.
    /// TurnManager polls this to block input while dice are visually in motion.
    /// </summary>
    /// <returns>true if at least one die has isRolling set</returns>
    public bool DiceIsRolling() {
        foreach (GameObject dieObject in existingDice) {
            if (dieObject == null) { continue; }
            Dice die = dieObject.GetComponent<Dice>();
            if (die != null && die.isRolling) {
                return true;
            }
        }

        return false;
    }
}