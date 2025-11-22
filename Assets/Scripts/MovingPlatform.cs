using UnityEngine;

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

    // Encapsulation: Public property for Speed
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    protected override void SetupPlatform()
    {
        transform.localScale = Size;
        startPosition = transform.position;
        
        // If target position is not set (zero), maybe set it relative or just warn? 
        // For now, we assume user sets it in inspector. 
        // If it's relative, we might want to add startPosition to it, but usually target is absolute or relative.
        // Let's assume absolute for simplicity unless specified otherwise, but often relative is better for prefabs.
        // Given "targetPosition" name, usually implies absolute world or local. Let's treat as offset for easier usage if it's (0,0,0) default?
        // Actually, let's stick to the prompt "set initial positions".
        
        if (moveOnTouch)
        {
            isMoving = false;
        }
    }

    public override void OnPlayerStep(GameObject player)
    {
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
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, destination) < 0.01f)
        {
            if (loop)
            {
                movingToTarget = !movingToTarget;
            }
            else
            {
                // If not looping, maybe stop? Or just stay there.
                // Prompt says "cycle between start and target", implying loop is usually true or it handles one way.
                // If loop is false, maybe we stop at target?
                if (movingToTarget) // Reached target
                {
                    isMoving = false; // Stop moving
                }
                else // Reached start
                {
                    // If we go back to start, do we stop?
                    isMoving = false;
                }
                // If we want to ping-pong once, we need more logic, but "cycle" implies continuous or at least back and forth.
                // Let's assume loop means continuous ping-pong. 
                // If !loop, maybe it just goes to target and stops?
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
