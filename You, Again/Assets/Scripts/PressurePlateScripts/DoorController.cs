using System.Linq;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Info")]
    public bool MovesDown = true;
    public float MoveStrength = 20f;
    public Vector3 InitialScale;

    void Start()
    {
        InitialScale = transform.localScale;
    }

    public void UpdatePosition(bool PressedDown)
    {
        Vector3 ChangeVector = new Vector3(0, 0, 0);
        if (PressedDown && transform.localScale.y > 0)
        {
            ChangeVector = new Vector3(0, -1f, 0);
        }
        else if (!PressedDown && transform.localScale.y < InitialScale.y)
        {
            ChangeVector = new Vector3(0, 1f, 0);
        }

        int type = 1;
        if (!MovesDown) type = -1;

        transform.localPosition -= type * ChangeVector * Time.fixedDeltaTime * MoveStrength / 2;
        transform.localScale += ChangeVector * Time.fixedDeltaTime * MoveStrength;
    }
}
