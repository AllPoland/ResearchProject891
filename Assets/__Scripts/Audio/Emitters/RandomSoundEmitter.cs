using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomSoundEmitter : MonoBehaviour
{
    private AudioSource source;

    [SerializeField] private RandomSound sounds;


    public void PlaySound()
    {
        AudioClip clip = sounds.GetClip();
        if(clip != null)
        {
            source.clip = clip;
            source.Play();
        }
    }


    private void OnEnable()
    {
        if(!source)
        {
            source = GetComponent<AudioSource>();
        }
    }
}


[Serializable]
public class RandomSound
{
    public List<AudioClip> Clips;

    private int previousIdx = -1;
    private int streak = 1;


    public AudioClip GetClip()
    {
        if(Clips.Count == 0)
        {
            return null;
        }

        int index = 0;
        for(int i = 0; i < streak; i++)
        {
            //Reroll the sound again if it's a repeat, up to the number of times we've already repeated
            //Kind of a scuffed way to weight the odds and reduce repeat sounds
            index = UnityEngine.Random.Range(0, Clips.Count);
            if(index != previousIdx)
            {
                break;
            }
        }

        if(index == previousIdx)
        {
            streak++;
        }
        else
        {
            previousIdx = index;
            streak = 1;
        }

        return Clips[index];
    }
}