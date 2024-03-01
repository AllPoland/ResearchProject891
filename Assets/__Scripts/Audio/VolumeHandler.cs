using UnityEngine;
using UnityEngine.Audio;

public class VolumeHandler : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string volumeAttribute;


    public void SetVolume(float volume)
    {
        if(volume <= 0)
        {
            volume = 0.0001f;
        }
        //Logarithmic scaling makes volume slider feel more natural to the user
        mixer.SetFloat(volumeAttribute, Mathf.Log10(volume) * 20);
    }
}