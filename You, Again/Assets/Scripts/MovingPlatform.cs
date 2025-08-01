using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed;
    public Transform start;
    public Transform stop;

    Transform platform;
    Rigidbody2D platformRB;
    private bool right = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        platform = GetComponent<Transform>();
        platformRB = GetComponent<Rigidbody2D>();
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
        } else
        {
            platformRB.linearVelocity = speed * Time.deltaTime * (start.position - platform.position).normalized;
            if (platform.position == start.position)
            {
                right = true;
            }
        }

    }
}
