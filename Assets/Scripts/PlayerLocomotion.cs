using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 1.0f;
    [SerializeField] private float rotationSpeed = 1.0f;

    [Header("Falling")]
    [SerializeField] private float levelMaxHeight = 100.0f;
    [SerializeField] private float leapingVelocity = 1.0f;
    [SerializeField] private float leapingTime = 0.5f;
    [SerializeField] LayerMask groundLayer;  // The layers the camera will collide with
    [SerializeField] private float isFallingThreshold = 0.02f;
    private float epsilon = 1e-6f;
    private bool isFalling = false;
    private float inAirTimer = 0.0f;


    private Rigidbody playerRigidBody;
    private Vector3 moveDirection;
    private Transform cameraObject;
    private CapsuleCollider playerCollider;

    private void Awake()
    {
        playerRigidBody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        cameraObject = Camera.main.transform;  // search for main camera in scene
    }

    public void HandleAllMovement(MovementVector movementVector)
    {
        HandleVelocity(movementVector);
        HandleRotation(movementVector);
        HandleFalling();
    }

    private void HandleVelocity(MovementVector movementVector)
    {
        Vector3 movementVelocity = getMovementDirection(movementVector);
        movementVelocity *= movementSpeed * movementVector.magnitude();
        setPlayerVelocity(movementVelocity);
    }

    private void HandleRotation(MovementVector movementVector)
    {
        Vector3 targetDirection = getMovementDirection(movementVector);
        if (targetDirection.magnitude < epsilon)
        {
            targetDirection = getPlayerForwardDirection();
        }

        float interpolationDelta = rotationSpeed * Time.deltaTime;  // make rotation speed independent of frame rate
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(getPlayerRotation(), targetRotation, interpolationDelta);
        setPlayerRotation(playerRotation);
    }

    private void HandleFalling()
    {
        float groundDistance = GroundDistance();
        if (groundDistance > isFallingThreshold)
        {
            isFalling = true;
            inAirTimer += Time.deltaTime;
            if (inAirTimer < leapingTime)
            {
                playerRigidBody.AddForce(Vector3.down * leapingVelocity, ForceMode.Impulse);
            }
        }
        else
        {
            inAirTimer = 0.0f;
            isFalling = false;
        }
    }

    private Vector3 getPlayerForwardDirection()
    {
        return transform.forward;
    }

    private Quaternion getPlayerRotation()
    {
        return transform.rotation;
    }

    private void setPlayerRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }

    private void setPlayerVelocity(Vector3 movementVelocity)
    {
        Vector3 velocity = playerRigidBody.velocity;
        velocity.x = movementVelocity.x;
        velocity.z = movementVelocity.z;
        playerRigidBody.velocity = velocity;
    }

    private Vector3 getMovementDirection(MovementVector movementVector)
    {

        Vector3 verticalDirection = getCameraMovementDirection(cameraObject.forward);
        if (verticalDirection.magnitude < epsilon)
        {
            verticalDirection.z = 1.0f;
        }
        Vector3 direction = verticalDirection * movementVector.vertical;

        Vector3 horizontalDirection = getCameraMovementDirection(cameraObject.right);
        if (horizontalDirection.magnitude < epsilon)
        {
            horizontalDirection.x = 1.0f;
        }
        direction += horizontalDirection * movementVector.horizontal;
        direction.Normalize();  // normalize vector magnitude to 1
        direction.y = 0.0f;  // don't float up or down

        return direction;
    }

    private Vector3 getCameraMovementDirection(Vector3 cameraDirection)
    {
        Vector3 cameraMovementDirection = Vector3.zero;
        cameraMovementDirection.x = cameraDirection.x;
        cameraMovementDirection.z = cameraDirection.z;
        return cameraMovementDirection;
    }

    private float GroundDistance()
    {
        RaycastHit hit;
        Vector3 raycastOrigin = transform.position;
        raycastOrigin.y += playerCollider.height;
        if (Physics.SphereCast(raycastOrigin, playerCollider.radius, Vector3.down, out hit, levelMaxHeight, groundLayer))
        {
            return hit.distance -= playerCollider.height - playerCollider.radius;
        }
        return 0.0f;
    }

    public bool IsFalling()
    {
        return isFalling;
    }
}
