using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public bool isFacingRight = true;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayerMask = 3;
    
    [Header("Collision Settings")]
    public int aliveClonesLayer = 9; // Layer for players/clones
    public int deadClonesLayer = 8; // Layer for players/clones
    
    public Rigidbody2D rb;
    private bool isGrounded;
    private bool isMainPlayer = true;
    private bool isReplaying = false;
    
    private List<InputFrame> inputsToReplay;
    private int replayIndex = 0;
    private float replayStartTime;
    public PickUp pickUpScript;

    [Header("State")]
    public bool isAlive = true;

    [Header("Friction Stuff")]
    public PhysicsMaterial2D Frictionless;
    public PhysicsMaterial2D Friction;

    [Header("Coyote Time")]
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [Header("Jump Buffering")]
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [Header("Animations")]
    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pickUpScript = GetComponent<PickUp>();
        gameObject.layer = aliveClonesLayer;

    }
    
    void Update()
    {
        //ANIMATIONS HERE
        animator.SetBool("isDead", !isAlive);
        animator.SetFloat("UpSpeed", rb.linearVelocity.y);

        if (isReplaying)
        {
            PlayRecordedInputs();
        }
        else if (isMainPlayer)
        {
            HandlePlayerInput();
        }

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        LayerMask CheckMask = groundLayerMask | (1 << deadClonesLayer) | (1 << 7);
        if ((CheckMask.value & (1 << collision.gameObject.layer)) != 0)
        {
            isGrounded = false;
        }    
    }


    void OnTriggerStay2D(Collider2D collision)
    {
        LayerMask CheckMask = groundLayerMask | (1 << deadClonesLayer) | (1 << 7);
        if ((CheckMask.value & (1 << collision.gameObject.layer)) != 0)
        {
            isGrounded = true;
        }
    }

    public void Reset()
    {
        isAlive = true;
        gameObject.layer = aliveClonesLayer;
        isFacingRight = true;
        gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    void HandlePlayerInput()
    {
        if (!isAlive)
        {
            return;
        }
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null) return;
        }

        if (pickUpScript == null)
        {
            pickUpScript = GetComponent<PickUp>();
        }

        float horizontal = Input.GetAxis("Horizontal");
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
        bool pickedUp = Input.GetKeyDown(KeyCode.G);
        bool dropped = Input.GetKeyDown(KeyCode.H);
        bool shot = Input.GetKeyDown(KeyCode.J);
        bool flipped = false;

        //ANIMATION LINE RIGHT HERE
        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        rb.linearVelocity = new Vector2(horizontal * moveSpeed * Time.fixedDeltaTime, rb.linearVelocity.y);


        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f && rb.linearVelocity.y <= 1f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * Time.fixedDeltaTime);
            coyoteTimeCounter = 0f;
            jumpBufferCounter = 0f;

        }

        //jump buffering for player
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        
        
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput > 0 && !isFacingRight)
        {
            Flip();
            flipped = true;
        }
        else if (moveInput < 0 && isFacingRight)
        {
            Flip();
            flipped = true;
        };


        if (isMainPlayer)
        {
            RecordInput(horizontal, jumpPressed, pickedUp, dropped, shot, flipped);
        }
    }
    
    void RecordInput(float horizontal, bool jumpPressed, bool pickedUp, bool dropped, bool shot, bool flipped)
    {
        ReplayManager manager = FindObjectOfType<ReplayManager>();
        if (manager != null)
        {
            manager.RecordInput(horizontal, jumpPressed, transform.position, pickedUp, dropped, shot, flipped);
        }
    }

    void PlayRecordedInputs()
    {
        // Don't play inputs if dead
        if (!isAlive)
        {
            return;
        }
        
        if (inputsToReplay == null || replayIndex >= inputsToReplay.Count)
            return;

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null) return;
        }

        if (pickUpScript == null)
        {
            pickUpScript = GetComponent<PickUp>();
        }

        float timeSinceStart = Time.time - replayStartTime;

        while (replayIndex < inputsToReplay.Count &&
            inputsToReplay[replayIndex].timestamp <= timeSinceStart)
        {
            InputFrame frame = inputsToReplay[replayIndex];

            //jump buffering for bot
            if (frame.jumpPressed)
            {
                jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                jumpBufferCounter -= Time.deltaTime;
            }

            rb.linearVelocity = new Vector2(frame.horizontalInput * moveSpeed * Time.fixedDeltaTime, rb.linearVelocity.y);

            if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f && rb.linearVelocity.y <= 1f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * Time.fixedDeltaTime);
                coyoteTimeCounter = 0f;
                jumpBufferCounter = 0f;
            }

            if(frame.pickedUp)
            {
                pickUpScript.PickUpCheck();
            }

            if(frame.dropped && pickUpScript.holding)
            {
                pickUpScript.DropItDown();
            }

            if (frame.shot)
            {
                pickUpScript.Shoot();
            }

            if (frame.flipped)
            {
                Flip();
            }

            replayIndex++;
        }

        if (replayIndex == inputsToReplay.Count)
        {
            SetDead();
        }

        //ANIMATIONS HERE
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }
    
    public bool IsMainPlayer()
    {
        return isMainPlayer;
    }

    public void SetDead()
    {
        isAlive = false;
        gameObject.layer = deadClonesLayer;
        pickUpScript.DropItDown();

        Collider2D col = gameObject.GetComponent<Collider2D>();
        col.sharedMaterial = Friction;

        rb.linearVelocity = new Vector2(0, 0);

        Debug.Log($"{gameObject.name} is now dead and stopped moving");
    }
    public void StartReplayingInputs(List<InputFrame> inputs)
    {
        isMainPlayer = false;
        isReplaying = true;
        inputsToReplay = new List<InputFrame>(inputs);
        replayIndex = 0;
        replayStartTime = Time.time;
        gameObject.layer = aliveClonesLayer;

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        if (pickUpScript == null)
        {
            pickUpScript = GetComponent<PickUp>();
        }

        if(pickUpScript.holding)
        {
            FindObjectOfType<ReplayManager>().revivedObjects.Add(pickUpScript.heldObject);
        }
    }

    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    void Flip()
    {
        Debug.Log($"{gameObject.name} flipped!");
        transform.Rotate(0f, 180f, 0f);
        isFacingRight = !isFacingRight;
    }
}

[System.Serializable]
public class InputFrame
{
    public float timestamp;
    public float horizontalInput;
    public bool jumpPressed;
    public Vector3 position;
    public bool pickedUp;
    public bool dropped;
    public bool shot;
    public bool flipped;
}
