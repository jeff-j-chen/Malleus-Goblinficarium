using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviou {
    public int diceNum;
    public string diceType;
    public string statAddedTo;
    public bool moveable = true;
    public bool isAttached = false;
    public bool isRerolled = false;
    public string isOnPlayerOrEnemy = "none";
    public Vector3 instantiationPos;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer childSpriteRenderer;
    private Scripts scripts;

    private WaitForSeconds[] rollTimes = { new WaitForSeconds(0.01f), new WaitForSeconds(0.03f), new WaitForSeconds(0.06f), new WaitForSeconds(0.09f), new WaitForSeconds(0.12f), new WaitForSeconds(0.15f), new WaitForSeconds(0.18f), new WaitForSeconds(0.21f), new WaitForSeconds(0.24f), new WaitForSeconds(0.3f) };
    // different times for rolling 

    private void Awake()  {
        // must be in awake, otherwise scripts not set fast enough
        scripts = FindObjectOfType<Scripts>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Color color = spriteRenderer.color;
        // get the color of the sprite of the spriterenderer
        color.a = 0;
        spriteRenderer.color = color;
        // set the alpha of the spriterenderer to be zero
    }

    private void OnMouseDown() {
        // as soon as the mouse button is pressed over
        if (moveable) {
            // if the dice is still moveable
            scripts.soundManager.PlayClip("click");
            // play sound clip
            childSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            // assign the child sprite renderer to be edited 
            scripts.highlightCalculator.ShowValidHighlights(gameObject.GetComponent<Dice>());
            // call the from class HighlightCalculator to show all valid highlights 
        }
        if (!moveable && isAttached && !isRerolled && isOnPlayerOrEnemy == "enemy") {
            // if an action can be performed on the dice (discard, reroll)
            if (!scripts.turnManager.isMoving || (scripts.turnManager.isMoving && scripts.turnManager.actionsAvailable)) {
                // if the situation permits action to occur on the die
                if (scripts.enemy.woundList.Contains("head") && !scripts.turnManager.diceDiscarded) {
                    // if the enemy is wounded in the head and a die has not been discarded yet
                    scripts.soundManager.PlayClip("click");
                    // play sound clip
                    SpriteRenderer numSR = GetComponent<SpriteRenderer>();
                    SpriteRenderer baseSR = transform.GetChild(0).GetComponent<SpriteRenderer>();
                    // assign the necessary sprite renderers
                    Color numTemp = numSR.color;
                    Color baseTemp = baseSR.color;
                    numTemp.a -= 0.33f;
                    numSR.color = numTemp;
                    baseTemp.a -= 0.25f;
                    baseSR.color = baseTemp;
                    // dim the colors of the die
                }
            }
        }
        if (isAttached && isOnPlayerOrEnemy == "player" && scripts.player.isCourageous && !scripts.turnManager.isMoving) {
            // if the player wants to save a die via scroll of courage by discarding the others
            scripts.soundManager.PlayClip("click");
            // play sound clip
            SpriteRenderer numSR = GetComponent<SpriteRenderer>();
            SpriteRenderer baseSR = transform.GetChild(0).GetComponent<SpriteRenderer>();
            // assign the sprite renderers
            Color numTemp = numSR.color;
            Color baseTemp = baseSR.color;
            numTemp.a -= 0.33f;
            numSR.color = numTemp;
            baseTemp.a -= 0.25f;
            baseSR.color = baseTemp;
            // dim the colors of the die
        }
    }

    private void OnMouseDrag() {
        // when the mouse is dragged
        if (moveable) {
            // if the dice can be moved
            spriteRenderer.sortingOrder = 3;
            childSpriteRenderer.sortingOrder = 2;
            // move the dice to the front of the screen
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // get the mouse position via a function
            transform.position = new Vector2(mousePos.x, mousePos.y);
            // assign the transform position of the dice to be where the mouse cursor is
            // no need ot move child as the positions are tied together
        }
    }

    private void OnMouseUp() {
        // when the mouse is released
        if (moveable) {
            // if the dice can be moved
            scripts.soundManager.PlayClip("click1");
            // play sound clip
            scripts.highlightCalculator.SnapToPosition(gameObject.GetComponent<Dice>(), instantiationPos, out moveable, out instantiationPos);
            // attempt to snap the position with a function defined in HighlightCalculator
            transform.position = instantiationPos;
            // set the transform position to be where the instantiation position is (snap back to the selection menu if it didn't get snapped in SnapToPosition)
            scripts.highlightCalculator.HideHighlights();
            // hide all the highlights
            spriteRenderer.sortingOrder = 1;
            childSpriteRenderer.sortingOrder = 0;
            // send the die to the background
        }
        if (!moveable && isAttached && !isRerolled && isOnPlayerOrEnemy == "enemy") {
            // if an action can be performed on the dice (discard, reroll)
            if (!scripts.turnManager.isMoving || scripts.turnManager.isMoving && scripts.turnManager.actionsAvailable) {
                // if the situation allows for an action to be performed
                if (scripts.enemy.woundList.Contains("head") && !scripts.turnManager.diceDiscarded) {
                    // if can discard from head wound
                    DiscardFromEnemy();
                    // discard the die
                }
                else if (scripts.itemManager.discardableDieCounter > 0) {
                    // if can discard from another source
                    if (scripts.turnManager.scimitarParry) { scripts.diceSummoner.breakOutOfScimitarParryLoop = true; }
                    // if source is from scimitarParry, break out of the waiting loop
                    DiscardFromEnemy();
                    // discard from the enemy
                    scripts.itemManager.discardableDieCounter--;
                    // decrease the counter for the number of die 
                }
                else if (scripts.enemy.woundList.Contains("chest")) {
                    // if enemy is wounded in the chest
                    Reroll();
                    // reroll the die
                }
            }
        }
        if (isAttached && isOnPlayerOrEnemy == "player" && scripts.player.isCourageous && scripts.turnManager.discardDieBecauseCourage && !scripts.turnManager.isMoving) {
            // if discarding can and should discard die from courage
            DiscardFromPlayer();
            // do so 
        }
    }

    /// <summary>
    /// Called upon a die to discard it from the enemy.
    /// </summary>
    private void DiscardFromEnemy() {
        scripts.soundManager.PlayClip("click1");
        // play sound clip
        int index = scripts.statSummoner.addedEnemyDice[statAddedTo].IndexOf(this);
        // set variable index to be the index of where the die was
        scripts.turnManager.alterationDuringMove = true;
        scripts.turnManager.diceDiscarded = true;
        // set necessary variables for the turnmanager
        scripts.statSummoner.addedEnemyDice[statAddedTo].Remove(this);
        scripts.diceSummoner.existingDice.Remove(gameObject);
        // remove the die from the lists
        Destroy(gameObject);
        // destroy the gameObject
        List<Dice> diceList = scripts.statSummoner.addedEnemyDice[statAddedTo];
        // assign reference variable for the enemy dice of the same category that just had a dice removed
        for (int i = index; i < diceList.Count; i++) {
            // for every dice at and after the index of where the removed die was
            diceList[i].transform.position = new Vector2(diceList[i].transform.position.x + scripts.statSummoner.diceOffset, diceList[i].transform.position.y);
            // shift the die so it fits properly after one was removed
            diceList[i].GetComponent<Dice>().instantiationPos = diceList[i].transform.position;
            // set the instantiation position for the dice
        }
        scripts.statSummoner.SetDebugInformationFor("enemy");
        // set the debug information
    }

    /// <summary>
    /// Called upon a die to discard it from the player.
    /// </summary>
    public void DiscardFromPlayer() {
        // very similar to discardfromenemy, just doesn't set certain variables
        int index = scripts.statSummoner.addedPlayerDice[statAddedTo].IndexOf(this);
        scripts.statSummoner.addedPlayerDice[statAddedTo].Remove(this);
        scripts.diceSummoner.existingDice.Remove(gameObject);
        Destroy(gameObject);
        List<Dice> diceList = scripts.statSummoner.addedPlayerDice[statAddedTo];
        for (int i = index; i < diceList.Count; i++) {
            diceList[i].transform.position = new Vector2(diceList[i].transform.position.x - scripts.statSummoner.diceOffset, diceList[i].transform.position.y);
            diceList[i].GetComponent<Dice>().instantiationPos = diceList[i].transform.position;
        }
        scripts.statSummoner.SetDebugInformationFor("player");
    }

    /// <summary>
    /// Only for the player, called on when a dice is to be rerolled.
    /// </summary>
    private void Reroll() {
        // self explanatory
        scripts.turnManager.alterationDuringMove = true;
        StartCoroutine(RerollAnimation());
        isRerolled = true;
    }

    /// <summary>
    /// Coroutine for playing the animation and rerolling the die. Sets debug information afterwards.
    /// </summary>
    /// <param name="playSound">Whether or not to play the clicking sound when rerolling.</param>
    public IEnumerator RerollAnimation(bool playSound=true) {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        // assign the spritrenderer reference
        for (int i = 0; i < 10; i++) {
            // 10 times
            yield return rollTimes[i];
            // wait for a set amount of time
            if (playSound) { scripts.soundManager.PlayClip("click"); }
            // play noise clip if necessary
            int randNum = UnityEngine.Random.Range(0, 5);
            // get a random number for the dice 
            spriteRenderer.sprite = scripts.diceSummoner.numArr[randNum].GetComponent<SpriteRenderer>().sprite;
            // asisgn the sprite to be the necessary sprite with the new number
            diceNum = randNum;
            // reassign the die's number
        }
        scripts.statSummoner.SetDebugInformationFor("player");
        scripts.statSummoner.SetDebugInformationFor("enemy");
        scripts.turnManager.RecalculateMaxFor("player");
        scripts.turnManager.RecalculateMaxFor("enemy");
        // set debug information and make sure that the player/enemy isn't aiming at something that they shouldn't be able to hit
    }

    /// <summary>
    /// Coroutine for decreasing the value of a die.
    /// </summary>
    /// <param name="wait">Whether or not to wait 1f before being decreased.</param>
    public IEnumerator DecreaseDiceValue(bool wait = true) {
        if (wait) { yield return scripts.delays[1f]; }
        // wait if necessary
        if (diceNum == 1) { StartCoroutine(FadeOut()); }
        // fade it out if decreasing dice value to 0
        else {
            diceNum--;
            GetComponent<SpriteRenderer>().sprite = scripts.diceSummoner.numArr[diceNum - 1].GetComponent<SpriteRenderer>().sprite;
            // otherwise decrement value and set the proper sprite
        }
        scripts.statSummoner.SetDebugInformationFor("player");
        scripts.statSummoner.SetDebugInformationFor("enemy");
        // set the debug information
    }
    
    /// <summary>
    /// Coroutine for fading out a die.
    /// </summary>
    /// <param name="wait">Whether or not to wait 0.55f before fading out the die.</param>
    /// <param name="decrease">Whether or not to set the debug information after fading out.</param>
    public IEnumerator FadeOut(bool wait=false, bool decrease=false) {
        if (wait) { yield return scripts.delays[0.55f]; }
        // wait if necessary
        SpriteRenderer numSR = GetComponent<SpriteRenderer>();
        SpriteRenderer baseSR = transform.GetChild(0).GetComponent<SpriteRenderer>();
        Color numTemp = numSR.color;
        Color baseTemp = baseSR.color;
        // assign the necessary variables to manipulate the color
        for (int i = 0; i < 40; i++) {
            // 40 times
            yield return scripts.delays[0.005f];
            // wait a small duration
            numTemp.a -= 0.025f;
            numSR.color = numTemp;
            baseTemp.a -= 0.025f;
            baseSR.color = baseTemp;
            // decrease the colors of the die and base
        }
        try { scripts.statSummoner.addedPlayerDice[statAddedTo].Remove(this); } catch { }
        try { scripts.statSummoner.addedEnemyDice[statAddedTo].Remove(this); } catch { }
        // attempt to remove from the player/enemy, this way is much easier than checking
        scripts.diceSummoner.existingDice.Remove(gameObject);
        // remove from existing die list so no errors later on
        Destroy(gameObject);
        // destroy the die
        if (decrease) {
            scripts.statSummoner.SetDebugInformationFor("enemy");
            scripts.statSummoner.SetDebugInformationFor("player");
            // display the debug information if needed
        }
    }
    
    /// <summary>
    /// Coroutine for fading in a die.
    /// </summary>
    public IEnumerator FadeIn() {
        // very similar to fadeout
        yield return scripts.delays[0.001f];
        // wait here or it breaks. I HAVE TRIED REMOVING IT, JUST KEEP THIS
        SpriteRenderer numSR = GetComponent<SpriteRenderer>();
        SpriteRenderer baseSR = transform.GetChild(0).GetComponent<SpriteRenderer>();
        Color numTemp = numSR.color;
        Color baseTemp = baseSR.color;
        baseTemp.a = 0;
        numTemp.a = 0;
        baseSR.color = baseTemp;
        for (int i = 0; i < 40; i++) {
            yield return scripts.delays[0.005f];
            numTemp.a += 0.025f;
            numSR.color = numTemp;
            baseTemp.a += 0.025f;
            baseSR.color = baseTemp;
        }
    }
}