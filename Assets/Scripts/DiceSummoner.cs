using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
public class DiceSummoner : MonoBehaviour
{
    [SerializeField] private GameObject diceBase;
    [SerializeField] public GameObject[] numArr;
    public bool breakOutOfScimitarParryLoop = false;
    private Scripts scripts;
    public List<GameObject> existingDice = new();
    public float yCoord = -5.51f;
    private readonly float[] desktopXCoords = { -2.75f, -1.65f, -0.55f, 0.55f, 1.65f, 2.75f };
    private readonly float[] mobileXCoords = { -2.75f*1.49f, -1.65f*1.49f, -0.55f*1.49f, 0.55f*1.49f, 1.65f*1.49f, 2.75f*1.49f };
    private float[] xCoords;
    private readonly List<Color> generatedTypes = new();
    public int lastNum;
    public string lastType;
    public string lastStat;
    public readonly Vector3 desktopDiceScale = new(1f, 1f, 1f);
    public readonly Vector3 mobileDiceScale = new(1.499f, 1.499f, 1f);
    private Vector3 diceScale;
    // 1.50f causes strange visual bugs on the dice, so 1.499f it is
    
    
    private void Start() {
        scripts = FindObjectOfType<Scripts>();
        if (scripts.mobileMode) {
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
    //         scripts.statSummoner.ResetDiceAndStamina();
    //         SummonDice(false, true);
    //         scripts.statSummoner.SummonStats();
    //     }
    // }

    /// <summary>
    /// Summon the initial round of dice.
    /// </summary>
    public void SummonDice(bool initialSummon, bool newSet) {
        StartCoroutine(SummonAfterFade(initialSummon, newSet));
    }

    /// <summary>
    /// Do not call this coroutine, use SummonDice() instead.
    /// </summary>
    private IEnumerator SummonAfterFade(bool initialSummon, bool newSet) {
        if (newSet) {
            if (scripts.turnManager.dieSavedFromLastRound != null) { 
                Dice fromLastRound = scripts.turnManager.dieSavedFromLastRound.GetComponent<Dice>();
                lastNum = fromLastRound.diceNum;
                lastType = fromLastRound.diceType;
                lastStat = fromLastRound.statAddedTo;
                // need to store them in primitives because the dice and its info will be destroyed
            }
            else { lastNum = -1; }
            // need to Save the die before the delay and summon it afterwards for some reason
            if (!initialSummon) {
                // delay if necessary
                yield return scripts.delays[0.25f];
            }
            existingDice.Clear();
            // clear the list so we have a fresh array
            GenerateDiceTypes();
            for (int i = 0; i < 6; i++) {
                GenerateSingleDie(Random.Range(1, 7), null, "none", null, i, initialSix:true);
                // generate the 6 base die for every round
            }
            if (scripts.itemManager.PlayerHasWeapon("flail")) {
                yield return scripts.delays[0.05f];
                if (scripts.itemManager.PlayerHasLegendary()) {
                    // give the player two red die if wielding a legendary flail, else one
                    GenerateSingleDie(Random.Range(1, 7), "red", "player", "red", initialSix:true);
                    GenerateSingleDie(Random.Range(1, 7), "red", "player", "red", initialSix:true);
                }
                else {
                    GenerateSingleDie(Random.Range(1, 7), "red", "player", "red", initialSix:true);
                }
            }
            if (Save.game.curCharNum == 1) { 
                yield return scripts.delays[0.05f];
                // if player character #2 (maul armor helm), give player yellow die
                scripts.diceSummoner.GenerateSingleDie(Random.Range(1, 7), "yellow", "player", "red", initialSix:true);
            }
            if (scripts.itemManager.PlayerHasWeapon("hatchet") && scripts.itemManager.PlayerHasLegendary()) {
                yield return scripts.delays[0.05f];
                // legendary hatchet lets player start out with yellow die
                scripts.diceSummoner.GenerateSingleDie(Random.Range(1, 7), "yellow", "player", "red", initialSix:true);
            }
            if (scripts.levelManager.level == 4 && scripts.levelManager.sub == 1) {
                // if devil
                yield return scripts.delays[0.05f];
                foreach (string typeToGen in scripts.itemManager.statArr) {
                    // generate a die for every stat
                    yield return scripts.delays[0.05f];
                    Dice created = GenerateSingleDie(Random.Range(1,7), typeToGen, "enemy", typeToGen, initialSix:true);
                    // attach it to the devil
                    if (typeToGen == "red" && scripts.enemy.woundList.Contains("armpits") || typeToGen == "white" && scripts.enemy.woundList.Contains("hand")) {
                        StartCoroutine(created.FadeOut(true));
                    }
                    // devil doesn't get to take its starting red and white if its wounded there
                }
            }
            if (lastNum != -1) {
                yield return scripts.delays[0.05f];
                // if there is a die Saved from last round (from scroll of courage)
                GenerateSingleDie(lastNum, lastType, "player", lastStat, initialSix:true);
                // create the die and add it to the player
            }
        }
        else { 
            existingDice.Clear();
            int initialSpawnCount = 0;
            for (int i = 0; i < Save.game.diceTypes.Count; i++) {
                // for every die
                if (Save.game.dicePlayerOrEnemy[i] == "none") {
                    // if its not attached, its part of the 6 pickup-able
                    GenerateSingleDie(
                        Save.game.diceNumbers[i],
                        Save.game.diceTypes[i],
                        "none",
                        Save.game.diceAttachedToStat[i],
                        initialSpawnCount,
                        initialSix:true
                    );
                    // create the die
                    initialSpawnCount++;
                    // increment the counter (used in generation to calculate offset)
                }
                else {
                    // else its a die attached by some other means (e.g. flail, devil)
                    GenerateSingleDie(
                        Save.game.diceNumbers[i],
                        Save.game.diceTypes[i],
                        Save.game.dicePlayerOrEnemy[i],
                        Save.game.diceAttachedToStat[i],
                        initialSpawnCount,
                        initialSix:true
                    );
                    // so create it and attach directly
                }
            }
        }
        SaveDiceValues();
    }

    /// <summary>
    /// Create a single die with specified variables.
    /// </summary>
    public Dice GenerateSingleDie(int diceNum, string diceType=null, string attachToPlayerOrEnemy="none", string statToAttachTo=null, int i=0, bool initialSix=false,bool isFromMight=false) {
        Vector2 instantiationPos;
        switch (attachToPlayerOrEnemy) {
            // reference variable for the die's attribute
            case "none":
                instantiationPos = new Vector2(xCoords[i], yCoord);
                break;
            // add to the bottom row with correct offset if not attaching
            case "player":
                instantiationPos = new Vector2(scripts.statSummoner.OutermostPlayerX(statToAttachTo), scripts.statSummoner.yCoords[Array.IndexOf(Colors.colorNameArr, statToAttachTo)] - 0.01f);
                break;
            case "enemy":
                instantiationPos = new Vector2(scripts.statSummoner.OutermostEnemyX(statToAttachTo) - scripts.statSummoner.diceOffset, scripts.statSummoner.yCoords[Array.IndexOf(Colors.colorNameArr, statToAttachTo)] - 0.01f);
                break;
            // set the instantiation pos to be by the correct stat with the correct position
            default:
                instantiationPos = new Vector2(0,0);
                break;
        }
        // reference variable for the die's color index relative to scripts.color.coloArr
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
            scripts.statSummoner.AddDiceToPlayer(statToAttachTo, number.GetComponent<Dice>());
            // add it to the array
            number.GetComponent<Dice>().statAddedTo = statToAttachTo;
            if (diceType != "yellow") { number.GetComponent<Dice>().moveable = false; }
            number.GetComponent<Dice>().isAttached = true;
            number.GetComponent<Dice>().isOnPlayerOrEnemy = "player";
            // set necessary attributes
        }
        else if (attachToPlayerOrEnemy == "enemy")  {
            // if attaching to enemy
            scripts.statSummoner.AddDiceToEnemy(statToAttachTo, number.GetComponent<Dice>());
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
        if (attachToPlayerOrEnemy == "player" && isFromMight)  { 
            if (scripts.player.woundList.Contains("guts")) { 
                StartCoroutine(number.GetComponent<Dice>().DecreaseDiceValue());
            }
            if (scripts.player.woundList.Contains("chest") && number.GetComponent<Dice>().diceNum >= 4) { 
                StartCoroutine(number.GetComponent<Dice>().RerollAnimation());
            }
        }
        // add it to the array of existing dice so that functions can be performed on all die at once
        if (attachToPlayerOrEnemy == "player") { scripts.statSummoner.SetDebugInformationFor("player"); }
        else if (attachToPlayerOrEnemy == "enemy") { scripts.statSummoner.SetDebugInformationFor("enemy"); }
        // set the necessary debug information
        if (!initialSix) { SaveDiceValues(); }
        return number.GetComponent<Dice>();
    }

    /// <summary>
    /// Used to Save all the dice properties and values into the player's local file/
    /// </summary>
    public void SaveDiceValues() { 
        StartCoroutine(SaveDiceValuesCoro());
    }

    /// <summary>
    /// Do not call this coroutine, use SaveDiceValues() instead
    /// </summary>
    private IEnumerator SaveDiceValuesCoro() { 
        yield return scripts.delays[0.1f];
        // KEEP THIS DELAY HERE, WITHOUT IT THE DICE WILL NOT SAVE PROPERLY!!!
        Save.game.diceNumbers.Clear();
        Save.game.diceTypes.Clear();
        Save.game.diceAttachedToStat.Clear();
        Save.game.dicePlayerOrEnemy.Clear();
        Save.game.diceRerolled.Clear();
        // make sure to clear everything before saving new data
        foreach (GameObject g in existingDice) {
            // for every existing dice
            Dice dice = g.GetComponent<Dice>();
            Save.game.diceNumbers.Add(dice.diceNum);
            Save.game.diceTypes.Add(dice.diceType);
            Save.game.diceAttachedToStat.Add(dice.statAddedTo);
            Save.game.dicePlayerOrEnemy.Add(dice.isOnPlayerOrEnemy);
            Save.game.diceRerolled.Add(dice.isRerolled);
            // add its info to the info 
        }
        if (scripts.tutorial == null) { Save.SaveGame(); }
        // make sure to Save it
    }

    /// <summary>
    /// Generates a list of dice types for the dice summoned each round.
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
    /// Turn all dice attached to the player into yellow dice (used for fury).
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
    /// Return the number of dice that have yet to be picked.
    /// </summary>
    public int CountUnattachedDice() {
        return existingDice.Count(curObject => curObject.GetComponent<Dice>().isAttached == false);
    }
}