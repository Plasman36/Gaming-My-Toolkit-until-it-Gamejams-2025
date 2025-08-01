using UnityEngine;

public class MovingSpikeHazard : MonoBehaviour
{
    [Header("Spike Settings")]
    public float topY = 5f;
    public float bottomY = 0f;
    public float moveSpeed = 2f;
    public float waitTime = 1f;
    public bool movesWhenPressed = false; // New: determines logic direction

    private float waitTimer = 0f;
    private bool movingDown = true;
    private LayerMask playerLayers = (1 << 8) | (1 << 9);

    private float defaultSpeed;
    private bool externallyActivated = true; // default to true so it runs by default

    private void Start()
    {
        defaultSpeed = moveSpeed;
        transform.position = new Vector3(transform.position.x, bottomY, transform.position.z);
    }

    private void Update()
    {
        // Check activation logic
        bool shouldMove = (movesWhenPressed && externallyActivated) || (!movesWhenPressed && !externallyActivated);
        if (!shouldMove) return;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        Vector3 targetPos = movingDown
            ? new Vector3(transform.position.x, bottomY, transform.position.z)
            : new Vector3(transform.position.x, topY, transform.position.z);

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            movingDown = !movingDown;
            waitTimer = waitTime;
        }
    }

    // Called by pressure plate to set current state
    public void SetActivated(bool isPressed)
    {
        externallyActivated = isPressed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;

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
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            if (playerController.IsMainPlayer())
            {
                ReplayManager manager = FindObjectOfType<ReplayManager>();
                if (manager != null)
                {
                    Debug.Log("Main player hit moving spikes! Creating clone and resetting...");
                    manager.Death();
                }
            }
            else
            {
                Debug.Log($"{player.name} hit moving spikes and died!");
                playerController.SetDead();
            }
        }
    }
}
