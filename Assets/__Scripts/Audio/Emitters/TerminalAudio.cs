using System;
using System.Collections.Generic;
using UnityEngine;

public class TerminalAudio : MonoBehaviour
{
    private static event Action<TerminalSoundType> OnSoundTriggered;

    [Header("Clips")]
    [SerializeField] private RandomSound clickSounds;
    [SerializeField] private RandomSound releaseSounds;
    [SerializeField] private RandomSound hoverSounds;

    [Space]
    [SerializeField] private AudioClip bigTextSound;
    [SerializeField] private AudioClip smallTextSound;

    [Header("Sources")]
    [SerializeField] private AudioSource clickSource;
    [SerializeField] private AudioSource releaseSource;
    [SerializeField] private AudioSource hoverSource;

    [Space]
    [SerializeField] private AudioSource notableSource;
    [SerializeField] private AudioSource backgroundSource;


    public static void PlayTerminalSound(TerminalSoundType soundType)
    {
        OnSoundTriggered?.Invoke(soundType);
    }


    private void StartSound(AudioSource source, AudioClip clip)
    {
        source.Stop();
        source.clip = clip;
        source.Play();
    }


    private void PlaySound(TerminalSoundType soundType)
    {
        switch(soundType)
        {
            case TerminalSoundType.Click:
                StartSound(clickSource, clickSounds.GetClip());
                return;
            case TerminalSoundType.Release:
                StartSound(releaseSource, releaseSounds.GetClip());
                return;
            case TerminalSoundType.Hover:
                StartSound(hoverSource, hoverSounds.GetClip());
                return;
        }
    }


    private void Start()
    {
        OnSoundTriggered += PlaySound;
    }
}


public enum TerminalSoundType
{
    Click,
    Release,
    Hover,
    TextBig,
    TextSmall
}


[Serializable]
public class RandomSound
{
    public List<AudioClip> Clips;


    public AudioClip GetClip()
    {
        if(Clips.Count == 0)
        {
            return null;
        }

        int index = (int)Mathf.Floor(UnityEngine.Random.Range(0, Clips.Count));
        return Clips[index];
    }
}