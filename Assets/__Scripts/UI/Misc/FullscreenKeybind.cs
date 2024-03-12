using UnityEngine;

public class FullscreenKeybind : MonoBehaviour
{
#if !UNITY_WEBGL || UNITY_EDITOR
    private int preferredWindowWidth = Screen.width;
    private int preferredWindowHeight = Screen.height;

    private void Update()
    {
        if(Input.GetButtonDown("ToggleFullscreen"))
        {
            if(Screen.fullScreen)
            {
                Debug.Log("Fullscreen disabled.");
                Screen.SetResolution(preferredWindowWidth, preferredWindowHeight, FullScreenMode.Windowed);
            }
            else
            {
                Debug.Log("Fullscreen enabled.");
                preferredWindowWidth = Screen.width;
                preferredWindowHeight = Screen.height;

                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
            }
        }
    }
#endif
}