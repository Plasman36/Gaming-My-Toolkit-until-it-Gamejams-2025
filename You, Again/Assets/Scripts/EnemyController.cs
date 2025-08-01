using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float rayDistance = 20f;
    public float moveSpeed = 5f;
    public LayerMask visionBlockingLayers = ~0;   // Default: collide with everything
    public LayerMask detectionLayer = 1 << 9;     // Layer 9 only

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

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
        return (detectionLayer.value & (1 << layer)) != 0;
    }

    private void HandlePlayerDeath(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            if (playerController.IsMainPlayer())
            {
                ReplayManager manager = FindObjectOfType<ReplayManager>();
                if (manager != null)
                {
                    Debug.Log("Main player hit enemy! Creating clone and resetting...");
                    manager.Death();
                }
            }
            else
            {
                Debug.Log($"{player.name} hit enemy and died!");
                playerController.SetDead();
            }
        }
    }

    void Update()
    {
        Vector2 position = transform.position;
        float rayOffset = transform.localScale.x/2 + 0.1f; // Start ray slightly away from enemy

        Vector2 rightStart = position + Vector2.right * rayOffset;
        Vector2 leftStart = position + Vector2.left * rayOffset;

        RaycastHit2D hitRight = Physics2D.Raycast(rightStart, Vector2.right, rayDistance - rayOffset, visionBlockingLayers);
        RaycastHit2D hitLeft = Physics2D.Raycast(leftStart, Vector2.left, rayDistance - rayOffset, visionBlockingLayers);

        if (hitRight.collider != null && ((1 << hitRight.collider.gameObject.layer) & detectionLayer) != 0)
        {
            // Saw something on Layer 9 to the right
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        }
        else if (hitLeft.collider != null && ((1 << hitLeft.collider.gameObject.layer) & detectionLayer) != 0)
        {
            // Saw something on Layer 9 to the left
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            // Nothing in sight or blocked
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
}
