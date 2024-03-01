using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    //The simplest, most braindead player controller ever conceived !!!!!!
    //No vertical movement, no velocity, built-in character controller, perfect.

    private bool terminalActive => TerminalScreen.TerminalActive;
    private Transform cameraTransform => Camera.main.transform;

    [SerializeField] private float playerHeight = 1.8f;
    [SerializeField] private float speed = 2f;

    [Space]
    [SerializeField] private float defaultSensitivityMult = 1f;

    [Space]
    [SerializeField] private Light pointLight;

    private CharacterController characterController;

    private Vector3 velocity;
    private float verticalRotation;
    private float horizontalRotation;


    private void UpdateVelocity()
    {
        float forwardInput = Input.GetAxis("Forward");
        float sidewaysInput = Input.GetAxis("Sideways");

        velocity = transform.forward * forwardInput + transform.right * sidewaysInput;
        velocity.y = 0;

        if(velocity.sqrMagnitude > 1f)
        {
            velocity.Normalize();
        }
        velocity *= speed;
    }


    private void UpdateCamera()
    {
        //Read the mouse input and adjust it for sensitivity
        Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        delta *= SettingsManager.GetFloat("camsensitivity") * defaultSensitivityMult;

        //Apply the mouse input to our rotation
        verticalRotation = Mathf.Clamp(verticalRotation - delta.y, -90f, 90f);
        horizontalRotation += delta.x;
        horizontalRotation %= 360;

        //Apply rotation to transforms
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);
    }


    private void UpdateTerminalActive(bool terminalActive)
    {
        if(terminalActive)
        {
            //Terminal is on, place the camera in front of it
            cameraTransform.position = TerminalScreen.Instance.targetCameraPosition;
            cameraTransform.eulerAngles = TerminalScreen.Instance.targetCameraRotation;

            pointLight.enabled = false;
            characterController.enabled = false;
        }
        else
        {
            //Terminal is exited, bring the camera back to the player position
            Vector2 newPos = TerminalScreen.Instance.targetPlayerPosition;
            transform.position = new Vector3(newPos.x, playerHeight, newPos.y);

            horizontalRotation = TerminalScreen.Instance.targetPlayerRotation;
            transform.eulerAngles = new Vector3(0f, horizontalRotation, 0f);

            cameraTransform.localPosition = Vector3.zero;
            cameraTransform.localEulerAngles = Vector3.zero;

            pointLight.enabled = true;
            characterController.enabled = true;
        }
    }


    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        TerminalScreen.OnTerminalToggled += UpdateTerminalActive;
        if(TerminalScreen.Instance)
        {
            UpdateTerminalActive(terminalActive);
        }
    }


    void Update()
    {
        if(terminalActive)
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
        }
    }
}