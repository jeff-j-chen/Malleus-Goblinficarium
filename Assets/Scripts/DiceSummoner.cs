using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DiceSummoner : MonoBehaviour
{
    [SerializeField] private GameObject diceBase;
    [SerializeField] public GameObject[] numArr;
    public bool breakOutOfScimitarParryLoop = false;
    private Scripts scripts;
    public List<GameObject> existingDice = new List<GameObject>();
    public float yCoord = -5.51f;
    private float[] xCoords = new float[] { -2.75f, -1.65f, -0.55f, 0.55f, 1.65f, 2.75f };
    private List<Color> generatedTypes = new List<Color>();
    public int lastNum;
    public string lastType;
    public string lastStat;
    
    private void Start() {
        scripts = FindObjectOfType<Scripts>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            // testing purposes only, use to refresh this given set of dice
            scripts.statSummoner.ResetDiceAndStamina();
            SummonDice(false, true);
            scripts.statSummoner.SummonStats();
        }
    }

    /// <summary>
    /// Summon the intiial round of dice.
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
            // need to save the die before the delay and summon it afterwards for some reason
            if (!initialSummon) {
                // delay if necessary
                yield return scripts.delays[0.25f];
            }
            existingDice.Clear();
            // clear the list so we have a fresh array
            GenerateDiceTypes();
            for (int i = 0; i < 6; i++) {
                GenerateSingleDie(UnityEngine.Random.Range(1, 7), null, "none", null, i, initialSix:true);
                // generate the 6 base die for every round
            }
            if (scripts.itemManager.PlayerHasWeapon("flail")) {
                // give the player a red die if wielding a flail
                GenerateSingleDie(UnityEngine.Random.Range(1, 7), "red", "player", "red", initialSix:true);
            }
            if (scripts.player.charNum == 1) { 
                // if player character #2 (maul armor helm), give player yellow die
                scripts.diceSummoner.GenerateSingleDie(UnityEngine.Random.Range(1, 7), "yellow", "player", "red", initialSix:true);
            }
            if (scripts.levelManager.level == 4 && scripts.levelManager.sub == 1) {
                // if devil
                foreach (string typeToGen in scripts.itemManager.statArr) {
                    // generate a die for every stat
                    GenerateSingleDie(UnityEngine.Random.Range(1,7), typeToGen, "enemy", typeToGen, initialSix:true);
                    // attach it to the devl
                }
            }
            if (lastNum != -1) {
                // if there is a die saved from last round (from scroll of courage)
                GenerateSingleDie (lastNum, lastType, "player", lastStat, initialSix:true);
                // create the die and add it to the player
            }
        }
        else { 
            existingDice.Clear();
            int initialSpawnCount = 0;
            for (int i = 0; i < scripts.gameData.diceTypes.Count; i++) {
                // for every die
                if (scripts.gameData.dicePlayerOrEnemy[i] == "none") {
                    // if its not attached, its part of the 6 pickupable
                    GenerateSingleDie(
                        scripts.gameData.diceNumbers[i],
                        scripts.gameData.diceTypes[i],
                        "none",
                        scripts.gameData.diceAttachedToStat[i],
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
                        scripts.gameData.diceNumbers[i],
                        scripts.gameData.diceTypes[i],
                        scripts.gameData.dicePlayerOrEnemy[i],
                        scripts.gameData.diceAttachedToStat[i],
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
    public void GenerateSingleDie(int diceNum, string diceType=null, string attachToPlayerOrEnemy="none", string statToAttachTo=null, int i=0, bool initialSix=false) {
        Vector2 instantiationPos;
        // reference variable for the die's attribute
        if (attachToPlayerOrEnemy == "none") { instantiationPos = new Vector2(xCoords[i], yCoord); }
        // add to the bottom row with correct offset if not attaching
        else if (attachToPlayerOrEnemy == "player")  { 
            instantiationPos = new Vector2(scripts.statSummoner.OutermostPlayerX(statToAttachTo), scripts.statSummoner.yCoords[Array.IndexOf(scripts.colors.colorNameArr, statToAttachTo)] - 0.01f); 
        }
        else if (attachToPlayerOrEnemy == "enemy") { 
            instantiationPos = new Vector2(scripts.statSummoner.OutermostEnemyX(statToAttachTo) - 1, scripts.statSummoner.yCoords[Array.IndexOf(scripts.colors.colorNameArr, statToAttachTo)] - 0.01f); 
        }
        // set the instantiation pos to be by the correct stat with the correct position
        else { instantiationPos = new Vector2(0,0);print("cannot attach to specified thing"); }
        int diceColorIndex;
        // reference variable for the die's color index relative to scripts.color.coloArr
        if (diceType is null) { diceColorIndex = Array.IndexOf(scripts.colors.colorArr, generatedTypes[i]); }
        // generate the respected die from the type list if not given a set die type
        else { diceColorIndex = Array.IndexOf(scripts.colors.colorNameArr, diceType); }
        // else create one of the specified type
        GameObject number = Instantiate(numArr[diceNum - 1], instantiationPos, Quaternion.identity);
        GameObject indivBase = Instantiate(diceBase, instantiationPos, Quaternion.identity);
        // create gameobjects
        indivBase.transform.parent = number.transform;
        number.transform.parent = transform;
        // parent the base to the number and the number to this (the manager)
        number.GetComponent<Dice>().diceNum = diceNum;
        number.GetComponent<Dice>().diceType = scripts.colors.colorNameArr[diceColorIndex];
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
        if (scripts.colors.colorArr[diceColorIndex] == scripts.colors.white || scripts.colors.colorArr[diceColorIndex] == scripts.colors.yellow) {
            // give the number the correct color relative to the base (e.g. black with yellow or white with red)
            number.GetComponent<SpriteRenderer>().color = Color.black;
        }
        indivBase.GetComponent<SpriteRenderer>().color = scripts.colors.colorArr[diceColorIndex];
        // set the color of the base
        // fade in the die
        existingDice.Add(number);
        // add it to the array of existing dice so that functions can be performed on all die at once
        if (attachToPlayerOrEnemy == "player") { scripts.statSummoner.SetDebugInformationFor("player"); }
        else if (attachToPlayerOrEnemy == "enemy") { scripts.statSummoner.SetDebugInformationFor("enemy"); }
        // set the necessary debug information
        if (!initialSix) { SaveDiceValues(); }
    }

    /// <summary>
    /// Used to save all the dice properties and values into the player's local file/
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
        scripts.gameData.diceNumbers.Clear();
        scripts.gameData.diceTypes.Clear();
        scripts.gameData.diceAttachedToStat.Clear();
        scripts.gameData.dicePlayerOrEnemy.Clear();
        scripts.gameData.diceRerolled.Clear();
        // make sure to clear everything before saving new data
        for (int i = 0; i < existingDice.Count; i++) { 
            // for every existing dice
            Dice dice = existingDice[i].GetComponent<Dice>();
            scripts.gameData.diceNumbers.Add(dice.diceNum);
            scripts.gameData.diceTypes.Add(dice.diceType);
            scripts.gameData.diceAttachedToStat.Add(dice.statAddedTo);
            scripts.gameData.dicePlayerOrEnemy.Add(dice.isOnPlayerOrEnemy);
            scripts.gameData.diceRerolled.Add(dice.isRerolled);
            // add its info to the info 
        }
        scripts.SaveGameData();
        // make sure to save it
    }

    /// <summary>
    /// Generates a list of dice types for the dice summoned each round.
    /// </summary>
    private void GenerateDiceTypes() {
        generatedTypes.Clear();
        // clear the list so we can start with a new one
        for (int a = 0; a < 3; a++) {
            // create 3 yellow dice
            generatedTypes.Add(scripts.colors.colorArr[4]);
        }
        for (int b = 0; b < 4; b++) {
            // for each dice type that is not yellow
            for (int c = 0; c < 3; c++) {
                // create 3 dice 
                generatedTypes.Add(scripts.colors.colorArr[b]);
            }
        }
        generatedTypes.RemoveAt(4);
        // remove a green
        for (int d = 0; d < 8; d++) {
            // remove dice needed to get just 6
            generatedTypes.RemoveAt(UnityEngine.Random.Range(0, generatedTypes.Count));
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
                dice.GetComponent<Dice>().transform.GetChild(0).GetComponent<SpriteRenderer>().color = scripts.colors.yellow;
                dice.GetComponent<Dice>().diceType = scripts.colors.colorNameArr[4];
                // make the die yellow
                dice.GetComponent<Dice>().moveable = true;
                // allow for moving the die around
            }
        }
        SaveDiceValues();
    }
}