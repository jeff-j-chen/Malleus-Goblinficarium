using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    public float transitionMultiplier = 2.5f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("Menu");
            // Initiate.Fade("Menu", Color.black, transitionMultiplier);
        }
    }
}