using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFadeIn : MonoBehaviour
{
    private SpriteRenderer boxSR;
    void Start() {
        boxSR = FindObjectOfType<SpriteRenderer>();
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn() {
        Color temp = boxSR.color;
        temp.a = 1;
        boxSR.color = temp;
        yield return new WaitForSeconds(0.25f);
        for (int i = 0; i < 15; i++) {
            yield return new WaitForSeconds(0.033f);
            temp.a -= 1f/15f;
            boxSR.color = temp;
        }
        temp.a = 0;
        boxSR.color = temp;
    }
}
