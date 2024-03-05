using System.Collections;
using UnityEngine;

public class ZapSequence : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Light zapLight;
    [SerializeField] private GameObject[] lightningObjects;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;

    [Header("Configuration")]
    [SerializeField] private float chargeTime = 1f;
    [SerializeField] private float strobeTime = 1f;
    [SerializeField] private float strobeFreqency = 10f;
    [SerializeField] private float strobeFreqencyVariance = 1f;

    [Space]
    [SerializeField] private Color chargeStartColor = Color.white;
    [SerializeField] private float chargeStartBrightness = 0f;

    [Space]
    [SerializeField] private Color chargeEndColor = Color.white;
    [SerializeField] private float chargeEndBrightness = 1f;

    [Space]
    [SerializeField] private Color lightStrobeColor = Color.white;
    [SerializeField] private Vector2 strobeBrightnessMinMax = new Vector2(0.2f, 1f);

    private bool animationActive;


    private float GetRandomStrobeLength()
    {
        float variance = Random.Range(-strobeFreqencyVariance, strobeFreqencyVariance);
        return 1f / (strobeFreqency + variance);
    }


    private float GetRandomStrobeBrightness(bool parity)
    {
        float brightnessDifference = strobeBrightnessMinMax.y - strobeBrightnessMinMax.x;
        float parityDifference = brightnessDifference / 2;

        float brightness = Random.Range(0f, parityDifference);
        if(parity)
        {
            brightness += parityDifference;
        }

        return brightness;
    }


    private IEnumerator PlayZapCoroutine()
    {
        animationActive = true;

        zapLight.enabled = true;
        zapLight.color = chargeStartColor;
        zapLight.intensity = chargeStartBrightness;

        //Charge up
        float t = 0f;
        while(t < 1f)
        {
            zapLight.color = Color.Lerp(chargeStartColor, chargeEndColor, t);
            zapLight.intensity = Mathf.Lerp(chargeStartBrightness, chargeEndBrightness, Easings.Quad.In(t));

            t += Time.deltaTime / chargeTime;
            yield return null;
        }

        //Do the zappy
        float strobeLength = GetRandomStrobeLength();
        bool strobeParity = false;

        zapLight.color = lightStrobeColor;
        zapLight.intensity = GetRandomStrobeBrightness(strobeParity);

        t = 0f;
        float strobeProgress = 0f;
        while(t < 1f)
        {
            //Strobe the light to make a zap effect
            if(strobeProgress > 1f)
            {
                strobeParity = !strobeParity;
                zapLight.intensity = GetRandomStrobeBrightness(strobeParity);

                strobeLength = GetRandomStrobeLength();
                strobeProgress = 0f;
            }

            t += Time.deltaTime / strobeTime;
            strobeProgress += Time.deltaTime / strobeLength;
            yield return null;
        }

        zapLight.enabled = false;

        animationActive = false;
    }


    private void PlayZap()
    {
        if(!animationActive)
        {
            StartCoroutine(PlayZapCoroutine());
        }
    }


    private void OnEnable()
    {
        StimulateFunction.OnStimulateFunctionTriggered += PlayZap;
    }


    private void OnDisable()
    {
        StimulateFunction.OnStimulateFunctionTriggered -= PlayZap;
    }
}