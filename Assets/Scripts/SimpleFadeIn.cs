using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleFadeIn : MonoBehaviour
{
    [SerializeField] public SpriteRenderer boxSR;
    [SerializeField] public bool lockChanges = false;
    private WaitForSeconds zeroPointTwoFive = new WaitForSeconds(0.25f);
    private WaitForSeconds pointZeroThreeThree = new WaitForSeconds(0.033f);
    // can't use scripts.delays because its weird
    Scripts scripts;
    private void Start() {
        scripts = FindObjectOfType<Scripts>();
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
        if (scripts.characterSelector.easy) { scripts.itemManager.floorItems[2].GetComponent<Item>().UnHide(); }
        else { scripts.itemManager.floorItems[2].GetComponent<Item>().Hide(); }
        scripts.itemManager.floorItems[1].GetComponent<Item>().Select(false);
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
