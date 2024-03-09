using System.Collections;
using UnityEngine;

public class ForestFogHandler : MonoBehaviour
{
    [SerializeField] private Color roomFogColor = Color.black;
    [SerializeField] private Color forestFogColor = Color.grey;

    [Space]
    [SerializeField] private float roomFogDensity;
    [SerializeField] private float forestFogDensity;

    [Space]
    [SerializeField] private float transitionTime = 1f;

    private bool forestFogActive = false;

    private bool transitioning;
    private Coroutine transitionCoroutine;


    private void SetFogColorAndDensity(Color color, float density)
    {
        RenderSettings.fogColor = color;
        RenderSettings.fogDensity = density;

        if(Camera.main)
        {
            Camera.main.backgroundColor = color;
        }
    }


    private IEnumerator TransitionFogCoroutine()
    {
        transitioning = true;

        SetFogColorAndDensity(roomFogColor, roomFogDensity);

        float t = 0;
        while(t < 1f)
        {
            Color color = Color.Lerp(roomFogColor, forestFogColor, t);
            float density = Mathf.Lerp(roomFogDensity, forestFogDensity, t);

            SetFogColorAndDensity(color, density);

            t += Time.deltaTime / transitionTime;
            yield return null;
        }

        SetFogColorAndDensity(forestFogColor, forestFogDensity);
        forestFogActive = true;

        transitioning = false;
    }


    private void TransitionFog()
    {
        if(transitioning)
        {
            StopCoroutine(transitionCoroutine);
        }

        transitionCoroutine = StartCoroutine(TransitionFogCoroutine());
    }


    private void UpdateTerminalActive(bool terminalActive)
    {
        if(!terminalActive && !forestFogActive)
        {
            TransitionFog();
        }
    }


    private void OnEnable()
    {
        SetFogColorAndDensity(roomFogColor, roomFogDensity);
        forestFogActive = false;

        TerminalScreen.OnTerminalToggled += UpdateTerminalActive;
    }


    private void OnDisable()
    {
        if(transitioning)
        {
            StopCoroutine(transitionCoroutine);
        }

        SetFogColorAndDensity(roomFogColor, roomFogDensity);
        forestFogActive = false;

        TerminalScreen.OnTerminalToggled -= UpdateTerminalActive;
    }
}