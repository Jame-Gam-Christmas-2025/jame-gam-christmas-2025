using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static SaveManager _instance;
    public static SaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject gameObject = new GameObject("SaveManager");
                _instance = gameObject.AddComponent<SaveManager>();
                DontDestroyOnLoad(gameObject);
            }
            return _instance;
        }
    }

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

    private void Start()
    {
        // Load the game
    }
    
    public void SaveGame()
    {
        
    }
}
