using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ScrollingText : MonoBehaviour
{
    public bool Scrolling { get; private set;}

    [SerializeField] private int charsPerSecond = 20;

    private TextMeshProUGUI textMesh;
    private Coroutine scrollCoroutine;

    private string currentText;


    private IEnumerator ScrollTextCoroutine()
    {
        Scrolling = true;
        textMesh.text = "";

        float t = 0f;
        float scrollTime = (float)currentText.Length / charsPerSecond;
        while(t < scrollTime)
        {
            int displayChars = (int)(t * charsPerSecond);
            textMesh.text = currentText[..displayChars];

            t += Time.deltaTime;
            yield return null;
        }

        //Set the text to the full thing at the very end in case something doesn't work idk
        textMesh.text = currentText;
        Scrolling = false;
    }


    public void StartScroll()
    {
        if(!textMesh)
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }

        if(Scrolling)
        {
            StopCoroutine(scrollCoroutine);
        }

        scrollCoroutine = StartCoroutine(ScrollTextCoroutine());
    }


    public void StopScroll()
    {
        if(!Scrolling)
        {
            return;
        }

        StopCoroutine(scrollCoroutine);
        scrollCoroutine = null;
        Scrolling = false;
    }


    public void SkipScroll()
    {
        if(!textMesh)
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }

        StopScroll();
        textMesh.text = currentText;
    }


    public void SetText(string newText)
    {
        currentText = newText;
        StartScroll();
    }


    private void OnDisable()
    {
        StopScroll();
    }
}