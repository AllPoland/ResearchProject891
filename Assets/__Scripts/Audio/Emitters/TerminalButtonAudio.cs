using UnityEngine;
using UnityEngine.EventSystems;

public class TerminalButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private bool playMouseDownSound = true;
    [SerializeField] private bool playMouseUpSound = true;
    [SerializeField] private bool playHoverSound = true;

    private bool hovered;


    private void PlaySound(TerminalSoundType soundType)
    {
        if(TerminalScreen.TerminalActive)
        {
            TerminalAudio.PlayTerminalSound(soundType);
        }
    }


    public void PlayClick()
    {
        PlaySound(TerminalSoundType.Click);
    }


    public void PlayRelease()
    {
        PlaySound(TerminalSoundType.Release);
    }


    public void PlayHover()
    {
        PlaySound(TerminalSoundType.Hover);
    }


    public void PlayText()
    {
        PlaySound(TerminalSoundType.Text);
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