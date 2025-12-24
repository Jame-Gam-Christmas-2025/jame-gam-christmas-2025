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
    }

    void Start()
    {
        // Initialize with default values (50 = mid-range if your RTPC is 0-100)
        SetMasterVolume(50f);
        SetMusicVolume(50f);
        SetSFXVolume(50f);

        if (showDebugLogs)
        {
            Debug.Log("AudioManager initialized with default volumes at 50");
        }
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
    public AK.Wwise.Event PlayBadEndingMusic;

    public AK.Wwise.Event StopBadEndingMusic;
    public AK.Wwise.Event PlayGoodEndingMusic;
    public AK.Wwise.Event StopGoodEndingMusic;
    public AK.Wwise.Event PlayMenuMusic;
    public AK.Wwise.Event StopMenuMusic;
    
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
    
    // Boss Music Handler

    public void PlayBossMusic(string bossName)
    {
        switch(bossName)
        {
            case "YuleCat":
                PlayYuleMUS(gameObject);
                break;

            case "Namahage":
                PlayYuleMUS(gameObject);
                break;

            case "Krampus":
                PlayYuleMUS(gameObject);
                break;

            case "Santa":
                PlayYuleMUS(gameObject);
                break;
            default:
            break;
        }
    }
    ////////// Event Boss /////////
}


   