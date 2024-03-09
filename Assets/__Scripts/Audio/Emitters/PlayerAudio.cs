using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private RandomSound footstepSounds;
    [SerializeField] private RandomSound forestFootstepSounds;

    [Space]
    [SerializeField] private RandomSound shuffleSounds;
    [SerializeField] private RandomSound forestShuffleSounds;

    [Space]
    [SerializeField] private float shuffleCooldownTime = 0.2f;

    [Space]
    [SerializeField] private ProgressionRange forestFootstepRange;

    private AudioSource source;

    //Initialize with a bit of cooldown as a spaghetti way to avoid
    //playing the sound right when the game starts
    private float shuffleCooldown = 0.1f;


    private void PlayClip(AudioClip clip)
    {
        if(clip == null)
        {
            return;
        }

        source.clip = clip;
        source.Play();
    }


    public void PlayFootstep()
    {
        bool useForestSound = forestFootstepRange.CheckInRange(ProgressionManager.ProgressionStage);
        RandomSound sounds = useForestSound ? forestFootstepSounds : footstepSounds;

        PlayClip(sounds.GetClip());
    }


    public void PlayFootShuffle()
    {
        if(shuffleCooldown <= 0f)
        {
            bool useForestSound = forestFootstepRange.CheckInRange(ProgressionManager.ProgressionStage);
            RandomSound sounds = useForestSound ? forestShuffleSounds : shuffleSounds;
            PlayClip(sounds.GetClip());
        }
        shuffleCooldown = shuffleCooldownTime;
    }


    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }


    private void Start()
    {
        TerminalScreen.OnTerminalToggled += (_) => PlayFootShuffle();
    }


    private void Update()
    {
        if(shuffleCooldown > 0f)
        {
            shuffleCooldown -= Time.deltaTime;
        }
    }
}