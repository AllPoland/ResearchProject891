using System;
using TMPro;
using UnityEngine;

public class DocumentViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private ScrollingText text;
    [SerializeField] private RectTransform contentTransform;


    public void SetDocument(string documentName, TextAsset documentText)
    {
        contentTransform.anchoredPosition = Vector2.zero;

        title.text = documentName;
        text.SetText(documentText.text);
    }


    public void ClearDocument()
    {
        title.text = "";
        text.SetText("");

        contentTransform.anchoredPosition = Vector2.zero;
    }


    public void OpenDocument(string documentName, TextAsset documentText, Action closeCallback)
    {
        TerminalScreen.Instance.SetWindow(new DocumentWindow(this, documentName, documentText, closeCallback));
    }


    public class DocumentWindow : TerminalWindow
    {
        public DocumentViewer Viewer;

        public string Name;
        public TextAsset Text;

        public Action Callback;


        public DocumentWindow(DocumentViewer documentViewer, string documentName, TextAsset documentText, Action callback)
        {
            Viewer = documentViewer;
            Name = documentName;
            Text = documentText;
            Callback = callback;

            ClosePrevious = true;
        }


        public override void Open()
        {
            Viewer.gameObject.SetActive(true);
            Viewer.SetDocument(Name, Text);
        }


        public override void Close()
        {
            Viewer.ClearDocument();
            Viewer.gameObject.SetActive(false);

            Callback();
        }
    }
}