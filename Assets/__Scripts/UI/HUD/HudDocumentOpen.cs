using UnityEngine;

public class HudDocumentOpen : MonoBehaviour
{
    [SerializeField] private TextAsset documentText;


    public void OpenDocument()
    {
        if(documentText != null)
        {
            HudDocument.OpenDocument(documentText);
        }
    }
}
