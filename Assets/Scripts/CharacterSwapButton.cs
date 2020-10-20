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
        characterSelector.ChangeToPressed(LeftOrRight);
        if (LeftOrRight == "Left") { characterSelector.SetSelection(characterSelector.selectionNum - 1); }
        else { characterSelector.SetSelection(characterSelector.selectionNum + 1); }
    }

    private void OnMouseUp() {
        characterSelector.ChangeToReleased(LeftOrRight);
    }
}
