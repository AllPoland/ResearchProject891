using UnityEngine;

public class StartButton : MonoBehaviour
{
    public void StartGame()
    {
        TerminalScreen.Instance.SetWindow(TerminalScreen.Instance.MainWindow);
    }
}