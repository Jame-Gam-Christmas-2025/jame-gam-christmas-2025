using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    public GameObject gameManagerComponent;
    public GameObject gameSceneManagerComponent;
    public GameObject audioManagerComponent;
    private void Awake() {
        InitializeManagers();
        LoadMainScene();
    }

    private void InitializeManagers() {
        if(gameManagerComponent != null)
        {
            Object.Instantiate(gameManagerComponent);
        }

        if(gameSceneManagerComponent != null)
        {
            Object.Instantiate(gameSceneManagerComponent);
        }

        if(audioManagerComponent != null)
        {
            Object.Instantiate(audioManagerComponent);
        }
    }

    private void LoadMainScene() {
        SceneManager.LoadScene("MainMenuScene");
    }
}
