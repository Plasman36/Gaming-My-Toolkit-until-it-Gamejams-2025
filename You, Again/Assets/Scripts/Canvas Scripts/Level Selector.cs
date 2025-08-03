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

    public void level8()
    {
        loadScene(9);
    }

    public void level9()
    {
        loadScene(10);
    }

    public void level10()
    {
        loadScene(11);
    }

    public void level11()
    {
        loadScene(12);
    }

    public void level12()
    {
        loadScene(13);
    }

    public void level13()
    {
        loadScene(14);
    }

    public void level14()
    {
        loadScene(15);
    }

    public void level15()
    {
        loadScene(16);
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
