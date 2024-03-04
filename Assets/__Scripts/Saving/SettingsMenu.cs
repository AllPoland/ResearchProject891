using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    private void OnDisable()
    {
        //Very big and important script yes
        SettingsManager.SaveSettingsStatic?.Invoke();
    }
}