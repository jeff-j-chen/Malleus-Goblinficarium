using UnityEngine;

public class BackToMenu : MonoBehaviour
{
    public float transitionMultiplier = 2.5f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Initiate.Fade("Menu", Color.black, transitionMultiplier);
        }
    }
}