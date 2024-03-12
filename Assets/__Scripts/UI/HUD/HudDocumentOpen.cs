using UnityEngine;
using UnityEngine.Events;

public class HudDocumentOpen : MonoBehaviour
{
    [SerializeField] private TextAsset documentText;
    [SerializeField] private TextAsset entityText;
    [SerializeField] private bool justify = true;
    [SerializeField] private bool isLetter = false;
    [SerializeField] private bool alternateEntityFont = false;

    [Space]
    [SerializeField] public UnityEvent OnCloseDocument;

    private bool active = false;


    public void OpenDocument()
    {
        active = true;
        HudDocument.OpenDocument(documentText, entityText, justify, isLetter, alternateEntityFont);
    }


    private void HandleDocumentUpdated(bool documentActive)
    {
        if(active && !documentActive)
        {
            active = false;
            OnCloseDocument?.Invoke();
        }
    }


    private void OnEnable()
    {
        HudDocument.OnDocumentUpdated += HandleDocumentUpdated;
    }


    private void OnDisable()
    {
        HudDocument.OnDocumentUpdated -= HandleDocumentUpdated;
    }
}