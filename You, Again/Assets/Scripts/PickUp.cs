using UnityEngine;

public class PickUp : MonoBehaviour
{
    // Process
    /*Be touching object
    Remove object collider once picked up
    Lift object above head (object scale.y / 2 ig) */

    [Header("Info")]
    public GameObject heldObject;
    public GameObject heldObjectClone;

    [Header("Object Check")]
    public Transform lCheck;
    public Transform rCheck;
    public float objectCheckRadius = 0.2f;
    public LayerMask objectLayerMask = 7;
    public LayerMask leverLayerMask = 10;

    [Header("Friction Stuff")]
    public PhysicsMaterial2D frictionless;

    /*
    When you pick up an object, the original is disabled and you end up with a clone that has no rigidbody, when you drop it,
    the original teleports to where the clone is, the clone is destroyed, and the original's rigidbody is altered to seem normal physics-wise
    */

    [Header("State")]
    public bool holding = false;

    private Collider2D result;

    private void PickItUp(GameObject pickMeUp)
    {
        if (pickMeUp.transform.parent != null)
        {
            return;
        }
        heldObject = pickMeUp;
        holding = true;
        Debug.Log("Picked up");

        Transform renderer = heldObject.transform.Find("renderer");
        PlayerController PC = GetComponent<PlayerController>();

        if (heldObject.transform.rotation.y < 90 && PC.isFacingRight)
        {
            heldObject.transform.rotation = new Quaternion(0, 0, 0, 0);
        } else if (heldObject.transform.rotation.y > 90 && PC.isFacingRight)
        {
            heldObject.transform.rotation = new Quaternion(0, 180, 0, 0);
        } else if (heldObject.transform.rotation.y < 90 && !PC.isFacingRight)
        {
            heldObject.transform.rotation = new Quaternion(0, 180, 0, 0);
        } else if (heldObject.transform.rotation.y > 90 && !PC.isFacingRight)
        {
            heldObject.transform.rotation = new Quaternion(0, 0, 0, 0);
        }

        if (heldObject.CompareTag("Gun") || heldObject.CompareTag("Explosive"))
        {
            heldObject.transform.parent = gameObject.transform;
            heldObject.transform.localPosition = new Vector3(gameObject.transform.localScale.x / 2, 0, 0);
            heldObject.GetComponent<Rigidbody2D>().simulated = false;
            Physics2D.IgnoreCollision(heldObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), true);
        }else{
            // clone object, hide original, remove rigidbody, set parent, put in place, make clone frictionless for convenience
            heldObjectClone = Instantiate(pickMeUp);
            pickMeUp.SetActive(false);
            pickMeUp.transform.parent = gameObject.transform;
            Destroy(heldObjectClone.GetComponent<Rigidbody2D>());
            heldObjectClone.transform.parent = gameObject.transform;
            heldObjectClone.transform.localPosition = new Vector3(0, heldObjectClone.transform.localScale.y / 2 + gameObject.transform.localScale.y / 2, 0);
            heldObjectClone.GetComponent<Collider2D>().sharedMaterial = frictionless;
            Physics2D.IgnoreCollision(heldObjectClone.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), true);
        }
    }

    public void Shoot()
    {
        if (!heldObject || !heldObject.CompareTag("Gun"))
        {
            return;
        }

        GunController GC = heldObject.GetComponent<GunController>();
        PlayerController PC = gameObject.GetComponent<PlayerController>();
        GC.Shoot(PC.isFacingRight);
    }

    public void DropItDown()
    {
        if (!holding)
        {
            return;
        }

        if(heldObjectClone != null){
            // get original, teleport to where clone is, Destroy the clone, change velocity, heldObject null
            heldObject.transform.position = heldObjectClone.transform.position;
            heldObject.transform.parent = null;
            heldObject.SetActive(true);
            Physics2D.IgnoreCollision(heldObjectClone.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), false);
            Destroy(heldObjectClone);
        }else{
            // this is a gun, drop gun, turn off ignore collision, turn on rigidbody
            heldObject.transform.parent = null;
            Physics2D.IgnoreCollision(heldObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), false);
            heldObject.GetComponent<Rigidbody2D>().simulated = true;
        }
        holding = false;
        Debug.Log("Dropped down");
    
        Rigidbody2D PlayerRB = this.gameObject.GetComponent<Rigidbody2D>();
        Vector2 normalizedForY = PlayerRB.linearVelocity.normalized;
        heldObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(PlayerRB.linearVelocity.x * 1.5f, 3f * normalizedForY.y + 3);
        heldObject = null;
    }

    public void PickUpCheck()
    {
        if (holding)
        {
            return;
        }
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

    private void LeverCheck()
    {
        Collider2D MaskResult = Physics2D.OverlapCircle(rCheck.position, objectCheckRadius * 3, leverLayerMask);
        if(MaskResult != null){
            Lever leverScript = MaskResult.GetComponent<Lever>();
            leverScript.FlipLever();
        }else{
            MaskResult = Physics2D.OverlapCircle(lCheck.position, objectCheckRadius * 3, leverLayerMask);
            if(MaskResult != null){
                Lever leverScript = MaskResult.GetComponent<Lever>();
                leverScript.FlipLever();
            }
        }
    }

    void Update()
    {

        PlayerController PC = this.gameObject.GetComponent<PlayerController>();
        if (!PC.IsMainPlayer())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.G) && !holding)
        {
            PickUpCheck();
        }

        if (Input.GetKeyDown(KeyCode.G)){
            LeverCheck();
        }

        if (Input.GetKeyDown(KeyCode.H) && holding)
        {
            DropItDown();
        }
        if(Input.GetKeyDown(KeyCode.J) && holding){
            Shoot();
        }

    }
}