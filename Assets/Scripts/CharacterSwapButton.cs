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

    private void OnMouseDown() {
        // when the mouse is pressed down
        characterSelector.ChangeToPressed(LeftOrRight);
        // change the button sprite
        if (LeftOrRight == "Left") { characterSelector.SetSelection(characterSelector.selectionNum - 1); }
        else { characterSelector.SetSelection(characterSelector.selectionNum + 1); }
        // move the selection left or right depending on which button was pressed
    }

    private void OnMouseUp() {
        characterSelector.ChangeToReleased(LeftOrRight);
        // change the psrite back only when the button is released
    }
}
