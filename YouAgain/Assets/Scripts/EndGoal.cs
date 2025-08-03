using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGoal : MonoBehaviour
{
    public GameObject winScreen;
    public bool hasWon;
    public Animator animator;

    [Header("Glitch Settings")]
    public float glitchMinInterval = 3f;
    public float glitchMaxInterval = 8f;

    private void Start()
    {
        // Start the glitching cycle
        ScheduleNextGlitch();
    }

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
        if (!hasWon)
        {
            FindAnyObjectByType<PlaySFX>().playSFX("win");
        }

        animator.SetBool("hasWon", true);
        hasWon = true;
        winScreen.SetActive(true);
    }

    // Randomly called glitch trigger
    private void ScheduleNextGlitch()
    {
        float nextGlitchIn = Random.Range(glitchMinInterval, glitchMaxInterval);
        Invoke(nameof(StartGlitch), nextGlitchIn);
    }

    private void StartGlitch()
    {
        animator.SetBool("isGlitching", true);
        // Wait for animation event to reset isGlitching
        // Do NOT schedule next glitch here, it’ll be done after resetting
    }

    // This will be called by Animation Event at end of glitch animation
    public void EndGlitch()
    {
        animator.SetBool("isGlitching", false);
        // Schedule the next glitch after this one ends
        ScheduleNextGlitch();
    }
}
