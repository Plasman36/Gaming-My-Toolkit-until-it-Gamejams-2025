using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Platform Settings")]
    public float maxSpeed;
    public float platformWidth = 5f; // Width of platform for raycast distribution
    public int raycastCount = 10; // Number of rays to cast per platform

    [Header("Weight Detection")]
    public float maxGapDistance = 1f; // Maximum gap between objects before breaking chain

    [Header("Colliders and Visual")]
    public Collider2D firstColl;
    public Collider2D secondColl;
    public LineRenderer lineRenderer;

    [Header("Rope Configuration")]
    public float firstRopeHeight;
    public float secondRopeHeight;

    [Header("Platform Transforms")]
    public Transform platformCheckOne;
    public Transform platformCheckTwo;
    public Transform checkOne;
    public Transform checkTwo;
    public Transform firstFloor;
    public Transform secondFloor;

    [Header("Visual Settings")]
    public float lineWidth;
    public Color colour; // WE ARE CANADIAN. CANADA, CANADA, CANADA!

    [Header("Debug Settings")]
    public bool showDebugRays = true;
    public float debugRayLength = 10f;
    public Color debugRayColorEmpty = Color.green;
    public Color debugRayColorHit = Color.red;

    // Private variables
    private float weightOne = 0.0f;
    private float weightTwo = 0.0f;

    private Vector3 firstRopePos;
    private Vector3 secondRopePos;

    private Vector3 startPosOne;
    private Vector3 startPosTwo;

    private float lowestPosOne;
    private float highestPosOne;
    private float lowestPosTwo;
    private float highestPosTwo;

    private bool blockedOne = false;
    private bool blockedTwo = false;

    private List<Collider2D> blocking = new List<Collider2D>();

    private float maxUp; // From perspective of first floor
    private float maxDown;

    void Start()
    {
        firstRopePos = firstFloor.position + new Vector3(0, firstRopeHeight, 0);
        secondRopePos = secondFloor.position + new Vector3(0, secondRopeHeight, 0);

        RaycastHit2D raycastOne = Physics2D.Raycast(checkOne.position, -Vector3.up, Mathf.Infinity, LayerMask.GetMask("Ground"));
        RaycastHit2D raycastTwo = Physics2D.Raycast(checkTwo.position, -Vector3.up, Mathf.Infinity, LayerMask.GetMask("Ground"));

        startPosOne = firstFloor.position;
        startPosTwo = secondFloor.position;

        maxUp = Mathf.Min(firstRopeHeight, raycastTwo.distance);
        maxDown = Mathf.Min(secondRopeHeight, raycastOne.distance);

        lowestPosOne = startPosOne.y - maxDown;
        highestPosOne = startPosOne.y + maxUp;

        lowestPosTwo = startPosTwo.y - maxUp;
        highestPosTwo = startPosTwo.y + maxDown;

        platformWidth = firstFloor.gameObject.transform.localScale.x;

        lineRenderer.positionCount = 4;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.startColor = colour;
        lineRenderer.endColor = colour;
    }

    void Update()
    {
        weightOne = CalculateWeightOnPlatform(platformCheckOne);
        weightTwo = CalculateWeightOnPlatform(platformCheckTwo);

        if ((!blockedOne && weightOne > weightTwo) || (!blockedTwo && weightTwo > weightOne))
        {
            float movement = maxSpeed * Time.deltaTime * (weightTwo - weightOne);
            firstFloor.position += new Vector3(0, movement, 0);
            secondFloor.position += new Vector3(0, -movement, 0);
        }

        // Clamp positions to valid ranges
        firstFloor.position = new Vector3(startPosOne.x, Mathf.Clamp(firstFloor.position.y, lowestPosOne, highestPosOne), startPosOne.z);
        secondFloor.position = new Vector3(startPosTwo.x, Mathf.Clamp(secondFloor.position.y, lowestPosTwo, highestPosTwo), startPosTwo.z);

        lineRenderer.SetPosition(0, firstFloor.position);
        lineRenderer.SetPosition(1, firstRopePos);
        lineRenderer.SetPosition(2, secondRopePos);
        lineRenderer.SetPosition(3, secondFloor.position);
    }

    float CalculateWeightOnPlatform(Transform platformCheck)
    {
        HashSet<GameObject> processedObjects = new HashSet<GameObject>();
        float totalWeight = 0f;

        // Cast multiple rays across the platform width (avoiding edges)
        for (int i = 0; i < raycastCount; i++)
        {
            // Calculate ray position evenly distributed within platform bounds (not on edges)
            float rayOffset = ((i) / (float)(raycastCount - 1) - 0.5f) * platformWidth;
            Vector3 rayStart = platformCheck.position + new Vector3(rayOffset, 0, 0);

            // Cast ray upward to find all objects
            RaycastHit2D[] hits = Physics2D.RaycastAll(rayStart, Vector2.up, Mathf.Infinity, ~LayerMask.GetMask("Ground"));

            // Sort hits by distance (closest first)
            var sortedHits = hits.OrderBy(hit => hit.distance).ToArray();

            // Process hits and get weight contribution from this ray
            float rayWeight = ProcessRaycastHits(sortedHits, processedObjects, out float maxRelevantDistance);
            totalWeight += rayWeight;

            // Debug visualization
            if (showDebugRays)
            {
                // Use the actual distance where objects contribute, or default length if no objects
                float debugLength = maxRelevantDistance > 0 ? maxRelevantDistance : debugRayLength;
                Color rayColor = rayWeight > 0 ? debugRayColorHit : debugRayColorEmpty;
                Debug.DrawRay(rayStart, Vector2.up * debugLength, rayColor);
            }
        }

        return totalWeight;
    }

    float ProcessRaycastHits(RaycastHit2D[] sortedHits, HashSet<GameObject> processedObjects, out float maxRelevantDistance)
    {
        float rayWeight = 0f;
        float lastDistance = 0f;
        maxRelevantDistance = 0f;

        foreach (RaycastHit2D hit in sortedHits)
        {
            // Skip if we've already processed this object
            if (processedObjects.Contains(hit.collider.gameObject))
                continue;

            // Check gap between objects (skip first object)
            float gap = hit.distance - lastDistance;
            if (gap > maxGapDistance)
            {
                // Gap is too large, break the chain
                break;
            }

            // Add object to processed list and accumulate weight
            processedObjects.Add(hit.collider.gameObject);

            Rigidbody2D rb = hit.collider.attachedRigidbody;
            if (rb != null)
            {
                rayWeight += rb.mass;
            }

            // Update last distance for next iteration
            BoxCollider2D boxCollider = hit.collider.GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                lastDistance = hit.distance + boxCollider.size.y;
                maxRelevantDistance = lastDistance; // Track the highest point that contributes
            }
            else
            {
                lastDistance = hit.distance;
                maxRelevantDistance = lastDistance;
            }
        }

        return rayWeight;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Collision detected with: {collision.otherCollider.name}");

        if (collision.otherCollider == firstColl)
        {
            HandlePlatformBlocking(platformCheckOne, collision.collider, ref blockedOne);
        }
        else if (collision.otherCollider == secondColl)
        {
            HandlePlatformBlocking(platformCheckTwo, collision.collider, ref blockedTwo);
        }
    }

    void HandlePlatformBlocking(Transform platformCheck, Collider2D collider, ref bool blocked)
    {
        Vector3 rayStart = platformCheck.position - new Vector3(platformWidth * 0.5f, 0, 0);
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayStart, Vector3.right, platformWidth);

        if (!hits.Any(hit => hit.collider == collider) && !blocking.Contains(collider))
        {
            blocked = true;
            blocking.Add(collider);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (blocking.Contains(collision.collider))
        {
            if (collision.otherCollider == firstColl)
            {
                blockedOne = false;
            }
            else if (collision.otherCollider == secondColl)
            {
                blockedTwo = false;
            }
            blocking.Remove(collision.collider);
        }
    }
}