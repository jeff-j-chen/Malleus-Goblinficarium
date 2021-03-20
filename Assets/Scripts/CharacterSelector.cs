using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelector : MonoBehaviour {
    [SerializeField] public int selectionNum;
    [SerializeField] public bool[] unlockedChars = new bool[4] { true, false, false, false };
    [SerializeField] public bool easy = false;
    [SerializeField] private Sprite[] icons; 
    private string[] quotes = new string[4] {
        "- \"they say 68% of adventurers die of starvation...\"",
        "- \"what comedy is your defiance, beasts!\"",
        "- \"...breastplate costs a fortune; dodging is free...\"",
        "- \"honestly all the carnage is making me sleepy...\"",
    }; 
    private string[] perks = new string[4] {
        "* Food restores more stamina",
        "* Gains a yellow die each round\n* Cannot use stamina",
        "* All white dice are set to 1\n* Gains 1 stamina upon inflicting a wound",
        "* White dice buff damage\n* Gains 3 stamina once wounded\n* As stamina reaches 10, wounds are cured and stamina is decreased by 10",
    }; 
    [SerializeField] private Sprite releasedButton;
    [SerializeField] private Sprite pressedButton;
    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject rightButton;
    [SerializeField] private TextMeshProUGUI quoteText;
    [SerializeField] private TextMeshProUGUI perkText;
    [SerializeField] public SimpleFadeIn simpleFadeIn;
    [SerializeField] public GameObject itemHider;
    private bool preventPlayingFX = true;
    private Scripts scripts;
    private void Start() {
        scripts = FindObjectOfType<Scripts>();
        simpleFadeIn = FindObjectOfType<SimpleFadeIn>();
        unlockedChars = scripts.persistentData.unlockedChars;
        selectionNum = 0;
        SetSelection(selectionNum);
        StartCoroutine(allowFX());
        // need to add a way to keep the easy between scene changes
    }
    
    private IEnumerator allowFX() { 
        yield return new WaitForSeconds(0.45f);
        preventPlayingFX = false;
    }


    private void Update() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { ChangeToPressed("Left"); }
        // holding down left, so press down the button
        else if (Input.GetKeyDown(KeyCode.RightArrow)) { ChangeToPressed("Right"); }
        // holding down right, so press down the button
        else if (Input.GetKeyUp(KeyCode.LeftArrow)) { 
            // left arrow released, so move the selection left
            SetSelection(selectionNum - 1);
            ChangeToReleased("Left");
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow)) { 
            // right arrow released, so move the selection right
            SetSelection(selectionNum + 1);
            ChangeToReleased("Right");
        }
        else if (Input.GetKeyDown(KeyCode.Space)) { ToggleEasy(); }
        // space toggles easy mode
        else if (Input.GetKeyDown(KeyCode.Return)) { 
            // enter selects the character
            if (unlockedChars[selectionNum]) { StartCoroutine(LoadMenuScene()); }
            // but only if its already unlocked
        }
    }

    private IEnumerator LoadMenuScene() { 
        scripts.soundManager.PlayClip("blip");
        // play sfx (this is when selected)
        scripts.gameData.newCharNum = selectionNum;
        // set the selection num
        scripts.SaveGameData();
        // save the selection num
        yield return scripts.delays[0.1f];
        // delay here, because i don't want a singleton and this allows blip to complete playing
        SceneManager.LoadScene("Menu");
        // load the menu scene after the delay
    }

    // make this the main selection function, with checking for unlocked chars and setting arrows etc.
    public void SetSelection(int num) {
        if (0 <= num && num <= 3) {
            // only allow selections between the number of available characters
            selectionNum = num;
            // set the current selection num
            if (unlockedChars[selectionNum]) {
                // if the current selected character is unlocked
                itemHider.SetActive(false);
                // show that character's items
                quoteText.text = quotes[selectionNum];
                perkText.text = perks[selectionNum];
                // show that character's quote and perk
            }
            else {
                // current selected character is not unlocked
                itemHider.SetActive(true);
                // make sure to hide items
                quoteText.text = "beat game on previous character to unlock";
                perkText.text = "";
                // let player knmow that it's locked
            }
            GetComponent<SpriteRenderer>().sprite = icons[selectionNum];
            // set the character icon
            if (!preventPlayingFX) { scripts.soundManager.PlayClip("click"); }
            // only play sfx if we want it 
            if (selectionNum == 0) { leftButton.transform.position = new Vector2(-8.53f, 20f); }
            // hide left button if we are the leftmost (first) character
            else { leftButton.transform.position = new Vector2(-8.53f, 1f); }
            // otherwise show the left button
            if (selectionNum == 3) { rightButton.transform.position = new Vector2(8.53f, 20f); }
            else { rightButton.transform.position = new Vector2(8.53f, 1f); }
            // same for the right button, but 
        }
        if (num == 0) { 
            scripts.itemManager.floorItems[0].GetComponent<Item>().itemName = "harsh sword";
            scripts.itemManager.floorItems[0].GetComponent<Item>().modifier = "harsh";
            scripts.itemManager.floorItems[0].GetComponent<SpriteRenderer>().sprite = scripts.itemManager.GetItemSprite("sword");
            scripts.itemManager.floorItems[1].GetComponent<Item>().itemName = "steak";
            scripts.itemManager.floorItems[1].GetComponent<SpriteRenderer>().sprite = scripts.itemManager.GetItemSprite("steak");
            scripts.itemManager.floorItems[2].GetComponent<Item>().itemName = "torch";
            scripts.itemManager.floorItems[2].GetComponent<SpriteRenderer>().sprite = scripts.itemManager.GetItemSprite("torch");
        }
        else if (num == 1) { 
            scripts.itemManager.floorItems[0].GetComponent<Item>().itemName = "common maul";
            scripts.itemManager.floorItems[0].GetComponent<Item>().modifier = "common";
            scripts.itemManager.floorItems[0].GetComponent<SpriteRenderer>().sprite = scripts.itemManager.GetItemSprite("maul");
            scripts.itemManager.floorItems[1].GetComponent<Item>().itemName = "armor";
            scripts.itemManager.floorItems[1].GetComponent<SpriteRenderer>().sprite = scripts.itemManager.GetItemSprite("armor");
            scripts.itemManager.floorItems[2].GetComponent<Item>().itemName = "helm of might";
            scripts.itemManager.floorItems[2].GetComponent<SpriteRenderer>().sprite = scripts.itemManager.GetItemSprite("helm_of_might");
        }
        else if (num == 2) {
            scripts.itemManager.floorItems[0].GetComponent<Item>().itemName = "quick dagger";
            scripts.itemManager.floorItems[0].GetComponent<Item>().modifier = "quick";
            scripts.itemManager.floorItems[0].GetComponent<SpriteRenderer>().sprite = scripts.itemManager.GetItemSprite("dagger");
            scripts.itemManager.floorItems[1].GetComponent<Item>().itemName = "boots of dodge";
            scripts.itemManager.floorItems[1].GetComponent<SpriteRenderer>().sprite = scripts.itemManager.GetItemSprite("boots_of_dodge");
            scripts.itemManager.floorItems[2].GetComponent<Item>().itemName = "ankh";
            scripts.itemManager.floorItems[2].GetComponent<SpriteRenderer>().sprite = scripts.itemManager.GetItemSprite("ankh");
        }
        else if (num == 3) { 
            scripts.itemManager.floorItems[0].GetComponent<Item>().itemName = "ruthless mace";
            scripts.itemManager.floorItems[0].GetComponent<Item>().modifier = "ruthless";
            scripts.itemManager.floorItems[0].GetComponent<SpriteRenderer>().sprite = scripts.itemManager.GetItemSprite("mace");
            scripts.itemManager.floorItems[1].GetComponent<Item>().itemName = "cheese";
            scripts.itemManager.floorItems[1].GetComponent<SpriteRenderer>().sprite = scripts.itemManager.GetItemSprite("cheese");
            scripts.itemManager.floorItems[2].GetComponent<Item>().itemName = "kapala";
            scripts.itemManager.floorItems[2].GetComponent<SpriteRenderer>().sprite = scripts.itemManager.GetItemSprite("kapala");
        }
        // give the character items based on their class, even if its not unlocked because it will be hidden regardless
        scripts.itemManager.floorItems[0].GetComponent<Item>().Select(false);
        // make sure to select the first item
    }

    public void ChangeToPressed(string LeftOrRight) {
        // set the button to be pressed down 
        if (LeftOrRight == "Left") { leftButton.GetComponent<CharacterSwapButton>().spriteRenderer.sprite = pressedButton; }
        else { rightButton.GetComponent<CharacterSwapButton>().spriteRenderer.sprite = pressedButton; }
    }

    public void ChangeToReleased(string LeftOrRight) {
        // make the button pop up
        if (LeftOrRight == "Left") { leftButton.GetComponent<CharacterSwapButton>().spriteRenderer.sprite = releasedButton; }
        else { rightButton.GetComponent<CharacterSwapButton>().spriteRenderer.sprite = releasedButton; }
    }

    private void ToggleEasy() {
        if (!simpleFadeIn.lockChanges) {
            // don't allow toggle of easy if we are fading rn
            scripts.soundManager.PlayClip("click1");
            // play clip
            easy = !easy;
            // toggle the boolean
            StartCoroutine(simpleFadeIn.FadeHide()); 
            // fade to black and then back
            scripts.persistentData.easyMode = easy; 
            scripts.SavePersistentData();
            // apply it to our save file so the next game will have the correct character
        }
    }
}
