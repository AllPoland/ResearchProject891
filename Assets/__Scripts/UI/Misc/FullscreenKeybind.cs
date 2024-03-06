using UnityEngine;

public class FullscreenKeybind : MonoBehaviour
{
#if !UNITY_WEBGL || UNITY_EDITOR
    private void Update()
    {
        if(Input.GetButtonDown("ToggleFullscreen"))
        {
            if(Screen.fullScreenMode != FullScreenMode.MaximizedWindow)
            {
                Debug.Log("Fullscreen enabled.");
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
            }
            else
            {
                Debug.Log("Fullscreen disabled.");
                Screen.fullScreenMode = FullScreenMode.Windowed;
            }
        }
    }
#endif
}