using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinScreen : MonoBehaviour
{
    public Animator transition;
    public float timeBetween;
    public void NextGame()
    {
        loadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Restart()
    {
        loadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitLevel()
    {
        loadScene(1);
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
