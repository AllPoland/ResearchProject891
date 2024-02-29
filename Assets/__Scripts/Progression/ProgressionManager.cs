using System;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    private static int _progressionStage = 0;
    public static int ProgressionStage
    {
        get => _progressionStage;
        set
        {
            _progressionStage = value;
            OnProgressionStageUpdated?.Invoke(_progressionStage);
        }
    }

    public static event Action<int> OnProgressionStageUpdated;
}