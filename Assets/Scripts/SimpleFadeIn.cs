using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleFadeIn : MonoBehaviour
{
    [SerializeField] public SpriteRenderer boxSR;
    [SerializeField] public bool lockChanges = false;
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
    }

    private IEnumerator FadeIn() {
        Color temp = boxSR.color;
        temp.a = 1;
        boxSR.color = temp;
        yield return scripts.delays[0.25f];
        for (int i = 0; i < 15; i++) {
            yield return scripts.delays[0.033f];
            temp.a -= 1f/15f;
            boxSR.color = temp;
        }
        temp.a = 0;
        boxSR.color = temp;
    }

    public IEnumerator FadeHide() { 
        lockChanges = true;
        Color temp = boxSR.color;
        temp.a = 0;
        boxSR.color = temp;
        for (int i = 0; i < 7; i++) {
            yield return scripts.delays[0.033f];
            temp.a += 1f/7f;
            boxSR.color = temp;
        }
        for (int i = 0; i < 7; i++) {
            yield return scripts.delays[0.033f];
            temp.a -= 1f/7f;
            boxSR.color = temp;
        }
        temp.a = 0;
        boxSR.color = temp;
        lockChanges = false;
    }
}
