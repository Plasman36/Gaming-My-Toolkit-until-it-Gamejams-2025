using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ReplayManager : MonoBehaviour
{
    [Header("UI")]
    public Button createCloneButton;
    
    [Header("Player Setup")]
    public GameObject playerPrefab;
    public PlayerController mainPlayer;
    
    [Header("Spawn Settings")]
    public Transform spawnPoint;
    
    [Header("Layer Settings")]
    [SerializeField] private int playerLayer = 8; // Should match PlayerController
    
    // Storage for all recorded inputs
    private List<List<InputFrame>> allRecordedSegments = new List<List<InputFrame>>();
    private List<InputFrame> currentSegment = new List<InputFrame>();
    private List<GameObject> clones = new List<GameObject>();
    public int mainPlayerLayer = 10; // Layer for players/clones
    public int aliveClonesLayer = 9; // Layer for players/clones
    public int deadClonesLayer = 8; // Layer for players/clones
    
    private float recordingStartTime;
    private Vector3 startPosition;
    private static bool layerCollisionsSetup = false;
    private float collisionEnableTime;
    private bool collisionsDisabled = false;
    public float collisionDisableTime = 3f; // Time in seconds to disable collisions
    
    void Start()
    {
        Physics2D.IgnoreLayerCollision(aliveClonesLayer, aliveClonesLayer, true);
        Physics2D.IgnoreLayerCollision(deadClonesLayer, deadClonesLayer, true);
        Physics2D.IgnoreLayerCollision(aliveClonesLayer, deadClonesLayer, true);
        Physics2D.IgnoreLayerCollision(mainPlayerLayer, aliveClonesLayer, true);
        Physics2D.IgnoreLayerCollision(mainPlayerLayer, deadClonesLayer, true);
        // Set up button
        if (createCloneButton != null)
        {
            createCloneButton.onClick.AddListener(CreateClone);
        }
        
        // Find main player if not assigned
        if (mainPlayer == null)
        {
            mainPlayer = FindObjectOfType<PlayerController>();
        }
        
        // Set spawn point
        if (spawnPoint != null)
        {
            startPosition = spawnPoint.position;
        }
        else if (mainPlayer != null)
        {
            startPosition = mainPlayer.transform.position;
        }
        
        // Ensure main player is on correct layer
        if (mainPlayer != null)
        {
            mainPlayer.gameObject.layer = playerLayer;
        }
        
        // Initialize layer collision settings (only once)
        if (!layerCollisionsSetup)
        {
            // Initially allow collisions between players
            Physics2D.IgnoreLayerCollision(playerLayer, playerLayer, false);
            layerCollisionsSetup = true;
        }
        
        // Start recording
        StartRecording();
    }

    public void DisableCollisions()
    {
        collisionsDisabled = true;
        collisionEnableTime = Time.time + collisionDisableTime;
        
        Physics2D.IgnoreLayerCollision(aliveClonesLayer, aliveClonesLayer, true);
        Physics2D.IgnoreLayerCollision(deadClonesLayer, deadClonesLayer, true);
        Physics2D.IgnoreLayerCollision(aliveClonesLayer, deadClonesLayer, true);
        Physics2D.IgnoreLayerCollision(mainPlayerLayer, aliveClonesLayer, true);
        Physics2D.IgnoreLayerCollision(mainPlayerLayer, deadClonesLayer, true);

        Debug.Log($"{gameObject.name} disabled collisions with other players");
    }
    
    public void EnableCollisions()
    {
        collisionsDisabled = false;

        // Allow all except main <-> alive clones
        Physics2D.IgnoreLayerCollision(aliveClonesLayer, deadClonesLayer, false);
        Physics2D.IgnoreLayerCollision(mainPlayerLayer, deadClonesLayer, false);
        Physics2D.IgnoreLayerCollision(aliveClonesLayer, aliveClonesLayer, false);
        Physics2D.IgnoreLayerCollision(deadClonesLayer, deadClonesLayer, false);

        // Keep main player and alive clones separate
        Physics2D.IgnoreLayerCollision(mainPlayerLayer, aliveClonesLayer, true);

        Debug.Log($"{gameObject.name} collisions with other players re-enabled");
    }
    
    void StartRecording()
    {
        DisableCollisions();


        currentSegment.Clear();
        recordingStartTime = Time.time;
        
        Debug.Log("Started recording new segment");
    }
    
    public void RecordInput(float horizontal, bool jumpPressed, Vector3 position)
    {
        InputFrame frame = new InputFrame
        {
            timestamp = Time.time - recordingStartTime,
            horizontalInput = horizontal,
            jumpPressed = jumpPressed,
            position = position
        };
        
        currentSegment.Add(frame);
    }
    
    public void CreateClone()
    {
        if (currentSegment.Count == 0)
        {
            Debug.LogWarning("No inputs recorded yet!");
            return;
        }
        
        // Save current segment to the list
        allRecordedSegments.Add(new List<InputFrame>(currentSegment));
        
        // Reset main player to start position and disable collisions temporarily
        if (mainPlayer != null)
        {
            mainPlayer.transform.position = startPosition;
            Rigidbody2D mainRb = mainPlayer.GetComponent<Rigidbody2D>();
            if (mainRb != null)
            {
                mainRb.linearVelocity = Vector2.zero;
            }
            // Disable collisions for main player too
            DisableCollisions();
        }
        
        // Destroy all existing clones
        foreach (GameObject clone in clones)
        {
            if (clone != null)
            {
                Destroy(clone);
            }
        }
        clones.Clear();
        
        // Recreate all clones from all segments
        for (int i = 0; i < allRecordedSegments.Count; i++)
        {
            // Create a clone
            GameObject clone = Instantiate(playerPrefab, startPosition, Quaternion.identity);
            
            // Set clone to player layer
            clone.layer = playerLayer;
            
            // Set clone appearance
            SpriteRenderer cloneRenderer = clone.GetComponent<SpriteRenderer>();
            if (cloneRenderer != null)
            {
                cloneRenderer.color = GetCloneColor(i);
            }
            
            // Set clone name
            clone.name = $"Clone_{i + 1}";
            
            // Start the clone replaying its recorded inputs (this will auto-disable collisions)
            PlayerController cloneController = clone.GetComponent<PlayerController>();
            if (cloneController != null)
            {
                cloneController.StartReplayingInputs(allRecordedSegments[i]);
            }
            
            clones.Add(clone);
        }
        
        // Start recording a new segment for the next clone
        StartRecording();
        
        Debug.Log($"Created Clone {allRecordedSegments.Count}! Total clones: {clones.Count}");
        Debug.Log("All players reset to start position with collisions temporarily disabled!");
    }
    
    Color GetCloneColor(int index)
    {
        Color[] colors = {
            new Color(1f, 0.5f, 0.5f, 0.9f), // Light red
            new Color(0.5f, 1f, 0.5f, 0.9f), // Light green
            new Color(0.5f, 0.5f, 1f, 0.9f), // Light blue
            new Color(1f, 1f, 0.5f, 0.9f),   // Light yellow
            new Color(1f, 0.5f, 1f, 0.9f),   // Light magenta
            new Color(0.5f, 1f, 1f, 0.9f),   // Light cyan
            new Color(1f, 0.7f, 0.3f, 0.9f), // Orange
            new Color(0.7f, 0.3f, 1f, 0.9f), // Purple
            new Color(0.3f, 1f, 0.7f, 0.9f), // Teal
            new Color(1f, 0.3f, 0.7f, 0.9f), // Pink
        };
        
        return colors[index % colors.Length];
    }
    
    public void ClearAllClones()
    {
        // Destroy all clones
        foreach (GameObject clone in clones)
        {
            if (clone != null)
            {
                Destroy(clone);
            }
        }
        
        clones.Clear();
        allRecordedSegments.Clear();
        
        // Reset main player to spawn
        if (mainPlayer != null)
        {
            mainPlayer.transform.position = startPosition;
            mainPlayer.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
        
        // Start fresh recording
        StartRecording();
        
        Debug.Log("Cleared all clones and reset!");
    }
    
    void Update()
    {
        // Handle collision re-enabling
        if (collisionsDisabled && Time.time >= collisionEnableTime)
        {
            EnableCollisions();
        }
        // Keyboard shortcuts
        if (Input.GetKeyDown(KeyCode.R))
        {
            CreateClone();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearAllClones();
        }
        
        // Debug info
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log($"=== STATUS ===");
            Debug.Log($"Recorded segments: {allRecordedSegments.Count}");
            Debug.Log($"Active clones: {clones.Count}");
            Debug.Log($"Current segment inputs: {currentSegment.Count}");
        }
    }
}