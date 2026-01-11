using UnityEngine;
using System.Collections.Generic;
using System.Linq;



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

    private string _currentBossName ="Yule Cat";
    private List<string> _defeatedBossNames = new();

    private Vector3? _lastCheckpointPosition;

    private int _currentPlayerAlignmentScore;

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

        AudioManager.Instance.StopBossMusic(_currentBossName);
        GameSceneManager.Instance.ReloadScene();
    }

    // Called in SceneManager to be applied on right time
    public void ApplyLastCheckpoint()
    {
        playerGameObject = GameObject.Find("Player");

        if(playerGameObject != null)
        {
            // Place Player to last checkpoint position
            if (_lastCheckpointPosition != null)
            {
                playerGameObject.transform.position = _lastCheckpointPosition.Value;
            }
        }
    }

    public void ApplyPlayerAlignmentScore()
    {
        playerGameObject = GameObject.Find("Player");

        if(playerGameObject != null)
        {
            PlayerAlignment playerAlignment = playerGameObject.GetComponent<PlayerAlignment>();

            if(playerAlignment != null)
            {
                playerAlignment.AlignmentScore = _currentPlayerAlignmentScore;
            }
        }
    }

    public void SpawnBoss(string bossName, Vector3 checkpointPosition)
    {
        _currentBossName = bossName;
        _lastCheckpointPosition = checkpointPosition;

        AudioManager.Instance.PlayBossMusic(bossName);

        GameObject bossGameObject = GameObject.Find(bossName);

        if(bossGameObject != null)
        {
            BossController bossController = bossGameObject.GetComponent<BossController>();

            bossController.IsActive = true;
        }

        playerGameObject = GameObject.Find("Player");

        if(playerGameObject != null)
        {
            PlayerAlignment playerAlignment = playerGameObject.GetComponent<PlayerAlignment>();

            if(playerAlignment != null)
            {
                _currentPlayerAlignmentScore = playerAlignment.AlignmentScore;
            }
        }
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

        AudioManager.Instance.StopBossMusic(bossName);
    }

    public void EndGame()
    {
        string lastDefeatedBoss = _defeatedBossNames.Last();
        if(lastDefeatedBoss == "Santa")
        {
            playerGameObject = GameObject.Find("Player");

            if(playerGameObject != null)
            {
                PlayerAlignment playerAlignment = playerGameObject.GetComponent<PlayerAlignment>();
                if(playerAlignment.AlignmentScore > 0)
                {
                    GameSceneManager.Instance.LoadSceneByName("GoodEndingScene");
                } else
                {
                    GameSceneManager.Instance.LoadSceneByName("BadEndingScene");
                }

                _currentPlayerAlignmentScore = 0;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            
        }
    }

    public string LastDefeatedBoss()
    {
        return _defeatedBossNames.Last();
    }

    public void ToggleArenaMode()
    {
        
    }

    public void PostProcessing()
    {
        
    }
}
