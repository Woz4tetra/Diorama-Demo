using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    private Animator animator;
    private int horizontalAnimation;
    private int verticalAnimation;
    private const float dampingTime = 0.1f;  // animation smoothing constant
    private const float snapThreshold = 0.55f;
    private const float snapValue = 0.5f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        horizontalAnimation = Animator.StringToHash("Horizontal");
        verticalAnimation = Animator.StringToHash("Vertical");
    }

    public void UpdateAnimatorValues(MovementVector movementVector)
    {
        float moveAmount = movementVector.magnitude();
        float horizontalValue = moveAmount;
        float verticalValue = 0.0f;  // TODO: add strafing
        animator.SetFloat(horizontalAnimation, getSnapped(horizontalValue), dampingTime, Time.deltaTime);
        animator.SetFloat(verticalAnimation, getSnapped(verticalValue), dampingTime, Time.deltaTime);
    }

    private float getSnapped(float movementValue)
    {
        // Animation snapping for unsmooth animation transitions
        float snapped;
        if (movementValue > 0 && movementValue < snapThreshold)
        {
            snapped = snapValue;
        }
        else if (movementValue > snapThreshold)
        {
            snapped = Mathf.Abs(movementValue);
        }
        else if (movementValue < 0 && movementValue > -snapThreshold)
        {
            snapped = -snapValue;
        }
        else if (movementValue < -snapThreshold)
        {
            snapped = -Mathf.Abs(movementValue);
        }
        else
        {
            snapped = 0.0f;
        }

        return snapped;
    }
}
