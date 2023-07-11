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
        animatorManager.UpdateAnimatorValues(inputManager.getMovement());
    }

    void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement(inputManager.getMovement());
    }

    // This is called after the frame is finished rendering
    void LateUpdate()
    {
        cameraManager.HandleAllMovement(inputManager.getCameraRotation());
    }
}
