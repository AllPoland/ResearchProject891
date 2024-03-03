using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ScreenTransition : MonoBehaviour
{
    [SerializeField] private float transitionTime = 0.05f;
    [SerializeField] private RectTransform targetTransform;

    private RectTransform rectTransform;
    private float fullHeight;

    private bool transitioning;
    private Coroutine transitionCoroutine;


    private IEnumerator DoTransitionCoroutine()
    {
        transitioning = true;
        targetTransform.gameObject.SetActive(true);

        float t = 0f;
        while(t < 1f)
        {
            float height = (1f - t) * fullHeight;
            targetTransform.sizeDelta = new Vector2(targetTransform.sizeDelta.x, height);

            t += Time.deltaTime / transitionTime;
            yield return null;
        }

        targetTransform.gameObject.SetActive(false);
        transitioning = false;
    }


    private void DoTransition()
    {
        if(transitioning)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(DoTransitionCoroutine());
    }


    private void Start()
    {
        rectTransform = (RectTransform)transform;

        fullHeight = rectTransform.rect.height;
        targetTransform.gameObject.SetActive(false);

        TerminalScreen.OnWindowUpdated += DoTransition;
    }
}