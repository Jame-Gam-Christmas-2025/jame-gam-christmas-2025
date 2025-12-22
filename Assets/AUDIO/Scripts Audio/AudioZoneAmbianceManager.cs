using UnityEngine;

/// <summary>
/// Gère les zones et change le state Wwise correspondant à chaque zone
/// </summary>
public class ZoneTriggerState : MonoBehaviour
{
    [Header("Zone Settings")]
    [SerializeField] private int zoneStateID = 0; // index du state dans le State Group
    [SerializeField] private int fallbackStateID = 0; // zone par défaut quand on sort

    [Header("Wwise State")]
    [SerializeField] private string stateGroupName = "STATE_AMB_PAD_Zones";

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AkSoundEngine.SetState(stateGroupName, GetStateName(zoneStateID));
            if (showDebugLogs)
                Debug.Log($"🎵 Player entered zone {zoneStateID} -> State {GetStateName(zoneStateID)}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AkSoundEngine.SetState(stateGroupName, GetStateName(fallbackStateID));
            if (showDebugLogs)
                Debug.Log($"🎵 Player exited zone {zoneStateID} -> fallback State {GetStateName(fallbackStateID)}");
        }
    }

    private string GetStateName(int index)
    {
        // Ici tu peux remplacer par les vrais noms des states Wwise si tu préfères
        switch(index)
        {
            case 0: return "ST_AMB_PAD_Forest";
            case 1: return "ST_AMB_PAD_Flower";
            case 2: return "ST_AMB_PAD_Village";
            case 3: return "ST_AMB_PAD_Castle";
            default: return "ST_AMB_PAD_Forest";
        }
    }
}