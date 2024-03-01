using UnityEngine;

public class DocumentSelector : MonoBehaviour
{
    [SerializeField] private string documentTitle;
    [SerializeField] private TextAsset documentText;

    [Space]
    [SerializeField] private ProgressionRange enabledProgressionRange;

    [Header("Progression Influence")]
    //The progression range where clicking on the document will progress the game
    [SerializeField] private ProgressionRange influenceProgressionRange;
    [SerializeField] private int progressionStageOnOpen;


    public void OpenDocument()
    {
        if(influenceProgressionRange.CheckInRange(ProgressionManager.ProgressionStage))
        {
            //Opening this document should progress the game
            ProgressionManager.ProgressionStage = progressionStageOnOpen;
        }
    }


    private void UpdateProgressionStage(int stage)
    {
        gameObject.SetActive(enabledProgressionRange.CheckInRange(stage));
    }


    private void Start()
    {
        ProgressionManager.OnProgressionStageUpdated += UpdateProgressionStage;
        UpdateProgressionStage(ProgressionManager.ProgressionStage);
    }
}