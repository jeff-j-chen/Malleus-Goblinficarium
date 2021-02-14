using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HighlightCalculator : MonoBehaviour {
    [SerializeField] private GameObject highlighter;
    private GameObject[] highlights = new GameObject[4];
    private BoxCollider2D[] highlightColliders = new BoxCollider2D[4];
    private Scripts scripts;
    private Vector2 offScreen = new Vector2(0, 20);
    public int diceTakenByPlayer = 0;
    private Vector2 small = new Vector2(10f, 1f);
    private Vector2 large = new Vector2(10f, 10f);


    private void Start()
    {
        scripts = FindObjectOfType<Scripts>();
        HandleHighlightInitiation();
    }
    
    /// <summary>
    /// Creates the highlights to use.
    /// </summary>
    private void HandleHighlightInitiation() {
        for (int i = 0; i < 4; i++) {
            // 4 highlights
            GameObject highlight = Instantiate(highlighter, new Vector2(0, 20 - 0.01f), Quaternion.identity);
            highlights[i] = highlight;
            highlights[i].transform.parent = transform;
            highlightColliders[i] = highlight.GetComponent<BoxCollider2D>();
            
        }
    }

    /// <summary>
    /// Show all the valid highlights depending on the die's type. 
    /// </summary>
    /// <param name="dice"></param>
    public void ShowValidHighlights(Dice dice) {
        if (dice.diceType == "yellow" || scripts.player.isFurious) {
            // if yellow
            MoveOtherDiceAfterYellow(dice.GetComponent<Dice>());
            // shift all die after (if necessary)
            ShowYellowHighlights();
            // show all 4 highlights for the player
            scripts.turnManager.RecalculateMaxFor("player");
            // recalculate the max (if the yellow was moved off accuracy)
        }
        else {
            // not yellow
            if (dice.diceType == "green" && scripts.itemManager.PlayerHasWeapon("dagger")) { ShowSingleHighlight("red"); }
            // if player is using dagger, show highlights for red
            else { ShowSingleHighlight(dice.diceType); }
            // else show highlights for the corresponding colors
        }
    }

    /// <summary>
    /// Show a single highlight based upon a dicetype.
    /// </summary>
    /// <param name="diceType">The dicetype of which highlight to show.</param>
    private void ShowSingleHighlight(string diceType) {
        int diceIndex = Array.IndexOf(scripts.colors.colorNameArr, diceType);
        // get the index of the color relative to the colorName array
        highlights[diceIndex].transform.position = new Vector2(scripts.statSummoner.OutermostPlayerX(scripts.colors.colorNameArr[diceIndex], diceType), scripts.statSummoner.yCoords[diceIndex] - 0.01f);
        highlights[diceIndex].GetComponent<BoxCollider2D>().size = large;
        // move it to the correct position
    }

    /// <summary>
    /// Show all 4 highlights, one for each stat.
    /// </summary>
    private void ShowYellowHighlights() {
        for (int i = 0; i < 4; i++) {
            // 4 highlights
            highlights[i].transform.position = new Vector2(scripts.statSummoner.OutermostPlayerX(scripts.colors.colorNameArr[i]), scripts.statSummoner.yCoords[i] - 0.01f);
            highlights[i].GetComponent<BoxCollider2D>().size = small;
            // move the highlight into position with the corresponding stat.
        }
    }

    /// <summary>
    /// Attempt to snap the die to position (nearest highlight).
    /// </summary>
    /// <param name="dice">The die to attempt to snap.</param>
    /// <param name="curInstantiationPos">The die's current instantiation position attribute.</param>
    /// <param name="moveable">Whether or not the die is moveable. Sent out as a variable.</param>
    /// <param name="instantiationPos">The new instantiation position. Sent out as a variable.</param>
    public void SnapToPosition(Dice dice, Vector3 curInstantiationPos, out bool moveable, out Vector3 instantiationPos) {
        moveable = true;
        // by default make the die still moveable
        instantiationPos = curInstantiationPos;
        // reassign the instantiation position
        Vector3 screenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mousePos = new Vector2(screenPos.x, screenPos.y);
        // get the mouse position as a vector
        HandleAllDiceDrops(dice, ref moveable, ref instantiationPos, mousePos);
    }

    /// <summary>
    /// Shift all die attached to a stat after a yellow one is moved.
    /// </summary>
    /// <param name="dice"></param>
    private void MoveOtherDiceAfterYellow(Dice dice) {
        if (dice.isAttached) {
            // if the die is attached to a stat
            int index = scripts.statSummoner.addedPlayerDice[dice.statAddedTo].IndexOf(dice);
            // get the index of the stat of which the die is attached to
            if (index != -1) {
                // if the die exists (to prevent errors)
                scripts.statSummoner.addedPlayerDice[dice.statAddedTo].Remove(dice);
                // remove the die from the array
                for (int i = index; i < scripts.statSummoner.addedPlayerDice[dice.statAddedTo].Count; i++) {
                    // for each die after the yellow's place
                    scripts.statSummoner.addedPlayerDice[dice.statAddedTo][i].transform.position = new Vector2(scripts.statSummoner.addedPlayerDice[dice.statAddedTo][i].transform.position.x - scripts.statSummoner.diceOffset, scripts.statSummoner.addedPlayerDice[dice.statAddedTo][i].transform.position.y);
                    // shift over to the correct position
                    scripts.statSummoner.addedPlayerDice[dice.statAddedTo][i].GetComponent<Dice>().instantiationPos = scripts.statSummoner.addedPlayerDice[dice.statAddedTo][i].transform.position;
                    // adjust the instantiation position of each accordingly
                }
                if (scripts.statSummoner.addedPlayerDice[dice.statAddedTo].Count > 0) {
                    dice.instantiationPos = new Vector2(scripts.statSummoner.addedPlayerDice[dice.statAddedTo][scripts.statSummoner.addedPlayerDice[dice.statAddedTo].Count - 1].transform.position.x + scripts.statSummoner.diceOffset, scripts.statSummoner.addedPlayerDice[dice.statAddedTo][scripts.statSummoner.addedPlayerDice[dice.statAddedTo].Count - 1].transform.position.y);
                    // create an instatiation position at the end of the die stack
                }
            }
        }
    }

    /// <summary>
    /// Performs various functions to handle die drops of different kinds.
    /// </summary>
    /// <param name="dice">The die script object.</param>
    /// <param name="moveable">Whether or not the die is moveable. Send out.</param>
    /// <param name="instantiationPos">The die's instantiation position. Send out. </param>
    /// <param name="mousePos">The mouse position as a Vector3.</param>
    private void HandleAllDiceDrops(Dice dice, ref bool moveable, ref Vector3 instantiationPos, Vector3 mousePos) {
        foreach (BoxCollider2D collider in highlightColliders) {
            // for each collider
            if (collider.OverlapPoint(mousePos)) {
                // if the mouse over the collider
                instantiationPos = collider.transform.position;
                // set the instantation position to be where the highligth is
                if (dice.diceType == "yellow" || scripts.player.isFurious) {
                    // if the dice is yellow or player is furious
                    if (scripts.player.isFurious) {
                        if (dice.diceType == "green" || dice.diceType == "red" || dice.diceType == "blue") { dice.GetComponent<SpriteRenderer>().color = Color.black; }
                        // change the color of the spots to match the die
                        dice.transform.GetChild(0).GetComponent<SpriteRenderer>().color = scripts.colors.yellow;
                        dice.diceType = scripts.colors.colorNameArr[4];
                        // make the die yellow
                    }
                    HandleYellowDrop(scripts.colors.colorNameArr[Array.IndexOf(highlightColliders, collider)], dice, collider);
                    // do stuff for yellow die
                }
                else {
                    // not yellow
                    if (dice.diceType == "green" && scripts.itemManager.PlayerHasWeapon("dagger")) {
                        // if die is green and the player has dagger
                        HandleNormalDrop("red", dice);
                        // make the die drop for red
                        moveable = false;
                        // die can't be moved
                    }
                    else {
                        HandleNormalDrop(dice.diceType, dice);
                        // drop for the die's type
                        moveable = false;
                        // die can't be moved
                    }
                }
                if (!dice.isAttached) {
                    // if the die was taken from the selection (as opposed to say moving a yellow)
                    diceTakenByPlayer++;
                    // increment counter
                    dice.isAttached = true;
                    dice.isOnPlayerOrEnemy = "player";
                    // set attributes
                    if (scripts.player.woundList.Contains("chest") && dice.diceNum >= 4) {
                        // if injured in chest and die num is 4 or more
                        StartCoroutine(dice.RerollAnimation());
                        // reroll the die, use the coroutine rather than reroll() because reroll is for player alterting others only
                    }
                    if (scripts.statSummoner.SumOfStat("green", "player") >= 0 && scripts.statSummoner.SumOfStat("green", "player") - dice.diceNum < 0) {
                        scripts.turnManager.RecalculateMaxFor("player");
                        // recalculate max for the player if necessary
                    }
                    if (scripts.player.isHasty) {
                        // if the player is affected by hase
                        if (diceTakenByPlayer >= 3) {
                            // if the player has taken 3 die
                            diceTakenByPlayer = 0;
                            scripts.player.SetPlayerStatusEffect("haste", false);
                            // reset and turn status effect off
                            StartCoroutine(scripts.turnManager.EnemyMove(false, true));
                            // make the enemy move and select all remaining die
                        }
                    }
                    else {
                        // not hasty
                        StartCoroutine(scripts.turnManager.EnemyMove(false));
                        // enemy moves normally
                    }
                }
                scripts.statSummoner.SetDebugInformationFor("player");
                // set the debug information
                return;
                // found a collider so no need to check others, just end funciton
            }
        }
        if (dice.diceType == "yellow" && dice.isAttached) {
            scripts.statSummoner.AddDiceToPlayer(dice.statAddedTo, dice);
            // if moving a yellow die make sure the correct actions are performed
        }
    }

    /// <summary>
    /// For when a die that is not yellow is dropped on a collider.
    /// </summary>
    /// <param name="addTo">Which stat to add the die to.</param>
    /// <param name="dice">The die's script.</param>
    private void HandleNormalDrop(string addTo, Dice dice) {
        scripts.statSummoner.AddDiceToPlayer(addTo, dice);
        // add the die to the player
        dice.statAddedTo = addTo;
        // set attributes
        if (scripts.player.woundList.Contains("guts")) {
            StartCoroutine(dice.DecreaseDiceValue(false));
        }
        if (dice.diceType == "red" && scripts.player.woundList.Contains("armpits")) {
            StartCoroutine(dice.FadeOut(decrease:true));
        }
        else if (dice.diceType == "white" && scripts.player.woundList.Contains("hand")) {
            StartCoroutine(dice.FadeOut(decrease:true));
        }
        else if (dice.diceType == "white" && scripts.player.charNum == 2) {
            dice.SetToOne();
        }
        // take actions depending on injuries and die types
    }

    /// <summary>
    /// For when a yellow die is dropped on a collider.
    /// </summary>
    /// <param name="addTo">Which stat to add the die to. </param>
    /// <param name="dice">The die's script.</param>
    /// <param name="collider">Which collider that thed die was dropped on.</param>
    private void HandleYellowDrop(string addTo, Dice dice, BoxCollider2D collider) {
        scripts.statSummoner.AddDiceToPlayer(addTo, dice);
        // add the die to player's die list
        dice.statAddedTo = addTo;
        // set attributes
        if (!dice.isAttached && scripts.player.woundList.Contains("guts")) {
            StartCoroutine(dice.DecreaseDiceValue(false));
        }
        // decrease if gut wound
        scripts.turnManager.SetTargetOf("player");
    }

    /// <summary>
    /// Move all highlights off the screen. 
    /// </summary>
    public void HideHighlights() {
        foreach (GameObject highlight in highlights) {
            // for every highlight
            highlight.transform.position = offScreen;
            // move the highlight offscreen.
        }
    }
}