using System.Collections;
using UnityEngine;

public class AudioProgressionFade : MonoBehaviour
{
    [SerializeField] private ProgressionRange enableRange;

    [Space]
    [SerializeField] private AudioSource source;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField] private float fadeInTime = 1f;
    [SerializeField] private float fadeOutTime = 1f;

    private bool audioActive;

    private bool fading;
    private Coroutine fadeCoroutine;


    private IEnumerator FadeVolumeCoroutine(float startVolume, float endVolume, float fadeTime)
    {
        fading = true;

        if(endVolume > 0.001f)
        {
            source.enabled = true;
            if(!source.isPlaying)
            {
                source.Play();
            }
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


    private void FadeVolume(float startVolume, float newVolume, float fadeTime)
    {
        if(fading)
        {
            //Keep the current volume to not make a re-transition jarring
            //This shouldn't really happen anyway
            startVolume = source.volume;
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeVolumeCoroutine(startVolume, newVolume, fadeTime));
    }


    private void UpdateProgressionStage(int newStage)
    {
        bool enable = enableRange.CheckInRange(newStage);
        if(enable && !audioActive)
        {
            FadeVolume(0f, volume, fadeInTime);
            audioActive = true;
        }
        else if(!enable && audioActive)
        {
            FadeVolume(volume, 0f, fadeOutTime);
            audioActive = false;
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