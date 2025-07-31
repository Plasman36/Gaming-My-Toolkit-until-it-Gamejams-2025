using System.Linq;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("Info")]
    public Transform playerCheck;
    public float objectCheckRadius = 0.2f;
    public LayerMask playerLayerMask = (1 << 9) | (1 << 8);
    public Collider2D state;

    [Header("Controllables")]
    public GameObject[] doors;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    bool CheckPlayerAbove()
    {
        state = Physics2D.OverlapCircle(playerCheck.position, objectCheckRadius, playerLayerMask);
        return state != null;
    }

    // Update is called once per frame
    void Update()
    {
        bool PlayerAbove = CheckPlayerAbove();
        foreach (GameObject door in doors)
        {
            DoorController DC = door.GetComponent<DoorController>();
            DC.UpdatePosition(PlayerAbove);
        }        
    }
}
