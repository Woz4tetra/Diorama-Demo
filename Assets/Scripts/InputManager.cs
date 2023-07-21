using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerControls playerControls;

    private Vector2 movementInput;
    private Vector2 cameraRotationInput;
    private bool shouldPause = false;
    private bool shouldPlayerRun = false;
    private bool shouldPlayerJump = false;
    [SerializeField] private float runSpeedMultiplier = 2.0f;
    [SerializeField] private float minRunMagnitude = 0.4f;

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            // Add lambda function callback to .performed that sets movementInput when a Movement control action is performed
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();

            // Add lambda function callback to .performed that sets cameraInput when a Camera control action is performed
            playerControls.PlayerMovement.Camera.performed += i => cameraRotationInput = i.ReadValue<Vector2>();

            // Add lambda function callback to .performed that sets shouldPause when the pause button is pressed
            playerControls.PlayerMovement.Pause.performed += i => shouldPause = !shouldPause;

            // Add lambda function callback to .performed that sets shouldPlayerRun when the run button is held
            playerControls.PlayerMovement.RunToggle.performed += i => shouldPlayerRun = true;
            playerControls.PlayerMovement.RunToggle.canceled += i => shouldPlayerRun = false;

            // Add lambda function callback to .performed that sets shouldPlayerRun when the jump button is held
            playerControls.PlayerMovement.Jump.performed += i => shouldPlayerJump = true;
        }
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public MovementVector GetMovement()
    {
        Vector2 movementVector = new Vector2(movementInput.x, movementInput.y);
        if (shouldPlayerRun)
        {
            if (movementVector.magnitude > minRunMagnitude)
            {
                movementVector = movementVector.normalized * runSpeedMultiplier;
            }
        }
        return new MovementVector(movementVector.x, movementVector.y);
    }

    public MovementVector GetCameraRotation()
    {
        return new MovementVector(cameraRotationInput.x, cameraRotationInput.y);
    }

    public bool GetShouldPause()
    {
        return shouldPause;
    }

    public bool GetShouldPlayerJump()
    {
        bool result = shouldPlayerJump;
        shouldPlayerJump = false;
        return result;
    }
}
