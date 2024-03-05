using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgressionSound : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;

    [Space]
    [SerializeField] private List<ProgressionRange> playRanges;


    private void PlaySound()
    {
        if(clip != null)
        {
            source.clip = clip;
        }

        source.Play();
    }


    private void UpdateProgressionStage(int stage)
    {
        if(playRanges.Any(x => x.CheckInRange(stage)))
        {
            PlaySound();
        }
    }


    private void OnEnable()
    {
        ProgressionManager.OnProgressionStageUpdated += UpdateProgressionStage;
    }


    private void OnDisable()
    {
        ProgressionManager.OnProgressionStageUpdated -= UpdateProgressionStage;
    }
}