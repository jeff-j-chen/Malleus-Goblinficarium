using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterSwapButton : MonoBehaviour {
    [SerializeField] private string leftOrRight;
    private CharacterSelector characterSelector;
    public SpriteRenderer spriteRenderer;

    private void Start() {
        characterSelector = FindObjectOfType<CharacterSelector>();
    }

    private void OnMouseDown() {
        // when the mouse is pressed down
        characterSelector.ChangeToPressed(leftOrRight);
        // change the button sprite
        if (leftOrRight == "Left") { characterSelector.SetSelection(characterSelector.selectionNum - 1); }
        else { characterSelector.SetSelection(characterSelector.selectionNum + 1); }
        // move the selection left or right depending on which button was pressed
    }

    private void OnMouseUp() {
        characterSelector.ChangeToReleased(leftOrRight);
        // change the sprite back only when the button is released
    }
}
