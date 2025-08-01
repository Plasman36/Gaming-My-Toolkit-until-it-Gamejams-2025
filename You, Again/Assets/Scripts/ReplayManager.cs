using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplayManager : MonoBehaviour
{    
    [Header("Level Settings")]
    public List<GameObject> interactables  = new List<GameObject>();
    private List<GameObject> objectClones  = new List<GameObject>();
    public List<GameObject> revivedObjects = new List<GameObject>();

    [Header("Player Setup")]
    public GameObject playerPrefab;
    public PlayerController mainPlayer;
    public CameraController cc;
    private GameObject cloneOfPlayer; // Stores most recent copy of player to add to clones

    [Header("Spawn Settings")]
    public Transform spawnPoint;
    
    [Header("Layer Settings")]
    
    public List<List<InputFrame>> allRecordedSegments = new List<List<InputFrame>>();
    private List<InputFrame> currentSegment = new List<InputFrame>();
    public List<GameObject> clones = new List<GameObject>();
    public List<GameObject> cloneClones /*Not confusing at all*/  = new List<GameObject>();
    public int aliveClonesLayer = 9; // Layer for players/clones
    public int deadClonesLayer = 8; // Layer for players/clones
    
    private float recordingStartTime;
    private Vector3 startPosition;
    private static bool layerCollisionsSetup = false;

    private bool restarting = false;

    
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

        cloneOfPlayer = Instantiate(mainPlayer.gameObject); // Creating copy of player in awake ig
        cloneOfPlayer.SetActive(false);
        
        foreach (GameObject obj in interactables){
            if(obj == null){
                continue;
            }
            GameObject clone = Instantiate(obj);
            clone.SetActive(false);
            objectClones.Add(clone); // Creat a copy of clones and store in dict
        }

        StartRecording();
    }
    
    void StartRecording()
    {
        currentSegment.Clear();
        recordingStartTime = Time.time;
        if(mainPlayer.pickUpScript != null){
            if(mainPlayer.pickUpScript.holding){
                revivedObjects.Add(mainPlayer.pickUpScript.heldObject); // Add currently held object
            }
        }
        Debug.Log("Started recording new segment");
    }
    
    public void RecordInput(float horizontal, bool jumpPressed, Vector3 position, bool pickedUp, bool dropped, bool shot, bool flipped, float time)
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
            flipped = flipped,
            DeltaTime = time,
        };
        
        currentSegment.Add(frame);
    }

    public void Death()
    {
        FindAnyObjectByType<PlaySFX>().playSFX("hit");
        StartCoroutine(HandleDeathSequence());
    }

    public void Revive()
    {
        FindAnyObjectByType<PlaySFX>().playSFX("hit");
        StartCoroutine(HandleDeathSequence(true));
    }

    public void Restart() // Resets current loop without creating a new clone or changing player state
    {
        FindAnyObjectByType<PlaySFX>().playSFX("hit");
        StartCoroutine(HandleDeathSequence(false, false));
    }

    private IEnumerator HandleDeathSequence(bool keepHeldObject = false, bool addClone = true)
    {
        restarting = true;
        if (currentSegment.Count == 0)
        {
            Debug.LogWarning("No inputs recorded yet!");
            yield break;
        }
        
        if(addClone){
            allRecordedSegments.Add(new List<InputFrame>(currentSegment));
        }
        
        // animation step: freeze and shake
        if(mainPlayer.pickUpScript.holding && !keepHeldObject){
            mainPlayer.pickUpScript.DropItDown();
        }
        mainPlayer.rb.linearVelocity = Vector2.zero;
        mainPlayer.rb.gravityScale = 0f;
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

        // Delete all original clones
        foreach(GameObject clone in clones){
            Destroy(clone);
        }

        // Move all cloned clones into original clones list
        clones.Clear();
        clones.AddRange(cloneClones);
        cloneClones.Clear();

        if(addClone){
            // Move copy of OG player to clones list, create new clone of the player
            clones.Add(cloneOfPlayer);
        }else{
            // Destroy original player, make clone the new mainPlayer
            Destroy(mainPlayer.gameObject);
            cloneOfPlayer.SetActive(true);
            mainPlayer = cloneOfPlayer.GetComponent<PlayerController>();
        }

        cloneOfPlayer = Instantiate(mainPlayer.gameObject);
        cloneOfPlayer.SetActive(false);

        // Create a new set of copies of clones
        foreach(GameObject clone in clones){
            GameObject newClone = Instantiate(clone);
            newClone.SetActive(false);
            cloneClones.Add(newClone);
        }
        
        
        // foreach (GameObject obj in revivedObjects)
        // {
        //     obj.SetActive(false);
        // }

        foreach (GameObject revObj in revivedObjects)
        {
            if (mainPlayer.pickUpScript.heldObject != revObj)
            {
                Destroy(revObj);
            }
        }

        // Set up all clones
        for (int i = 0; i < allRecordedSegments.Count; i++)
        {
            GameObject clone = clones[i];

            Transform AnimationHandler = clone.gameObject.transform.Find("AnimationsHandler");
            if (AnimationHandler != null)
            {
                Debug.Log("Found");
                SpriteRenderer SR = AnimationHandler.GetComponent<SpriteRenderer>();
                SR.color = GetCloneColor(i);
            }

            clone.name = $"Clone_{i + 1}";

            PlayerController cloneController = clone.GetComponent<PlayerController>();
            if (cloneController != null)
            {
                cloneController.Reset();
                cloneController.StartReplayingInputs(allRecordedSegments[i]);
            }

            clone.SetActive(true);
        }

        for (int i = 0; i < interactables.Count; i++)
        {
            GameObject original = interactables[i];
            if(original == null){
                continue;
            }
            objectClones[i].SetActive(true);

            interactables[i] = objectClones[i];

            if(!keepHeldObject){
                Destroy(original);
            }else{
                if(mainPlayer.pickUpScript.heldObject != original){
                    Destroy(original);
                }
            }
        }

        for(int i = 0; i< interactables.Count; i++){
            GameObject obj = interactables[i];
            if(obj == null){
                continue;
            }
            GameObject clone = Instantiate(obj);
            clone.name = $"{obj.name}";
            clone.SetActive(false);
            objectClones[i] = clone; // Creat a copy of clones and store in dict
        }

        StartRecording();
        PlayerController PC = mainPlayer.GetComponent<PlayerController>();
        PC.Reset();

        Debug.Log($"Created Clone {allRecordedSegments.Count}! Total clones: {clones.Count}");
        Debug.Log("All players reset to start position with collisions temporarily disabled!");
        restarting = false;
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
    
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log($"=== STATUS ===");
            Debug.Log($"Recorded segments: {allRecordedSegments.Count}");
            Debug.Log($"Active clones: {clones.Count}");
            Debug.Log($"Current segment inputs: {currentSegment.Count}");
        }

        if(Input.GetKeyDown(KeyCode.R) && !restarting){
            restarting = true;
            Restart();
            
        }
    }
}