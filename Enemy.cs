using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [SerializeField] private RuntimeAnimatorController[] controllers;
    [SerializeField] private Sprite[] icons;
    [SerializeField] private Sprite[] deathSprites;
    [SerializeField] public TextMeshProUGUI enemyName;
    [SerializeField] public TextMeshProUGUI woundGUIElement;
    [SerializeField] public TextMeshProUGUI staminaCounter;
    [SerializeField] public TextMeshProUGUI target;
    public List<string> availableTargets = new List<string>();
    public List<string> woundList = new List<string>();
    public bool isDead = false;
    public Dictionary<string, int> stats;
    private string[] enemyArr = { "Cloaked", "Devil", "Lich", "Skeleton", "Kobold", "Gog", "Goblin", "Merchant" };
    private string[] valueArr = { "yellow6", "red6", "white6", "yellow5", "red5", "white5", "yellow4", "red4", "white4", "yellow3", "red3", "white3", "green6", "yellow2", "red2", "white2", "yellow1", "red1", "white1", "green5", "green4", "blue6", "green3", "blue5", "blue4", "green2", "blue3", "green1", "blue2", "blue1" };
    public int stamina = 1;
    public int targetIndex = 0;
    private Vector2 enemyPos = new Vector2(1.9f, -1.866667f);
    private Vector2 childPos = new Vector2(6.166667f, 3.333333f);

    private Scripts scripts;

    private void Start() {
        scripts = FindObjectOfType<Scripts>();
        SpawnNewEnemy(UnityEngine.Random.Range(3, 7));
        // spawn an enemy at the start of the round
    }

    /// <summary>
    /// Make the enemy target the best wound that it can.
    /// </summary>
    public void TargetBest() {
        // change this later so that it prioritizes certain wounds rather than just aiming for the highest wound
        scripts.turnManager.SetAvailableTargetsOf("enemy");
        // set the list of available targets
        targetIndex = availableTargets.Count - 1;
        // start at the end of the array
        for (int i = availableTargets.Count; i > -1; i--) {
            // iterating from the end to the start
            if (!scripts.player.woundList.Contains(scripts.turnManager.targetArr[i])) {
                // if the player does not have the wound
                targetIndex = i;
                break;
                // set target index and break
            }
        }
        scripts.turnManager.SetTargetOf("enemy");
        // set the target of the enemy with the new targetindex
    }

    /// <summary>
    /// Make the enemy pick the dice it deems to be most valuable. 
    /// </summary>
    public void ChooseBestDie() {
        // create a more advanced system to value dice based on context
        List<Dice> availableDice = new List<Dice>();
        List<int> diceValuations = new List<int>();
        // create lists to hold the data in so that we can compare
        foreach (GameObject dice in scripts.diceSummoner.existingDice) {
            Dice diceScript = dice.GetComponent<Dice>();
            if (diceScript.isAttached == false) {
                availableDice.Add(diceScript);
                diceValuations.Add(Array.IndexOf(valueArr, diceScript.diceType + diceScript.diceNum));
                // add values to the lists if the dice is not attached
            }
        }
        if (availableDice.Count > 0) {
            // if there is more than 1 available to choose
            Dice chosenDie = availableDice[diceValuations.IndexOf(diceValuations.Min())];
            // select the die
            if (woundList.Contains("guts")) { StartCoroutine(chosenDie.DecreaseDiceValue(false)); }
            // decrase if necessary
            chosenDie.isAttached = true;
            chosenDie.moveable = false;
            chosenDie.isOnPlayerOrEnemy = "enemy";
            // assign associated attributes
            if (chosenDie.diceType != "yellow") {
                // if the dice is not a yellow die
                chosenDie.statAddedTo = chosenDie.diceType;
                scripts.statSummoner.AddDiceToEnemy(chosenDie.diceType, chosenDie);
                // add it to the corresponding dice type
                chosenDie.transform.position = new Vector2(scripts.statSummoner.OutermostEnemyX(chosenDie.diceType), scripts.statSummoner.yCoords[Array.IndexOf(scripts.colors.colorNameArr, chosenDie.diceType)] - 0.01f);
                // set the correct transform position
                if ((chosenDie.diceType == "red" && woundList.Contains("armpits")) || (chosenDie.diceType == "white" && woundList.Contains("hand"))) {
                    StartCoroutine(chosenDie.FadeOut(false, true));
                }
                // fade out if necessary 
            }
            else {
                // if the dice is yellow
                chosenDie.statAddedTo = "red";
                scripts.statSummoner.AddDiceToEnemy("red", chosenDie);
                // attach to red
                chosenDie.transform.position = new Vector2(scripts.statSummoner.OutermostEnemyX("red"), scripts.statSummoner.yCoords[Array.IndexOf(scripts.colors.colorNameArr, "red")] - 0.01f);
                // set the correct transform position
                if (scripts.itemManager.PlayerHasWeapon("hatchet")) {
                    StartCoroutine(chosenDie.FadeOut(false, true));
                }
                // fade out if necessary
            }
            TargetBest();
            // if green then retarget
            scripts.statSummoner.SetDebugInformationFor("enemy");
            // set the debug info
        }
    }

    /// <summary>
    /// Spawn an enemy from the array at specified index.
    /// </summary>
    /// <param name="enemyNum"></param>
    public void SpawnNewEnemy(int enemyNum) {
        isDead = false;
        float[] temp;
        if (enemyNum == 2) { temp = scripts.levelManager.GenStats("lich"); }
        else if (enemyNum == 0) { temp = scripts.levelManager.GenStats("devil"); }
        else { temp = scripts.levelManager.GenStats(); }
        stats = new Dictionary<string, int>() {
            { "green", (int)temp[0] },
            { "blue", (int)temp[1] },
            { "red", (int)temp[2] },
            { "white", (int)temp[3] },
        };
        woundList.Clear();
        // woundList = new List<string>() { "armpits" };
        try { scripts.turnManager.DisplayWounds(); } catch {}
        Transform child = transform.GetChild(0);
        child.GetComponent<SpriteRenderer>().sprite = icons[enemyNum];
        GetComponent<Animator>().enabled = true;
        GetComponent<Animator>().runtimeAnimatorController = controllers[enemyNum];
        if (enemyArr[enemyNum] == "Devil" || enemyArr[enemyNum] == "Cloaked") {
            transform.position = new Vector2(enemyPos.x, -1.3333f);
            child.transform.position = new Vector2(childPos.x, childPos.y - 1.3333f / 4f);
        }
        else  {
            transform.position = enemyPos;
            child.transform.position = childPos;
        }
        enemyName.text = enemyArr[enemyNum];
        staminaCounter.text = stamina.ToString();
    }

    /// <summary>
    /// Run calculations to find the most valuable player die and discard it.
    /// </summary>
    public void DiscardBestPlayerDie() {
        List<Dice> availableDice = new List<Dice>();
        List<int> diceValuations = new List<int>();
        // create lists to store the information in
        foreach (GameObject dice in scripts.diceSummoner.existingDice) {
            Dice diceScript = dice.GetComponent<Dice>();
            availableDice.Add(diceScript);
            diceValuations.Add(Array.IndexOf(valueArr, diceScript.diceType + diceScript.diceNum));
            // add all the information (script and each die's valuation)
        }
        Dice chosenDie = availableDice[diceValuations.IndexOf(diceValuations.Min())];
        // choose the best die (lowest valuation = best)
        chosenDie.DiscardFromPlayer();
        // discard it
    }

    /// <summary>
    /// Gets the death sprite of the enemy.
    /// </summary>
    /// <returns>Object Sprite</returns>
    public Sprite GetDeathSprite() {
        return deathSprites[Array.IndexOf(enemyArr, enemyName.text)];
    }

    /// <summary>
    /// After death, move the enemy's corpse to the correct position on the gruond
    /// </summary>
    public void SetEnemyPositionAfterDeath()
    {
        if (enemyName.text == "Devil") {
            MoveBy(-0.073333f - 0.266667f, 0.916667f); }
        else if (enemyName.text == "Lich") {
            MoveBy(0f, -0.001333f); }
        else if (enemyName.text == "Skeleton") {
            MoveBy(-0.135333f - 0.266667f / 2f, 0.250333f); }
        else if (enemyName.text == "Kobold") {
            MoveBy(0.134333333f - 0.266667f, 0.653333f); }
        else if (enemyName.text == "Gog") {
            MoveBy(0.1306667f - 0.266667f, 0.653333f); }
        else if (enemyName.text == "Goblin") {
            MoveBy(-0.2f, 0.653333f); }
        // different enemies need to be shifted different amounts in death. 
        else { print("bad"); }
    }

    private void MoveBy(float x, float y) {
        transform.position = new Vector2(transform.position.x - x, transform.position.y - y);
        // move the enemy the specified amount
        transform.GetChild(0).transform.position = new Vector2(transform.GetChild(0).transform.position.x + x, transform.GetChild(0).transform.position.y + y);
        // move the child in the opposite direction (child is moved when parent is, so we need to move it back into position)
    }
}