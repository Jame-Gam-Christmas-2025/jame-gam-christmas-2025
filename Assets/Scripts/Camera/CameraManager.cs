using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera _followCamera;
    [SerializeField] private CinemachineCamera _lockOnCamera;

    [Header("Camera Priorities")]
    [SerializeField] private int _activePriority = 10;
    [SerializeField] private int _inactivePriority = 0;

    private CinemachineCamera _currentBossCamera;
    private Coroutine _bossCloseUpCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SwitchToFollowCamera();
    }

    /// <summary>
    /// Switches to the normal third-person follow camera
    /// </summary>
    public void SwitchToFollowCamera()
    {
        _followCamera.Priority = _activePriority;
        _lockOnCamera.Priority = _inactivePriority;

        
        if (_currentBossCamera != null)
        {
            _currentBossCamera.Priority = _inactivePriority;
        }
    }

    /// <summary>
    /// Switches to the lock-on target camera
    /// </summary>
    public void SwitchToLockOnCamera()
    {
        _lockOnCamera.Priority = _activePriority;
        _followCamera.Priority = _inactivePriority;
    }

    /// <summary>
    /// Checks if lock-on camera is currently active
    /// </summary>
    public bool IsLockOnCameraActive()
    {
        return _lockOnCamera.Priority > _followCamera.Priority;
    }

    /// <summary>
    /// Switches to a boss close-up camera for a specific duration, then returns to follow camera
    /// </summary>
    /// <param name="bossCamera">The boss camera to activate</param>
    /// <param name="duration">How long to show the boss camera in seconds</param>
    public void SwitchToBossCloseUp(CinemachineCamera bossCamera, float duration)
    {
        
        if (_bossCloseUpCoroutine != null)
        {
            StopCoroutine(_bossCloseUpCoroutine);
        }

        _bossCloseUpCoroutine = StartCoroutine(BossCloseUpSequence(bossCamera, duration));
    }

    private IEnumerator BossCloseUpSequence(CinemachineCamera bossCamera, float duration)
    {
        
        _currentBossCamera = bossCamera;

        // Deactivate other cameras
        _followCamera.Priority = _inactivePriority;
        _lockOnCamera.Priority = _inactivePriority;

        // Activate boss camera
        bossCamera.Priority = _activePriority;

        
        yield return new WaitForSeconds(duration);

        // Return to follow camera
        SwitchToFollowCamera();

        _bossCloseUpCoroutine = null;
    }
}