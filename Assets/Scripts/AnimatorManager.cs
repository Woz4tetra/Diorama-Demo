using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    private Animator animator;
    private int horizontalAnimation;
    private int verticalAnimation;
    private const float dampingTime = 0.1f;  // animation smoothing constant
    private const float snapThreshold = 0.55f;
    private const float snapValue = 0.5f;
    private const float crossFadeValue = 0.2f;
    private bool wasFalling = false;

    private enum AnimationState
    {
        JUMPING,
        MOVEMENT,
        FALLING,
        LAND
    }

    private AnimationState animationState = AnimationState.MOVEMENT;

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

    public void UpdateInteracting(bool isFalling, bool isJumping)
    {
        if (isJumping && !IsInteracting())
        {
            PlayTargetAnimation("Jump", true);
            animationState = AnimationState.JUMPING;
        }

        if ((animationState == AnimationState.JUMPING || isFalling) && !IsInteracting())
        {
            // This will will set IsInteracting to true
            PlayTargetAnimation("Falling", true);
            animationState = AnimationState.FALLING;
        }
        if (wasFalling != isFalling && !isFalling && animationState == AnimationState.FALLING)
        {
            PlayTargetAnimation("Land", true);
            animationState = AnimationState.LAND;
        }
        if (animationState == AnimationState.LAND && !IsInteracting())
        {
            animationState = AnimationState.MOVEMENT;
        }
        wasFalling = isFalling;
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

    private void PlayTargetAnimation(string targetAnimation, bool isInteracting)
    {
        animator.applyRootMotion = isInteracting;
        animator.SetBool("isInteracting", isInteracting);
        animator.CrossFade(targetAnimation, crossFadeValue);
    }

    private bool IsInteracting()
    {
        // Check if an animation is playing that should lock controls out
        return animator.GetBool("isInteracting");
    }

    public bool AllowMovement()
    {
        return (
            animationState == AnimationState.MOVEMENT ||
            animationState == AnimationState.FALLING ||
            animationState == AnimationState.JUMPING
        );
    }
}
