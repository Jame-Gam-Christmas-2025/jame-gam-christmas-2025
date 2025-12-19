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
}

   