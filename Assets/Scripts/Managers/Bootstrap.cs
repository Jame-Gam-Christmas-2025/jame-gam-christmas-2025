using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    public GameObject gameManagerComponent;
    public GameObject gameSceneManagerComponent;
    public GameObject audioManagerComponent;
    public GameObject inputManagerComponent;
    private void Awake() {
        InitializeManagers();
    }

    void Start()
    {
        LoadMainScene();
    }

    private void InitializeManagers() {
        if(gameManagerComponent != null)
        {
            Instantiate(gameManagerComponent);
        }

        if(gameSceneManagerComponent != null)
        {
            Instantiate(gameSceneManagerComponent);
        }

        if(audioManagerComponent != null)
        {
            Instantiate(audioManagerComponent);
        }

        if(inputManagerComponent != null)
        {
            Instantiate(inputManagerComponent);
        }
    }

    private void LoadMainScene() {
        SceneManager.LoadScene("MainMenuScene");
    }
}
