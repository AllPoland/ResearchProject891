using System.Collections;
using UnityEngine;

public class ZapSequence : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Light zapLight;
    [SerializeField] private LineRenderer[] lightningRenderers;
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

    [Space]
    [SerializeField] private Vector2 lightningBrightnessMinMax = Vector2.one;
    [SerializeField] private Vector2 lightningXIncrementMinMax = Vector2.one;
    [SerializeField] private Vector2 lightningYZMin = new Vector2(-1f, -1f);
    [SerializeField] private Vector2 lightningYZMax = Vector2.one;

    private bool animationActive;


    private void RandomizeLightning()
    {
        foreach(LineRenderer renderer in lightningRenderers)
        {
            int pointCount = renderer.positionCount;

            float x = 0f;
            for(int i = 0; i < pointCount; i++)
            {
                float lineProgress = ((float)i / (pointCount - 1));
                float yzMult = Easings.Cubic.Out(lineProgress);

                Vector2 yzMin = lightningYZMin * yzMult;
                Vector2 yzMax = lightningYZMax * yzMult;

                float y = Random.Range(yzMin.x, yzMax.x);
                float z = Random.Range(yzMin.y, yzMax.y);

                renderer.SetPosition(i, new Vector3(x, y, z));
                x += Random.Range(lightningXIncrementMinMax.x, lightningXIncrementMinMax.y);
            }
        }
    }


    private void RandomizeLightningBrightness(bool parity)
    {
        float brightnessDifference = lightningBrightnessMinMax.y - lightningBrightnessMinMax.x;
        float parityDifference = brightnessDifference / 2;

        Color newColor = Color.white;
        newColor.a = Random.Range(lightningBrightnessMinMax.y, lightningBrightnessMinMax.y + parityDifference);
        if(parity)
        {
            newColor.a += parityDifference;
        }

        foreach(LineRenderer renderer in lightningRenderers)
        {
            renderer.startColor = newColor;
            renderer.endColor = newColor;
        }
    }


    private void SetLightningActive(bool active)
    {
        foreach(LineRenderer renderer in lightningRenderers)
        {
            renderer.enabled = active;
        }
    }


    private float GetRandomStrobeLength()
    {
        float variance = Random.Range(-strobeFreqencyVariance, strobeFreqencyVariance);
        return 1f / (strobeFreqency + variance);
    }


    private float GetRandomStrobeBrightness(bool parity)
    {
        float brightnessDifference = strobeBrightnessMinMax.y - strobeBrightnessMinMax.x;
        float parityDifference = brightnessDifference / 2;

        float brightness = Random.Range(strobeBrightnessMinMax.y, strobeBrightnessMinMax.y + parityDifference);
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

        SetLightningActive(false);

        //Charge up
        float t = 0f;
        while(t < 1f)
        {
            zapLight.color = Color.Lerp(chargeStartColor, chargeEndColor, t);
            zapLight.intensity = Mathf.Lerp(chargeStartBrightness, chargeEndBrightness, Easings.Quad.Out(t));

            t += Time.deltaTime / chargeTime;
            yield return null;
        }

        //Do the zappy
        float strobeLength = GetRandomStrobeLength();
        bool strobeParity = false;

        zapLight.color = lightStrobeColor;
        zapLight.intensity = GetRandomStrobeBrightness(strobeParity);

        SetLightningActive(true);

        t = 0f;
        float strobeProgress = 0f;
        while(t < 1f)
        {
            //Strobe the light to make a zap effect
            if(strobeProgress > 1f)
            {
                strobeParity = !strobeParity;
                zapLight.intensity = GetRandomStrobeBrightness(strobeParity);

                RandomizeLightning();
                RandomizeLightningBrightness(strobeParity);

                strobeLength = GetRandomStrobeLength();
                strobeProgress = 0f;
            }

            t += Time.deltaTime / strobeTime;
            strobeProgress += Time.deltaTime / strobeLength;
            yield return null;
        }

        zapLight.enabled = false;
        SetLightningActive(false);

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
        zapLight.enabled = false;
        SetLightningActive(false);

        StimulateFunction.OnStimulateFunctionTriggered += PlayZap;
    }


    private void OnDisable()
    {
        StimulateFunction.OnStimulateFunctionTriggered -= PlayZap;
    }
}