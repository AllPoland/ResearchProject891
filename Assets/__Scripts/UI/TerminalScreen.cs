using System;
using System.Collections.Generic;
using UnityEngine;

public class TerminalScreen : MonoBehaviour
{
    //This class serves as a sort of UI manager, and handles anything the terminal does
    public static TerminalScreen Instance { get; private set; }

    private static bool _terminalActive;
    public static bool TerminalActive
    {
        get => _terminalActive;
        set
        {
            _terminalActive = value;
            EventSystemHelper.Deselect();

            OnTerminalToggled?.Invoke(_terminalActive);
        }
    }

    public static event Action<bool> OnTerminalToggled;
    public static event Action OnWindowUpdated;

    [Header("UI Tabs")]
    [SerializeField] private RectTransform startMenu;
    [SerializeField] private RectTransform optionsMenu;
    [SerializeField] private RectTransform mainMenu;

    [Header("Camera Position")]
    [SerializeField] private Vector3 TargetCameraPosition;
    [SerializeField] private Vector3 TargetCameraRotation;

    [NonSerialized] public List<TerminalWindow> windowHistory = new List<TerminalWindow>();
    [NonSerialized] public TerminalWindow currentWindow;

    [NonSerialized] public NoDeleteTerminalWindow StartWindow;
    [NonSerialized] public NoDeleteTerminalWindow OptionsWindow;
    [NonSerialized] public NoDeleteTerminalWindow MainWindow;


    private void UpdateCurrentWindow()
    {
        if(windowHistory.Count < 1)
        {
            //oops
            return;
        }
        
        TerminalWindow newWindow = windowHistory[windowHistory.Count - 1];

        if(newWindow.closePrevious)
        {
            //Close the current window to make space for the new one
            currentWindow?.Close();
        }

        //Set the new window and enable it
        currentWindow = newWindow;
        currentWindow.Open();

        OnWindowUpdated?.Invoke();
    }


    public void SetWindow(TerminalWindow newScreen)
    {
        windowHistory.Add(newScreen);
        UpdateCurrentWindow();
    }


    public void GoBack()
    {
        if(windowHistory.Count <= 1)
        {
            //We can't go back any further than this stupid! !!
            return;
        }

        //Pop the last window in the history
        windowHistory.RemoveAt(windowHistory.Count - 1);
        UpdateCurrentWindow();
    }


    private void Start()
    {
        if(Instance != null && Instance != this)
        {
            enabled = false;
            return;
        }

        Instance = this;

        optionsMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(false);

        StartWindow = new NoDeleteTerminalWindow(startMenu);
        OptionsWindow = new NoDeleteTerminalWindow(optionsMenu);
        MainWindow = new NoDeleteTerminalWindow(mainMenu);

        SetWindow(StartWindow);
    }
}


public abstract class TerminalWindow
{
    public bool closePrevious;

    public abstract void Open();
    public abstract void Close();
}


public class NoDeleteTerminalWindow : TerminalWindow
{
    public RectTransform target;


    public NoDeleteTerminalWindow(RectTransform rectTransform)
    {
        closePrevious = true;
        target = rectTransform;
    }


    public override void Open()
    {
        target.gameObject.SetActive(true);
    }


    public override void Close()
    {
        target.gameObject.SetActive(false);
    }
}


public class PrefabTerminalWindow<T> : TerminalWindow where T : MonoBehaviour
{
    public T target;


    public PrefabTerminalWindow(bool closePrevious, T prefab, RectTransform parent)
    {
        this.closePrevious = closePrevious;
        target = GameObject.Instantiate<T>(prefab, parent, false);
        target.gameObject.SetActive(false);
    }


    public override void Open()
    {
        target.gameObject.SetActive(true);
    }


    public override void Close()
    {
        target.gameObject.SetActive(false);
        GameObject.Destroy(target.gameObject);
    }
}


public enum ScreenValue
{
    StartScreen,
    OptionsMenu,
    MainMenu
}