using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterSelector : MonoBehaviour {
    [SerializeField] private int selectionNum;
    [SerializeField] private bool easy;
    [SerializeField] private Sprite[] icons; 
    [SerializeField] private string[] quotes; 
    [SerializeField] private string[] perks; 
    [SerializeField] private Sprite releasedButton;
    [SerializeField] private Sprite pressedButton;
    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject rightButton;
    [SerializeField] private TextMeshProUGUI quoteText;
    [SerializeField] private TextMeshProUGUI perkText;
    private Scripts scripts;
    private void Start() {
        selectionNum = 1;
        easy = false;
        scripts = FindObjectOfType<Scripts>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { ChangeToPressed("Left"); }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) { ChangeToPressed("Right"); }
        else if (Input.GetKeyUp(KeyCode.LeftArrow)) { MoveLeft(); }
        else if (Input.GetKeyUp(KeyCode.RightArrow)) { MoveRight(); }
        else if (Input.GetKeyDown(KeyCode.Space)) { ToggleEasy(); }
    }

    public void MoveLeft() {
        selectionNum = selectionNum <= 1 ? 1 : selectionNum - 1;
        // decrement selection num, no less than 1
        GetComponent<SpriteRenderer>().sprite = icons[selectionNum - 1];
        quoteText.text = quotes[selectionNum - 1];
        perkText.text = perks[selectionNum - 1];
        // set the sprites and text based on the current selection num
        scripts.soundManager.PlayClip("click");
        // play sound clip
        ChangeToReleased("Left");
        // make the button release
    }

    public void MoveRight() {
        selectionNum = selectionNum >= 4 ? 4 : selectionNum + 1;
        GetComponent<SpriteRenderer>().sprite = icons[selectionNum - 1];
        quoteText.text = quotes[selectionNum - 1];
        perkText.text = perks[selectionNum - 1];
        scripts.soundManager.PlayClip("click");
        ChangeToReleased("Right");
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
