using UnityEngine;

public class StaminaButton : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Scripts scripts;
    public string stat;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        scripts = FindObjectOfType<Scripts>();
    }

    private void OnMouseUpAsButton()
    {
        if (!scripts.turnManager.isMoving && !scripts.player.woundList.Contains("hip"))
        {
            if (name == "plus(Clone)")
            {
                if (scripts.player.stamina > 0)
                {
                    ShiftDiceAccordingly(stat, 1);
                    scripts.statSummoner.addedPlayerStamina[stat]++;
                    scripts.turnManager.ChangeStaminaOf("player", -1);
                    scripts.statSummoner.SummonStats();
                    scripts.statSummoner.SetDebugInformationFor("player");
                }
            }
            else if (name == "minus(Clone)")
            {
                if (scripts.statSummoner.addedPlayerStamina[stat] > 0)
                {
                    ShiftDiceAccordingly(stat, -1);
                    scripts.statSummoner.addedPlayerStamina[stat]--;
                    scripts.turnManager.ChangeStaminaOf("player", 1);
                    scripts.statSummoner.SummonStats();
                    scripts.statSummoner.SetDebugInformationFor("player");
                }
            }
            else
            {
                Debug.LogError("StaminaButton is not attached to the correct object");
            }
        }
    }

    private void ShiftDiceAccordingly(string stat, int shiftAmount)
    {
        int modifier = 0;
        if (scripts.player.stats[stat] + scripts.itemManager.neckletStats[stat] + scripts.player.potionStats[stat] + scripts.statSummoner.addedPlayerStamina[stat] >= 0 && shiftAmount > 0)
        {
            // total sum is positive and is adding, so shift forwards
            modifier = 1;
        }
        else if (scripts.player.stats[stat] + scripts.itemManager.neckletStats[stat] + scripts.player.potionStats[stat] + scripts.statSummoner.addedPlayerStamina[stat] >= 1 && shiftAmount < 0)
        {
            // total sum is > 1 and is subtracting, so shift backwards
            modifier = -1;
        }
        else if (scripts.player.stats[stat] + scripts.itemManager.neckletStats[stat] + scripts.player.potionStats[stat] + scripts.statSummoner.addedPlayerStamina[stat] >= 0 && shiftAmount < 0)
        {
            // total sum is 1 or 0 (before adding on the stamina) and is subtracting, so shift forwards
            modifier = 1;
        }
        else if (scripts.player.stats[stat] + scripts.itemManager.neckletStats[stat] + scripts.player.potionStats[stat] + scripts.statSummoner.addedPlayerStamina[stat] < 0 && shiftAmount > 0)
        {
            // total sum is negative and is adding, so shift backwards 
            modifier = -1;
        }
        else if (scripts.player.stats[stat] + scripts.itemManager.neckletStats[stat] + scripts.player.potionStats[stat] + scripts.statSummoner.addedPlayerStamina[stat] < 0 && shiftAmount < 0)
        {
            // total sum is negative and is subtracting, so shift forwards
            modifier = 1;
        }
        foreach (Dice dice in scripts.statSummoner.addedPlayerDice[stat])
        {
            dice.transform.position = new Vector2(dice.transform.position.x + scripts.statSummoner.xOffset * Mathf.Abs(shiftAmount) * modifier, dice.transform.position.y);
            dice.instantiationPos = dice.transform.position;
        }
    }

    private void OnMouseEnter()
    {
        spriteRenderer.color = scripts.colors.hovered;
    }

    private void OnMouseDown()
    {
        scripts.soundManager.PlayClip("click");
        spriteRenderer.color = scripts.colors.clicked;
    }

    private void OnMouseExit()
    {
        spriteRenderer.color = Color.white;
    }

    private void OnMouseUp()
    {
        spriteRenderer.color = Color.white;
    }
}