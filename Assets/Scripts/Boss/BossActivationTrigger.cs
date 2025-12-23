using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Example script to activate boss when player enters trigger zone
/// Attach this to a trigger collider near the boss
/// </summary>
[RequireComponent(typeof(Collider))]
public class BossActivationTrigger : MonoBehaviour
{
    [Header("Boss Reference")]
    [Tooltip("The boss controller to activate")]
    [SerializeField] private BossController _bossToActivate;
    
    [Header("Trigger Settings")]
    [Tooltip("Only activate once?")]
    [SerializeField] private bool _activateOnce = true;
    
    [Tooltip("Tag to check (usually 'Player')")]
    [SerializeField] private string _targetTag = "Player";
    
    [Header("Activation Delay")]
    [Tooltip("Delay before activating boss (for dramatic effect)")]
    [SerializeField] private float _activationDelay = 0.5f;
    
    [Header("Events")]
    public UnityEvent OnBossActivated;
    
    private bool _hasActivated = false;
    private Collider _triggerCollider;
    
    void Awake()
    {
        _triggerCollider = GetComponent<Collider>();
        _triggerCollider.isTrigger = true;
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if already activated
        if (_activateOnce && _hasActivated) return;
        
        // Check tag
        if (!other.CompareTag(_targetTag)) return;
        
        // Check if boss is assigned
        if (_bossToActivate == null)
        {
            Debug.LogError("BossActivationTrigger: Boss controller not assigned!");
            return;
        }
        
        // Activate boss
        StartCoroutine(ActivateBossWithDelay());
    }
    
    private System.Collections.IEnumerator ActivateBossWithDelay()
    {
        _hasActivated = true;
        
        Debug.Log($"Boss activation triggered! Activating in {_activationDelay}s...");
        
        yield return new WaitForSeconds(_activationDelay);
        
        // Activate the boss
        _bossToActivate.IsActive = true;
        
        // Invoke event
        OnBossActivated?.Invoke();
        
        Debug.Log($"Boss {_bossToActivate.gameObject.name} has been ACTIVATED!");
        
        // Optionally disable trigger
        if (_activateOnce)
        {
            _triggerCollider.enabled = false;
        }
    }
    
    // Manual activation method (can be called from other scripts or UnityEvents)
    public void ActivateBossManually()
    {
        if (_bossToActivate != null)
        {
            _bossToActivate.IsActive = true;
            _hasActivated = true;
            OnBossActivated?.Invoke();
        }
    }
    
    void OnDrawGizmos()
    {
        // Draw trigger zone
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = _hasActivated ? Color.red : Color.yellow;
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }
    }
}
