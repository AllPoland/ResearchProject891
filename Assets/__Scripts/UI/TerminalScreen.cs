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

    [NonSerialized] public List<ITerminalWindow> windowHistory = new List<ITerminalWindow>();
    [NonSerialized] public ITerminalWindow currentWindow;

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

        //Close the current window to make space for the new one
        currentWindow?.Close();

        //Set the new window and enable it
        currentWindow = windowHistory[windowHistory.Count - 1];
        currentWindow.Open();

        OnWindowUpdated?.Invoke();
    }


    public void SetWindow(ITerminalWindow newScreen)
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


public interface ITerminalWindow
{
    public void Open();
    public void Close();
}


public class NoDeleteTerminalWindow : ITerminalWindow
{
    public RectTransform target;


    public NoDeleteTerminalWindow(RectTransform rectTransform)
    {
        target = rectTransform;
    }


    public void Open()
    {
        target.gameObject.SetActive(true);
    }


    public void Close()
    {
        target.gameObject.SetActive(false);
    }
}


public class PrefabTerminalWindow<T> : ITerminalWindow where T : MonoBehaviour
{
    public T prefab;
    public RectTransform parent;

    public T target;


    public PrefabTerminalWindow(T prefab)
    {
        this.prefab = prefab;
    }


    public void Open()
    {
        target = GameObject.Instantiate<T>(prefab, parent, false);
    }


    public void Close()
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