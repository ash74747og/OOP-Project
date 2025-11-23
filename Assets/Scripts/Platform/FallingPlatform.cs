using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class FallingPlatform : PlatformBase
{
    // Encapsulation: Private serialized fields
    [SerializeField] private float fallDelay = 1.0f;
    [SerializeField] private float respawnDelay = 3.0f;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rb;
    private bool isFalling = false;

    protected override void SetupPlatform()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Smooth rendering during fall
    }

    public override void OnPlayerStep(GameObject player)
    {
        // DON'T call base.OnPlayerStep if we're already falling
        // because base class calls StopAllCoroutines which would kill our fall routine!
        if (!isFalling)
        {
            base.OnPlayerStep(player);
            StartCoroutine(FallRoutine());
        }
    }

    protected override void FixedUpdate()
    {
        // If falling, let physics take over completely
        // We don't want the base "sticky" logic to interfere with the natural fall
        if (isFalling && !rb.isKinematic)
        {
            return;
        }
        
        base.FixedUpdate();
    }

    private IEnumerator FallRoutine()
    {
        isFalling = true;
        yield return new WaitForSeconds(fallDelay);
        
        // Detach player before falling to prevent jitter
        // The player will fall naturally with gravity instead of being "stuck" to the platform
        activePlayer = null;
        
        rb.isKinematic = false; // Enable physics to fall
        
        yield return new WaitForSeconds(respawnDelay);
        
        ResetPlatform();
    }

    // Logic: Reset method
    private void ResetPlatform()
    {
        rb.isKinematic = true;
        
        transform.position = startPosition;
        transform.rotation = startRotation;
        
        // Reset base class state to prevent massive delta calculation on next frame
        lastPosition = startPosition;
        lastRotation = startRotation;
        
        isFalling = false;
    }
}
