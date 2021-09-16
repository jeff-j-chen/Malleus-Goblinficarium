using UnityEngine;
using UnityEngine.SceneManagement;
public class BackToMenu : MonoBehaviour {
    public float transitionMultiplier = 2.5f;
    private Scripts scripts;

    private void Awake() {
        scripts = FindObjectOfType<Scripts>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            GoBack();
        }
    }
    public void GoBack() {
        // on escape pressed
        if (scripts != null && scripts.player != null && scripts.turnManager != null && !scripts.turnManager.isMoving) {
            // if in game and not moving
            if (scripts.tutorial == null) { Save.SaveGame(); }
            Save.SavePersistent();
            // Save data first
        }
        SceneManager.LoadScene("Menu");
        // exit back to the menu scene
    }
}