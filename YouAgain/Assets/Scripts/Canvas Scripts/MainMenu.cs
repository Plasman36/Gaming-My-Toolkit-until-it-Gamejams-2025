using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public Animator transition;
    public float timeBetween;
    public void PlayGame()
    {
        loadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
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
}
