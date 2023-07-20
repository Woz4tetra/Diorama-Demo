using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerLocomotion playerLocomotion;
    private AnimatorManager animatorManager;
    private InputManager inputManager;
    [SerializeField] private CameraManager cameraManager;

    void Awake()
    {
        playerLocomotion = GetComponent<PlayerLocomotion>();
        inputManager = GetComponent<InputManager>();
        animatorManager = GetComponent<AnimatorManager>();
    }

    void Update()
    {
        setOpacity(cameraManager.GetOpacityFromCameraDistance());
        MovementVector movement = MovementVector.zero;
        if (!ShouldPaused())
        {
            movement = inputManager.GetMovement();
        }
        animatorManager.UpdateAnimatorValues(movement);
        animatorManager.UpdateInteracting(playerLocomotion.IsFalling());
        SetCursorLock(!ShouldPaused());
    }

    void FixedUpdate()
    {
        MovementVector movement = MovementVector.zero;
        if (!ShouldPaused() && animatorManager.AllowMovement())
        {
            movement = inputManager.GetMovement();
        }
        playerLocomotion.HandleAllMovement(movement);
    }

    // This is called after the frame is finished rendering
    void LateUpdate()
    {
        MovementVector movement = MovementVector.zero;
        if (!ShouldPaused())
        {
            movement = inputManager.GetCameraRotation();
        }
        cameraManager.HandleAllMovement(movement);
    }

    private bool ShouldPaused()
    {
        return inputManager.GetShouldPause();
    }

    private void setOpacity(float opacity)
    {
        foreach (Transform child in transform)
        {
            Renderer renderer = child.gameObject.GetComponent<Renderer>();
            if (renderer == null)
            {
                continue;
            }
            if (opacity >= 1.0f)
            {
                MaterialExtensions.ToOpaqueMode(renderer.material);
            }
            else
            {
                MaterialExtensions.ToFadeMode(renderer.material);
                Color color = renderer.material.color;
                color.a = opacity;
                renderer.material.color = color;
            }
        }
    }

    private void SetCursorLock(bool shouldLock)
    {
        if (shouldLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
