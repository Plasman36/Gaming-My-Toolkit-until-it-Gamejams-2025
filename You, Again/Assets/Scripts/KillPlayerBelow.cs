using UnityEngine;

public class KillPlayerBelow : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach(PlayerController player in players)
        {
            if (player.gameObject.layer == 9 && player.transform.position.y < transform.position.y)
            {
                HandlePlayerDeath(player.gameObject);
            }
        }
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
                    Debug.Log("Main player hit moving spikes! Creating clone and resetting...");
                    manager.Death();
                }
            }
            else
            {
                Debug.Log($"{player.name} fell out of the world!");
                playerController.SetDead();
            }
        }
    }
}
