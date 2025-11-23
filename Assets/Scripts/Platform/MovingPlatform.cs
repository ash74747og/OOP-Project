using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : PlatformBase
{
    // Encapsulation: Private serialized fields
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private bool loop = true;
    [SerializeField] private bool moveOnTouch = false;

    private Vector3 startPosition;
    private bool isMoving = true;
    private bool movingToTarget = true;
    private Rigidbody rb;

    // Encapsulation: Public property for Speed
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    protected override void SetupPlatform()
    {
        startPosition = transform.position;
        
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Ensure it's kinematic so it pushes but isn't pushed
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Enable interpolation for smooth movement with player
        
        if (moveOnTouch)
        {
            isMoving = false;
        }
    }

    public override void OnPlayerStep(GameObject player)
    {
        base.OnPlayerStep(player); // Enable sticky behavior

        // Polymorphism: Trigger movement if waiting for player
        if (moveOnTouch && !isMoving)
        {
            isMoving = true;
        }
    }

    protected override void FixedUpdate()
    {
        // If not moving, let the base class handle sticky logic (e.g. for rotation or if pushed)
        if (!isMoving) 
        {
            base.FixedUpdate();
            return;
        }

        Vector3 destination = movingToTarget ? targetPosition : startPosition;
        
        // Calculate new position
        // Using FixedUpdate + MovePosition for physics-based movement
        Vector3 newPosition = Vector3.MoveTowards(rb.position, destination, speed * Time.fixedDeltaTime);
        
        // Calculate delta explicitly for the player
        Vector3 moveDelta = newPosition - rb.position;

        // Move the platform
        rb.MovePosition(newPosition);
        
        // Manually move the player in the same frame to prevent lag/jitter
        MoveActivePlayer(moveDelta, Quaternion.identity);

        // Update base class state so it doesn't get confused if we switch back to base logic
        lastPosition = newPosition;
        lastRotation = transform.rotation;

        if (Vector3.Distance(rb.position, destination) < 0.01f)
        {
            if (loop)
            {
                movingToTarget = !movingToTarget;
            }
            else
            {
                if (movingToTarget) // Reached target
                {
                    isMoving = false; // Stop moving
                }
                else // Reached start
                {
                    isMoving = false;
                }
            }
        }
    }
    
    // Draw gizmos to see the path in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, targetPosition);
        Gizmos.DrawWireSphere(targetPosition, 0.5f);
    }
}
