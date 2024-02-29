using UnityEngine;

public class OptionsButton : MonoBehaviour
{
    public void OpenOptions()
    {
        TerminalScreen.Instance.SetWindow(TerminalScreen.Instance.OptionsWindow);
    }
}