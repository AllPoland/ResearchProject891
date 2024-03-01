using System;
using System.Collections.Generic;
using UnityEngine;

public class TerminalProgressionSetter : MonoBehaviour
{
    [SerializeField] private List<TerminalProgressionStage> enterStages;
    [SerializeField] private List<TerminalProgressionStage> exitStages;


    private void UpdateProgression(List<TerminalProgressionStage> stages)
    {
        foreach(TerminalProgressionStage stage in stages)
        {
            if(stage.StartStage == ProgressionManager.ProgressionStage)
            {
                //This terminal interaction progressed the game
                ProgressionManager.ProgressionStage = stage.TargetStage;
                return;
            }
        }
    }


    private void UpdateProgression()
    {
        if(TerminalScreen.TerminalActive)
        {
            UpdateProgression(enterStages);
        }
        else UpdateProgression(exitStages);
    }


    private void Start()
    {
        PlayerController.OnTerminalTransitionFinished += UpdateProgression;
    }


    [Serializable]
    private struct TerminalProgressionStage
    {
        public int StartStage;
        public int TargetStage;
    }
}