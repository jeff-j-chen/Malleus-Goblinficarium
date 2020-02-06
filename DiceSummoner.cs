using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start() {
        scripts = FindObjectOfType<Scripts>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // needed for testing purposes
            scripts.statSummoner.ResetDiceAndStamina();
            SummonDice(false);
            scripts.statSummoner.SummonStats();
        }
    }

    /// <summary>
    /// Start the summoning of the dice.
    /// </summary>
    /// <param name="initialSummon">If not intial summon => delay 0.25f</param>
    public void SummonDice(bool initialSummon) {
        StartCoroutine(SummonAfterFade(initialSummon));
    }

    private IEnumerator SummonAfterFade(bool initialSummon) {
        if (scripts.turnManager.dieSavedFromLastRound != null) {
            // if there is a die saved from last round (from scroll of courage)
            GenerateSingleDie (
                scripts.turnManager.dieSavedFromLastRound.GetComponent<Dice>().diceNum, 
                scripts.turnManager.dieSavedFromLastRound.GetComponent<Dice>().diceType, 
                "player",
                scripts.turnManager.dieSavedFromLastRound.GetComponent<Dice>().statAddedTo
            );
            // create the die and add it to the player
        }
        if (!initialSummon) {
            // delay is necessary
            yield return scripts.delays[0.25f];
        }
        existingDice.Clear();
        // clear the list so we have a fresh array
        GenerateDiceTypes();
        for (int i = 0; i < 6; i++) {
            GenerateSingleDie(UnityEngine.Random.Range(1, 7), null, "none", null, i);
        }
        if (scripts.itemManager.PlayerHasWeapon("flail")) {
            GenerateSingleDie(UnityEngine.Random.Range(1, 7), "red", "player", "red");
        }
    }

    /// <summary>
    /// Create a single die.
    /// </summary>
    /// <param name="diceNum">The integer of the die to be generated./param>
    /// <param name="diceType">The string of type of the die to be generated. Default null.</param>
    /// <param name="attachToPlayerOrEnemy">If attaching the die to the player, enemy, or none. Default none.</param>
    /// <param name="statToAttachTo">The string of the stat to add the die to. Default null.</param>
    /// <param name="i">The integer representing the number of die spawned if spawned in a chain. Default 0.</param>
    public void GenerateSingleDie(int diceNum, string diceType=null, string attachToPlayerOrEnemy="none", string statToAttachTo=null, int i=0)
    {
        Vector2 instantiationPos;
        // reference variable for the die's attribute.
        if (attachToPlayerOrEnemy == "none") { instantiationPos = new Vector2(xCoords[i], yCoord); }
        // add to the bottom row with correct offset if not attaching.
        else if (attachToPlayerOrEnemy == "player")  { 
            instantiationPos = new Vector2(scripts.statSummoner.OutermostPlayerX(statToAttachTo), scripts.statSummoner.yCoords[Array.IndexOf(scripts.colors.colorNameArr, statToAttachTo)] - 0.01f); 
        }
        else if (attachToPlayerOrEnemy == "enemy") { 
            instantiationPos = new Vector2(scripts.statSummoner.OutermostEnemyX(statToAttachTo) - 1, scripts.statSummoner.yCoords[Array.IndexOf(scripts.colors.colorNameArr, statToAttachTo)] - 0.01f); 
        }
        // set the instant. pos to be by the correct stat with the correct position
        else { instantiationPos = new Vector2(0,0);print("cannot attach to specified thing"); }
        int diceColorIndex;
        // reference variable for the die's color index relative to scripts.color.coloArr
        if (diceType == null) { diceColorIndex = Array.IndexOf(scripts.colors.colorArr, generatedTypes[i]); }
        // generate the respected die from the type list if not given a set die type
        else { diceColorIndex = Array.IndexOf(scripts.colors.colorNameArr, diceType); }
        // else create one of the specified type
        GameObject number = Instantiate(numArr[diceNum - 1], instantiationPos, Quaternion.identity);
        GameObject indivBase = Instantiate(diceBase, instantiationPos, Quaternion.identity);
        SpriteRenderer numSR = number.GetComponent<SpriteRenderer>();
        SpriteRenderer baseSR = indivBase.GetComponent<SpriteRenderer>();
        Color numTemp = numSR.color;
        Color baseTemp = baseSR.color;
        baseTemp.a = 0;
        numTemp.a = 0;
        // instantly make them not visible.
        indivBase.transform.parent = number.transform;
        StartCoroutine(number.GetComponent<Dice>().FadeIn());
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
    }

    /// <summary>
    /// Generates a list of necessary dice types
    /// </summary>
    private void GenerateDiceTypes() {
        generatedTypes.Clear();
        // clear the list so we can start with a new one
        for (int a = 0; a < 3; a++) {
            // create 4 yellow dice
            generatedTypes.Add(scripts.colors.colorArr[4]);
        }
        for (int b = 0; b < 4; b++) {
            // for each dice type that is not yellow
            for (int c = 0; c < 3; c++) {
                // create 3 dice 
                generatedTypes.Add(scripts.colors.colorArr[b]);
            }
        }
        generatedTypes.RemoveAt(3);
        // remove a green
        for (int d = 0; d < 8; d++) {
            // remove dice needed to get just 6
            generatedTypes.RemoveAt(UnityEngine.Random.Range(0, generatedTypes.Count));
        }
    }
}