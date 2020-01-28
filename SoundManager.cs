using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;
    [SerializeField] string[] audioClipNames;
    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClip(string clipName)
    {
        audioSource.PlayOneShot(audioClips[Array.IndexOf(audioClipNames, clipName)]);
    }
    public void PlayClip(int clipIndex)
    {
        audioSource.PlayOneShot(audioClips[clipIndex]);
    }
}
