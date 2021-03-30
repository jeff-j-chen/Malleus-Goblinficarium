using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// for music: menu screen is intro.ogg, main game is through.ogg, trader is smoke.ogg, boss is labossa.ogg
// intro music is changed to main game by cutting
// main game music will be quieted during level change
// main and trader are hard loops, assume boss is as well
// boss does a quick fade into intro after you kill him
// lich has boss music as well

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
