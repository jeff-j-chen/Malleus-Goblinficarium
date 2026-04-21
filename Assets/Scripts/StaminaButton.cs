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
                    s.statSummoner.addedPlayerStamina[stat]++;
                    if (s.tutorial != null && s.tutorial.curIndex == 19 && s.statSummoner.addedPlayerStamina["green"] >= 3) { s.tutorial.Increment(); }
                    Save.game.expendedStamina++;
                    s.turnManager.ChangeStaminaOf("player", -1);
                    // change the variables properly
                    s.statSummoner.SummonStats();
                    s.statSummoner.RepositionAllDice("player");
                    s.statSummoner.SetCombatDebugInformationFor("player");
                    // summon stats and update the debug information
                    Save.persistent.staminaUsed++;
                }
                
            }
            else if (name == "minus(Clone)") {
                if (s.statSummoner.addedPlayerStamina[stat] > 0) {
                    // if there is stamina added
                    s.statSummoner.addedPlayerStamina[stat]--;
                    Save.game.expendedStamina--;
                    s.turnManager.ChangeStaminaOf("player", 1);
                    // change the variables properly
                    s.statSummoner.SummonStats();
                    s.statSummoner.RepositionAllDice("player");
                    s.statSummoner.SetCombatDebugInformationFor("player");
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