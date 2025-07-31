using UnityEngine;

public class PickUp : MonoBehaviour
{

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
        pickMeUp.transform.localPosition = new Vector3(0, pickMeUp.transform.localScale.y/2 + gameObject.transform.localScale.y/1.5f, 0);
        pickMeUp.transform.localRotation = Quaternion.identity;
    }

    public void DropItDown()
    {
        holding = false;
        Debug.Log("Dropped down");
        heldObject.transform.parent = null;
        heldObject.GetComponent<Rigidbody2D>().simulated = true;
        heldObject = null;
    }

    public void PickUpCheck()
    {
        result = Physics2D.OverlapCircle(rCheck.position, objectCheckRadius, objectLayerMask);
        if(result != null){
            PickItUp(result.gameObject);
        }else{
            result = Physics2D.OverlapCircle(lCheck.position, objectCheckRadius, objectLayerMask);
            if(result != null){
                PickItUp(result.gameObject);
            }
        }
    }

    void Update()
    {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        PlayerController PC = this.gameObject.GetComponent<PlayerController>();
        if (!PC.IsMainPlayer())
        {
            return;
        }
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        if(Input.GetKeyDown(KeyCode.E) && !holding){
            PickUpCheck();
        }

        if(Input.GetKeyDown(KeyCode.Q) && holding){
            DropItDown();
        }
<<<<<<< Updated upstream
<<<<<<< Updated upstream

        if (holding) {
            Rigidbody2D PlayerRB = this.gameObject.GetComponent<Rigidbody2D>();
            heldObject.GetComponent<Rigidbody2D>().linearVelocity = PlayerRB.linearVelocity;
            heldObject.transform.localPosition = new Vector3(0, heldObject.transform.localScale.y / 2 + gameObject.transform.localScale.y / 2, 0);
        }


=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    }
}
