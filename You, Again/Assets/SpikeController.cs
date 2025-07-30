using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    [Header("Spike Settings")]
    private LayerMask playerLayers = (1 << 10) | (1 << 9); // mainPlayer, aliveClones, deadClones
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object is on a player layer
        if (IsPlayerLayer(other.gameObject.layer))
        {
            HandlePlayerDeath(other.gameObject);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Also handle regular collisions (non-trigger)
        if (IsPlayerLayer(collision.gameObject.layer))
        {
            HandlePlayerDeath(collision.gameObject);
        }
    }
    
    private bool IsPlayerLayer(int layer)
    {
        return (playerLayers.value & (1 << layer)) != 0;
    }
    
    private void HandlePlayerDeath(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            // Check if this is the main player
            if (playerController.IsMainPlayer())
            {
                // Main player died - trigger clone creation (same as pressing R)
                ReplayManager manager = FindObjectOfType<ReplayManager>();
                if (manager != null)
                {
                    Debug.Log("Main player hit spikes! Creating clone and resetting...");
                    manager.CreateClone();
                }
            }
            else
            {
                // Clone died - use the SetDead method to stop movement
                Debug.Log($"{player.name} hit spikes and died!");
                playerController.SetDead();
                
                // Optional: Add death effects here (particle system, sound, etc.)
            }
        }
    }
}