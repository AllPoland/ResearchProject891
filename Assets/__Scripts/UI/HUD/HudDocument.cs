using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class HudDocument : MonoBehaviour
{
    private static bool _documentActive = false;
    public static bool DocumentActive
    {
        get => _documentActive;
        private set
        {
            _documentActive = value;

            if(_documentActive)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            OnDocumentUpdated?.Invoke(_documentActive);
        }
    }

    public static event Action<bool> OnDocumentUpdated;

    private static event Action<TextAsset> OnDocumentOpened;

    [Header("Components")]
    [SerializeField] private GameObject documentContainer;
    [SerializeField] private RectTransform documentTransform;
    [SerializeField] private TextMeshProUGUI textMesh;

    [Header("Animation")]
    [SerializeField] private float animationTime = 1f;

    [Space]
    [SerializeField] private Vector2 hiddenPosition;
    [SerializeField] private float hiddenRotation;

    [Space]
    [SerializeField] private Vector2 shownPosition;
    [SerializeField] private float shownRotation;

    private bool animating;
    private Coroutine animationCoroutine;


    public static void OpenDocument(TextAsset documentText)
    {
        OnDocumentOpened?.Invoke(documentText);
    }


    private IEnumerator ShowDocumentCoroutine(Vector2 startPos, float startRotation)
    {
        animating = true;

        float t = 0f;
        while(t < 1f)
        {
            float positionTransition = Easings.Cubic.Out(t);
            float rotationTransition = Easings.Sine.Out(t);

            float angle = Mathf.Lerp(startRotation, shownRotation, rotationTransition);
            documentTransform.anchoredPosition = Vector2.Lerp(startPos, shownPosition, positionTransition);
            documentTransform.localEulerAngles = new Vector3(0f, 0f, angle);

            t += Time.deltaTime / animationTime;
            yield return null;
        }

        documentTransform.anchoredPosition = shownPosition;
        documentTransform.localEulerAngles = new Vector3(0f, 0f, shownRotation);

        animating = false;
    }


    private IEnumerator HideDocumentCoroutine(Vector2 startPos, float startRotation)
    {
        animating = true;

        float t = 0f;
        while(t < 1f)
        {
            float positionTransition = Easings.Quad.In(t);
            float rotationTransition = Easings.Sine.In(t);

            float angle = Mathf.Lerp(startRotation, hiddenRotation, rotationTransition);
            documentTransform.anchoredPosition = Vector2.Lerp(startPos, hiddenPosition, positionTransition);
            documentTransform.localEulerAngles = new Vector3(0f, 0f, angle);

            t += Time.deltaTime / animationTime;
            yield return null;
        }

        documentTransform.anchoredPosition = hiddenPosition;
        documentTransform.localEulerAngles = new Vector3(0f, 0f, hiddenRotation);

        documentContainer.SetActive(false);

        animating = false;
    }


    public void ShowDocument(TextAsset documentText, bool animate = true)
    {
        documentContainer.SetActive(true);
        textMesh.text = documentText.text;

        DocumentActive = true;

        if(animating)
        {
            StopCoroutine(animationCoroutine);
        }

        if(animate)
        {
            Vector2 startPos = documentTransform.anchoredPosition;
            float startRotation = documentTransform.eulerAngles.z;
            animationCoroutine = StartCoroutine(ShowDocumentCoroutine(startPos, startRotation));
        }
        else
        {
            documentTransform.anchoredPosition = shownPosition;
            documentTransform.localEulerAngles = new Vector3(0f, 0f, shownRotation);
        }
    }


    public void HideDocument(bool animate = true)
    {
        DocumentActive = false;

        if(animating)
        {
            StopCoroutine(animationCoroutine);
        }

        if(animate)
        {
            Vector2 startPos = documentTransform.anchoredPosition;
            float startRotation = documentTransform.eulerAngles.z;
            animationCoroutine = StartCoroutine(HideDocumentCoroutine(startPos, startRotation));
        }
        else
        {
            documentTransform.anchoredPosition = hiddenPosition;
            documentTransform.localEulerAngles = new Vector3(0f, 0f, hiddenRotation);
            documentContainer.SetActive(false);
        }
    }


    private void Start()
    {
        if(!DocumentActive)
        {
            HideDocument(false);
        }

        OnDocumentOpened += (TextAsset textAsset) => ShowDocument(textAsset);
    }

    
    private void Update()
    {
        if(DocumentActive && Input.GetButtonDown("ExitTerminal"))
        {
            HideDocument();
        }
    }
}