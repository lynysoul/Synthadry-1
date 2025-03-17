using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;
    void Start()
    {
        Instance = this;
    }

    public void ChangeScene(string scene)
    {
        switch(scene)
        {
            case "MainMenu":
                HandleMainMenuScene();
                break;
            case "TestPlayButtonScene":
                HandleLevelScene(scene);
                break;
        }
    }

    private void HandleMainMenuScene()
    {
        if (SceneManager.GetSceneByName("HUD").IsValid())
            SceneManager.UnloadSceneAsync("HUD");
        SceneManager.LoadScene("MainMenu");
    }

    private void HandleLevelScene(string level)
    {
        SceneManager.LoadSceneAsync(level);
        if (!SceneManager.GetSceneByName("HUD").IsValid())
            SceneManager.LoadSceneAsync("HUD", LoadSceneMode.Additive);
    }
}
