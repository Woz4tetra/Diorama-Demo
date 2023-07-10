using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private PlayerControls playerControls;

    private Vector2 movementInput;

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            // Add lambda function callback to .performed that sets movementInput when a control action is performed
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
        }
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public MovementVector getMovement()
    {
        return new MovementVector(movementInput.x, movementInput.y);
    }
}
