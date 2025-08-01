using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MovingPlatform : MonoBehaviour
{
    public float speed;
    public Transform start;
    public Transform stop;

    [Header("Object Handling")]
    public int raycastCount = 10;
    public float platformWidth;
    public float maxGapDistance = 0.2f;
    public HashSet<GameObject> ObjectsOnPlatform = new HashSet<GameObject>();

    Transform platform;
    Rigidbody2D platformRB;
    private bool right = true;

    [Header("Debug Settings")]
    public bool showDebugRays = true;
    public float debugRayLength = 10f;
    public Color debugRayColorEmpty = Color.green;
    public Color debugRayColorHit = Color.red;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        platform = GetComponent<Transform>();
        platformRB = GetComponent<Rigidbody2D>();
        platformWidth = transform.localScale.x;
    }

    HashSet<GameObject> GetObjectsOnPlatform()
    {
        HashSet<GameObject> processedObjects = new HashSet<GameObject>();

        for (int i = 0; i < raycastCount; i++)
        {
            float rayOffset = (i / (float)(raycastCount - 1) - 0.5f) * platformWidth;
            float rayStartOffset = 0.05f; // Just above the platform
            Vector3 rayStart = transform.position + new Vector3(rayOffset, rayStartOffset, 0);


            RaycastHit2D[] hits = Physics2D.RaycastAll(rayStart, Vector2.up, Mathf.Infinity, ~LayerMask.GetMask("Ground"));

            var sortedHits = hits.OrderBy(hit => hit.distance).ToArray();

            ProcessRaycastHits(sortedHits, processedObjects, out float maxRelevantDistance);


            if (showDebugRays)
            {
                // Use the actual distance where objects contribute, or default length if no objects
                float debugLength = maxRelevantDistance > 0 ? maxRelevantDistance : debugRayLength;
                Color rayColor = processedObjects.Count > 0 ? debugRayColorHit : debugRayColorEmpty;
                Debug.DrawRay(rayStart, Vector2.up * debugLength, rayColor);
            }
        }



        return processedObjects;
    }

    void ProcessRaycastHits(RaycastHit2D[] sortedHits, HashSet<GameObject> processedObjects, out float maxRelevantDistance)
    {
        float lastDistance = 0f;
        maxRelevantDistance = 0f;

        foreach (RaycastHit2D hit in sortedHits)
        {
            if (processedObjects.Contains(hit.collider.gameObject))
                continue;

            float gap = hit.distance - lastDistance;
            if (gap > maxGapDistance)
            {
                break;
            }

            processedObjects.Add(hit.collider.gameObject);

            BoxCollider2D boxCollider = hit.collider.GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                lastDistance = hit.distance + boxCollider.size.y;
                maxRelevantDistance = lastDistance;
            }
            else
            {
                lastDistance = hit.distance;
                maxRelevantDistance = lastDistance;
            }

        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 position = new Vector2(0, 0);
        position.x = Mathf.Clamp(platform.position.x, start.position.x, stop.position.x);
        position.y = Mathf.Clamp(platform.position.y, start.position.y, stop.position.y);
        platform.position = position;

        if (right)
        {
            platformRB.linearVelocity = speed * Time.deltaTime * (stop.position - platform.position).normalized;
            if (platform.position == stop.position)
            {
                right = false;
            }
        }
        else
        {
            platformRB.linearVelocity = speed * Time.deltaTime * (start.position - platform.position).normalized;
            if (platform.position == start.position)
            {
                right = true;
            }
        }

        HashSet<GameObject> currentObjects = GetObjectsOnPlatform();
        foreach (GameObject curr in currentObjects)
        {
            Rigidbody2D RB = curr.GetComponent<Rigidbody2D>();
            RB.linearVelocity = platformRB.linearVelocity;
        }
    }
}
