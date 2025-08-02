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
        Transform pivot = transform.Find("StickPivot");
        pivot.localRotation = Quaternion.Euler(0, 0, activated ? 45f : -45f);
        activated = !activated;
    }
}
