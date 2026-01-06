using Unity.Cinemachine;
using UnityEngine;


public class BossCloseUpCameratrigger : MonoBehaviour
{
    [Header("Boss Camera Settings")]
    [SerializeField] private CinemachineCamera _bossCloseUpCamera;
    [SerializeField] private float _closeUpDuration = 3f;

    [Header("Trigger Settings")]
    [SerializeField] private string _playerTag = "Player";

    [SerializeField] private Collider _triggerCollider;
    private bool _hasTriggered = false;

    private void Awake()
    {
        
        _triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (_hasTriggered)
            return;

        
        if (!other.CompareTag(_playerTag))
            return;

        
        if (CameraManager.Instance == null)
        {
            Debug.LogError("CameraManager instance not found!");
            return;
        }

        
        if (_bossCloseUpCamera == null)
        {
            Debug.LogError("Boss close-up camera not assigned on " + gameObject.name);
            return;
        }

        // Trigger the boss close-up
        CameraManager.Instance.SwitchToBossCloseUp(_bossCloseUpCamera, _closeUpDuration);

        // Mark as triggered and disable the collider
        _hasTriggered = true;
        _triggerCollider.enabled = false;
    }

    /// <summary>
    /// Call this if you want to reset the trigger (e.g., if player dies and respawns)
    /// </summary>
    public void ResetTrigger()
    {
        _hasTriggered = false;
        _triggerCollider.enabled = true;
    }
}