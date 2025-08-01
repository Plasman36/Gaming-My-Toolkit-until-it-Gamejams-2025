using UnityEngine;
using System.Collections.Generic;
public class NewMonoBehaviourScript : MonoBehaviour
{
    public float checkDist;
    public float maxSpeed;
    public float platformCheckRadius;

    public Collider2D firstColl;
    public Collider2D secondColl;
    public LineRenderer lineRenderer;

    public float firstRopeHeight;
    public float secondRopeHeight;

    public Transform platformCheckOne;
    public Transform platformCheckTwo;

    public Transform checkOne;
    public Transform checkTwo;

    public float lineWidth;
    public Color colour; //WE ARE CANADIAN. CANADA, CANADA, CANADA!

    public Transform firstFloor;
    public Transform secondFloor;

    private float weightOne = 0.0f;
    private float weightTwo = 0.0f;

    Vector3 firstRopePos;
    Vector3 secondRopePos;

    Vector3 startPosOne;
    Vector3 startPosTwo;

    float lowestPosOne;
    float highestPosOne;

    float lowestPosTwo;
    float highestPosTwo;

    bool blockedOne = false;
    bool blockedTwo = false;

    List<Collider2D> blocking = new List<Collider2D>();

    float maxUp; //From perspective of first floor.
    float maxDown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

        lineRenderer.positionCount = 4;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        lineRenderer.startColor = colour;
        lineRenderer.endColor = colour;
    }

    void checkObjectsOnOne()
    {
        List<GameObject> activeObjects = new List<GameObject>();

        weightOne = 0.0f;
        for (int i = 0; i < 3; i++)
        {
            bool newObjects = true;
            Vector3 start = platformCheckOne.position + (new Vector3(platformCheckRadius*3.5f, 0, 0)*(i-1));
            Debug.DrawRay(start, Vector3.up);
            float maxHeight = 0.0f;
            while (newObjects)
            {
                newObjects = false;


                RaycastHit2D[] rays = Physics2D.RaycastAll(start, Vector2.up, Mathf.Infinity, ~LayerMask.GetMask("Ground"));
                foreach (RaycastHit2D ray in rays)
                {
                    //Debug.Log($"Found object: {ray.collider.gameObject.name}, current distance: {ray.distance}, max distance: {maxHeight}");
                    if (!activeObjects.Contains(ray.collider.gameObject) && ray.distance < maxHeight+checkDist)
                    {
                        maxHeight = Mathf.Max(maxHeight, ray.distance+ray.collider.gameObject.GetComponent<BoxCollider2D>().size.y);
                        activeObjects.Add(ray.collider.gameObject);
                        weightOne += ray.collider.attachedRigidbody.mass;
                        newObjects = true;
                    }
                }
            }
        }
        //Debug.Log($"New weight on one after adding: {weightOne}");
    }
    void checkObjectsOnTwo()
    {
        List<GameObject> activeObjects = new List<GameObject>();

        weightTwo = 0.0f;
        for (int i = 0; i < 3; i++)
        {
            bool newObjects = true;
            Vector3 start = platformCheckTwo.position + (new Vector3(platformCheckRadius*3.5f, 0, 0) * (i - 1));
            float maxHeight = 0.0f;
            while (newObjects)
            {
                newObjects = false;


                RaycastHit2D[] rays = Physics2D.RaycastAll(start, Vector2.up, Mathf.Infinity, ~LayerMask.GetMask("Ground"));
                foreach (RaycastHit2D ray in rays)
                {
                    //Debug.Log($"Found object: {ray.collider.gameObject.name}, current distance: {ray.distance}, max distance: {maxHeight}");
                    if (!activeObjects.Contains(ray.collider.gameObject) && ray.distance < maxHeight + checkDist)
                    {
                        maxHeight = Mathf.Max(maxHeight, ray.distance + ray.collider.gameObject.GetComponent<BoxCollider2D>().size.y);
                        activeObjects.Add(ray.collider.gameObject);
                        weightTwo += ray.collider.attachedRigidbody.mass;
                        newObjects = true;
                    }
                }
            }
        }
        //Debug.Log($"New weight on two after adding: {weightTwo}");
    }

    bool findInRays(RaycastHit2D[] rays, Collider2D target)
    {
        foreach(RaycastHit2D hit in rays)
        {
            if (hit.collider == target)
            {
                return true;
            }
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        checkObjectsOnOne();
        checkObjectsOnTwo();

        if ((!blockedOne && weightOne > weightTwo) || (!blockedTwo && weightTwo > weightOne))
        {
            firstFloor.position += new Vector3(0, maxSpeed * Time.deltaTime * (weightTwo - weightOne), 0);
            secondFloor.position += new Vector3(0, maxSpeed * Time.deltaTime * (weightOne - weightTwo), 0);
        }

        firstFloor.position = new Vector3(startPosOne.x, Mathf.Clamp(firstFloor.position.y, lowestPosOne, highestPosOne), startPosOne.z);
        secondFloor.position = new Vector3(startPosTwo.x, Mathf.Clamp(secondFloor.position.y, lowestPosTwo, highestPosTwo), startPosTwo.z);

        lineRenderer.SetPosition(0, firstFloor.position);
        lineRenderer.SetPosition(1, firstRopePos);
        lineRenderer.SetPosition(2, secondRopePos);
        lineRenderer.SetPosition(3, secondFloor.position);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"collision is {collision.otherCollider.name}");
        if (collision.otherCollider == firstColl)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(platformCheckOne.position - new Vector3(platformCheckRadius*3.5f, 0, 0), Vector3.right, platformCheckRadius * 7);
            if (!findInRays(hits, collision.collider) && !blocking.Contains(collision.collider))
            {
                blockedOne = true;
                blocking.Add(collision.collider);
            }
        }
        else if (collision.otherCollider == secondColl)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(platformCheckTwo.position - new Vector3(platformCheckRadius * 3.5f, 0, 0), Vector3.right, platformCheckRadius * 7);
            if (!findInRays(hits, collision.collider) && !blocking.Contains(collision.collider))
            {
                blockedTwo = true;
                blocking.Add(collision.collider);
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (blocking.Contains(collision.collider))
        {
            if (collision.otherCollider == firstColl)
            {
                blockedOne = false;
            } else if (collision.otherCollider == secondColl)
            {
                blockedTwo = false;
            }
            blocking.Remove(collision.collider);
        }
    }

}
