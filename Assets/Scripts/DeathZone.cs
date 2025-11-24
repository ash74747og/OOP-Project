using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering is the Player
        if (other.CompareTag("Player"))
        {
            // Try to get the PlayerController component
            PlayerController player = other.GetComponent<PlayerController>();
            
            // If found, call Respawn
            if (player != null)
            {
                Debug.Log("Player entered Death Zone. Respawning...");
                player.Respawn();
            }
        }
    }
}
