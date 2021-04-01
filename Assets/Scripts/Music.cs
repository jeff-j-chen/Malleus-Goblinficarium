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
    private void Awake() {
        SetUpSingleton();
        audioSource = GetComponent<AudioSource>();
        scripts = FindObjectOfType<Scripts>();
    }
    private void Start() {
        PlayMusic("Intro");
    }

    public void PlayMusic(string pieceName) {
        audioSource.clip = musicPieces[Array.IndexOf(musicPieceNames, pieceName)];
        audioSource.Play();
    }

    public void FadeVolume() { StartCoroutine(FadeVolumeCoro()); }

    public void FadeVolume(String pieceName) { StartCoroutine(FadeVolumeCoro(pieceName)); }
    
    private IEnumerator FadeVolumeCoro() { 
        for (int i = 0; i < 5; i++) {
            yield return scripts.delays[0.05f];
            audioSource.volume -= 0.15f;
        }
        audioSource.volume = 0;
        yield return scripts.delays[0.75f];
        for (int i = 0; i < 5; i++) {
            yield return scripts.delays[0.05f];
            audioSource.volume += 0.15f;
        }
        audioSource.volume = 0.5f;
    }

    private IEnumerator FadeVolumeCoro(string pieceName) { 
        for (int i = 0; i < 5; i++) {
            yield return scripts.delays[0.05f];
            audioSource.volume -= 0.1f;
        }
        audioSource.Stop();
        audioSource.volume = 0;
        yield return scripts.delays[1f];
        PlayMusic(pieceName);
        for (int i = 0; i < 5; i++) {
            yield return scripts.delays[0.05f];
            audioSource.volume += 0.1f;
        }
        audioSource.volume = 0.5f;
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
