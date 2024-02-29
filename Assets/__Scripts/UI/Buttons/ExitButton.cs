using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void ExitGame()
    {
        try
        {
            SettingsManager.SaveSettingsStatic();
        }
        catch { }

        Debug.Log("Exiting the game.");
        Application.Quit();
    }
}