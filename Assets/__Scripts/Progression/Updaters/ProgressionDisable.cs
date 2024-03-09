using System;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionDisable : MonoBehaviour
{
    [SerializeField] public List<ProgressionRange> EnableRanges;

    [NonSerialized] public bool initialized = false;


    private void UpdateProgressionStage(int stage)
    {
        foreach(ProgressionRange range in EnableRanges)
        {
            if(stage >= range.minProgression && stage < range.maxProgression)
            {
                //This object should be enabled
                gameObject.SetActive(true);
                return;
            }
        }

        //The current stage is outside this object's enable range, disable it
        gameObject.SetActive(false);
    }


    public void Init()
    {
        ProgressionManager.OnProgressionStageUpdated += UpdateProgressionStage;
        UpdateProgressionStage(ProgressionManager.ProgressionStage);

        initialized = true;
    }


    private void OnEnable()
    {
        if(!initialized)
        {
            Init();
        }
    }
}