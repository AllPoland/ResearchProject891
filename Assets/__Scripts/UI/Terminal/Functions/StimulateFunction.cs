using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StimulateFunction : MonoBehaviour
{
    //Manages the stimulate minigame, a mini rotating puzzle
    public static event Action OnStimulateFunctionTriggered;

    [SerializeField] private List<StimulateChargeIcon> chargeIcons;

    [SerializeField] private StimulateDisc bottomLeftDisc;
    [SerializeField] private StimulateDisc topLeftDisc;
    [SerializeField] private StimulateDisc bottomRightDisc;
    [SerializeField] private StimulateDisc topRightDisc;

    [Space]
    [SerializeField] private Image[] backgrounds;

    [Space]
    [SerializeField] public UnityEvent OnFunctionTriggered;


    private void SetBackgroundsActive(bool active)
    {
        foreach(Image image in backgrounds)
        {
            image.enabled = active;
        }
    }


    private bool CheckWinState()
    {
        foreach(StimulateChargeIcon chargeIcon in chargeIcons)
        {
            if(!chargeIcon.correctPos)
            {
                //There's still a charge icon not in the correct position
                return false;
            }
        }

        return true;
    }


    public void UpdateWinState()
    {
        if(!CheckWinState())
        {
            return;
        }

        //All the charge icons are correct
        Debug.Log("Stimulation function complete.");

        bottomLeftDisc.Deactivate();
        topLeftDisc.Deactivate();
        bottomRightDisc.Deactivate();
        topRightDisc.Deactivate();

        SetBackgroundsActive(false);

        OnFunctionTriggered?.Invoke();
        OnStimulateFunctionTriggered?.Invoke();
    }


    public void StartGame()
    {
        List<int> unusedPositions = new List<int>
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11
        };

        foreach(StimulateChargeIcon chargeIcon in chargeIcons)
        {
            int randomIndex = UnityEngine.Random.Range(0, unusedPositions.Count);
            int newPosition = unusedPositions[randomIndex];

            //Positions go from top left, reading left-to-right before going down a row
            //This is yucky but it's a game jam and I can't be assed to figure out a smart way of doing this
            switch(newPosition)
            {
                //Top row
                case 0:
                    topLeftDisc.chargeIcons[0] = chargeIcon;
                    break;
                case 1:
                    topRightDisc.chargeIcons[0] = chargeIcon;
                    break;
                //Upper row
                case 2:
                    topLeftDisc.chargeIcons[3] = chargeIcon;
                    break;
                case 3:
                    topLeftDisc.chargeIcons[1] = chargeIcon;
                    topRightDisc.chargeIcons[3] = chargeIcon;
                    break;
                case 4:
                    topRightDisc.chargeIcons[1] = chargeIcon;
                    break;
                //Mid row
                case 5:
                    topLeftDisc.chargeIcons[2] = chargeIcon;
                    bottomLeftDisc.chargeIcons[0] = chargeIcon;
                    break;
                case 6:
                    topRightDisc.chargeIcons[2] = chargeIcon;
                    bottomRightDisc.chargeIcons[0] = chargeIcon;
                    break;
                //Lower row
                case 7:
                    bottomLeftDisc.chargeIcons[3] = chargeIcon;
                    break;
                case 8:
                    bottomLeftDisc.chargeIcons[1] = chargeIcon;
                    bottomRightDisc.chargeIcons[3] = chargeIcon;
                    break;
                case 9:
                    bottomRightDisc.chargeIcons[1] = chargeIcon;
                    break;
                //Bottom row
                case 10:
                    bottomLeftDisc.chargeIcons[2] = chargeIcon;
                    break;
                case 11:
                    bottomRightDisc.chargeIcons[2] = chargeIcon;
                    break;
            }

            unusedPositions.Remove(newPosition);
        }

        bottomLeftDisc.UpdateIcons(false);
        topLeftDisc.UpdateIcons(false);
        bottomRightDisc.UpdateIcons(false);
        topRightDisc.UpdateIcons(false);

        bottomLeftDisc.Activate();
        topLeftDisc.Activate();
        bottomRightDisc.Activate();
        topRightDisc.Activate();

        if(CheckWinState())
        {
            //Reroll if we end up with the win state
            //Hopefully nobody rolls a win thousands of times in a row
            StartGame();
        }
    }


    private void OnEnable()
    {
        SetBackgroundsActive(true);
        StartGame();
    }
}