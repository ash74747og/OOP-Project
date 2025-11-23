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
    }

    // Abstract method forcing children to implement input processing
    protected abstract void ProcessInput();

    protected virtual void Update()
    {
        ProcessInput();
    }

    // Functionality: Apply physics velocity
    protected void Move(Vector3 direction)
    {
        // Preserve vertical velocity (gravity/jumping) while setting horizontal velocity
        Vector3 velocity = direction * moveSpeed;
        velocity.y = rb.linearVelocity.y; // Using linearVelocity for Unity 6 compatibility, or velocity for older
        // Since I used linearVelocity in FallingPlatform and then reverted to velocity based on user feedback/check, 
        // I will stick to 'velocity' here to be safe unless I know for sure.
        // Actually, the user corrected me to use 'linearVelocity' in FallingPlatform earlier? 
        // Wait, looking back at Step 52, the USER changed 'velocity' to 'linearVelocity'. 
        // So I should use 'linearVelocity'.
        
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;
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
