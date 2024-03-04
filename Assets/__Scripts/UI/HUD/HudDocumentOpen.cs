using UnityEngine;
using UnityEngine.Events;

public class HudDocumentOpen : MonoBehaviour
{
    [SerializeField] private TextAsset documentText;

    [Space]
    [SerializeField] public UnityEvent OnCloseDocument;

    private bool active = false;


    public void OpenDocument()
    {
        if(documentText != null)
        {
            active = true;
            HudDocument.OpenDocument(documentText);
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