using System.Linq;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("Info")]
    public Transform playerCheck;
    public float objectCheckRadius = 0.2f;
    public LayerMask ActivationMask = (1 << 9) | (1 << 8) | (1 << 0) | (1 << 7);
    public Collider2D state;

    [Header("Controllables")]
    public GameObject[] doors;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    bool CheckIfObjectAbove()
    {
        state = Physics2D.OverlapCircle(playerCheck.position, objectCheckRadius, ActivationMask);
        return state != null;
    }

    // Update is called once per frame
    void Update()
    {
        bool PlayerAbove = CheckIfObjectAbove();
        foreach (GameObject door in doors)
        {
            DoorController DC = door.GetComponent<DoorController>();
            DC.UpdatePosition(PlayerAbove);
        }        
    }
}
