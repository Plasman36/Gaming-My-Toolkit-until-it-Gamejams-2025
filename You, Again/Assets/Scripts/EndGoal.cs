using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGoal : MonoBehaviour
{
    public GameObject winScreen;
    public bool hasWon;

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
        hasWon = true;
        winScreen.SetActive(true);
    }
}
