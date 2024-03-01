using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ScrollingText))]
public class ObjectiveText : MonoBehaviour
{
    [SerializeField] private List<ObjectivePrompt> prompts;

    private ScrollingText scrollingText;
    private int knownStage = -1;


    private void UpdateProgressionStage(int stage)
    {
        //Don't scroll the text yet if the player isn't looking here
        if(!TerminalScreen.TerminalActive)
        {
            scrollingText.SetText("");
            return;
        }

        //Don't rescroll the text when re-enabling and the stage hasn't changed
        if(stage == knownStage)
        {
            scrollingText.SkipScroll();
            return;
        }

        //Find the first prompt that should be used for this stage and display it
        foreach(ObjectivePrompt prompt in prompts)
        {
            if(prompt.ProgressionStage == stage)
            {
                //This prompt should be displayed, start displaying text
                scrollingText.SetText(prompt.Text);
                break;
            }
        }

        if(!scrollingText.Scrolling)
        {
            //Didn't find any prompt, so clear the string
            scrollingText.SetText("");
        }

        knownStage = stage;
    }


    private void UpdateTerminalActive(bool terminalActive)
    {
        if(terminalActive)
        {
            UpdateProgressionStage(ProgressionManager.ProgressionStage);
        }
    }


    private void OnEnable()
    {
        if(!scrollingText)
        {
            scrollingText = GetComponent<ScrollingText>();
        }

        ProgressionManager.OnProgressionStageUpdated += UpdateProgressionStage;
        TerminalScreen.OnTerminalToggled += UpdateTerminalActive;

        UpdateProgressionStage(ProgressionManager.ProgressionStage);
    }


    private void OnDisable()
    {
        ProgressionManager.OnProgressionStageUpdated -= UpdateProgressionStage;
        TerminalScreen.OnTerminalToggled -= UpdateTerminalActive;
    }


    [Serializable]
    public struct ObjectivePrompt
    {
        [TextArea(1, 3)]
        public string Text;
        public int ProgressionStage;
    }
}