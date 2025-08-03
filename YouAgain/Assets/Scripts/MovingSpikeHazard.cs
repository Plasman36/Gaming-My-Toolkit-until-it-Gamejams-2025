using UnityEngine;

public class MovingSpikeHazard : MonoBehaviour
{
    [Header("Movement Settings")]
    public float topY = 5f;
    public float bottomY = 0f;
    public float downSpeed = 5f;
    public float upSpeed = 2f;
    public float waitTime = 1f;
    public bool movesWhenPressed = false; // For pressure plate behavior

    private float waitTimer = 0f;
    private bool movingDown = true;
    private bool externallyActivated = true;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, bottomY, transform.position.z);
    }

    private void Update()
    {
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

        float currentSpeed = movingDown ? downSpeed : upSpeed;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, currentSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            movingDown = !movingDown;
            waitTimer = waitTime;
        }
    }

    public void SetActivated(bool isPressed)
    {
        externallyActivated = isPressed;
    }
}
