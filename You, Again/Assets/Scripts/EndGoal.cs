using UnityEngine;

public class EndGoal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D maybePlayer)
    {
        int otherLayer = maybePlayer.gameObject.layer;

        if (otherLayer == 8 || otherLayer == 9)
        {
            Win();
        }
    }

    private void Win()
    {
        Debug.Log("congrats you won");
        //go to next scene or smth here
        
    }
}
