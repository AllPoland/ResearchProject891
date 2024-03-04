using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    //The simplest, most braindead player controller ever conceived !!!!!!
    //No vertical movement, no velocity, built-in character controller, perfect.

    //This is used to progress the game only when the player is staring at the terminal
    //Avoids things changing while they're still visible
    public static event Action OnTerminalTransitionFinished;

    private bool terminalActive => TerminalScreen.TerminalActive;
    private Transform cameraTransform => Camera.main.transform;

    [Header("Configuration")]
    [SerializeField] private float playerHeight = 1.8f;
    [SerializeField] private float speed = 2f;

    [Space]
//Field is never used
#pragma warning disable 0414
    [SerializeField] private float defaultSensitivityMult = 1f;
    [SerializeField] private float defaultSensitivityWebgl = 1f;
#pragma warning restore 0414

    [Space]
    [SerializeField] private float cameraTransitionTime = 1f;
    [SerializeField] private float footstepLength = 1f;
    [SerializeField] private float headBobAmplitude = 0.25f;
    [SerializeField] private float footstepDecay = 10f;

    [Space]
    [SerializeField] private float breathTime = 2f;
    [SerializeField] private float breathAmplitude = 0.025f;

    [Header("Components")]
    [SerializeField] private Light pointLight;

    [Space]
    [SerializeField] public UnityEvent OnPlayerStep;
    [SerializeField] public UnityEvent OnPlayerShuffle;

    private CharacterController characterController;

    private Vector3 velocity;
    private Vector2 rotation;

    private float lightIntensity;

    private Vector3 previousFootstepPos;
    private float footstepOffset;

    private float breathProgress;
    private float breathOffset;

    private bool cameraTransitioning;
    private Coroutine cameraTransitionCoroutine;

    //Forces the camera to instantly go to its proper place the first time
    //the terminal is updated (scuffed)
    private bool cameraInitialized = false;


    private Vector3 GetCameraPos()
    {
        return new Vector3(0f, footstepOffset + breathOffset - (headBobAmplitude / 2), 0f);
    }


    private IEnumerator EnterTerminalCoroutine(Vector3 startPos, Vector2 startRotation, Vector3 targetPos, Vector2 targetRotation)
    {
        cameraTransitioning = true;

        float t = 0f;
        while(t < 1f)
        {
            float lerp = Easings.Quad.Out(t);
            cameraTransform.position = Vector3.Lerp(startPos, targetPos, lerp);

            Vector2 currentRotation = Vector2.Lerp(startRotation, targetRotation, lerp);
            cameraTransform.eulerAngles = new Vector3(currentRotation.y, currentRotation.x, 0f);

            float lightBrightness = 1 - Easings.Cubic.Out(t);
            pointLight.intensity = lightIntensity * lightBrightness;

            t += Time.deltaTime / cameraTransitionTime;
            yield return null;
        }

        cameraTransform.position = targetPos;
        cameraTransform.eulerAngles = new Vector3(targetRotation.y, targetRotation.x, 0f);

        pointLight.enabled = false;

        OnTerminalTransitionFinished?.Invoke();
        cameraTransitioning = false;
    }


    private IEnumerator ExitTerminalCoroutine(Vector3 startPos, Vector2 startRotation)
    {
        cameraTransitioning = true;

        pointLight.enabled = true;
        pointLight.intensity = 0f;

        float t = 0f;
        while(t < 1f)
        {
            Vector3 targetPos = transform.position + GetCameraPos();
            
            float lerp = Easings.Quad.Out(t);
            cameraTransform.position = Vector3.Lerp(startPos, targetPos, lerp);

            Vector2 currentRotation = Vector2.Lerp(startRotation, rotation, lerp);
            cameraTransform.eulerAngles = new Vector3(currentRotation.y, currentRotation.x, 0f);

            float lightBrightness = Easings.Cubic.In(t);
            pointLight.intensity = lightIntensity * lightBrightness;

            t += Time.deltaTime / cameraTransitionTime;
            yield return null;
        }
        
        cameraTransform.localPosition = GetCameraPos();
        cameraTransform.localEulerAngles = new Vector3(rotation.y, 0f, 0f);
        transform.eulerAngles = new Vector3(0f, rotation.x, 0f);

        pointLight.intensity = lightIntensity;

        OnTerminalTransitionFinished?.Invoke();
        cameraTransitioning = false;
    }


    private void InstantCameraTransition()
    {
        //Instantly updates the camera position instead of smoothly transitioning
        if(terminalActive)
        {
            Vector3 targetPos = TerminalScreen.Instance.targetCameraPosition;
            Vector2 targetRotation = TerminalScreen.Instance.targetCameraRotation;

            cameraTransform.position = targetPos;
            cameraTransform.eulerAngles = new Vector3(targetRotation.y, targetRotation.x, 0f);

            pointLight.enabled = false;
        }
        else
        {
            cameraTransform.localPosition = GetCameraPos();
            cameraTransform.localEulerAngles = new Vector3(rotation.y, 0f, 0f);
            transform.eulerAngles = new Vector3(0f, rotation.x, 0f);

            pointLight.enabled = true;
            pointLight.intensity = lightIntensity;
        }
    }


    private void StartCameraTransition()
    {
        if(cameraTransitioning)
        {
            StopCoroutine(cameraTransitionCoroutine);
        }

        if(!cameraInitialized)
        {
            InstantCameraTransition();
            cameraInitialized = true;
            return;
        }

        Vector3 startPos = cameraTransform.position;
        Vector2 startRotation = new Vector2(cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.x);

        if(startRotation.x > 180f)
        {
            startRotation.x -= 360f;
        }
        if(startRotation.y > 180f)
        {
            startRotation.y -= 360f;
        }

        if(terminalActive)
        {
            //If the terminal is active now, that means we're transitioning *from* walking mode
            Vector3 targetPos = TerminalScreen.Instance.targetCameraPosition;
            Vector2 targetRotation = TerminalScreen.Instance.targetCameraRotation;
            cameraTransitionCoroutine = StartCoroutine(EnterTerminalCoroutine(startPos, startRotation, targetPos, targetRotation));
        }
        else
        {
            cameraTransitionCoroutine = StartCoroutine(ExitTerminalCoroutine(startPos, startRotation));
        }
    }


    private void UpdateVelocity()
    {
        float forwardInput = Input.GetAxis("Forward");
        float sidewaysInput = Input.GetAxis("Sideways");

        Vector3 newVelocity = transform.forward * forwardInput + transform.right * sidewaysInput;
        newVelocity.y = 0;

        if(newVelocity.sqrMagnitude > 1f)
        {
            newVelocity.Normalize();
        }
        newVelocity *= speed;

        if(newVelocity.sqrMagnitude > 0.001f != velocity.sqrMagnitude > 0.001f || Vector3.Dot(newVelocity, velocity) < 0)
        {
            //The player has started/stopped walking or swapped direction
            OnPlayerShuffle?.Invoke();
        }

        velocity = newVelocity;
    }


    private void UpdateCamera()
    {
        //Read the mouse input and adjust it for sensitivity
        Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
#if UNITY_WEBGL && !UNITY_EDITOR
        delta *= SettingsManager.GetFloat("camsensitivity") * defaultSensitivityWebgl;
#else
        delta *= SettingsManager.GetFloat("camsensitivity") * defaultSensitivityMult;
#endif

        //Apply the mouse input to our rotation
        rotation.y = Mathf.Clamp(rotation.y - delta.y, -90f, 90f);
        rotation.x += delta.x;
        rotation.x %= 360;

        if(!cameraTransitioning)
        {
            //Apply rotation to transforms
            cameraTransform.localRotation = Quaternion.Euler(rotation.y, 0f, 0f);
            transform.rotation = Quaternion.Euler(0f, rotation.x, 0f);
        }
    }


    private void UpdateFootstep(bool moving)
    {
        float targetFootstepOffset = 0f;

        //If the player isn't moving, the camera should settle to 0
        if(moving)
        {
            float stepDistance = Vector3.Distance(transform.position, previousFootstepPos);
            float stepProgress = (stepDistance / footstepLength * 2) * Mathf.PI;

            targetFootstepOffset = Mathf.Sin(stepProgress) * headBobAmplitude;
        }

        //Smoothly move the camera to its target position
        float travelDistance = Mathf.Abs(footstepOffset - targetFootstepOffset) * footstepDecay * Time.deltaTime;
        footstepOffset = Mathf.MoveTowards(footstepOffset, targetFootstepOffset, travelDistance);
    }


    private void UpdateBreath()
    {
        breathProgress += Time.deltaTime / breathTime;
        breathProgress = Mathf.Repeat(breathProgress, 1f);

        breathOffset = Mathf.Sin(breathProgress * 2 * Mathf.PI) * breathAmplitude;
    }


    private void UpdateTerminalActive(bool terminalActive)
    {
        if(terminalActive)
        {
            characterController.enabled = false;
        }
        else
        {
            //Terminal is exited, bring the player back in front of it
            Vector2 newPos = TerminalScreen.Instance.targetPlayerPosition;
            Vector3 cameraPos = cameraTransform.position;

            transform.position = new Vector3(newPos.x, playerHeight, newPos.y);
            rotation = TerminalScreen.Instance.targetPlayerRotation;

            previousFootstepPos = transform.position;

            //Maintain the camera's position to not break the transition
            cameraTransform.position = cameraPos;
            characterController.enabled = true;
        }

        //Transition the camera in/out of the terminal view
        StartCameraTransition();
    }


    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        lightIntensity = pointLight.intensity;

        TerminalScreen.OnTerminalToggled += UpdateTerminalActive;
        if(TerminalScreen.Instance)
        {
            UpdateTerminalActive(terminalActive);
        }
    }


    void Update()
    {
        if(!terminalActive)
        {
            //Always update camera motion if we're not in the terminal
            UpdateBreath();
            if(HudDocument.DocumentActive)
            {
                //If a document is open, act as if the player isn't moving for head bob
                UpdateFootstep(false);
            }
            cameraTransform.localPosition = GetCameraPos();
        }

        if(terminalActive || HudDocument.DocumentActive)
        {
            //Disable input while the terminal is being used
            velocity = Vector2.zero;
        }
        else
        {
            //Update camera rotation
            UpdateCamera();
            //Get the player movement and change our velocity accordingly
            UpdateVelocity();

            //Update the transform position
            characterController.Move(velocity * Time.deltaTime);

            bool moving = velocity.sqrMagnitude > 0.001f;
            float footstepDistance = Vector3.Distance(transform.position, previousFootstepPos);
            if(footstepDistance >= footstepLength)
            {
                //The player has moved far enough to perform a step
                OnPlayerStep?.Invoke();
                previousFootstepPos = transform.position;
            }
            else if(!moving)
            {
                //Steps should decay while the player isn't moving
                float travelDistance = footstepDistance * footstepDecay * Time.deltaTime;
                previousFootstepPos = Vector3.MoveTowards(previousFootstepPos, transform.position, travelDistance);
            }

            UpdateFootstep(moving);
        }
    }
}