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
    }

    private void OnMouseUp() {
        characterSelector.ChangeToReleased(LeftOrRight);
    }

    private void OnMouseUpAsButton() {
        if (LeftOrRight == "Left") { characterSelector.MoveLeft(); }
        else { characterSelector.MoveRight(); }
    }
}
