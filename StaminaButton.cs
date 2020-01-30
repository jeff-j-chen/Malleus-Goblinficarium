using UnityEngine;

public class StaminaButton : MonoBehaviour {
    private SpriteRenderer spriteRenderer;
    private Scripts scripts;
    public string stat;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        scripts = FindObjectOfType<Scripts>();
    }

    private void OnMouseUpAsButton() {
        // when mouse button is released over the button
        if (!scripts.turnManager.isMoving && !scripts.player.woundList.Contains("hip")) {
            // if situation allows
            if (name == "plus(Clone)") {
                // if plus button
                if (scripts.player.stamina > 0) {
                    // if player has the stamina
                    ShiftDiceAccordingly(stat, 1);
                    // move the die 
                    scripts.statSummoner.addedPlayerStamina[stat]++;
                    scripts.turnManager.ChangeStaminaOf("player", -1);
                    // change the variables properly
                    scripts.statSummoner.SummonStats();
                    scripts.statSummoner.SetDebugInformationFor("player");
                    // summon stats and update the debug information
                }
            }
            else if (name == "minus(Clone)") {
                if (scripts.statSummoner.addedPlayerStamina[stat] > 0) {
                    // if there is stamina added
                    ShiftDiceAccordingly(stat, -1);
                    // move the die
                    scripts.statSummoner.addedPlayerStamina[stat]--;
                    scripts.turnManager.ChangeStaminaOf("player", 1);
                    // change the variables properly
                    scripts.statSummoner.SummonStats();
                    scripts.statSummoner.SetDebugInformationFor("player");
                    // summon teh stats and update the debug information
                }
            }
            else {
                Debug.LogError("StaminaButton is not attached to the correct object");
                // something is wrong
            }
        }
    }

    private void ShiftDiceAccordingly(string stat, int shiftAmount) {
        int modifier = 0;
        if (scripts.player.stats[stat] + scripts.itemManager.neckletStats[stat] + scripts.player.potionStats[stat] + scripts.statSummoner.addedPlayerStamina[stat] >= 0 && shiftAmount > 0) {
            // total sum is positive and is adding, so shift forwards
            modifier = 1;
        }
        else if (scripts.player.stats[stat] + scripts.itemManager.neckletStats[stat] + scripts.player.potionStats[stat] + scripts.statSummoner.addedPlayerStamina[stat] >= 1 && shiftAmount < 0) {
            // total sum is > 1 and is subtracting, so shift backwards
            modifier = -1;
        }
        else if (scripts.player.stats[stat] + scripts.itemManager.neckletStats[stat] + scripts.player.potionStats[stat] + scripts.statSummoner.addedPlayerStamina[stat] >= 0 && shiftAmount < 0) {
            // total sum is 1 or 0 (before adding on the stamina) and is subtracting, so shift forwards
            modifier = 1;
        }
        else if (scripts.player.stats[stat] + scripts.itemManager.neckletStats[stat] + scripts.player.potionStats[stat] + scripts.statSummoner.addedPlayerStamina[stat] < 0 && shiftAmount > 0) {
            // total sum is negative and is adding, so shift backwards 
            modifier = -1;
        }
        else if (scripts.player.stats[stat] + scripts.itemManager.neckletStats[stat] + scripts.player.potionStats[stat] + scripts.statSummoner.addedPlayerStamina[stat] < 0 && shiftAmount < 0) {
            // total sum is negative and is subtracting, so shift forwards
            modifier = 1;
        }
        // depending on the situation, set a variable so teh die either shift forwards or backwards
        foreach (Dice dice in scripts.statSummoner.addedPlayerDice[stat]) {
            // for every die in the stat
            dice.transform.position = new Vector2(dice.transform.position.x + scripts.statSummoner.xOffset * Mathf.Abs(shiftAmount) * modifier, dice.transform.position.y);
            // shift the die correctly
            dice.instantiationPos = dice.transform.position;
            // update the instantiation position of the die
        }
    }

    private void OnMouseEnter() {
        spriteRenderer.color = scripts.colors.hovered;
    }

    private void OnMouseDown() {
        scripts.soundManager.PlayClip("click");
        spriteRenderer.color = scripts.colors.clicked;
    }

    private void OnMouseExit() {
        spriteRenderer.color = Color.white;
    }

    private void OnMouseUp() {
        spriteRenderer.color = Color.white;
    }
    // depnding on the mouse action, set the correct color for the button
}