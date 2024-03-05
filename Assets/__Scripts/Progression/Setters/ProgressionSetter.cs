using UnityEngine;

public class ProgressionSetter : MonoBehaviour
{
    public void SetProgression(int stage)
    {
        ProgressionManager.ProgressionStage = stage;
    }


    public void SetProgression(string stageString)
    {
        if(int.TryParse(stageString, out int stage))
        {
            SetProgression(stage);
        }
    }
}