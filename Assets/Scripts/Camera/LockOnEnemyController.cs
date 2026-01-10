using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class LockOnEnemyController : MonoBehaviour
{
    [SerializeField] private float _detectionRadius = 15f;
    [SerializeField] private float _breakLockDistance = 20f;

    [SerializeField] private CinemachineTargetGroup _targetGroup;

    [SerializeField] private InputActionReference _lockOnInput;

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

            if (distance > _breakLockDistance || _currentLockedEnemy.CurrentHealth <= 0)
            {
                ReleaseLockOn();
            }
        }
    }

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

    private void TryLockOn()
    {
        IDamageable closestEnemy = FindClosestAliveEnemy();

        if (closestEnemy != null)
        {
            _currentLockedEnemy = closestEnemy;
            _currentLockedEnemyTransform = ((MonoBehaviour)closestEnemy).transform;
            _isLockedOn = true;

            AddEnemyToTargetGroup(_currentLockedEnemyTransform);

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

    private void ReleaseLockOn()
    {
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.SwitchToFollowCamera();
        }

        if (_currentLockedEnemyTransform != null)
        {
            RemoveEnemyFromTargetGroup(_currentLockedEnemyTransform);
        }

        _currentLockedEnemy = null;
        _currentLockedEnemyTransform = null;
        _isLockedOn = false;

        Debug.Log("Lock-on released");
    }

    private IDamageable FindClosestAliveEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _detectionRadius);

        EnemyState closestEnemyState = hitColliders
            .Select(col => col.GetComponentInParent<EnemyState>())
            .Where(enemy => enemy != null && enemy.CurrentHealth > 0)
            .OrderBy(enemy => Vector3.Distance(transform.position, enemy.transform.position))
            .FirstOrDefault();

        if (closestEnemyState != null)
        {
            return closestEnemyState;
        }

        BossEnemyState closestBossEnemyState = hitColliders
            .Select(col => col.GetComponentInParent<BossEnemyState>())
            .Where(boss => boss != null && boss.CurrentHealth > 0)
            .OrderBy(boss => Vector3.Distance(transform.position, boss.transform.position))
            .FirstOrDefault();

        return closestBossEnemyState;
    }

    private void AddEnemyToTargetGroup(Transform enemy)
    {
        if (_targetGroup != null && enemy != null)
        {
            _targetGroup.AddMember(enemy, _enemyWeight, _enemyRadius);
        }
    }

    private void RemoveEnemyFromTargetGroup(Transform enemy)
    {
        if (_targetGroup != null && enemy != null)
        {
            _targetGroup.RemoveMember(enemy);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _breakLockDistance);

        if (_isLockedOn && _currentLockedEnemyTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, _currentLockedEnemyTransform.position);
        }
    }
}