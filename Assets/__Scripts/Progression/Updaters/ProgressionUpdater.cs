using System.Collections.Generic;
using UnityEngine;

public abstract class ProgressionUpdater : MonoBehaviour
{
    [SerializeField] public List<ProgressionRange> EnableRanges;


    public abstract void Enable();
    public abstract void Disable();


    private void UpdateProgressionStage(int stage)
    {
        foreach(ProgressionRange range in EnableRanges)
        {
            if(range.CheckInRange(stage))
            {
                //This object should be enabled
                Enable();
                return;
            }
        }

        //The current stage is outside this object's enable range, disable it
        Disable();
    }


    public void Start()
    {
        ProgressionManager.OnProgressionStageUpdated += UpdateProgressionStage;
        UpdateProgressionStage(ProgressionManager.ProgressionStage);
    }
}