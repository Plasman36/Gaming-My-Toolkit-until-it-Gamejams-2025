using UnityEngine;

public class Explosive : MonoBehaviour
{

    public float explosiveRadius;
    public ParticleSystem explosion;

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
    public void explode()
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(self.position, explosiveRadius);
        foreach (Collider2D collider in collisions)
        {
            if (collider.gameObject.layer == 8 || collider.gameObject.layer == 9)
            {
                HandlePlayerDeath(collider.gameObject);
            } else if (collider.gameObject.CompareTag("Enemy"))
            {
                Debug.Log($"{collider.gameObject.name} blew up!");
                collider.gameObject.SetActive(false);
                Destroy(collider.gameObject);
            }
        }
        ParticleSystem playExplosion = Instantiate(explosion, self.position, Quaternion.identity);
        if (gameObject.CompareTag("Enemy"))
        {
            gameObject.SetActive(false);
        }
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
