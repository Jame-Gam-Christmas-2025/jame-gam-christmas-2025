using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager s_Instance;

    public Language Language { get; private set; } = Language.English;

    private void Awake()
    {
        if (s_Instance == null)
            s_Instance = this;
    }

    public void SetLanguage(Language language)
    {
        Language = language;
    }
}
