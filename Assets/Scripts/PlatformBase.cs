using UnityEngine;

public abstract class PlatformBase : MonoBehaviour
{
    // Encapsulation: Private serialized fields with public properties
    [SerializeField] private Vector3 size;
    [SerializeField] private bool isCollidable;

    public Vector3 Size
    {
        get { return size; }
        set 
        { 
            size = value; 
            transform.localScale = size; // Update physical size when property is set
        }
    }

    public bool IsCollidable
    {
        get { return isCollidable; }
        set { isCollidable = value; }
    }

    protected GameObject activePlayer;
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    protected virtual void Awake()
    {
        // Auto-take size from the object's transform
        size = transform.localScale;
        lastPosition = transform.position;
        lastRotation = transform.rotation;

        // Ensure a collider exists
        if (GetComponent<Collider>() == null)
        {
            // Adding a BoxCollider as a default if none exists, 
            // though specific platforms might want different colliders.
            gameObject.AddComponent<BoxCollider>();
        }

        SetupPlatform();
    }

    protected virtual void LateUpdate()
    {
        // Calculate how much the platform moved and rotated this frame
        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;

        Vector3 positionDelta = currentPosition - lastPosition;
        Quaternion rotationDelta = currentRotation * Quaternion.Inverse(lastRotation);

        // If player is on the platform, move/rotate them
        if (activePlayer != null)
        {
            CharacterController cc = activePlayer.GetComponent<CharacterController>();
            Rigidbody playerRb = activePlayer.GetComponent<Rigidbody>();

            if (cc != null)
            {
                // Rotate the player around the platform's pivot
                // Note: CharacterController doesn't handle rotation well via Move, so we rotate the transform directly
                // But we need to rotate the position offset too if the platform rotates!
                
                // 1. Rotation
                // Only rotate the player's facing direction if you want them to turn with the platform
                // activePlayer.transform.rotation = rotationDelta * activePlayer.transform.rotation; 
                
                // 2. Position offset due to rotation (if player isn't at center)
                Vector3 offset = activePlayer.transform.position - transform.position;
                Vector3 rotatedOffset = rotationDelta * offset;
                Vector3 rotationPositionDelta = rotatedOffset - offset;

                // Combine deltas
                Vector3 totalDelta = positionDelta + rotationPositionDelta;

                if (totalDelta != Vector3.zero)
                {
                    cc.Move(totalDelta);
                }
                
                // Optional: Rotate player look direction
                activePlayer.transform.rotation = rotationDelta * activePlayer.transform.rotation;
            }
            else if (playerRb != null && !playerRb.isKinematic)
            {
                // Rigidbody Logic
                // We need to move the Rigidbody to keep up with the platform
                // MovePosition is best for kinematic, but for dynamic player we might want to just adjust position or velocity
                // Adjusting position directly is safest for "sticking"
                
                // Calculate target position
                Vector3 offset = playerRb.position - transform.position;
                Vector3 rotatedOffset = rotationDelta * offset;
                Vector3 targetPos = transform.position + rotatedOffset + positionDelta; // Wait, positionDelta is already included in transform.position change? 
                // No, transform.position is current. lastPosition was previous.
                // New platform pos = Old platform pos + positionDelta
                // New player pos should be = New platform pos + rotatedOffset
                // Let's re-calculate:
                // Target Player Pos = Current Platform Pos + (Current Rotation * (Inverse Last Rotation * (Old Player Pos - Old Platform Pos)))
                // Which simplifies to: Current Platform Pos + (RotationDelta * (Old Player Pos - Old Platform Pos))
                // But we only have current player pos. 
                // Let's stick to deltas.
                
                Vector3 rotationPositionDelta = rotatedOffset - offset;
                Vector3 totalDelta = positionDelta + rotationPositionDelta;
                
                playerRb.MovePosition(playerRb.position + totalDelta);
                
                // Rotate player facing
                Quaternion targetRot = rotationDelta * playerRb.rotation;
                playerRb.MoveRotation(targetRot);
            }
            else
            {
                // Fallback for non-CC objects
                activePlayer.transform.position += positionDelta;
                activePlayer.transform.rotation = rotationDelta * activePlayer.transform.rotation;
            }
        }

        lastPosition = currentPosition;
        lastRotation = currentRotation;
    }

    // Abstraction: Abstract methods to be implemented by derived classes
    protected abstract void SetupPlatform();
    
    // Changed to virtual to allow base implementation for sticky behavior
    public virtual void OnPlayerStep(GameObject player)
    {
        activePlayer = player;
        StopAllCoroutines(); // Stop any pending exit routines
    }

    // Virtual method that can be overridden
    public virtual void OnPlayerLeave(GameObject player)
    {
        if (activePlayer == player)
        {
            StartCoroutine(ResetActivePlayerDelay());
        }
    }

    private System.Collections.IEnumerator ResetActivePlayerDelay()
    {
        yield return new WaitForEndOfFrame(); // Wait a frame to ensure it wasn't a jitter
        yield return new WaitForEndOfFrame();
        activePlayer = null;
    }
}
