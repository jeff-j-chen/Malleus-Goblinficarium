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
            // on escape pressed
            if (scripts is not null && scripts.player is not null && scripts.turnManager is not null && !scripts.turnManager.isMoving) { 
                // if in game and not moving
                if (scripts.tutorial is null) { Save.SaveGame(); }
                Save.SavePersistent();
                // Save data first
            }
            SceneManager.LoadScene("Menu");
            // exit back to the menu scene
        }
    }
}