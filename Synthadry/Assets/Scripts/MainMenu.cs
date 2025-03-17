using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        ScenesManager.Instance.ChangeScene("TestPlayButtonScene");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
