using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Dice : MonoBehaviour {
    public int diceNum;
    public string diceType;
    public string statAddedTo;
    public bool moveable = true;
    public bool isAttached = false;
    public bool isRerolled = false;
    public bool isRolling = false;
    public bool tarotUpgradeApplied = false;
    public bool spawnedByCursedDice = false;
    public string isOnPlayerOrEnemy = "none";
    public Vector3 instantiationPos;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer childSpriteRenderer;
    private Scripts s;
    private bool wasClickedRecently = false;
    private bool suppressPointerReleaseAfterInstantAction = false;
    private bool suspendedEnemyPlanRefreshForDrag = false;
    private bool dragStartedAttachedToPlayer = false;
    private string dragStartedPlayerStat = "";

    private readonly WaitForSeconds[] rollTimes = { new(0.01f), new(0.03f), new(0.06f), new(0.09f), new(0.12f), new(0.15f), new(0.18f), new(0.21f), new(0.24f), new(0.3f) };
    // different times for rolling 

    private void Awake()  {
        // must be in awake, otherwise s not set fast enough
        s = FindFirstObjectByType<Scripts>();
        // assign the necessary sprite renderers
    }

    private void Start() {
        StartCoroutine(FadeIn());
    }

    private void EnsureRenderersAssigned() {
        spriteRenderer ??= GetComponent<SpriteRenderer>();
        childSpriteRenderer ??= transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private bool TryResolvePendingMirrorCopy() {
        if (!Save.game.pendingMirrorCopy || s.turnManager.isMoving || !s.itemManager.IsFightableEncounter()) { return false; }

        suppressPointerReleaseAfterInstantAction = true;
        Save.game.pendingMirrorCopy = false;
        s.diceSummoner.DuplicateDieToPlayer(diceNum, diceType);
        s.statSummoner.SummonStats();
        s.statSummoner.SetCombatDebugInformationFor("player");
        s.turnManager.RecalculateMaxFor("player");
        s.diceSummoner.SaveDiceValues();
        return true;
    }

    private bool TryResolvePendingSpellbookTransmute() {
        if (!Save.game.pendingSpellbookTransmute || s.turnManager.isMoving) {
            return false;
        }

        if (ShouldPrioritizeEnemyDieActionOverSpellbook()) {
            return false;
        }

        suppressPointerReleaseAfterInstantAction = true;
        Save.game.pendingSpellbookTransmute = false;
        s.itemManager.TransmuteDie(this);
        return true;
    }

    private bool TryResolvePendingGemTransform() {
        if (string.IsNullOrWhiteSpace(Save.game.pendingGemTransformColor) || s.turnManager.isMoving) {
            return false;
        }

        if (!isAttached || isOnPlayerOrEnemy != "player") {
            return false;
        }

        if (diceType == Save.game.pendingGemTransformColor) {
            return true;
        }

        suppressPointerReleaseAfterInstantAction = true;
        string transformColor = Save.game.pendingGemTransformColor;
        Save.game.pendingGemTransformColor = "";
        s.itemManager.TransformAttachedPlayerDieToColor(this, transformColor);
        return true;
    }

    private bool ShouldPrioritizeEnemyDieActionOverSpellbook() {
        if (!isAttached || isOnPlayerOrEnemy != "enemy" || s.enemy.enemyName.text == "Lich") { return false; }

        bool isEnemyHeadWounded = s.enemy.woundList.Contains("head");
        bool isEnemyChestWounded = s.enemy.woundList.Contains("chest") || s.itemManager.EnemyHasTemporaryChestInjury();

        if (isEnemyHeadWounded && Save.game.discardableDieCounter > 0) { return true; }
        if (isEnemyChestWounded && !isRerolled) { return true; }

        return false;
    }

    private void OnMouseDown() {
        if (TryResolvePendingMirrorCopy() || TryResolvePendingSpellbookTransmute() || TryResolvePendingGemTransform()) { return; }
        // as soon as the mouse button is pressed down
        if (s.tutorial != null) { 
            // if within the tutorial, make sure player can only do certain actions (so that they win)
            if (s.tutorial.isAnimating || s.tutorial.curIndex is 12 or 13 or 22)  {
                if (s.diceSummoner.CountUnattachedDice() == 6 && diceType == "red") { DiceDown(); }
                // only allow the red 6 to be picked
                else if (s.diceSummoner.CountUnattachedDice() == 4 && diceType == "green") { DiceDown(); }
                // then take the green
                else if (s.diceSummoner.CountUnattachedDice() == 2) { DiceDown(); }
                // after that it doesnt matter
                else if (s.tutorial.curIndex == 22) { DiceDown(); }
                else { s.turnManager.SetStatusText("bad choice"); }
            }
        }
        else { DiceDown(); }
        // else just regular dice down
    }


    /// <summary>
    /// Handle what happens when the player presses down on a dice.
    /// </summary>
    private void DiceDown() { 
        if (moveable && !s.turnManager.isMoving && !s.turnManager.draftInputLocked) {
            // if the dice is still moveable
            dragStartedAttachedToPlayer = isAttached && isOnPlayerOrEnemy == "player";
            dragStartedPlayerStat = dragStartedAttachedToPlayer ? statAddedTo : "";
            if (!suspendedEnemyPlanRefreshForDrag) {
                s.turnManager.BeginEnemyPlanRefreshBatch();
                suspendedEnemyPlanRefreshForDrag = true;
            }
            s.soundManager.PlayClip("click0");
            // play sound clip
            childSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            // assign the child sprite renderer to be edited 
            s.highlightCalculator.ShowValidHighlights(gameObject.GetComponent<Dice>());
            // call the from class HighlightCalculator to show all valid highlights 
        }
        // TODO: LOOKS BETTER BUT CAUSES GLITCHES, FIX IN THE FUTURE
        
        // if (!moveable && isAttached && !isRerolled && isOnPlayerOrEnemy == "enemy") {
        //     // if an action can be performed on the dice (discard, reroll)
        //     if (!s.turnManager.isMoving || (s.turnManager.isMoving && s.turnManager.actionsAvailable)) {
        //         // if the situation permits action to occur on the die
        //         if (s.itemManager.discardableDieCounter > 0) {
        //             // if the enemy is wounded in the head and a die has not been discarded yet
        //             s.soundManager.PlayClip("click0");
        //             // play sound clip
        //             Color numTemp = spriteRenderer.color;
        //             Color baseTemp = childSpriteRenderer.color;
        //             numTemp.a -= 0.33f;
        //             spriteRenderer.color = numTemp;
        //             baseTemp.a -= 0.25f;
        //             childSpriteRenderer.color = baseTemp;
        //             // dim the colors of the die
        //         }
        //     }
        // }
        // if (isAttached && isOnPlayerOrEnemy == "player" && s.player.isCourageous && !s.turnManager.isMoving) {
        //     // if the player wants to Save a die via scroll of courage by discarding the others
        //     s.soundManager.PlayClip("click0");
        //     // play sound clip
        //     Color numTemp = spriteRenderer.color;
        //     Color baseTemp = childSpriteRenderer.color;
        //     numTemp.a -= 0.33f;
        //     spriteRenderer.color = numTemp;
        //     baseTemp.a -= 0.25f;
        //     childSpriteRenderer.color = baseTemp;
        //     // dim the colors of the die
        // }
    }
    
    private void OnMouseUp() {
        if (suppressPointerReleaseAfterInstantAction) {
            suppressPointerReleaseAfterInstantAction = false;
            return;
        }

        // self explanatory, tutorial restricts which dice can be picked, else is just normal
        if (s.tutorial != null) { 
            if (s.tutorial.isAnimating || s.tutorial.curIndex == 12 || s.tutorial.curIndex == 13 || s.tutorial.curIndex == 22) {
                if (s.diceSummoner.CountUnattachedDice() == 6 && diceType == "red") { DiceUp(); }
                else if (s.diceSummoner.CountUnattachedDice() == 4 && diceType == "green") { DiceUp(); }
                else if (s.diceSummoner.CountUnattachedDice() == 2) { DiceUp(); }
                else if (s.tutorial.curIndex == 22) { DiceUp(); }
                else { s.turnManager.SetStatusText("bad choice"); }
            }
        }
        else { DiceUp(); }
    }

    private void OnMouseDrag() {
        if (suppressPointerReleaseAfterInstantAction) { return; }

        if (s.tutorial != null) { 
            if (s.tutorial.isAnimating || s.tutorial.curIndex == 12 || s.tutorial.curIndex == 13 || s.tutorial.curIndex == 22) {
                if (s.diceSummoner.CountUnattachedDice() == 6 && diceType == "red") { DiceDrag(); }
                else if (s.diceSummoner.CountUnattachedDice() == 4 && diceType == "green") { DiceDrag(); }
                else if (s.diceSummoner.CountUnattachedDice() == 2) { DiceDrag(); }
                else if (s.tutorial.curIndex == 22) { DiceDrag(); }
                else { s.turnManager.SetStatusText("bad choice"); }
            }
        }
        else { DiceDrag(); }
    }

    /// <summary>
    /// Handle what happens when a dice is attempted to be dragged.
    /// </summary>
    private void DiceDrag() { 
        // when the mouse is dragged
        if (moveable && !s.turnManager.isMoving && !s.turnManager.draftInputLocked) {
            // if the dice can be moved
            spriteRenderer.sortingOrder = 3;
            childSpriteRenderer.sortingOrder = 2;
            // move the dice and its number to the front of the screen
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // get the mouse position via a function
            transform.position = new Vector2(mousePos.x, mousePos.y);
            // assign the transform position of the dice to be where the mouse cursor is
            // no need ot move child as the positions are tied together
        }
    }

    /// <summary>
    /// Handle what happens when the player releases a dice.
    /// </summary>
    private void DiceUp() {
        // when the mouse is released
        if (moveable && !s.turnManager.isMoving && !s.turnManager.draftInputLocked) {
            // if the dice can be moved
            s.soundManager.PlayClip("click1");
            // play sound clip
            if (wasClickedRecently) {
                // was clicked recently, so select the dice
                float xOverride;
                float yOverride;
                GameObject[] highlights = s.highlightCalculator.highlights;
                // 0: accuracy, 1: speed, 2: damage, 3: parry
                if (diceType == "yellow" || 
                    diceType == "green" && s.itemManager.PlayerHasWeapon("dagger") || diceType == "white" && Save.game.curCharNum == 3) { 
                    // yellow dice drop onto red by default
                    // green die + dagger drop onto red
                    // white die + char 3 do as well
                    xOverride = highlights[2].transform.position.x;
                    yOverride = highlights[2].transform.position.y;
                }
                else {
                    // regular die, so drop it to the regular position
                    int diceIndex = Array.IndexOf(Colors.colorNameArr, diceType);
                    xOverride = highlights[diceIndex].transform.position.x;
                    yOverride = highlights[diceIndex].transform.position.y;
                }
                s.highlightCalculator.SnapToPosition(gameObject.GetComponent<Dice>(), instantiationPos, out moveable, out instantiationPos, xOverride, yOverride);
            }
            else { 
                // dice was not clicked recently, so start the timer
                StartCoroutine(ClickedTimer());
                s.highlightCalculator.SnapToPosition(gameObject.GetComponent<Dice>(), instantiationPos, out moveable, out instantiationPos);
            }
            // attempt to snap the position with a function defined in HighlightCalculator
            transform.position = instantiationPos;
            // set the transform position to be where the instantiation position is (snap back to the selection menu if it didn't get snapped in SnapToPosition)
            s.highlightCalculator.HideHighlights();
            // hide all the highlights
            spriteRenderer.sortingOrder = 1;
            childSpriteRenderer.sortingOrder = 0;
            // send the die to the background
        }
        if (suspendedEnemyPlanRefreshForDrag) {
            bool isAttachedToPlayerAfterDrop = isAttached && isOnPlayerOrEnemy == "player";
            bool placedNewPlayerDie = isAttachedToPlayerAfterDrop && !dragStartedAttachedToPlayer;
            bool changedPlayerStat = isAttachedToPlayerAfterDrop
                && dragStartedAttachedToPlayer
                && dragStartedPlayerStat != statAddedTo;
            bool shouldRefreshAfterDrop = placedNewPlayerDie || changedPlayerStat;
            s.turnManager.EndEnemyPlanRefreshBatch(shouldRefreshAfterDrop);
            suspendedEnemyPlanRefreshForDrag = false;
        }
        if (!moveable && isAttached && !isRerolled && isOnPlayerOrEnemy == "enemy" && s.enemy.enemyName.text != "Lich") {
            //  && !s.turnManager.isMoving
            // if an action can be performed on the dice (discard, reroll)
            if (!s.turnManager.isMoving || s.turnManager.isMoving && s.turnManager.actionsAvailable) {
                // if the situation allows for an action to be performed
                if (Save.game.discardableDieCounter > 0 || s.turnManager.scimitarParryCount > 0) {
                    // if can discard from another source
                    if (s.turnManager.scimitarParryCount > 1) {
                        s.turnManager.scimitarParryCount = 1;
                        // decrease the scimitarparry count
                        if (s.tutorial == null) { Save.SaveGame(); }
                    }
                    else {
                        // <= 1 scimitar die parry
                        s.turnManager.scimitarParryCount = 0;
                        s.diceSummoner.breakOutOfScimitarParryLoop = true;
                        // set the # of parries to 0 and break out of the loop
                    }
                    DiscardFromEnemy();
                    // discard from the enemy
                    Save.game.discardableDieCounter = Mathf.Max(0, Save.game.discardableDieCounter - 1);
                    // decrease the counter for the number of die able to be discarded
                    // if source is from scimitarParry, break out of the waiting loop
                }
                else if (s.enemy.woundList.Contains("chest") || s.itemManager.EnemyHasTemporaryChestInjury() || s.itemManager.PlayerHasWeapon("mace") && s.itemManager.PlayerHasLegendary()) {
                    // if enemy is wounded in the chest or player has legendary mace
                    Reroll();
                    // reroll the die
                }
            }
        }
        if (isAttached && isOnPlayerOrEnemy == "player" && Save.game.isCourageous && s.turnManager.discardDieBecauseCourage) {
            // if discarding can and should discard die from courage
            DiscardFromPlayer();
            // do so 
        }
    }

    private IEnumerator ClickedTimer() { 
        if (!wasClickedRecently) { 
            wasClickedRecently = true;
            yield return s.delays[0.3f];
            wasClickedRecently = false;
        }
    }

    /// <summary>
    /// Discard this dice from the enemy.
    /// </summary>
    private void DiscardFromEnemy() {
        if (!isAttached || isOnPlayerOrEnemy != "enemy") { return; }

        s.soundManager.PlayClip("click1");
        // play sound clip
        s.turnManager.alterationDuringMove = true;
        // set necessary variables for the turnmanager
        s.statSummoner.addedEnemyDice[statAddedTo].Remove(this);
        s.diceSummoner.existingDice.Remove(gameObject);
        // remove the die from the lists
        Destroy(gameObject);
        // destroy the gameObject
        s.statSummoner.RepositionDice("enemy", statAddedTo);
        s.statSummoner.SetCombatDebugInformationFor("enemy");
        // set the debug information
        Save.persistent.diceDiscarded++;
        Save.SavePersistent();
        // increment stats and Save them
    }

    /// <summary>
    /// Discard this die from the player.
    /// </summary>
    public void DiscardFromPlayer() {
        // very similar to discardfromenemy, just doesn't set certain variables in TurnManager and such
        s.statSummoner.addedPlayerDice[statAddedTo].Remove(this);
        s.diceSummoner.existingDice.Remove(gameObject);
        Destroy(gameObject);
        s.statSummoner.RepositionDice("player", statAddedTo);
        s.statSummoner.SetCombatDebugInformationFor("player");
        s.turnManager.RecalculateMaxFor("player");
        s.diceSummoner.SaveDiceValues();
    }

    /// <summary>
    /// (Player only) Reroll an enemy's dice.
    /// </summary>
    private void Reroll() {
        if (!isAttached || isOnPlayerOrEnemy != "enemy") { return; }

        // pretty self explanatory self explanatory
        Save.persistent.diceRerolled++;
        s.turnManager.alterationDuringMove = true;
        StartCoroutine(RerollAnimation());
        isRerolled = true;
        s.diceSummoner.SaveDiceValues();
        Save.SavePersistent();
    }

    /// <summary>
    /// Coroutine for playing the animation and rerolling the dice.
    /// </summary>
    public IEnumerator RerollAnimation(bool playSound=true) {
        if (isRolling) { yield break; }

        isRolling = true;
        isRerolled = true;
        // assign the spriterenderer reference
        for (int i = 0; i < 10; i++) {
            // 10 times
            yield return rollTimes[i];
            // wait for a set amount of time
            if (playSound) { s.soundManager.PlayClip("click0"); }
            // play sound clip if necessary
            int randNum = UnityEngine.Random.Range(1, 7);
            // get a random number for the dice 
            spriteRenderer.sprite = s.diceSummoner.numArr[randNum - 1].GetComponent<SpriteRenderer>().sprite;
            // assign the sprite to be the necessary sprite with the new number
            diceNum = randNum;
            // reassign the die's number
        }
        s.statSummoner.SetDebugInformationFor("player");
        s.statSummoner.SetDebugInformationFor("enemy");
        s.turnManager.RefreshEnemyPlanIfNeeded();
        s.turnManager.RecalculateMaxFor("player");
        s.turnManager.RecalculateMaxFor("enemy");
        // set debug information and make sure that the player/enemy isn't aiming at something that they shouldn't be able to hit
        s.diceSummoner.SaveDiceValues();
        isRolling = false;
    }

    /// <summary>
    /// Coroutine for decreasing the value of this die.
    /// </summary>
    public IEnumerator DecreaseDiceValue(bool wait = true) {
        if (wait) { yield return s.delays[1f]; }
        // wait if necessary
        if (diceNum == 1) { StartCoroutine(FadeOut()); }
        // fade it out if decreasing dice value to 0
        else {
            diceNum--;
            GetComponent<SpriteRenderer>().sprite = s.diceSummoner.numArr[diceNum - 1].GetComponent<SpriteRenderer>().sprite;
            // otherwise decrement value and set the proper sprite
        }
        s.statSummoner.SetDebugInformationFor("player");
        s.statSummoner.SetDebugInformationFor("enemy");
        s.turnManager.RefreshEnemyPlanIfNeeded();
        // set the debug information
        s.diceSummoner.SaveDiceValues();
    }

    /// <summary>
    /// Coroutine for increasing the value of this die.
    /// </summary>
    public IEnumerator IncreaseDiceValue(bool wait = true) {
        if (wait) { yield return s.delays[1f]; }
        if (diceNum >= 6) { yield break; }

        diceNum++;
        GetComponent<SpriteRenderer>().sprite = s.diceSummoner.numArr[diceNum - 1].GetComponent<SpriteRenderer>().sprite;
        s.statSummoner.SetDebugInformationFor("player");
        s.statSummoner.SetDebugInformationFor("enemy");
        s.turnManager.RefreshEnemyPlanIfNeeded();
        s.diceSummoner.SaveDiceValues();
    }

    public void SetDiceValue(int newValue) {
        EnsureRenderersAssigned();
        diceNum = Mathf.Clamp(newValue, 1, 6);
        spriteRenderer.sprite = s.diceSummoner.numArr[diceNum - 1].GetComponent<SpriteRenderer>().sprite;
    }

    public void SetDieType(string newType) {
        EnsureRenderersAssigned();
        diceType = newType;
        spriteRenderer.color = newType is "white" or "yellow" ? Color.black : Color.white;
        childSpriteRenderer.color = newType switch {
            "green" => Colors.green,
            "blue" => Colors.blue,
            "red" => Colors.red,
            "white" => Colors.white,
            "yellow" => Colors.yellow,
            _ => childSpriteRenderer.color,
        };
    }

    /// <summary>
    /// Sets this die's value to one.
    /// </summary>
    public void SetToOne() {
        // pretty self explanatory
        diceNum = 1;
        spriteRenderer.sprite = s.diceSummoner.numArr[0].GetComponent<SpriteRenderer>().sprite;
        s.statSummoner.SetCombatDebugInformationFor("player");
        // this can only happen to player, so don't worry about enemies stuff
        s.diceSummoner.SaveDiceValues();
    }
    
    /// <summary>
    /// Coroutine for fading out this die.
    /// </summary>
    public IEnumerator FadeOut(bool wait=false, bool shiftOver = true) {
        if (wait) { yield return s.delays[0.55f]; }
        // wait if necessary
        Color numTemp = spriteRenderer.color;
        Color baseTemp = childSpriteRenderer.color;
        numTemp.a = 1;
        baseTemp.a = 1;
        // set them to 1 here because for some reason sometimes alpha starts at 2 and nothing works right
        // assign the necessary variables to manipulate the color
        for (int i = 0; i < 12; i++) {
            // 40 times
            yield return s.delays[0.005f];
            // wait a small duration
            numTemp.a -= 1/12f;
            spriteRenderer.color = numTemp;
            baseTemp.a -= 1/12f;
            childSpriteRenderer.color = baseTemp;
            // decrease the colors of the die and base
        }
        string removedStat = statAddedTo;
        string removedSide = isOnPlayerOrEnemy;
        try { s.statSummoner.addedPlayerDice[statAddedTo].Remove(this); } catch { }
        try { s.statSummoner.addedEnemyDice[statAddedTo].Remove(this); } catch { }
        // attempt to remove from the player/enemy, checking with if statements causes a plethora of bugs for no reason
        if (removedStat != "" && shiftOver && (removedSide == "player" || removedSide == "enemy")) {
            s.statSummoner.RepositionDice(removedSide, removedStat);
        }
        s.diceSummoner.existingDice.Remove(gameObject);
        // remove from existing die list so no errors later on
        Destroy(gameObject);
        // destroy the die
        s.diceSummoner.SaveDiceValues();
        // Save the dice values to the Save file
        if (isOnPlayerOrEnemy != "none") {
            s.turnManager.RecalculateMaxFor(isOnPlayerOrEnemy);
        }
        s.statSummoner.SetDebugInformationFor("enemy");
        s.statSummoner.SetDebugInformationFor("player");
        s.turnManager.RefreshEnemyPlanIfNeeded();
    }
    
    /// <summary>
    /// Coroutine for fading in a die.
    /// </summary>
    private IEnumerator FadeIn() {
        // very similar to fadeout
        spriteRenderer = GetComponent<SpriteRenderer>();
        childSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        Color numTemp = spriteRenderer.color;
        Color baseTemp = childSpriteRenderer.color;
        numTemp.a = 0;
        baseTemp.a = 0;
        spriteRenderer.color = numTemp;
        childSpriteRenderer.color = baseTemp;
        yield return s.delays[0.005f];
        for (int i = 0; i < 40; i++) {
            numTemp.a += 0.025f;
            spriteRenderer.color = numTemp;
            baseTemp.a += 0.025f;
            childSpriteRenderer.color = baseTemp;
            yield return s.delays[0.005f];
        }
    }
}