using UnityEngine;

/// <summary>
/// Handles mouse interaction for the left/right character selection buttons.
/// </summary>
public class CharacterSwapButton : MonoBehaviour {
    [SerializeField] private string leftOrRight; // which direction this button moves the character selection
    private CharacterSelector characterSelector; // owning selector that updates the portrait and loadout preview
    public SpriteRenderer spriteRenderer; // renderer exposed so the selector can swap button sprites directly

    /// <summary>
    /// Caches the active `CharacterSelector` in the scene.
    /// </summary>
    private void Start() {
        characterSelector = FindFirstObjectByType<CharacterSelector>();
    }

    /// <summary>
    /// Shows the pressed button state immediately on mouse-down.
    /// </summary>
    private void OnMouseDown() {
        characterSelector.ChangeToPressed(leftOrRight);
    }

    /// <summary>
    /// Releases the button sprite and steps the selection one slot.
    /// </summary>
    private void OnMouseUp() {
        characterSelector.ChangeToReleased(leftOrRight);
        if (leftOrRight == "Left") { characterSelector.SetSelection(characterSelector.selectionNum - 1); }
        else { characterSelector.SetSelection(characterSelector.selectionNum + 1); }
    }
}
