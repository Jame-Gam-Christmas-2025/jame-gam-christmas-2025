using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [Header("Virtual Cameras")]
    [SerializeField] private CinemachineCamera followCamera;
    [SerializeField] private CinemachineCamera lockOnCamera;

    [Header("Camera Priorities")]
    [SerializeField] private int activePriority = 10;
    [SerializeField] private int inactivePriority = 0;

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
        followCamera.Priority = activePriority;
        lockOnCamera.Priority = inactivePriority;
    }

    /// <summary>
    /// Switches to the lock-on target camera
    /// </summary>
    public void SwitchToLockOnCamera()
    {
        lockOnCamera.Priority = activePriority;
        followCamera.Priority = inactivePriority;
    }

    /// <summary>
    /// Checks if lock-on camera is currently active
    /// </summary>
    public bool IsLockOnCameraActive()
    {
        return lockOnCamera.Priority > followCamera.Priority;
    }
}