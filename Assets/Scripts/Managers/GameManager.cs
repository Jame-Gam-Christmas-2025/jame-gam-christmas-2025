using UnityEngine;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

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

    public GameObject playerGameObject;

    [SerializeField] private bool lockCursor = true;

    private string _currentBossName ="YuleCat";
    private List<string> _defeatedBossNames = new();

    private Vector3 _lastCheckpointPosition;

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

        // Reset Scene States

        GameSceneManager.Instance.ReloadScene();
    }

    // Called in SceneManager to be applied on right time
    public void ApplyLastCheckpoint()
    {
        // Get new player instance
        if(playerGameObject == null)
        {
            playerGameObject = GameObject.Find("Player");
        }

        // Place Player to last checkpoint position
        if(_lastCheckpointPosition != null)
        {
            playerGameObject.transform.position = _lastCheckpointPosition;
        } else
        {
            _lastCheckpointPosition = playerGameObject.transform.position;
        }
    }

    public void SpawnBoss(string bossName, Vector3 checkpointPosition)
    {
        _currentBossName = bossName;
        _lastCheckpointPosition = checkpointPosition;
    }

    public void DefeatBoss(string bossName)
    {
        _defeatedBossNames.Add(bossName);

        // Destroy the current boss altar
        GameObject bellAltar = GameObject.FindWithTag(bossName);
        if(bellAltar != null)
        {
            Destroy(bellAltar);
        }
    }

    public void ToggleArenaMode()
    {
        
    }

    public void PostProcessing()
    {
        
    }
}
