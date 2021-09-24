using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
public class Enemy : MonoBehaviour {
    [SerializeField] public RuntimeAnimatorController[] controllers;
    [SerializeField] public RuntimeAnimatorController lichDeathController;
    [SerializeField] private Sprite[] icons;
    [SerializeField] private Sprite tombstoneIcon;
    [SerializeField] private Sprite[] deathSprites;
    [SerializeField] private GameObject iconGameobject;
    [SerializeField] public TextMeshProUGUI enemyName;
    [SerializeField] public TextMeshProUGUI woundGUIElement;
    [SerializeField] public TextMeshProUGUI staminaCounter;
    [SerializeField] public TextMeshProUGUI target;
    public List<string> woundList = new();
    public Dictionary<string, int> stats;
    private readonly string[] enemyArr = { "Cloaked", "Devil", "Lich", "Skeleton", "Kobold", "Gog", "Goblin", "Merchant", "Tombstone" };
    private readonly string[] valueArr = { "yellow6", "red6", "white6", "yellow5", "red5", "white5", "yellow4", "red4", "white4", "yellow3", "red3", "white3", "green6", "yellow2", "red2", "white2", "yellow1", "red1", "white1", "green5", "green4", "blue6", "green3", "blue5", "blue4", "green2", "blue3", "green1", "blue2", "blue1" };
    public int stamina = 1;
    public int targetIndex = 0;
    private readonly Vector2 basePosition = new(1.9f, -1.866667f);
    private readonly Vector2 iconPosition = new(6.16667f, 3.333333f);
    private readonly Dictionary<string, Vector2> deathPositions = new() {
        {"Devil", new Vector2(2.24f, -2.25f)},
        {"Lich", new Vector2(1.9f, -1.865334f)},
        {"Skeleton", new Vector2(2.1686665f, -2.117f)},
        {"Kobold", new Vector2(2.03233367f, -2.52f)},
        {"Gog", new Vector2(2.0360003f, -2.52f)},
        {"Goblin", new Vector2(2.1f, -2.52f)},
    };
    private readonly Dictionary<string, Vector2> offsetPositions = new() {
        {"Devil", new Vector2(1.9f, -1.33334f)},
        {"Tombstone", new Vector2(1.9f, -2.118f)},
    };
    private readonly Dictionary<string, int> givenStamina = new() {
        {"11", 1},
        {"12", 1},
        {"13", 2},
        {"21", 2},
        {"22", 2},
        {"23", 3},
        {"31", 3},
        {"32", 3},
        {"33", 4},
        {"41", 5},
    };
    private Scripts scripts;
    public int spawnNum;
    private readonly List<Dice> availableDice = new();
    private readonly List<int> diceValuations = new();
    public readonly int lichStamina = 3;

    private void Start() {
        scripts = FindObjectOfType<Scripts>();
        scripts.turnManager.blackBox.transform.position = scripts.turnManager.offScreen;
        // make sure to show the enemy's stats at the start
        if (scripts.levelManager.level == Save.persistent.tsLevel && scripts.levelManager.sub == Save.persistent.tsSub) {
            // on the tombstone level
            SpawnNewEnemy(8, Save.game.newGame);
            // spawn th tombstone
            scripts.itemManager.lootText.text = "loot:";
            // indicate that the player can loot
            scripts.tombstoneData.SpawnSavedTSItems(true);
            // spawn the Saved tombstone items
            scripts.turnManager.blackBox.transform.position = scripts.turnManager.onScreen;
            // hide the stats (don't fight tombstones)
        }
        else if (scripts.levelManager.sub == 4) {
            // on a merchant level
            SpawnNewEnemy(7, Save.game.newGame);
            // spawn the merchant
            scripts.itemManager.lootText.text = "goods:";
            // indicate that the player should trade
            scripts.tombstoneData.SpawnSavedMerchantItems(true);
            // spawn the Saved merchant items
            scripts.turnManager.blackBox.transform.position = scripts.turnManager.onScreen;
            // hide the stats (don't fight merchants)
        }
        else { 
            // else some fightable enemy
            if (Save.game.newGame) { SpawnNewEnemy(Random.Range(3, 7), true); }
            // if in a new game, spawn a generic enemy
            else { 
                // else resuming game from teh Savefile
                SpawnNewEnemy(Save.game.enemyNum, false); 
                // spawn the same enemy back in
                if (Save.game.enemyIsDead) { 
                    // if resuming after the enemy has been killed
                    scripts.itemManager.lootText.text = "loot:";
                    if (scripts.levelManager.level == 4 || Save.game.enemyNum == 2) { scripts.tombstoneData.SpawnSavedMerchantItems(true); }
                    else { 
                        if (Save.game.floorItemTypes[0] == "weapon") { scripts.tombstoneData.SpawnSavedFloorItems(true);  }
                        else { scripts.tombstoneData.SpawnSavedMerchantItems(true); }
                    }
                    // spawn in the enemy's loot again for the player
                    // devil and lich dont have weapons, so make sure it doesnt bug
                    GetComponent<SpriteRenderer>().sprite = GetDeathSprite();
                    SetEnemyPositionAfterDeath();
                    // show the enemy as dead
                }
                else { scripts.itemManager.lootText.text = ""; }
                // enemy is not dead, so nothing special 
            }
        }
        // spawn an enemy at the start of the round
        iconGameobject.transform.position = iconPosition;
        // set the position of the icon, enemy's is set in spawnnewenemy()
    }

    /// <summary>
    /// Make the enemy target the best wound that it can.
    /// </summary>
    public void TargetBest() {
        // could change this later so that it prioritizes certain wounds rather than just aiming for the highest wound
        targetIndex = 7;
        // start at the end of the array of targetbales
        // for (int i = Mathf.Clamp(scripts.statSummoner.SumOfStat("green", "enemy"), 0, 6); i >= 0; i--) {
        for (int i = Mathf.Clamp(scripts.statSummoner.SumOfStat("green", "enemy"), 0, 7); i >= 0; i--) {
            // print($"enemy target starting at its accuracy stat, {Mathf.Clamp(scripts.statSummoner.SumOfStat("green", "enemy"), 0, 7)}");
           // iterating through the array backwards
            if (!scripts.player.woundList.Contains(scripts.turnManager.targetArr[i])) {
                // if the player does not have the wound
                targetIndex = i;
                break;
                // set target index and break
            }
        }
        if (targetIndex == 7 && scripts.statSummoner.SumOfStat("green", "enemy") < 7) { targetIndex = 0; }
        // this happens if enemy already wounded everything, so it targets face by default (which is wrong)
        scripts.turnManager.SetTargetOf("enemy");
        // set the target of the enemy with the new targetindex
    }

    /// <summary>
    /// Make the enemy pick the dice it deems to be most valuable. 
    /// </summary>
    public void ChooseBestDie() {
        // create a more advanced system to value dice based on context
        availableDice.Clear();
        diceValuations.Clear();
        // clear the data holding lists
        foreach (GameObject dice in scripts.diceSummoner.existingDice) {
            if (dice.GetComponent<Dice>().isAttached == false) {
                availableDice.Add(dice.GetComponent<Dice>());
                diceValuations.Add(Array.IndexOf(valueArr, dice.GetComponent<Dice>().diceType + dice.GetComponent<Dice>().diceNum));
                // add values to the lists if the dice is not attached
            }
        }
        if (availableDice.Count > 0) {
            // if there is more than 1 available to choose
            Dice chosenDie = availableDice[diceValuations.IndexOf(diceValuations.Min())];
            // select the die
            if (woundList.Contains("guts") && enemyName.text != "Lich") { StartCoroutine(chosenDie.DecreaseDiceValue(false)); }
            // decrease if necessary
            chosenDie.isAttached = true;
            chosenDie.moveable = false;
            chosenDie.isOnPlayerOrEnemy = "enemy";
            // assign associated attributes
            if (chosenDie.diceType != "yellow") {
                // if the dice is not a yellow die
                chosenDie.statAddedTo = chosenDie.diceType;
                scripts.statSummoner.AddDiceToEnemy(chosenDie.diceType, chosenDie);
                // add it to the corresponding dice type
                chosenDie.transform.position = new Vector2(scripts.statSummoner.OutermostEnemyX(chosenDie.diceType), scripts.statSummoner.yCoords[Array.IndexOf(Colors.colorNameArr, chosenDie.diceType)] - 0.01f);
                // set the correct transform position
                if ((chosenDie.diceType == "red" && woundList.Contains("armpits")) || (chosenDie.diceType == "white" && woundList.Contains("hand"))) {
                    if (enemyName.text != "Lich") { StartCoroutine(chosenDie.FadeOut(false)); }
                }
                // only fade if its not already going to fade, and player is not facing the lich
            }
            else {
                // if the dice is yellow
                chosenDie.statAddedTo = "red";
                scripts.statSummoner.AddDiceToEnemy("red", chosenDie); 
                // attach to red
                chosenDie.transform.position = new Vector2(scripts.statSummoner.OutermostEnemyX("red"), scripts.statSummoner.yCoords[Array.IndexOf(Colors.colorNameArr, "red")] - 0.01f);
                // set the correct transform position
                if (scripts.itemManager.PlayerHasWeapon("hatchet")) { StartCoroutine(chosenDie.FadeOut(false)); }
                // fade out the dice if the enemy picked a yellow
            }
            TargetBest();
            // make the enemy update its aim (incase it picked up green)
            scripts.statSummoner.SetDebugInformationFor("enemy");
            // set the debug info
        }
    }

    /// <summary>
    /// Spawn an enemy based on its number on in the array.
    /// </summary>
    public void SpawnNewEnemy(int enemyNum, bool isNewEnemy) {
        if (isNewEnemy) {
            // creating new enemy
            Save.game.enemyIsDead = false;
            // make sure enemy is not dead
            float[] temp;
            // array to hold stats
            if (enemyNum == 2) { temp = scripts.levelManager.GenStats("lich"); }
            else if (enemyNum == 0) { temp = scripts.levelManager.GenStats("devil"); }
            else { temp = scripts.levelManager.GenStats(); }
            stats = new Dictionary<string, int> {
                { "green", (int)temp[0] },
                { "blue", (int)temp[1] },
                { "red", (int)temp[2] },
                { "white", (int)temp[3] },
            };
            Save.game.enemyAcc = stats["green"];
            Save.game.enemySpd = stats["blue"];
            Save.game.enemyDmg = stats["red"];
            Save.game.enemyDef = stats["white"];
            // set stats of the enemy
            spawnNum = enemyNum;
            iconGameobject.GetComponent<SpriteRenderer>().sprite = icons[enemyNum];
            // set the sprite for the icon
            GetComponent<Animator>().enabled = true;
            // enable the animator (which is disabled from enemies dying)
            try {GetComponent<Animator>().runtimeAnimatorController = controllers[enemyNum]; } 
            catch { 
                GetComponent<Animator>().runtimeAnimatorController = null; 
                GetComponent<SpriteRenderer>().sprite = icons[enemyNum];
            }
            // try set the controller (none for tombstone), must use runtimeanimationcontroller here
            if (enemyArr[enemyNum] == "Devil" || enemyArr[enemyNum] == "Cloaked") {
                // devil needs to be in a different spot
                transform.position = offsetPositions["Devil"];
            }
            else if (enemyArr[enemyNum] == "Tombstone") {
                // tombstone also needs to be different
                transform.position = offsetPositions["Tombstone"];
                iconGameobject.GetComponent<SpriteRenderer>().sprite = tombstoneIcon;
            }
            else {
                // set normal position
                transform.position = basePosition;
            }
            enemyName.text = enemyArr[enemyNum] == "Cloaked" ? "Devil" : enemyArr[enemyNum];
            // set the name, when spawning the cloaked just set it to be "Devil"
            if (enemyArr[enemyNum] == "Tombstone" || enemyArr[enemyNum] == "Merchant") {
                stamina = 0;
                // tombstone and merchant don't have stamina
            }
            else if (enemyArr[enemyNum] == "Lich") {
                // if lich, has 3 stamina by default
                stamina = lichStamina;
            }
            else {
                // normal enemy 
                stamina = givenStamina[$"{scripts.levelManager.level}{scripts.levelManager.sub}"];
                // assign stamina based on level and sub
            }
            woundList.Clear();
        }
        else { 
            // spawning old enemy
            stats = new Dictionary<string, int> {
                { "green", Save.game.enemyAcc },
                { "blue", Save.game.enemySpd },
                { "red", Save.game.enemyDmg },
                { "white", Save.game.enemyDef },
            };
            spawnNum = enemyNum;
            // enemy inherits its stats from the Save
            try { scripts.turnManager.DisplayWounds(); } catch {}
            // try displaying wounds
            iconGameobject.GetComponent<SpriteRenderer>().sprite = icons[enemyNum];
            // set its sprite
            GetComponent<Animator>().enabled = !Save.game.enemyIsDead;
            // enable/disable the animator, depending on if the enemy is dead or not
            try { GetComponent<Animator>().runtimeAnimatorController = controllers[enemyNum]; } 
            catch { 
                GetComponent<Animator>().runtimeAnimatorController = null; 
                GetComponent<SpriteRenderer>().sprite = icons[enemyNum];
            }
            // try setting the controller, if it doesnt work (tombstone) then set it to null
            if (enemyArr[enemyNum] == "Devil" || enemyArr[enemyNum] == "Cloaked") {
                transform.position = offsetPositions["Devil"];
            }
            else if (enemyArr[enemyNum] == "Tombstone") {
                transform.position = offsetPositions["Tombstone"];
                iconGameobject.GetComponent<SpriteRenderer>().sprite = tombstoneIcon;
            }
            else { transform.position = basePosition; }
            // devil and tombstone have special positions
            enemyName.text = enemyArr[enemyNum] == "Cloaked" ? "Devil" : enemyArr[enemyNum];
            // devil should only be called 'cloaked', not devil
            if (enemyArr[enemyNum] == "Tombstone" || enemyArr[enemyNum] == "Merchant") { stamina = 0; }
            else if (enemyArr[enemyNum] == "Lich") { stamina = 3; }
            else { stamina = Save.game.enemyStamina; }
            woundList = Save.game.enemyWounds;
            // set stamina, show wounds and name
        }
        staminaCounter.text = stamina.ToString();
        // show the amount of stamina the enemy has
        try { scripts.turnManager.SetTargetOf("enemy"); } catch {} 
        try { scripts.turnManager.DisplayWounds(); } catch {}
        Save.game.enemyNum = enemyNum;
        if (scripts.tutorial == null) { Save.SaveGame(); }
    }

    /// <summary>
    /// Discard the player's most valuable dice from them.
    /// </summary>
    public void DiscardBestPlayerDie() {
        StartCoroutine(DiscardBestPlayerDieCoro());
    }

    /// <summary>
    /// Do not call this coroutine, use DiscardBestPlayerDie() instead.
    /// </summary>
    private IEnumerator DiscardBestPlayerDieCoro() {
        print("hello");
        // maybe change this in the future to discard situationally (e.g. discard blue so it can get a first hit if its going to get hit regardless)
        yield return scripts.delays[0.25f];
        // dont discard immediately, otherwise its buggy
        List<Dice> curAvailableDice = new();
        List<int> curDiceValuations = new();
        // create lists to store the information in
        foreach (GameObject curDice in scripts.diceSummoner.existingDice) {
            // for every existing dice
            Dice diceScript = curDice.GetComponent<Dice>();
            if (curDice.GetComponent<Dice>().isOnPlayerOrEnemy == "player") { 
                curAvailableDice.Add(diceScript);
                curDiceValuations.Add(Array.IndexOf(valueArr, diceScript.diceType + diceScript.diceNum));
                // add all the information (script and each die's valuation), only if on player
            }
        }
        Dice chosenDie = curAvailableDice[curDiceValuations.IndexOf(curDiceValuations.Min())];
        // choose the best die (lowest valuation = best)
        chosenDie.DiscardFromPlayer();
        // discard it
    }

    /// <summary>
    /// Gets the death sprite of the enemy.
    /// </summary>
    public Sprite GetDeathSprite() {
        return deathSprites[Array.IndexOf(enemyArr, enemyName.text)];
    }

    /// <summary>
    /// After death, move the enemy's corpse to the correct position on the ground.
    /// </summary>
    public void SetEnemyPositionAfterDeath() {
        transform.position = deathPositions[enemyName.text];
    }
}