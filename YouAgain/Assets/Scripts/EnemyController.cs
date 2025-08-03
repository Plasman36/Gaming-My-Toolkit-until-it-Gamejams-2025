using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float rayDistance = 20f;
    public float moveSpeed = 5f;
    public LayerMask visionBlockingLayers = ~0;   // Default: collide with everything
    public LayerMask detectionLayer = 1 << 9;     // Layer 9 only

    private Rigidbody2D rb;

    public Animator animator;

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

        bool foundPlayerRight = CheckDirectionForPlayer(rightStart, Vector2.right);
        bool foundPlayerLeft = CheckDirectionForPlayer(leftStart, Vector2.left);


        //if ((foundPlayerRight || foundPlayerLeft) && rb.linearVelocity == Vector2.zero)
        //{
        //    FindAnyObjectByType<PlaySFX>().playSFX("beep");
        //}

        if (foundPlayerRight)
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
            transform.localRotation = new Quaternion(0,0,0,0);
        }
        else if (foundPlayerLeft)
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
            transform.localRotation = new Quaternion(0, 180, 0, 0);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        //animations
        animator.SetFloat("speed", rb.linearVelocity.x);
    }

    private bool CheckDirectionForPlayer(Vector2 start, Vector2 direction)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(start, direction, rayDistance - (transform.localScale.x/2 + 0.1f), visionBlockingLayers);
        
        // Sort hits by distance
        System.Array.Sort(hits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                // Skip bullets and guns
                if (hit.collider.CompareTag("Bullet") || hit.collider.CompareTag("Gun"))
                {
                    continue;
                }

                // If it's a player on the detection layer, return true
                if (((1 << hit.collider.gameObject.layer) & detectionLayer) != 0)
                {
                    return true;
                }

                // Any other object blocks vision, break out
                break;
            }
        }

        return false;
    }
}