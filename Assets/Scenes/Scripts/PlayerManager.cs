using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerLocomotion playerLocomotion;
    private AnimatorManager animatorManager;
    private InputManager inputManager;

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
}
