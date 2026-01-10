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
    public AK.Wwise.Event Play_MC_Attacks;
    
    [Header("SFX_MC_TakeDamage")]
    public AK.Wwise.Event Play_MC_TakeDamage;

    [Header("SFX_MC_AttackHitEnemy")]
    public AK.Wwise.Event Play_SFX_MC_AttackHitEnemy;
    
    [Header("Switch_MC_AttackType")]
    public AK.Wwise.Switch SW_MC_Attack_Light;
    public AK.Wwise.Switch SW_MC_Attack_Heavy;
    public AK.Wwise.Switch SW_MC_Attack_Distance;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private bool _canAttack = true;
    private int _currentComboIndex = 0;
    private int _attackIndexForDamage = 0;
    public bool IsAttacking { get; set; } = false;
    private bool _hasQueuedInput = false;
    private Coroutine _comboResetCoroutine;

    private bool _isListeningCombo = false;
    private bool _isComboTriggered = false;
    private bool _isAttacking = false;

    private void Start()
    {
        _weaponHitbox.SetOwner(gameObject);
    }

    private void Update()
    {
        // Traiter l'input en queue uniquement si on n'attaque plus
        if (_hasQueuedInput && !IsAttacking)
        {
            _hasQueuedInput = false;
            /* PerformAttack(); */
        }
    }

    public void SetIsAttackingFalse()
    {
        IsAttacking = false;
    }

    public void EnableAttack()
    {
        _canAttack = true;
    }

    public void DisableAttack()
    {
        _canAttack = false;
    }

    public void OnLightAttack(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        // Ne traiter que le moment du press, pas le hold
        if (!context.performed) return;

        InitAttack();
    }

    public void OnRangedAttack(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!_canAttack) return;

        if (!context.performed) return;
        if (_isAttacking) return;

        _animator.SetTrigger("RangedAttack");
        _isAttacking = true;
    }
    
    // === ANIMATION EVENT METHODS ===
    // Ces méthodes sont appelées par les Animation Events, PAS par le code !
    
    /// <summary>
    /// Called by Animation Event - Play melee attack sound
    /// Place this event at the moment of impact in your attack animation
    /// </summary>
    public void Play_Attack_Melee()
    {
        // Le switch est déjà set dans PerformAttack(), donc on poste juste l'event
        Play_MC_Attacks.Post(gameObject);
    }
    
    public void SFX_MC_TakeDamage()
    {
        Play_MC_TakeDamage.Post(gameObject);
    }
    /// <summary>
    /// Called by Animation Event - Play ranged attack sound
    /// </summary>
    public void Play_Attack_Distance()
    {
        SW_MC_Attack_Distance.SetValue(gameObject);
        Play_MC_Attacks.Post(gameObject);
    }

    public void SFX_MC_AttackHitEnemy()
    {
        Play_SFX_MC_AttackHitEnemy.Post(gameObject);
    }
    

    private void InitAttack()
    {
        if(!_isAttacking && _canAttack)
        {
            // IMPORTANT : Reset trigger avant de le set pour éviter les doubles déclenchements
            _animator.ResetTrigger("Attack");
            
            // Set combo index et trigger
            _animator.SetInteger("ComboIndex", _currentComboIndex);
            _animator.SetTrigger("Attack");

            _isAttacking = true;
            _attackIndexForDamage = _currentComboIndex;
        }

        switch (_currentComboIndex)
        {
            case 0:
                SW_MC_Attack_Light.SetValue(gameObject);
                if (showDebugLogs) Debug.Log("Switch set: Light Attack 1");
                break;
            case 1:
                SW_MC_Attack_Light.SetValue(gameObject);
                if (showDebugLogs) Debug.Log("Switch set: Light Attack 2");
                break;
            case 2:
                SW_MC_Attack_Heavy.SetValue(gameObject);
                if (showDebugLogs) Debug.Log("Switch set: Heavy Attack (finisher)");
                break;
            default:
                SW_MC_Attack_Light.SetValue(gameObject);
                break;
        }

        if(_isListeningCombo && !_isComboTriggered)
        {
            TriggerNextAttack();
        }
    }

    private void AddComboListener()
    {
        _isListeningCombo = true;
    }

    private void RemoveComboListener()
    {
        _isListeningCombo = false;
    }

    private void TriggerNextAttack()
    {
        _currentComboIndex++;
        _isComboTriggered = true;
    }

    private void OnAttackEnd()
    {
        _isAttacking = false;
        if(_isComboTriggered)
        {
            _isComboTriggered = false;
            InitAttack();
        } else
        {
            _currentComboIndex = 0;
        }
    }
    private void PerformAttack()
    {
        if (!_canAttack) return;

        IsAttacking = true;
        _attackIndexForDamage = _currentComboIndex;

        // Arrêter le timer de reset si en cours
        if (_comboResetCoroutine != null)
        {
            StopCoroutine(_comboResetCoroutine);
            _comboResetCoroutine = null;
        }

        // IMPORTANT : Reset trigger avant de le set pour éviter les doubles déclenchements
        _animator.ResetTrigger("Attack");
        
        // Set combo index et trigger
        _animator.SetInteger("ComboIndex", _currentComboIndex);
        _animator.SetTrigger("Attack");
        
        // === WWISE AUDIO ===
        // Set le switch selon l'attaque (le son sera joué par l'Animation Event)
        switch (_currentComboIndex)
        {
            case 0:
                SW_MC_Attack_Light.SetValue(gameObject);
                if (showDebugLogs) Debug.Log("Switch set: Light Attack 1");
                break;
            case 1:
                SW_MC_Attack_Light.SetValue(gameObject);
                if (showDebugLogs) Debug.Log("Switch set: Light Attack 2");
                break;
            case 2:
                SW_MC_Attack_Heavy.SetValue(gameObject);
                if (showDebugLogs) Debug.Log("Switch set: Heavy Attack (finisher)");
                break;
            default:
                SW_MC_Attack_Light.SetValue(gameObject);
                break;
        }
    }

    private IEnumerator ComboResetTimer(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetCombo();
    }

    private void ResetCombo()
    {
        _currentComboIndex = 0;
        IsAttacking = false;
        _hasQueuedInput = false;
        _comboResetCoroutine = null;
    }

    // Called by Animation Event
    public void EnableWeaponHitbox()
    {
        float damage = _comboDamages[_attackIndexForDamage];
        _weaponHitbox.SetDamage(damage);
    
        // Set attack type based on combo index
        string attackType = "Light";
        switch (_attackIndexForDamage)
        {
            case 0:
            case 1:
                attackType = "Light";
                break;
            case 2:
                attackType = "Heavy";
                break;
        }
    
        // IMPORTANT: Set attack type BEFORE enabling hitbox
        _weaponHitbox.SetAttackType(attackType);
        _weaponHitbox.EnableHitbox();
    
        if (showDebugLogs)
        {
            Debug.Log($"Weapon hitbox enabled - Damage: {damage}, Type: {attackType}");
        }
        //float damage = _comboDamages[_attackIndexForDamage];
        //_weaponHitbox.SetDamage(damage);
        //_weaponHitbox.EnableHitbox();
    }

    // Called by Animation Event
    public void DisableWeaponHitbox()
    {
        _weaponHitbox.DisableHitbox();
    }

    // Called by Animation Event - MUST be at the END of attack animation
    public void OnAttackAnimationEnd()
    {
        /* IsAttacking = false;

        // Si pas d'input en queue, démarrer le timer de reset
        if (!_hasQueuedInput)
        {
            if (_comboResetCoroutine == null)
            {
                _comboResetCoroutine = StartCoroutine(ComboResetTimer(_comboWindows[_attackIndexForDamage]));
            }
        }
        else
        {
            // Sinon, avancer dans le combo
            _currentComboIndex++;
            
            // Si on dépasse le nombre d'attaques, reset
            if (_currentComboIndex >= _comboDamages.Length)
            {
                _currentComboIndex = 0;
                _hasQueuedInput = false;
            }
            else
            {
                if (showDebugLogs)
                {
                    Debug.Log($"Combo continues to index {_currentComboIndex}");
                }
            }
        } */
    }

    // Called by Animation Event
    public void OnRangedAttackAnimationEnd()
    {
        IsAttacking = false;
    }

    // Called by Animation Event
    private void SpawnProjectile()
    {
        if (_projectilePool != null && _projectileSpawnPoint != null)
        {
            // Appliquer self-damage
            if (_playerState != null)
            {
                _playerState.TakeDamage(_projectileSelfDamage);
            }

            // Spawn projectile depuis le pool
            GameObject projectile = _projectilePool.GetObject();
            projectile.transform.position = _projectileSpawnPoint.position;
            projectile.transform.rotation = _projectileSpawnPoint.rotation;

            // Initialize projectile
            ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
            if (projectileController != null)
            {
                projectileController.Initialize(_projectileDamage, gameObject, _projectilePool);
            }
        }
    }
}