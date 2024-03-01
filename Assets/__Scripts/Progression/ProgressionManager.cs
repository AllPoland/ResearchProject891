using System;
using UnityEngine;

public class ProgressionManager
{
    private static int _progressionStage = 0;
    public static int ProgressionStage
    {
        get => _progressionStage;
        set
        {
            _progressionStage = value;
            Debug.Log("New progression stage: " + ProgressionStage);
            OnProgressionStageUpdated?.Invoke(_progressionStage);
        }
    }

    public static event Action<int> OnProgressionStageUpdated;
}


[Serializable]
public struct ProgressionRange
{
    //Minimum progression stage to enable this object (inclusive)
    public int minProgression;
    //Maximum progression stage to enable this object (exclusive)
    public int maxProgression;


    public readonly bool CheckInRange(int stage)
    {
        return stage >= minProgression && stage < maxProgression;
    }
}