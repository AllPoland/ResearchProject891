using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Canvas hudCanvas;


    private void UpdateTerminalActive(bool terminalActive)
    {
        hudCanvas.gameObject.SetActive(!terminalActive);
    }


    private void Start()
    {
        TerminalScreen.OnTerminalToggled += UpdateTerminalActive;
        if(TerminalScreen.Instance)
        {
            UpdateTerminalActive(TerminalScreen.TerminalActive);
        }
    }
}