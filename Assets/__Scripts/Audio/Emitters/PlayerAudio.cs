using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private RandomSound footstepSounds;
    [SerializeField] private RandomSound shuffleSounds;

    private AudioSource source;


    private void PlayClip(AudioClip clip)
    {
        if(clip == null)
        {
            return;
        }

        source.clip = clip;
        source.Play();
    }


    public void PlayFootstep()
    {
        PlayClip(footstepSounds.GetClip());
    }


    public void PlayFootShuffle()
    {
        PlayClip(shuffleSounds.GetClip());
    }


    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }
}