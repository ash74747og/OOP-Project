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
            // Safety check: Ensure scale is positive to avoid BoxCollider errors
            Vector3 safeSize = new Vector3(
                Mathf.Abs(size.x), 
                Mathf.Abs(size.y), 
                Mathf.Abs(size.z)
            );
            transform.localScale = safeSize; 
        }
    }

    public bool IsCollidable
    {
        get { return isCollidable; }
        set { isCollidable = value; }
    }

    protected GameObject activePlayer;
    private Rigidbody platformRb;
    protected Vector3 lastPosition;
    protected Quaternion lastRotation;

    protected virtual void Awake()
    {
        platformRb = GetComponent<Rigidbody>();

        // Safety Check: Ensure transform scale is positive immediately
        Vector3 validScale = new Vector3(
            Mathf.Abs(transform.localScale.x),
            Mathf.Abs(transform.localScale.y),
            Mathf.Abs(transform.localScale.z)
        );
        transform.localScale = validScale;

        // Auto-take size from the object's transform
        size = transform.localScale;
        // Use RB position if available to avoid interpolation artifacts
        lastPosition = platformRb != null ? platformRb.position : transform.position;
        lastRotation = platformRb != null ? platformRb.rotation : transform.rotation;

        // Ensure a collider exists
        if (GetComponent<Collider>() == null)
        {
            // Adding a BoxCollider as a default if none exists, 
            // though specific platforms might want different colliders.
            gameObject.AddComponent<BoxCollider>();
        }

        SetupPlatform();
    }

    protected virtual void FixedUpdate()
    {
        // Calculate how much the platform moved and rotated this frame
        // Use RB position if available to ensure we get the raw physics position, not the interpolated visual position
        Vector3 currentPosition = platformRb != null ? platformRb.position : transform.position;
        Quaternion currentRotation = platformRb != null ? platformRb.rotation : transform.rotation;

        Vector3 positionDelta = currentPosition - lastPosition;
        Quaternion rotationDelta = currentRotation * Quaternion.Inverse(lastRotation);

        // Move player
        MoveActivePlayer(positionDelta, rotationDelta);

        lastPosition = currentPosition;
        lastRotation = currentRotation;
    }

    // Helper method to allow derived classes to manually move the player (e.g. MovingPlatform)
    protected void MoveActivePlayer(Vector3 positionDelta, Quaternion rotationDelta)
    {
        if (activePlayer != null)
        {
            CharacterController cc = activePlayer.GetComponent<CharacterController>();
            Rigidbody playerRb = activePlayer.GetComponent<Rigidbody>();

            if (cc != null)
            {
                Vector3 offset = activePlayer.transform.position - transform.position;
                Vector3 rotatedOffset = rotationDelta * offset;
                Vector3 rotationPositionDelta = rotatedOffset - offset;
                Vector3 totalDelta = positionDelta + rotationPositionDelta;

                if (totalDelta != Vector3.zero)
                {
                    cc.Move(totalDelta);
                }
                activePlayer.transform.rotation = rotationDelta * activePlayer.transform.rotation;
            }
            else if (playerRb != null && !playerRb.isKinematic)
            {
                // Rigidbody Logic: Use Velocity Injection
                // Calculate velocity required to move the distance this frame
                Vector3 platformVelocity = positionDelta / Time.fixedDeltaTime;
                
                // Inject into EntityLocomotion if available
                EntityLocomotion locomotion = playerRb.GetComponent<EntityLocomotion>();
                if (locomotion != null)
                {
                    locomotion.AddExternalVelocity(platformVelocity);
                }
                else
                {
                    // Fallback for generic Rigidbodies: Add force or velocity directly
                    // Note: This might fight with other controllers, but it's a fallback
                    playerRb.linearVelocity += platformVelocity;
                }
                
                Quaternion targetRot = rotationDelta * playerRb.rotation;
                playerRb.MoveRotation(targetRot);
            }
            else
            {
                activePlayer.transform.position += positionDelta;
                activePlayer.transform.rotation = rotationDelta * activePlayer.transform.rotation;
            }
        }
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

    protected virtual void OnValidate()
    {
        // Fix negative scale in Editor
        if (transform.localScale.x < 0 || transform.localScale.y < 0 || transform.localScale.z < 0)
        {
            Vector3 validScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                Mathf.Abs(transform.localScale.y),
                Mathf.Abs(transform.localScale.z)
            );
            transform.localScale = validScale;
            size = validScale;
        }
    }
}
