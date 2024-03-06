using UnityEngine;

public class InspireKey : MonoBehaviour
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
}