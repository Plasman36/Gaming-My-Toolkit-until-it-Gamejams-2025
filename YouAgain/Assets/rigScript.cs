using UnityEngine;

public class rigScript : MonoBehaviour
{
    public GameObject player;
    void Update()
    {
        transform.position = player.transform.position;
    }
}
