using UnityEngine;

public class GunController : MonoBehaviour
{
    public GameObject Bullet;
    public float bulletSpeed = 10f;
    public float horizontalOffset = 0.5f; // How far to the side the bullet spawns
    public int MaxBullets = 5;
    public int BulletsShot = 0;

    public void Shoot(bool IsFacingRight)
    {
        if (BulletsShot >= MaxBullets)
        {
            return;
        }
        // Calculate spawn position based on gun's position and direction
        Vector3 spawnPos = transform.position + new Vector3(IsFacingRight ? horizontalOffset : -horizontalOffset, 0f, 0f);

        // Instantiate the bullet
        GameObject newBullet = Instantiate(Bullet, spawnPos, Quaternion.identity);

        // Set bullet velocity
        Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2((IsFacingRight ? 1f : -1f) * bulletSpeed, 0f);
        }

        // Optional: Flip the bullet sprite if needed
        SpriteRenderer sr = newBullet.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = !IsFacingRight;
        }
        BulletsShot++;
    }
}
