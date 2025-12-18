#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private bool lockCursor = true;

    public Language Language { get; private set; } = Language.English;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        switch (lockCursor)
        {
            case true:
                Cursor.lockState = CursorLockMode.Locked;
                break;
            case false:
                Cursor.lockState = CursorLockMode.None;
                break;
        }
    }

    public void SetLanguage(Language language)
    {
        Language = language;
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
