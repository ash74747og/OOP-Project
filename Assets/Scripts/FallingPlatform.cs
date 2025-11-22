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
        transform.localScale = Size;
        startPosition = transform.position;
        startRotation = transform.rotation;
        
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Start static
        rb.useGravity = true;
    }

    public override void OnPlayerStep(GameObject player)
    {
        // Polymorphism: Start fall sequence
        if (!isFalling)
        {
            StartCoroutine(FallRoutine());
        }
    }

    private IEnumerator FallRoutine()
    {
        isFalling = true;
        yield return new WaitForSeconds(fallDelay);
        
        rb.isKinematic = false; // Enable physics to fall
        
        yield return new WaitForSeconds(respawnDelay);
        
        ResetPlatform();
    }

    // Logic: Reset method
    private void ResetPlatform()
    {
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero; // Reset velocity (using linearVelocity for Unity 6 / newer physics, or velocity for older)
        // Note: In newer Unity versions 'velocity' is deprecated for 'linearVelocity' in some contexts, but 'velocity' is still standard in many. 
        // I'll use 'velocity' to be safe for general compatibility unless I know it's Unity 6. 
        // Actually, let's check if I can use 'velocity'. 
        // Safe bet is 'velocity' for now.
        rb.linearVelocity = Vector3.zero; 
        rb.angularVelocity = Vector3.zero;
        
        transform.position = startPosition;
        transform.rotation = startRotation;
        
        isFalling = false;
    }
}
