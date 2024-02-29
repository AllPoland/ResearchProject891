using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class SelectableHandler : MonoBehaviour
{
    [SerializeField] private bool isDefault;

    private Selectable selectable;

    private bool selected => EventSystemHelper.GetSelectedGameObject() == gameObject;


    private void OnEnable()
    {
        if(!selectable)
        {
            selectable = GetComponent<Selectable>();
        }

        TerminalScreen.OnTerminalToggled += UpdateTerminalActive;
    }


    private void OnDisable()
    {
        if(selected)
        {
            EventSystemHelper.Deselect();
        }

        TerminalScreen.OnTerminalToggled += UpdateTerminalActive;
    }


    private void UpdateTerminalActive(bool terminalActive)
    {
        selectable.interactable = terminalActive;

        if(!terminalActive && selected)
        {
            EventSystemHelper.Deselect();
        }
    }
}