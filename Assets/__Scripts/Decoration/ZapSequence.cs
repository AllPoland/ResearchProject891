using System.Collections;
using UnityEngine;

public class ZapSequence : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Light[] chargeLights;
    [SerializeField] private Light zapLight;
    [SerializeField] private LineRenderer[] lightningRenderers;
    [SerializeField] private Renderer[] emitterRenderers;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;

    [Header("Configuration")]
    [SerializeField] private float chargeTime = 1f;
    [SerializeField] private float strobeTime = 1f;
    [SerializeField] private float cooldownTime = 1f;

    [Space]
    [SerializeField] private float strobeFreqency = 10f;
    [SerializeField] private float strobeFreqencyVariance = 1f;

    [Space]
    [SerializeField] private Color chargeStartColor = Color.white;
    [SerializeField] private float chargeStartBrightness = 0f;

    [Space]
    [SerializeField] private Color chargeEndColor = Color.white;
    [SerializeField] private float chargeEndBrightness = 1f;
    [SerializeField] private float emitterEmissionMult = 1f;

    [Space]
    [SerializeField] private Color lightStrobeColor = Color.white;
    [SerializeField] private Vector2 strobeBrightnessMinMax = new Vector2(0.2f, 1f);

    [Space]
    [SerializeField] private Vector2 lightningBrightnessMinMax = Vector2.one;
    [SerializeField] private Vector2 lightningXIncrementMinMax = Vector2.one;
    [SerializeField] private Vector2 lightningYZMin = new Vector2(-1f, -1f);
    [SerializeField] private Vector2 lightningYZMax = Vector2.one;

    [Header("Progression")]
    [SerializeField] private ProgressionRange influenceProgressionRange;
    [SerializeField] private int progressionOnComplete;
    [SerializeField] private int progressionOnCompleteOutsideTerminal;

    private bool animationActive;

    private MaterialPropertyBlock emitterProperties;


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
        newColor.a = Random.Range(lightningBrightnessMinMax.x, lightningBrightnessMinMax.x + parityDifference);
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


    private void SetChargeLightsActive(bool active)
    {
        foreach(Light light in chargeLights)
        {
            light.enabled = active;
        }
    }


    private void SetChargeLightColorAndIntensity(Color color, float intensity)
    {
        foreach(Light light in chargeLights)
        {
            light.color = color;
            light.intensity = intensity;
        }

        if(emitterProperties == null)
        {
            emitterProperties = new MaterialPropertyBlock();
        }

        emitterProperties.SetColor("_EmissionColor", color * intensity * emitterEmissionMult);
        foreach(Renderer renderer in emitterRenderers)
        {
            renderer.SetPropertyBlock(emitterProperties);
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

        float brightness = Random.Range(strobeBrightnessMinMax.x, strobeBrightnessMinMax.x + parityDifference);
        if(parity)
        {
            brightness += parityDifference;
        }

        return brightness;
    }


    private IEnumerator PlayZapCoroutine()
    {
        animationActive = true;

        SetChargeLightsActive(true);
        SetChargeLightColorAndIntensity(chargeStartColor, chargeStartBrightness);

        SetLightningActive(false);

        source.clip = clip;
        source.Play();

        //Charge up
        float t = 0f;
        while(t < 1f)
        {
            Color color = Color.Lerp(chargeStartColor, chargeEndColor, t);
            float intensity = Mathf.Lerp(chargeStartBrightness, chargeEndBrightness, Easings.Quad.In(t));
            SetChargeLightColorAndIntensity(color, intensity);

            t += Time.deltaTime / chargeTime;
            yield return null;
        }

        //Do the zappy
        float strobeLength = GetRandomStrobeLength();
        bool strobeParity = false;

        zapLight.enabled = true;
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

        //Update progression right when zapping completes
        if(influenceProgressionRange.CheckInRange(ProgressionManager.ProgressionStage))
        {
            int newStage = TerminalScreen.TerminalActive ? progressionOnComplete : progressionOnCompleteOutsideTerminal;
            ProgressionManager.ProgressionStage = newStage;
        }

        //Start the cooldown
        zapLight.enabled = false;
        SetLightningActive(false);

        t = 0f;
        while(t < 1f)
        {
            Color color = Color.Lerp(chargeEndColor, chargeStartColor, Easings.Quad.Out(t));
            float intensity = Mathf.Lerp(chargeEndBrightness, chargeStartBrightness, t);
            SetChargeLightColorAndIntensity(color, intensity);

            t += Time.deltaTime / cooldownTime;
            yield return null;
        }

        SetChargeLightColorAndIntensity(chargeEndColor, 0f);

        SetChargeLightsActive(false);
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
        zapLight.enabled = false;
        SetChargeLightsActive(false);
        SetLightningActive(false);

        StimulateFunction.OnStimulateFunctionTriggered += PlayZap;
    }


    private void OnDisable()
    {
        StimulateFunction.OnStimulateFunctionTriggered -= PlayZap;
    }
}