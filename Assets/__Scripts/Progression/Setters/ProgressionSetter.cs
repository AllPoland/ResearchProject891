using UnityEngine;

public class ProgressionSetter : MonoBehaviour
{
    public void SetProgression(int stage)
    {
        ProgressionManager.ProgressionStage = stage;
    }
}