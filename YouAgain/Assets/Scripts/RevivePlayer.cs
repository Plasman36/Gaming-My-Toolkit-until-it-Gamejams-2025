using UnityEngine;

public class RevivePlayer : MonoBehaviour
{
    [Header("Revive Pad Settings")]
    private LayerMask playerLayers = 1 << 9;
    public float timeOnPad = 0f;
    public int reviveTime = 1;
    
    
    void OnTriggerStay2D(Collider2D collision){
        if(IsPlayerLayer(collision.gameObject.layer)){
            if(timeOnPad >= reviveTime){
                PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
                Revive(playerController);
            }
            timeOnPad += Time.deltaTime;
        }
    }

    void OnTriggerExit2D(){
        timeOnPad = 0;
    }

    private bool IsPlayerLayer(int layer)
    {
        return (playerLayers.value & (1 << layer)) != 0;
    }

    void Revive(PlayerController playerController){
        if (playerController.IsMainPlayer())
        {
            ReplayManager manager = FindObjectOfType<ReplayManager>();
            if (manager != null)
            {
                Debug.Log("Main player used a revive pad! Creating clone and resetting...");
                manager.Revive();
            }
        }else{
            Debug.Log($"{playerController.name} used a revive pad");
            playerController.SetDead();
        }
    }
}
