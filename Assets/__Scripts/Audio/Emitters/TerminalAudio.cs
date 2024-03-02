using System;
using System.Collections.Generic;
using UnityEngine;

public class TerminalAudio : MonoBehaviour
{
    private static event Action<TerminalSoundType> OnSoundTriggered;
    private static event Action<AudioClip, TerminalSoundType> OnCustomSoundTriggered;

    [Header("Clips")]
    [SerializeField] private RandomSound clickSounds;
    [SerializeField] private RandomSound releaseSounds;
    [SerializeField] private RandomSound hoverSounds;
    [SerializeField] private RandomSound textSounds;

    [Header("Sources")]
    [SerializeField] private AudioSource clickSource;
    [SerializeField] private AudioSource releaseSource;
    [SerializeField] private AudioSource hoverSource;
    [SerializeField] private AudioSource textSource;

    [Space]
    [SerializeField] private AudioSource notableSource;
    [SerializeField] private AudioSource backgroundSource;


    public static void PlayTerminalSound(TerminalSoundType soundType)
    {
        OnSoundTriggered?.Invoke(soundType);
    }


    public static void PlayTerminalSound(TerminalSoundType soundType, AudioClip clip)
    {
        OnCustomSoundTriggered?.Invoke(clip, soundType);
    }


    private void StartSound(AudioSource source, AudioClip clip)
    {
        if(clip == null)
        {
            Debug.LogWarning("Tried to play a null AudioClip!");
            return;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        //SUUUUUUUUUUPER hacky fix for a weird audio bug
        //Webgl refuses to update spatial stuff when the camera and source haven't moved
        //which leads sounds potentially always playing from the direction they were when
        //when the player was last moving during a sound

        //This fixes the issue by moving the audio source a tiny bit to poke Webgl into updating
        source.transform.localPosition = new Vector3(UnityEngine.Random.Range(-0.01f, 0.01f), 0f);
#endif

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
            case TerminalSoundType.Text:
                StartSound(textSource, textSounds.GetClip());
                return;
        }
    }


    private void PlaySound(AudioClip clip, TerminalSoundType soundType)
    {
        switch(soundType)
        {
            case TerminalSoundType.Click:
                StartSound(clickSource, clip);
                return;
            case TerminalSoundType.Release:
                StartSound(releaseSource, clip);
                return;
            case TerminalSoundType.Hover:
                StartSound(hoverSource, clip);
                return;
            case TerminalSoundType.Text:
                StartSound(textSource, clip);
                return;
            case TerminalSoundType.Notable:
                StartSound(notableSource, clip);
                return;
        }
    }


    private void Start()
    {
        OnSoundTriggered += PlaySound;
        OnCustomSoundTriggered += PlaySound;
    }
}


public enum TerminalSoundType
{
    Click,
    Release,
    Hover,
    Text,
    Notable
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