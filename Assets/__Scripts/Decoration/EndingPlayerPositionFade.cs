using UnityEngine;

public class EndingPlayerPositionFade : MonoBehaviour
{
    [SerializeField] private float minPlayerZ;
    [SerializeField] private float maxPlayerZ;

    [Space]
    [SerializeField] private AudioSource humSource;
    [SerializeField] private float maxHumVolume = 1f;

    [Space]
    [SerializeField] private float minFogDensity = 0.1f;
    [SerializeField] private float maxFogDensity = 1f;

    [Space]
    [SerializeField] private int progressionAfterFade;

    private float fadeRange => maxPlayerZ - minPlayerZ;

    private bool hasFaded = false;


    private void UpdateFadeProgress(float fadeProgress)
    {
        humSource.volume = maxHumVolume * fadeProgress;
        RenderSettings.fogDensity = Mathf.Lerp(minFogDensity, maxFogDensity, fadeProgress);
    }


    private void UpdatePlayerPos()
    {
        float playerPos = PlayerController.Instance.transform.position.z;

        float fadeProgress = Mathf.Max((playerPos - minPlayerZ) / fadeRange, 0f);
        if(fadeProgress >= 1f)
        {
            UpdateFadeProgress(1f);
            ProgressionManager.ProgressionStage = progressionAfterFade;
            hasFaded = true;
        }
        else UpdateFadeProgress(fadeProgress);
    }


    private void Update()
    {
        if(!hasFaded)
        {
            UpdatePlayerPos();
        }
    }


    private void OnEnable()
    {
        hasFaded = false;
        humSource.Play();
    }


    private void OnDisable()
    {
        humSource.Stop();
    }
}