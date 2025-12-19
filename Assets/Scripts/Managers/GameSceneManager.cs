using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
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

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public List<string> GetAllBuiltSceneNames()
    {
        List<string> allBuiltSceneNames = new();

        // Index starts at 2 because we want to ignore 2 firsts scenes in build settings (Bootstrap and SceneSelectionScene)
        for (int i = 2; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string currentScenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string currentSceneName = currentScenePath.Split("/").Last().Split(".")[0];
            allBuiltSceneNames.Add(currentSceneName);
        }

        return allBuiltSceneNames;
    }
}
