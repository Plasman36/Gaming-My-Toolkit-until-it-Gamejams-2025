using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public Animator transition;
    public float timeBetween;

    public static bool isPaused = false;

    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI; // assign this in the inspector

    EndGoal goal;

    private void Start()
    {
        goal = FindAnyObjectByType<EndGoal>();
        GameObject.FindGameObjectWithTag("Music").GetComponent<MusicClass>().PlayMusic();
    }

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

    public void Restart()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        loadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void loadScene(int scene)
    {
        Debug.Log("Started coroutine");
        StartCoroutine(transit(scene));
    }
    IEnumerator transit(int scene)
    {
        transition.SetTrigger("Start");
        Debug.Log("Triggering Animation");
        yield return new WaitForSeconds(timeBetween);
        SceneManager.LoadScene(scene);
    }

    public void QuitLevel()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        loadScene(1);
    }

    public void OpenOptions()
    {
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
    }
}
