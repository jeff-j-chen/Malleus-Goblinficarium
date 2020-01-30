using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {
    private void Awake() {
        SetUpSingleton();
        GetComponent<AudioSource>().ignoreListenerVolume = true;
        // play the music through the audio mute (but not through the music mute)
    }

    private void SetUpSingleton() {
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
        else {
            DontDestroyOnLoad(gameObject);
        }
    }
    // make sure the music doesn't cut off weirdly
}
