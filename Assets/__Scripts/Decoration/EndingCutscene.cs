using System.Collections;
using UnityEngine;

public class EndingCutscene : MonoBehaviour
{
    [SerializeField] private ProgressionRange activeRange;
    [SerializeField] private float fogDensity;
    [SerializeField] private Light spotLight;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Space]
    [SerializeField] private AudioClip introClip;
    [SerializeField] private AudioClip paperSlideClip;
    [SerializeField] private AudioClip paperPickupClip;
    [SerializeField] private AudioClip paperDropClip;

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

    [Space]
    [SerializeField, ColorUsage(false, true)] private Color soupGlowEmission;
    [SerializeField, ColorUsage(false, true)] private Color soupOffEmission;

    [Header("Letter")]
    [SerializeField] private Transform letterTransform;
    [SerializeField] private float letterSlideDelay;
    [SerializeField] private float letterSlideTime;
    [SerializeField] private Vector3 letterSlideStartPos;
    [SerializeField] private Vector3 letterSlideEndPos;

    [Space]
    [SerializeField] private TextAsset letterText;
    [SerializeField] private float letterOpenDelay;

    [Header("Final Fadeout")]
    [SerializeField] private float cameraReturnTime;
    [SerializeField] private float fadeOutDelay;
    [SerializeField] private float fadeOutTime;

    [Space]
    [SerializeField] private int progressionOnEnd;

    private MaterialPropertyBlock soupEyeProperties;


    private void SetEyeEmission(Color emissionColor)
    {
        soupEyeProperties.SetColor("_EmissionColor", emissionColor);
        foreach(Renderer renderer in soupEyes)
        {
            renderer.SetPropertyBlock(soupEyeProperties);
        }
    }


    private IEnumerator FadeOutCoroutine()
    {
        Transform cameraTransform = Camera.main.transform;
        Quaternion startCameraRotation = cameraTransform.rotation;

        float t = 0f;
        while(t < 1f)
        {
            float ease = Easings.Sine.Out(t);
            cameraTransform.rotation = Quaternion.Slerp(startCameraRotation, Quaternion.identity, ease);

            t += Time.deltaTime / cameraReturnTime;
            yield return null;
        }

        yield return new WaitForSeconds(fadeOutDelay);

        t = 0f;
        while(t < 1f)
        {
            soupTransform.position = Vector3.Lerp(soupEndPos, soupStartPos, Easings.Quad.In(t));

            t += Time.deltaTime / fadeOutTime;
            yield return null;
        }

        soupTransform.gameObject.SetActive(false);
        ProgressionManager.ProgressionStage = progressionOnEnd;
    }


    private void StartFinalFadeOut(bool documentActive)
    {
        if(documentActive)
        {
            //Uhhhhhhhhhhhhhhhhhhhhhhhhh
            Debug.LogWarning("Uhhhhhhhhhhhhhhhhhhhhh");
            return;
        }

        audioSource.PlayOneShot(paperDropClip);

        HudDocument.OnDocumentUpdated -= StartFinalFadeOut;
        StartCoroutine(FadeOutCoroutine());
    }


    private IEnumerator CutsceneCoroutine()
    {
        Transform cameraTransform = Camera.main.transform;

        RenderSettings.fogDensity = fogDensity;
        spotLight.gameObject.SetActive(true);

        letterTransform.gameObject.SetActive(false);

        soupTransform.gameObject.SetActive(true);
        soupTransform.position = soupStartPos;

        soupEyeProperties = new MaterialPropertyBlock();
        SetEyeEmission(soupOffEmission);
        
        audioSource.PlayOneShot(introClip);

        yield return new WaitForSeconds(eyeInDelay);

        //Start fading the eyes in
        float t = 0f;
        while(t < 1f)
        {
            SetEyeEmission(Color.Lerp(soupOffEmission, soupGlowEmission, Easings.Quad.InOut(t)));

            t += Time.deltaTime / eyeInTime;
            yield return null;
        }

        SetEyeEmission(soupGlowEmission);

        yield return new WaitForSeconds(soupMoveDelay);

        //Start moving the soup in
        t = 0f;
        while(t < 1f)
        {
            float ease = Easings.Quad.InOut(t);
            soupTransform.position = Vector3.Lerp(soupStartPos, soupEndPos, ease);
            SetEyeEmission(Color.Lerp(soupGlowEmission, soupOffEmission, ease));

            t += Time.deltaTime / soupMoveTime;
            yield return null;
        }

        soupTransform.position = soupEndPos;
        SetEyeEmission(soupOffEmission);

        yield return new WaitForSeconds(letterSlideDelay);

        letterTransform.gameObject.SetActive(true);
        letterTransform.position = letterSlideStartPos;

        audioSource.PlayOneShot(paperSlideClip);

        t = 0f;
        while(t < 1f)
        {
            float ease = Easings.Sine.InOut(t);
            letterTransform.position = Vector3.Lerp(letterSlideStartPos, letterSlideEndPos, ease);

            Vector3 lookDirection = letterTransform.position - cameraTransform.position;
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);

            cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, lookRotation, 0.5f);

            t += Time.deltaTime / letterSlideTime;
            yield return null;
        }

        yield return new WaitForSeconds(letterOpenDelay);

        letterTransform.gameObject.SetActive(false);

        audioSource.PlayOneShot(paperPickupClip);

        //Show the player the letter
        HudDocument.OpenDocument(letterText, null, true);
        HudDocument.OnDocumentUpdated += StartFinalFadeOut;
    }


    private void PlayCutscene()
    {
        //Steal the camera and take away player control
        Camera mainCamera = Camera.main;
        Transform cameraTransform = mainCamera.transform;

        cameraTransform.SetParent(transform);
        cameraTransform.position = Vector3.zero;
        cameraTransform.rotation = Quaternion.identity;

        PlayerController.Instance.gameObject.SetActive(false);

        StartCoroutine(CutsceneCoroutine());
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
        spotLight.gameObject.SetActive(false);
        soupTransform.gameObject.SetActive(false);
        letterTransform.gameObject.SetActive(false);

        ProgressionManager.OnProgressionStageUpdated += UpdateProgressionStage;
        UpdateProgressionStage(ProgressionManager.ProgressionStage);
    }


    private void OnDisable()
    {
        ProgressionManager.OnProgressionStageUpdated -= UpdateProgressionStage;
    }
}