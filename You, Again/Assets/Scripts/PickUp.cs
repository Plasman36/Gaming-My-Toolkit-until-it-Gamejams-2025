using UnityEngine;

public class PickUp : MonoBehaviour
{
    // Process
    /*Be touching object
    Remove object collider once picked up
    Lift object above head (object scale.y / 2 ig) */

    [Header("Info")]
    [SerializeField] private GameObject heldObject;

    [Header("Object Check")]
    public Transform lCheck;
    public Transform rCheck;
    public float objectCheckRadius = 0.2f;
    public LayerMask objectLayerMask = 7;

    [Header("State")]
    public bool holding = false;

    private Collider2D result;

    private void PickItUp(GameObject pickMeUp)
    {
        heldObject = pickMeUp;
        holding = true;
        Debug.Log("Picked up");
        pickMeUp.transform.parent = gameObject.transform;
        pickMeUp.GetComponent<Rigidbody2D>().simulated = false;
        pickMeUp.transform.localPosition = new Vector3(0, pickMeUp.transform.localScale.y/2, 0);
    }

    private void DropItDown()
    {
        holding = false;
        Debug.Log("Dropped down");
        heldObject.transform.parent = null;
        heldObject.GetComponent<Rigidbody2D>().simulated = true;
        heldObject = null;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)){
            result = Physics2D.OverlapCircle(rCheck.position, objectCheckRadius, objectLayerMask);
            if (result != null)
            {
                PickItUp(result.gameObject);
            }
            else
            {
                result = Physics2D.OverlapCircle(lCheck.position, objectCheckRadius, objectLayerMask);
                if (result != null)
                {
                    PickItUp(result.gameObject);
                }
            }
            Debug.Log(result);
        }

        if(Input.GetKeyDown(KeyCode.Q) && holding){
            DropItDown();
        }
    }
}
