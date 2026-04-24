using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
public class TurnManager : MonoBehaviour {
    public const int PlayerGuardTargetIndex = -1;
    private const int BaseGuardParryBonus = 2;
    private const int BucklerGuardParryBonus = 2;
    [SerializeField] public GameObject blackBox;
    private readonly Vector3 mobileScale = new(1.45f, 0.65f, 1f);
    private readonly Vector2 mobileOnScreen = new(2.29f, 10.29f);
    private readonly Vector3 desktopScale = new(1.45f, 0.55f, 1f);
    private readonly Vector2 desktopOnScreen = new(-0.2f, 10.89f);
    public Vector3 onScreen;
    public Vector3 offScreen;
    public Vector3 scale;
    private Coroutine coroutine = null;
    [SerializeField] public TextMeshProUGUI statusText;
    [SerializeField] public string[] targetArr = { "chest", "guts", "knee", "hip", "head", "hand", "armpits", "neck" };
    [SerializeField] public string[] targetInfoArr = { "reroll any number of enemy's dice", "all enemy's dice suffer a penalty of -1", "your speed is always higher than enemy's", "enemy can't use stamina", "discard one of enemy's die", "enemy can't use white dice", "enemy can't use red dice", "target bleeds out next round" };
    private Scripts s;
    public bool isMoving = false;
    public bool actionsAvailable = false;
    public bool alterationDuringMove = false;
    public int scimitarParryCount = 0;
    public GameObject dieSavedFromLastRound = null;
    public bool discardDieBecauseCourage = false;
    public bool dontRemoveLeechYet = false;
    public bool draftInputLocked = false;
    private bool refreshingEnemyPlan = false;
    private bool enemyPlanRefreshEnabled = false;
    private int enemyPlanRefreshSuspendDepth = 0;
    private bool enemyPlanRefreshPendingWhileSuspended = false;
    private Coroutine queuedEnemyPlanRefresh;
    private Coroutine queuedEnemyDraftMove;
    private bool playerBleedOutPendingThisRound = false;
    private bool enemyBleedOutPendingThisRound = false;
    private bool enemyAttackedFirst = false;   // set at the start of RoundOne
    public bool playerHasAttackedThisRound = false;
    private bool queuePlayerKillAsBleedOut;
    private bool queueEnemyKillAsBleedOut;
    private bool playerAttackStatusSetThisRound;
    private bool enemyAttackStatusSetThisRound;
    private bool pendingEnemyHeadCounterDiscard;
    private bool playerGuardActive;
    private int lastNonGuardPlayerTargetIndex;
    private bool softKillInProgress;
    private bool criticalRoundInProgress;

    public bool IsPlayerGuarding() {
        return s != null
            && s.player != null
            && s.itemManager != null
            && Save.game != null
            && s.itemManager.IsFightableEncounter()
            && !s.player.isDead
            && !Save.game.enemyIsDead
            && s.player.targetIndex == PlayerGuardTargetIndex;
    }

    public bool IsPlayerGuardActive() {
        return playerGuardActive && IsPlayerGuarding();
    }

    public int GetPlayerGuardParryBonus(bool includePendingSelection = false) {
        bool shouldApplyGuardBonus = includePendingSelection ? IsPlayerGuarding() : IsPlayerGuardActive();
        if (!shouldApplyGuardBonus) { return 0; }

        return BaseGuardParryBonus + (s.itemManager.PlayerHasWeapon("buckler") ? BucklerGuardParryBonus : 0);
    }

    public int GetMaxPlayerTargetIndex() {
        if (s == null || s.statSummoner == null) { return 0; }

        int playerAim = s.statSummoner.SumOfStat("green", "player");
        if (playerAim < 0) { return PlayerGuardTargetIndex; }
        return Mathf.Clamp(playerAim, 0, targetArr.Length - 1);
    }

    public int GetPlayerDraftReferenceTargetIndex() {
        if (s?.player == null) { return 0; }
        if (s.player.targetIndex >= 0) {
            return Mathf.Clamp(s.player.targetIndex, 0, targetArr.Length - 1);
        }

        return Mathf.Clamp(lastNonGuardPlayerTargetIndex, 0, targetArr.Length - 1);
    }

    private void Awake() { 
        s = FindFirstObjectByType<Scripts>();
    }

    private void Start() {
        enemyPlanRefreshEnabled = false;
        
        if (s.mobileMode) {
            statusText.transform.localPosition = new Vector3(0f, -199.33f, 0f);
            onScreen = mobileOnScreen;
            blackBox.transform.localScale = mobileScale;
        }
        else { 
            statusText.transform.localPosition = new Vector3(0f, -262.5f, 0f);
            onScreen = desktopOnScreen;
            blackBox.transform.localScale = desktopScale;
        }
        // depending on whether the mobile buttons are toggled on or not, move the status text out of the way
        DisplayWounds();
        SetTargetOf("player");
        SetTargetOf("enemy");
        s.enemy.TargetBest();
        s.statSummoner.ResetDiceAndStamina();
        if (s.tutorial == null) {
            if (!(s.levelManager.level == Save.persistent.tsLevel && s.levelManager.sub == Save.persistent.tsSub)
                && s.enemy.enemyName.text != "Merchant"
                && s.enemy.enemyName.text != "Blacksmith") {
                // spawn in new dice based on the state of the game
                s.diceSummoner.SummonDice(true, Save.game.newGame);
            }
        }

        if (s.itemManager != null) {
            if (s.itemManager.IsFightableEncounter()) {
                s.itemManager.BeginNewEncounterWeaponState();
            }
            else {
                s.itemManager.EndEncounterWeaponState();
            }
        }

        s.statSummoner.SummonStats();
        RefreshBlackBoxVisibility();
        DetermineMove(true);
        enemyPlanRefreshEnabled = true;
    }

    private bool ShouldShowBlackBox(GameObject selectedItem = null) {
        if (s == null || s.enemy == null || Save.game == null) { return false; }

        if (s.itemManager == null) {
            return !Save.game.enemyIsDead && s.enemy.enemyName.text == "Merchant";
        }

        if (s.itemManager.IsFightableEncounter()) { return false; }

        return !s.itemManager.IsShowingForeignWeaponPreview(selectedItem);
    }

    public void RefreshBlackBoxVisibility(GameObject selectedItem = null) {
        if (blackBox == null) { return; }

        blackBox.transform.localPosition = ShouldShowBlackBox(selectedItem) ? onScreen : offScreen;
    }

    private bool EnemyAlreadyHasDraftedDie(bool ignoreSavedDraftState) {
        if (s?.diceSummoner?.existingDice != null) {
            foreach (GameObject diceObject in s.diceSummoner.existingDice) {
                if (diceObject == null) { continue; }

                Dice dice = diceObject.GetComponent<Dice>();
                if (dice != null && dice.isAttached && dice.isOnPlayerOrEnemy == "enemy") {
                    return true;
                }
            }
        }

        if (ignoreSavedDraftState || Save.game?.dicePlayerOrEnemy == null) { return false; }

        return Save.game.dicePlayerOrEnemy.Any(side => side == "enemy");
    }

    public bool EnemyShouldDraftFirst() {
        if (s == null || s.player == null || s.enemy == null || s.statSummoner == null || s.itemManager == null) {
            return false;
        }

        int playerSpeed = s.statSummoner.SumOfStat("blue", "player");
        int enemySpeed = s.statSummoner.SumOfStat("blue", "enemy");
        return !PlayerDraftsFirstThisRound(playerSpeed, enemySpeed);
    }

    public bool EnemyShouldDraftFirstForFreshDraft() {
        if (s == null || s.player == null || s.enemy == null || s.statSummoner == null || s.itemManager == null) {
            return false;
        }

        int playerFreshSpeed = s.statSummoner.SumOfStat("blue", "player");
        int enemyFreshSpeed = s.statSummoner.SumOfStat("blue", "enemy");
        return !PlayerDraftsFirstThisRound(playerFreshSpeed, enemyFreshSpeed);
    }

    /// <summary>
    /// Make the enemy move (if it is their turn).
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="ignoreSavedDraftState">true when starting a fresh dice set that should not reuse saved draft state.</param>
    public void DetermineMove(bool delay = false, bool ignoreSavedDraftState = false, bool? enemyDraftsFirstOverride = null) {
        if (!ignoreSavedDraftState && EnemyAlreadyHasDraftedDie(false)) { return; }

        bool shouldDeferDraftOrderResolution = enemyDraftsFirstOverride == null
            && (s == null
                || s.diceSummoner == null
                || s.diceSummoner.CountUnattachedDice() < 6
                || !s.diceSummoner.IsDraftSpawnComplete()
                || s.diceSummoner.DiceIsRolling());
        if (shouldDeferDraftOrderResolution) {
            draftInputLocked = true;
            if (queuedEnemyDraftMove != null) {
                StopCoroutine(queuedEnemyDraftMove);
                queuedEnemyDraftMove = null;
            }
            queuedEnemyDraftMove = StartCoroutine(DetermineEnemyDraftOrderWhenReady(delay, ignoreSavedDraftState));
            return;
        }

        bool enemyDraftsFirst = enemyDraftsFirstOverride
            ?? (ignoreSavedDraftState ? EnemyShouldDraftFirstForFreshDraft() : EnemyShouldDraftFirst());
        if (enemyDraftsFirst) {
            // if the current draft-order rules put the enemy first
            draftInputLocked = true;
            if (queuedEnemyDraftMove != null) {
                StopCoroutine(queuedEnemyDraftMove);
                queuedEnemyDraftMove = null;
            }
            if (ignoreSavedDraftState) {
                queuedEnemyDraftMove = StartCoroutine(EnemyMoveWhenDraftReady(delay));
            }
            else {
                queuedEnemyDraftMove = StartCoroutine(EnemyMove(delay));
            }
            // make the enemy move
        }
        else {
            draftInputLocked = false;
        }
    }

    private IEnumerator EnemyMoveWhenDraftReady(bool delay) {
        try {
            while (s == null
                || s.diceSummoner == null
                || s.diceSummoner.CountUnattachedDice() < 6
                || !s.diceSummoner.IsDraftSpawnComplete()
                || s.diceSummoner.DiceIsRolling()) {
                yield return null;
            }

            yield return s.delays[0.1f];

            yield return StartCoroutine(EnemyMove(delay));
        }
        finally {
            queuedEnemyDraftMove = null;
        }
    }

    private IEnumerator DetermineEnemyDraftOrderWhenReady(bool delay, bool ignoreSavedDraftState) {
        try {
            while (s == null
                || s.diceSummoner == null
                || s.diceSummoner.CountUnattachedDice() < 6
                || !s.diceSummoner.IsDraftSpawnComplete()
                || s.diceSummoner.DiceIsRolling()) {
                yield return null;
            }

            bool enemyDraftsFirst = ignoreSavedDraftState
                ? EnemyShouldDraftFirstForFreshDraft()
                : EnemyShouldDraftFirst();
            if (!enemyDraftsFirst) {
                draftInputLocked = false;
                yield break;
            }

            yield return s.delays[0.1f];
            yield return StartCoroutine(EnemyMove(delay));
        }
        finally {
            queuedEnemyDraftMove = null;
        }
    }

    /// <summary>
    /// Make the enemy choose die.
    /// </summary>
    /// <param name="delay">true to have a 0.45s delay before choosing, false to instantly choose.</param>
    /// <param name="selectAllRemaining"></param>
    /// <returns></returns>
    public IEnumerator EnemyMove(bool delay, bool selectAllRemaining = false) {
        try {
            if (delay) { yield return s.delays[0.45f]; }
            // delay if necessary
            if (selectAllRemaining) {
                for (int i = 0; i < 3; i++) {
                    // if the player has used a scroll of haste, make the enemy choose all remaining die
                    s.enemy.ChooseBestDie();
                }
            }
            else { s.enemy.ChooseBestDie(); }
            // otherwise, just choose a die
        }
        finally {
            queuedEnemyDraftMove = null;
            draftInputLocked = false;
        }
    }

    /// <summary>
    /// Make sure the player/enemy isn't aiming at a place they can't with their current accuracy. Reassigns the target if they can't.
    /// </summary>
    public void RecalculateMaxFor(string playerOrEnemy) {
        if (playerOrEnemy == "player") {
            int maxPlayerTargetIndex = GetMaxPlayerTargetIndex();
            if (s.player.targetIndex > maxPlayerTargetIndex) {
                s.player.targetIndex = maxPlayerTargetIndex;
            }
            if (s.player.targetIndex < PlayerGuardTargetIndex) {
                s.player.targetIndex = PlayerGuardTargetIndex;
            }
            // check the available targets
            SetTargetOf("player");
            // reset the target
        }
        else if (playerOrEnemy == "enemy") {
            if (s.enemy.targetIndex > s.statSummoner.SumOfStat("green", "enemy")) {
                s.enemy.targetIndex = s.statSummoner.SumOfStat("green", "enemy");
            }
            SetTargetOf("enemy");
            // same as above
        }
        else { Debug.LogError("Invalid string passed in to RecalculateMax() in TurnManager.cs"); }
    }

    /// <summary>
    /// Show the current wounds of the player and enemy.
    /// </summary>
    public void DisplayWounds() {
        // add in something to fade in the wound text here
        if (s.player.woundList.Count > 0) {
            // if player has 1 wound or more
            s.player.woundGUIElement.text = "";
            // clear the text
            foreach (string wound in s.player.woundList) {
                s.player.woundGUIElement.text += ("*" + wound + "\n");
                // set the wound next fo each one
            }
        }
        else { s.player.woundGUIElement.text = "[no wounds]"; }
        // 0 wounds, so display as such
        if (s.enemy.spawnNum == 0) {
            // is the cloaked devil
            s.enemy.woundGUIElement.text = "[cloaked]";
        }
        else if (s.enemy.enemyName.text == "Merchant") {
            s.enemy.woundGUIElement.text = "[no wounds]";
        }
        else if (s.enemy.enemyName.text == "Blacksmith") {
            s.enemy.woundGUIElement.text = "[no wounds]";
        }
        else {
            // any other enemy
            if (s.enemy.woundList.Count > 0) {
                s.enemy.woundGUIElement.text = "";
                foreach (string wound in s.enemy.woundList) {
                    s.enemy.woundGUIElement.text += ("*" + wound + "\n");
                }
            }
            else { s.enemy.woundGUIElement.text = "[no wounds]"; }
            // pretty much the same as the above block
        }
    }

    /// <summary>
    /// Fade and change the color of the player or enemy's wound text.
    /// </summary>
    private IEnumerator InjuredTextChange(TextMeshProUGUI text, string clipAtBlack = null, Action onBlackBeforeSound = null, Action onBlackAfterSound = null) {
        yield return s.delays[0.55f];
        // set a delay
        Color temp = text.color;
        temp.a = 0.5f;
        // set text transparency to 1/2
        for (int i = 0; i < 20; i++) {
            yield return s.delays[0.01f];
            temp.a -= 0.025f;
            text.color = temp;
        }
        // fade out
        onBlackBeforeSound?.Invoke();
        DisplayWounds();
        if (!string.IsNullOrEmpty(clipAtBlack)) { s.soundManager.PlayClip(clipAtBlack); }
        onBlackAfterSound?.Invoke();
        // update the wound display
        for (int i = 0; i < 40; i++) {
            yield return s.delays[0.005f];
            temp.a += 0.025f;
            text.color = temp;
        }
        // fade back in
    }

    public void AnimatePlayerWoundRefresh(string clipAtBlack = null, Action onBlackBeforeSound = null, Action onBlackAfterSound = null) {
        if (s == null || s.player == null) { return; }

        StartCoroutine(InjuredTextChange(s.player.woundGUIElement, clipAtBlack, onBlackBeforeSound, onBlackAfterSound));
    }

    /// <summary>
    /// Set the current target and text based on the target index.
    /// </summary>
    public void SetTargetOf(string playerOrEnemy) {
        if (playerOrEnemy == "player") {
            if (s.levelManager.sub == Save.persistent.tsSub && s.levelManager.level == Save.persistent.tsLevel && !(s.levelManager.sub == 1 && s.levelManager.level == 1)) {
                // tombstone
                if (s.player.isDead) {
                    // dead, i.e. was just killed
                    s.player.target.text = "chest";
                    s.player.targetInfo.text = targetInfoArr[0];
                }
                else {
                    // not dead, just came across the tombstone
                    s.player.target.text = "none";
                    s.player.targetInfo.text = "why would you try to wound a tombstone?";
                }
            }
            else {
                // normal setting
                if (s.player.targetIndex < PlayerGuardTargetIndex) {
                    s.player.targetIndex = PlayerGuardTargetIndex;
                }
                if (s.statSummoner.SumOfStat("green", "player") < 0) { s.player.targetIndex = PlayerGuardTargetIndex; }
                // check in case they swapped to a new weapon and are aiming at an old wound
                if (s.player.targetIndex == PlayerGuardTargetIndex) {
                    s.player.target.text = "guard";
                    s.player.targetInfo.text = "+2 parry\ndo not attack";
                }
                else {
                    lastNonGuardPlayerTargetIndex = Mathf.Clamp(s.player.targetIndex, 0, targetArr.Length - 1);
                    if (!isMoving) { 
                        // prevents a nasty bug where wounds would not be applied properly due to the * being added
                        if (LevelManager.IsDevilSub(s.levelManager.level, s.levelManager.sub)) {
                            // if devil
                            if (s.enemy.woundList.Contains(targetArr[s.player.targetIndex])) { s.player.target.text = "*" + targetArr[s.player.targetIndex]; }
                            // add an asterisks if already injured
                            else {
                                if (targetArr[s.player.targetIndex] == "neck" && !Save.game.enemyIsDead) {
                                    // can't aim at the devil's neck if hes alive, so notify player
                                    s.turnManager.SetStatusText("you cannot aim at his neck");
                                    s.player.targetIndex--;
                                }
                                else {
                                    s.player.target.text = targetArr[s.player.targetIndex];
                                }
                            }
                            if (targetArr[s.player.targetIndex] != "neck") {
                                s.player.targetInfo.text = targetInfoArr[s.player.targetIndex];
                            }
                        }
                        else {
                            // set the player's attack indicator + description based on the target index
                            if (s.enemy.woundList.Contains(targetArr[s.player.targetIndex])) { s.player.target.text = "*" + targetArr[s.player.targetIndex]; }
                            // add an asterisks if already injured
                            else { s.player.target.text = targetArr[s.player.targetIndex]; }

                            s.player.targetInfo.text = s.enemy.enemyName.text == "Lich" 
                                ? "no effect since enemy is immune" 
                                : targetInfoArr[s.player.targetIndex];
                        }
                    }
                    
                }
            }
            if (!isMoving) { RefreshEnemyPlanIfNeeded(); }
        }
        else if (playerOrEnemy == "enemy") {
            if (s.player.isDead) { return; }
            // instantly return if the player is already dead, bcs it doesnt matter anymore
            if (s.enemy.enemyName.text == "Merchant" || s.enemy.enemyName.text == "Blacksmith") {
                // trader
                s.enemy.target.text = "bargain";
            }
            // else if (s.levelManager.sub == s.tombstoneData.sub && s.levelManager.level == s.tombstoneData.level) {
            else if (s.enemy.enemyName.text == "Tombstone") {
                // tombstone
                s.enemy.target.text = "serenity";
            }
            else {
                // normal enemy, so set enemy's target indicator based on the target index
                s.enemy.target.text = targetArr[s.enemy.targetIndex];
            }
        }
        else { Debug.LogError("Invalid string passed in to SetTarget() in TurnManager.cs"); }
    }

    /// <summary>
    /// Change the available stamina of the player or enemy by the specified amount.
    /// </summary>
    public void ChangeStaminaOf(string playerOrEnemy, int amount) {
        if (playerOrEnemy == "player") {
            s.player.stamina += amount;
            // change stamina
            if (s.player.stamina >= 10 && Save.game.curCharNum == 3) {
                // heal wounds at 10 stamina, only applies for foods (during attack is handled elsewhere)
                s.player.woundList.Clear();
                StartCoroutine(HealAfterDelay());
            }
            s.player.staminaCounter.text = s.player.stamina.ToString();
            // update counter
            RecalculateMaxFor(playerOrEnemy);
            // recalculate max (in case stamina was taken from green)
            Save.game.playerStamina = s.player.stamina;
            if (s.tutorial == null) { Save.SaveGame(); }
            if (s.itemManager != null) {
                if (s.itemManager.PlayerHasWeapon("claymore")) {
                    s.itemManager.RefreshPlayerCombatStatsAndDice();
                }
                else {
                    s.itemManager.RefreshHighlightedItemDescription();
                }
            }
            RefreshEnemyPlanIfNeeded();
        }
        else if (playerOrEnemy == "enemy") {
            s.enemy.stamina += amount;
            s.enemy.staminaCounter.text = s.enemy.stamina.ToString();
            RecalculateMaxFor(playerOrEnemy);
            Save.game.enemyStamina = s.enemy.stamina;
            Save.game.enemyTargetIndex = s.enemy.targetIndex;
            if (s.tutorial == null) { Save.SaveGame(); }
            // same as above
        }
        else { Debug.LogError("Invalid string passed in to ChangeStaminaAndUpdate() in TurnManager.cs"); }
    }

    /// <summary>
    /// Start the first round of attack.
    /// </summary>
    public void RoundOne() {
        criticalRoundInProgress = true;
        playerHasAttackedThisRound = false;
        if (s.itemManager.PlayerHas("rabadons_deathcap") && s.player.stamina < 3) {
            Save.game.pendingDeathcapRestore = true;
        }
        scimitarParryCount = 0;
        queuePlayerKillAsBleedOut = false;
        queueEnemyKillAsBleedOut = false;
        playerAttackStatusSetThisRound = false;
        enemyAttackStatusSetThisRound = false;
        pendingEnemyHeadCounterDiscard = false;
        // reset the scimitar parry (if set true from a previous round)
        isMoving = true;
        Save.game.pendingMirrorCopy = false;
        Save.game.pendingSpellbookTransmute = false;
        // set the variable to true so that certain actions can check this and make sure actions are not taken when they shouldn't be
        playerBleedOutPendingThisRound = Save.game.playerBleedsOutNextRound || s.player.woundList.Contains("neck");
        enemyBleedOutPendingThisRound = Save.game.enemyBleedsOutNextRound || (s.enemy.enemyName.text != "Lich" && s.enemy.woundList.Contains("neck"));
        if (DifficultyHelper.IsNightmare(Save.persistent.gameDifficulty)) {
            // nightmare: animate the plan reveal, then execute the round
            StartCoroutine(NightmareRevealThenExecuteRound());
            return;
        }
        RunEnemyCalculations();
        // make enemy add stamina to stats as necessary
        ExecuteRoundOneLogic();
    }

    public bool CanEscapeToMenu() {
        bool isAnyDieRolling = s?.diceSummoner != null && s.diceSummoner.DiceIsRolling();
        return !criticalRoundInProgress && !isAnyDieRolling;
    }

    // easy and hard both keep the committed enemy plan visible once drafting is complete
    private static bool ShouldRevealPlanDuringDraft() {
        return DifficultyHelper.IsEasy(Save.persistent.gameDifficulty)
            || DifficultyHelper.IsHard(Save.persistent.gameDifficulty);
    }

    /// <summary>
    /// Execute the initiative + attack logic shared by all difficulties after the plan is applied.
    /// </summary>
    private void ExecuteRoundOneLogic() {
        Save.persistent.turnsTaken++;
        s.itemManager.ClearRoundAttackWeaponBonuses(refreshCombatUI:false);
        if (IsPlayerGuarding()) {
            ExecuteGuardRoundLogic();
            RecordEquippedWeaponUse();
            Save.SavePersistent();
            return;
        }

        InitializeVariables(out int playerAim, out int enemyAim, out int playerSpd, out int enemySpd, out int playerAtt, out int enemyAtt, out int playerDef, out int enemyDef);
        // get all the stats so we can use them
        bool playerActsFirst = PlayerActsFirstThisRound(playerSpd, enemySpd);
        enemyAttackedFirst = !playerActsFirst;
        if (playerActsFirst) {
            s.itemManager.ActivatePlayerActsFirstWeaponBonuses(refreshCombatUI:true);
            // make player go first
            // CHARM: aether — player is faster → +1 speed next round
            if (s.itemManager.PlayerHasCharm("aether")) {
                s.itemManager.QueueCharmTrigger("aether", s.itemManager.GetCharmCount("aether"));
            }
            if (!PlayerAttacks()) {
                // if enemy was not killed
                StartCoroutine(RoundTwo("enemy"));
                // begin the next round where the player will attack
            }
            else {
                // enemy was killed
                StartCoroutine(Kill("enemy", ConsumeQueuedKillAsBleedOut("enemy"), false));
                ResolveBleedOutAfterRound();
                // execute the proper commands
            }
        }
        else {
            // enemy goes first
            s.player.SetPlayerStatusEffect("dodge", false);
            // enemy went first, so player can't be dodgy
            // CHARM: bulwark — player attacks second → +1 parry
            if (s.itemManager.PlayerHasCharm("bulwark")) {
                s.itemManager.ActivateCharmTriggerImmediately("bulwark", s.itemManager.GetCharmCount("bulwark"));
            }
            // CHARM: inevitable — player attacks second → +1 attack after sound plays (handled in EnemyAttacks)
            if (!EnemyAttacks()) {
                // if player doesn't die
                StartCoroutine(RoundTwo("player"));
                // start next round with player going
            }
            else {
                // player was killed
                StartCoroutine(Kill("player", ConsumeQueuedKillAsBleedOut("player"), false));
                ResolveBleedOutAfterRound();
                // show animation
            }
        }
        RecordEquippedWeaponUse();
        // increment the # of times the player's current weapon has been used
        Save.SavePersistent();
    }

    private void ExecuteGuardRoundLogic() {
        playerGuardActive = true;
        s.statSummoner.SummonStats();
        s.statSummoner.RepositionDice("player", "white");
        enemyAttackedFirst = false;
        s.player.SetPlayerStatusEffect("dodge", false);

        if (EnemyAttacks()) {
            StartCoroutine(Kill("player", ConsumeQueuedKillAsBleedOut("player"), false));
        }

        ResolveBleedOutAfterRound();
        StartCoroutine(CompleteRoundAfterResolution());
    }

    public void KillPlayerBySuicide() {
        if (s == null || s.player == null || s.player.isDead) { return; }

        criticalRoundInProgress = true;
        isMoving = true;
        playerBleedOutPendingThisRound = false;
        enemyBleedOutPendingThisRound = false;
        queuePlayerKillAsBleedOut = false;
        queueEnemyKillAsBleedOut = false;
        s.player.isDead = true;
        SetBleedOutNextRound("player", false, saveGame:false);
        Save.game.expendedStamina = 0;

        bool resurrectWithAmulet = s.tombstoneData != null && s.tombstoneData.HasUsableResurrectionAmulet();
        s.soundManager.PlayClip(resurrectWithAmulet ? "cloak" : "death");
        ApplyPlayerDeathVisualState();
        FinalizePlayerDeathState(resurrectWithAmulet);

        if (s.tutorial == null) { Save.SaveGame(); }
        Save.SavePersistent();
        criticalRoundInProgress = false;
        if (!resurrectWithAmulet) {
            isMoving = false;
        }
    }

    private void ApplyPlayerDeathVisualState() {
        if (s == null || s.player == null) { return; }

        SpriteRenderer spriteRenderer = s.player.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) {
            spriteRenderer.color = Color.white;
            spriteRenderer.sprite = s.player.GetDeathSprite();
        }

        Animator playerAnimator = s.player.GetComponent<Animator>();
        if (playerAnimator != null) {
            playerAnimator.enabled = false;
        }

        s.player.SetPlayerPositionAfterDeath();
    }

    private void FinalizePlayerDeathState(bool resurrectWithAmulet) {
        if (s == null || s.player == null) { return; }

        if (resurrectWithAmulet) {
            s.tombstoneData.PrepareResurrectionAmulet();
            s.tombstoneData.BeginPreparedResurrection();
        }
        else {
            s.player.stamina = 3;
            if (s.player.staminaCounter != null) {
                s.player.staminaCounter.text = s.player.stamina.ToString();
            }
            Save.game.playerStamina = s.player.stamina;
            s.tombstoneData.SetTombstoneData();
            Save.persistent.deaths++;
        }

        foreach (GameObject dice in s.diceSummoner.existingDice) {
            StartCoroutine(dice.GetComponent<Dice>().FadeOut(false));
        }
        s.statSummoner.ResetDiceAndStamina(refundEnemyPlannedStamina:resurrectWithAmulet);
        ClearVariablesAfterRound();
        s.statSummoner.SetDebugInformationFor("player");
        RecalculateMaxFor("player");
        RecalculateMaxFor("enemy");
    }

    /// <summary>
    /// Animate the nightmare plan reveal, then execute the round once the animation finishes.
    /// </summary>
    private IEnumerator NightmareRevealThenExecuteRound() {
        yield return StartCoroutine(EnemyAI.AnimateAndApplyNightmarePlan(s));
        // mirror the bare saves from RunEnemyCalculations after the reveal animation
        Save.game.enemyStamina = s.enemy.stamina;
        Save.game.enemyTargetIndex = s.enemy.targetIndex;
        if (s.tutorial == null) { Save.SaveGame(); }
        ExecuteRoundOneLogic();
    }

    private void RecordEquippedWeaponUse() {
        if (s == null || s.itemManager == null || s.player == null || s.player.inventory == null || s.player.inventory.Count == 0) { return; }

        Item equippedWeapon = s.player.inventory[0].GetComponent<Item>();
        if (equippedWeapon == null) { return; }

        string equippedWeaponName = ItemManager.NormalizeWeaponSaveName(equippedWeapon.itemName);
        int weaponIndex = ItemManager.IndexOfWeaponName(equippedWeaponName);
        if (weaponIndex < 0) {
            string fallbackWeaponName = ItemManager.GetWeaponBaseName(equippedWeapon.itemName).ToLowerInvariant();
            weaponIndex = ItemManager.IndexOfWeaponName(fallbackWeaponName);
        }
        if (weaponIndex < 0) {
            Debug.LogWarning($"Unable to record weapon use for '{equippedWeaponName}'");
            return;
        }

        if (Save.persistent.weaponUses == null || Save.persistent.weaponUses.Length <= weaponIndex) {
            Save.persistent.Normalize();
        }

        Save.persistent.weaponUses[weaponIndex]++;
    }

    private IEnumerator AttemptRegenStaminaAfterDelay() {
        int staminaToRestore = 0;
        bool deathcapRestorePending = Save.game.pendingDeathcapRestore;
        Save.game.pendingDeathcapRestore = false;

        if (s.itemManager.PlayerHasWeapon("dagger") && s.itemManager.PlayerHasLegendary()) {
            staminaToRestore += 1;
        }

        if (deathcapRestorePending && s.itemManager.PlayerHas("rabadons_deathcap")) {
            staminaToRestore += s.itemManager.GetPlayerItemCount("rabadons_deathcap");
        }

        staminaToRestore += s.itemManager.GetCursedMaskRegenAmount();

        if (staminaToRestore <= 0) {
            yield break;
        }

        yield return s.delays[0.3f];
        ChangeStaminaOf("player", staminaToRestore);
        s.soundManager.PlayClip("blip0");
    }

    /// <summary>
    /// Start the second round of attack, with the specified player or enemy attacking.
    /// </summary>
    private IEnumerator RoundTwo(string toMove) {
        isMoving = true;
        while (s?.diceSummoner != null && s.diceSummoner.DiceIsRolling()) {
            yield return null;
        }

        if (s.itemManager.PlayerHas("rabadons_deathcap") && s.player.stamina < 3) {
            Save.game.pendingDeathcapRestore = true;
        }
        // make the player ready to move
        yield return s.delays[2f];
        // wait 2 seconds for animation/status text from previous round to finish
        if (toMove == "player") {
            // if player is the one attacking
            if (scimitarParryCount > 0) {
                // if they parried and had a scimitar
                s.turnManager.SetStatusText(scimitarParryCount == 1 ? "discard enemy's die" : "discard two of enemy's dice");
                // notify player to discard die, depending on the #
                actionsAvailable = true;
                // allow for player to take actions
                for (float i = 5f; i > 0; i -= 0.1f) {
                    // 5s time slot to discard
                    if (s.diceSummoner.breakOutOfScimitarParryLoop) { break; }
                    // handle the discard somewhere else, if the action was taken then will break out
                    yield return s.delays[0.1f];
                    // wait 
                }
                actionsAvailable = false;
                // prevent further action
            }
            yield return StartCoroutine(HandleEnemyCounterattackRescuesBeforePlayerAttack());
            // get necessary stats 
            if (PlayerAttacks()) {
                // if player kills the enemy
                SetTargetOf("player");
                // reset target
                StartCoroutine(Kill("enemy", ConsumeQueuedKillAsBleedOut("enemy"), false));
                // play information
            }
        }
        else if (toMove == "enemy") {
            // enemy is the one attacking
            if ((s.enemy.woundList.Contains("chest") || s.itemManager.EnemyHasTemporaryChestInjury()) && Rerollable() && s.enemy.enemyName.text != "Lich" || Save.game.discardableDieCounter > 0 && s.enemy.enemyName.text != "Lich") {
                // if player can reroll or discard enemy's die and hints are on
                if (Save.game.discardableDieCounter > 0) { SetStatusText("note: you can discard enemy's die"); }
                else if (s.enemy.woundList.Contains("chest") || s.itemManager.EnemyHasTemporaryChestInjury()) { SetStatusText("note: you can reroll enemy's dice"); }
                // notify the player
                actionsAvailable = true;
                // allow actions
                for (float t = 2f; t > 0; t -= 0.1f) {
                    // 2 second time slot
                    if (alterationDuringMove) {
                        // actions handled elsewhere, but if there is an action taken (e.g. discard)
                        t += 1f;
                        // increase time slot
                        alterationDuringMove = false;
                        // allow timer to be changed again
                    }
                    yield return s.delays[0.1f];
                    // wait
                    if (Save.game.discardableDieCounter == 0 && !s.enemy.woundList.Contains("chest") && !s.itemManager.EnemyHasTemporaryChestInjury()) {
                        // if player has taken the action to reroll/discard and can't take it again, end the time slot early
                        t -= 1f;
                        break;
                    }
                    if (s.diceSummoner.existingDice.Where(d => d.GetComponent<Dice>().isOnPlayerOrEnemy == "enemy").All(d => d.GetComponent<Dice>().isRerolled)) {
                        // check every dice on the enemy; if all of them contain property isRerolled, break as well
                        if (t > 1.5f) { 
                            t = 1.4f;
                        }
                    }
                }
                actionsAvailable = false;
            }
            // get necessary stats
            if (EnemyAttacks()) {
                // if enemy kills player
                StartCoroutine(Kill("player", ConsumeQueuedKillAsBleedOut("player"), false));
                // play animation
            }
        }

        ResolveBleedOutAfterRound();

        yield return StartCoroutine(CompleteRoundAfterResolution());
    }

    private IEnumerator CompleteRoundAfterResolution() {
        if (!s.player.isDead && !Save.game.enemyIsDead) {
            // if neither player or enemy is dead
            yield return s.delays[2f];
            // wait for status text/animation etc.
            if (Save.game.isCourageous) {
                // if player is courageous (Save die to next round)
                actionsAvailable = true;
                // allow actions
                discardDieBecauseCourage = true;
                // make sure the die will discard under the correct pretense
                s.turnManager.SetStatusText("discard all your dice, except one");
                // notify player
                for (int i = 0; i < 50; i++) {
                    if ((from a in s.diceSummoner.existingDice where a.GetComponent<Dice>().isAttached && a.GetComponent<Dice>().isOnPlayerOrEnemy == "player" select a).ToList().Count <= 1) {
                        // if player has 1 (or less) die attached
                        dieSavedFromLastRound = (from a in s.diceSummoner.existingDice where a.GetComponent<Dice>().isOnPlayerOrEnemy == "player" select a).ToList()[0];
                        // Save the one remaining die
                        break;
                        // break out of the loop
                    }
                    yield return s.delays[0.1f];
                    // wait
                }
                if (dieSavedFromLastRound == null && s.diceSummoner.existingDice.Count > 0) {
                    // player has not discarded enough dice in the 5s time slot, so choose a random one
                    dieSavedFromLastRound = (from a in s.diceSummoner.existingDice where a.GetComponent<Dice>().isOnPlayerOrEnemy == "player" select a).ToList()[Random.Range(0, (from a in s.diceSummoner.existingDice where a.GetComponent<Dice>().isOnPlayerOrEnemy == "player" select a).ToList().Count)];
                    // Save random die
                }
                actionsAvailable = false;
                discardDieBecauseCourage = false;
                s.player.SetPlayerStatusEffect("courage", false);
                // reset necessary variables
            }
            // make the next person go again
            yield return s.delays[0.45f];
            ClearVariablesAfterRound(rerollLuckyDice:true);
            SetTargetOf("player");
            RecalculateMaxFor("enemy");
            
            // stop moving
            s.statSummoner.ResetDiceAndStamina();
            // reset die and stamina
            if (s.tutorial != null && s.tutorial.curIndex == 21) {
                s.tutorial.QueueParryDraftRound();
            }
            s.diceSummoner.SummonDice(false, true);
            // summon the die again
            s.statSummoner.SummonStats();
            // summon the stats again
            // RecalculateMaxFor("player");
            s.player.targetIndex = 0;
            // make sure the player and enemy are aiming at the correct place
            DetermineMove(true, true);
            yield return s.delays[0.35f];
            isMoving = false;
            SetTargetOf("player");
            SetTargetOf("enemy");
            // set the targets after isMoving is set to false, allowing the asterisks to be added without bugs 
        }
        if (s.tutorial == null) { Save.SaveGame(); }
        Save.SavePersistent();
        criticalRoundInProgress = false;
    }

    /// <summary>
    /// Reset all variables used in preparation for the next round.
    /// </summary>
    public void ClearVariablesAfterRound(bool rerollLuckyDice = false) {
        s.itemManager.EndEncounterWeaponState(rerollLuckyDice);
        s.itemManager.AdvanceWarhammerStunTurnState();
        playerGuardActive = false;
        ClearPotionStats();
        s.player.SetPlayerStatusEffect("fury", false);
        s.player.SetPlayerStatusEffect("dodge", false);
        s.player.SetPlayerStatusEffect("destructive", false);
        s.player.SetPlayerStatusEffect("fortified", false);
        s.player.SetPlayerStatusEffect("empowered", false);
        if (!dontRemoveLeechYet) {
            // if we don't want to remove the leech yet (from phylactery, don't do so)
            s.player.SetPlayerStatusEffect("leech", false);
        }
        s.highlightCalculator.diceTakenByPlayer = 0;
        Save.game.usedMace = false;
        Save.game.usedAnkh = false;
        Save.game.usedSpellbook = false;
        Save.game.usedBoots = false;
        Save.game.usedHelm = false;
        Save.game.pendingGemTransformColor = "";
        s.itemManager.ClearTemporaryEnemyInjuryScrollEffects();
        playerHasAttackedThisRound = false;
        s.diceSummoner.breakOutOfScimitarParryLoop = false;
        scimitarParryCount = 0;
        pendingEnemyHeadCounterDiscard = false;
        Save.game.discardableDieCounter = !Save.game.enemyIsDead && s.itemManager.IsFightableEncounter() && s.enemy.woundList.Contains("head") ? 1 : 0;
        Save.game.usedMace = false;
        Save.game.usedAnkh = false;
        Save.game.usedSpellbook = false;
        Save.game.usedBoots = false;
        Save.game.usedHelm = false;
        Save.game.expendedStamina = 0;
        if (s.tutorial == null) { Save.SaveGame(); }
        if (s.enemy.enemyName.text == "Lich" && !Save.game.enemyIsDead) {
            bool lichStaminaChanged = s.enemy.stamina != s.enemy.lichStamina;
            s.enemy.stamina = s.enemy.lichStamina;
            Save.game.enemyStamina = s.enemy.stamina;
            // refresh lich's stamina
            if (lichStaminaChanged) {
                s.soundManager.PlayClip("blip1");
            }
            // play sound clip
            s.enemy.staminaCounter.text = s.enemy.stamina.ToString();
        }
        dontRemoveLeechYet = false;
        // set it to be false regardless afterwards, because we only want it to persist for 1 round
        // CHARM: rotate pending bonuses → active, clear pending, save to GameData
        s.itemManager.RotatePendingCharmBonusesToActive();
        StartCoroutine(AttemptRegenStaminaAfterDelay());
    }

    /// <summary>
    /// Clear the stats gained from potions from the player.
    /// </summary>
    public void ClearPotionStats() {
        s.player.potionStats["green"] = 0;
        s.player.potionStats["blue"] = 0;
        s.player.potionStats["red"] = 0;
        s.player.potionStats["white"] = 0;
        s.statSummoner.SummonStats();
        Save.game.potionAcc = 0;
        Save.game.potionSpd = 0;
        Save.game.potionDmg = 0;
        Save.game.potionDef = 0;
        if (s.tutorial == null) { Save.SaveGame(); }
    }

    /// <summary>
    /// Coroutine to play the death animation, set status text, toggle variables, etc.
    /// </summary>
    private IEnumerator Kill(string playerOrEnemy, bool bleedOut = false, bool showStatusText = true) {
        bool usePlayerSoftKill = playerOrEnemy == "player"
            && s != null
            && s.tombstoneData != null
            && s.tombstoneData.HasUsableResurrectionAmulet();

        if (playerOrEnemy == "player") {
            s.player.isDead = true;
            SetBleedOutNextRound("player", false, saveGame:false);
        }
        else if (playerOrEnemy == "enemy") {
            Save.game.enemyIsDead = true;
            SetBleedOutNextRound("enemy", false, saveGame:false);
        }
        if (s.tutorial == null) { Save.SaveGame(); }
        yield return s.delays[0.55f];
        // short delay
        if (showStatusText) {
            if (bleedOut) {
                if (playerOrEnemy == "player") { SetStatusText(GetPlayerBleedOutStatusText()); }
                else { SetStatusText(GetEnemyBleedOutStatusText()); }
            }
            else if (playerOrEnemy == "player") {
                SetStatusText(GetPlayerKilledStatusText());
            }
            else {
                SetStatusText(GetEnemyKilledStatusText());
            }
        }
        if (usePlayerSoftKill) {
            StartCoroutine(PlaySoftPlayerKillAnimation());
        }
        else {
            StartCoroutine(PlayDeathAnimation(playerOrEnemy));
        }
        if (playerOrEnemy == "enemy") { Save.persistent.enemiesSlain++; }
        Save.game.expendedStamina = 0;
        Save.SavePersistent();
        if (s.tutorial == null) { Save.SaveGame(); }
        criticalRoundInProgress = false;
        yield return s.delays[1.4f];  
        isMoving = false;
    }

    /// <summary>
    /// Play the hit animation for the player or enemy.
    /// </summary>
    private IEnumerator PlayHitAnimation(string playerOrEnemy) {
        SpriteRenderer spriteRenderer = playerOrEnemy == "player" ? s.player.GetComponent<SpriteRenderer>() : s.enemy.GetComponent<SpriteRenderer>();
        // get the proper spriterenderer
        // conditional is true ? yes : no
        Color temp = Color.white;
        temp.a = 0.5f;
        // white with 50% transparency
        spriteRenderer.color = temp;
        for (int i = 0; i < 14; i++) {
            yield return s.delays[0.0125f];
            temp.a -= 1f / 28f;
            spriteRenderer.color = temp;
        }
        // fade out
        if (s.enemy.spawnNum == 0 && playerOrEnemy == "enemy") {
            // if cloaked devil
            s.enemy.spawnNum = 1;
            Save.game.enemyNum = s.enemy.spawnNum;
            s.enemy.GetComponent<Animator>().runtimeAnimatorController = s.enemy.controllers[1];
            s.player.GetComponent<Animator>().Rebind();
            s.player.GetComponent<Animator>().Update(0f);
            s.enemy.GetComponent<Animator>().Rebind();
            s.enemy.GetComponent<Animator>().Update(0f);
            // reset devil animation after his cloak shatters, so it stays synced up
            // turn the cloaked into devil
            spriteRenderer.color = temp;
            DisplayWounds();
            // show the wounds (go from [cloaked] to [no wounds]).
        }
        for (int i = 0; i < 28; i++) {
            yield return s.delays[0.0125f];
            temp.a += 1f / 28f;
            spriteRenderer.color = temp;
        }
        // fade back in
    }

    /// <summary>
    /// Play the death animation for the player or enemy. Also handle things like clearing potion stats.
    /// </summary>
    private IEnumerator PlayDeathAnimation(string playerOrEnemy) {
        bool resurrectWithAmulet = playerOrEnemy == "player" && s.tombstoneData.HasUsableResurrectionAmulet();
        SpriteRenderer spriteRenderer = playerOrEnemy == "player" ? s.player.GetComponent<SpriteRenderer>() : s.enemy.GetComponent<SpriteRenderer>();
        // get the proper spriterenderer
        // conditional is true ? yes : no
        Color temp = Color.white;
        temp.a = 0.5f;
        // white with 50% transparency
        for (int i = 0; i < 14; i++) {
            yield return s.delays[0.0125f];
            temp.a -= 1f / 28f;
            spriteRenderer.color = temp;
        }
        // fade out
        if (playerOrEnemy == "player") { s.player.GetComponent<Animator>().enabled = false; }
        else if (playerOrEnemy == "enemy") {
            s.enemy.GetComponent<Animator>().enabled = false;
        }
        // disable the animator for whoever is dead
        for (int i = 0; i < 28; i++) {
            yield return s.delays[0.0125f];
            temp.a += 1f / 28f;
            spriteRenderer.color = temp;
        }
        // fade back in
        yield return s.delays[0.8f];
        // pause (animation is not playing so it looks like they are just standing there)
        if (playerOrEnemy == "enemy") {
            if (s.enemy.enemyName.text == "Skeleton") { s.soundManager.PlayClip("clank"); }
            else if (s.enemy.enemyName.text == "Lich") {
                s.soundManager.PlayClip("scream");
                s.enemy.GetComponent<Animator>().enabled = true;
                s.enemy.GetComponent<Animator>().runtimeAnimatorController = s.enemy.lichDeathController;
            }
            else { s.soundManager.PlayClip("death"); }
        }
        else {
            s.soundManager.PlayClip(resurrectWithAmulet ? "cloak" : "death");
        }
        // play sound clip
        if (s.itemManager.PlayerHasWeapon("rapier") && playerOrEnemy == "enemy") {
            StartCoroutine(RapierGainAfterDelay());
        }
        // if player has rapier and the enemy dies, add to their stamina based on whether its legendary or not
        if (playerOrEnemy == "player") {
            ApplyPlayerDeathVisualState();
            // if player dies, set sprite and proper position
            FinalizePlayerDeathState(resurrectWithAmulet);
        }
        else if (playerOrEnemy == "enemy") {
            if (s.enemy.enemyName.text == "Devil") {
                // after killing the devil, the music fades to normal
                s.music.FadeVolume("Through");
            }
            if (s.enemy.enemyName.text == "Lich") {
                s.enemy.GetComponent<Animator>().enabled = true;
                yield return s.delays[1f];
                s.soundManager.PlayClip("clang");
            }
            s.enemy.GetComponent<SpriteRenderer>().sprite = s.enemy.GetDeathSprite();
            s.enemy.SetEnemyPositionAfterDeath();
            // if enemy dies, set sprite and proper position
            s.itemManager.SpawnItems();
            // spawn items
            RefreshBlackBoxVisibility();
            // hide the enemy's stats
            if (s.tutorial != null) { s.tutorial.Increment(); }
            foreach (GameObject dice in s.diceSummoner.existingDice) {
                StartCoroutine(dice.GetComponent<Dice>().FadeOut(false));
                // fade out all existing die
            }
            s.statSummoner.ResetDiceAndStamina();
            // clear them
            ClearVariablesAfterRound();
            // clear potion stats
            s.statSummoner.SetDebugInformationFor("player");
            // set debug (only player needed here)
            RecalculateMaxFor("player");
            RecalculateMaxFor("enemy");
            // reset target for both
            Save.SavePersistent();
            yield break;
        }
        Save.SavePersistent();
    }

    private IEnumerator PlaySoftPlayerKillAnimation() {
        if (softKillInProgress) { yield break; }
        softKillInProgress = true;

        SpriteRenderer spriteRenderer = s.player.GetComponent<SpriteRenderer>();
        Color temp = Color.white;
        temp.a = 0.5f;
        for (int i = 0; i < 14; i++) {
            yield return s.delays[0.0125f];
            temp.a -= 1f / 28f;
            spriteRenderer.color = temp;
        }

        Animator playerAnimator = s.player.GetComponent<Animator>();
        if (playerAnimator != null) {
            playerAnimator.enabled = false;
        }

        for (int i = 0; i < 28; i++) {
            yield return s.delays[0.0125f];
            temp.a += 1f / 28f;
            spriteRenderer.color = temp;
        }

        yield return s.delays[0.8f];
        s.soundManager.PlayClip("cloak");
        s.player.GetComponent<SpriteRenderer>().sprite = s.player.GetDeathSprite();
        s.player.SetPlayerPositionAfterDeath();

        s.tombstoneData.PrepareResurrectionAmulet();
        s.tombstoneData.BeginPreparedResurrection();
        softKillInProgress = false;
    }

    private IEnumerator RapierGainAfterDelay() { 
        yield return s.delays[1.15f];
        ChangeStaminaOf("player", s.itemManager.PlayerHasLegendary() ? 5 : 3); 
        s.soundManager.PlayClip("blip0");
    }
    /// <summary>
    /// Coroutine for fading in the status text.
    /// </summary>
    private IEnumerator StatusTextCoroutine(string text, bool extraDuration) {
        Color temp = statusText.color;
        // make the text invisible
        statusText.text = "";
        temp.a = 0f;
        statusText.color = temp;
        // set the status text to the desired text
        for (int i = 0; i < 10; i++) {
            statusText.text = text;
            yield return s.delays[0.033f];
            temp.a += 0.1f;
            statusText.color = temp;
        }
        // fade in
        if (extraDuration) { 
            yield return s.delays[3f];
        }
        else { 
            yield return s.delays[1.5f];
        }
        // wait 1 sec (so player has time to read)
        for (int i = 0; i < 10; i++) {
            yield return s.delays[0.033f];
            temp.a -= 0.1f;
            statusText.color = temp;
        }
        // fade out
        statusText.text = "";
        // reset the text
    }

    /// <summary>
    /// Update the status text.
    /// </summary>
    /// <param name="text">What to set the new status text to</param>
    public void SetStatusText(string text, bool extraDuration = false) {

        // BRIEFLY GLITCHES!

        if (text != statusText.text) {
            // if the message is not already displayed
            statusText.text = "";
            // clear the status text
            try { StopCoroutine(coroutine); } catch {}
            // stop any existing status text coroutines
            coroutine = StartCoroutine(StatusTextCoroutine(text, extraDuration));
            // set the status text, and allow for it to be stopped
        }
    }

    private void SetPlayerAttackStatusText(string text) {
        if (string.IsNullOrWhiteSpace(text) || playerAttackStatusSetThisRound) { return; }

        playerAttackStatusSetThisRound = true;
        SetStatusText(text);
    }

    private void SetEnemyAttackStatusText(string text) {
        if (string.IsNullOrWhiteSpace(text) || enemyAttackStatusSetThisRound) { return; }

        enemyAttackStatusSetThisRound = true;
        SetStatusText(text);
    }

    public void SetBleedOutNextRound(string playerOrEnemy, bool bleedsOut, bool saveGame = true) {
        if (playerOrEnemy == "player") {
            Save.game.playerBleedsOutNextRound = bleedsOut;
        }
        else if (playerOrEnemy == "enemy") {
            Save.game.enemyBleedsOutNextRound = bleedsOut;
        }
        else {
            Debug.LogError("invalid string passed in to SetBleedOutNextRound() in TurnManager.cs");
            return;
        }

        if (saveGame && s.tutorial == null) {
            Save.SaveGame();
        }
    }

    private void QueueKillAsBleedOut(string playerOrEnemy) {
        if (playerOrEnemy == "player") {
            queuePlayerKillAsBleedOut = true;
        }
        else if (playerOrEnemy == "enemy") {
            queueEnemyKillAsBleedOut = true;
        }
    }

    private bool ConsumeQueuedKillAsBleedOut(string playerOrEnemy) {
        if (playerOrEnemy == "player") {
            bool useBleedOutStatus = queuePlayerKillAsBleedOut;
            queuePlayerKillAsBleedOut = false;
            return useBleedOutStatus;
        }

        bool enemyUsesBleedOutStatus = queueEnemyKillAsBleedOut;
        queueEnemyKillAsBleedOut = false;
        return enemyUsesBleedOutStatus;
    }

    /// <summary>
    /// Perform actions (sound, animation) for when a player or enemy is hit.
    /// </summary>
    private IEnumerator DoStuffForAttack(string hitOrParry, string playerOrEnemy, bool showAnimation = true, bool armor = false, bool charmShatter = false, Action onHitSound = null) {
        yield return s.delays[0.55f];
        // wait
        if (s.statSummoner.SumOfStat("green", "player") < 0 && playerOrEnemy == "enemy") {
            s.soundManager.PlayClip("miss");
        }
        else {
            if (hitOrParry == "hit") {
                if (Save.game.isDodgy && playerOrEnemy == "player") {
                    s.soundManager.PlayClip("miss");
                    // play sound clip
                }
                // player dodges
                else {
                    if (charmShatter) {
                        s.soundManager.PlayClip("cloak");
                        onHitSound?.Invoke();
                    }
                    else if (!(playerOrEnemy == "player" && armor || (s.enemy.spawnNum == 0 && playerOrEnemy == "enemy"))) {
                        s.soundManager.PlayClip("hit");
                        onHitSound?.Invoke();
                    }
                    // play sound clip if conditions apply (not hitting armored player or devil)
                    else if (s.enemy.spawnNum == 0 && playerOrEnemy == "enemy") {
                        s.soundManager.PlayClip("cloak");
                        onHitSound?.Invoke();
                    }
                    // play cloak shatter here if needed
                    if (showAnimation) {
                        // if showing an animation
                        if (!(playerOrEnemy == "player" && armor)) {
                            StartCoroutine(PlayHitAnimation(playerOrEnemy));
                        }
                        else if (!charmShatter) { s.soundManager.PlayClip("armor"); } // play sound clip
                    }
                }
            }
            else if (hitOrParry == "parry") {
                s.soundManager.PlayClip("parry");
                // play sound clip
            }
            else { Debug.LogError("invalid string passed"); }
        }
    }

    /// <summary>
    /// helper coroutine to trigger inevitable charm after sound plays
    /// </summary>
    private IEnumerator TriggerInevitableCharmAfterSound() {
        yield return s.delays[0.55f];
        if (s.itemManager.PlayerHasCharm("inevitable")) {
            s.itemManager.ActivateCharmTriggerImmediately("inevitable", s.itemManager.GetCharmCount("inevitable"));
        }
    }

    /// <summary>
    /// Perform actions for the enemy's attack.
    /// </summary>
    private bool EnemyAttacks() {
        InitializeVariables(out int playerAim, out int enemyAim, out int playerSpd, out int enemySpd, out int playerAtt, out int enemyAtt, out int playerDef, out int enemyDef);
        bool armor = false;
        string enemyAttackStatusText = null;
        bool chaliceWillDrink = false;
        bool playerBleedsOutThisRound = PlayerBleedsOutThisRound();
        bool playerWouldAutoHeal = Save.game.curCharNum == 3 && s.player.woundList.Count < 2 && s.player.stamina >= 7;
        bool enemyHitOnExistingWound = s.player.woundList.Contains(s.enemy.target.text);
        bool enemyHitCountsAsWounded = !Save.game.isDodgy;
        bool enemyHitAddsPersistentWound = enemyHitCountsAsWounded && !enemyHitOnExistingWound && !playerWouldAutoHeal;
        bool enemyHitIsFatal = enemyHitAddsPersistentWound && s.player.woundList.Count == 2;
        bool phylacteryTriggers = !armor
            && enemyHitAddsPersistentWound
            && !enemyHitIsFatal
            && s.itemManager.PlayerHas("phylactery")
            && !Save.game.isBloodthirsty;
        // initialize armor as false
        s.soundManager.PlayClip("swing");
        // play sound clip
        if (enemyAtt > playerDef) {
            // if enemy is hitting player
            if (Save.game.isDodgy) { enemyAttackStatusText = $"{s.enemy.enemyName.text.ToLower()} hits you... you dodge"; }
            // if player dodges, notify 
            else {
                if (s.itemManager.PlayerHas("armor")) {
                    armor = true;
                    phylacteryTriggers = false;
                    // set armor to true
                    enemyAttackStatusText = $"{s.enemy.enemyName.text.ToLower()} hits you... your armor shatters";
                    Save.persistent.armorBroken++;
                    s.itemManager.MarkItemUsed("armor");
                    // notify player
                    StartCoroutine(RemoveArmorAfterDelay());
                    s.itemManager.Select(s.player.inventory, 0, playAudio:false);
                    // select weapon
                }
                else if (phylacteryTriggers) {
                    enemyAttackStatusText = $"{s.enemy.enemyName.text.ToLower()} hits you... you thirst for blood";
                }
                else if (s.enemy.spawnNum is 0 or 1) {
                    enemyAttackStatusText = $"devil twists claws in your {s.enemy.target.text}!";
                }
                else if (enemyHitIsFatal) {
                    enemyAttackStatusText = GetPlayerKilledStatusText();
                }
                else if (!enemyHitIsFatal) {
                    // if player won't die immediately
                    chaliceWillDrink = enemyHitCountsAsWounded && s.itemManager.PlayerHas("sacrificial_chalice");
                    enemyAttackStatusText = chaliceWillDrink
                        ? $"{s.enemy.enemyName.text.ToLower()} hits you... the chalice drinks your blood!"
                        : $"{s.enemy.enemyName.text.ToLower()} hits you, damaging {s.enemy.target.text}!";
                    // notify player
                    Save.persistent.woundsReceived++;
                }
            }
            SetEnemyAttackStatusText(enemyAttackStatusText);
            // determine whether this hit should use cloak because something shatters
            bool woundCountsForTriggers = !armor && enemyHitCountsAsWounded;
            bool crystalShardShatters = woundCountsForTriggers && s.itemManager.PlayerHas("crystal_shard");
            bool glassSwordShatters = woundCountsForTriggers
                && s.itemManager.PlayerHasWeapon("glass sword")
                && !Save.game.glassSwordShattered
                && !crystalShardShatters;
            bool charmShatter = crystalShardShatters || glassSwordShatters;
            Action onHitSound = woundCountsForTriggers
                ? () => s.itemManager.TryAdvanceSacrificialChalice()
                : null;
            StartCoroutine(DoStuffForAttack("hit", "player", true, armor, charmShatter, onHitSound));
            // play animation + sound for the attack
            // CHARM: inevitable — +1 attack when enemy attacks first (triggered after sound)
            if (enemyAttackedFirst) {
                StartCoroutine(TriggerInevitableCharmAfterSound());
            }
            if (!armor && !Save.game.isDodgy) {
                // if the hit landed without armor/dodge mitigation:
                if (s.itemManager.PlayerHasCharm("vindictive")) {
                    if (enemyAttackedFirst) {
                        s.itemManager.ActivateCharmTriggerImmediately("vindictive", s.itemManager.GetCharmCount("vindictive"));
                    }
                    else {
                        s.itemManager.QueueCharmTrigger("vindictive", s.itemManager.GetCharmCount("vindictive"));
                    }
                }
                if (crystalShardShatters) {
                    int crystalShardCount = s.itemManager.GetPlayerItemCount("crystal_shard");
                    for (int i = 0; i < crystalShardCount; i++) {
                        s.itemManager.MarkItemUsed("crystal_shard");
                    }
                    s.itemManager.RegisterCrystalShardShatter(crystalShardCount);
                    StartCoroutine(RemoveItemsAfterDelay("crystal_shard", crystalShardCount));
                }
                if (glassSwordShatters) {
                    Save.game.glassSwordShattered = true;
                    StartCoroutine(ShatterGlassSwordAfterDelay());
                }

                if (Save.game.curCharNum == 3 && s.player.woundList.Count < 2 && s.player.stamina >= 7) {
                    // if on the 4th char, they are able to heal back (wont die instantly), and has sufficient stamina to heal the next move
                    s.player.woundList.Clear();
                    StartCoroutine(HealAfterDelay());
                    // decrementing and healing handled in the coro
                }
                else {
                    if (Save.game.curCharNum == 3 && s.player.stamina < 7) {
                        // on the 4th char but does not have sufficient stamina to heal
                        ChangeStaminaOf("player", 3);
                        // so merely increment their stamina
                    }
                    if (!enemyHitOnExistingWound) {
                        s.player.woundList.Add(s.enemy.target.text);
                        // add the hit, but only if the player did not yet have that wound
                        Save.game.playerWounds = s.player.woundList;
                        StartCoroutine(InjuredTextChange(
                            s.player.woundGUIElement, null,
                            phylacteryTriggers ? GrantPhylacteryLeech : null));
                        // make it change
                        RecalculateMaxFor("player");
                        // reset stuff
                        if (s.tutorial == null) { Save.SaveGame(); }
                        if (s.player.woundList.Count > 0) {
                            // wounds were not healed, so apply them normally
                            pendingEnemyHeadCounterDiscard = enemyAttackedFirst && s.enemy.target.text == "head";
                            bool playerDiesFromAppliedWound = ApplyInjuriesDuringMove(s.enemy.target.text, "player");
                            if (playerDiesFromAppliedWound) {
                                pendingEnemyHeadCounterDiscard = false;
                            }
                            if (playerDiesFromAppliedWound && playerBleedsOutThisRound) {
                                QueueKillAsBleedOut("player");
                                return true;
                            }
                            return playerDiesFromAppliedWound;
                        }
                        // wounds were healed, so don't apply them and don't kill the player
                    }
                }
                return false;
                // return that the player did not die
            }
        }
        else {
            // player parried
            StartCoroutine(DoStuffForAttack("parry", "player"));
            // play sound and animation
            // CHARM: inevitable — +1 attack when enemy attacks first (triggered after sound)
            if (enemyAttackedFirst) {
                StartCoroutine(TriggerInevitableCharmAfterSound());
            }
            if (enemyAtt < 0) { enemyAttackStatusText = $"{s.enemy.enemyName.text.ToLower()} hits you... the attack is to weak"; }
            else {
                if (s.itemManager.PlayerHasWeapon("scimitar")) {
                    scimitarParryCount += s.itemManager.PlayerHasLegendary() ? 2 : 1;
                    // legendary scimitar gets 2 discard, regular only gets 1
                }
                // player has parried (this resets at the start of every round so we can do it regardless)
                // CHARM: unbroken — parry grants +1 parry next round
                if (s.itemManager.PlayerHasCharm("unbroken")) {
                    s.itemManager.QueueCharmTrigger("unbroken", s.itemManager.GetCharmCount("unbroken"));
                }
                // CHARM: riposte — parry grants +1 attack (immediate if enemy went first, else next round)
                if (s.itemManager.PlayerHasCharm("riposte")) {
                    if (enemyAttackedFirst) {
                        s.itemManager.ActivateCharmTriggerImmediately("riposte", s.itemManager.GetCharmCount("riposte"));
                    }
                    else {
                        s.itemManager.QueueCharmTrigger("riposte", s.itemManager.GetCharmCount("riposte"));
                    }
                }
                enemyAttackStatusText = IsPlayerGuardActive() && !playerBleedsOutThisRound
                    ? $"{s.enemy.enemyName.text.ToLower()} hits you... you guard"
                    : $"{s.enemy.enemyName.text.ToLower()} hits you... you parry";
                Save.persistent.attacksParried++;
                Save.SavePersistent();
            }
            SetEnemyAttackStatusText(enemyAttackStatusText);
            // notify player
        }
        return false;
        // player hasn't died, so return false
    }

    /// <summary>
    /// Coroutine to remove the player's armor after being hit.
    /// </summary>
    private IEnumerator RemoveArmorAfterDelay() {
        yield return s.delays[0.45f];
        s.itemManager.GetPlayerItem("armor").GetComponent<Item>().Remove(armorFade:true);
        
    }

    private IEnumerator RemoveItemAfterDelay(string itemName, float delay = 0.45f) {
        if (delay > 0f) {
            yield return new WaitForSeconds(delay);
        }

        GameObject itemObj = s.itemManager.GetPlayerItem(itemName);
        if (itemObj != null) { itemObj.GetComponent<Item>().Remove(); }
    }

    private IEnumerator RemoveItemsAfterDelay(string itemName, int amount, float delay = 0.45f) {
        if (amount <= 0) { yield break; }
        if (delay > 0f) {
            yield return new WaitForSeconds(delay);
        }

        for (int i = 0; i < amount; i++) {
            GameObject itemObj = s.itemManager.GetPlayerItem(itemName);
            if (itemObj == null) { yield break; }
            itemObj.GetComponent<Item>().Remove();
            yield return null;
        }
    }

    /// <summary>
    /// Downgrades glass sword to broken stats after a short delay (1/1/0/0 pattern).
    /// </summary>
    private IEnumerator ShatterGlassSwordAfterDelay() {
        yield return s.delays[0.55f];
        Item weapon = s.player.inventory[0].GetComponent<Item>();
        if (weapon == null || !weapon.itemName.Contains("glass sword")) {
            yield break;
        }

        weapon.modifier = "shattered";
        weapon.itemName = "shattered glass sword";
        weapon.weaponStats["green"] = 0;
        weapon.weaponStats["blue"]  = 1;
        weapon.weaponStats["red"]   = 1;
        weapon.weaponStats["white"] = 0;
        weapon.GetComponent<SpriteRenderer>().sprite = s.itemManager.GetItemSprite("glass_sword_shattered");
        s.player.stats = weapon.weaponStats;
        Save.game.resumeAcc = 0;
        Save.game.resumeSpd = 1;
        Save.game.resumeDmg = 1;
        Save.game.resumeDef = 0;
        s.itemManager.RefreshPlayerCombatStatsAndDice();
        if (s.itemManager.highlightedItem == weapon.gameObject) {
            weapon.Select(playAudio:false);
        }
        s.itemManager.SaveInventoryItems();
    }


    private IEnumerator HealAfterDelay() {
        yield return s.delays[1f];
        s.turnManager.ChangeStaminaOf("player", -7);
        // bypass changestaminaof and instead directly change the stamina, to differentiate from healing from eating
        if (s.tutorial == null) { Save.SaveGame(); }
        s.soundManager.PlayClip("blip0");
        DisplayWounds();
    }

    /// <summary>
    /// Handles the player's attack, returns true if it was a killing blow.
    /// </summary>
    private bool PlayerAttacks() {
        playerHasAttackedThisRound = true;
        InitializeVariables(out int playerAim, out int enemyAim, out int playerSpd, out int enemySpd, out int playerAtt, out int enemyAtt, out int playerDef, out int enemyDef);
        bool enemyBleedsOutThisRound = EnemyBleedsOutThisRound();
        string playerAttackStatusText = null;
        string currentTargetWound = GetNormalizedTargetName(s.player.target.text);
        bool playerHasLeech = Save.game.isBloodthirsty;
        bool playerWillLeech = playerHasLeech && !string.IsNullOrEmpty(currentTargetWound) && s.player.woundList.Contains(currentTargetWound);
        bool clearLeechAtBlackout = false;
        bool glaiveWouldKill = s.itemManager.PlayerHasWeapon("glaive")
            && !string.IsNullOrEmpty(currentTargetWound)
            && !s.player.target.text.Contains("*")
            && s.enemy.woundList.Count == 1;
        string FinalizePlayerAttackStatusText(string text) {
            if (!string.IsNullOrWhiteSpace(text)) { return text; }
            return enemyBleedsOutThisRound ? GetEnemyBleedOutStatusText() : text;
        }
        // CHARM: ruthless \u2014 targeting neck this attack grants +3 accuracy next round
        if (s.itemManager.PlayerHasCharm("ruthless") && currentTargetWound == "neck") {
            s.itemManager.QueueCharmTrigger("ruthless", s.itemManager.GetCharmCount("ruthless"));
        }
        s.soundManager.PlayClip("swing");
        // play sound clip
        bool playerAttackClearsDefense = PlayerAttackClearsEnemyDefense(playerAtt, enemyDef);
        if (playerAttackClearsDefense) {
            // if player will hit enemy
            if (playerAim < 0) {
                // player doesn't have enough accuracy to hit, so notify
                playerAttackStatusText = enemyBleedsOutThisRound
                    ? GetEnemyBleedOutStatusText()
                    : $"you hit {s.enemy.enemyName.text.ToLower()}... you miss";
                // s.soundManager.PlayClip("miss");-
                // play sound clip
                StartCoroutine(DoStuffForAttack("hit", "enemy"));
                SetPlayerAttackStatusText(FinalizePlayerAttackStatusText(playerAttackStatusText));
            }
            else {
                if (enemyBleedsOutThisRound) {
                    StartCoroutine(DoStuffForAttack("hit", "enemy", false));
                    playerAttackStatusText = GetEnemyBleedOutStatusText();
                    Save.persistent.woundsInflicted++;
                    Save.persistent.woundsInflictedArr[s.player.targetIndex]++;
                }
                else if ((s.enemy.woundList.Count == 2 || glaiveWouldKill) && !s.player.target.text.Contains("*")) {
                    // enemy is going to die
                    StartCoroutine(DoStuffForAttack("hit", "enemy", false));
                    playerAttackStatusText = GetEnemyKilledStatusText();
                    // play sound but no animation
                    Save.persistent.woundsInflicted++;
                    Save.persistent.woundsInflictedArr[s.player.targetIndex]++;
                }
                else {
                    // enemy will not die
                    StartCoroutine(DoStuffForAttack("hit", "enemy"));
                    // play sound and animation 
                    if (s.enemy.spawnNum == 0) {
                        playerAttackStatusText = "you hit devil... his cloak shatters";
                    }
                    else if (playerWillLeech) {
                        playerAttackStatusText = $"you hit {s.enemy.enemyName.text.ToLower()}, leeching his {currentTargetWound}!";
                    }
                    else if (s.player.target.text.Contains("*")) {
                        playerAttackStatusText = $"you hit {s.enemy.enemyName.text.ToLower()}, damaging {s.player.target.text.Substring(1)}!";
                    }
                    else {
                        playerAttackStatusText = $"you hit {s.enemy.enemyName.text.ToLower()}, damaging {s.player.target.text}!";
                    }
                    Save.persistent.woundsInflicted++;
                    Save.persistent.woundsInflictedArr[s.player.targetIndex]++;
                }
                SetPlayerAttackStatusText(FinalizePlayerAttackStatusText(playerAttackStatusText));
                if (s.statSummoner.SumOfStat("green", "player") >= 0) {
                    if (s.enemy.spawnNum != 0) {
                        if (s.itemManager.PlayerHasWeapon("warhammer")) {
                            s.itemManager.QueueWarhammerStunForNextTurn();
                        }
                        if (s.itemManager.PlayerHasCharm("relentless")) {
                            s.itemManager.QueueCharmTrigger("relentless", s.itemManager.GetCharmCount("relentless"));
                        }
                    }
                    if (playerWillLeech) {
                        s.player.woundList.Remove(currentTargetWound);
                        ClearBleedOutForRecoveredWound("player", currentTargetWound);
                        clearLeechAtBlackout = true;
                    }
                    bool enemyWoundTextChanged = false;
                    if (!s.enemy.woundList.Contains(s.player.target.text) && !s.player.target.text.Contains("*")) {
                        // if wound was not injured and player has enough accuracy to hit
                        Action onEnemyWoundBlackAfterSound = null;
                        bool enemyHadNoWoundsBeforeHit = s.enemy.woundList.Count == 0;
                        if (s.enemy.spawnNum != 0) {
                            s.enemy.woundList.Add(s.player.target.text);
                            // add the wound
                            Save.game.enemyWounds = s.enemy.woundList;
                            UpdateSavedEnemyDeathStateFromWounds();
                            onEnemyWoundBlackAfterSound = () => s.itemManager.TryApplyKatarFirstWoundEffect(enemyHadNoWoundsBeforeHit);
                        }
                        if (s.tutorial == null) { Save.SaveGame(); }
                        if (Save.game.curCharNum == 2) {
                            s.turnManager.ChangeStaminaOf("player", 1);
                            // increment stamina if on 3rd character
                        }
                        enemyWoundTextChanged = true;
                        // make the text change
                        RecalculateMaxFor("enemy");
                        StartCoroutine(InjuredTextChange(s.enemy.woundGUIElement, onBlackAfterSound:onEnemyWoundBlackAfterSound));
                        if (playerWillLeech) {
                            StartCoroutine(InjuredTextChange(s.player.woundGUIElement, "blip0", null, ClearPlayerLeechEffect));
                        }
                        // recalculate max
                        if (s.enemy.spawnNum != 0) {
                            if (playerHasLeech && !clearLeechAtBlackout) { ClearPlayerLeechEffect(); }
                            // cloaked devil is unaffected by all wounds
                            bool enemyDiesFromAppliedWound = ApplyInjuriesDuringMove(
                                s.player.target.text,
                                "enemy",
                                neckStyleBleedOut: s.itemManager.PlayerHasWeapon("maul") && s.enemy.enemyName.text != "Lich"
                            );
                            if (enemyDiesFromAppliedWound && enemyBleedsOutThisRound) {
                                QueueKillAsBleedOut("enemy");
                            }
                            return enemyDiesFromAppliedWound;
                        }
                        // return if the enemy dies and at the same time apply wounds instantly
                        
                    }
                    if (playerWillLeech) {
                        if (!enemyWoundTextChanged) {
                            StartCoroutine(InjuredTextChange(s.player.woundGUIElement, "blip0", null, ClearPlayerLeechEffect));
                        }
                    }
                }
            }
        }
        else {
            if (enemyBleedsOutThisRound) {
                StartCoroutine(DoStuffForAttack("hit", "enemy", false));
                playerAttackStatusText = GetEnemyBleedOutStatusText();
            }
            else {
                // enemy will parry
                StartCoroutine(DoStuffForAttack("parry", "enemy"));
                // play animation and sound
                if (PlayerAttackIsTooWeak(playerAtt)) { playerAttackStatusText = $"you hit {s.enemy.enemyName.text.ToLower()}... the attack is too weak"; }
                else if (playerAim < 0) {
                    playerAttackStatusText = $"you hit {s.enemy.enemyName.text.ToLower()}... you miss";
                    // s.soundManager.PlayClip("miss");
                    // play sound clip
                }
                else { playerAttackStatusText = $"you hit {s.enemy.enemyName.text.ToLower()}... he parries"; }
            }
            SetPlayerAttackStatusText(FinalizePlayerAttackStatusText(playerAttackStatusText));
            // depending on the stats, notify player accordingly
        }
        return false;
        // enemy has not died, so return false
    }

    /// <summary>
    /// Instantly apply injury effects (such as decreasing die on gut wound).
    /// </summary>
    private bool ApplyInjuriesDuringMove(string injury, string appliedTo, bool neckStyleBleedOut = false) {
        StartCoroutine(ApplyInjuriesDuringMoveCoro(injury, appliedTo));
        // start applying the injuries
        if (injury == "neck" || neckStyleBleedOut) {
            if (!(appliedTo == "enemy" && s.enemy.enemyName.text == "Lich")) {
                SetBleedOutNextRound(appliedTo, true, saveGame:false);
            }
        }
        if (appliedTo == "player" && s.player.woundList.Count >= 3) { return true; }
        if (appliedTo == "enemy" && s.itemManager.PlayerHasWeapon("glaive") && s.enemy.woundList.Count >= 2) { return true; }
        if (appliedTo == "enemy" && s.enemy.woundList.Count >= 3) { return true; }
        return false;
        // return true or false here, based on whether the enemy was killed or not.
    }

    private void UpdateSavedEnemyDeathStateFromWounds() {
        if (s == null || s.enemy == null) { return; }
        if (s.enemy.enemyName.text == "Lich") { return; }

        bool enemyHasFatalWounds = s.enemy.woundList.Count >= 3
            || (s.itemManager.PlayerHasWeapon("glaive") && s.enemy.woundList.Count >= 2);

        if (!enemyHasFatalWounds) { return; }

        Save.game.enemyIsDead = true;
        Save.game.enemyBleedsOutNextRound = false;
    }
    // hello 
    /// <summary>
    /// Do not call this coroutine, use ApplyInjuriesDuringMove() instead.
    /// </summary>
    private IEnumerator ApplyInjuriesDuringMoveCoro(string injury, string appliedTo) {
        yield return s.delays[0.45f];
        if (injury == "guts") {
            // for guts, decrease all die
            if (appliedTo == "player") {
                foreach (string key in s.statSummoner.addedPlayerDice.Keys) {
                    foreach (Dice dice in s.statSummoner.addedPlayerDice[key]) {
                        StartCoroutine(dice.DecreaseDiceValue(false));
                    }
                }
                RecalculateMaxFor("player");
            }
            else if (appliedTo == "enemy" && s.enemy.enemyName.text != "Lich") {
                foreach (string key in s.statSummoner.addedEnemyDice.Keys) {
                    foreach (Dice dice in s.statSummoner.addedEnemyDice[key]) {
                        StartCoroutine(dice.DecreaseDiceValue(false));
                    }
                }
                RecalculateMaxFor("enemy");
            }
        }
        else if (injury == "hip") {
            // if hip, remove all applied stamina
            if (appliedTo == "player") {
                s.statSummoner.addedPlayerStamina = new Dictionary<string, int> {
                    { "green", 0 },
                    { "blue", 0 },
                    { "red", 0 },
                    { "white", 0 },
                };
            }
            else if (appliedTo == "enemy" && s.enemy.enemyName.text != "Lich") {
                int refundedEnemyStamina = s.statSummoner.addedEnemyStamina.Values.Sum();
                s.statSummoner.addedEnemyStamina = new Dictionary<string, int> {
                    { "green", 0 },
                    { "blue", 0 },
                    { "red", 0 },
                    { "white", 0 },
                };
                ChangeStaminaOf("enemy", refundedEnemyStamina);
                s.statSummoner.RepositionAllDice("enemy");
                // same as player, except in the opposite direction
            }
            s.statSummoner.SummonStats();
            if (appliedTo == "player") {
                s.statSummoner.RepositionAllDice("player");
            }
        }
        else if (injury == "head") {
            if (appliedTo == "player") {
                if (!pendingEnemyHeadCounterDiscard) {
                    s.enemy.DiscardBestPlayerDie();
                }
            }
            else if (appliedTo == "enemy" && s.enemy.enemyName.text != "Lich") {
                Save.game.discardableDieCounter++;
            }
        }
        else if (injury == "hand") {
            // if hand, remove white die
            if (appliedTo == "player") {
                StartCoroutine(RemoveDice("white", "player"));
            }
            else if (appliedTo == "enemy" && s.enemy.enemyName.text != "Lich") {
                StartCoroutine(RemoveDice("white", "enemy"));
            }
        }
        else if (injury == "armpits") {
            // if armpits, remove red die
            if (appliedTo == "player") {
                StartCoroutine(RemoveDice("red", "player"));
            }
            else if (appliedTo == "enemy" && s.enemy.enemyName.text != "Lich") {
                StartCoroutine(RemoveDice("red", "enemy"));
            }
        }
        s.statSummoner.SetDebugInformationFor("player");
        s.statSummoner.SetDebugInformationFor("enemy");
        // update debug information
    }


    /// <summary>
    /// Coroutine to remove all of a dice type from the player or enemy.
    /// </summary>
    /// <param name="diceType">The type of die to remove.</param>
    /// <param name="removeFrom">Who to remove the die from.</param>
    private IEnumerator RemoveDice(string diceType, string removeFrom) {
        List<Dice> diceList = removeFrom == "player" ? s.statSummoner.addedPlayerDice[diceType] : s.statSummoner.addedEnemyDice[diceType];
        // assign the correct dicelist
        // conditional is true ? yes: no
        yield return s.delays[0.45f];
        foreach (Dice dice in diceList.ToList()) {
            // KEEP AS FOREACH
            // for every die in the stat we want to remove from
            if (dice.diceType == diceType) {
                // if the die type matches
                s.diceSummoner.existingDice.Remove(dice.gameObject);
                diceList.Remove(dice);
                Destroy(dice.gameObject);
                s.diceSummoner.SaveDiceValues();
                s.statSummoner.SetDebugInformationFor(removeFrom);
                // StartCoroutine(dice.FadeOut(false, true));
                // remove from arrays and destroy

            }
        }
        s.statSummoner.RepositionDice(removeFrom, diceType);
    }

    /// <summary>
    /// Make the enemy evaluate the current stats and add stamina to attack/defend etc.
    /// </summary>
    private void RunEnemyCalculations() {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
        #endif
        EnemyAI.ApplyLivePlan(s);
        Save.game.enemyStamina = s.enemy.stamina;
        Save.game.enemyTargetIndex = s.enemy.targetIndex;
        if (s.tutorial == null) { Save.SaveGame(); }
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        stopwatch.Stop();
        // Debug.Log($"enemy plan refresh total={stopwatch.Elapsed.TotalMilliseconds:F3}ms target={s.enemy.targetIndex} stamina={s.enemy.stamina}");
        #endif
    }

    public void RecalculateEnemyCombatIntent() {
        if (s == null || s.enemy == null || s.player == null || s.statSummoner == null) { return; }
        if (!s.itemManager.IsFightableEncounter() || isMoving) { return; }
        // only hard reveals the plan live; other difficulties apply it at attack time
        if (!ShouldRevealPlanDuringDraft()) { return; }

        BeginEnemyPlanRefreshBatch();
        try {
            RunEnemyCalculations();
            s.statSummoner.SummonStats();
            s.statSummoner.RepositionAllDice("player");
            s.statSummoner.RepositionAllDice("enemy");
            s.statSummoner.SetCombatDebugInformationFor("player");
            s.statSummoner.SetCombatDebugInformationFor("enemy");
            RecalculateMaxFor("player");
            RecalculateMaxFor("enemy");
        }
        finally {
            EndEnemyPlanRefreshBatch();
        }
    }

    /// <summary>
    /// queue a deferred enemy-plan refresh if the board is stable enough to do it
    /// </summary>
    public void RefreshEnemyPlanIfNeeded() {
        // only hard shows the live plan during the draft; other difficulties reveal at attack time
        if (!ShouldRevealPlanDuringDraft()) { return; }
        if (enemyPlanRefreshSuspendDepth > 0) {
            enemyPlanRefreshPendingWhileSuspended = true;
            return;
        }
        if (refreshingEnemyPlan || queuedEnemyPlanRefresh != null || s == null || s.enemy == null || s.player == null || s.statSummoner == null) { return; }
        if (!enemyPlanRefreshEnabled) { return; }
        if (isMoving || s.player.isDead || Save.game.enemyIsDead) { return; }
        if (s.enemy.enemyName.text == "Merchant" || s.enemy.enemyName.text == "Blacksmith" || s.enemy.enemyName.text == "Tombstone") { return; }

        queuedEnemyPlanRefresh = StartCoroutine(RefreshEnemyPlanDeferred());
    }

    /// <summary>
    /// suspend queued enemy replans until a grouped combat-state update finishes
    /// </summary>
    public void BeginEnemyPlanRefreshBatch() {
        enemyPlanRefreshSuspendDepth++;
    }

    /// <summary>
    /// resume queued enemy replans and flush one refresh if the batch changed combat state
    /// </summary>
    public void EndEnemyPlanRefreshBatch() {
        EndEnemyPlanRefreshBatch(true);
    }

    /// <summary>
    /// resume queued enemy replans and optionally flush one refresh if the batch changed combat state
    /// </summary>
    public void EndEnemyPlanRefreshBatch(bool flushRefresh) {
        if (enemyPlanRefreshSuspendDepth <= 0) { return; }

        enemyPlanRefreshSuspendDepth--;
        if (enemyPlanRefreshSuspendDepth > 0) { return; }
        if (!flushRefresh) {
            enemyPlanRefreshPendingWhileSuspended = false;
            return;
        }
        if (!enemyPlanRefreshPendingWhileSuspended) { return; }

        enemyPlanRefreshPendingWhileSuspended = false;
        RefreshEnemyPlanIfNeeded();
    }

    /// <summary>
    /// wait one frame so multiple board changes can collapse into one replan
    /// </summary>
    private IEnumerator RefreshEnemyPlanDeferred() {
        yield return null;
        queuedEnemyPlanRefresh = null;
        if (!ShouldRevealPlanDuringDraft()) { yield break; }
        if (refreshingEnemyPlan || s == null || s.enemy == null || s.player == null || s.statSummoner == null) { yield break; }
        if (!enemyPlanRefreshEnabled) { yield break; }
        if (isMoving || s.player.isDead || Save.game.enemyIsDead) { yield break; }
        if (s.enemy.enemyName.text == "Merchant" || s.enemy.enemyName.text == "Blacksmith" || s.enemy.enemyName.text == "Tombstone") { yield break; }

        refreshingEnemyPlan = true;
        try {
            RunEnemyCalculations();
        }
        finally {
            refreshingEnemyPlan = false;
        }
    }

    /// <summary>
    /// Make the enemy add stamina to green and blue to gain an advantage.
    /// </summary>
    private void AddSpeedAndAccuracy(int enemyAim, int playerSpd, int enemySpd, int playerAtt, int enemyDef) {
        if (playerSpd >= enemySpd && playerAtt > enemyDef) {
            // if player will attack first
            if (enemySpd + s.enemy.stamina > playerSpd) {
                // if enemy can attack first
                // add speed to go first
                UseEnemyStaminaOn("blue", (playerSpd - enemySpd) + 1);
            }
        }
        if (enemyAim < 7 && enemyAim + s.enemy.stamina > 6 && !s.itemManager.PlayerHas("armor")) {
            // if enemy can target neck, and the player doesnt have a set of armor
            UseEnemyStaminaOn("green", 7 - enemyAim);
            s.enemy.TargetBest();
            // add accuracy to target neck
        }
    }

    /// <summary>
    /// Make the enemy use stamina on a given stat.
    /// </summary>
    private void UseEnemyStaminaOn(string stat, int amount) {
        if (s.itemManager != null && s.itemManager.EnemyHasTemporaryHipInjury() && s.enemy.enemyName.text != "Lich") { return; }
        if (s.enemy.stamina >= amount) {
            s.statSummoner.addedEnemyStamina[stat] += amount;
            // incrase stat
            s.turnManager.ChangeStaminaOf("enemy", -amount);
            // decrease available
            s.statSummoner.SummonStats();
            // summon stats again
            foreach (Dice dice in s.statSummoner.addedEnemyDice[stat]) {
                // for every die in the stat
                dice.transform.position = new Vector2(dice.transform.position.x - s.statSummoner.xOffset * amount, dice.transform.position.y);
                // shift it over
                dice.instantiationPos = dice.transform.position;
                // set the instantiation position
            }
        }
    }

    /// <summary>
    /// Create variables for local scope based on the current stats.
    /// </summary>
    private void InitializeVariables(out int playerAim, out int enemyAim, out int playerSpd, out int enemySpd, out int playerAtt, out int enemyAtt, out int playerDef, out int enemyDef) {
        playerAim = s.statSummoner.SumOfStat("green", "player");
        enemyAim = s.statSummoner.SumOfStat("green", "enemy");
        playerSpd = s.statSummoner.SumOfStat("blue", "player");
        enemySpd = s.statSummoner.SumOfStat("blue", "enemy");
        playerAtt = s.statSummoner.SumOfStat("red", "player");
        enemyAtt = s.statSummoner.SumOfStat("red", "enemy");
        playerDef = s.statSummoner.SumOfStat("white", "player");
        enemyDef = s.statSummoner.SumOfStat("white", "enemy");
    }

    private bool PlayerActsFirstThisRound(int playerSpd, int enemySpd) {
        if (s != null && s.itemManager != null) {
            if (s.itemManager.PlayerAlwaysActsFirst()) { return true; }
            if (s.itemManager.PlayerAlwaysActsLast()) { return false; }
        }

        return playerSpd >= enemySpd;
    }

    private bool PlayerDraftsFirstThisRound(int playerSpd, int enemySpd) {
        if (s != null && s.itemManager != null) {
            if (s.itemManager.PlayerAlwaysChoosesFirstDraftDie()) { return true; }
            if (s.itemManager.PlayerAlwaysChoosesLastDraftDie()) { return false; }
        }

        return playerSpd >= enemySpd;
    }

    private void GrantPhylacteryLeech() {
        if (!s.player.SetPlayerStatusEffect("leech", true)) { return; }

        dontRemoveLeechYet = true;
    }

    private void ClearPlayerLeechEffect() {
        if (!Save.game.isBloodthirsty) { return; }

        s.player.SetPlayerStatusEffect("leech", false);
        dontRemoveLeechYet = false;
    }

    private string GetNormalizedTargetName(string targetName) {
        return string.IsNullOrEmpty(targetName) ? targetName : targetName.TrimStart('*');
    }

    private bool PlayerBleedsOutThisRound() {
        return playerBleedOutPendingThisRound && Save.game.playerBleedsOutNextRound && !s.player.isDead;
    }

    private bool EnemyBleedsOutThisRound() {
        return enemyBleedOutPendingThisRound && Save.game.enemyBleedsOutNextRound && !Save.game.enemyIsDead;
    }

    private string GetPlayerBleedOutStatusText() {
        return $"{s.enemy.enemyName.text.ToLower()} hits you... {(s.tombstoneData.HasUsableResurrectionAmulet() ? "your amulet shatters" : "you bleed out")}";
    }

    private string GetEnemyBleedOutStatusText() {
        return $"you hit {s.enemy.enemyName.text.ToLower()}... he bleeds out";
    }

    private string GetPlayerKilledStatusText() {
        bool shattersAmulet = s.tombstoneData.HasUsableResurrectionAmulet();
        if (s.enemy.spawnNum is 0 or 1) {
            return shattersAmulet
                ? "devil twists claws into you... your amulet shatters"
                : "devil twists claws into you... you die";
        }

        return shattersAmulet
            ? $"{s.enemy.enemyName.text.ToLower()} hits you... your amulet shatters"
            : $"{s.enemy.enemyName.text.ToLower()} hits you... you die";
    }

    private string GetEnemyKilledStatusText() {
        return $"you hit {s.enemy.enemyName.text.ToLower()}... he dies";
    }

    private void ClearBleedOutForRecoveredWound(string playerOrEnemy, string woundName) {
        if (woundName != "neck") { return; }

        SetBleedOutNextRound(playerOrEnemy, false, saveGame:false);
        if (playerOrEnemy == "player") {
            playerBleedOutPendingThisRound = false;
        }
        else if (playerOrEnemy == "enemy") {
            enemyBleedOutPendingThisRound = false;
        }
    }

    private void ResolveBleedOutAfterRound() {
        bool enemyBleedsOutNow = enemyBleedOutPendingThisRound && Save.game.enemyBleedsOutNextRound && !Save.game.enemyIsDead;
        bool playerBleedsOutNow = playerBleedOutPendingThisRound && Save.game.playerBleedsOutNextRound && !s.player.isDead;

        enemyBleedOutPendingThisRound = false;
        playerBleedOutPendingThisRound = false;

        if (enemyBleedsOutNow) {
            StartCoroutine(Kill("enemy", true, false));
        }
        if (playerBleedsOutNow) {
            StartCoroutine(Kill("player", true, false));
        }
    }

    private IEnumerator HandleEnemyChestRerollsBeforePlayerAttack() {
        if (!PlayerWouldDamageEnemyNow()) { yield break; }
        if (!EnemyHasChestRescueReroll()) { yield break; }

        // if (PlayerPrefs.GetString(s.HINTS_KEY) == "on") {
        //     SetStatusText($"{s.enemy.enemyName.text.ToLower()} twists the wound... rerolling");
        //     yield return s.delays[0.55f];
        // }

        bool rerolledAtLeastOneDie = false;
        while (PlayerWouldDamageEnemyNow() && TryGetBestEnemyChestRescueDie(out Dice rescueDie)) {
            rerolledAtLeastOneDie = true;
            rescueDie.isRerolled = true;
            yield return StartCoroutine(rescueDie.RerollAnimation(!rerolledAtLeastOneDie));
        }

        if (rerolledAtLeastOneDie && PlayerPrefs.GetString(s.HINTS_KEY) == "on") {
            yield return s.delays[0.35f];
        }
    }

    private IEnumerator HandleEnemyCounterattackRescuesBeforePlayerAttack() {
        if (pendingEnemyHeadCounterDiscard) {
            pendingEnemyHeadCounterDiscard = false;

            // if (PlayerPrefs.GetString(s.HINTS_KEY) == "on") {
            //     SetStatusText($"{s.enemy.enemyName.text.ToLower()} crushes your focus... discarding");
            //     yield return s.delays[0.55f];
            // }

            s.enemy.DiscardBestPlayerDie();
            yield return s.delays[0.35f];
        }

        if (s.player.woundList.Contains("chest")) {
            yield return StartCoroutine(HandleEnemyChestRerollsBeforePlayerAttack());
        }
    }

    private bool EnemyHasChestRescueReroll() {
        return TryGetBestEnemyChestRescueDie(out _);
    }

    private bool TryGetBestEnemyChestRescueDie(out Dice bestDie) {
        bestDie = null;
        int bestSafeOutcomes = 0;
        int bestKillBreakingOutcomes = 0;
        bool playerKillsNow = PlayerWouldKillEnemyNow();

        foreach (string key in s.statSummoner.addedPlayerDice.Keys) {
            foreach (Dice dice in s.statSummoner.addedPlayerDice[key]) {
                if (dice == null || !dice.isAttached || dice.isOnPlayerOrEnemy != "player" || dice.isRerolled || dice.diceNum < 3) { continue; }

                int safeOutcomes = 0;
                int killBreakingOutcomes = 0;
                for (int rerolledValue = 1; rerolledValue <= 6; rerolledValue++) {
                    if (!PlayerWouldDamageEnemyWithChestRescueBestCase(dice, rerolledValue)) {
                        safeOutcomes++;
                    }
                    if (playerKillsNow && !PlayerWouldKillEnemyWithChestRescueBestCase(dice, rerolledValue)) {
                        killBreakingOutcomes++;
                    }
                }

                if (safeOutcomes <= 0 && killBreakingOutcomes <= 0) { continue; }
                if (bestDie == null
                    || safeOutcomes > bestSafeOutcomes
                    || safeOutcomes == bestSafeOutcomes && killBreakingOutcomes > bestKillBreakingOutcomes
                    || safeOutcomes == bestSafeOutcomes && killBreakingOutcomes == bestKillBreakingOutcomes && GetChestRescueRerollPriority(dice) > GetChestRescueRerollPriority(bestDie)
                    || safeOutcomes == bestSafeOutcomes && killBreakingOutcomes == bestKillBreakingOutcomes && GetChestRescueRerollPriority(dice) == GetChestRescueRerollPriority(bestDie) && dice.diceNum > bestDie.diceNum) {
                    bestDie = dice;
                    bestSafeOutcomes = safeOutcomes;
                    bestKillBreakingOutcomes = killBreakingOutcomes;
                }
            }
        }

        return bestDie != null;
    }

    private int GetChestRescueRerollPriority(Dice dice) {
        if (dice == null) { return -1; }

        string stat = string.IsNullOrEmpty(dice.statAddedTo)
            ? dice.diceType
            : dice.statAddedTo;

        return stat switch {
            "red" => 3,
            "green" => 2,
            "yellow" => 1,
            _ => 0,
        };
    }

    private bool PlayerWouldDamageEnemyNow() {
        return PlayerWouldDamageEnemyWithEnemyStatOverride(null, 0);
    }

    private bool PlayerWouldKillEnemyNow() {
        return PlayerWouldKillEnemyWithEnemyStatOverride(null, 0);
    }

    private bool PlayerWouldDamageEnemyWithRerolledDieValue(Dice dice, int rerolledValue) {
        return PlayerWouldDamageEnemyWithEnemyStatOverride(dice, rerolledValue);
    }

    private bool PlayerWouldKillEnemyWithRerolledDieValue(Dice dice, int rerolledValue) {
        return PlayerWouldKillEnemyWithEnemyStatOverride(dice, rerolledValue);
    }

    private bool PlayerWouldDamageEnemyWithChestRescueBestCase(Dice overriddenDie, int overriddenValue) {
        return PlayerWouldDamageEnemyWithEnemyStatOverride(overriddenDie, overriddenValue, applyBestCaseToOtherChestRerolls:true);
    }

    private bool PlayerWouldKillEnemyWithChestRescueBestCase(Dice overriddenDie, int overriddenValue) {
        return PlayerWouldKillEnemyWithEnemyStatOverride(overriddenDie, overriddenValue, applyBestCaseToOtherChestRerolls:true);
    }

    private bool PlayerWouldDamageEnemyWithEnemyStatOverride(Dice overriddenDie, int overriddenValue) {
        return PlayerWouldDamageEnemyWithEnemyStatOverride(overriddenDie, overriddenValue, applyBestCaseToOtherChestRerolls:false);
    }

    private bool PlayerWouldDamageEnemyWithEnemyStatOverride(Dice overriddenDie, int overriddenValue, bool applyBestCaseToOtherChestRerolls) {
        GetPlayerAttackPreviewAgainstEnemyOverride(overriddenDie, overriddenValue, out int playerAim, out int playerAtt, out int enemyDef, out string playerTarget, applyBestCaseToOtherChestRerolls);
        bool playerCanHit = PlayerCanHitEnemy(playerAim, playerAtt, enemyDef);
        if (!playerCanHit) { return false; }
        if (s.itemManager.PlayerHasWeapon("maul")) { return true; }
        return playerTarget == "neck" || !s.enemy.woundList.Contains(playerTarget);
    }

    // preview shorthand: this treats neck as a fatal line because the bleed-out is guaranteed
    private bool PlayerWouldKillEnemyWithEnemyStatOverride(Dice overriddenDie, int overriddenValue) {
        return PlayerWouldKillEnemyWithEnemyStatOverride(overriddenDie, overriddenValue, applyBestCaseToOtherChestRerolls:false);
    }

    private bool PlayerWouldKillEnemyWithEnemyStatOverride(Dice overriddenDie, int overriddenValue, bool applyBestCaseToOtherChestRerolls) {
        GetPlayerAttackPreviewAgainstEnemyOverride(overriddenDie, overriddenValue, out int playerAim, out int playerAtt, out int enemyDef, out string playerTarget, applyBestCaseToOtherChestRerolls);
        bool playerCanHit = PlayerCanHitEnemy(playerAim, playerAtt, enemyDef);
        if (!playerCanHit) { return false; }
        if (s.itemManager.PlayerHasWeapon("maul")) { return true; }
        return !s.enemy.woundList.Contains(playerTarget)
            && (s.enemy.woundList.Count >= 2 || s.itemManager.PlayerHasWeapon("glaive") && s.enemy.woundList.Count >= 1);
    }

    private bool PlayerCanHitEnemy(int playerAim, int playerAtt, int enemyDef) {
        return playerAim >= 0 && PlayerAttackClearsEnemyDefense(playerAtt, enemyDef);
    }

    private bool PlayerAttackClearsEnemyDefense(int playerAtt, int enemyDef) {
        if (s.itemManager.PlayerHasWeapon("crossbow")) { return playerAtt > 0; }
        return playerAtt > enemyDef;
    }

    private bool PlayerAttackIsTooWeak(int playerAtt) {
        return s.itemManager.PlayerHasWeapon("crossbow") ? playerAtt <= 0 : playerAtt < 0;
    }

    private void GetPlayerAttackPreviewAgainstEnemyOverride(Dice overriddenDie, int overriddenValue, out int playerAim, out int playerAtt, out int enemyDef, out string playerTarget, bool applyBestCaseToOtherChestRerolls = false) {
        playerAim = s.statSummoner.SumOfStat("green", "player");
        playerAtt = s.statSummoner.SumOfStat("red", "player");
        enemyDef = s.statSummoner.SumOfStat("white", "enemy");
        playerTarget = targetArr[Mathf.Clamp(s.player.targetIndex, 0, targetArr.Length - 1)];

        if (applyBestCaseToOtherChestRerolls) {
            foreach (string key in s.statSummoner.addedPlayerDice.Keys) {
                foreach (Dice dice in s.statSummoner.addedPlayerDice[key]) {
                    if (dice == null || !dice.isAttached || dice.isOnPlayerOrEnemy != "player" || dice.isRerolled || dice.diceNum < 3 || dice == overriddenDie) {
                        continue;
                    }

                    ApplyPlayerAttackPreviewDieDelta(ref playerAim, ref playerAtt, ref enemyDef, dice, 1);
                }
            }
        }

        if (overriddenDie == null) { return; }

        ApplyPlayerAttackPreviewDieDelta(ref playerAim, ref playerAtt, ref enemyDef, overriddenDie, overriddenValue);
    }

    private void ApplyPlayerAttackPreviewDieDelta(ref int playerAim, ref int playerAtt, ref int enemyDef, Dice die, int previewValue) {
        if (die == null) { return; }

        string overriddenStat = string.IsNullOrEmpty(die.statAddedTo)
            ? die.diceType
            : die.statAddedTo;
        int delta = previewValue - die.diceNum;

        if (die.isOnPlayerOrEnemy == "player") {
            if (overriddenStat == "green") {
                playerAim += delta;
            }
            else if (overriddenStat == "red") {
                playerAtt += delta;
            }
        }
        else if (overriddenStat == "white") {
            enemyDef += delta;
        }
    }

    /// <summary>
    /// Checks if there is a die on the enemy that can be rerolled and is >= 3.
    /// </summary>
    /// <returns>true if there are die to be rerolled, false if not.</returns>
    private bool Rerollable() {
        foreach (string key in s.statSummoner.addedEnemyDice.Keys) {
            // for every key
            foreach (Dice dice in s.statSummoner.addedEnemyDice[key]) {
                // for every die
                if (!dice.isRerolled && dice.diceNum >= 3) {
                    // if the die is not rerolled and the number is >=3 
                    return true;
                    // return true
                }
            }
        }
        return false;
        // none found, so return false
    }
}
