#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject gameObject = new GameObject("GameManager");
                _instance = gameObject.AddComponent<GameManager>();
                DontDestroyOnLoad(gameObject);
            }
            return _instance;
        }
    }

    [SerializeField] private bool lockCursor = true;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /* switch (lockCursor)
        {
            case true:
                Cursor.lockState = CursorLockMode.Locked;
                break;
            case false:
                Cursor.lockState = CursorLockMode.None;
                break;
        } */
    }

    private void Start()
    {
        // Load the game
    }
    public void QuitApplication()
    {
        #if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
        #else
                Application.Quit();
        #endif
    }

    public void GameOver()
    {
        // Notify Player

        // Reload Scene

        // Reset Player States, then use checkpoints location

        // Reset Scene States

        GameSceneManager.Instance.ReloadScene();
    }

    public void SaveGame()
    {
        
    }

    public void ToggleArenaMode()
    {
        
    }

    public void PostProcessing()
    {
        
    }
}
