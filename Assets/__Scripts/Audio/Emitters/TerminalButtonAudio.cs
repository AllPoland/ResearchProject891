using UnityEngine;
using UnityEngine.EventSystems;

public class TerminalButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private bool playMouseDownSound = true;
    [SerializeField] private bool playMouseUpSound = true;
    [SerializeField] private bool playHoverSound = true;

    private bool hovered;


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
        TerminalAudio.PlayTerminalSound(TerminalSoundType.Hover);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if(hovered)
        {
            return;
        }
        hovered = true;

        if(playHoverSound)
        {
            PlayHover();
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if(playMouseDownSound)
        {
            PlayClick();
        }
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        if(playMouseUpSound)
        {
            PlayRelease();
        }
    }


    private void OnDisable()
    {
        hovered = false;
    }
}