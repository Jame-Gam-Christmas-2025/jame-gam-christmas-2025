using UnityEngine;
using System.Collections;

public class PlayerCombatController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private WeaponHitbox _weaponHitbox;
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private ObjectPool _projectilePool;
    [SerializeField] private PlayerState _playerState;

    [Header("Combo Settings")]
    [SerializeField] private float[] _comboDamages = { 10f, 15f, 20f };
    [SerializeField] private float[] _comboWindows = { 1f, 1f, 1f };

    [Header("Projectile Settings")]
    [SerializeField] private float _projectileDamage = 15f;
    [SerializeField] private float _projectileSelfDamage = 5f;
    
    [Header("SFX_MC_Attack")]
    public AK.Wwise.Event Play_MC_Attacks; // ton Event unique pour toutes les attaques

    [Header("Switch_MC_AttackType")]
    public AK.Wwise.Switch SW_MC_Attack_Light;
    public AK.Wwise.Switch SW_MC_Attack_Heavy;
    public AK.Wwise.Switch SW_MC_Attack_Distance;

    private int _currentComboIndex = 0;
    private int _attackIndexForDamage = 0;
    private bool _isAttacking = false;
    private bool _hasQueuedInput = false;
    private Coroutine _comboResetCoroutine;

    void Start()
    {
        _weaponHitbox.SetOwner(gameObject);
    }

    void Update()
    {
        if (_hasQueuedInput && !_isAttacking)
        {
            _hasQueuedInput = false;
            PerformAttack();
        }
    }

    public void OnLightAttack(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Debug.Log($"OnLightAttack: _isAttacking={_isAttacking}, _hasQueuedInput={_hasQueuedInput}");

        if (_isAttacking)
        {
            if (!_hasQueuedInput)
            {
                _hasQueuedInput = true;
                Debug.Log("Input queued");
            }
            return;
        }

        PerformAttack();
    }

    public void OnRangedAttack(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (_isAttacking) return;

        _animator.SetTrigger("RangedAttack");
        _isAttacking = true;
        
        SW_MC_Attack_Distance.SetValue(gameObject);
        Play_MC_Attacks.Post(gameObject);
    }

    private void PerformAttack()
    {
        _isAttacking = true;
        _attackIndexForDamage = _currentComboIndex;

        if (_comboResetCoroutine != null)
        {
            StopCoroutine(_comboResetCoroutine);
            _comboResetCoroutine = null;
        }

        _animator.SetInteger("ComboIndex", _currentComboIndex);
        _animator.SetTrigger("Attack");
        
        // Wwise switch pour l'attaque
        switch (_currentComboIndex)
        {
            case 0:
                SW_MC_Attack_Light.SetValue(gameObject);
                break;
            case 1:
                SW_MC_Attack_Light.SetValue(gameObject);
                break;
            case 2:
                SW_MC_Attack_Heavy.SetValue(gameObject);
                break;
            default:
                SW_MC_Attack_Light.SetValue(gameObject);
                break;
        }

        // Event
        Play_MC_Attacks.Post(gameObject);
    }

    private IEnumerator ComboResetTimer(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetCombo();
    }

    private void ResetCombo()
    {
        _currentComboIndex = 0;
        _isAttacking = false;
        _hasQueuedInput = false;
        _comboResetCoroutine = null;
    }

    public void EnableWeaponHitbox()
    {
        float damage = _comboDamages[_attackIndexForDamage];
        _weaponHitbox.SetDamage(damage);
        _weaponHitbox.EnableHitbox();
    }

    public void DisableWeaponHitbox()
    {
        _weaponHitbox.DisableHitbox();
    }

    public void OnAttackAnimationEnd()
    {
        Debug.Log($"=== OnAttackAnimationEnd CALLED === _hasQueuedInput={_hasQueuedInput}, ComboIndex={_currentComboIndex}");
        _isAttacking = false;

        if (!_hasQueuedInput)
        {
            if (_comboResetCoroutine == null)
            {
                _comboResetCoroutine = StartCoroutine(ComboResetTimer(_comboWindows[_attackIndexForDamage]));
            }
        }
        else
        {
            _currentComboIndex++;
            if (_currentComboIndex >= _comboDamages.Length)
            {
                _currentComboIndex = 0;
                _hasQueuedInput = false; 
            }
        }
    }

    public void OnRangedAttackAnimationEnd()
    {
        _isAttacking = false;
    }

    private void SpawnProjectile()
    {
        if (_projectilePool != null && _projectileSpawnPoint != null)
        {
            if (_playerState != null)
            {
                _playerState.TakeDamage(_projectileSelfDamage);
            }

            GameObject projectile = _projectilePool.GetObject();
            projectile.transform.position = _projectileSpawnPoint.position;
            projectile.transform.rotation = _projectileSpawnPoint.rotation;

            ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
            if (projectileController != null)
            {
                projectileController.Initialize(_projectileDamage, gameObject, _projectilePool);
            }
        }
    }
}