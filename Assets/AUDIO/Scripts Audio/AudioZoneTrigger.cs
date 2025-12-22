using UnityEngine;

/// <summary>
/// Trigger zone that changes ambient music when player enters
/// Can use a single collider on this GameObject OR multiple colliders on child GameObjects
/// </summary>
public class ZoneTrigger : MonoBehaviour
{
    [Header("Zone Settings")]
    [SerializeField] private int zoneID = 0;
    [Tooltip("0 = Forest, 1 = Cave, 2 = Mountain, 3 = Dungeon")]
    
    [SerializeField] private int fallbackZoneID = -1;
    [Tooltip("Zone to return to when exiting this zone. -1 = no fallback (use for largest zones)")]

    [Header("Crossfade Settings")]
    [SerializeField] private float enterCrossfadeDuration = -1f;
    [Tooltip("Custom crossfade duration when entering this zone. -1 = use default")]
    
    [SerializeField] private float exitCrossfadeDuration = -1f;
    [Tooltip("Custom crossfade duration when exiting this zone. -1 = use default")]

    [Header("Collider Setup")]
    [SerializeField] private bool useChildColliders = false;
    [Tooltip("If true, will use colliders on child GameObjects instead of this GameObject")]

    [Header("Visual Debug")]
    [SerializeField] private Color gizmoColor = Color.green;
    [SerializeField] private bool showGizmo = true;
    [SerializeField] private bool showZoneLabel = true;

    private Collider[] _triggers;

    void Awake()
    {
        if (useChildColliders)
        {
            // Get all colliders from children
            _triggers = GetComponentsInChildren<Collider>();
            
            foreach (var trigger in _triggers)
            {
                if (!trigger.isTrigger)
                {
                    Debug.LogWarning($"ZoneTrigger child {trigger.gameObject.name}: Setting as Trigger");
                    trigger.isTrigger = true;
                }
            }
        }
        else
        {
            // Use collider on this GameObject
            Collider trigger = GetComponent<Collider>();
            
            if (trigger != null)
            {
                if (!trigger.isTrigger)
                {
                    Debug.LogWarning($"ZoneTrigger on {gameObject.name}: Setting as Trigger");
                    trigger.isTrigger = true;
                }
                _triggers = new Collider[] { trigger };
            }
            else
            {
                Debug.LogError($"ZoneTrigger on {gameObject.name}: No collider found! Add a Box/Sphere/Capsule Collider.");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if it's the player
        if (other.CompareTag("Player"))
        {
            if (ZoneAmbianceManager.Instance != null)
            {
                // Use custom duration if set, otherwise use default
                ZoneAmbianceManager.Instance.ChangeZone(zoneID, enterCrossfadeDuration);
                Debug.Log($"🎵 Player entered Zone {zoneID} - {gameObject.name}");
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        // Check if it's the player
        if (other.CompareTag("Player"))
        {
            // Only trigger fallback if one is defined
            if (fallbackZoneID >= 0 && ZoneAmbianceManager.Instance != null)
            {
                // Use custom exit duration if set
                ZoneAmbianceManager.Instance.ChangeZone(fallbackZoneID, exitCrossfadeDuration);
                Debug.Log($"🎵 Player exited Zone {zoneID}, returning to Zone {fallbackZoneID}");
            }
        }
    }

    // Visual feedback in Scene view
    void OnDrawGizmos()
    {
        if (!showGizmo) return;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            
            if (col is BoxCollider box)
            {
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (col is SphereCollider sphere)
            {
                Gizmos.DrawWireSphere(sphere.center, sphere.radius);
            }
        }

        // Draw zone label
        if (showZoneLabel)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, 
                $"Zone {zoneID}", 
                new GUIStyle() { 
                    normal = new GUIStyleState() { textColor = gizmoColor }, 
                    fontSize = 14, 
                    fontStyle = FontStyle.Bold 
                });
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmo) return;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Color transparentColor = gizmoColor;
            transparentColor.a = 0.2f;
            Gizmos.color = transparentColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            
            if (col is BoxCollider box)
            {
                Gizmos.DrawCube(box.center, box.size);
            }
            else if (col is SphereCollider sphere)
            {
                Gizmos.DrawSphere(sphere.center, sphere.radius);
            }
        }
    }
}