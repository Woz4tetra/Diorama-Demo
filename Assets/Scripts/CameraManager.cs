using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float cameraFollowDamping = 0.2f; // The speed of the camera
    [SerializeField] private float panScale = 1.0f;
    [SerializeField] private float tiltScale = 0.1f;
    [SerializeField] private float sphereCastRadius = 0.2f;  // How much the camera will jump off of objects it's colliding with
    [SerializeField] private float minimumCollisionOffset = 0.05f;

    [SerializeField] Transform targetTransform;  // The object the camera will follow
    [SerializeField] Transform cameraPivot;  // The pivot the camera will locally rotate around
    [SerializeField] Transform cameraTransform;  // The transform of the camera object in the scene
    [SerializeField] LayerMask collisionLayers;  // The layers the camera will collide with
    [Range(-90.0f, 0.0f)][SerializeField] private float minTiltAngle = -35.0f;
    [Range(0.0f, 90.0f)][SerializeField] private float maxTiltAngle = 35.0f;


    private float panAngle = 0.0f;
    private float tiltAngle = 0.0f;
    private float defaultZDistance;
    private float cameraToTargetDistance = 0.0f;

    private Vector3 cameraFollowVelocity = Vector3.zero; // The velocity of the camera
    private Vector3 cameraVectorPosition = Vector3.zero;


    void Awake()
    {
        defaultZDistance = Vector3.Distance(cameraTransform.position, cameraPivot.position);
        cameraToTargetDistance = defaultZDistance;
    }

    public void HandleAllMovement(MovementVector cameraRotationInput)
    {
        FollowTarget();
        RotateCamera(cameraRotationInput);
        HandleCameraCollisions();
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(
            transform.position,
            targetTransform.position,
            ref cameraFollowVelocity,
            cameraFollowDamping
        );
        transform.position = targetPosition;
    }

    private void RotateCamera(MovementVector cameraRotationInput)
    {
        panAngle = panAngle + cameraRotationInput.horizontal * panScale;

        Vector3 tiltRotation = Vector3.zero;
        tiltRotation.y = panAngle;
        Quaternion targetTiltRotation = Quaternion.Euler(tiltRotation);
        transform.rotation = targetTiltRotation;

        tiltAngle = tiltAngle - cameraRotationInput.vertical * tiltScale;
        tiltAngle = Mathf.Clamp(tiltAngle, minTiltAngle, maxTiltAngle);

        Vector3 panRotation = Vector3.zero;
        panRotation.x = tiltAngle;
        Quaternion targetPanRotation = Quaternion.Euler(panRotation);
        cameraPivot.localRotation = targetPanRotation;
    }

    private void HandleCameraCollisions()
    {
        float targetDistance = defaultZDistance;
        Vector3 direction = cameraPivot.position - cameraTransform.position;
        direction.Normalize();
        RaycastHit hit;
        if (Physics.SphereCast(cameraPivot.position, sphereCastRadius, -direction, out hit, defaultZDistance, collisionLayers))
        {
            targetDistance = Vector3.Distance(cameraPivot.position, hit.point) - minimumCollisionOffset;
        }

        cameraToTargetDistance = targetDistance;

        cameraVectorPosition.z = -targetDistance;
        cameraTransform.localPosition = cameraVectorPosition;
    }

    public float GetCameraToTargetDistance()
    {
        return cameraToTargetDistance;
    }

    public float GetOpacityFromCameraDistance()
    {
        float scale = GetCameraToTargetDistance() - sphereCastRadius * 2;
        return Mathf.Clamp01(scale);
    }
}
