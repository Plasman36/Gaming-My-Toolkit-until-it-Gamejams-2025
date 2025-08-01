using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RayGunScript : MonoBehaviour
{
    public float SizeChange = 0.5f;
    public LayerMask NotAllowed = (1 << 0) | (1 << 3);
    private List<GameObject> HitList = new List<GameObject>();
    // Update is called once per frame
    void Update()
    {
        Vector2 StartPosition = gameObject.transform.position;
        float zRotationDegrees = transform.eulerAngles.z;
        float zRotationRadians = zRotationDegrees * Mathf.Deg2Rad;

        Vector2 direction = new Vector2(Mathf.Cos(zRotationRadians), Mathf.Sin(zRotationRadians));

        RaycastHit2D hit = Physics2D.Raycast(StartPosition, direction, 200f);
        if (!(bool) hit || !hit.collider.gameObject)
        {
            return;
        }

        LineRenderer LR = GetComponent<LineRenderer>();
        LR.SetPosition(0, transform.position);
        LR.SetPosition(1, transform.position + new Vector3(hit.distance * direction.x, hit.distance * direction.y, 0));

        GameObject other = hit.collider.gameObject;
        if ((~NotAllowed.value & (1 << other.layer)) != 0 && !HitList.Contains(other))
        {
            other.transform.localScale = other.transform.localScale * SizeChange;
            HitList.Add(other);
            Debug.Log(HitList);
        }
    }
}
