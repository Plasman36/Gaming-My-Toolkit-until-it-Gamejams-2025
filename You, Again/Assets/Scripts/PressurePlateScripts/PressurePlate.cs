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
    public MovingSpikeHazard[] spikeHazards;

    void Start()
    {

    }

    bool CheckIfObjectAbove()
    {
        state = Physics2D.OverlapCircle(playerCheck.position, objectCheckRadius, ActivationMask);
        return state != null;
    }

    void Update()
    {
        bool PlayerAbove = CheckIfObjectAbove();

        foreach (GameObject door in doors)
        {
            DoorController DC = door.GetComponent<DoorController>();
            DC.UpdatePosition(PlayerAbove);
        }

        foreach (MovingSpikeHazard spike in spikeHazards)
        {
            spike.SetPaused(PlayerAbove); // Pause if something is on the plate
        }
    }
}
