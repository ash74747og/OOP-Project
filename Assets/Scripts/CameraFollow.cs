using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -5);

    [Header("Settings")]
    [SerializeField] private float sensitivity = 5f;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private Vector2 pitchLimits = new Vector2(-20, 60);

    private float yaw;
    private float pitch;
    private Vector3 currentVelocity;

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        // Initialize angles
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Input
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

        // Calculate rotation and position
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        // Smooth movement
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);
        
        // Always look at target (or slightly above)
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
