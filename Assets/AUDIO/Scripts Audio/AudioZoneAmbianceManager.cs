using UnityEngine;

/// <summary>
/// Manages ambient sound transitions between zones using Wwise RTPC
/// Enhanced version with smooth crossfades and interpolation curves
/// </summary>
public class ZoneAmbianceManager : MonoBehaviour
{
    public static ZoneAmbianceManager Instance;

    [Header("Wwise Events")]
    [SerializeField] private AK.Wwise.Event playAmbianceEvent;
    [SerializeField] private AK.Wwise.Event stopAmbianceEvent;

    [Header("RTPC Settings")]
    [SerializeField] private string zoneRTPCName = "RTPC_PAD_AMB_Transition";

    [Header("Crossfade Settings")]
    [SerializeField] private float defaultCrossfadeDuration = 2f;
    [Tooltip("Time in seconds to blend between zones")]
    
    [SerializeField] private AnimationCurve crossfadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [Tooltip("Shape of the transition: Linear, EaseIn, EaseOut, or custom")]
    
    [SerializeField] private float hysteresisDelay = 0.3f;
    [Tooltip("Delay before accepting a new zone change (prevents rapid flickering)")]

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    [SerializeField] private bool showDebugUI = true;

    // Internal state
    private float currentZoneID = 0f;
    private float targetZoneID = 0f;
    private float crossfadeProgress = 0f; // 0 to 1
    private float crossfadeDuration = 2f;
    private bool isTransitioning = false;
    
    // Hysteresis
    private float lastZoneChangeTime = 0f;
    private int pendingZoneID = -1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Start playing the ambient blend
        if (playAmbianceEvent != null && playAmbianceEvent.IsValid())
        {
            playAmbianceEvent.Post(gameObject);
            
            if (showDebugLogs)
            {
                Debug.Log("🎵 Zone ambiance started");
            }
        }

        // Set initial zone
        SetZoneImmediate(0);
        crossfadeDuration = defaultCrossfadeDuration;
    }

    void Update()
    {
        // Handle hysteresis (delayed zone changes)
        if (pendingZoneID >= 0 && Time.time - lastZoneChangeTime >= hysteresisDelay)
        {
            StartCrossfade(pendingZoneID);
            pendingZoneID = -1;
        }

        // Smooth crossfade interpolation
        if (isTransitioning)
        {
            crossfadeProgress += Time.deltaTime / crossfadeDuration;
            crossfadeProgress = Mathf.Clamp01(crossfadeProgress);

            // Apply easing curve
            float curvedProgress = crossfadeCurve.Evaluate(crossfadeProgress);
            
            // Interpolate between current and target zone
            float startZone = currentZoneID;
            float interpolatedValue = Mathf.Lerp(startZone, targetZoneID, curvedProgress);

            // Update RTPC
            AkSoundEngine.SetRTPCValue(zoneRTPCName, interpolatedValue, gameObject);

            // Check if crossfade is complete
            if (crossfadeProgress >= 1f)
            {
                currentZoneID = targetZoneID;
                isTransitioning = false;
                
                if (showDebugLogs)
                {
                    Debug.Log($"✅ Crossfade complete: Zone {targetZoneID}");
                }
            }
        }
    }

    /// <summary>
    /// Change to a new zone with smooth crossfade
    /// </summary>
    /// <param name="zoneID">Zone ID (0-3)</param>
    /// <param name="customDuration">Optional custom crossfade duration (uses default if <= 0)</param>
    public void ChangeZone(int zoneID, float customDuration = -1f)
    {
        // Clamp zone ID
        zoneID = Mathf.Clamp(zoneID, 0, 3);

        // Ignore if already at/transitioning to this zone
        if (Mathf.Approximately(targetZoneID, zoneID))
        {
            return;
        }

        // Apply hysteresis (delay rapid zone changes)
        if (Time.time - lastZoneChangeTime < hysteresisDelay)
        {
            pendingZoneID = zoneID;
            
            if (showDebugLogs)
            {
                Debug.Log($"⏳ Zone change pending: {zoneID} (hysteresis delay)");
            }
            return;
        }

        // Set custom duration if provided
        if (customDuration > 0f)
        {
            crossfadeDuration = customDuration;
        }
        else
        {
            crossfadeDuration = defaultCrossfadeDuration;
        }

        StartCrossfade(zoneID);
    }

    /// <summary>
    /// Internal method to start the crossfade
    /// </summary>
    private void StartCrossfade(int zoneID)
    {
        // Store current zone as starting point
        if (!isTransitioning)
        {
            currentZoneID = targetZoneID;
        }

        targetZoneID = zoneID;
        crossfadeProgress = 0f;
        isTransitioning = true;
        lastZoneChangeTime = Time.time;

        if (showDebugLogs)
        {
            Debug.Log($"🎵 Crossfading: Zone {currentZoneID:F1} → {targetZoneID} ({crossfadeDuration:F1}s)");
        }
    }

    /// <summary>
    /// Change to a new zone instantly (no crossfade)
    /// Useful for teleports or initial spawn
    /// </summary>
    public void SetZoneImmediate(int zoneID)
    {
        zoneID = Mathf.Clamp(zoneID, 0, 3);
        
        currentZoneID = zoneID;
        targetZoneID = zoneID;
        crossfadeProgress = 1f;
        isTransitioning = false;
        pendingZoneID = -1;

        AkSoundEngine.SetRTPCValue(zoneRTPCName, currentZoneID, gameObject);

        if (showDebugLogs)
        {
            Debug.Log($"⚡ Zone set immediately: Zone {zoneID}");
        }
    }

    /// <summary>
    /// Get current zone info (for UI/debug)
    /// </summary>
    public float GetCurrentZoneValue()
    {
        if (isTransitioning)
        {
            float curvedProgress = crossfadeCurve.Evaluate(crossfadeProgress);
            return Mathf.Lerp(currentZoneID, targetZoneID, curvedProgress);
        }
        return currentZoneID;
    }

    void OnGUI()
    {
        if (!showDebugUI) return;

        // Debug display
        GUI.Box(new Rect(10, 10, 300, 100), "");
        
        float currentValue = GetCurrentZoneValue();
        string status = isTransitioning ? $"Crossfading ({crossfadeProgress * 100:F0}%)" : "Stable";
        
        GUI.Label(new Rect(20, 20, 280, 20), $"<b>Zone Ambiance</b>");
        GUI.Label(new Rect(20, 40, 280, 20), $"Current: {currentValue:F2} | Target: {targetZoneID}");
        GUI.Label(new Rect(20, 60, 280, 20), $"Status: {status}");
        GUI.Label(new Rect(20, 80, 280, 20), $"Duration: {crossfadeDuration:F1}s");
    }

    void OnDestroy()
    {
        if (stopAmbianceEvent != null && stopAmbianceEvent.IsValid())
        {
            stopAmbianceEvent.Post(gameObject);
        }
    }
}