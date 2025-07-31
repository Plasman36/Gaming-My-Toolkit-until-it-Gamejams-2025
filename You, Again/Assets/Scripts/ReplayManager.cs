using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplayManager : MonoBehaviour
{    
    [Header("Level Settings")]
    public List<GameObject> interactables  = new List<GameObject>();
    private Dictionary<GameObject, GameObject> objectClones  = new Dictionary<GameObject, GameObject>();

    [Header("Player Setup")]
    public GameObject playerPrefab;
    public PlayerController mainPlayer;
    public CameraController cc;

    [Header("Spawn Settings")]
    public Transform spawnPoint;
    
    [Header("Layer Settings")]
    
    public List<List<InputFrame>> allRecordedSegments = new List<List<InputFrame>>();
    private List<InputFrame> currentSegment = new List<InputFrame>();
    public List<GameObject> clones = new List<GameObject>();
    public int aliveClonesLayer = 9; // Layer for players/clones
    public int deadClonesLayer = 8; // Layer for players/clones
    
    private float recordingStartTime;
    private Vector3 startPosition;
    private static bool layerCollisionsSetup = false;
    
    void Awake()
    {
        if (mainPlayer == null)
        {
            mainPlayer = FindObjectOfType<PlayerController>();
        }
        
        if (spawnPoint != null)
        {
            startPosition = spawnPoint.position;
        }
        else if (mainPlayer != null)
        {
            startPosition = mainPlayer.transform.position;
        }
        
        if (mainPlayer != null)
        {
            mainPlayer.gameObject.layer = aliveClonesLayer;
        }
        
        if (!layerCollisionsSetup)
        {
            Physics2D.IgnoreLayerCollision(aliveClonesLayer, aliveClonesLayer, true);
            Physics2D.IgnoreLayerCollision(deadClonesLayer, deadClonesLayer, false);
            Physics2D.IgnoreLayerCollision(aliveClonesLayer, deadClonesLayer, false);
            layerCollisionsSetup = true;
        }
        
        foreach (GameObject obj in interactables){
            if(obj == null){
                continue;
            }
            GameObject clone = Instantiate(obj);
            clone.SetActive(false);
            objectClones[obj] = clone; // Creat a copy of clones and store in dict
        }

        StartRecording();
    }
    
    void StartRecording()
    {
        currentSegment.Clear();
        recordingStartTime = Time.time;
        
        Debug.Log("Started recording new segment");
    }
    
    public void RecordInput(float horizontal, bool jumpPressed, Vector3 position, bool pickedUp, bool dropped, bool shot, bool flipped)
    {
        InputFrame frame = new InputFrame
        {
            timestamp = Time.time - recordingStartTime,
            horizontalInput = horizontal,
            jumpPressed = jumpPressed,
            position = position,
            pickedUp = pickedUp,
            dropped = dropped,
            shot = shot,
            flipped = flipped
        };
        
        currentSegment.Add(frame);
    }

    public void Death()
    {
        StartCoroutine(HandleDeathSequence());
    }

    private IEnumerator HandleDeathSequence()
    {
        if (currentSegment.Count == 0)
        {
            Debug.LogWarning("No inputs recorded yet!");
            yield break;
        }

        allRecordedSegments.Add(new List<InputFrame>(currentSegment));

        // animation step: freeze and shake
        if(mainPlayer.pickUpScript.holding){
            mainPlayer.pickUpScript.DropItDown();
        }
        mainPlayer.rb.linearVelocity = Vector2.zero;
        mainPlayer.rb.gravityScale = 0f;
        mainPlayer.rb.constraints = RigidbodyConstraints2D.FreezeAll;
        cc.startShaking = true;
        mainPlayer.isAlive = false;
        mainPlayer.gameObject.layer = deadClonesLayer;

        yield return new WaitForSeconds(1f); // pause frozen

        // restore gravity but still not resetting position
        mainPlayer.rb.gravityScale = 2f;
        mainPlayer.rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        yield return new WaitForSeconds(2f); // extra time before reset

        // now reset position and start clones
        if (mainPlayer != null)
        {
            mainPlayer.transform.position = startPosition;
            Rigidbody2D mainRb = mainPlayer.GetComponent<Rigidbody2D>();
            if (mainRb != null)
            {
                mainRb.linearVelocity = Vector2.zero;
            }
        }

        foreach (GameObject clone in clones)
        {
            if (clone != null)
            {
                Destroy(clone);
            }
        }
        clones.Clear();

        for (int i = 0; i < allRecordedSegments.Count; i++)
        {
            GameObject clone = Instantiate(playerPrefab, startPosition, Quaternion.identity);
            clone.layer = aliveClonesLayer;

            SpriteRenderer cloneRenderer = clone.GetComponent<SpriteRenderer>();
            if (cloneRenderer != null)
            {
                cloneRenderer.color = GetCloneColor(i);
            }

            clone.name = $"Clone_{i + 1}";

            PlayerController cloneController = clone.GetComponent<PlayerController>();
            if (cloneController != null)
            {
                cloneController.StartReplayingInputs(allRecordedSegments[i]);
            }

            clones.Add(clone);
        }

        for (int i = 0; i < interactables.Count; i++)
        {
            GameObject original = interactables[i];
            objectClones[original].SetActive(true);

            interactables[i] = objectClones[original];

            if (original != null)
            {
                Destroy(original);
            }
        }

        foreach (GameObject obj in interactables){
            if(obj == null){
                continue;
            }
            GameObject clone = Instantiate(obj);
            clone.SetActive(false);
            objectClones[obj] = clone; // Creat a copy of clones and store in dict
        }

        StartRecording();
        mainPlayer.isAlive = true;
        mainPlayer.gameObject.layer = aliveClonesLayer;
        mainPlayer.isFacingRight = true;
        mainPlayer.gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

        Debug.Log($"Created Clone {allRecordedSegments.Count}! Total clones: {clones.Count}");
        Debug.Log("All players reset to start position with collisions temporarily disabled!");
    }


    Color GetCloneColor(int index)
    {
        Color[] colors = {
            new Color(1f, 0.5f, 0.5f, 0.9f), // Light red
            new Color(1f, 0.7f, 0.3f, 0.9f), // Orange
            new Color(1f, 1f, 0.5f, 0.9f),   // Light yellow
            new Color(0.5f, 1f, 0.5f, 0.9f), // Light green
            new Color(0.5f, 0.5f, 1f, 0.9f), // Light blue
            new Color(1f, 0.5f, 1f, 0.9f),   // Light magenta
            new Color(0.7f, 0.3f, 1f, 0.9f), // Purple
            new Color(0.3f, 1f, 0.7f, 0.9f), // Teal
            new Color(1f, 0.3f, 0.7f, 0.9f), // Pink
        };
        
        return colors[index % colors.Length];
    }
    
    public void ClearAllClones()
    {
        foreach (GameObject clone in clones)
        {
            if (clone != null)
            {
                Destroy(clone);
            }
        }

        clones.Clear();
        allRecordedSegments.Clear();

        if (mainPlayer != null)
        {
            mainPlayer.transform.position = startPosition;
            mainPlayer.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        StartRecording();

        Debug.Log("Cleared all clones and reset!");
    }
    
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log($"=== STATUS ===");
            Debug.Log($"Recorded segments: {allRecordedSegments.Count}");
            Debug.Log($"Active clones: {clones.Count}");
            Debug.Log($"Current segment inputs: {currentSegment.Count}");
        }
    }
}