using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    [SerializeField] private PlatformBase parentPlatform;

    private void Awake()
    {
        // Auto-find parent if not assigned, assuming this script is on a child object
        if (parentPlatform == null)
        {
            parentPlatform = GetComponentInParent<PlatformBase>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object is the player
        // Assuming the player has a tag "Player" or we just check if it's the intended object.
        // The prompt says "if the other object is the Player". 
        // We'll use a tag check for simplicity, or just pass everything if we assume layers handle collision.
        // Let's check for "Player" tag to be safe, or just pass it. 
        // Prompt: "if the other object is the Player, call parentPlatform.OnPlayerStep(other.gameObject)."
        
        if (other.CompareTag("Player"))
        {
            if (parentPlatform != null)
            {
                parentPlatform.OnPlayerStep(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (parentPlatform != null)
            {
                parentPlatform.OnPlayerLeave(other.gameObject);
            }
        }
    }
}
