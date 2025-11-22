using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float turnSmoothTime = 0.1f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer = ~0; // Default to everything
    [SerializeField] private float groundCheckOffset = 0.1f;
    [SerializeField] private float groundCheckRadius = 0.3f;

    private CharacterController controller;
    private Vector3 velocity;
    private float turnSmoothVelocity;
    private bool isGrounded;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // If camera not assigned, try to find Main Camera
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Ground check
        // Combine CharacterController's check with a custom SphereCast for better moving platform support
        bool ccGrounded = controller.isGrounded;
        
        // Custom check: OverlapSphere to find ground, ignoring ourself
        // We use controller.bounds.min.y to get the bottom of the capsule (feet)
        // transform.position is usually the center for primitive capsules
        Vector3 feetPosition = new Vector3(transform.position.x, controller.bounds.min.y, transform.position.z);
        Vector3 checkPosition = feetPosition + Vector3.down * groundCheckOffset;
        
        bool customGrounded = false;
        Collider[] hits = Physics.OverlapSphere(checkPosition, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
        foreach (var hit in hits)
        {
            if (hit.gameObject != gameObject)
            {
                customGrounded = true;
                break;
            }
        }
        
        isGrounded = ccGrounded || customGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep grounded
        }

        // Input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        
        Vector3 moveDir = Vector3.zero;

        // Movement Calculation
        if (direction.magnitude >= 0.1f)
        {
            if (cameraTransform != null)
            {
                // Calculate target angle based on camera
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            }
            else
            {
                // Fallback if no camera assigned (move relative to world)
                moveDir = direction;
            }
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply Gravity
        velocity.y += gravity * Time.deltaTime;

        // Combine Movement (Horizontal + Vertical)
        Vector3 finalMovement = (moveDir.normalized * moveSpeed) + velocity;
        
        // Execute Move once per frame
        controller.Move(finalMovement * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        if (controller == null) return;

        Gizmos.color = Color.red;
        Vector3 feetPosition = new Vector3(transform.position.x, controller.bounds.min.y, transform.position.z);
        Gizmos.DrawWireSphere(feetPosition + Vector3.down * groundCheckOffset, groundCheckRadius);
    }
}
