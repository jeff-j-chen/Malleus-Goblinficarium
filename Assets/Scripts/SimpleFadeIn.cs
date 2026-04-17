using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SimpleFadeIn : MonoBehaviour
{
    [SerializeField] public SpriteRenderer boxSR;
    [SerializeField] public bool lockChanges = false;
    private readonly WaitForSeconds zeroPointTwoFive = new(0.25f);
    private readonly WaitForSeconds pointZeroThreeThree = new(0.033f);
    // can't use s.delays because its weird
    private Scripts s;
    
    private void Start() {
        s = FindFirstObjectByType<Scripts>();
        boxSR = GetComponent<SpriteRenderer>();
        if (SceneManager.GetActiveScene().name == "CharSelect") { 
            Color temp = boxSR.color;
            temp.a = 0;
            boxSR.color = temp;
        }
        else { StartCoroutine(FadeIn()); }
        // assign necessary variables
    }

    /// <summary>
    /// Coroutine to fade in the scene.
    /// </summary>
    private IEnumerator FadeIn() {
        Color temp = boxSR.color;
        temp.a = 1;
        boxSR.color = temp;
        yield return zeroPointTwoFive;
        for (int i = 0; i < 15; i++) {
            yield return pointZeroThreeThree;
            temp.a -= 1f/15f;
            boxSR.color = temp;
        }
        temp.a = 0;
        boxSR.color = temp;
    }

    /// <summary>
    /// Coroutine to fade out, then fade back in the scene.
    /// </summary>
    public IEnumerator FadeHide() { 
        lockChanges = true;
        Color temp = boxSR.color;
        temp.a = 0;
        boxSR.color = temp;
        for (int i = 0; i < 7; i++) {
            yield return pointZeroThreeThree;
            temp.a += 1f/7f;
            boxSR.color = temp;
        }
        s.characterSelector.UpdatePerkText();
        s.characterSelector.HideItemsByDifficulty();
        for (int i = 0; i < 7; i++) {
            yield return pointZeroThreeThree;
            temp.a -= 1f/7f;
            boxSR.color = temp;
        }
        temp.a = 0;
        boxSR.color = temp;
        lockChanges = false;
    }
}
