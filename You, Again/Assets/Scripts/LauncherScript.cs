using UnityEngine;

public class LauncherScript : MonoBehaviour
{
    public float LaunchForce = 20f;
    private LayerMask GroundMask = (1 << 3) | (1 << 30);
    public BoxCollider2D launcherCollider;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider != launcherCollider) return;
        Debug.Log("Player enter launch stuff");
        GameObject other = collision.gameObject;

        // Check if the collided object is on a player layer
        if (IsNotGroundLayer(other.layer))
        {
            HandleLaunch(other);
        }
    }

    private bool IsNotGroundLayer(int layer)
    {
        return (GroundMask.value & (1 << layer)) == 0;
    }

    private void HandleLaunch(GameObject player)
    {
        FindAnyObjectByType<PlaySFX>().playSFX("bounce");
        Rigidbody2D PlayerRB = player.GetComponent<Rigidbody2D>();
        Debug.Log(PlayerRB.linearVelocity);
        PlayerRB.AddForce(new Vector2(0, LaunchForce));
    }
}
