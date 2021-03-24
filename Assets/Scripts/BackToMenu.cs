using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    public float transitionMultiplier = 2.5f;
    Scripts scripts;

    private void Awake() {
        scripts = FindObjectOfType<Scripts>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (scripts != null && scripts.player != null) { 
                scripts.SaveGameData();
                scripts.SavePersistentData();
            }
            SceneManager.LoadScene("Menu");
            // Initiate.Fade("Menu", Color.black, transitionMultiplier);
        }
    }
}