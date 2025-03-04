using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadSceneAsync("TestPlayButtonScene");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
