using UnityEngine;
using UnityEngine.EventSystems;

public class InspireKey : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private InspireFunction parentFunction;

    [Space]
    [SerializeField] private int noteType;
    [SerializeField] private AudioClip clip;


    public void PlayNote()
    {
        parentFunction.PlayNote(noteType);
        TerminalAudio.PlayTerminalSound(TerminalSoundType.Notable, clip);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        PlayNote();
    }
}