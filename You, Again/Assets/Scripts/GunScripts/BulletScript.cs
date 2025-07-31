using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Bullet Settings")]
    private LayerMask playerLayers = (1 << 9) | (1 << 8); // aliveClones/mainPlayer

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        // Check if the collided object is on a player layer
        if (IsPlayerLayer(other.layer))
        {
            HandlePlayerDeath(other);
        }
    }

    private bool IsPlayerLayer(int layer)
    {
        return (playerLayers.value & (1 << layer)) != 0;
    }

    private void HandlePlayerDeath(GameObject player)
    {
        PlayerController PC = player.GetComponent<PlayerController>();
        if (!PC.isAlive)
        {
            return;
        }
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            Destroy(gameObject);
            
            if (playerController.IsMainPlayer())
            {
                ReplayManager manager = FindObjectOfType<ReplayManager>();
                if (manager != null)
                {
                    Debug.Log("Main player hit spikes! Creating clone and resetting...");
                    manager.Death();
                }
            }
            else
            {
                Debug.Log($"{player.name} hit spikes and died!");
                playerController.SetDead();
            }
        }
    }
}
