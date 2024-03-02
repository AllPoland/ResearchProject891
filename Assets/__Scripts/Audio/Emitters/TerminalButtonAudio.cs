using UnityEngine;
using UnityEngine.EventSystems;

public class TerminalButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private bool playMouseDownSound = true;
    [SerializeField] private bool playMouseUpSound = true;
    [SerializeField] private bool playHoverSound = true;

    private bool clicked;


    public void PlayClick()
    {
        TerminalAudio.PlayTerminalSound(TerminalSoundType.Click);
    }


    public void PlayRelease()
    {
        TerminalAudio.PlayTerminalSound(TerminalSoundType.Release);
    }


    public void PlayHover()
    {
        if(!clicked)
        {
            TerminalAudio.PlayTerminalSound(TerminalSoundType.Hover);
        }
    }


    public void PlayText()
    {
        TerminalAudio.PlayTerminalSound(TerminalSoundType.Text);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if(playHoverSound)
        {
            PlayHover();
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if(playMouseDownSound && Input.GetMouseButtonDown(0))
        {
            PlayClick();
        }

        clicked = true;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        if(playMouseUpSound && Input.GetMouseButtonUp(0))
        {
            PlayRelease();
        }

        clicked = false;
    }


    private void UpdateTerminalActive(bool terminalActive)
    {
        if(!terminalActive)
        {
            clicked = false;
        }
    }


    private void OnEnable()
    {
        TerminalScreen.OnTerminalToggled += UpdateTerminalActive;
    }


    private void OnDisable()
    {
        clicked = false;
        TerminalScreen.OnTerminalToggled -= UpdateTerminalActive;
    }
}