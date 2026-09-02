using UnityEngine;

/// <summary>
/// Handles mouse interaction for one of the almanac page-swap buttons.
/// </summary>
public class AlmanacSwapButton : MonoBehaviour {
    [SerializeField] private string leftOrRight; // which page direction this button controls: Left or Right
    [SerializeField] private Sprite releasedButton; // sprite shown when the button is idle
    [SerializeField] private Sprite pressedButton; // sprite shown while the mouse is held down
    private AlmanacController almanacController; // owning controller that actually swaps pages
    public SpriteRenderer spriteRenderer; // cached renderer so controller scripts can swap sprites directly

    /// <summary>
    /// Caches the almanac controller in the active scene.
    /// </summary>
    private void Start() {
        almanacController = FindFirstObjectByType<AlmanacController>();
    }

    /// <summary>
    /// Shows the pressed state as soon as the mouse button goes down.
    /// </summary>
    private void OnMouseDown() {
        spriteRenderer.sprite = pressedButton;
        almanacController.ChangeToPressed(leftOrRight);
    }

    /// <summary>
    /// Restores the button sprite and asks the controller to step pages.
    /// </summary>
    private void OnMouseUp() {
        spriteRenderer.sprite = releasedButton;
        // route through the controller so page rebuilding, audio, and button visibility stay centralized
        if (leftOrRight == "Left") { almanacController.StepPage(-1); }
        else { almanacController.StepPage(1); }
        almanacController.ChangeToReleased(leftOrRight);
    }
}
