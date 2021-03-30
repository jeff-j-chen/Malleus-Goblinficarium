using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.Linq;

public class Player : MonoBehaviour {
    [SerializeField] public int charNum;
    [SerializeField] private RuntimeAnimatorController[] controllers;
    [SerializeField] private Sprite[] icons;
    [SerializeField] private Sprite[] deathSprites;
    [SerializeField] public TextMeshProUGUI woundGUIElement;
    [SerializeField] public TextMeshProUGUI staminaCounter;
    [SerializeField] public TextMeshProUGUI target;
    [SerializeField] public TextMeshProUGUI targetInfo;
    [SerializeField] public List<GameObject> inventory = new List<GameObject>();
    [SerializeField] public TextMeshProUGUI identifier;
    [SerializeField] private GameObject statusEffectIcon;
    [SerializeField] public GameObject iconGameobject;
    [SerializeField] public string[] statusEffectNames = new string[] { "dodge", "leech", "fury", "haste", "courage" };
    [SerializeField] public string[] statusEffectDescs = new string[] { "if you strike first, ignore all damage", "cure the same wound as inflicted", "all picked die turn yellow", "pick 3 dice, enemy gets the rest", "keep 1 of your die till next round" };
    [SerializeField] public Sprite[] statusEffectSprites;
    private List<GameObject> statusEffectList = new List<GameObject>();
    public List<string> woundList = new List<string>();
    public bool isDead;
    public bool cancelMove = false;
    public float hintTimer;
    public Coroutine coroutine = null;
    public Dictionary<string, int> stats = new Dictionary<string, int>() {
        { "green", 0 },
        { "blue", 0 },
        { "red", 0 },
        { "white", 0 },
    };
    public Dictionary<string, int> potionStats = new Dictionary<string, int>() {
        { "green", 0 },
        { "blue", 0 },
        { "red", 0 },
        { "white", 0 },
    };
    public int stamina = 3;
    public int targetIndex = 0;
    private Scripts scripts;
    public bool isFurious = false;
    public bool isDodgy = false;
    public bool isHasty = false;
    public bool isBloodthirsty = false;
    public bool isCourageous = false;
    private Vector2 basePosition = new Vector2(-2.166667f, -1.866667f);
    private Vector2 iconPosition = new Vector2(-12.16667f, 3.333333f);
    private Dictionary<int, Vector2> deathPositions = new Dictionary<int, Vector2>() {
        {0, new Vector2(-2.04f, -2.53f)},
        {1, new Vector2(-2.166667f, -2.53f)},
        {2, new Vector2(-2.166667f, -2.53f)},
        {3, new Vector2(-2.2233337f, -2.53f)},
    };

    private void Start() {
        scripts = FindObjectOfType<Scripts>();
        // something here to check if we are continuing or starting a new game
        if (scripts.gameData.newGame) { charNum = scripts.persistentData.newCharNum; }
        else { charNum = scripts.gameData.curCharNum; }
        scripts.gameData.curCharNum = charNum;
        scripts.itemManager.GiveStarterItems(charNum);
        transform.position = basePosition;
        iconGameobject.transform.position = iconPosition;
        woundList = scripts.gameData.playerWounds;
        // set the initial positions
        identifier.text = "You";
        // set the identifier text
        iconGameobject.GetComponent<SpriteRenderer>().sprite = icons[charNum];
        // set the correct sprite
        GetComponent<Animator>().runtimeAnimatorController = controllers[charNum];
        // set the correct animation controller (using runtime so that it works in the actual game, and not only the editor)
        SetPlayerStatusEffect("fury", scripts.gameData.isFurious);
        SetPlayerStatusEffect("dodge", scripts.gameData.isDodgy);
        SetPlayerStatusEffect("haste", scripts.gameData.isHasty);
        SetPlayerStatusEffect("leech", scripts.gameData.isBloodthirsty);
        SetPlayerStatusEffect("courage", scripts.gameData.isCourageous);
        potionStats["green"] = scripts.gameData.potionAcc;
        potionStats["blue"] = scripts.gameData.potionSpd;
        potionStats["red"] = scripts.gameData.potionDmg;
        potionStats["white"] = scripts.gameData.potionDef;
        stamina = scripts.gameData.playerStamina + scripts.gameData.expendedStamina;
        scripts.gameData.expendedStamina = 0;
        staminaCounter.text = stamina.ToString();
        scripts.SaveGameData();
    }

    private void Update() {
        if (((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && !scripts.turnManager.isMoving) || (Input.GetAxis("Mouse ScrollWheel") < 0f  && !scripts.turnManager.isMoving)) {
            // if player is trying to change the target (w/s or up/down arrow or scroll wheel)
            // set the available targets to make sure the player can do that
            if (targetIndex < Mathf.Clamp(scripts.statSummoner.SumOfStat("green", "player"), 0, 7)) {
                // if player can target there
                if (hintTimer > 0.05f) { hintTimer += 0.1f; }
                // if there is still time left on the hint timer (for targeting face or targeting wounded body part)
                targetIndex++;
                // increment target index
                scripts.turnManager.SetTargetOf("player");
                // and set target based off the new target index
            }
        }
        else if (((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && !scripts.turnManager.isMoving) || (Input.GetAxis("Mouse ScrollWheel") > 0f  && !scripts.turnManager.isMoving)) {
            // pretty much the same as above
            if (targetIndex > 0) {  
                if (hintTimer > 0.05f) { hintTimer += 0.1f; }
                targetIndex--;
                scripts.turnManager.SetTargetOf("player");
            }
        }
        else if (Input.GetKeyDown(KeyCode.R) && !isDead && !scripts.turnManager.isMoving) {
            if (scripts.enemy.isDead) {
                // don't let player suicide when enemy is dead, because it is glitchy
                scripts.turnManager.SetStatusText("you've killed him");
            }
            else if (scripts.enemy.enemyName.text == "Tombstone" || scripts.enemy.enemyName.text == "Merchant") {
                // or on tombstone
                scripts.turnManager.SetStatusText("mind your manners");
            }
            else {
                if (!scripts.levelManager.lockActions) {
                    // don't let player restart while actions are lockedd
                    // player wants to restart
                    // player death on r is instant, so don't do animation stuff
                    isDead = true;
                    // mark player as dead
                    scripts.soundManager.PlayClip("death");
                    // play sound clip
                    scripts.player.GetComponent<Animator>().enabled = false;
                    GetComponent<SpriteRenderer>().sprite = GetDeathSprite();
                    SetPlayerPositionAfterDeath();
                    foreach (GameObject dice in scripts.diceSummoner.existingDice) {
                        StartCoroutine(dice.GetComponent<Dice>().FadeOut(false, true));
                        // fade out all existing die
                    }
                    scripts.statSummoner.ResetDiceAndStamina();
                    // clear them
                    scripts.turnManager.ClearPotionStats();
                    // clear potion stats
                    scripts.statSummoner.SummonStats();
                    // summon stats
                    scripts.statSummoner.SetDebugInformationFor("player");
                    // set debug (only player needed here)
                    scripts.turnManager.RecalculateMaxFor("player");
                    // reset target
                    scripts.tombstoneData.SetTombstoneData();;
                    // allow player to retry
                }
            }
        }
    }

    /// <summary>
    /// Use the player's weapon. 
    /// </summary>
    public void UseWeapon() {
        List<Dice> availableDice = new List<Dice>();
        // create an empty list to hold die in
        foreach (GameObject dice in scripts.diceSummoner.existingDice) {
            // for every die
            if (dice.GetComponent<Dice>().isAttached == false) {
                availableDice.Add(dice.GetComponent<Dice>());
                // if the die has not been chosen, add it to the list 
            }
        }
        if (availableDice.Count == 0) {
            if (hintTimer > 0.05f) {
                // player hits enter again, so immediately start the round
                StopCoroutine(coroutine);
                coroutine = null;
                // stop the existing coroutine
                scripts.turnManager.statusText.text = "";
                // clear the status text
                hintTimer = 0f;
                // reset the hint timer
                scripts.turnManager.RoundOne();
                // begin the round
            }
            else if (scripts.statSummoner.SumOfStat("green", "player") >= 7 && target.text != "face" && hintTimer <= 0.05f && PlayerPrefs.GetString("hints") == "on" && scripts.enemy.enemyName.text != "Devil" && scripts.enemy.enemyName.text != "Lich" && !scripts.itemManager.PlayerHasWeapon("maul")) {
                // if player wants hints, can aim for the face, but is not doing so, and doesn't have a maul
                coroutine = StartCoroutine(HintFace());
                // hint the player
            }
            else if (scripts.enemy.woundList.Contains(target.text.Substring(1)) && PlayerPrefs.GetString("hints") == "on") {
                // if body part is already wounded
                coroutine = StartCoroutine(HintTargetingWounded());
                // hint the player
            }
            else { scripts.turnManager.RoundOne(); }
        }
        else { 
            // dice are available
            if (scripts.itemManager.PlayerHasWeapon("mace") && !scripts.turnManager.usedMace) {
                // if player has mace
                scripts.turnManager.usedMace = true;
                scripts.gameData.usedMace = true;
                scripts.SaveGameData();
                // prevent player from using mace again
                scripts.soundManager.PlayClip("click");
                foreach (Dice dice in from a in scripts.diceSummoner.existingDice where a.GetComponent<Dice>().isAttached == false select a.GetComponent<Dice>()) {
                    // for every die that is not attached
                    StartCoroutine(dice.RerollAnimation(false));
                    // reroll the die
                }
            }
            else if (scripts.itemManager.PlayerHasWeapon("mace") && scripts.turnManager.usedMace) {
                scripts.turnManager.SetStatusText("already rerolled");
            }
            else {
                scripts.turnManager.SetStatusText("choose a die"); 
                // player doesn't have mace, so notify them to choose a die
            }
        }
    }

    /// <summary>
    /// Coroutine for hinting to the player that they can aim to the enemy's face.
    /// </summary>
    private IEnumerator HintFace() {
        scripts.turnManager.SetStatusText("note: you can aim to the face");
        // notify the player
        for (hintTimer = 3f; hintTimer > 0; hintTimer -= 0.025f) {
            yield return scripts.delays[0.025f];
            // start the timer (they can do actions here)
        }
        scripts.turnManager.RoundOne();
        // after the timer is up, start the round.
    }
    
    /// <summary>
    /// Coroutine for hinting to the player that they are targeting a wounded body part.
    /// </summary>
    public IEnumerator HintTargetingWounded() {
        // pretty much the exact same thing has hintface
        scripts.turnManager.SetStatusText("note: you are targeting a wounded body part");
        for (hintTimer = 3f; hintTimer > 0; hintTimer -= 0.025f) {
            yield return scripts.delays[0.025f];
        }
        scripts.turnManager.RoundOne();
    }

    /// <summary>
    /// Gets the death sprite of the player based on the character.
    /// </summary>
    /// <returns>The sprite tha was found.</returns>
    public Sprite GetDeathSprite() {
        return deathSprites[charNum];
    }

    /// <summary>
    /// Makes the player's corpse move to the correct position on the ground. 
    /// </summary>
    public void SetPlayerPositionAfterDeath() {
        transform.position = deathPositions[charNum];
    }
    
    /// <summary>
    ///  Sets the status effect of a player.
    /// </summary>
    /// <param name="statusEffect">The name of the status effect to toggle.</param>
    /// <param name="onOrOff">To turn on or off the status effect.</param>
    public void SetPlayerStatusEffect(string statusEffect, bool onOrOff) {
        if (statusEffect == "fury") { if (scripts.player.isFurious && onOrOff == true) { return; } else { 
            scripts.player.isFurious = onOrOff; 
            scripts.gameData.isFurious = onOrOff;
        } }
        else if (statusEffect == "dodge") { if (scripts.player.isDodgy && onOrOff == true) { return; } else { 
            scripts.player.isDodgy = onOrOff; 
            scripts.gameData.isDodgy = onOrOff;
        } }
        else if (statusEffect == "haste") { if (scripts.player.isHasty && onOrOff == true) { return; } else { 
            scripts.player.isHasty = onOrOff; 
            scripts.gameData.isHasty = onOrOff;
        } }
        else if (statusEffect == "leech") { if (scripts.player.isBloodthirsty && onOrOff == true) { return; } else { 
            scripts.player.isBloodthirsty = onOrOff; 
            scripts.gameData.isBloodthirsty = onOrOff;
        } }
        else if (statusEffect == "courage") { if (scripts.player.isCourageous && onOrOff == true) { return; } else { 
            scripts.player.isCourageous = onOrOff; 
            scripts.gameData.isCourageous = onOrOff;
        } }
        // set as desired, if turning on something already on then instantly exit function
        if (onOrOff == true) {
            // if turning on
            identifier.text = ":";
            // set colon (instead of you)
            GameObject icon = Instantiate(statusEffectIcon, new Vector2(-10.25f + 1f * statusEffectList.Count, 3.333f), Quaternion.identity);
            icon.GetComponent<SpriteRenderer>().sprite = statusEffectSprites[Array.IndexOf(statusEffectNames, statusEffect)];
            statusEffectList.Add(icon);
            // get the icon and add it to the status effect list.
        }
        if (onOrOff == false) {
            // turning off
            GameObject matchingIcon = null;
            try {
                matchingIcon = (from icon in statusEffectList where icon.GetComponent<SpriteRenderer>().sprite.name == statusEffect select icon).ToList()[0];
                // get the icon
                int shiftFrom = statusEffectList.IndexOf(matchingIcon);
                // try to shift each status effect over by 1
                for (int i = shiftFrom; i < statusEffectList.Count; i++) {
                    statusEffectList[i].transform.position = new Vector2(statusEffectList[i].transform.position.x - 1f, statusEffectList[i].transform.position.y);
                }
                statusEffectList.Remove(matchingIcon);
                Destroy(matchingIcon); 
            }
            catch {}
            if (statusEffectList.Count <= 0) { identifier.text = "You"; }
            // if no status effects, reset identifier text
        }
    }
}