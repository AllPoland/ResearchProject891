using System.Collections;
using UnityEngine;

public class ForestLightHandler : MonoBehaviour
{
    [SerializeField] private Light forestLight;
    [SerializeField] private float forestLightIntensity = 1f;

    [Space]
    [SerializeField] private float transitionTime = 1f;

    private bool forestLightOn = false;

    private bool transitioning = false;
    private Coroutine transitionCoroutine;


    private IEnumerator TransitionLightsCoroutine()
    {
        transitioning = true;

        forestLight.intensity = 0f;

        float t = 0;
        while(t < 1f)
        {
            forestLight.intensity = t * forestLightIntensity;

            t += Time.deltaTime / transitionTime;
            yield return null;
        }

        forestLight.intensity = forestLightIntensity;
        forestLightOn = true;

        transitioning = false;
    }


    private void TransitionLights()
    {
        if(transitioning)
        {
            StopCoroutine(transitionCoroutine);
        }

        transitionCoroutine = StartCoroutine(TransitionLightsCoroutine());
    }


    private void UpdateTerminalActive(bool terminalActive)
    {
        if(!terminalActive && !forestLightOn)
        {
            TransitionLights();
        }
    }


    private void OnEnable()
    {
        forestLight.intensity = 0f;
        forestLightOn = false;

        TerminalScreen.OnTerminalToggled += UpdateTerminalActive;
    }


    private void OnDisable()
    {
        if(transitioning)
        {
            StopCoroutine(transitionCoroutine);
        }

        TerminalScreen.OnTerminalToggled -= UpdateTerminalActive;
    }
}