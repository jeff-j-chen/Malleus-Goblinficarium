using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CharacterSelector : MonoBehaviour {
    [SerializeField] public int selectionNum;
    [SerializeField] private Sprite[] icons;
    [SerializeField] private Sprite releasedButton;
    [SerializeField] private Sprite pressedButton;
    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject rightButton;
    [SerializeField] private TextMeshProUGUI quoteText;
    [SerializeField] private TextMeshProUGUI perkText;
    [SerializeField] private GameObject bottomText;
    [SerializeField] public SimpleFadeIn simpleFadeIn;
    [SerializeField] public GameObject itemHider;
    private readonly string[] quotes = {
        "- \"they say 68% of adventurers die of starvation...\"",
        "- \"what comedy is your defiance, beasts!\"",
        "- \"...breastplate costs a fortune; dodging is free...\"",
        "- \"honestly all the carnage is making me sleepy...\"",
    }; 
    private readonly string[] perks = {
        "* Food restores more stamina",
        "* Gains a yellow die each round\n* Cannot use stamina",
        "* All white dice are set to 1\n* Gains 1 stamina upon inflicting a wound",
        "* White dice buff damage\n* Gains 3 stamina once wounded\n* As stamina reaches 10, wounds are cured and stamina is decreased by 10",
    }; 
    private bool preventPlayingFX = true;
    private Scripts s;
    
    private void Start() {
        s = FindFirstObjectByType<Scripts>();
        simpleFadeIn = FindFirstObjectByType<SimpleFadeIn>();
        // get necessary objects
        // pull in data from the Savefile
        HideItemsByDifficulty(false);
        
        // select 0 and go to it
        bottomText.SetActive(PlayerPrefs.GetString(s.BUTTONS_KEY) != "on");
        StartCoroutine(AllowFX());
    }
    
    public void HideItemsByDifficulty(bool preserveSelection = true) { 
        int selectionToKeep = preserveSelection ? selectionNum : 0;
        if (DifficultyHelper.IsEasy(Save.persistent.gameDifficulty)) { s.itemManager.floorItems[2].GetComponent<Item>().UnHide(); }
        else { s.itemManager.floorItems[2].GetComponent<Item>().Hide(); }
        // easy has 2nd item
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
            // enter selects the character
            Select();
            // but only if its already unlocked
        }
    }

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
    /// Select (preview) a character and display it to the player
    /// </summary>
    public void SetSelection(int num) {
        if (num is >= 0 and <= 3) {
            // only allow selections between the number of available characters
            selectionNum = num;
            // set the current selection num
            if (Save.persistent.unlockedChars[selectionNum]) {
                // if the current selected character is unlocked
                itemHider.SetActive(false);
                // show that character's items
                quoteText.text = quotes[selectionNum];
                UpdatePerkText();
                // show that character's quote and perk
            }
            else {
                // current selected character is not unlocked
                itemHider.SetActive(true);
                // make sure to hide items
                quoteText.text = "beat game on previous character to unlock";
                perkText.text = "";
                // let player know that it's locked
            }
            GetComponent<SpriteRenderer>().sprite = icons[selectionNum];
            // set the character icon
            if (!preventPlayingFX) { s.soundManager.PlayClip("click0"); }
            // only play sfx if we want it 
            if (selectionNum == 0) { leftButton.transform.position = new Vector2(-8.53f, 20f); }
            // hide left button if we are the leftmost (first) character
            else { leftButton.transform.position = new Vector2(-8.53f, 1f); }
            // otherwise show the left button
            if (selectionNum == 3) { rightButton.transform.position = new Vector2(8.53f, 20f); }
            else { rightButton.transform.position = new Vector2(8.53f, 1f); }
            // same for the right button, but 
        }
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
            // rename weapon to be rusty if in nightmare mode
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
        // give the character items based on their class, even if its not unlocked because it will be hidden regardless
        s.itemManager.floorItems[0].GetComponent<Item>().Select(false);
        // select the first item so its not buggy
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
    /// Toggles easy mode and handles the hiding.
    /// </summary>
    public void CycleDifficulty() {
        if (!simpleFadeIn.lockChanges) {
            // don't allow toggle of easy if we are fading rn
            s.soundManager.PlayClip("click1");
            // play clip
            // toggle the boolean
            StartCoroutine(simpleFadeIn.FadeHide()); 
            // fade to black and then back
            Save.persistent.gameDifficulty = DifficultyHelper.Next(Save.persistent.gameDifficulty);
            Save.persistent.difficultyVersion = DifficultyHelper.CurrentDifficultyVersion;

            Save.SavePersistent();
            // apply it to the Save file so the next game will have the correct character
        }
    }

    public void ToggleEndlessMode() {
        if (simpleFadeIn.lockChanges) { return; }

        s.soundManager.PlayClip("click1");
        Save.persistent.endlessModeEnabled = !Save.persistent.endlessModeEnabled;
        Save.SavePersistent();
        UpdatePerkText();
    }

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
