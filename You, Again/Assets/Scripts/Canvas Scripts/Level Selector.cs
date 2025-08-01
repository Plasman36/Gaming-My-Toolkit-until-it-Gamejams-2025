using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public void GoBack()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void level1()
    {
        SceneManager.LoadScene(2);
    }

    public void level2()
    {
        SceneManager.LoadScene(3);
    }
}
