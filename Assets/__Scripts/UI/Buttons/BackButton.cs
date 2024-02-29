using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    [SerializeField] private Button button;


    private void OnEnable()
    {
        TerminalScreen.OnWindowUpdated += UpdateWindow;
        UpdateWindow();
    }


    private void OnDisable()
    {
        TerminalScreen.OnWindowUpdated += UpdateWindow;
    }


    private void UpdateWindow()
    {
        //Hide the back button when there's nothing to go back to
        bool backAvailable = TerminalScreen.Instance?.windowHistory.Count > 1;
        button.gameObject.SetActive(backAvailable);
    }


    public void GoBack()
    {
        TerminalScreen.Instance.GoBack();
    }
}