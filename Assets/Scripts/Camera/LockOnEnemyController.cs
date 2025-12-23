using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class LockOnEnemyController : MonoBehaviour
{
    [Header("Lock-On Settings")]
    [SerializeField] private float _detectionRadius = 15f;
    [SerializeField] private float _breakLockDistance = 20f;

    [Header("Camera Settings")]
    [SerializeField] private CinemachineTargetGroup _targetGroup;

    [Header("Input")]
    [SerializeField] private InputActionReference _lockOnInput;

    [Header("Target Group Settings")]
    [SerializeField] private float _enemyWeight = 1f;
    [SerializeField] private float _enemyRadius = 1f;

    private IDamageable _currentLockedEnemy;
    private Transform _currentLockedEnemyTransform;
    private bool _isLockedOn = false;

    private void OnEnable()
    {
        if (_lockOnInput != null)
        {
            _lockOnInput.action.Enable();
            _lockOnInput.action.performed += OnLockOnInputPressed;
        }
    }

    private void OnDisable()
    {
        if (_lockOnInput != null)
        {
            _lockOnInput.action.performed -= OnLockOnInputPressed;
            _lockOnInput.action.Disable();
        }
    }

    private void Update()
    {
        if (_isLockedOn && _currentLockedEnemy != null && _currentLockedEnemyTransform != null)
        {
            float distance = Vector3.Distance(transform.position, _currentLockedEnemyTransform.position);

            // Release lock if enemy is too far or dead
            if (distance > _breakLockDistance || _currentLockedEnemy.CurrentHealth <= 0)
            {
                ReleaseLockOn();
            }
        }
    }

    /// <summary>
    /// Called when lock-on input (R key) is pressed
    /// </summary>
    private void OnLockOnInputPressed(InputAction.CallbackContext context)
    {
        if (_isLockedOn)
        {
            ReleaseLockOn();
        }
        else
        {
            TryLockOn();
        }
    }

    /// <summary>
    /// Attempts to lock onto the closest alive enemy in detection radius
    /// </summary>
    private void TryLockOn()
    {
        IDamageable closestEnemy = FindClosestAliveEnemy();

        if (closestEnemy != null)
        {
            _currentLockedEnemy = closestEnemy;
            _currentLockedEnemyTransform = ((MonoBehaviour)closestEnemy).transform;
            _isLockedOn = true;

            // Add enemy to target group
            AddEnemyToTargetGroup(_currentLockedEnemyTransform);

            // Switch camera
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.SwitchToLockOnCamera();
            }

            Debug.Log($"Locked onto: {_currentLockedEnemyTransform.name}");
        }
        else
        {
            Debug.Log("No valid enemy found in detection radius");
        }
    }

    /// <summary>
    /// Releases the current lock-on
    /// </summary>
    private void ReleaseLockOn()
    {
        // Switch camera
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.SwitchToFollowCamera();
        }

        // Remove enemy from target group
        if (_currentLockedEnemyTransform != null)
        {
            RemoveEnemyFromTargetGroup(_currentLockedEnemyTransform);
        }

        _currentLockedEnemy = null;
        _currentLockedEnemyTransform = null;
        _isLockedOn = false;

        Debug.Log("Lock-on released");
    }

    /// <summary>
    /// Finds the closest alive enemy within detection radius
    /// Searches for both EnemyState and BossEnemyState
    /// </summary>
    private IDamageable FindClosestAliveEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _detectionRadius);

        // First try to find EnemyState
        EnemyState closestEnemyState = hitColliders
            .Select(col => col.GetComponent<EnemyState>())
            .Where(enemy => enemy != null && enemy.CurrentHealth > 0)
            .OrderBy(enemy => Vector3.Distance(transform.position, enemy.transform.position))
            .FirstOrDefault();

        if (closestEnemyState != null)
        {
            return closestEnemyState;
        }

        // If no EnemyState found, try BossEnemyState
        BossEnemyState closestBossEnemyState = hitColliders
            .Select(col => col.GetComponent<BossEnemyState>())
            .Where(boss => boss != null && boss.CurrentHealth > 0)
            .OrderBy(boss => Vector3.Distance(transform.position, boss.transform.position))
            .FirstOrDefault();

        return closestBossEnemyState;
    }

    /// <summary>
    /// Adds enemy to the Cinemachine Target Group
    /// </summary>
    private void AddEnemyToTargetGroup(Transform enemy)
    {
        if (_targetGroup != null && enemy != null)
        {
            _targetGroup.AddMember(enemy, _enemyWeight, _enemyRadius);
        }
    }

    /// <summary>
    /// Removes enemy from the Cinemachine Target Group
    /// </summary>
    private void RemoveEnemyFromTargetGroup(Transform enemy)
    {
        if (_targetGroup != null && enemy != null)
        {
            _targetGroup.RemoveMember(enemy);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize detection radius (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);

        // Visualize break lock distance (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _breakLockDistance);

        // Draw line to locked enemy (green)
        if (_isLockedOn && _currentLockedEnemyTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, _currentLockedEnemyTransform.position);
        }
    }
}