using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{

    [SerializeField] private float movementSpeed = 1.0f;
    [SerializeField] private float rotationSpeed = 1.0f;
    private float epsilon = 1e-6f;


    private Rigidbody playerRigidBody;
    private Vector3 moveDirection;
    private Transform cameraObject;

    private void Awake()
    {
        playerRigidBody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;  // search for main camera in scene
    }

    public void HandleAllMovement(MovementVector movementVector)
    {
        HandleVelocity(movementVector);
        HandleRotation(movementVector);
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
        playerRigidBody.velocity = movementVelocity;
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
}
