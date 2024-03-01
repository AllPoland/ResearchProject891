using UnityEngine;

public class DocumentSelector : MonoBehaviour
{
    [Header("Document")]
    [SerializeField] private string documentTitle;
    [SerializeField] private TextAsset documentText;

    [Space]
    [SerializeField] private Optional<PasswordSettings> password;

    [Header("Components")]
    [SerializeField] private DocumentViewer documentViewer;

    [Header("Progression Influence")]
    //The progression range where clicking on the document will progress the game
    [SerializeField] private ProgressionRange influenceProgressionRange;
    [SerializeField] private int progressionStageOnOpen;

    private bool passwordUnlocked = false;


    public void SetProgression()
    {
        if(influenceProgressionRange.CheckInRange(ProgressionManager.ProgressionStage))
        {
            //Opening this document should progress the game
            ProgressionManager.ProgressionStage = progressionStageOnOpen;
        }
    }


    private void OpenViewer()
    {
        documentViewer.OpenDocument(documentTitle, documentText, SetProgression);
    }


    private void PasswordCallback(bool correct)
    {
        //If the user didn't get the password correct we don't need to do anything
        if(correct)
        {
            passwordUnlocked = true;

            //Exit the password screen and open the document
            TerminalScreen.Instance.GoBack();
            OpenViewer();
        }
    }


    public void OpenDocument()
    {
        if(password.Enabled && !passwordUnlocked)
        {
            //This document is password locked and the player hasn't gotten the password yet
            password.Value.StartPasswordPrompt(PasswordCallback);
            return;
        }

        OpenViewer();
    }
}