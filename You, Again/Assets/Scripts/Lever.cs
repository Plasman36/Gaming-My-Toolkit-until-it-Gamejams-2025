using UnityEngine;

public class Lever : MonoBehaviour
{
    [Header("Controllables")]
    public GameObject[] doors;
    public MovingSpikeHazard[] spikeHazards;
    private bool activated = false;


    void Update()
    {
        foreach (GameObject door in doors)
        {
            DoorController DC = door.GetComponent<DoorController>();
            DC.UpdatePosition(activated);
        }

        foreach (MovingSpikeHazard spike in spikeHazards)
        {
            spike.SetActivated(activated); // Pause if lever is flipped
        }
    }

    public void FlipLever()
    {
        transform.Rotate(0f, 180f, 0f);
        activated = !activated;
    }
}
