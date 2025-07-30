using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayerMask = 1;
    
    [Header("Collision Settings")]
    public int mainPlayerLayer = 10; // Layer for players/clones
    public int aliveClonesLayer = 9; // Layer for players/clones
    public int deadClonesLayer = 8; // Layer for players/clones
    [SerializeField] private int groundLayer = 0; // Default layer for ground
    
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isMainPlayer = true;
    private bool isReplaying = false;
    
    private List<InputFrame> inputsToReplay;
    private int replayIndex = 0;
    private float replayStartTime;
    
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (!isMainPlayer)
        {
            gameObject.layer = aliveClonesLayer;
        }
        else
        {
            gameObject.layer = mainPlayerLayer;
        }
    }
    
    void Update()
    {
        CheckGrounded();
        
        if (isReplaying)
        {
            PlayRecordedInputs();
        }
        else if (isMainPlayer)
        {
            HandlePlayerInput();
        }
    }
    
    void CheckGrounded()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
        }
        else
        {
            Vector2 checkPosition = new Vector2(transform.position.x, transform.position.y - 0.5f);
            isGrounded = Physics2D.OverlapCircle(checkPosition, 0.1f, groundLayerMask);
        }
    }
    
    void HandlePlayerInput()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null) return;
        }
        
        float horizontal = Input.GetAxis("Horizontal");
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
        
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
        
        if (jumpPressed && isGrounded && rb.linearVelocity.y <= 1f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        
        if (isMainPlayer)
        {
            RecordInput(horizontal, jumpPressed);
        }
    }
    
    void RecordInput(float horizontal, bool jumpPressed)
    {
        ReplayManager manager = FindObjectOfType<ReplayManager>();
        if (manager != null)
        {
            manager.RecordInput(horizontal, jumpPressed, transform.position);
        }
    }

    void PlayRecordedInputs()
    {
        if (inputsToReplay == null || replayIndex >= inputsToReplay.Count)
            return;

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null) return;
        }

        float timeSinceStart = Time.time - replayStartTime;

        while (replayIndex < inputsToReplay.Count &&
               inputsToReplay[replayIndex].timestamp <= timeSinceStart)
        {
            InputFrame frame = inputsToReplay[replayIndex];

            rb.linearVelocity = new Vector2(frame.horizontalInput * moveSpeed, rb.linearVelocity.y);

            if (frame.jumpPressed && isGrounded && rb.linearVelocity.y <= 1f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }

            replayIndex++;
        }

        if (replayIndex == inputsToReplay.Count)
        {
            gameObject.layer = deadClonesLayer;
        }
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
    }

    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}

[System.Serializable]
public class InputFrame
{
    public float timestamp;
    public float horizontalInput;
    public bool jumpPressed;
    public Vector3 position;
}