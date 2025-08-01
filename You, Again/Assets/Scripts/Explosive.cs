using UnityEngine;

public class Explosive : MonoBehaviour
{

    public float explosiveRadius;
    public ParticleSystem explosion;
    public float timeToExplode;

    bool willExplode = false;

    Transform self;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        self = GetComponent<Transform>();
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
                    Debug.Log("Main player blew up! Creating clone and resetting...");
                    manager.Death();
                }
            }
            else
            {
                Debug.Log($"{player.name} blew up and died!");
                playerController.SetDead();
            }
        }
    }
    private void explode()
    {
        FindAnyObjectByType<PlaySFX>().playSFX("explode");
        Collider2D[] collisions = Physics2D.OverlapCircleAll(self.position, explosiveRadius);
        foreach (Collider2D collider in collisions)
        {
            if (collider.gameObject.layer == 9)
            {
                HandlePlayerDeath(collider.gameObject);
            } else if (collider.gameObject.CompareTag("Enemy") || collider.gameObject.CompareTag("Breakable"))
            {
                Debug.Log($"{collider.gameObject.name} blew up!");
                collider.gameObject.SetActive(false);
            }
        }
        ParticleSystem playExplosion = Instantiate(explosion, self.position, Quaternion.identity);

        gameObject.SetActive(false);
    }
    public void triggerExplosive()
    {
        willExplode = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (willExplode)
        {
            timeToExplode -= Time.deltaTime;
        }
        if (timeToExplode <= 0.0f)
        {
            explode();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosiveRadius);
    }
}
