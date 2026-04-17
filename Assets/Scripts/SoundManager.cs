using System;
using UnityEngine;
public class SoundManager : MonoBehaviour {
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private string[] audioClipNames;
    private AudioSource audioSource;
    private Scripts s;

    private void Start() {
        s = FindFirstObjectByType<Scripts>();
        audioSource = GetComponent<AudioSource>();
        audioClipNames = new string[audioClips.Length];
        for (int i = 0; i < audioClips.Length; i++) {
            audioClipNames[i] = audioClips[i].name;
        }
        audioSource.volume = PlayerPrefs.GetString(s.SOUNDS_KEY) == "on" ? 1f : 0f;
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

    /// <summary>
    /// Get the length of a sound clip by name.
    /// </summary>
    public float GetClipLength(string clipName) {
        int clipIndex = Array.IndexOf(audioClipNames, clipName);
        if (clipIndex < 0) { return 0f; }
        return audioClips[clipIndex].length;
    }
}
