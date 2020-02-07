using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class TurnManager : MonoBehaviour {
    [SerializeField] public GameObject blackBox;
    public Vector3 onScreen = new Vector2(0.33f, 10f);
    public Vector3 offScreen = new Vector2(0.33f, 20f);
    private Coroutine coroutine = null;
    [SerializeField] public TextMeshProUGUI statusText;
    [SerializeField] public string[] targetArr = { "chest", "guts", "knee", "hip", "head", "hand", "armpits", "face" };
    [SerializeField] public string[] targetInfoArr = { "reroll any number of enemy's dice", "all enemy's dice suffer a penalty of -1", "your speed is always higher than enemy's", "enemy can't use stamina", "discard one of enemy's die", "enemy can't use white dice", "enemy can't use red dice", "instantaneous death" };
    private Scripts scripts;
    public string toMove;
    public bool isMoving = false;
    public bool actionsAvailable = false;
    public bool alterationDuringMove = false;
    public bool diceDiscarded = false;
    public bool scimitarParry = false;
    public bool maceUsed = false;
    public GameObject dieSavedFromLastRound = null;
    public bool discardDieBecauseCourage = false;

    private void Start() {
        blackBox.transform.position = offScreen;
        scripts = FindObjectOfType<Scripts>();
        DisplayWounds();
        SetTargetOf("player");
        SetTargetOf("enemy");
        scripts.enemy.TargetBest();
        scripts.statSummoner.ResetDiceAndStamina();
        scripts.diceSummoner.SummonDice(true);
        scripts.statSummoner.SummonStats();
        DetermineMove(true);
    }

    /// <summary>
    /// Make the enemy move (if it is their turn).
    /// </summary>
    /// <param name="delay"></param>
    public void DetermineMove(bool delay=false) {
        if (scripts.statSummoner.SumOfStat("blue", "player") < scripts.statSummoner.SumOfStat("blue", "enemy") && !(scripts.itemManager.PlayerHasWeapon("spear"))) {
            // if enemy is faster or player doesn't have spear
            StartCoroutine(EnemyMove(delay));
            // make the enemy move
        }
    } 

    /// <summary>
    /// Make the enemy choose die.
    /// </summary>
    /// <param name="delay">true to have a 0.45s delay before choosing, false to instantly chhoose.</param>
    /// <param name="selectAllRemaining"></param>
    /// <returns></returns>
    public IEnumerator EnemyMove(bool delay, bool selectAllRemaining=false) {
        if (delay) { yield return scripts.delays[0.45f]; }
        // delay if necessary
        if (selectAllRemaining) {
            for (int i = 0; i < 3; i++) {
                // if the player has used a scroll of haste, make the enemy choose all remaining die
                scripts.enemy.ChooseBestDie();
            }
        }
        else { scripts.enemy.ChooseBestDie(); }
        // otherwise, just choose a die
    }

    /// <summary>
    /// Make sure the player/enemy isn't aiming at a place they can't with their current accuracy. Reassigns the target if they can't.
    /// </summary>
    /// <param name="playerOrEnemy">Who to perform the check on.</param>
    public void RecalculateMaxFor(string playerOrEnemy) {
        if (playerOrEnemy == "player") {
            SetAvailableTargetsOf(playerOrEnemy);
            if (scripts.player.targetIndex > scripts.player.availableTargets.Count) {
                scripts.player.targetIndex = scripts.player.availableTargets.Count;
            }
            // check the available targets
            SetTargetOf("player");
            // reset the target
        }
        else if (playerOrEnemy == "enemy") {
            SetAvailableTargetsOf(playerOrEnemy);
            if (scripts.enemy.targetIndex > scripts.enemy.availableTargets.Count) {
                scripts.enemy.targetIndex = scripts.enemy.availableTargets.Count;
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
        if (scripts.player.woundList.Count > 0) {
            // if player has 1 wound or more
            scripts.player.woundGUIElement.text = "";
            // clear the text
            foreach (string wound in scripts.player.woundList) {
                scripts.player.woundGUIElement.text += ("*" + wound + "\n");
                // set the wound next fo each one
            }
        }
        else { scripts.player.woundGUIElement.text = "[no wounds]"; }
        // 0 wounds, so display as such
        if (scripts.enemy.woundList.Count > 0) {
            scripts.enemy.woundGUIElement.text = "";
            foreach (string wound in scripts.enemy.woundList) {
                scripts.enemy.woundGUIElement.text += ("*" + wound + "\n");
            }
        }
        else { scripts.enemy.woundGUIElement.text = "[no wounds]"; }
        // pretty much the same as the above block
    }

    /// <summary>
    /// Fade and change the color of the text.
    /// </summary>
    /// <param name="text">The textmeshpro element for the text to change.</param>
    public IEnumerator InjuredTextChange(TextMeshProUGUI text) {
        yield return scripts.delays[0.55f];
        // set a delay
        Color temp = text.color;
        temp.a = 0.5f;
        // set text transparency to 1/2
        for (int i = 0; i < 20; i++) {
            yield return scripts.delays[0.01f];
            temp.a -= 0.025f;
            text.color = temp;
        }
        // fade out
        DisplayWounds();
        // update the wound display
        for (int i = 0; i < 40; i++) {
            yield return scripts.delays[0.005f];
            temp.a += 0.025f;
            text.color = temp;
        }
        // fade back in
    }

    /// <summary>
    /// Set the current target and text based on the target index.
    /// </summary>
    /// <param name="playerOrEnemy">Update the target for either the player or the enemy.</param>
    public void SetTargetOf(string playerOrEnemy) {
        if (playerOrEnemy == "player") {
            if (scripts.statSummoner.SumOfStat("green", "player") < 0) {
                // if not enough accuracy, set the proper target text
                scripts.player.target.text = "none";
                scripts.player.targetInfo.text = "not enough accuracy to inflict any wound";
            }
            else {
                // set the player's attack indicator + description based on the target index
                if (scripts.enemy.woundList.Contains(targetArr[scripts.player.targetIndex])) { scripts.player.target.text = "*" + targetArr[scripts.player.targetIndex]; }
                // add an asterick if already injured
                else { scripts.player.target.text = targetArr[scripts.player.targetIndex]; }
                scripts.player.targetInfo.text = targetInfoArr[scripts.player.targetIndex];
            }
        }
        else if (playerOrEnemy == "enemy") {
            // set enemy's target indicator based on the target index
            if (scripts.player.woundList.Contains("*")) { scripts.enemy.target.text = "*"+targetArr[scripts.enemy.targetIndex]; }
            else { scripts.enemy.target.text = targetArr[scripts.enemy.targetIndex]; }
        }
        else { Debug.LogError("Invalid string passed in to SetTarget() in TurnManager.cs"); }
    }

    /// <summary>
    /// Update the list of available targets based on accuracy. 
    /// </summary>
    /// <param name="playerOrEnemy">Whether to update the player or enemy's target list.</param>
    public void SetAvailableTargetsOf(string playerOrEnemy) {
        int accuracy = scripts.statSummoner.SumOfStat("green", playerOrEnemy);
        // get the accuracy sum 
        if (accuracy > 7) { accuracy = 7; }
        // limit accuracy to be 7
        if (playerOrEnemy == "player") {
            scripts.player.availableTargets.Clear();
            // clear the list of targets
            foreach (string targetingString in targetArr.Take(accuracy)) {
                // take the # of wounds based on accuracy (system.linq function)
                scripts.player.availableTargets.Add(targetingString);
                // add each one to the available targets
            }
        }
        else if (playerOrEnemy == "enemy") {
            scripts.enemy.availableTargets.Clear();
            foreach (string targetingString in targetArr.Take(accuracy)) {
                scripts.enemy.availableTargets.Add(targetingString);
            }
            // same as above
        }
        else { Debug.LogError("Invalid string passed in to SetAvailableTargetsOf() in TurnManager.cs"); }
    }

    /// <summary>
    /// Change the available stamina of the player or enemy by the specified amount.
    /// </summary>
    /// <param name="playerOrEnemy">Who to change the stamina of.</param>
    /// <param name="amount">The amount to change the stamina of.</param>
    public void ChangeStaminaOf(string playerOrEnemy, int amount) {
        if (playerOrEnemy == "player") {
            scripts.player.stamina += amount;
            // change stamina
            scripts.player.staminaCounter.text = scripts.player.stamina.ToString();
            // update counter
            RecalculateMaxFor(playerOrEnemy);
            // recalculate max (in case stamina was taken from green)
        }
        else if (playerOrEnemy == "enemy") {
            scripts.enemy.stamina += amount;
            scripts.enemy.staminaCounter.text = scripts.enemy.stamina.ToString();
            RecalculateMaxFor(playerOrEnemy);
            // same as above
        }
        else { Debug.LogError("Invalid string passed in to ChangeStaminaAndUpdate() in TurnManager.cs"); }
    }

    /// <summary>
    /// Start the first round of attack.
    /// </summary>
    public void RoundOne() {
        scimitarParry = false;
        // reset the scimitar parry (if set true from a previous round)
        isMoving = true;
        // set the variable to true so that certain actions can check this and make sure actions are not taken when they shouldn't be
        List<Dice> availableDice = new List<Dice>();
        // create an empty list to hold die in
        foreach (GameObject dice in scripts.diceSummoner.existingDice) {
            // for every die
            if (dice.GetComponent<Dice>().isAttached == false) {
                availableDice.Add(dice.GetComponent<Dice>());
                // if the die has not been chosen, add it to the list 
            }
        }
        if (availableDice.Count == 0) {
            // if there are no available die
            RunEnemyCalculations();
            // make enemy add stamina to stats as necessary
            if (scripts.player.woundList.Contains("head")) {
                scripts.enemy.DiscardBestPlayerDie();
                // if the player has a head wound, make the enemy discard it
            }
            InitializeVariables(out int playerAim, out int enemyAim, out int playerSpd, out int enemySpd, out int playerAtt, out int enemyAtt, out int playerDef, out int enemyDef);
            // get all the stats so we can use them
            if (playerSpd >= enemySpd) {
                // make player go first
                if (!PlayerAttacks(playerAtt, enemyDef)) {
                    // if enemy was not killed
                    StartCoroutine(RoundTwo("enemy"));
                    // begin the next round where the player will attadck
                }
                else {
                    // enemy was killed
                    SetTargetOf("player");
                    // reset target
                    StartCoroutine(Kill("enemy"));
                    // make the enemy die
                    // reset ismoving
                }
            }
            else {
                // enemy goes first
                scripts.player.isDodgy = false;
                scripts.player.SetPlayerStatusEffect("dodge", "off");
                // enemy went first, so player can't be dodgy
                if (!EnemyAttacks(enemyAtt, playerDef)) {
                    // if player doesn't die
                    StartCoroutine(RoundTwo("player"));
                    // start next round with player going
                }
                else {
                    // player was killed
                    StartCoroutine(Kill("player"));
                    // show animation
                }
            }
        }
        else { 
            // dice are available
            isMoving = false;
            // stop moving
            if (scripts.itemManager.PlayerHasWeapon("mace") && !maceUsed) {
                // if player has mace
                maceUsed = true;
                // prevent player from using mace again
                foreach (Dice dice in from a in scripts.diceSummoner.existingDice where a.GetComponent<Dice>().isAttached == false select a.GetComponent<Dice>()) {
                    // for every die that is not attached
                    StartCoroutine(dice.RerollAnimation(false));
                    // reroll the die
                }
            }
            else {
                SetStatusText("choose a die"); 
                // player doesn't have mace, so notify them to choose a die
            }
        }
    }

    /// <summary>
    /// Start the second round of attacks with the specified thing attacking.
    /// </summary>
    /// <param name="toMove">Who should be the one attacking.</param>
    private IEnumerator RoundTwo(string toMove) {
        int playerAtt;
        int playerDef;
        int enemyAtt;
        int enemyDef;
        // variables to hold stats
        isMoving = true;
        // make the player ready to move
        yield return scripts.delays[2f];
        // wait 2 seconds for animation/status text from previous round to finish
        if (toMove == "player") {
            // if player is the one attacking
            if (scimitarParry) {
                // if they parried and had a scimitar
                scripts.turnManager.SetStatusText("discard enemy's die");
                // notify player
                scripts.itemManager.discardableDieCounter++;
                // increment # is discardable die
                actionsAvailable = true;
                // allow for player to take actions
                for (float i = 2.5f; i > 0; i -= 0.1f) {
                    // 2.5s time slot
                    if (scripts.diceSummoner.breakOutOfScimitarParryLoop) { break; }
                    // handle the discard somewhere else, if the action was taken then will break out
                    yield return scripts.delays[0.1f];
                    // wait 
                }
                actionsAvailable = false;
                // prevent further action
                scimitarParry = false;
                // reset the variable
            }
            playerAtt = scripts.statSummoner.SumOfStat("red", "player");
            enemyDef = scripts.statSummoner.SumOfStat("white", "enemy");
            // get necessary stats 
            if (PlayerAttacks(playerAtt, enemyDef)) {
                // if player kills the enemy
                SetTargetOf("player");
                // reset target
                StartCoroutine(Kill("enemy"));
                // play information
            }
        }
        else if (toMove == "enemy") {
            // enemy is the one attacking
            if ((scripts.enemy.woundList.Contains("chest") && Rerollable() || scripts.enemy.woundList.Contains("head") && !diceDiscarded) && PlayerPrefs.GetString("hints") == "on") {
                // if player can reroll or discard enemy's die and hints are on
                if (scripts.enemy.woundList.Contains("head")) { SetStatusText("note: you can discard enemy's die"); }
                else if (scripts.enemy.woundList.Contains("chest")) { SetStatusText("note: you can reroll enemy's dice"); }
                // notify the player
                actionsAvailable = true;
                // allow actions
                for (float i = 2.5f; i > 0; i -= 0.1f) {
                    // 2.5 second time slot
                    if (alterationDuringMove) {
                        // actions handled elsewhere, but if there is an action taken (e.g. discard)
                        i += 0.75f;
                        // increase time slot
                        alterationDuringMove = false;
                        // allow timer to be changed again
                    }
                    yield return scripts.delays[0.1f];
                    // wait
                }
                actionsAvailable = false;
                diceDiscarded = false;
                // reset the available actions
            }
            enemyAtt = scripts.statSummoner.SumOfStat("red", "enemy");
            playerDef = scripts.statSummoner.SumOfStat("white", "player");
            // get necessary stats
            if (EnemyAttacks(enemyAtt, playerDef)) {
                // if enemy kills player
                StartCoroutine(Kill("player"));
                // play animation
            }
        }
        else { print("error passing into ienumerator attack"); }
        if (!scripts.player.isDead && !scripts.enemy.isDead) { 
            // if neither player or enemy is dead
            yield return scripts.delays[2f];
            // wait for status text/animation etc.
            if (scripts.player.isCourageous) {
                // if player is courageous (save die to next round)
                actionsAvailable = true;
                // allow actions
                discardDieBecauseCourage = true;
                // make sure the die will discard under the correct pretense
                scripts.turnManager.SetStatusText("discard all your dice, except one");
                // notify player
                for (int i = 0; i < 50; i++) {
                    if ((from a in scripts.diceSummoner.existingDice where a.GetComponent<Dice>().isAttached && a.GetComponent<Dice>().isOnPlayerOrEnemy == "player" select a).ToList().Count <= 1) {
                        // if player has 1 (or less) die attached
                        dieSavedFromLastRound = (from a in scripts.diceSummoner.existingDice where a.GetComponent<Dice>().isOnPlayerOrEnemy == "player" select a).ToList()[0];
                        // save the one remaining die
                        break;
                        // break out of the loop
                    }
                    yield return scripts.delays[0.1f];
                    // wait
                }
                if (dieSavedFromLastRound == null && scripts.diceSummoner.existingDice.Count > 0) {
                    // player has not discarded enough dice in the 5s time slot, so choose a random one
                    dieSavedFromLastRound = (from a in scripts.diceSummoner.existingDice where a.GetComponent<Dice>().isOnPlayerOrEnemy == "player" select a).ToList()[UnityEngine.Random.Range(0, (from a in scripts.diceSummoner.existingDice where a.GetComponent<Dice>().isOnPlayerOrEnemy == "player" select a).ToList().Count)];
                    // save random die
                }
                actionsAvailable = false;
                discardDieBecauseCourage = false;
                scripts.player.isCourageous = false;
                scripts.player.SetPlayerStatusEffect("courage", "off");
                // reset necessary variables
            }
            isMoving = false;
            // stop moving
            scripts.statSummoner.ResetDiceAndStamina();
            // reset die and stamina
            scripts.diceSummoner.SummonDice(false);
            // summon the die again
            scripts.statSummoner.SummonStats();
            // summon the stats again
            RecalculateMaxFor("player");
            RecalculateMaxFor("enemy");
            // make sure the player and enemy are aiming at the correct place
            DetermineMove(true);
            // make the next person go again
        }
        yield return scripts.delays[0.45f];
        // small delay
        scripts.player.isFurious = false;
        scripts.player.SetPlayerStatusEffect("fury", "off");
        scripts.player.isDodgy = false;
        scripts.player.SetPlayerStatusEffect("dodge", "off");
        scripts.player.isBloodthirsty = false;
        scripts.player.SetPlayerStatusEffect("leech", "off");
        scripts.highlightCalculator.diceTakenByPlayer = 0;
        scripts.itemManager.discardableDieCounter = 0;
        scripts.itemManager.usedAnkh = false;
        scripts.itemManager.usedBoots = false;
        scripts.itemManager.usedHelm = false;
        scripts.diceSummoner.breakOutOfScimitarParryLoop = false;
        maceUsed = false;
        ClearPotionStats();
        // reset all variables used in preparation for the next round
    }
    
    /// <summary>
    /// Clear the stats gained from potions from the player.
    /// </summary>
    public void ClearPotionStats() {
        foreach (string key in scripts.itemManager.statArr) {
            // for every key
            scripts.player.potionStats[key] = 0;
            // clear stats
        }
    }

    /// <summary>
    /// Coroutine to play the death animation, set status text, toggle variables, etc.
    /// </summary>
    /// <param name="playerOrEnemy">Who to perform the function on.</param>
    public IEnumerator Kill(string playerOrEnemy) {
        if (playerOrEnemy == "player") { scripts.player.isDead = true; }
        else if (playerOrEnemy == "enemy") { scripts.enemy.isDead = true; }
        // make sure whoever is killed is set to be dead
        yield return scripts.delays[0.55f];
        // short delay
        if (playerOrEnemy == "player") {
            scripts.turnManager.SetStatusText($"{scripts.enemy.enemyName.text.ToLower()} hits you... you die");
            StartCoroutine(PlayDeathAnimation("player"));
            // set status text and play the animation
        }
        else if (playerOrEnemy == "enemy") {
            scripts.turnManager.SetStatusText($"you hit {scripts.enemy.enemyName.text.ToLower()}... he dies");
            StartCoroutine(PlayDeathAnimation("enemy"));
            // set status text and play the animation
        }
        else { print("invalid string passed"); }
        yield return scripts.delays[0.45f];
        isMoving = false;
    }

    /// <summary>
    /// Play the hit animation for the player or enemy.
    /// </summary>
    /// <param name="playerOrEnemy">Who to play the hit animation for.</param>
    private IEnumerator PlayHitAnimation(string playerOrEnemy) {
        SpriteRenderer spriteRenderer = playerOrEnemy == "player" ? scripts.player.GetComponent<SpriteRenderer>() : scripts.enemy.GetComponent<SpriteRenderer>();
        // get the proper spriterenderer
        // conditional is true ? yes : no
        Color temp = Color.white;
        temp.a = 0.5f;
        // white with 50% transparency
        for (int i = 0; i < 14; i++) {
            yield return scripts.delays[0.0125f];
            temp.a -= 1f / 28f;
            spriteRenderer.color = temp;
        }
        // fade out
        for (int i = 0; i < 28; i++) {
            yield return scripts.delays[0.0125f];
            temp.a += 1f / 28f;
            spriteRenderer.color = temp;
        }
        // fade back in
    }

    /// <summary>
    /// Play the death animation for the player or enemy.
    /// </summary>
    /// <param name="playerOrEnemy">Who to play the death animation for.</param>
    public IEnumerator PlayDeathAnimation(string playerOrEnemy) {
        SpriteRenderer spriteRenderer = playerOrEnemy == "player" ? scripts.player.GetComponent<SpriteRenderer>() : scripts.enemy.GetComponent<SpriteRenderer>();
        // get the proper spriterenderer
        // conditional is true ? yes : no
        Color temp = Color.white;
        temp.a = 0.5f;
        // white with 50% transparency
        for (int i = 0; i < 14; i++) {
            yield return scripts.delays[0.0125f];
            temp.a -= 1f / 28f;
            spriteRenderer.color = temp;
        }
        // fade out
        if (playerOrEnemy == "player") { scripts.player.GetComponent<Animator>().enabled = false; }
        else if (playerOrEnemy == "enemy") { scripts.enemy.GetComponent<Animator>().enabled = false; }
        // disable the animator for whoever is dead
        for (int i = 0; i < 28; i++) {
            yield return scripts.delays[0.0125f];
            temp.a += 1f / 28f;
            spriteRenderer.color = temp;
        }
        // fade back in
        yield return scripts.delays[0.8f];
        // pause (animation is not playing so it looks like they are just standing there)
        scripts.soundManager.PlayClip("death");
        // play sound clip
        if (scripts.itemManager.PlayerHasWeapon("rapier") && playerOrEnemy == "enemy") { ChangeStaminaOf("player", 3); }
        // if player has rapier and the enemy dies, add to their stamina
        if (playerOrEnemy == "player") {
            scripts.player.GetComponent<SpriteRenderer>().sprite = scripts.player.GetDeathSprite();
            scripts.player.SetPlayerPositionAfterDeath();
            // if player dies, set sprite and proper position
        }
        else if (playerOrEnemy == "enemy") {
            scripts.enemy.GetComponent<SpriteRenderer>().sprite = scripts.enemy.GetDeathSprite();
            scripts.enemy.SetEnemyPositionAfterDeath();
            // if enemy dies, set sprite and proper position
            scripts.itemManager.SpawnItems();
            // spawn items
            blackBox.transform.position = onScreen;
            // hide the enemy's tats
        }
        else { print("invalid string passed"); }
        foreach (GameObject dice in scripts.diceSummoner.existingDice) {
            StartCoroutine(dice.GetComponent<Dice>().FadeOut(false, true));
            // fade out all existing die
        }
        scripts.statSummoner.ResetDiceAndStamina();
        // clear them
        ClearPotionStats();
        // clear potion stats
        scripts.statSummoner.SummonStats();
        // summon stats
        scripts.statSummoner.SetDebugInformationFor("player");
        // set debug (only player needed here)
        RecalculateMaxFor("player");
        RecalculateMaxFor("enemy");
        // reset target for both
        scripts.itemManager.GivePlayerRetry();
        // allow the player to retry
    }

    /// <summary>
    /// Coroutine for fading in the status text.
    /// </summary>
    /// <param name="text">The text to set the status text to.</param>
    private IEnumerator StatusTextCoroutine(string text) {
        Color temp = statusText.color;
        temp.a = 0f;
        // make the text invisible
        statusText.text = text;
        // set the status text to the desired text
        for (int i = 0; i < 10; i++) {
            yield return scripts.delays[0.033f];
            temp.a += 0.1f;
            statusText.color = temp;
        }
        // fade in
        yield return scripts.delays[1f];
        // wait 1 sec (so player has time to read)
        for (int i = 0; i < 10; i++) {
            yield return scripts.delays[0.033f];
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
    public void SetStatusText(string text) {
        try { StopCoroutine(coroutine); } catch {}
        // stop any existing status text coroutines
        coroutine = StartCoroutine(StatusTextCoroutine(text));
        // set the status text, and allow for it to be stopped
    }

    /// <summary>
    /// Perform actions (sound, animation) for when a player or enemy is hit.
    /// </summary>
    /// <param name="hitOrParry">Whether the attack was a hit or a parry.</param>
    /// <param name="playerOrEnemy">Who is getting hit/parrying.</param>
    /// <param name="showAnimation">true to show the animation, false to not (true by default).</param>
    /// <param name="armor">true if the player has armor, false if not (false by default).</param>
    /// <returns></returns>
    public IEnumerator DoStuffForAttack(string hitOrParry, string playerOrEnemy, bool showAnimation=true, bool armor=false) {
        yield return scripts.delays[0.55f];
        // wait
        if (hitOrParry == "hit") {
            if (scripts.player.isDodgy && playerOrEnemy == "player") {
                scripts.soundManager.PlayClip("miss");
                // play sound clip
            }
            // player dodges
            else {
                scripts.soundManager.PlayClip("hit");
                // play sound clip
                if (showAnimation) {
                    // if showing an animation
                    if (!(playerOrEnemy == "player" && armor)) { 
                        // if NOT player is getting hit and has armor
                        StartCoroutine(PlayHitAnimation(playerOrEnemy)); 
                    }
                }
            }
        }
        else if (hitOrParry == "parry") {
            scripts.soundManager.PlayClip("parry");
            // play sound clip
        }
        else { Debug.LogError("invalid string passed"); }
    }

    /// <summary>
    /// Perform actions for the enemy's attack.
    /// </summary>
    /// <param name="enemyAtt">The enemy's attack stat.</param>
    /// <param name="playerDef">The player's parry stat.</param>
    /// <returns>true if the player was killed, false otherwise</returns>
    private bool EnemyAttacks(int enemyAtt, int playerDef) {
        bool armor = false;
        // set armor to false
        scripts.soundManager.PlayClip("swing");
        // play sound clip
        if (enemyAtt > playerDef) {
            // if enemy is hitting player
            if (scripts.player.isDodgy) { SetStatusText($"{scripts.enemy.enemyName.text.ToLower()} hits you... you dodge"); }
            // if player dodges, notify 
            else {
                if (scripts.itemManager.PlayerHas("armour")) {
                    scripts.soundManager.PlayClip("armor");
                    armor = true;
                    // set armor to true
                    SetStatusText($"{scripts.enemy.enemyName.text.ToLower()} hits you... your armour shatters");
                    // notify player
                    scripts.itemManager.GetPlayerItem("armour").GetComponent<Item>().Remove();
                    // remove armor from the player's inventory
                    scripts.itemManager.Select(scripts.player.inventory, 0, playAudio: false);
                    // select weapon
                }
                else {
                    if (scripts.enemy.target.text != "face") {
                        // if player is not targeting face
                        if (scripts.enemy.target.text.Contains("*")) {
                            // if previously wounded
                            SetStatusText($"{scripts.enemy.enemyName.text.ToLower()} hits you, damaging {scripts.enemy.target.text.Substring(1)}!");
                            // notify player
                        }
                        else {
                            if (scripts.player.woundList.Count != 2) {
                                // if player won't die
                                SetStatusText($"{scripts.enemy.enemyName.text.ToLower()} hits you, damaging {scripts.enemy.target.text}!");
                                // notify player
                            }
                        }
                    }
                }
            }
            StartCoroutine(DoStuffForAttack("hit", "player", true, armor));
            // play animation + sound for the attack
            if (!(scripts.player.woundList.Contains(scripts.enemy.target.text) || scripts.player.woundList.Contains(scripts.enemy.target.text.Substring(1)) || armor || scripts.player.isDodgy)) {
                // if the player hasn't been injured before, doesn't have armor, and didnt' dodge:
                scripts.player.woundList.Add(scripts.enemy.target.text);
                // add the hit
                StartCoroutine(InjuredTextChange(scripts.player.woundGUIElement));
                // make it change
                RecalculateMaxFor("player");
                // reset stuff
                return InstantlyApplyInjuries(scripts.enemy.target.text, "player");
                // return if player died or not
            }
        }
        else {
            // player parried
            StartCoroutine(DoStuffForAttack("parry", "player"));
            // play sound and animation
            if (enemyAtt < 0) { SetStatusText($"{scripts.enemy.enemyName.text.ToLower()} hits you... the attack is to weak"); }
            else { 
                if (scripts.itemManager.PlayerHasWeapon("scimitar")) { scimitarParry = true; }
                // player has parried (this resets at the start of every round so we can do it regardless)
                SetStatusText($"{scripts.enemy.enemyName.text.ToLower()} hits you... you parry"); 
            }
            // notify player
        }
        return false;
        // player hasn't died, so return false
    }

    private bool PlayerAttacks(int playerAtt, int enemyDef) {
        scripts.soundManager.PlayClip("swing");
        // play sound clip
        if (playerAtt > enemyDef) {
            // if player will hit enemy
            if (scripts.statSummoner.SumOfStat("green", "player") < 0) {
                // player doesn't have enough accuracy to hit, so notify
                SetStatusText($"you hit {scripts.enemy.enemyName.text.ToLower()}... you miss");
                scripts.soundManager.PlayClip("miss");
                // play sound clip
            }
            else {
                if (scripts.player.target.text == "face" || (scripts.enemy.woundList.Count == 2 && !scripts.player.target.text.Contains("*"))) {
                    // enemy is going to die
                    StartCoroutine(DoStuffForAttack("hit", "enemy", false));
                    // play sound but no animation
                }
                else {
                    // enemy will not die
                    StartCoroutine(DoStuffForAttack("hit", "enemy"));
                    // play sound and animation
                    if (scripts.player.target.text.Contains("*")) {
                        // if already injured
                        if (scripts.itemManager.PlayerHasWeapon("maul")) {}
                        // don't say anything for maul (we want it to show that it is an instant kill)
                        else {
                            SetStatusText($"you hit {scripts.enemy.enemyName.text.ToLower()}, damaging {scripts.player.target.text.Substring(1)}!");
                        }
                        // notify player if hitting injured
                    }
                    else {
                        // not injured
                        if (scripts.itemManager.PlayerHasWeapon("maul")) {}
                        else {
                            SetStatusText($"you hit {scripts.enemy.enemyName.text.ToLower()}, damaging {scripts.player.target.text}!");
                        }
                        // same as above
                    }
                }
                if (!scripts.player.target.text.Contains("*") && scripts.statSummoner.SumOfStat("green", "player") >= 0) {
                    // if wound was not injurd and player has enough accuracy to hit
                    scripts.enemy.woundList.Add(scripts.player.target.text);
                    // add the wound
                    if (scripts.player.isBloodthirsty) {
                        // if player is wounded
                        try { scripts.player.woundList.Remove(scripts.player.target.text); } catch { print("no need to heal"); }
                        // try to heal the wound, else don't do anything
                        StartCoroutine(InjuredTextChange(scripts.player.woundGUIElement));
                        // update the text
                        scripts.player.isBloodthirsty = false;
                        scripts.player.SetPlayerStatusEffect("leech", "off");
                        // turn off bloodthirsty
                    }
                    StartCoroutine(InjuredTextChange(scripts.enemy.woundGUIElement));
                    // make the text change
                    RecalculateMaxFor("enemy");
                    // recalculate max
                    return InstantlyApplyInjuries(scripts.player.target.text, "enemy");
                    // return if the enemy dies and at the same time apply wounds instantly
                }
                if (scripts.itemManager.PlayerHasWeapon("maul")) { return true; }
                // kill enemy instantly if player has a maul
            }
        }
        else {
            // enemy will parry
            StartCoroutine(DoStuffForAttack("parry", "enemy"));
            // play animation and sound
            if (playerAtt < 0) { SetStatusText($"you hit {scripts.enemy.enemyName.text.ToLower()}... the attack is too weak"); }
            else if (scripts.statSummoner.SumOfStat("green", "player") < 0) {
                SetStatusText($"you hit {scripts.enemy.enemyName.text.ToLower()}... you miss");
                scripts.soundManager.PlayClip("miss");
                // play sound clip
            }
            else { SetStatusText($"you hit {scripts.enemy.enemyName.text.ToLower()}... he parries"); }
            // depending on the stats, notify player accordingly
        }
        return false;
        // enemy has not died, so return false
    }

    /// <summary>
    /// Instantly apply injury effects (such as decreasing die on gut wound).
    /// </summary>
    /// <param name="injury">The injury name to apply the effects of.</param>
    /// <param name="appliedTo">Who the injury was applied to.</param>
    /// <returns>true if who the injury was applied to dies, false otherwise.</returns>
    private bool InstantlyApplyInjuries(string injury, string appliedTo) {
        if (appliedTo != "enemy" && appliedTo != "player") { print("invalid string passed into param. appliedTo in InstantlyApplyInjuries"); }
        // just checking
        if (scripts.itemManager.PlayerHasWeapon("maul") && appliedTo == "enemy") { return true; }
        // return true immediately if maul
        if (injury == "guts") {
            // for guts, decrease all die
            if (appliedTo == "player") {
                foreach (string key in scripts.statSummoner.addedPlayerDice.Keys) {
                    foreach (Dice dice in scripts.statSummoner.addedPlayerDice[key]) {
                        StartCoroutine(dice.DecreaseDiceValue());
                    }
                }
                RecalculateMaxFor("player");
            }
            else if (appliedTo == "enemy") {
                foreach (string key in scripts.statSummoner.addedEnemyDice.Keys) {
                    foreach (Dice dice in scripts.statSummoner.addedEnemyDice[key]) {
                        StartCoroutine(dice.DecreaseDiceValue());
                    }
                }
                RecalculateMaxFor("enemy");
            }
        }
        else if (injury == "hip") {
            // if hip, remove all applied stamina
            if (appliedTo == "player") {
                scripts.statSummoner.addedPlayerStamina = new Dictionary<string, int>() {
                    { "green", 0 },
                    { "blue", 0 },
                    { "red", 0 },
                    { "white", 0 },
                };
            }
            else {
                scripts.statSummoner.addedEnemyStamina = new Dictionary<string, int>() {
                    { "green", 0 },
                    { "blue", 0 },
                    { "red", 0 },
                    { "white", 0 },
                };
            }
        }
        else if (injury == "hand") {
            // if hand, remove white die
            if (appliedTo == "player") {
                StartCoroutine(RemoveDice("white", "player"));
            }
            else if (appliedTo == "enemy") {
                StartCoroutine(RemoveDice("white", "enemy"));
            }
        }
        else if (injury == "armpits") {
            // if armpits, remove red die
            if (appliedTo == "player") {
                StartCoroutine(RemoveDice("red", "player"));
            }
            else if (appliedTo == "enemy") {
                StartCoroutine(RemoveDice("red", "enemy"));
            }
        }
        else if (injury == "face") {
            // if face, kill instantly
            return true;
        }
        scripts.statSummoner.SetDebugInformationFor("player");
        scripts.statSummoner.SetDebugInformationFor("enemy");
        // update debug information
        if (appliedTo == "player" && scripts.player.woundList.Count == 3) { return true; }
        else if (appliedTo == "enemy" && scripts.enemy.woundList.Count == 3) { return true; }
        // if 3 wounds, return killed
        return false;
        // appliedto has ot been killed, so return as such
    }

    /// <summary>
    /// Coroutine to remove all of a dice type from the player or enemy.
    /// </summary>
    /// <param name="diceType">The type of die to remove.</param>
    /// <param name="removeFrom">Who to remove the die from.</param>
    private IEnumerator RemoveDice(string diceType, string removeFrom) {
        yield return scripts.delays[1f];
        print("function called");
        List<Dice> diceList = removeFrom == "player" ? scripts.statSummoner.addedPlayerDice[diceType] : scripts.statSummoner.addedEnemyDice[diceType];
        // assign the correct dicelist
        // conditional is true ? yes: no
        foreach (Dice dice in diceList.ToList()) {
            // for every die in the stat we want to remove from
            if (dice.diceType == diceType) {
                // if the die type matches
                int index = diceList.IndexOf(dice);
                // get where the die is currently in the array
                scripts.diceSummoner.existingDice.Remove(dice.gameObject);
                diceList.Remove(dice);
                StartCoroutine(dice.FadeOut());
                // remove from arrays and destroy
                for (int i = index; i < diceList.Count; i++) {
                    // for every die that was after where the die that was removed was
                    if (removeFrom == "player") { diceList[i].transform.position = new Vector2(diceList[i].transform.position.x - scripts.statSummoner.diceOffset, diceList[i].transform.position.y); }
                    else { diceList[i].transform.position = new Vector2(diceList[i].transform.position.x + scripts.statSummoner.diceOffset, diceList[i].transform.position.y); } 
                    
                    // shift the die over
                    diceList[i].GetComponent<Dice>().instantiationPos = diceList[i].transform.position;
                    // set the new instantiation position
                }
            }
        }
    }

    /// <summary>
    /// Make the enemy evaluate the current stats and add stamina to attack/defend etc.
    /// </summary>
    private void RunEnemyCalculations() {
        if (!scripts.enemy.woundList.Contains("hip")) {
            InitializeVariables(out int playerAim, out int enemyAim, out int playerSpd, out int enemySpd, out int playerAtt, out int enemyAtt, out int playerDef, out int enemyDef);
            if (enemyAtt <= playerDef && enemyAtt + scripts.enemy.stamina > playerDef) {
                // if enemy can hit player
                UseEnemyStaminaOn("red", (playerDef - enemyAtt) + 1);
                // add stamina to hit player
                if (playerSpd >= enemySpd && playerAtt > enemyDef) {
                    // if player will attack first
                    if (enemySpd + scripts.enemy.stamina > playerSpd) {
                        // if enemy can attack first
                        // add speed to go first
                        UseEnemyStaminaOn("blue", (playerSpd - enemySpd) + 1);
                    }
                }
                if (enemyAim < 7 && enemyAim + scripts.enemy.stamina > 6) {
                    // if enemy can target face
                    // add accuracy to target face
                    UseEnemyStaminaOn("green", 7 - enemyAim);
                    scripts.enemy.TargetBest();
                }
            }
            if (enemyDef < playerAtt && enemyDef + scripts.enemy.stamina >= playerAtt) {
                // if enemy will be hit and can defend
                // add stamina to defend
                UseEnemyStaminaOn("white", playerAtt - enemyDef);
                // TO DO: check for gut/hip/chest strike and take actions correspondingly
            }
            scripts.statSummoner.SetDebugInformationFor("enemy");
            // update the debug
        }
    }

    /// <summary>
    /// Make the enemy use stamina.
    /// </summary>
    /// <param name="stat">Which stat to use the stamina on.</param>
    /// <param name="amount">The amount of stamina to use on the stat.</param>
    private void UseEnemyStaminaOn(string stat, int amount) {
        if (scripts.enemy.stamina < amount) { print("too much stamina to use!"); }
        // restrict just in case
        else {
            scripts.statSummoner.addedEnemyStamina[stat] += amount;
            // incrase stat
            scripts.turnManager.ChangeStaminaOf("enemy", -amount);
            // decrease available
            scripts.statSummoner.SummonStats();
            // summon stats again
            foreach (Dice dice in scripts.statSummoner.addedEnemyDice[stat]) {
                // for every die in the stat
                dice.transform.position = new Vector2(dice.transform.position.x - scripts.statSummoner.xOffset * amount, dice.transform.position.y);
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
        playerAim = scripts.statSummoner.SumOfStat("green", "player");
        enemyAim = scripts.statSummoner.SumOfStat("green", "enemy");
        playerSpd = scripts.statSummoner.SumOfStat("blue", "player");
        enemySpd = scripts.statSummoner.SumOfStat("blue", "enemy");
        playerAtt = scripts.statSummoner.SumOfStat("red", "player");
        enemyAtt = scripts.statSummoner.SumOfStat("red", "enemy");
        playerDef = scripts.statSummoner.SumOfStat("white", "player");
        enemyDef = scripts.statSummoner.SumOfStat("white", "enemy");
    }

    /// <summary>
    /// Checks if there is a die on the enemy that can be rerolled and is >= 3.
    /// </summary>
    /// <returns>true if there are die to be rerolled, false if not.</returns>
    private bool Rerollable() {
        foreach (string key in scripts.statSummoner.addedEnemyDice.Keys) {
            // for every key
            foreach (Dice dice in scripts.statSummoner.addedEnemyDice[key]) {
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