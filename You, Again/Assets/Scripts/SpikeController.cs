using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    [Header("Spike Settings")]
    private LayerMask playerLayers = 1 << 9; //aliveClones/mainPlayer


    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object is on a player layer
        if (IsPlayerLayer(other.gameObject.layer))
        {
            HandlePlayerDeath(other.gameObject);
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
                    manager.Death();
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