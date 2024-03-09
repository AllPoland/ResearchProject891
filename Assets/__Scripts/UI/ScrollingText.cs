using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ScrollingText : MonoBehaviour
{
    public bool Scrolling { get; private set;}

    [TextArea(4, 6)]
    public string text;

    [SerializeField] private int charsPerSecond = 20;
    [SerializeField] private char suffixChar = '_';

    [SerializeField] private bool playSound = true;
    [SerializeField] private int maxSoundsPerSecond = 15;

    [SerializeField] private bool startHidden = false;

    [Space]
    [SerializeField] public UnityEvent OnFinishScrolling;

    private TextMeshProUGUI textMesh;
    private Coroutine scrollCoroutine;


    private string ReplaceEndText(string text)
    {
        StringBuilder output = new StringBuilder(text);
        for(int i = 0; i < output.Length; i++)
        {
            if(output[i] != '\n')
            {
                output[i] = ' ';
            }
        }
        return output.ToString();
    }


    private IEnumerator ScrollTextCoroutine()
    {
        Scrolling = true;
        textMesh.text = "";

        int textLength = text.Length;

        int soundsPerSecond = Mathf.Min(charsPerSecond, maxSoundsPerSecond);
        int soundsPlayed = -1;

        float t = 0f;
        float scrollTime = (float)textLength / charsPerSecond;
        while(t < scrollTime)
        {
            int displayChars = (int)(t * charsPerSecond);

            string displayedText = text[..displayChars];
            string hiddenText = text[displayChars..];

            displayedText += suffixChar;
            displayedText += ReplaceEndText(hiddenText);

            textMesh.text = displayedText;

            if(playSound)
            {
                int soundIndex = (int)(t * soundsPerSecond);
                if(soundIndex > soundsPlayed)
                {
                    TerminalAudio.PlayTerminalSound(TerminalSoundType.Text);
                    soundsPlayed = soundIndex;
                }
            }

            t += Time.deltaTime;
            yield return null;
        }

        //Set the text to the full thing at the very end in case something doesn't work idk
        textMesh.text = text;
        Scrolling = false;

        OnFinishScrolling?.Invoke();
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

        if(text == null)
        {
            text = textMesh.text;
        }

        if(gameObject.activeInHierarchy)
        {
            scrollCoroutine = StartCoroutine(ScrollTextCoroutine());
        }
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
        textMesh.text = text;

        OnFinishScrolling?.Invoke();
    }


    public void SetText(string newText)
    {
        text = newText;
        StartScroll();
    }


    private void OnEnable()
    {
        if(!textMesh)
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }

        if(startHidden)
        {
            textMesh.text = ReplaceEndText(text);
        }
    }


    private void OnDisable()
    {
        StopScroll();
    }
}