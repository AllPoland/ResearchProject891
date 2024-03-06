using UnityEngine;
using UnityEngine.Events;

public class HudDocumentOpen : MonoBehaviour
{
    [SerializeField] private TextAsset documentText;
    [SerializeField] private TextAsset entityText;

    [Space]
    [SerializeField] public UnityEvent OnCloseDocument;

    private bool active = false;


    public void OpenDocument()
    {
        if(documentText || entityText)
        {
            active = true;
            HudDocument.OpenDocument(documentText, entityText);
        }
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