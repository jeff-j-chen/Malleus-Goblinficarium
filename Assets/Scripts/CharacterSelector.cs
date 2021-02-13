using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelector : MonoBehaviour {
    [SerializeField] public int selectionNum;
    [SerializeField] private bool[] unlockedChars = new bool[4] { true, false, false, false };
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
    private bool preventPlayingFX = true;
    private Scripts scripts;
    private void Start() {
        // allow for selection of 1
        scripts = FindObjectOfType<Scripts>();
        simpleFadeIn = FindObjectOfType<SimpleFadeIn>();
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
        else if (Input.GetKeyDown(KeyCode.RightArrow)) { ChangeToPressed("Right"); }
        else if (Input.GetKeyUp(KeyCode.LeftArrow)) { 
            SetSelection(selectionNum - 1);
            ChangeToReleased("Left");
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow)) { 
            SetSelection(selectionNum + 1);
            ChangeToReleased("Right");
        }
        else if (Input.GetKeyDown(KeyCode.Space)) { ToggleEasy(); }
        else if (Input.GetKeyDown(KeyCode.Return)) { 
            if (unlockedChars[selectionNum]) {
                StartCoroutine(LoadMenuScene());
            }
        }
    }

    private IEnumerator LoadMenuScene() { 
        scripts.soundManager.PlayClip("blip");
        SaveSystem.SaveData(scripts, false);
        scripts.data = SaveSystem.LoadData();
        yield return scripts.delays[0.15f];
        SceneManager.LoadScene("Menu");
    }

    // make this the main selection function, with checking for unlocked chars and setting arrows etc.
    public void SetSelection(int num) {
        if (0 <= num && num <= 3) {
            selectionNum = num;
            if (unlockedChars[selectionNum]) {
                quoteText.text = quotes[selectionNum];
                perkText.text = perks[selectionNum];
            }
            else {
                quoteText.text = "beat game on previous character to unlock";
                perkText.text = "";
            }
            GetComponent<SpriteRenderer>().sprite = icons[selectionNum];
            if (!preventPlayingFX) { scripts.soundManager.PlayClip("click"); }
            if (selectionNum == 0) { leftButton.transform.position = new Vector2(-8.53f, 20f); }
            else { leftButton.transform.position = new Vector2(-8.53f, 1f); }
            if (selectionNum == 3) { rightButton.transform.position = new Vector2(8.53f, 20f); }
            else { rightButton.transform.position = new Vector2(8.53f, 1f); }
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
            scripts.itemManager.floorItems[1].GetComponent<Item>().itemName = "armour";
            scripts.itemManager.floorItems[1].GetComponent<SpriteRenderer>().sprite = scripts.itemManager.GetItemSprite("armour");
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
        scripts.itemManager.floorItems[0].GetComponent<Item>().Select(false);
    }

    public void ChangeToPressed(string LeftOrRight) {
        if (LeftOrRight == "Left") { leftButton.GetComponent<CharacterSwapButton>().spriteRenderer.sprite = pressedButton; }
        else { rightButton.GetComponent<CharacterSwapButton>().spriteRenderer.sprite = pressedButton; }
    }

    public void ChangeToReleased(string LeftOrRight) {
        if (LeftOrRight == "Left") { leftButton.GetComponent<CharacterSwapButton>().spriteRenderer.sprite = releasedButton; }
        else { rightButton.GetComponent<CharacterSwapButton>().spriteRenderer.sprite = releasedButton; }
    }

    private void ToggleEasy() {
        if (!simpleFadeIn.lockChanges) {
            scripts.soundManager.PlayClip("click1");
            easy = !easy;
            StartCoroutine(simpleFadeIn.FadeHide()); 
            // change selection from the 3rd item -> 2nd one
        }
    }
}
