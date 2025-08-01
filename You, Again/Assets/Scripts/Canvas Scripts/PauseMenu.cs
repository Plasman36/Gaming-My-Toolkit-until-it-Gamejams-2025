using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;

    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI; // assign this in the inspector

    public EndGoal goal;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !goal.hasWon)
        {
            if (optionsMenuUI.activeSelf)
            {
                // If we're in the options menu, go back to pause menu
                optionsMenuUI.SetActive(false);
                pauseMenuUI.SetActive(true);
                return;
            }

            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void QuitLevel()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(1);
    }

    public void OpenOptions()
    {
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
    }
}
