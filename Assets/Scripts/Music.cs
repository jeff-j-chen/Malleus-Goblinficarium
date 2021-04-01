using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
// for music: menu screen is intro.ogg, main game is through.ogg, trader is smoke.ogg, boss is labossa.ogg
// intro music is changed to main game by cutting
// main game music will be quieted during level change
// main and trader are hard loops, assume boss is as well
// boss does a quick fade into intro after you kill him
// lich has boss music as well

public class Music : MonoBehaviour {
    [SerializeField] AudioClip[] musicPieces;
    [SerializeField] string[] musicPieceNames;
    public AudioSource audioSource;
    private Scripts scripts;
    private bool shouldPlayMusic;
    private void Awake() {
        SetUpSingleton();
        audioSource = GetComponent<AudioSource>();
        scripts = FindObjectOfType<Scripts>();
        shouldPlayMusic = PlayerPrefs.GetString("music") == "on" ? true : false;
        if (shouldPlayMusic) { audioSource.volume = 0.5f; }
        else { audioSource.volume = 0f; }
    }
    private void Start() {
        PlayMusic("Intro");
        // play the intro music when intialized
    }

    /// <summary>
    /// Play a track given its name.
    /// </summary>
    public void PlayMusic(string pieceName) {
        audioSource.clip = musicPieces[Array.IndexOf(musicPieceNames, pieceName)];
        audioSource.Play();
        // set the clip for the audiosource and play it (allows for it to be stopped)
    }

    /// <summary>
    /// Fade the volume out, then back in.
    /// </summary>
    public void FadeVolume() { 
        shouldPlayMusic = PlayerPrefs.GetString("music") == "on" ? true : false;
        if (shouldPlayMusic) { StartCoroutine(FadeVolumeCoro()); }
    }

    /// <summary>
    /// Fade the volume out, change the track, and fade back in.
    /// </summary>
    public void FadeVolume(String pieceName) { 
        shouldPlayMusic = PlayerPrefs.GetString("music") == "on" ? true : false;
        if (shouldPlayMusic) { StartCoroutine(FadeVolumeCoro(pieceName)); }
    }
    
    /// <summary>
    /// Do not use this coroutine, call FadeVolume() instead.
    /// </summary>
    private IEnumerator FadeVolumeCoro() { 
        for (int i = 0; i < 5; i++) {
            yield return scripts.delays[0.05f];
            audioSource.volume -= 0.1f;
        }
        // fade the volume to 0
        audioSource.volume = 0;
        // prevent rounding errors resulting in the base level changing over time
        yield return scripts.delays[1.5f];
        // short time where there is no music
        for (int i = 0; i < 5; i++) {
            yield return scripts.delays[0.05f];
            audioSource.volume += 0.1f;
        }
        audioSource.volume = 0.5f;
        // fade it back in
    }

    /// <summary>
    /// Do not use this coroutine, use FadeVolume() instead.
    /// </summary>
    private IEnumerator FadeVolumeCoro(string pieceName) { 
        for (int i = 0; i < 5; i++) {
            yield return scripts.delays[0.05f];
            audioSource.volume -= 0.1f;
        }
        audioSource.Stop();
        audioSource.volume = 0;
        yield return scripts.delays[1.5f];
        PlayMusic(pieceName);
        for (int i = 0; i < 5; i++) {
            yield return scripts.delays[0.05f];
            audioSource.volume += 0.1f;
        }
        audioSource.volume = 0.5f;
        // same as above, only assign a new track 
    }

    private void SetUpSingleton() {
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
        else {
            DontDestroyOnLoad(gameObject);
        }
    }
    // ensure that this object persists through scenes, allowing the music to continuously play
}
