using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    private static GameSceneManager _instance;
    public static GameSceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject gameObject = new GameObject("GameSceneManager");
                _instance = gameObject.AddComponent<GameSceneManager>();
                DontDestroyOnLoad(gameObject);
            }
            return _instance;
        }
    }

    private string _currentScene;

    private bool _isMenuOpen = false;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Apply game logic
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Feature-PlayerState-Combat")
        {
            GameManager.Instance.ApplyLastCheckpoint();
        }
    }

    public void LoadSceneByName(string sceneName)
    {
        // Handle Controls
        InputManager.Instance.UpdateSubscription(IsControlRequiredBySceneName(sceneName));

        // Handle Views
        UIManager.Instance.CloseAllViews();

        // Load scene
        SceneManager.LoadScene(sceneName);
        _currentScene = sceneName;
    }

    public void ReloadScene()
    {
        #if UNITY_EDITOR
            _currentScene = SceneManager.GetActiveScene().name;
        #endif

        SceneManager.LoadScene(_currentScene);
    }

    public List<string> GetAllBuiltSceneNames()
    {
        List<string> allBuiltSceneNames = new();

        // Index starts at 2 because we want to ignore 2 firsts scenes in build settings (Bootstrap and SceneSelectionScene)
        for (int i = 4; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string currentScenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string currentSceneName = currentScenePath.Split("/").Last().Split(".")[0];
            allBuiltSceneNames.Add(currentSceneName);
        }

        return allBuiltSceneNames;
    }

    private bool IsControlRequiredBySceneName(string sceneName)
    {
        string[] inputDisabledSceneNames = {"BootstrapScene","MainMenuScene", /* "SceneSelectionScene", "SettingsScene" */};
        return !inputDisabledSceneNames.Contains(sceneName);
    }
}
