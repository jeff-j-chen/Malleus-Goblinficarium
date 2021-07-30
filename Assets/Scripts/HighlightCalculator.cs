using System;
using UnityEngine;
public class HighlightCalculator : MonoBehaviour {
    [SerializeField] private GameObject highlighter;
    private readonly GameObject[] highlights = new GameObject[4];
    private readonly BoxCollider2D[] highlightColliders = new BoxCollider2D[4];
    private Scripts scripts;
    private readonly Vector2 offScreen = new(0, 20);
    public int diceTakenByPlayer = 0;
    private readonly Vector2 small = new(10f, 1f);
    private readonly Vector2 large = new(10f, 10f);

    private void Start() {
        scripts = FindObjectOfType<Scripts>();
        HandleHighlightInitiation();
    }
    
    /// <summary>
    /// Creates the highlights for the player to drop dice onto.
    /// </summary>
    private void HandleHighlightInitiation() {
        for (int i = 0; i < 4; i++) {
            // 4 highlights
            GameObject highlight = Instantiate(highlighter, new Vector2(0, 20 - 0.01f), Quaternion.identity);
            highlights[i] = highlight;
            highlights[i].transform.parent = transform;
            highlightColliders[i] = highlight.GetComponent<BoxCollider2D>();
            // put the data of each highlight in
        }
    }

    /// <summary>
    /// Show all the valid highlights, depending on the die's type. 
    /// </summary>
    public void ShowValidHighlights(Dice dice) {
        if (dice.diceType == "yellow" || scripts.player.isFurious) {
            // if yellow
            MoveOtherDiceAfterYellow(dice.GetComponent<Dice>());
            // shift all die after into place
            ShowYellowHighlights();
            // show all 4 highlights for the player
            scripts.turnManager.RecalculateMaxFor("player");
            // recalculate the max (if the yellow was moved off accuracy)
        }
        else {
            // not yellow
            if (dice.diceType == "green" && scripts.itemManager.PlayerHasWeapon("dagger")) { ShowSingleHighlight("red"); }
            // if player is using dagger, show highlights for red when picking green dice
            else if (dice.diceType == "white" && Save.game.curCharNum == 3) { ShowSingleHighlight("red"); }
            // if player is 4th char, show highlights for red when picking white dice
            else { ShowSingleHighlight(dice.diceType); }
            // no special conditions, so show highlights for the corresponding colors
        }
    }

    /// <summary>
    /// Show a single highlight for a given typ eof dice..
    /// </summary>
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
            // move the highlight into position with the corresponding stat
        }
    }

    /// <summary>
    /// Attempt to snap the die to position (nearest highlight).
    /// </summary>
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
                    // create an instantiation position at the end of the die stack
                }
            }
        }
    }

    /// <summary>
    /// Handles dice drops of all kinds.
    /// </summary>
    private void HandleAllDiceDrops(Dice dice, ref bool moveable, ref Vector3 instantiationPos, Vector3 mousePos) {
        foreach (BoxCollider2D curCollider in highlightColliders) {
            // for each collider
            if (curCollider.OverlapPoint(mousePos)) {
                // if the mouse over the collider
                instantiationPos = curCollider.transform.position;
                // set the instantiation position to be where the highlight is
                if (dice.diceType == "yellow" || scripts.player.isFurious) {
                    // if the dice is yellow or player is furious
                    if (scripts.player.isFurious) {
                        if (dice.diceType is "green" or "red" or "blue") { dice.GetComponent<SpriteRenderer>().color = Color.black; }
                        // change the color of the spots to match the die
                        dice.transform.GetChild(0).GetComponent<SpriteRenderer>().color = scripts.colors.yellow;
                        dice.diceType = scripts.colors.colorNameArr[4];
                        // make the die yellow
                    }
                    HandleYellowDrop(scripts.colors.colorNameArr[Array.IndexOf(highlightColliders, curCollider)], dice);
                    // do stuff for yellow die
                }
                else {
                    // not yellow
                    if (dice.diceType == "green" && scripts.itemManager.PlayerHasWeapon("dagger")) {
                        // if die is green and the player has dagger
                        HandleNormalDrop("red", dice);
                        // make the die drop for red
                        moveable = false;
                    }
                    else if (dice.diceType == "white" && Save.game.curCharNum == 3) { 
                        // white dice buff damage on 4th character
                        HandleNormalDrop("red", dice);
                        moveable = false;
                    }
                    else {
                        HandleNormalDrop(dice.diceType, dice);
                        // drop for the die's type
                        moveable = false;
                    }
                    // moveable it put out using the 'ref' word, so it changes the moveability of the dice
                }
                if (!dice.isAttached) {
                    // if the die was taken from the selection (as opposed to moving a yellow)
                    diceTakenByPlayer++;
                    // increment counter
                    dice.isAttached = true;
                    dice.isOnPlayerOrEnemy = "player";
                    // set attributes
                    if (scripts.player.woundList.Contains("chest") && dice.diceNum >= 4) {
                        // if injured in chest and die num is 4 or more
                        StartCoroutine(dice.RerollAnimation());
                        // reroll the die, use the coroutine rather than reroll() because reroll is for player altering others only
                    }
                    if (scripts.player.woundList.Contains("head") && diceTakenByPlayer >= 3) {
                        // if injured in head the player has taken 3 dice 
                        scripts.enemy.DiscardBestPlayerDie();
                        // note that this calls a coroutine which has a slightly delay, avoiding a weird bug with discarding too early
                    }
                    if (scripts.statSummoner.SumOfStat("green", "player") >= 0 && scripts.statSummoner.SumOfStat("green", "player") - dice.diceNum < 0) {
                        scripts.turnManager.RecalculateMaxFor("player");
                        // recalculate max for the player if necessary
                    }
                    if (scripts.player.isHasty) {
                        // if the player is affected by haste
                        if (diceTakenByPlayer >= 3) {
                            // allow the player to take 3 dice
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
                    if (scripts.tutorial != null) {
                        if (scripts.diceSummoner.CountUnattachedDice() == 4 || scripts.diceSummoner.CountUnattachedDice() == 0) {
                            scripts.tutorial.Increment();
                        }
                    }
                }
                scripts.statSummoner.SetDebugInformationFor("player");
                // set the debug information
                return;
                // found a collider so no need to check others, just end function
            }   
        }
    }

    /// <summary>
    /// Handle a normal dice being dropped onto a highlight.
    /// </summary>
    private void HandleNormalDrop(string addTo, Dice dice) {
        scripts.statSummoner.AddDiceToPlayer(addTo, dice);
        // add the die to the player
        dice.statAddedTo = addTo;
        // set attributes
        if (scripts.player.woundList.Contains("guts")) { StartCoroutine(dice.DecreaseDiceValue(false)); }
        if (dice.diceType == "red" && scripts.player.woundList.Contains("armpits")) { StartCoroutine(dice.FadeOut()); }
        else if (dice.diceType == "white" && scripts.player.woundList.Contains("hand")) { StartCoroutine(dice.FadeOut()); }
        else if (dice.diceType == "white" && Save.game.curCharNum == 2) { dice.SetToOne(); }
        // take actions depending on injuries and die types, and character numbers
        // chest wounds are handled elsewhere, so dont worry about it here
        scripts.diceSummoner.SaveDiceValues();
    }

    /// <summary>
    /// Handle a yellow dice being dropped on to a highlight.
    /// </summary>
    private void HandleYellowDrop(string addTo, Dice dice) {
        scripts.statSummoner.AddDiceToPlayer(addTo, dice);
        // add the die to player's die list
        dice.statAddedTo = addTo;
        // set attributes
        if (!dice.isAttached && scripts.player.woundList.Contains("guts")) { StartCoroutine(dice.DecreaseDiceValue(false)); }
        // decrease if gut wound
        scripts.turnManager.SetTargetOf("player");
        // update the player's targets
        scripts.diceSummoner.SaveDiceValues();
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