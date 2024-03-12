using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScreen : MonoBehaviour
{
    [SerializeField] private Graphic[] slowTargets;
    [SerializeField] private float slowFadeInTime = 1f;
    [SerializeField] private float slowFadeInDelay = 1f;

    [Space]
    [SerializeField] private Graphic[] targets;
    [SerializeField] private float fadeInTime = 1f;
    [SerializeField] private float fadeInDelay = 1f;


    private IEnumerator FadeInObjectCoroutine(Graphic target, float time)
    {
        target.gameObject.SetActive(true);
        target.enabled = true;

        Color color = target.color;
        color.a = 0f;

        target.color = color;

        float t = 0f;
        while(t < 1f)
        {
            color.a = Easings.Quad.InOut(t);
            target.color = color;

            t += Time.deltaTime / time;
            yield return null;
        }

        color.a = 1f;
        target.color = color;
    }


    private IEnumerator FadeInObjectsCoroutine()
    {
        foreach(Graphic target in slowTargets)
        {
            StartCoroutine(FadeInObjectCoroutine(target, slowFadeInTime));
            yield return new WaitForSeconds(slowFadeInDelay);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        foreach(Graphic target in targets)
        {
            StartCoroutine(FadeInObjectCoroutine(target, fadeInTime));
            yield return new WaitForSeconds(fadeInDelay);
        }
    }


    private void OnEnable()
    {
        foreach(Graphic target in slowTargets)
        {
            target.enabled = false;
        }

        foreach(Graphic target in targets)
        {
            target.enabled = false;
        }

        StartCoroutine(FadeInObjectsCoroutine());
    }
}