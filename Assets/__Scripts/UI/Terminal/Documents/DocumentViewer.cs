using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DocumentViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private ScrollingText text;
    [SerializeField] private Image soupIcon;
    [SerializeField] private RectTransform contentTransform;


    public void SetDocument(string documentName, TextAsset documentText)
    {
        contentTransform.anchoredPosition = Vector2.zero;

        title.text = documentName;
        text.SetText(documentText.text);
        soupIcon.enabled = false;
    }


    public void ClearDocument()
    {
        title.text = "";
        text.SetText("");

        contentTransform.anchoredPosition = Vector2.zero;
    }


    public void OpenDocument(string documentName, TextAsset documentText, Action closeCallback, bool lockTerminal)
    {
        if(lockTerminal)
        {
            //Clear the terminal history so the back button is disabled
            while(TerminalScreen.Instance.windowHistory.Count > 1)
            {
                TerminalScreen.Instance.GoBack();
            }

            //Don't add this screen to the window history, just set it active
            gameObject.SetActive(true);
            SetDocument(documentName, documentText);
            return;
        }

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

            Callback?.Invoke();
        }
    }
}