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

    public void level2()
    {
        loadScene(3);
    }

    public void level3()
    {
        loadScene(4);
    }

    public void level4()
    {
        loadScene(5);
    }

    public void level5()
    {
        loadScene(6);
    }

    public void level6()
    {
        loadScene(7);
    }

    public void level7()
    {
        loadScene(8);
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
