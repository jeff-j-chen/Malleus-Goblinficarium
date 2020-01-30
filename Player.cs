using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.Linq;

public class Player : MonoBehaviour {
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
    [SerializeField] public string[] statusEffectNames = new string[] { "dodge", "leech", "fury", "haste", "courage" };
    [SerializeField] public string[] statusEffectDescs = new string[] { "if you strike first, ignore all damage", "cure the same wound as inflicted", "all picked die turn yellow", "pick 3 dice, enemy gets the rest", "keep 1 of your die till next round" };
    [SerializeField] public Sprite[] statusEffectSprites;
    private List<GameObject> statusEffectList = new List<GameObject>();
    public List<string> availableTargets = new List<string>();
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
    public int charNum = 0;
    public int stamina = 3;
    public int targetIndex = 0;
    private Scripts scripts;
    public bool isFurious = false;
    public bool isDodgy = false;
    public bool isHasty = false;
    public bool isBloodthirsty = false;
    public bool isCourageous;

    private void Start() {
        scripts = FindObjectOfType<Scripts>();
        identifier.text = "You";
        // set the identifier text
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = icons[charNum];
        // set the correct sprite
        GetComponent<Animator>().runtimeAnimatorController = controllers[charNum];
        // set the correct animation controller (using runtime so that it works in the actual game, and not only the editor)
        staminaCounter.text = stamina.ToString();
        // set up the information for the stamina counter
    }

    private void Update() {
        if ((Input.GetKeyDown(KeyCode.DownArrow) && !scripts.turnManager.isMoving) || (Input.GetAxis("Mouse ScrollWheel") < 0f  && !scripts.turnManager.isMoving)) {
            // if player is trying to change the target (up/down arrow or scroll wheel)
            scripts.turnManager.SetAvailableTargetsOf("player");
            // set the available targets to make sure the player can do that
            if (targetIndex < scripts.player.availableTargets.Count) {
                // if player can target there
                if (hintTimer > 0.05f) { hintTimer += 0.1f; }
                // if there is still time left on the hint timer (for targeting face or targeting wounded body part)
                targetIndex++;
                // increment target index
                scripts.turnManager.SetTargetOf("player");
                // and set target based off the new target index
            }
        }
        else if ((Input.GetKeyDown(KeyCode.UpArrow) && !scripts.turnManager.isMoving) || (Input.GetAxis("Mouse ScrollWheel") > 0f  && !scripts.turnManager.isMoving)) {
            // pretty much the same as above
            if (targetIndex > 0) {  
                if (hintTimer > 0.05f) { hintTimer += 0.1f; }
                targetIndex--;
                scripts.turnManager.SetTargetOf("player");
            }
        }
    }

    /// <summary>
    /// Use the player's weapon. 
    /// </summary>
    public void UseWeapon() {
        if (scripts.statSummoner.SumOfStat("green", "player") >= 7 && target.text != "face" && hintTimer <= 0.05 && PlayerPrefs.GetString("hints") == "on") {
            // if player wants hints, can aim for the face, but is not doing so
            coroutine = StartCoroutine(HintFace());
            // hint the player
        }
        else if (scripts.player.woundList.Contains(target.text.Substring(1)) && PlayerPrefs.GetString("hints") == "on") {
            // if body part is already wounded
            coroutine = StartCoroutine(HintTargetingWounded());
            // hint the player
        }
        else if (hintTimer > 0.05f) {
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
        else { scripts.turnManager.RoundOne(); }
    }

    /// <summary>
    /// Coroutine for hinting to the player that they can aim to the enemy's face.
    /// </summary>
    private IEnumerator HintFace() {
        scripts.turnManager.SetStatusText("note: you can aim to the face");
        // notify the player
        for (hintTimer = 3f; hintTimer > 0; hintTimer -= 0.1f) {
            yield return scripts.delays[0.1f];
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
        for (hintTimer = 3f; hintTimer > 0; hintTimer -= 0.1f) {
            yield return scripts.delays[0.1f];
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
        if (charNum == 0) { MoveBy(-0.1266667f, 0.6633333f); }
        else if (charNum == 1) { MoveBy(0f, 0.6633333f); }
        else if (charNum == 2) { MoveBy(0f, 0.6633333f); }
        else if (charNum == 3) { MoveBy(0.0566667f, 0.6633333f); }
        else { print("bad"); }
    }

    /// <summary>
    /// Moves the player by the specified amount while keeping the icon in place. 
    /// </summary>
    /// <param name="x">Horizontal movement.</param>
    /// <param name="y">Vertical movement.</param>
    private void MoveBy(float x, float y) {
        transform.position = new Vector2(transform.position.x - x, transform.position.y - y);
        // move the player
        transform.GetChild(0).transform.position = new Vector2(transform.GetChild(0).transform.position.x + x, transform.GetChild(0).transform.position.y + y);
        // move the child in the opposite direction
    }
    
    /// <summary>
    ///  Sets the status effect of a player.
    /// </summary>
    /// <param name="statusEffect">The name of the status effect to toggle.</param>
    /// <param name="onOrOff">To turn on or off the status effect.</param>
    public void SetPlayerStatusEffect(string statusEffect, string onOrOff) {
        if (onOrOff == "on") {
            // if turning on
            identifier.text = ":";
            // set colon (instead of you)
            GameObject icon = Instantiate(statusEffectIcon, new Vector2(-10.25f + 1f * statusEffectList.Count, 3.333f), Quaternion.identity);
            icon.GetComponent<SpriteRenderer>().sprite = statusEffectSprites[Array.IndexOf(statusEffectNames, statusEffect)];
            statusEffectList.Add(icon);
            // get the icon and add it to the status effect list.
        }
        if (onOrOff == "off") {
            // turning off
            IEnumerable<GameObject> matchingIcons = from icon in statusEffectList where icon.GetComponent<SpriteRenderer>().sprite.name == statusEffect select icon;
            // get matching icons (just in case.)
            foreach (GameObject icon in matchingIcons.ToList()) {
                statusEffectList.Remove(icon);
                Destroy(icon); 
                // remove any matching icons
            }
            if (statusEffectList.Count <= 0) { identifier.text = "You"; }
            // if no status effects, reset identifier text
        }
    }
}