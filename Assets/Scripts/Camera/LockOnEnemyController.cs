using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class LockOnEnemyController : MonoBehaviour
{
    [Header("Lock-On Settings")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private float breakLockDistance = 20f;

    [Header("Camera Settings")]
    [SerializeField] private CinemachineTargetGroup targetGroup;

    [Header("Input")]
    [SerializeField] private InputActionReference lockOnInput;

    [Header("Target Group Settings")]
    [SerializeField] private float enemyWeight = 1f;
    [SerializeField] private float enemyRadius = 1f;

    private EnemyState currentLockedEnemy;
    private bool isLockedOn = false;

    private void OnEnable()
    {
        if (lockOnInput != null)
        {
            lockOnInput.action.Enable();
            lockOnInput.action.performed += OnLockOnInputPressed;
        }
    }

    private void OnDisable()
    {
        if (lockOnInput != null)
        {
            lockOnInput.action.performed -= OnLockOnInputPressed;
            lockOnInput.action.Disable();
        }
    }

    private void Update()
    {
        
        if (isLockedOn && currentLockedEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, currentLockedEnemy.transform.position);

            // Release lock if enemy is too far or dead
            if (distance > breakLockDistance || currentLockedEnemy.CurrentHealth <= 0)
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
        if (isLockedOn)
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
        EnemyState closestEnemy = FindClosestAliveEnemy();

        if (closestEnemy != null)
        {
            currentLockedEnemy = closestEnemy;
            isLockedOn = true;

            
            AddEnemyToTargetGroup(closestEnemy);

            
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.SwitchToLockOnCamera();
            }

            Debug.Log($"Locked onto: {closestEnemy.name}");
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
        
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.SwitchToFollowCamera();
        }

        
        if (currentLockedEnemy != null)
        {
            RemoveEnemyFromTargetGroup(currentLockedEnemy);
        }

        currentLockedEnemy = null;
        isLockedOn = false;

        Debug.Log("Lock-on released");
    }

    /// <summary>
    /// Finds the closest alive enemy within detection radius using "Enemy" tag
    /// </summary>
    private EnemyState FindClosestAliveEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);

        EnemyState closestEnemy = hitColliders
            .Select(col => col.GetComponent<EnemyState>())
            .Where(enemy => enemy != null &&
                            enemy.CompareTag("Enemy") &&
                            enemy.CurrentHealth > 0)
            .OrderBy(enemy => Vector3.Distance(transform.position, enemy.transform.position))
            .FirstOrDefault();

        return closestEnemy;
    }

    /// <summary>
    /// Adds enemy to the Cinemachine Target Group
    /// </summary>
    private void AddEnemyToTargetGroup(EnemyState enemy)
    {
        if (targetGroup != null && enemy != null)
        {
            targetGroup.AddMember(enemy.transform, enemyWeight, enemyRadius);
        }
    }

    /// <summary>
    /// Removes enemy from the Cinemachine Target Group
    /// </summary>
    private void RemoveEnemyFromTargetGroup(EnemyState enemy)
    {
        if (targetGroup != null && enemy != null)
        {
            targetGroup.RemoveMember(enemy.transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize detection radius (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Visualize break lock distance (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, breakLockDistance);

        // Draw line to locked enemy (green)
        if (isLockedOn && currentLockedEnemy != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, currentLockedEnemy.transform.position);
        }
    }
}