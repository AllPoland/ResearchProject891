using System;
using System.Collections.Generic;
using UnityEngine;

public class TerminalScreen : MonoBehaviour
{
    //This class serves as a sort of UI manager, and handles anything the terminal does
    public static TerminalScreen Instance { get; private set; }

    private static bool _terminalActive = true;
    public static bool TerminalActive
    {
        get => _terminalActive;
        set
        {
            _terminalActive = value;
            Debug.Log(_terminalActive ? "Terminal active." : "Terminal inactive.");

            EventSystemHelper.Deselect();

            Cursor.visible = _terminalActive;
            Cursor.lockState = _terminalActive ? CursorLockMode.None : CursorLockMode.Locked;

            OnTerminalToggled?.Invoke(_terminalActive);
        }
    }

    public static event Action<bool> OnTerminalToggled;
    public static event Action OnWindowUpdated;

    [SerializeField] private StartupSequence startup;

    [Header("UI Tabs")]
    [SerializeField] private RectTransform startMenu;
    [SerializeField] private RectTransform optionsMenu;
    [SerializeField] private RectTransform mainMenu;

    [Header("Positioning")]
    [SerializeField] public Vector3 targetCameraPosition;
    [SerializeField] public Vector3 targetCameraRotation;

    [Space]
    [SerializeField] public Vector2 targetPlayerPosition;
    [SerializeField] public Vector2 targetPlayerRotation;

    [NonSerialized] public List<TerminalWindow> windowHistory = new List<TerminalWindow>();
    [NonSerialized] public TerminalWindow currentWindow;

    [NonSerialized] public NoDeleteTerminalWindow StartWindow;
    [NonSerialized] public NoDeleteTerminalWindow OptionsWindow;
    [NonSerialized] public NoDeleteTerminalWindow MainWindow;


    public void EnterTerminal()
    {
        TerminalActive = true;
    }


    private void UpdateCurrentWindow()
    {
        if(windowHistory.Count < 1)
        {
            //oops
            return;
        }
        
        TerminalWindow newWindow = windowHistory[windowHistory.Count - 1];

        if(newWindow.ClosePrevious)
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


    public void SetWindowTransform(RectTransform newScreen)
    {
        NoDeleteTerminalWindow newWindow = new NoDeleteTerminalWindow(newScreen);
        SetWindow(newWindow);
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

        TerminalActive = true;

        if(startup)
        {
            startup.PlayStartup();
        }
        else SetWindow(StartWindow);
    }


    private void Update()
    {
        if(TerminalActive)
        {
            if(Input.GetButtonDown("GoBack"))
            {
                GoBack();
            }
            if(Input.GetButtonDown("ExitTerminal") && !EventSystemHelper.GetSelectedGameObject())
            {
                TerminalActive = false;
            }
        }
    }
}


public abstract class TerminalWindow
{
    public bool ClosePrevious;

    public abstract void Open();
    public abstract void Close();
}


public class NoDeleteTerminalWindow : TerminalWindow
{
    public RectTransform target;


    public NoDeleteTerminalWindow(RectTransform rectTransform)
    {
        ClosePrevious = true;
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


    public PrefabTerminalWindow(bool ClosePrevious, T prefab, RectTransform parent)
    {
        this.ClosePrevious = ClosePrevious;
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