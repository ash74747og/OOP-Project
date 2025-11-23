using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class EntityLocomotion : MonoBehaviour
{
    // Encapsulation: Private serialized fields with protected properties
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    protected float MoveSpeed => moveSpeed;
    protected float JumpForce => jumpForce;

    protected Rigidbody rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Usually good for characters
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Ensure smooth camera follow
        
        // Prevent sinking/tunneling, especially with higher mass or speed
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    // Abstract method forcing children to implement input processing
    protected abstract void ProcessInput();

    private Vector3 moveInput;
    private Vector3 externalVelocity;

    protected virtual void Update()
    {
        ProcessInput();
    }

    protected virtual void FixedUpdate()
    {
        // Apply physics velocity in FixedUpdate for smooth movement
        Vector3 velocity = moveInput * moveSpeed;
        velocity.y = rb.linearVelocity.y; // Preserve vertical velocity (gravity/jumping)
        
        // Add external velocity (e.g. moving platforms)
        if (externalVelocity.y > 0)
        {
            // If platform is moving UP, we must override gravity/falling speed 
            // to ensure we stick to it, otherwise gravity pulls us down and we "bop"
            velocity.y = Mathf.Max(velocity.y, externalVelocity.y);
            
            // Add horizontal normally
            velocity.x += externalVelocity.x;
            velocity.z += externalVelocity.z;
        }
        else
        {
            // Standard addition for other directions
            velocity += externalVelocity;
        }
        
        rb.linearVelocity = velocity;
        
        // Reset external velocity each frame so it doesn't persist if we leave the platform
        externalVelocity = Vector3.zero;
    }

    // Functionality: Apply physics velocity
    protected void Move(Vector3 direction)
    {
        // Store input for FixedUpdate
        moveInput = direction;
    }

    // Functionality: Allow external systems (platforms) to add velocity
    public void AddExternalVelocity(Vector3 velocity)
    {
        externalVelocity += velocity;
    }

    // Functionality: Apply upward force
    protected void Jump()
    {
        // Reset vertical velocity for consistent jump height
        Vector3 currentVel = rb.linearVelocity;
        currentVel.y = 0;
        rb.linearVelocity = currentVel;
        
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
