using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Runs the character-select screen, including selection changes, difficulty/endless toggles,
/// and previewing each character's starting loadout.
/// </summary>
public class CharacterSelector : MonoBehaviour {
    [SerializeField] public int selectionNum; // currently previewed character index
    [SerializeField] private Sprite[] icons; // full-size portrait sprite for each character slot
    [SerializeField] private Sprite releasedButton; // default sprite for left/right selector buttons
    [SerializeField] private Sprite pressedButton; // pressed sprite for left/right selector buttons
    [SerializeField] private GameObject leftButton; // previous-character button that hides on the first slot
    [SerializeField] private GameObject rightButton; // next-character button that hides on the last slot
    [SerializeField] private TextMeshProUGUI quoteText; // flavor quote shown under the portrait
    [SerializeField] private TextMeshProUGUI perkText; // perk and mode summary text for the selected character
    [SerializeField] private GameObject bottomText; // button-hint footer hidden when icon prompts are enabled
    [SerializeField] public SimpleFadeIn simpleFadeIn; // fade controller used when cycling difficulty
    [SerializeField] public GameObject itemHider; // overlay that obscures starter items for locked characters
    private readonly string[] quotes = {
        "- \"they say 68% of adventurers die of starvation...\"",
        "- \"what comedy is your defiance, beasts!\"",
        "- \"...breastplate costs a fortune; dodging is free...\"",
        "- \"honestly all the carnage is making me sleepy...\"",
    }; // quote text aligned by character index
    private readonly string[] perks = {
        "* Food restores more stamina",
        "* Gains a yellow die each round\n* Cannot use stamina",
        "* All white dice are set to 1\n* Gains 1 stamina upon inflicting a wound",
        "* White dice buff damage\n* Gains 3 stamina once wounded\n* As stamina reaches 10, wounds are cured and stamina is decreased by 10",
    }; // base perk text aligned by character index
    private bool preventPlayingFX = true; // prevents initial screen setup from playing click sounds
    private Scripts s; // shared systems and item references for the menu scene
    
    /// <summary>
    /// Initializes the character-select screen and applies current difficulty visibility rules.
    /// </summary>
    private void Start() {
        s = FindFirstObjectByType<Scripts>();
        simpleFadeIn = FindFirstObjectByType<SimpleFadeIn>();
        HideItemsByDifficulty(false);
        
        bottomText.SetActive(PlayerPrefs.GetString(s.BUTTONS_KEY) != "on");
        StartCoroutine(AllowFX());
    }
    
    /// <summary>
    /// Shows or hides difficulty-dependent starter items, then refreshes the current selection.
    /// </summary>
    /// <param name="preserveSelection">keep the current character selection when true</param>
    public void HideItemsByDifficulty(bool preserveSelection = true) { 
        int selectionToKeep = preserveSelection ? selectionNum : 0;
        if (DifficultyHelper.IsEasy(Save.persistent.gameDifficulty)) { s.itemManager.floorItems[2].GetComponent<Item>().UnHide(); }
        else { s.itemManager.floorItems[2].GetComponent<Item>().Hide(); }
        // slot 2 is the easy-only bonus item, while slot 1 is always visible
        s.itemManager.floorItems[1].GetComponent<Item>().UnHide();
        selectionNum = Mathf.Clamp(selectionToKeep, 0, icons.Length - 1);
        SetSelection(selectionNum);
    }

    /// <summary>
    /// Only allow sound effects to be played after a short delay, preventing extra clicking.
    /// </summary>
    private IEnumerator AllowFX() { 
        yield return s.delays[0.45f];
        preventPlayingFX = false;
    }

    /// <summary>
    /// Handles keyboard navigation, difficulty toggles, and character confirmation.
    /// </summary>
    private void Update() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { 
            SetSelection(selectionNum - 1);
            ChangeToPressed("Left"); 
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) { 
            SetSelection(selectionNum + 1);
            ChangeToPressed("Right"); 
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow)) { 
            ChangeToReleased("Left");
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow)) { 
            ChangeToReleased("Right");
        }
        // depending on the input, shift the selection in that direction and shows a small animation
        else if (Input.GetKeyDown(KeyCode.Space)) { CycleDifficulty(); }
        // space toggles easy mode
        else if (Input.GetKeyDown(KeyCode.E)) { ToggleEndlessMode(); }
        // e toggles endless mode
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) { 
            Select();
        }
    }

    /// <summary>
    /// Confirms the current character if it has been unlocked.
    /// </summary>
    public void Select() {
        if (Save.persistent.unlockedChars[selectionNum]) { StartCoroutine(LoadMenuScene()); }
    }

    /// <summary>
    /// Coroutine used to load the menu scene after the player locks in their character selection.
    /// </summary>
    private IEnumerator LoadMenuScene() { 
        s.soundManager.PlayClip("blip0");
        // play sfx (this is when selected)
        Save.persistent.newCharNum = selectionNum;
        // set the selection num
        Save.SavePersistent();
        // Save the selection num
        yield return s.delays[0.1f];
        // delay here, because i don't want a singleton and this allows blip to complete playing
        SceneManager.LoadScene("Menu");
        // load the menu scene after the delay
    }

    /// <summary>
    /// Selects a character slot, updates the portrait/description, and previews its starter loadout.
    /// </summary>
    /// <param name="num">requested character index</param>
    public void SetSelection(int num) {
        if (num is >= 0 and <= 3) {
            selectionNum = num;
            if (Save.persistent.unlockedChars[selectionNum]) {
                itemHider.SetActive(false);
                quoteText.text = quotes[selectionNum];
                UpdatePerkText();
            }
            else {
                itemHider.SetActive(true);
                quoteText.text = "beat game on previous character to unlock";
                perkText.text = "";
            }
            GetComponent<SpriteRenderer>().sprite = icons[selectionNum];
            if (!preventPlayingFX) { s.soundManager.PlayClip("click0"); }
            if (selectionNum == 0) { leftButton.transform.position = new Vector2(-8.53f, 20f); }
            else { leftButton.transform.position = new Vector2(-8.53f, 1f); }
            if (selectionNum == 3) { rightButton.transform.position = new Vector2(8.53f, 20f); }
            else { rightButton.transform.position = new Vector2(8.53f, 1f); }
        }

        // starter loadout preview objects are reused, so each selection rewrites their names/modifiers/sprites in place
        switch (num) {
            case 0:
                s.itemManager.floorItems[0].GetComponent<Item>().itemName = "harsh sword";
                s.itemManager.floorItems[0].GetComponent<Item>().modifier = "harsh";
                s.itemManager.floorItems[0].GetComponent<SpriteRenderer>().sprite = 
                    s.itemManager.GetItemSprite("sword");
                s.itemManager.floorItems[1].GetComponent<Item>().itemName = "steak";
                s.itemManager.floorItems[1].GetComponent<SpriteRenderer>().sprite = 
                    s.itemManager.GetItemSprite("steak");
                s.itemManager.floorItems[2].GetComponent<Item>().itemName = "torch";
                s.itemManager.floorItems[2].GetComponent<SpriteRenderer>().sprite = 
                    s.itemManager.GetItemSprite("torch");
                break;
            case 1:
                s.itemManager.floorItems[0].GetComponent<Item>().itemName = "common maul";
                s.itemManager.floorItems[0].GetComponent<Item>().modifier = "common";
                s.itemManager.floorItems[0].GetComponent<SpriteRenderer>().sprite = 
                    s.itemManager.GetItemSprite("maul");
                s.itemManager.floorItems[1].GetComponent<Item>().itemName = "armor";
                s.itemManager.floorItems[1].GetComponent<SpriteRenderer>().sprite = 
                    s.itemManager.GetItemSprite("armor");
                s.itemManager.floorItems[2].GetComponent<Item>().itemName = "helm of might";
                s.itemManager.floorItems[2].GetComponent<SpriteRenderer>().sprite = 
                    s.itemManager.GetItemSprite("helm_of_might");
                break;
            case 2:
                s.itemManager.floorItems[0].GetComponent<Item>().itemName = "quick dagger";
                s.itemManager.floorItems[0].GetComponent<Item>().modifier = "quick";
                s.itemManager.floorItems[0].GetComponent<SpriteRenderer>().sprite = 
                    s.itemManager.GetItemSprite("dagger");
                s.itemManager.floorItems[1].GetComponent<Item>().itemName = "boots of dodge";
                s.itemManager.floorItems[1].GetComponent<SpriteRenderer>().sprite = 
                    s.itemManager.GetItemSprite("boots_of_dodge");
                s.itemManager.floorItems[2].GetComponent<Item>().itemName = "ankh";
                s.itemManager.floorItems[2].GetComponent<SpriteRenderer>().sprite = 
                    s.itemManager.GetItemSprite("ankh");
                break;
            case 3:
                s.itemManager.floorItems[0].GetComponent<Item>().itemName = "ruthless mace";
                s.itemManager.floorItems[0].GetComponent<Item>().modifier = "ruthless";
                s.itemManager.floorItems[0].GetComponent<SpriteRenderer>().sprite = 
                    s.itemManager.GetItemSprite("mace");
                s.itemManager.floorItems[1].GetComponent<Item>().itemName = "cheese";
                s.itemManager.floorItems[1].GetComponent<SpriteRenderer>().sprite = 
                    s.itemManager.GetItemSprite("cheese");
                s.itemManager.floorItems[2].GetComponent<Item>().itemName = "kapala";
                s.itemManager.floorItems[2].GetComponent<SpriteRenderer>().sprite = 
                    s.itemManager.GetItemSprite("kapala");
                break;
        }
        if (DifficultyHelper.IsNightmare(Save.persistent.gameDifficulty)) {
            // nightmare replaces the normal starter weapon modifier and the non-weapon trade item
            s.itemManager.floorItems[0].GetComponent<Item>().modifier = "rusty";
            string itemName = s.itemManager.floorItems[0].GetComponent<Item>().itemName;
            s.itemManager.floorItems[0].GetComponent<Item>().itemName = "rusty " + ItemManager.GetWeaponBaseName(itemName);
            string nightmareTradeItemName = ItemManager.GetNightmareStarterTradeItemName(selectionNum);
            s.itemManager.floorItems[1].GetComponent<Item>().itemName = nightmareTradeItemName;
            s.itemManager.floorItems[1].GetComponent<Item>().modifier = "";
            s.itemManager.floorItems[1].GetComponent<SpriteRenderer>().sprite = 
                s.itemManager.GetItemSprite(nightmareTradeItemName.Replace(' ', '_'));
            // print(s.itemManager.floorItems[0].GetComponent<Item>().itemName);
        }
        // selecting the first starter item keeps the item description/highlight logic from being left in an invalid state
        s.itemManager.floorItems[0].GetComponent<Item>().Select(false);
    }

    /// <summary>
    /// Changes a L/R Character Select button to its 'pressed' sprite.
    /// </summary>
    public void ChangeToPressed(string leftOrRight) {
        // set the button to be pressed down 
        if (leftOrRight == "Left") { leftButton.GetComponent<CharacterSwapButton>().spriteRenderer.sprite = pressedButton; }
        else { rightButton.GetComponent<CharacterSwapButton>().spriteRenderer.sprite = pressedButton; }
    }

    /// <summary>
    /// Changes a L/R Character Select button to its 'released' sprite.
    /// </summary>
    public void ChangeToReleased(string leftOrRight) {
        // make the button pop up
        if (leftOrRight == "Left") { leftButton.GetComponent<CharacterSwapButton>().spriteRenderer.sprite = releasedButton; }
        else { rightButton.GetComponent<CharacterSwapButton>().spriteRenderer.sprite = releasedButton; }
    }

    /// <summary>
    /// Cycles to the next difficulty and fades the preview so starter items can be rebuilt cleanly.
    /// </summary>
    public void CycleDifficulty() {
        if (!simpleFadeIn.lockChanges) {
            s.soundManager.PlayClip("click1");
            StartCoroutine(simpleFadeIn.FadeHide()); 
            Save.persistent.gameDifficulty = DifficultyHelper.Next(Save.persistent.gameDifficulty);
            Save.persistent.difficultyVersion = DifficultyHelper.CurrentDifficultyVersion;

            Save.SavePersistent();
        }
    }

    /// <summary>
    /// Toggles endless mode and refreshes the perk summary text.
    /// </summary>
    public void ToggleEndlessMode() {
        if (simpleFadeIn.lockChanges) { return; }

        s.soundManager.PlayClip("click1");
        Save.persistent.endlessModeEnabled = !Save.persistent.endlessModeEnabled;
        Save.SavePersistent();
        UpdatePerkText();
    }

    /// <summary>
    /// Rebuilds the perk text block for the selected character, difficulty, and endless mode.
    /// </summary>
    public void UpdatePerkText() { 
        perkText.text = perks[selectionNum];
        if (DifficultyHelper.IsEasy(Save.persistent.gameDifficulty)) { 
            perkText.text += "\n> Selected Difficulty: EASY";
        }
        else if (DifficultyHelper.IsNormal(Save.persistent.gameDifficulty)) { 
            perkText.text += "\n> Selected Difficulty: NORMAL";
        }
        else if (DifficultyHelper.IsHard(Save.persistent.gameDifficulty)) { 
            perkText.text += "\n> Selected Difficulty: HARD";
        }
        else if (DifficultyHelper.IsNightmare(Save.persistent.gameDifficulty)) {
            perkText.text += "\n> Selected Difficulty: NIGHTMARE";
        }
        perkText.text += Save.persistent.endlessModeEnabled
            ? "\n> Endless Mode: ENABLED"
            : "\n> Endless Mode: DISABLED";
    }
}
