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
    public bool usedMace = false;
    public GameObject dieSavedFromLastRound = null;
    public bool discardDieBecauseCourage = false;
    public bool dontRemoveLeechYet = false;

    private void Start() {
        scripts = FindObjectOfType<Scripts>();
        DisplayWounds();
        SetTargetOf("player");
        SetTargetOf("enemy");
        scripts.enemy.TargetBest();
        scripts.statSummoner.ResetDiceAndStamina();
        if (!(scripts.levelManager.level == scripts.gameData.tsLevel && scripts.levelManager.sub == scripts.gameData.tsSub) && scripts.levelManager.sub != 4) { scripts.diceSummoner.SummonDice(true); }
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
    /// <param name="delay">true to have a 0.45s delay before choosing, false to instantly choose.</param>
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
        if (scripts.enemy.spawnNum == 0) {
            // is the cloaked devil
            scripts.enemy.woundGUIElement.text = "[cloaked]";
        }
        else {
            // any other enemy
            if (scripts.enemy.woundList.Count > 0) {
                scripts.enemy.woundGUIElement.text = "";
                foreach (string wound in scripts.enemy.woundList) {
                    scripts.enemy.woundGUIElement.text += ("*" + wound + "\n");
                }
            }
            else { scripts.enemy.woundGUIElement.text = "[no wounds]"; }
            // pretty much the same as the above block
        }
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
        if (scripts.player.charNum == 3 && scripts.player.woundList.Count != 3 && scripts.enemy.target.text != "face") { 
            // 4th char gets stamina refresh upon wound, assuming he doesn't get killed instantly
            scripts.turnManager.ChangeStaminaOf("player", 3);
        }
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
            if (scripts.levelManager.sub == scripts.gameData.tsSub && scripts.levelManager.level == scripts.gameData.tsLevel && !(scripts.levelManager.sub == 1 && scripts.levelManager.level == 1)) {
                // tombstone
                scripts.player.target.text = "none";
                scripts.player.targetInfo.text = "why would you try to wound a tombstone?";
            }
            else {
                // normal setting
                if (scripts.statSummoner.SumOfStat("green", "player") < 0) {
                    // if not enough accuracy, set the proper target text
                    scripts.player.target.text = "none";
                    scripts.player.targetInfo.text = "not enough accuracy to inflict any wound";
                }
                else {
                    if (scripts.levelManager.level == 4 && scripts.levelManager.sub == 1) {
                        // if devil
                        if (scripts.enemy.woundList.Contains(targetArr[scripts.player.targetIndex])) { scripts.player.target.text = "*" + targetArr[scripts.player.targetIndex]; }
                        // add an asteriks if already injured
                        else { 
                            if (targetArr[scripts.player.targetIndex] == "face") {
                                // can't aim at the devil's face, so notify player
                                scripts.turnManager.SetStatusText("you cannot aim at his face");
                                scripts.player.targetIndex--;
                            }
                            else {
                                scripts.player.target.text = targetArr[scripts.player.targetIndex]; 
                            }
                        }
                        if (targetArr[scripts.player.targetIndex] != "face") {
                            scripts.player.targetInfo.text = targetInfoArr[scripts.player.targetIndex];
                        }
                    }
                    else {
                        // set the player's attack indicator + description based on the target index
                        if (scripts.enemy.woundList.Contains(targetArr[scripts.player.targetIndex])) { scripts.player.target.text = "*" + targetArr[scripts.player.targetIndex]; }
                        // add an asteriks if already injured
                        else { scripts.player.target.text = targetArr[scripts.player.targetIndex]; }
                        
                        if (scripts.enemy.enemyName.text == "Lich") { scripts.player.targetInfo.text = "no effect since enemy is immune"; }
                        // different text for lich (letting player know)
                        else { scripts.player.targetInfo.text = targetInfoArr[scripts.player.targetIndex]; }
                    }
                }
            }
        }
        else if (playerOrEnemy == "enemy") {
            if (scripts.enemy.enemyName.text == "Merchant") { 
                // trader
                scripts.enemy.target.text = "bargain";
            }
            // else if (scripts.levelManager.sub == scripts.tombstoneData.sub && scripts.levelManager.level == scripts.tombstoneData.level) {
            else if (scripts.enemy.enemyName.text == "Tombstone") {
                // tombstone
                scripts.enemy.target.text = "serenity";
            }
            else {
                // normal enemy, so set enemy's target indicator based on the target index
                if (scripts.player.woundList.Contains("*")) { scripts.enemy.target.text = "*"+targetArr[scripts.enemy.targetIndex]; }
                else { scripts.enemy.target.text = targetArr[scripts.enemy.targetIndex]; }
            }
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
            if (scripts.player.stamina >= 10 && scripts.player.charNum == 3) { 
                // heal wounds at 10 stamina
                scripts.player.woundList.Clear();
                StartCoroutine(HealAfterDelay());
            }
            scripts.gameData.playerStamina = scripts.player.stamina;
            scripts.SaveGameData();
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
            if (!PlayerAttacks()) {
                // if enemy was not killed
                StartCoroutine(RoundTwo("enemy"));
                // begin the next round where the player will attack
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
            scripts.player.SetPlayerStatusEffect("dodge", false);
            // enemy went first, so player can't be dodgy
            if (!EnemyAttacks()) {
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
        scripts.persistentData.weaponUses[Array.IndexOf(scripts.itemManager.weaponNames, scripts.player.inventory[0].GetComponent<Item>().itemName.Split(' ')[1])]++;
        // increment the # of times the player's current weapon has been used
        scripts.SavePersistentData();
    }

    /// <summary>
    /// Start the second round of attacks with the specified thing attacking.
    /// </summary>
    /// <param name="toMove">Who should be the one attacking.</param>
    private IEnumerator RoundTwo(string toMove) {
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
            // get necessary stats 
            if (PlayerAttacks()) {
                // if player kills the enemy
                SetTargetOf("player");
                // reset target
                StartCoroutine(Kill("enemy"));
                // play information
            }
        }
        else if (toMove == "enemy") {
            // enemy is the one attacking
            if (scripts.enemy.woundList.Contains("chest") && Rerollable() && scripts.enemy.enemyName.text != "Lich" || scripts.enemy.woundList.Contains("head") && !diceDiscarded && scripts.enemy.enemyName.text != "Lich") {
                // if player can reroll or discard enemy's die and hints are on
                if (PlayerPrefs.GetString("hints") == "on") {
                    if (scripts.enemy.woundList.Contains("head")) { SetStatusText("note: you can discard enemy's die"); }
                    else if (scripts.enemy.woundList.Contains("chest")) { SetStatusText("note: you can reroll enemy's dice"); }
                }
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
            // get necessary stats
            if (EnemyAttacks()) {
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
                scripts.player.SetPlayerStatusEffect("courage", false);
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
            // RecalculateMaxFor("player");
            scripts.player.targetIndex = 0;
            SetTargetOf("player");
            RecalculateMaxFor("enemy");
            // make sure the player and enemy are aiming at the correct place
            DetermineMove(true);
            // make the next person go again
        }
        scripts.SaveGameData();
        scripts.SavePersistentData();
        yield return scripts.delays[0.45f];
        // small delay
        ClearVariablesAfterRound();
    }

    /// <summary>
    /// Reset all variables used in preparation for the next round.
    /// </summary>
    public void ClearVariablesAfterRound() {
        scripts.player.SetPlayerStatusEffect("fury", false);
        scripts.player.SetPlayerStatusEffect("dodge", false);
        if (!dontRemoveLeechYet) {
            // if we don't want to remove the leech yet (from phylactery, don't do so)
            scripts.player.SetPlayerStatusEffect("leech", false);
        }
        scripts.highlightCalculator.diceTakenByPlayer = 0;
        scripts.itemManager.discardableDieCounter = 0;
        scripts.itemManager.usedAnkh = false;
        scripts.itemManager.usedBoots = false;
        scripts.itemManager.usedHelm = false;
        scripts.diceSummoner.breakOutOfScimitarParryLoop = false;
        usedMace = false;
        scripts.gameData.discardableDieCounter = 0;
        scripts.gameData.usedMace = false;
        scripts.gameData.usedAnkh = false;
        scripts.gameData.usedBoots = false;
        scripts.gameData.usedHelm = false;
        scripts.SaveGameData();
        ClearPotionStats();
        if (scripts.enemy.enemyName.text == "Lich" && scripts.enemy.stamina < 5 && !scripts.enemy.isDead) {
            scripts.enemy.stamina = 5;
            // refresh lich's stamina
            scripts.soundManager.PlayClip("blip");
            // play sound clip
            scripts.enemy.staminaCounter.text = scripts.enemy.stamina.ToString();
        }
        dontRemoveLeechYet = false;
        // set it to be false regardless afterwards, because we only want it to persist for 1 round
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
        scripts.gameData.enemyIsDead = true;
        scripts.SaveGameData();
        yield return scripts.delays[0.55f];
        // short delay
        if (playerOrEnemy == "player") {
            if (scripts.enemy.spawnNum == 0 || scripts.enemy.spawnNum == 1) {
                SetStatusText($"devil twists claws into you... you die");
            }
            else {
                SetStatusText($"{scripts.enemy.enemyName.text.ToLower()} hits you... you die");
            }
            StartCoroutine(PlayDeathAnimation("player"));
            // set status text and play the animation
            //
        }
        else if (playerOrEnemy == "enemy") {
            SetStatusText($"you hit {scripts.enemy.enemyName.text.ToLower()}... he dies");
            StartCoroutine(PlayDeathAnimation("enemy"));
            // set status text and play the animation
        }
        else { print("invalid string passed"); }
        scripts.persistentData.enemiesSlain++;
        scripts.SavePersistentData();
        yield return scripts.delays[0.45f];
        isMoving = false;
    }

    /// <summary>
    /// Play the hit animation for the player or enemy.
    /// </summary>
    /// <param name="playerOrEnemy">Who to play the hit animation for.</param>
    /// <param name="destroyDevilCloak">True to destroy the devil's cloak, false (default) otherwise.</param>
    private IEnumerator PlayHitAnimation(string playerOrEnemy) {
        SpriteRenderer spriteRenderer = playerOrEnemy == "player" ? scripts.player.GetComponent<SpriteRenderer>() : scripts.enemy.GetComponent<SpriteRenderer>();
        // get the proper spriterenderer
        // conditional is true ? yes : no
        Color temp = Color.white;
        temp.a = 0.5f;
        // white with 50% transparency
        spriteRenderer.color = temp;
        for (int i = 0; i < 14; i++) {
            yield return scripts.delays[0.0125f];
            temp.a -= 1f / 28f;
            spriteRenderer.color = temp;
        }
        // fade out
        if (scripts.enemy.spawnNum == 0) {
            // if cloaked devil
            scripts.enemy.spawnNum = 1;
            scripts.gameData.enemyNum = scripts.enemy.spawnNum;
            scripts.enemy.GetComponent<Animator>().runtimeAnimatorController = scripts.enemy.controllers[1];
            if (playerOrEnemy == "enemy") { 
                print("syncing up!");
                scripts.player.GetComponent<Animator>().Rebind();
                scripts.player.GetComponent<Animator>().Update(0f);
                scripts.enemy.GetComponent<Animator>().Rebind();
                scripts.enemy.GetComponent<Animator>().Update(0f);
                // reset devil animation after his cloak shatters, so it stays synced up
            }
            // turn the cloaked into devil
            spriteRenderer.color = temp;
            DisplayWounds();
            // show the wounds (go from [cloaked] to [no wounds]).
        }
        for (int i = 0; i < 28; i++) {
            yield return scripts.delays[0.0125f];
            temp.a += 1f / 28f;
            spriteRenderer.color = temp;
        }
        // fade back in
    }

    /// <summary>
    /// Play the death animation for the player or enemy. Also handle things like clearing potion stats.
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
        if (playerOrEnemy == "enemy" && scripts.enemy.enemyName.text == "Skeleton") { 
            scripts.soundManager.PlayClip("skeletonDeath");
        }
        else { 
            scripts.soundManager.PlayClip("death");
        }
        // play sound clip
        if (scripts.itemManager.PlayerHasWeapon("rapier") && playerOrEnemy == "enemy") { ChangeStaminaOf("player", 3); }
        // if player has rapier and the enemy dies, add to their stamina
        if (playerOrEnemy == "player") {
            scripts.player.GetComponent<SpriteRenderer>().sprite = scripts.player.GetDeathSprite();
            scripts.player.SetPlayerPositionAfterDeath();
            // if player dies, set sprite and proper position
            scripts.tombstoneData.SetTombstoneData();
            // allow the player to retry
        }
        else if (playerOrEnemy == "enemy") {
            scripts.enemy.GetComponent<SpriteRenderer>().sprite = scripts.enemy.GetDeathSprite();
            scripts.enemy.SetEnemyPositionAfterDeath();
            // if enemy dies, set sprite and proper position
            scripts.itemManager.SpawnItems();
            // spawn items
            blackBox.transform.position = onScreen;
            // hide the enemy's stats
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
        if (!(text == statusText.text))
        {
            // if the message is not already displayed
            statusText.text = "";
            // clear the status text
            try { StopCoroutine(coroutine); } catch {}
            // stop any existing status text coroutines
            coroutine = StartCoroutine(StatusTextCoroutine(text));
            // set the status text, and allow for it to be stopped
        }
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
        if (scripts.statSummoner.SumOfStat("green", "player") < 0 && playerOrEnemy == "enemy") { 
            scripts.soundManager.PlayClip("miss");
        }
        else { 
            if (hitOrParry == "hit") {
                if (scripts.player.isDodgy && playerOrEnemy == "player") {
                    scripts.soundManager.PlayClip("miss");
                    // play sound clip
                }
                // player dodges
                else {
                    if (!(playerOrEnemy == "player" && armor || (scripts.enemy.spawnNum == 0 && playerOrEnemy == "enemy"))) { 
                        scripts.soundManager.PlayClip("hit"); 
                    }
                    // play sound clip if conditions apply (not hitting armored player or devil)
                    else if (scripts.enemy.spawnNum == 0 && playerOrEnemy == "enemy") {
                        scripts.soundManager.PlayClip("cloak");
                    }
                    // play cloak shatter here if needed
                    if (showAnimation) {
                        // if showing an animation
                        if (!(playerOrEnemy == "player" && armor)) { 
                            // if NOT player is getting hit and has armor
                            StartCoroutine(PlayHitAnimation(playerOrEnemy)); 
                        }
                        else { scripts.soundManager.PlayClip("armor"); } // play sound clip
                    }
                }
            }
            else if (hitOrParry == "parry") {
                scripts.soundManager.PlayClip("parry");
                // play sound clip
            }
            else { Debug.LogError("invalid string passed"); }
        }
    }

    /// <summary>
    /// Perform actions for the enemy's attack.
    /// </summary>
    /// <param name="enemyAtt">The enemy's attack stat.</param>
    /// <param name="playerDef">The player's parry stat.</param>
    /// <returns>true if the player was killed, false otherwise</returns>
    private bool EnemyAttacks() {
        InitializeVariables(out int playerAim, out int enemyAim, out int playerSpd, out int enemySpd, out int playerAtt, out int enemyAtt, out int playerDef, out int enemyDef);
        bool armor = false;
        // initialize armor as false
        scripts.soundManager.PlayClip("swing");
        // play sound clip
        if (enemyAtt > playerDef) {
            // if enemy is hitting player
            if (scripts.player.isDodgy) { SetStatusText($"{scripts.enemy.enemyName.text.ToLower()} hits you... you dodge"); }
            // if player dodges, notify 
            else {
                if (scripts.itemManager.PlayerHas("armor")) {
                    armor = true;
                    // set armor to true
                    SetStatusText($"{scripts.enemy.enemyName.text.ToLower()} hits you... your armor shatters");
                    scripts.persistentData.armorBroken++;
                    // notify player
                    StartCoroutine(RemoveArmorAfterDelay());
                    scripts.itemManager.Select(scripts.player.inventory, 0, playAudio: false);
                    // select weapon
                }
                else {
                    if (scripts.enemy.target.text != "face") {
                        // if player is not targeting face
                        if (scripts.enemy.target.text.Contains("*")) {
                            // if previously wounded
                            if (scripts.enemy.spawnNum == 0 || scripts.enemy.spawnNum == 1) {
                                SetStatusText($"devil twists claws in your {scripts.enemy.target.text.Substring(1)}!");
                            }
                            SetStatusText($"{scripts.enemy.enemyName.text.ToLower()} hits you, damaging {scripts.enemy.target.text.Substring(1)}!");
                            scripts.persistentData.woundsReceived++;
                            // notify player
                        }
                        else {
                            if (scripts.enemy.spawnNum == 0 || scripts.enemy.spawnNum == 1) {
                                SetStatusText($"devil twists claws in your {scripts.enemy.target.text}!");
                            }
                            if (scripts.player.woundList.Count != 2) {
                                // if player won't die
                                SetStatusText($"{scripts.enemy.enemyName.text.ToLower()} hits you, damaging {scripts.enemy.target.text}!");
                                scripts.persistentData.woundsReceived++;
                                // notify player
                            }
                        }
                        if (scripts.itemManager.PlayerHas("phylactery")) {
                            // if player has phylactery
                            StartCoroutine(GiveLeechAfterDelay());
                            // give them leech buff after waiting, so it looks better
                        }
                    }
                }
            }
            StartCoroutine(DoStuffForAttack("hit", "player", true, armor));
            // play animation + sound for the attack
            if (!(scripts.player.woundList.Contains(scripts.enemy.target.text) || scripts.player.woundList.Contains(scripts.enemy.target.text.Substring(1)) || armor || scripts.player.isDodgy)) {
                // if the player hasn't been injured before, doesn't have armor, and didn't dodge:
                scripts.player.woundList.Add(scripts.enemy.target.text);
                // add the hit
                scripts.gameData.playerWounds = scripts.player.woundList;
                scripts.SaveGameData();
                StartCoroutine(InjuredTextChange(scripts.player.woundGUIElement));
                // make it change
                RecalculateMaxFor("player");
                // reset stuff
                if (scripts.player.woundList.Count > 0) { 
                    // wounds were not healed, so apply them normally
                    return ApplyInjuriesDuringMove(scripts.enemy.target.text, "player");
                }
                else {
                    // wounds were healed, so don't apply them and don't kill the player
                    return false;
                }
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
                scripts.persistentData.attacksParried++;
                scripts.SavePersistentData(); 
            }
            // notify player
        }
        return false;
        // player hasn't died, so return false
    }

    public IEnumerator RemoveArmorAfterDelay() { 
        yield return scripts.delays[0.45f];
        scripts.itemManager.GetPlayerItem("armor").GetComponent<Item>().Remove(armorFade:true);
    }

    public IEnumerator HealAfterDelay() { 
        yield return scripts.delays[1f];
        scripts.turnManager.ChangeStaminaOf("player", -10);
        // StartCoroutine(InjuredTextChange(scripts.player.woundGUIElement));
        scripts.soundManager.PlayClip("blip");
        DisplayWounds();
    }

    private IEnumerator GiveLeechAfterDelay() {
        yield return scripts.delays[0.55f];
        scripts.player.SetPlayerStatusEffect("leech", true);
    }

    private bool PlayerAttacks() {
        InitializeVariables(out int playerAim, out int enemyAim, out int playerSpd, out int enemySpd, out int playerAtt, out int enemyAtt, out int playerDef, out int enemyDef);
        scripts.soundManager.PlayClip("swing");
        // play sound clip
        if (playerAtt > enemyDef) {
            // if player will hit enemy
            if (scripts.statSummoner.SumOfStat("green", "player") < 0) {
                // player doesn't have enough accuracy to hit, so notify
                SetStatusText($"you hit {scripts.enemy.enemyName.text.ToLower()}... you miss");
                // scripts.soundManager.PlayClip("miss");-
                // play sound clip
                StartCoroutine(DoStuffForAttack("hit", "enemy"));
            }
            else {
                if (scripts.player.target.text == "face" && scripts.enemy.enemyName.text != "Lich" || (scripts.enemy.woundList.Count == 2 && !scripts.player.target.text.Contains("*"))) {
                    // enemy is going to die
                    StartCoroutine(DoStuffForAttack("hit", "enemy", false));
                    // play sound but no animation
                }
                else {
                    // enemy will not die
                    StartCoroutine(DoStuffForAttack("hit", "enemy"));
                    // play sound and animation
                    if (scripts.enemy.spawnNum == 0) {
                        SetStatusText("you hit devil... his cloak shatters");
                    }
                    if (scripts.player.target.text.Contains("*")) {
                        SetStatusText($"you hit {scripts.enemy.enemyName.text.ToLower()}, damaging {scripts.player.target.text.Substring(1)}!");
                        scripts.persistentData.woundsInflicted++;
                        scripts.persistentData.woundsInflictedArr[Array.IndexOf(targetArr, scripts.player.target.text.Substring(1))]++;
                    }
                    else {
                        // not injured
                        if (scripts.itemManager.PlayerHasWeapon("maul")) {}
                        // don't say anything for maul (we want it to show that it is an instant kill)
                        else if (scripts.enemy.spawnNum == 0) {}
                        // don't say anything for cloaked devil (shattering his cloak handlded later on)
                        else {
                            SetStatusText($"you hit {scripts.enemy.enemyName.text.ToLower()}, damaging {scripts.player.target.text}!");
                        }
                        scripts.persistentData.woundsInflicted++;
                        scripts.persistentData.woundsInflictedArr[Array.IndexOf(targetArr, scripts.player.target.text)]++;
                        // same as above
                    }
                }
                if (!scripts.player.target.text.Contains("*") && scripts.statSummoner.SumOfStat("green", "player") >= 0) {
                    // if wound was not injured and player has enough accuracy to hit
                    if (scripts.enemy.spawnNum != 0) {
                        scripts.enemy.woundList.Add(scripts.player.target.text);
                        // add the wound
                        scripts.gameData.enemyWounds = scripts.enemy.woundList;
                        scripts.SaveGameData();
                        if (scripts.player.charNum == 2) { 
                            scripts.turnManager.ChangeStaminaOf("player", 1);
                            // increment stamina if on 3rd character
                        }
                        if (scripts.player.isBloodthirsty) {
                            // if player is wounded
                            try { 
                                scripts.player.woundList.Remove(scripts.player.target.text); 
                                scripts.soundManager.PlayClip("blip");
                            } catch {}
                            // try to heal the wound, else don't do anything
                            StartCoroutine(InjuredTextChange(scripts.player.woundGUIElement));
                            // update the text
                            scripts.player.SetPlayerStatusEffect("leech", false);
                            // turn off bloodthirsty
                        }
                        StartCoroutine(InjuredTextChange(scripts.enemy.woundGUIElement));
                        // make the text change
                        RecalculateMaxFor("enemy");
                        // recalculate max
                        return ApplyInjuriesDuringMove(scripts.player.target.text, "enemy");
                        // return if the enemy dies and at the same time apply wounds instantly
                    }
                }
                if (scripts.itemManager.PlayerHasWeapon("maul") && scripts.enemy.spawnNum != 0) { return true; }
                // kill enemy instantly if player has a maul, excluding cloaked devil
            }
        }
        else {
            // enemy will parry
            StartCoroutine(DoStuffForAttack("parry", "enemy"));
            // play animation and sound
            if (playerAtt < 0) { SetStatusText($"you hit {scripts.enemy.enemyName.text.ToLower()}... the attack is too weak"); }
            else if (scripts.statSummoner.SumOfStat("green", "player") < 0) {
                SetStatusText($"you hit {scripts.enemy.enemyName.text.ToLower()}... you miss");
                // scripts.soundManager.PlayClip("miss");
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
    private bool ApplyInjuriesDuringMove(string injury, string appliedTo) {
        StartCoroutine(ApplyInjuriesDuringMoveCoro(injury, appliedTo));
        if (scripts.itemManager.PlayerHasWeapon("maul") && appliedTo == "enemy") { return true; }
        else if (injury == "face" && !(appliedTo == "enemy" && scripts.enemy.enemyName.text == "Lich")) {
            return true;
        }
        else if (appliedTo == "player" && scripts.player.woundList.Count == 3) { return true; }
        else if (appliedTo == "enemy" && scripts.enemy.woundList.Count == 3) { return true; }
        else { return false; }
    }

    private IEnumerator ApplyInjuriesDuringMoveCoro(string injury, string appliedTo) {
        yield return scripts.delays[0.45f];
        // 0.5 instead of 0.55 just in case
        if (appliedTo != "enemy" && appliedTo != "player") { print("invalid string passed into param. appliedTo in ApplyInjuriesDuringMove"); }
        // just checking
        
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
            else if (appliedTo == "enemy" && scripts.enemy.enemyName.text != "Lich") {
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
                foreach (String stat in scripts.itemManager.statArr) { 
                    foreach (Dice dice in scripts.statSummoner.addedPlayerDice[stat]) {
                        dice.transform.position = new Vector2(dice.transform.position.x + scripts.statSummoner.xOffset * -scripts.statSummoner.addedPlayerStamina[stat], dice.transform.position.y);
                        dice.instantiationPos = dice.transform.position;
                    }
                }
                // for every stat (g b r w), shift the die over by the amount of stamina added
                scripts.statSummoner.addedPlayerStamina = new Dictionary<string, int>() {
                    { "green", 0 },
                    { "blue", 0 },
                    { "red", 0 },
                    { "white", 0 },
                };
            }
            else if (appliedTo == "enemy" && scripts.enemy.enemyName.text != "Lich") {
                foreach (String stat in scripts.itemManager.statArr) { 
                    foreach (Dice dice in scripts.statSummoner.addedEnemyDice[stat]) {
                        dice.transform.position = new Vector2(dice.transform.position.x + scripts.statSummoner.xOffset * scripts.statSummoner.addedEnemyStamina[stat], dice.transform.position.y);
                        dice.instantiationPos = dice.transform.position;
                    }
                }
                scripts.statSummoner.addedEnemyStamina = new Dictionary<string, int>() {
                    { "green", 0 },
                    { "blue", 0 },
                    { "red", 0 },
                    { "white", 0 },
                };
                // same as player, except in the opposite direction
            }
            scripts.statSummoner.SummonStats();
        }
        else if (injury == "hand") {
            // if hand, remove white die
            if (appliedTo == "player") {
                StartCoroutine(RemoveDice("white", "player"));
            }
            else if (appliedTo == "enemy" && scripts.enemy.enemyName.text != "Lich") {
                StartCoroutine(RemoveDice("white", "enemy"));
            }
        }
        else if (injury == "armpits") {
            // if armpits, remove red die
            if (appliedTo == "player") {
                StartCoroutine(RemoveDice("red", "player"));
            }
            else if (appliedTo == "enemy" && scripts.enemy.enemyName.text != "Lich") {
                StartCoroutine(RemoveDice("red", "enemy"));
            }
        }
        scripts.statSummoner.SetDebugInformationFor("player");
        scripts.statSummoner.SetDebugInformationFor("enemy");
        // update debug information
    }


    /// <summary>
    /// Coroutine to remove all of a dice type from the player or enemy.
    /// </summary>
    /// <param name="diceType">The type of die to remove.</param>
    /// <param name="removeFrom">Who to remove the die from.</param>
    private IEnumerator RemoveDice(string diceType, string removeFrom) {
        yield return scripts.delays[1f];
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
        if (!scripts.enemy.woundList.Contains("hip") || scripts.enemy.enemyName.text == "Lich") {
            InitializeVariables(out int playerAim, out int enemyAim, out int playerSpd, out int enemySpd, out int playerAtt, out int enemyAtt, out int playerDef, out int enemyDef);
            if (enemyAtt > playerDef) {
                // if enemy can just straight up hit the player
                AddSpeedAndAccuracy(enemyAim, playerSpd, enemySpd, playerAtt, enemyDef);
                // add blue and green if needed
            }
            else if (enemyAtt <= playerDef && enemyAtt + scripts.enemy.stamina > playerDef)
            {
                // if enemy can hit player with the use of stamina
                UseEnemyStaminaOn("red", (playerDef - enemyAtt) + 1);
                // add stamina to hit player
                AddSpeedAndAccuracy(enemyAim, playerSpd, enemySpd, playerAtt, enemyDef);
                // and blue and green if needed
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
        scripts.gameData.enemyStamina = scripts.enemy.stamina;
        scripts.SaveGameData();
    }

    private void AddSpeedAndAccuracy(int enemyAim, int playerSpd, int enemySpd, int playerAtt, int enemyDef)
    {
        if (playerSpd >= enemySpd && playerAtt > enemyDef)
        {
            // if player will attack first
            if (enemySpd + scripts.enemy.stamina > playerSpd)
            {
                // if enemy can attack first
                // add speed to go first
                UseEnemyStaminaOn("blue", (playerSpd - enemySpd) + 1);
            }
        }
        if (enemyAim < 7 && enemyAim + scripts.enemy.stamina > 6)
        {
            // if enemy can target face
            // add accuracy to target face
            UseEnemyStaminaOn("green", 7 - enemyAim);
            scripts.enemy.TargetBest();
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