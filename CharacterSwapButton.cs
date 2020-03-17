using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwapButton : MonoBehaviour {
    [SerializeField] private string LeftOrRight;
    private CharacterSelector characterSelector;
    public SpriteRenderer spriteRenderer;

    private void Start() {
        characterSelector = FindObjectOfType<CharacterSelector>();
    }

    private void OnMouseOver() {
        characterSelector.ChangeToPressed(LeftOrRight);
    }

    private void OnMouseExit() {
        characterSelector.ChangeToReleased(LeftOrRight);
    }

    private void OnMouseDown() {
        if (LeftOrRight == "Left") { characterSelector.SetSelection(characterSelector.selectionNum - 1); }
        else { characterSelector.SetSelection(characterSelector.selectionNum + 1); }
    }
}
