using UnityEngine;

public class ProgressionDisableInit : MonoBehaviour
{
    private void Start()
    {
        //This is a super yucky high overhead call to make sure every
        //ProgressionDisable component subscribes to the progression event.
        //Thankfully this only needs to happen once

        //This component could also be disabled if I build the game with everything enabled

        ProgressionDisable[] targets = GetComponentsInChildren<ProgressionDisable>(true);
        foreach(ProgressionDisable target in targets)
        {
            if(!target.isActiveAndEnabled && !target.initialized)
            {
                target.Init();
            }
        }
    }
}