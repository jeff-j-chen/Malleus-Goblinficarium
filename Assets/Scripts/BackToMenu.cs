using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Returns the player to the main menu when escape or a back button is used.
/// </summary>
public class BackToMenu : MonoBehaviour {
    public float transitionMultiplier = 2.5f; // shared fade speed used by other scene-transition callers
    private Scripts s; // central project references used to query combat state before leaving

    /// <summary>
    /// Caches shared scene references as soon as this object is created.
    /// </summary>
    private void Awake() {
        s = FindFirstObjectByType<Scripts>();
    }

    /// <summary>
    /// Allows escape to return to the menu when the current state permits it.
    /// </summary>
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            GoBack();
        }
    }

    /// <summary>
    /// Saves current run state when appropriate, then loads the menu scene.
    /// </summary>
    public void GoBack() {
        if (s != null && s.turnManager != null && !s.turnManager.CanEscapeToMenu()) {
            // some combat states intentionally lock escape to prevent breaking scripted flows
            return;
        }

        if (s != null && s.player != null && s.turnManager != null && !s.turnManager.isMoving) {
            // tutorial runs do not persist a combat save, but persistent unlock/stat data still should
            if (s.tutorial == null) { Save.SaveGame(); }
            Save.SavePersistent();
        }
        SceneManager.LoadScene("Menu");
    }
}