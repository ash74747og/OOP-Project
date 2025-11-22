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
        // transform.localScale = Size; // REMOVED: Now handled automatically by PlatformBase
        startPosition = transform.position;
        
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Ensure it's kinematic so it pushes but isn't pushed
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.None; // Disable interpolation since we move transform directly in Update
        
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

    private void Update()
    {
        if (!isMoving) return;

        Vector3 destination = movingToTarget ? targetPosition : startPosition;
        
        // Calculate new position
        // Using Update + MoveTowards is often smoother for CharacterController riding than FixedUpdate
        Vector3 newPosition = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        
        // DIRECTLY set transform.position to ensure it updates immediately for the next frame's calculation
        // This prevents "drift" where MoveTowards calculates from a stale position
        transform.position = newPosition;
        
        // Sync Rigidbody if present (just to keep it happy, though for kinematic it's redundant if we set transform)
        if (rb != null)
        {
            rb.position = newPosition; 
        }

        if (Vector3.Distance(transform.position, destination) < 0.01f)
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
