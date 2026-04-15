using UnityEngine;
using UnityEngine.SceneManagement;
public class BackToMenu : MonoBehaviour {
    public float transitionMultiplier = 2.5f;
    private Scripts s;

    private void Awake() {
        s = FindObjectOfType<Scripts>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            GoBack();
        }
    }
    public void GoBack() {
        // on escape pressed
        if (s != null && s.player != null && s.turnManager != null && !s.turnManager.isMoving) {
            // if in game and not moving
            if (s.tutorial == null) { Save.SaveGame(); }
            Save.SavePersistent();
            // Save data first
        }
        SceneManager.LoadScene("Menu");
        // exit back to the menu scene
    }
}