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
    [SerializeField] public string[] targetArr = { "chest", "guts", "knee", "hip", "head", "hand", "armpits", "face" };
    [SerializeField] public string[] targetInfoArr = { "reroll any number of enemy's dice", "all enemy's dice suffer a penalty of -1", "your speed is always higher than enemy's", "enemy can't use stamina", "discard one of enemy's die", "enemy can't use white dice", "enemy can't use red dice", "instantaneous death" };
    private Scripts s;
    public bool isMoving = false;
    public bool actionsAvailable = false;
    public bool alterationDuringMove = false;
    public int scimitarParryCount = 0;
    public GameObject dieSavedFromLastRound = null;
    public bool discardDieBecauseCourage = false;
    public bool dontRemoveLeechYet = false;
    private bool refreshingEnemyPlan = false;
    private bool enemyPlanRefreshEnabled = false;

    private void Start() {
        enemyPlanRefreshEnabled = false;
        s = FindObjectOfType<Scripts>();
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
        s.statSummoner.SummonStats();
        DetermineMove(true);
        enemyPlanRefreshEnabled = true;
    }

    /// <summary>
    /// Make the enemy move (if it is their turn).
    /// </summary>
    /// <param name="delay"></param>
    public void DetermineMove(bool delay = false) {
        if (s.statSummoner.SumOfStat("blue", "player") < s.statSummoner.SumOfStat("blue", "enemy") && !(s.itemManager.PlayerHasWeapon("spear"))) {
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
    public IEnumerator EnemyMove(bool delay, bool selectAllRemaining = false) {
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

    /// <summary>
    /// Make sure the player/enemy isn't aiming at a place they can't with their current accuracy. Reassigns the target if they can't.
    /// </summary>
    public void RecalculateMaxFor(string playerOrEnemy) {
        if (playerOrEnemy == "player") {
            if (s.player.targetIndex > s.statSummoner.SumOfStat("green", "player")) {
                s.player.targetIndex = s.statSummoner.SumOfStat("green", "player");
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
    private IEnumerator InjuredTextChange(TextMeshProUGUI text) {
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
        DisplayWounds();
        // update the wound display
        for (int i = 0; i < 40; i++) {
            yield return s.delays[0.005f];
            temp.a += 0.025f;
            text.color = temp;
        }
        // fade back in
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
                if (s.statSummoner.SumOfStat("green", "player") < 0) { s.player.targetIndex = -1; }
                // check in case they swapped to a new weapon and are aiming at an old wound
                if (s.player.targetIndex < 0) {
                    // aiming at none currently
                    if (s.statSummoner.SumOfStat("green", "player") < 0) {
                        // if they are supposed to be aiming at none, the usual
                        s.player.target.text = "none";
                        s.player.targetInfo.text = "not enough accuracy to inflict any wound";
                    }
                    else {
                        // not supposed to be aiming at none, bump it up
                        s.player.targetIndex = 0;
                        s.player.target.text = !s.enemy.woundList.Contains("chest") ? targetArr[0] : "*" + targetArr[0];
                        s.player.targetInfo.text = targetInfoArr[0];
                    }
                }
                else {
                    if (!isMoving) { 
                        // prevents a nasty bug where wounds would not be applied properly due to the * being added
                        if (s.levelManager.level == 4 && s.levelManager.sub == 1) {
                            // if devil
                            if (s.enemy.woundList.Contains(targetArr[s.player.targetIndex])) { s.player.target.text = "*" + targetArr[s.player.targetIndex]; }
                            // add an asterisks if already injured
                            else {
                                if (targetArr[s.player.targetIndex] == "face" && !Save.game.enemyIsDead) {
                                    // can't aim at the devil's face if hes alive, so notify player
                                    s.turnManager.SetStatusText("you cannot aim at his face");
                                    s.player.targetIndex--;
                                }
                                else {
                                    s.player.target.text = targetArr[s.player.targetIndex];
                                }
                            }
                            if (targetArr[s.player.targetIndex] != "face") {
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
            RefreshEnemyPlanIfNeeded();
        }
        else if (playerOrEnemy == "enemy") {
            s.enemy.stamina += amount;
            s.enemy.staminaCounter.text = s.enemy.stamina.ToString();
            RecalculateMaxFor(playerOrEnemy);
            Save.game.enemyStamina = s.enemy.stamina + s.statSummoner.addedEnemyStamina.Values.Sum();
            if (s.tutorial == null) { Save.SaveGame(); }
            // same as above
        }
        else { Debug.LogError("Invalid string passed in to ChangeStaminaAndUpdate() in TurnManager.cs"); }
    }

    /// <summary>
    /// Start the first round of attack.
    /// </summary>
    public void RoundOne() {
        scimitarParryCount = 0;
        // reset the scimitar parry (if set true from a previous round)
        isMoving = true;
        // set the variable to true so that certain actions can check this and make sure actions are not taken when they shouldn't be
        RunEnemyCalculations();
        // make enemy add stamina to stats as necessary
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
                StartCoroutine(Kill("enemy"));
                // execute the proper commands
            }
        }
        else {
            // enemy goes first
            s.player.SetPlayerStatusEffect("dodge", false);
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
        Save.persistent.weaponUses[Array.IndexOf(s.itemManager.weaponNames, s.player.inventory[0].GetComponent<Item>().itemName.Split(' ')[1])]++;
        // increment the # of times the player's current weapon has been used
        Save.SavePersistent();
    }

    private IEnumerator AttemptRegenStaminaAfterDelay() {
        if (s.itemManager.PlayerHasWeapon("dagger") && s.itemManager.PlayerHasLegendary()) {
            // only legendary dagger has stamina regen
            yield return s.delays[0.34f];
            ChangeStaminaOf("player", 1);
            s.soundManager.PlayClip("blip0");
        }
    }

    /// <summary>
    /// Start the second round of attack, with the specified player or enemy attacking.
    /// </summary>
    private IEnumerator RoundTwo(string toMove) {
        isMoving = true;
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
            if (s.enemy.woundList.Contains("chest") && Rerollable() && s.enemy.enemyName.text != "Lich" || Save.game.discardableDieCounter > 0 && s.enemy.enemyName.text != "Lich") {
                // if player can reroll or discard enemy's die and hints are on
                if (PlayerPrefs.GetString(s.HINTS_KEY) == "on") {
                    if (Save.game.discardableDieCounter > 0) { SetStatusText("note: you can discard enemy's die"); }
                    else if (s.enemy.woundList.Contains("chest")) { SetStatusText("note: you can reroll enemy's dice"); }
                    // notify the player
                    actionsAvailable = true;
                    // allow actions
                    for (float i = 2.5f; i > 0; i -= 0.1f) {
                        // 2.5 second time slot
                        if (alterationDuringMove) {
                            // actions handled elsewhere, but if there is an action taken (e.g. discard)
                            i += 1.5f;
                            // increase time slot
                            alterationDuringMove = false;
                            // allow timer to be changed again
                        }
                        yield return s.delays[0.1f];
                        // wait
                    }
                    actionsAvailable = false;
                }
            }
            // get necessary stats
            if (EnemyAttacks()) {
                // if enemy kills player
                StartCoroutine(Kill("player"));
                // play animation
            }
        }
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
            ClearVariablesAfterRound();
            SetTargetOf("player");
            RecalculateMaxFor("enemy");
            
            // stop moving
            s.statSummoner.ResetDiceAndStamina();
            // reset die and stamina
            s.diceSummoner.SummonDice(false, true);
            // summon the die again
            s.statSummoner.SummonStats();
            // summon the stats again
            // RecalculateMaxFor("player");
            s.player.targetIndex = 0;
            // make sure the player and enemy are aiming at the correct place
            DetermineMove(true);
            yield return s.delays[0.35f];
            isMoving = false;
            SetTargetOf("player");
            SetTargetOf("enemy");
            // set the targets after isMoving is set to false, allowing the asterisks to be added without bugs 
        }
        if (s.tutorial == null) { Save.SaveGame(); }
        Save.SavePersistent();
    }

    /// <summary>
    /// Reset all variables used in preparation for the next round.
    /// </summary>
    public void ClearVariablesAfterRound() {
        ClearPotionStats();
        s.player.SetPlayerStatusEffect("fury", false);
        s.player.SetPlayerStatusEffect("dodge", false);
        if (!dontRemoveLeechYet) {
            // if we don't want to remove the leech yet (from phylactery, don't do so)
            s.player.SetPlayerStatusEffect("leech", false);
        }
        s.highlightCalculator.diceTakenByPlayer = 0;
        Save.game.usedMace = false;
        Save.game.usedAnkh = false;
        Save.game.usedBoots = false;
        Save.game.usedHelm = false;
        s.diceSummoner.breakOutOfScimitarParryLoop = false;
        scimitarParryCount = 0;
        Save.game.discardableDieCounter = s.enemy.woundList.Contains("head") ? 1 : 0;
        Save.game.usedMace = false;
        Save.game.usedAnkh = false;
        Save.game.usedBoots = false;
        Save.game.usedHelm = false;
        Save.game.expendedStamina = 0;
        if (s.tutorial == null) { Save.SaveGame(); }
        if (s.enemy.enemyName.text == "Lich" && s.enemy.stamina < s.enemy.lichStamina && !Save.game.enemyIsDead) {
            s.enemy.stamina = s.enemy.lichStamina;
            // refresh lich's stamina
            s.soundManager.PlayClip("blip1");
            // play sound clip
            s.enemy.staminaCounter.text = s.enemy.stamina.ToString();
        }
        dontRemoveLeechYet = false;
        // set it to be false regardless afterwards, because we only want it to persist for 1 round
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
    private IEnumerator Kill(string playerOrEnemy) {
        if (playerOrEnemy == "player") { s.player.isDead = true; }
        else if (playerOrEnemy == "enemy") { Save.game.enemyIsDead = true; }
        // make sure whoever is killed is set to be dead
        Save.game.enemyIsDead = true;
        if (s.tutorial == null) { Save.SaveGame(); }
        yield return s.delays[0.55f];
        // short delay
        if (playerOrEnemy == "player") {
            if (s.enemy.spawnNum is 0 or 1) {
                SetStatusText("devil twists claws into you... you die");
            }
            else {
                SetStatusText($"{s.enemy.enemyName.text.ToLower()} hits you... you die");
            }
            StartCoroutine(PlayDeathAnimation("player"));
            // set status text and play the animation
            //
        }
        else if (playerOrEnemy == "enemy") {
            SetStatusText($"you hit {s.enemy.enemyName.text.ToLower()}... he dies");
            StartCoroutine(PlayDeathAnimation("enemy"));
            // set status text and play the animation
        }
        Save.persistent.enemiesSlain++;
        Save.game.expendedStamina = 0;
        Save.SavePersistent();
        if (s.tutorial == null) { Save.SaveGame(); }
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
            s.soundManager.PlayClip("death");
        }
        // play sound clip
        if (s.itemManager.PlayerHasWeapon("rapier") && playerOrEnemy == "enemy") {
            StartCoroutine(RapierGainAfterDelay());
        }
        // if player has rapier and the enemy dies, add to their stamina based on whether its legendary or not
        if (playerOrEnemy == "player") {
            s.player.GetComponent<SpriteRenderer>().sprite = s.player.GetDeathSprite();
            s.player.SetPlayerPositionAfterDeath();
            // if player dies, set sprite and proper position
            s.tombstoneData.SetTombstoneData();
            // allow the player to retry
            Save.persistent.deaths++;
        }
        else if (playerOrEnemy == "enemy") {
            if (s.enemy.enemyName.text == "Devil") {
                // after killing the devil, the music fades to normal
                s.music.FadeVolume("Through");
            }
            if (s.enemy.enemyName.text == "Lich") {
                s.enemy.GetComponent<Animator>().enabled = true;
                yield return s.delays[0.65f];
                s.soundManager.PlayClip("clang");
            }
            s.enemy.GetComponent<SpriteRenderer>().sprite = s.enemy.GetDeathSprite();
            s.enemy.SetEnemyPositionAfterDeath();
            // if enemy dies, set sprite and proper position
            s.itemManager.SpawnItems();
            // spawn items
            blackBox.transform.localPosition = onScreen;
            // hide the enemy's stats
            if (s.tutorial != null) { s.tutorial.Increment(); }
        }
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

    /// <summary>
    /// Perform actions (sound, animation) for when a player or enemy is hit.
    /// </summary>
    private IEnumerator DoStuffForAttack(string hitOrParry, string playerOrEnemy, bool showAnimation = true, bool armor = false) {
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
                    if (!(playerOrEnemy == "player" && armor || (s.enemy.spawnNum == 0 && playerOrEnemy == "enemy"))) {
                        s.soundManager.PlayClip("hit");
                    }
                    // play sound clip if conditions apply (not hitting armored player or devil)
                    else if (s.enemy.spawnNum == 0 && playerOrEnemy == "enemy") {
                        s.soundManager.PlayClip("cloak");
                    }
                    // play cloak shatter here if needed
                    if (showAnimation) {
                        // if showing an animation
                        if (!(playerOrEnemy == "player" && armor)) {
                            // if NOT player is getting hit and has armor
                            StartCoroutine(PlayHitAnimation(playerOrEnemy));
                        }
                        else { s.soundManager.PlayClip("armor"); } // play sound clip
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
    /// Perform actions for the enemy's attack.
    /// </summary>
    private bool EnemyAttacks() {
        InitializeVariables(out int playerAim, out int enemyAim, out int playerSpd, out int enemySpd, out int playerAtt, out int enemyAtt, out int playerDef, out int enemyDef);
        bool armor = false;
        // initialize armor as false
        s.soundManager.PlayClip("swing");
        // play sound clip
        if (enemyAtt > playerDef) {
            // if enemy is hitting player
            if (Save.game.isDodgy) { SetStatusText($"{s.enemy.enemyName.text.ToLower()} hits you... you dodge"); }
            // if player dodges, notify 
            else {
                if (s.itemManager.PlayerHas("armor")) {
                    armor = true;
                    // set armor to true
                    SetStatusText($"{s.enemy.enemyName.text.ToLower()} hits you... your armor shatters");
                    Save.persistent.armorBroken++;
                    // notify player
                    StartCoroutine(RemoveArmorAfterDelay());
                    s.itemManager.Select(s.player.inventory, 0, playAudio:false);
                    // select weapon
                }
                else {
                    if (s.enemy.target.text != "face") {
                        if (s.enemy.spawnNum is 0 or 1) {
                            SetStatusText($"devil twists claws in your {s.enemy.target.text}!");
                        }
                        if (s.player.woundList.Count != 2) {
                            // if player won't die
                            SetStatusText($"{s.enemy.enemyName.text.ToLower()} hits you, damaging {s.enemy.target.text}!");
                            // notify player
                            Save.persistent.woundsReceived++;
                        }
                        if (s.itemManager.PlayerHas("phylactery")) {
                            // if player has phylactery
                            StartCoroutine(GiveLeechAfterDelay());
                            // give them leech buff after waiting, so it looks better
                            dontRemoveLeechYet = true;
                        }
                    }
                }
            }
            StartCoroutine(DoStuffForAttack("hit", "player", true, armor));
            // play animation + sound for the attack
            if (!(s.player.woundList.Contains(s.enemy.target.text) || s.player.woundList.Contains(s.enemy.target.text) || armor || Save.game.isDodgy)) {
                // if the player hasn't been injured before, doesn't have armor, and didn't dodge:
                if (Save.game.curCharNum == 3 && s.player.woundList.Count < 2 && s.enemy.target.text != "face" && s.player.stamina >= 7) {
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
                    StartCoroutine(InjuredTextChange(s.player.woundGUIElement));
                    if (!s.player.woundList.Contains(s.enemy.target.text)) {
                        s.player.woundList.Add(s.enemy.target.text);
                        // add the hit, but only if the player did not yet have that wound
                        Save.game.playerWounds = s.player.woundList;
                        // make it change
                        RecalculateMaxFor("player");
                        // reset stuff
                        if (s.tutorial == null) { Save.SaveGame(); }
                        if (s.player.woundList.Count > 0) {
                            // wounds were not healed, so apply them normally
                            return ApplyInjuriesDuringMove(s.enemy.target.text, "player");
                        }
                        // wounds were healed, so don't apply them and don't kill the player
                    }
                }
                return false;
                // return that the player did not ie
            }
        }
        else {
            // player parried
            StartCoroutine(DoStuffForAttack("parry", "player"));
            // play sound and animation
            if (enemyAtt < 0) { SetStatusText($"{s.enemy.enemyName.text.ToLower()} hits you... the attack is to weak"); }
            else {
                if (s.itemManager.PlayerHasWeapon("scimitar")) {
                    scimitarParryCount += s.itemManager.PlayerHasLegendary() ? 2 : 1;
                    // legendary scimitar gets 2 discard, regular only gets 1
                }
                // player has parried (this resets at the start of every round so we can do it regardless)
                SetStatusText($"{s.enemy.enemyName.text.ToLower()} hits you... you parry");
                Save.persistent.attacksParried++;
                Save.SavePersistent();
            }
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

    /// <summary>
    /// Coroutine to heal the player's wounds, used by the 4th character.
    /// </summary>
    private IEnumerator HealAfterDelay() {
        yield return s.delays[1f];
        s.turnManager.ChangeStaminaOf("player", -7);
        // bypass changestaminaof and instead directly change the stamina, to differentiate from healing from eating
        if (s.tutorial == null) { Save.SaveGame(); }
        s.soundManager.PlayClip("blip0");
        DisplayWounds();
    }

    /// <summary>
    /// Coroutine to give the player leech after a delay, given by the phylactery.
    /// </summary>
    /// <returns></returns>
    private IEnumerator GiveLeechAfterDelay() {
        yield return s.delays[0.5f];
    }

    /// <summary>
    /// Handles the player's attack, returns true if it was a killing blow.
    /// </summary>
    private bool PlayerAttacks() {
        InitializeVariables(out int playerAim, out int enemyAim, out int playerSpd, out int enemySpd, out int playerAtt, out int enemyAtt, out int playerDef, out int enemyDef);
        s.soundManager.PlayClip("swing");
        // play sound clip
        if (playerAtt > enemyDef) {
            // if player will hit enemy
            if (s.statSummoner.SumOfStat("green", "player") < 0) {
                // player doesn't have enough accuracy to hit, so notify
                SetStatusText($"you hit {s.enemy.enemyName.text.ToLower()}... you miss");
                // s.soundManager.PlayClip("miss");-
                // play sound clip
                StartCoroutine(DoStuffForAttack("hit", "enemy"));
            }
            else {
                if (s.player.target.text == "face" && s.enemy.enemyName.text != "Lich" || (s.enemy.woundList.Count == 2 && !s.player.target.text.Contains("*"))) {
                    // enemy is going to die
                    StartCoroutine(DoStuffForAttack("hit", "enemy", false));
                    // play sound but no animation
                    Save.persistent.woundsInflicted++;
                    Save.persistent.woundsInflictedArr[s.player.targetIndex]++;
                }
                else {
                    // enemy will not die
                    StartCoroutine(DoStuffForAttack("hit", "enemy"));
                    // play sound and animation 
                    if (s.enemy.spawnNum == 0) {
                        SetStatusText("you hit devil... his cloak shatters");
                    }
                    if (s.player.target.text.Contains("*")) {
                        SetStatusText($"you hit {s.enemy.enemyName.text.ToLower()}, damaging {s.player.target.text.Substring(1)}!");
                        Save.persistent.woundsInflicted++;
                        Save.persistent.woundsInflictedArr[s.player.targetIndex]++;
                    }
                    else {
                        // not injured
                        if (s.itemManager.PlayerHasWeapon("maul")) {}
                        // don't say anything for maul (we want it to show that it is an instant kill)
                        else if (s.enemy.spawnNum == 0) {}
                        // don't say anything for cloaked devil (shattering his cloak handled later on)
                        else {
                            SetStatusText($"you hit {s.enemy.enemyName.text.ToLower()}, damaging {s.player.target.text}!");
                        }
                        Save.persistent.woundsInflicted++;
                        Save.persistent.woundsInflictedArr[s.player.targetIndex]++;
                        
                        // same as above
                    }
                }
                if (s.statSummoner.SumOfStat("green", "player") >= 0) {
                    if (Save.game.isBloodthirsty) {
                        // if player is wounded
                        try {
                            StartCoroutine(HealSFXFromPhylactery());
                        } catch {}
                        // try to heal the wound, else don't do anything
                        s.player.SetPlayerStatusEffect("leech", false);
                        // turn off bloodthirsty
                    }
                    if (!s.enemy.woundList.Contains(s.player.target.text) && !s.player.target.text.Contains("*")) {
                        // if wound was not injured and player has enough accuracy to hit
                        if (s.enemy.spawnNum != 0) {
                            s.enemy.woundList.Add(s.player.target.text);
                            // add the wound
                            Save.game.enemyWounds = s.enemy.woundList;
                        }
                        if (s.tutorial == null) { Save.SaveGame(); }
                        if (Save.game.curCharNum == 2) {
                            s.turnManager.ChangeStaminaOf("player", 1);
                            // increment stamina if on 3rd character
                        }
                        StartCoroutine(InjuredTextChange(s.enemy.woundGUIElement));
                        // make the text change
                        RecalculateMaxFor("enemy");
                        // recalculate max
                        if (s.enemy.spawnNum != 0) {
                            // cloaked devil is unaffected by all wounds
                            return ApplyInjuriesDuringMove(s.player.target.text, "enemy");
                        }
                        // return if the enemy dies and at the same time apply wounds instantly
                        
                    }
                }
                if (s.itemManager.PlayerHasWeapon("maul") && s.enemy.spawnNum != 0) { return true; }
                // kill enemy instantly if player has a maul, excluding cloaked devil
            }
        }
        else {
            // enemy will parry
            StartCoroutine(DoStuffForAttack("parry", "enemy"));
            // play animation and sound
            if (playerAtt < 0) { SetStatusText($"you hit {s.enemy.enemyName.text.ToLower()}... the attack is too weak"); }
            else if (s.statSummoner.SumOfStat("green", "player") < 0) {
                SetStatusText($"you hit {s.enemy.enemyName.text.ToLower()}... you miss");
                // s.soundManager.PlayClip("miss");
                // play sound clip
            }
            else { SetStatusText($"you hit {s.enemy.enemyName.text.ToLower()}... he parries"); }
            // depending on the stats, notify player accordingly
        }
        return false;
        // enemy has not died, so return false
    }

    /// <summary>
    /// Coroutine to heal the player after using leech (works for the scroll too).
    /// </summary>
    private IEnumerator HealSFXFromPhylactery() {
        yield return s.delays[0.5f];
        if (s.player.target.text.Contains("*")) {
            if (s.player.woundList.Remove(s.player.target.text.Substring(1))) {
                StartCoroutine(InjuredTextChange(s.player.woundGUIElement));
                s.soundManager.PlayClip("blip0");
            }
        }
        else {
            if (s.player.woundList.Remove(s.player.target.text)) {
                StartCoroutine(InjuredTextChange(s.player.woundGUIElement));
                s.soundManager.PlayClip("blip0");
            }
        }
    }

    /// <summary>
    /// Instantly apply injury effects (such as decreasing die on gut wound).
    /// </summary>
    private bool ApplyInjuriesDuringMove(string injury, string appliedTo) {
        StartCoroutine(ApplyInjuriesDuringMoveCoro(injury, appliedTo));
        // start applying the injuries
        if (s.itemManager.PlayerHasWeapon("maul") && appliedTo == "enemy") { return true; }
        if (injury == "face" && !(appliedTo == "enemy" && s.enemy.enemyName.text == "Lich")) { return true; }
        if (appliedTo == "player" && s.player.woundList.Count == 3) { return true; }
        if (appliedTo == "enemy" && s.enemy.woundList.Count == 3) { return true; }
        return false;
        // return true or false here, based on whether the enemy was killed or not.
    }
    // hello 
    /// <summary>
    /// Do not call this coroutine, use ApplyInjuriesDuringMove() instead.
    /// </summary>
    private IEnumerator ApplyInjuriesDuringMoveCoro(string injury, string appliedTo) {
        yield return s.delays[0.45f];
        // return true immediately if maul
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
                foreach (string stat in s.itemManager.statArr) {
                    foreach (Dice dice in s.statSummoner.addedPlayerDice[stat]) {
                        dice.transform.position = new Vector2(dice.transform.position.x + s.statSummoner.xOffset * -s.statSummoner.addedPlayerStamina[stat], dice.transform.position.y);
                        dice.instantiationPos = dice.transform.position;
                    }
                }
                // for every stat (g b r w), shift the die over by the amount of stamina added
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
        }
        else if (injury == "head") {
            if (appliedTo == "player") {
                s.enemy.DiscardBestPlayerDie();
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
        if (!s.enemy.woundList.Contains("hip") || s.enemy.enemyName.text == "Lich") {
            EnemyAI.ApplyLivePlan(s);
            s.statSummoner.SetDebugInformationFor("enemy");
        }
        Save.game.enemyStamina = s.enemy.stamina + s.statSummoner.addedEnemyStamina.Values.Sum();
        if (s.tutorial == null) { Save.SaveGame(); }
    }

    public void RefreshEnemyPlanIfNeeded() {
        if (refreshingEnemyPlan || s == null || s.enemy == null || s.player == null || s.statSummoner == null) { return; }
        if (!enemyPlanRefreshEnabled) { return; }
        if (isMoving || s.player.isDead || Save.game.enemyIsDead) { return; }
        if (s.enemy.enemyName.text == "Merchant" || s.enemy.enemyName.text == "Blacksmith" || s.enemy.enemyName.text == "Tombstone") { return; }

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
            // if enemy can target face, and the player doesnt have a set of armor
            UseEnemyStaminaOn("green", 7 - enemyAim);
            s.enemy.TargetBest();
            // add accuracy to target face
        }
    }

    /// <summary>
    /// Make the enemy use stamina on a given stat.
    /// </summary>
    private void UseEnemyStaminaOn(string stat, int amount) {
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
