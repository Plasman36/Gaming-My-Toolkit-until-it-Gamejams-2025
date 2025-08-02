using UnityEngine;

public class MovingSpikeHazard : MonoBehaviour
{
    [Header("Movement Settings")]
    public float topY = 5f;
    public float bottomY = 0f;
    public float moveSpeed = 2f;
    public float waitTime = 1f;
    public bool movesWhenPressed = false; // Toggle behavior for pressure plates

    private float waitTimer = 0f;
    private bool movingDown = true;
    private float defaultSpeed;
    private bool externallyActivated = true; // For pressure plates

    private void Start()
    {
        defaultSpeed = moveSpeed;
        transform.position = new Vector3(transform.position.x, topY, transform.position.z);
    }

    private void Update()
    {
        // Activation Logic:
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

    // Called by Pressure Plate to activate/deactivate
    public void SetActivated(bool isPressed)
    {
        externallyActivated = isPressed;
    }
}
