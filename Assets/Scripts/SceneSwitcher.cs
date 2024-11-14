using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    static int numScenes;
    static int currentScene;
    private void Awake()
    {
        numScenes = SceneManager.sceneCount;
        currentScene = 0;
    }
    public void NextScene()
    {
        currentScene++;
        currentScene %= numScenes;
        SceneManager.LoadScene(currentScene);
    }
}
