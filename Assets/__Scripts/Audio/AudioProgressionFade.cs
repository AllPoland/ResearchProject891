using System.Collections;
using UnityEngine;

public class AudioProgressionFade : MonoBehaviour
{
    [SerializeField] private ProgressionRange enableRange;

    [Space]
    [SerializeField] private AudioSource source;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField] private float fadeTime = 1f;

    private bool fading;
    private Coroutine fadeCoroutine;


    private IEnumerator FadeVolumeCoroutine(float startVolume, float endVolume)
    {
        fading = true;

        if(endVolume > 0.001f)
        {
            source.enabled = true;
            source.Play();
        }

        float t = 0;
        while(t < fadeTime)
        {
            source.volume = Mathf.Lerp(startVolume, endVolume, t);

            t += Time.deltaTime / fadeTime;
            yield return null;
        }

        source.volume = endVolume;
        if(endVolume <= 0.001f)
        {
            source.Stop();
            source.enabled = false;
        }

        fading = false;
    }


    private void FadeVolume(float startVolume, float newVolume)
    {
        if(fading)
        {
            //Keep the current volume to not make a re-transition jarring
            //This shouldn't really happen anyway
            startVolume = source.volume;
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeVolumeCoroutine(startVolume, newVolume));
    }


    private void UpdateProgressionStage(int newStage)
    {
        if((!source.isPlaying || fading) && enableRange.CheckInRange(newStage))
        {
            FadeVolume(0f, volume);
        }
        else if(source.isPlaying || fading)
        {
            FadeVolume(volume, 0f);
        }
    }


    private void OnEnable()
    {
        ProgressionManager.OnProgressionStageUpdated += UpdateProgressionStage;
        UpdateProgressionStage(ProgressionManager.ProgressionStage);
    }


    private void OnDisable()
    {
        ProgressionManager.OnProgressionStageUpdated -= UpdateProgressionStage;
    }
}