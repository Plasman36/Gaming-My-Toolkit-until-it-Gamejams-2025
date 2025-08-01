using UnityEngine;

public class TriggerExplosive : MonoBehaviour
{
    Explosive explosive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        explosive = GetComponent<Explosive>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponentInParent<PlayerController>() != null)
        {
            explosive.explode();
        }
    }
}
