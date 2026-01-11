using UnityEngine;
using AK.Wwise;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("RTPC Names (match Wwise exactly)")]
    [SerializeField] private string masterVolumeRTPC = "RTPC_VolumeMaster";
    [SerializeField] private string musicVolumeRTPC = "RTPC_VolumeMUS";
    [SerializeField] private string sfxVolumeRTPC = "RTPC_VolumeSFX";

    [Header("Test - Remove in production")]
    [SerializeField] private bool showDebugLogs = true;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize with default values (50 = mid-range if your RTPC is 0-100)
        SetMasterVolume(100f);
        SetMusicVolume(100f);
        SetSFXVolume(100f);

        if (showDebugLogs)
        {
            Debug.Log("AudioManager initialized with default volumes at 100");
        }
    }

    void Start()
    {
        /* // Initialize with default values (50 = mid-range if your RTPC is 0-100)
        SetMasterVolume(80f);
        SetMusicVolume(80f);
        SetSFXVolume(80f);

        if (showDebugLogs)
        {
            Debug.Log("AudioManager initialized with default volumes at 80");
        } */

        #if UNITY_EDITOR
        InitSceneAudio(GameSceneManager.Instance.currentScene);
        #endif
    }
    
    /// Set Master Volume (0-100 range recommended)
    public void SetMasterVolume(float value)
    {
        AKRESULT result = AkSoundEngine.SetRTPCValue(masterVolumeRTPC, value);
        
        if (showDebugLogs)
        {
            Debug.Log($"Master Volume set to: {value} | Result: {result}");
        }
    }

   
    /// Set Music Volume (0-100 range recommended)
  
    public void SetMusicVolume(float value)
    {
        AKRESULT result = AkSoundEngine.SetRTPCValue(musicVolumeRTPC, value);
        
        if (showDebugLogs)
        {
            Debug.Log($"Music Volume set to: {value} | Result: {result}");
        }
    }

    
    /// Set SFX Volume (0-100 range recommended)
   
    public void SetSFXVolume(float value)
    {
        AKRESULT result = AkSoundEngine.SetRTPCValue(sfxVolumeRTPC, value);
        
        if (showDebugLogs)
        {
            Debug.Log($"SFX Volume set to: {value} | Result: {result}");
        }
    }


    /// Test method - Call this to verify RTPCs work
   
    [ContextMenu("Test All Volumes")]
    public void TestAllVolumes()
    {
        Debug.Log("=== Testing RTPCs ===");
        SetMasterVolume(100f);
        SetMusicVolume(75f);
        SetSFXVolume(50f);
        Debug.Log("=== Test Complete - Check Wwise Profiler ===");
    }
    
    
    ///// Event Musiques //////
    
    [Header("Music Events")]
    public AK.Wwise.Event PlayKrampusMusic;
    public AK.Wwise.Event StopKrampusMusic;
    public AK.Wwise.Event PlayNamahageMusic;
    public AK.Wwise.Event StopNamahageMusic;
    public AK.Wwise.Event PlayYuleMusic;
    public AK.Wwise.Event StopYuleMusic;
    public AK.Wwise.Event PlaySantaMusic;
    public AK.Wwise.Event StopSantaMusic;
    public AK.Wwise.Event PlayBadEndingMusic;

    public AK.Wwise.Event StopBadEndingMusic;
    public AK.Wwise.Event PlayGoodEndingMusic;
    public AK.Wwise.Event StopGoodEndingMusic;
    public AK.Wwise.Event PlayMenuMusic;
    public AK.Wwise.Event StopMenuMusic;
    public AK.Wwise.Event PlayExplorationMusic;
    public AK.Wwise.Event StopExplorationMusic;
    
    public AK.Wwise.Event PlayBellAltar;

    public void PlayBellSFX(GameObject gameObject)
    {
        PlayBellAltar.Post(gameObject);
    }
    
    public void PlayNamahageMUS(GameObject gameObject)
    {
        PlayNamahageMusic.Post(gameObject);
    }
    
    public void StopNamahageMUS(GameObject gameObject)
    {
        StopNamahageMusic.Post(gameObject);
    }
    
    public void PlayKrampusMUS(GameObject gameObject)
    {
        PlayKrampusMusic.Post(gameObject);
    }
    
    public void StopKrampusMUS(GameObject gameObject)
    {
        StopKrampusMusic.Post(gameObject);
    }
    
    public void PlayYuleMUS(GameObject gameObject)
    {
        PlayYuleMusic.Post(gameObject);
    }
    
    public void StopYuleMUS(GameObject gameObject)
    {
        StopYuleMusic.Post(gameObject);
    }

    public void PlaySantaMUS(GameObject gameObject)
    {
        PlaySantaMusic.Post(gameObject);
    }

    public void StopSantsMUS(GameObject gameObject)
    {
        StopSantaMusic.Post(gameObject);
    }

    public void PlayGoodEndingMUS(GameObject gameObject)
    {
        PlayGoodEndingMusic.Post(gameObject);
    }
    
    public void StopGoodEndingMUS(GameObject gameObject)
    {
        StopGoodEndingMusic.Post(gameObject);
    }
    
    public void PlayBadEndingMUS(GameObject gameObject)
    {
        PlayBadEndingMusic.Post(gameObject);
    }
    
    public void StopBadEndingMUS(GameObject gameObject)
    {
        StopBadEndingMusic.Post(gameObject);
    }
    
    public void PlayMenuMUS(GameObject gameObject)
    {
        PlayMenuMusic.Post(gameObject);
    }
    
    public void StopMenuMUS(GameObject gameObject)
    {
        StopMenuMusic.Post(gameObject);
    }

    public void PlayExplorationMUS(GameObject gameObject)
    {
        PlayExplorationMusic.Post(gameObject);
    }
    
    public void StopExplorationMUS(GameObject gameObject)
    {
        StopExplorationMusic.Post(gameObject);
    }
    
    // Boss Music Handler

    public void PlayBossMusic(string bossName)
    {
        switch(bossName)
        {
            case "YuleCat":
                PlayYuleMUS(gameObject);
                break;

            case "Namahage":
                PlayNamahageMUS(gameObject);
                break;

            case "Krampus":
                PlayKrampusMUS(gameObject);
                break;

            case "Santa":
                PlaySantaMUS(gameObject);
                break;
            default:
            break;
        }

        StopExplorationMUS(gameObject);
    }

    public void StopBossMusic(string bossName)
    {
        switch(bossName)
        {
            case "YuleCat":
                StopYuleMUS(gameObject);
                break;

            case "Namahage":
                StopNamahageMUS(gameObject);
                break;

            case "Krampus":
                StopKrampusMUS(gameObject);
                break;

            case "Santa":
                StopSantsMUS(gameObject);
                break;
            default:
            break;
        }

        PlayExplorationMUS(gameObject);
    }

    public void StopAllBossMusic()
    {
        StopYuleMUS(gameObject);
        StopNamahageMUS(gameObject);
        StopKrampusMUS(gameObject);
        StopSantsMUS(gameObject);
    }



    private uint _menuMusicId;

    public void InitSceneAudio(string sceneName)
    {
        Debug.Log("Init scene audio : " + sceneName);
        switch(sceneName)
        {
            case "MainMenuScene":
                Debug.Log("main");
                if(_menuMusicId == 0)
                {
                    _menuMusicId = PlayMenuMusic.Post(gameObject);
                }
                StopExplorationMUS(gameObject);
                StopAllBossMusic();
                StopBadEndingMUS(gameObject);
                StopGoodEndingMUS(gameObject);
                break;

            case "leveldesign":
                StopMenuMUS(gameObject);
                _menuMusicId = 0;
                PlayExplorationMUS(gameObject);
                break;

            case "GoodEndingScene":
                StopExplorationMUS(gameObject);
                PlayGoodEndingMUS(gameObject);
                break;

            case "BadEndingScene":
                StopExplorationMUS(gameObject);
                PlayBadEndingMUS(gameObject);
                break;

            default:
                break;
        }
    }
}