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

    [Header("Switch_MC_AttackType")]
    public AK.Wwise.Switch SW_MC_Attack_Light;
    public AK.Wwise.Switch SW_MC_Attack_Heavy;
    public AK.Wwise.Switch SW_MC_Attack_Distance;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private int _currentComboIndex = 0;
    private int _attackIndexForDamage = 0;
    private bool _isAttacking = false;
    private bool _hasQueuedInput = false;
    private Coroutine _comboResetCoroutine;

    void Start()
    {
        _weaponHitbox.SetOwner(gameObject);
        
        if (showDebugLogs)
        {
            Debug.Log("PlayerCombatController initialized");
        }
    }

    void Update()
    {
        // Traiter l'input en queue uniquement si on n'attaque plus
        if (_hasQueuedInput && !_isAttacking)
        {
            if (showDebugLogs)
            {
                Debug.Log("Processing queued input");
            }
            
            _hasQueuedInput = false;
            PerformAttack();
        }
    }

    public void OnLightAttack(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        // Ne traiter que le moment du press, pas le hold
        if (!context.performed) return;

        if (showDebugLogs)
        {
            Debug.Log($"OnLightAttack: _isAttacking={_isAttacking}, _hasQueuedInput={_hasQueuedInput}");
        }

        // Si on attaque déjà, mettre en queue
        if (_isAttacking)
        {
            if (!_hasQueuedInput)
            {
                _hasQueuedInput = true;
                
                if (showDebugLogs)
                {
                    Debug.Log("Input queued for combo");
                }
            }
            return;
        }

        // Sinon, attaquer directement
        PerformAttack();
    }

    public void OnRangedAttack(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (_isAttacking) return;

        if (showDebugLogs)
        {
            Debug.Log("Performing ranged attack");
        }

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
        
        if (showDebugLogs)
        {
            Debug.Log($"Melee attack sound played - ComboIndex: {_attackIndexForDamage}");
        }
    }
    
    /// <summary>
    /// Called by Animation Event - Play ranged attack sound
    /// </summary>
    public void Play_Attack_Distance()
    {
        SW_MC_Attack_Distance.SetValue(gameObject);
        Play_MC_Attacks.Post(gameObject);
        
        if (showDebugLogs)
        {
            Debug.Log("Ranged attack sound played");
        }
    }

    private void PerformAttack()
    {
        // PROTECTION : Bloquer si déjà en train d'attaquer
        if (_isAttacking)
        {
            Debug.LogWarning("⚠️ PerformAttack called while already attacking! BLOCKED to prevent loop.");
            return;
        }

        if (showDebugLogs)
        {
            Debug.Log($"PerformAttack called - ComboIndex: {_currentComboIndex}");
        }

        _isAttacking = true;
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

        // NOTE: Le son sera joué par l'Animation Event AE_PlayMeleeAttackSound()
        // Ne PAS poster l'event ici pour avoir le timing exact avec l'animation
    }

    private IEnumerator ComboResetTimer(float delay)
    {
        if (showDebugLogs)
        {
            Debug.Log($"Combo reset timer started: {delay}s");
        }
        
        yield return new WaitForSeconds(delay);
        
        if (showDebugLogs)
        {
            Debug.Log("Combo reset timer expired - resetting combo");
        }
        
        ResetCombo();
    }

    private void ResetCombo()
    {
        if (showDebugLogs)
        {
            Debug.Log("=== COMBO RESET ===");
        }
        
        _currentComboIndex = 0;
        _isAttacking = false;
        _hasQueuedInput = false;
        _comboResetCoroutine = null;
    }

    // Called by Animation Event
    public void EnableWeaponHitbox()
    {
        float damage = _comboDamages[_attackIndexForDamage];
        _weaponHitbox.SetDamage(damage);
        _weaponHitbox.EnableHitbox();
        
        if (showDebugLogs)
        {
            Debug.Log($"Weapon hitbox enabled - Damage: {damage}");
        }
    }

    // Called by Animation Event
    public void DisableWeaponHitbox()
    {
        _weaponHitbox.DisableHitbox();
        
        if (showDebugLogs)
        {
            Debug.Log("Weapon hitbox disabled");
        }
    }

    // Called by Animation Event - MUST be at the END of attack animation
    public void OnAttackAnimationEnd()
    {
        if (showDebugLogs)
        {
            Debug.Log($"=== OnAttackAnimationEnd === HasQueuedInput: {_hasQueuedInput}, ComboIndex: {_currentComboIndex}");
        }
        
        _isAttacking = false;

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
                if (showDebugLogs)
                {
                    Debug.Log("Combo finished - resetting to 0");
                }
                
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
        }
    }

    // Called by Animation Event
    public void OnRangedAttackAnimationEnd()
    {
        if (showDebugLogs)
        {
            Debug.Log("Ranged attack animation ended");
        }
        
        _isAttacking = false;
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
            
            if (showDebugLogs)
            {
                Debug.Log("Projectile spawned");
            }
        }
    }
}