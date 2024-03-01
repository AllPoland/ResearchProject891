using System;
using TMPro;
using UnityEngine;

public class PasswordInput : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private ScrollingText incorrectText;

    [Space]
    [SerializeField] private string incorrectString = "Incorrect passcode";

    private Action<bool> Callback;
    private string Password;


    public void InputPassword(string userInput)
    {
        //Send back whether the user got the password correct
        bool correct = userInput.Equals(Password, StringComparison.InvariantCultureIgnoreCase);
        incorrectText.SetText(correct ? "" : incorrectString);

        Callback(correct);
    }


    public void SetPassword(string prompt, string password, Action<bool> callback)
    {
        promptText.text = prompt;
        inputField.text = "";
        incorrectText.SetText("");

        Password = password;
        Callback = callback;
    }


    public void ClearPassword()
    {
        promptText.text = "";
        inputField.text = "";
        incorrectText.SetText("");

        Password = "";
        Callback = null;
    }


    public void StartPasswordPrompt(PasswordSettings settings, Action<bool> callback)
    {
        TerminalScreen.Instance.SetWindow(new PasswordWindow(this, settings.Prompt, settings.Password, callback));
    }


    public class PasswordWindow : TerminalWindow
    {
        public PasswordInput Input;

        public string Prompt;
        public string Password;
        
        public Action<bool> Callback;


        public PasswordWindow(PasswordInput input, string prompt, string password, Action<bool> callback)
        {
            Input = input;
            Prompt = prompt;
            Password = password;
            Callback = callback;

            ClosePrevious = true;
        }


        public override void Open()
        {
            Input.gameObject.SetActive(true);
            Input.SetPassword(Prompt, Password, Callback);
        }


        public override void Close()
        {
            Input.ClearPassword();
            Input.gameObject.SetActive(false);
        }
    }
}


[Serializable]
public class PasswordSettings
{
    public string Prompt;
    public string Password;

    public PasswordInput InputScreen;


    public void StartPasswordPrompt(Action<bool> callback)
    {
        InputScreen.StartPasswordPrompt(this, callback);
    }
}