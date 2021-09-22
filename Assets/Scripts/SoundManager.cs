using System;
using UnityEngine;
public class SoundManager : MonoBehaviour {
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private string[] audioClipNames;
    private AudioSource audioSource;
    private Scripts scripts;

    private void Start() {
        scripts = FindObjectOfType<Scripts>();
        audioSource = GetComponent<AudioSource>();
        audioClipNames = new string[audioClips.Length];
        for (int i = 0; i < audioClips.Length; i++) {
            audioClipNames[i] = audioClips[i].name;
        }
        audioSource.volume = PlayerPrefs.GetString(scripts.SOUNDS_KEY) == "on" ? 1f : 0f;
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
