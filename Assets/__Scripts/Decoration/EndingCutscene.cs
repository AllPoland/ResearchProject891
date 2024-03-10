using System.Collections;
using UnityEngine;

public class EndingCutscene : MonoBehaviour
{
    [SerializeField] private ProgressionRange activeRange;
    [SerializeField] private float fogDensity;

    [Space]
    [SerializeField] private AudioSource mainSource;
    [SerializeField] private AudioClip introClip;

    [Header("Soup")]
    [SerializeField] private Transform soupTransform;
    [SerializeField] private float soupMoveDelay;
    [SerializeField] private float soupMoveTime;
    [SerializeField] private Vector3 soupStartPos;
    [SerializeField] private Vector3 soupEndPos;
    
    [Space]
    [SerializeField] private Renderer[] soupEyes;
    [SerializeField] private float eyeInDelay;
    [SerializeField] private float eyeInTime;
    [SerializeField] private float eyeOutDelay;
    [SerializeField] private float eyeOutTime;

    [Space]
    [SerializeField, ColorUsage(false, true)] private Color soupGlowEmission;
    [SerializeField, ColorUsage(false, true)] private Color soupOffEmission;

    [Header("Letter")]
    [SerializeField] private Transform letterTransform;
    [SerializeField] private float letterSlideDelay;
    [SerializeField] private float letterSlideTime;
    [SerializeField] private Vector3 letterSlideStartPos;
    [SerializeField] private Vector3 lettereSlideEndPos;

    private bool cutsceneActive;
    private Coroutine cutsceneCoroutine;

    private MaterialPropertyBlock soupEyeProperties;


    private void SetEyeEmission(Color emissionColor)
    {
        soupEyeProperties.SetColor("_EmissionColor", emissionColor);
        foreach(Renderer renderer in soupEyes)
        {
            renderer.SetPropertyBlock(soupEyeProperties);
        }
    }


    private IEnumerator CutsceneCoroutine()
    {
        cutsceneActive = true;

        RenderSettings.fogDensity = fogDensity;

        soupTransform.gameObject.SetActive(true);


        soupEyeProperties = new MaterialPropertyBlock();
        SetEyeEmission(soupOffEmission);

        yield return null;

        cutsceneActive = false;
    }


    private void StopCutscene()
    {
        //This shouldn't happen but I'm paranoid about my game breaking the wrong way
        if(cutsceneActive)
        {
            StopCoroutine(cutsceneCoroutine);
        }
    }


    private void PlayCutscene()
    {
        StopCutscene();

        //Steal the camera and take away player control
        Camera mainCamera = Camera.main;
        Transform cameraTransform = mainCamera.transform;

        cameraTransform.SetParent(transform);
        cameraTransform.position = Vector3.zero;
        cameraTransform.rotation = Quaternion.identity;

        PlayerController.Instance.gameObject.SetActive(false);
    }


    private void UpdateProgressionStage(int stage)
    {
        if(activeRange.CheckInRange(stage))
        {
            PlayCutscene();
        }
    }


    private void OnEnable()
    {
        ProgressionManager.OnProgressionStageUpdated += UpdateProgressionStage;
        UpdateProgressionStage(ProgressionManager.ProgressionStage);
    }


    private void OnDisable()
    {
        StopCutscene();
        ProgressionManager.OnProgressionStageUpdated -= UpdateProgressionStage;
    }
}