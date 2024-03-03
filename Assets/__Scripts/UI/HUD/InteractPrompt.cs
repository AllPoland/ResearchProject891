using TMPro;
using UnityEngine;

public class InteractPrompt : MonoBehaviour
{
    [SerializeField] private GameObject background;
    [SerializeField] private TextMeshProUGUI text;

    [Space]
    [SerializeField] private string promptPrefix = "[E] - ";


    private void SetPrompt(string prompt)
    {
        background.SetActive(true);
        text.gameObject.SetActive(true);
        text.text = promptPrefix + prompt;
    }


    private void DisablePrompt()
    {
        background.SetActive(false);
        text.gameObject.SetActive(false);
    }


    private void UpdateInteractable(Interactable interactable)
    {
        if(interactable == null)
        {
            DisablePrompt();
        }
        else SetPrompt(interactable.Prompt);
    }


    private void OnEnable()
    {
        PlayerInteract.OnTargetChanged += UpdateInteractable;
    }


    private void OnDisable()
    {
        PlayerInteract.OnTargetChanged -= UpdateInteractable;
        UpdateInteractable(null);
    }
}