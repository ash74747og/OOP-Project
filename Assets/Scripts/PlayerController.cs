using UnityEngine;

public class PlayerController : EntityLocomotion
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float turnSmoothTime = 0.1f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer = ~0; // Default to everything
    [SerializeField] private float groundCheckOffset = 0.1f;
    [SerializeField] private float groundCheckRadius = 0.3f;

    private float turnSmoothVelocity;
    private bool isGrounded;
    private Collider playerCollider;

    protected override void Awake()
    {
        base.Awake();
        playerCollider = GetComponent<Collider>();
        
        // If camera not assigned, try to find Main Camera
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private Quaternion targetRotation;
    private bool hasRotationInput;

    // Polymorphism: Override the abstract method
    protected override void ProcessInput()
    {
        // Ground Check
        CheckGround();

        // Input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        
        Vector3 moveDir = Vector3.zero;
        hasRotationInput = false;

        // Movement Calculation
        if (direction.magnitude >= 0.1f)
        {
            hasRotationInput = true;
            if (cameraTransform != null)
            {
                // Calculate target angle based on camera
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                
                // Store rotation for FixedUpdate
                targetRotation = Quaternion.Euler(0f, angle, 0f);

                moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            }
            else
            {
                moveDir = direction;
                // Also rotate if no camera
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                targetRotation = Quaternion.Euler(0f, angle, 0f);
            }
        }
        else
        {
            // Maintain current rotation if no input
            targetRotation = transform.rotation;
        }

        // Call base Move method
        Move(moveDir.normalized);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        // Apply rotation in FixedUpdate to sync with physics interpolation
        if (hasRotationInput)
        {
            rb.MoveRotation(targetRotation);
        }
    }

    private void CheckGround()
    {
        // Custom check: OverlapSphere to find ground, ignoring ourself
        // We use bounds.min.y to get the bottom of the collider (feet)
        Vector3 feetPosition = new Vector3(transform.position.x, playerCollider.bounds.min.y, transform.position.z);
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
        
        isGrounded = customGrounded;
    }

    private void OnDrawGizmosSelected()
    {
        if (playerCollider == null) playerCollider = GetComponent<Collider>();
        if (playerCollider == null) return;

        Gizmos.color = Color.red;
        Vector3 feetPosition = new Vector3(transform.position.x, playerCollider.bounds.min.y, transform.position.z);
        Gizmos.DrawWireSphere(feetPosition + Vector3.down * groundCheckOffset, groundCheckRadius);
    }
}
