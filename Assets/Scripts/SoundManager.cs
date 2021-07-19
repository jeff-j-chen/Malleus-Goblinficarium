using System;
using UnityEngine;
public class SoundManager : MonoBehaviour {
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private string[] audioClipNames;
    private AudioSource audioSource;

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetString("sounds") == "on" ? 1f : 0f;
    }

    /// <summary>
    /// Play a sound clip with the given clip name.
    /// </summary>
    public void PlayClip(string clipName) {
        audioSource.PlayOneShot(audioClips[Array.IndexOf(audioClipNames, clipName)]);
    }

    /// <summary>
    /// Play a sound clip with the given clip index.
    /// </summary>
    public void PlayClip(int clipIndex) {
        audioSource.PlayOneShot(audioClips[clipIndex]);
    }
}
