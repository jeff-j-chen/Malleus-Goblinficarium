using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterSelector : MonoBehaviour {
    [SerializeField] public int selectionNum;
    [SerializeField] private List<int> unlockedChars = new List<int>();
    [SerializeField] private bool easy;
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
    private Scripts scripts;
    private void Start() {
        unlockedChars.Add(1);
        // allow for seleection of 1
        scripts = FindObjectOfType<Scripts>();
        selectionNum = 1;
        // start off selecting character 1
        GetComponent<SpriteRenderer>().sprite = icons[selectionNum - 1];
        quoteText.text = quotes[selectionNum - 1];
        perkText.text = perks[selectionNum - 1];
        // set necessary items
        easy = false;
        // set easy to false by default
        leftButton.transform.position = new Vector2(-8.53f, 1f);
        // start off with hiding the left button
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { ChangeToPressed("Left"); }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) { ChangeToPressed("Right"); }
        else if (Input.GetKeyUp(KeyCode.LeftArrow)) { SetSelection(selectionNum - 1); }
        else if (Input.GetKeyUp(KeyCode.RightArrow)) { SetSelection(selectionNum + 1); }
        else if (Input.GetKeyDown(KeyCode.Space)) { ToggleEasy(); }
    }

    // make this the main selection function, with checking for unlocked chars and setting arrows etc.
    public void SetSelection(int num) {
        if (num == 1 || num == 2 || num == 3 || num == 4) {
            selectionNum = num;
            if (unlockedChars.Contains(selectionNum)) {
                quoteText.text = quotes[selectionNum - 1];
                perkText.text = perks[selectionNum - 1];
            }
            else {
                quoteText.text = "beat game on previous character to unlock";
                perkText.text = "";
            }
            GetComponent<SpriteRenderer>().sprite = icons[selectionNum - 1];
            scripts.soundManager.PlayClip("click");
            if (selectionNum == 1) { leftButton.transform.position = new Vector2(-8.53f, 20f); }
            else { leftButton.transform.position = new Vector2(-8.53f, 1f); }
            if (selectionNum == 4) { rightButton.transform.position = new Vector2(8.53f, 20f); }
            else { rightButton.transform.position = new Vector2(8.53f, 1f); }
        }
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
        easy = !easy;
    }
}
