using UnityEngine;
using UnityEngine.EventSystems;

public class TerminalButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private bool playMouseDownSound = true;
    [SerializeField] private bool playMouseUpSound = true;
    [SerializeField] private bool playHoverSound = true;


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
        if(playHoverSound)
        {
            PlayHover();
        }
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
}