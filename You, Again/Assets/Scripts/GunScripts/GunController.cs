using UnityEngine;

public class GunController : MonoBehaviour
{
    public GameObject Bullet;
    public float bulletSpeed = 10f;
    public float horizontalOffset = 0.5f; // Offset to the side
    public int MaxBullets = 5;
    public int BulletsShot = 0;

    public void Shoot(bool IsFacingRight)
    {
        FindAnyObjectByType<PlaySFX>().playSFX("shoot");

        if (BulletsShot >= MaxBullets)
            return;

        // Calculate world spawn position
        Vector3 spawnOffset = new Vector3(IsFacingRight ? horizontalOffset : -horizontalOffset, 0f, 0f);
        Vector3 spawnPos = transform.position + spawnOffset;

        // Instantiate the bullet as a child of the gun
        GameObject newBullet = Instantiate(Bullet, spawnPos, Quaternion.identity, transform);

        // Assign velocity in world space
        Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2((IsFacingRight ? 1f : -1f) * bulletSpeed, 0f);
        }

        // Optional: flip sprite visually
        SpriteRenderer sr = newBullet.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = !IsFacingRight;
        }

        BulletsShot++;
    }
}
