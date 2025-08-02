using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelSelector : MonoBehaviour
{
    public Animator transition;
    public float timeBetween;
    public void GoBack()
    {
        loadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void level1()
    {
        loadScene(2);
    }
    void loadScene(int scene)
    {
        StartCoroutine(transit(scene));
    }
    IEnumerator transit(int scene)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(timeBetween);
        SceneManager.LoadScene(scene);
    }

}
