using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    [SerializeField] AudioClip[] audioClips;
    [SerializeField] string[] audioClipNames;
    AudioSource audioSource;
    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Play a sound clip with the given clip name.
    /// </summary>
    /// <param name="clipName">The clip name to play.</param>
    public void PlayClip(string clipName) {
        audioSource.PlayOneShot(audioClips[Array.IndexOf(audioClipNames, clipName)]);
    }

    /// <summary>
    /// Play a sound clip with the given clip index.
    /// </summary>
    /// <param name="clipIndex">The clip index to play.</param>
    public void PlayClip(int clipIndex) {
        audioSource.PlayOneShot(audioClips[clipIndex]);
    }
}
