using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Renderer))]
public class SoupEyes : MonoBehaviour
{
    [SerializeField] private bool activateOnEnable;
    [SerializeField] private bool terminalMode;
    [SerializeField] private float activateChance = 1f;

    [SerializeField, ColorUsage(false, true)] private Color glowEmission;

    [Space]
    [SerializeField] private Vector3 minPosition;
    [SerializeField] private Vector3 maxPosition;

    [Space]
    [SerializeField] private float fadeOutTime = 1f;

    [SerializeField] public UnityEvent OnFinishFade;

    private Color offEmission = Color.black;

    private Renderer target;
    private MaterialPropertyBlock propertyBlock;

    private bool active;
    private bool randomizeOnFade;

    private bool fading;
    private Coroutine fadeCoroutine;


    private void RandomizePosition()
    {
        float x = Random.Range(minPosition.x, maxPosition.x);
        float y = Random.Range(minPosition.y, maxPosition.y);
        float z = Random.Range(minPosition.z, maxPosition.z);

        transform.localPosition = new Vector3(x, y, z);
    }


    private void SetEmission(Color newEmission)
    {
        if(propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        propertyBlock.SetColor("_EmissionColor", newEmission);
        target.SetPropertyBlock(propertyBlock);
    }


    private bool CheckRandomChance()
    {
        return Random.value <= activateChance;
    }


    private IEnumerator FadeOutCoroutine()
    {
        fading = true;

        SetEmission(glowEmission);

        float t = 0f;
        while(t < 1f)
        {
            SetEmission(Color.Lerp(glowEmission, offEmission, Easings.Quad.In(t)));

            t += Time.deltaTime / fadeOutTime;
            yield return null;
        }

        SetEmission(offEmission);

        fading = false;
        OnFinishFade?.Invoke();

        if(randomizeOnFade)
        {
            RandomizePosition();
        }
    }


    private void FadeOut()
    {
        if(fading)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeOutCoroutine());
    }


    private void UpdateTerminalActive(bool terminalActive)
    {
        if(!terminalActive && CheckRandomChance())
        {
            FadeOut();
            TerminalScreen.OnTerminalToggled -= UpdateTerminalActive;
        }
    }


    public void Activate(bool randomizeNow = true)
    {
        if(active)
        {
            return;
        }

        if(randomizeNow)
        {
            RandomizePosition();
        }
        else randomizeOnFade = true;

        if(terminalMode)
        {
            TerminalScreen.OnTerminalToggled += UpdateTerminalActive;
        }
        else active = true;
    }


    private void OnBecameVisible()
    {
        if(active && CheckRandomChance())
        {
            FadeOut();
            active = false;
        }
    }


    private void OnEnable()
    {
        if(!target)
        {
            target = GetComponent<Renderer>();
        }

        if(activateOnEnable)
        {
            Activate();
        }
    }


    private void OnDisable()
    {
        TerminalScreen.OnTerminalToggled -= UpdateTerminalActive;
    }


    private void LateUpdate()
    {
        transform.LookAt(PlayerController.Instance.transform);
    }
}