using UnityEngine;
public class StaminaButton : MonoBehaviour {
    private SpriteRenderer spriteRenderer;
    private Scripts s;
    public string stat;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        s = FindFirstObjectByType<Scripts>();
    }

    private void OnMouseUpAsButton() {
        // when mouse button is released over the button
        if (!s.turnManager.isMoving && !s.player.woundList.Contains("hip") && Save.game.curCharNum != 1) {
            // if situation allows
            if (name == "plus(Clone)") {
                // if plus button
                if (s.player.stamina > 0) { // && s.statSummoner.addedPlayerStamina[stat] < 8
                    // if player has the stamina and hasn't put 7 into the stat already
                    ShiftDiceAccordingly(stat, 1);
                    // move the die 
                    s.statSummoner.addedPlayerStamina[stat]++;
                    if (s.tutorial != null && s.tutorial.curIndex == 19 && s.statSummoner.addedPlayerStamina["green"] >= 3) { s.tutorial.Increment(); }
                    Save.game.expendedStamina++;
                    s.turnManager.ChangeStaminaOf("player", -1);
                    // change the variables properly
                    s.statSummoner.SummonStats();
                    s.statSummoner.SetDebugInformationFor("player");
                    s.turnManager.RefreshEnemyPlanIfNeeded();
                    // summon stats and update the debug information
                    Save.persistent.staminaUsed++;
                }
                
            }
            else if (name == "minus(Clone)") {
                if (s.statSummoner.addedPlayerStamina[stat] > 0) {
                    // if there is stamina added
                    ShiftDiceAccordingly(stat, -1);
                    // move the die
                    s.statSummoner.addedPlayerStamina[stat]--;
                    Save.game.expendedStamina--;
                    s.turnManager.ChangeStaminaOf("player", 1);
                    // change the variables properly
                    s.statSummoner.SummonStats();
                    s.statSummoner.SetDebugInformationFor("player");
                    s.turnManager.RefreshEnemyPlanIfNeeded();
                    // summon teh stats and update the debug information
                    Save.persistent.staminaUsed--;
                }
            }
            else {
                Debug.LogError("StaminaButton is not attached to the correct object");
                // something is wrong
            }
            if (s.tutorial == null) { Save.SaveGame(); }
            Save.SavePersistent();
        }
    }

    /// <summary>
    /// Shift dice left or right a given amount, based on stamina added.
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="shiftAmount"></param>
    private void ShiftDiceAccordingly(string stat, int shiftAmount) {
        int modifier = 0;
        if (s.player.stats[stat] + s.itemManager.neckletStats[stat] + s.player.potionStats[stat] + s.statSummoner.addedPlayerStamina[stat] >= 0 && shiftAmount > 0) {
            // total sum is positive and is adding, so shift forwards
            modifier = 1;
        }
        else if (s.player.stats[stat] + s.itemManager.neckletStats[stat] + s.player.potionStats[stat] + s.statSummoner.addedPlayerStamina[stat] >= 1 && shiftAmount < 0) {
            // total sum is > 1 and is subtracting, so shift backwards
            modifier = -1;
        }
        else if (s.player.stats[stat] + s.itemManager.neckletStats[stat] + s.player.potionStats[stat] + s.statSummoner.addedPlayerStamina[stat] >= 0 && shiftAmount < 0) {
            // total sum is 1 or 0 (before adding on the stamina) and is subtracting, so shift forwards
            modifier = 1;
        }
        else if (s.player.stats[stat] + s.itemManager.neckletStats[stat] + s.player.potionStats[stat] + s.statSummoner.addedPlayerStamina[stat] < 0 && shiftAmount > 0) {
            // total sum is negative and is adding, so shift backwards 
            modifier = -1;
        }
        else if (s.player.stats[stat] + s.itemManager.neckletStats[stat] + s.player.potionStats[stat] + s.statSummoner.addedPlayerStamina[stat] < 0 && shiftAmount < 0) {
            // total sum is negative and is subtracting, so shift forwards
            modifier = 1;
        }
        // depending on the situation, set a variable so teh die either shift forwards or backwards
        foreach (Dice dice in s.statSummoner.addedPlayerDice[stat]) {
            // for every die in the stat
            Vector3 position = dice.transform.position;
            position = new Vector2(position.x + s.statSummoner.xOffset * Mathf.Abs(shiftAmount) * modifier, position.y);
            dice.transform.position = position;
            // shift the die correctly
            dice.instantiationPos = position;
            // update the instantiation position of the die
        }
    }

    private void OnMouseEnter() {
        spriteRenderer.color = Colors.hovered;
    }

    private void OnMouseDown() {
        s.soundManager.PlayClip("click0");
        spriteRenderer.color = Colors.clicked;
    }

    private void OnMouseExit() {
        spriteRenderer.color = Color.white;
    }

    private void OnMouseUp() {
        spriteRenderer.color = Color.white;
    }
    // depending on the mouse action, set the correct color for the button
}