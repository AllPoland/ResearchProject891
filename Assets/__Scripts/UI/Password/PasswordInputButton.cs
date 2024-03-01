using UnityEngine;
using TMPro;

public class PasswordInputButton : MonoBehaviour
{
    [SerializeField] private PasswordInput passwordInput;
    [SerializeField] private TMP_InputField inputField;


    public void InputPassword()
    {
        string userInput = inputField.text;
        if(!string.IsNullOrEmpty(userInput))
        {
            passwordInput.InputPassword(userInput);
        }
    }


    private void Update()
    {
        bool inputFieldSelected = EventSystemHelper.GetSelectedGameObject() == inputField.gameObject;
        if(inputFieldSelected && Input.GetButtonDown("Submit"))
        {
            InputPassword();
        }
    }
}