using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LightSwitch : MonoBehaviour
{
    [SerializeField] private float onAngle = 180f;
    [SerializeField] private float offAngle = 0f;

    [SerializeField] private float flipTime = 1f;

    [Space]
    [SerializeField] public UnityEvent<bool> OnFlipped;

    private bool flipped = false;

    private bool animating = false;
    private Coroutine flipCoroutine;


    private IEnumerator FlipSwitchCoroutine(float startAngle, float endAngle)
    {
        animating = true;

        float t = 0f;
        while(t < 1f)
        {
            float angle = Mathf.Lerp(startAngle, endAngle, Easings.Quad.In(t));
            transform.localEulerAngles = new Vector3(0f, 0f, angle);

            t += Time.deltaTime / flipTime;
            yield return null;
        }

        transform.localEulerAngles = new Vector3(0f, 0f, endAngle);
        animating = false;

        OnFlipped?.Invoke(flipped);
    }


    public void SetFlipped(bool flip)
    {
        if(flip == flipped)
        {
            return;
        }

        flipped = flip;
        if(animating)
        {
            StopCoroutine(flipCoroutine);
        }

        float startAngle = transform.localEulerAngles.z;
        float endAngle = flipped ? onAngle : offAngle;
        StartCoroutine(FlipSwitchCoroutine(startAngle, endAngle));
    }


    public void SetFlippedNoAnimation(bool flip)
    {
        if(flip == flipped)
        {
            return;
        }

        flipped = flip;
        transform.localEulerAngles = new Vector3(0f, 0f, flipped ? onAngle : offAngle);

        OnFlipped?.Invoke(flipped);
    }
}