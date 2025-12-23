using System.Collections;
using UnityEngine;
using ImpactFrameSystem;
using UnityEngine.Events;

/// <summary>
/// Boss version of EnemyState that doesn't destroy the GameObject on death.
/// Instead, it transitions to a "Lie Down" state for further interactions.
/// </summary>
public class BossEnemyState : MonoBehaviour, IDamageable
{
    [Header("Boss Settings")]
    [SerializeField] private BossConfig _config;
    
    [Header("Impact Frame Settings")]
    [SerializeField] private float _impactDuration = 2f;
    [SerializeField] private float _impactIntensity = 1.2f;
    
    public UnityEvent OnDeath;
    public UnityEvent OnDamageTaken;
    
    public float CurrentHealth { get; private set; }
    public float MaxHealth => _config != null ? _config.maxHealth : 500f;
    public bool IsDead { get; private set; }
    
    private BossController _bossController;
    
    void Awake()
    {
        _bossController = GetComponent<BossController>();
    }
    
    void Start()
    {
        CurrentHealth = MaxHealth;
        IsDead = false;
    }
    
    public void TakeDamage(float damageAmount)
    {
        if (IsDead) return;
        
        CurrentHealth -= damageAmount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);
        
        OnDamageTaken?.Invoke();
        
        Debug.Log($"{gameObject.name} took {damageAmount} damage. Current HP: {CurrentHealth}/{MaxHealth}");
        
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Die()
    {
        if (IsDead) return;
        
        IsDead = true;
        StartCoroutine(DeathSequence());
    }
    
    private IEnumerator DeathSequence()
    {
        // Trigger impact frame
        if (ImpactFrameManager.Instance != null)
        {
            ImpactFrameManager.Instance.TriggerImpactFrame(
                _impactDuration,
                _impactIntensity,
                transform.position
            );
        }
        
        // Wait for death delay
        float deathDelay = _config != null ? _config.deathDelay : 0.3f;
        yield return new WaitForSeconds(deathDelay);
        
        // Notify boss controller to transition to death state
        if (_bossController != null)
        {
            _bossController.OnBossDied();
        }
        
        OnDeath?.Invoke();
        
        // NOTE: We do NOT destroy the GameObject - boss remains in scene
        Debug.Log($"{gameObject.name} has died but remains in scene for interaction.");
    }
    
    public void Heal(float healAmount)
    {
        if (IsDead) return;
        
        CurrentHealth += healAmount;
        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
        Debug.Log($"{gameObject.name} healed {healAmount}. Current HP: {CurrentHealth}/{MaxHealth}");
    }
    
    /// <summary>
    /// Set the boss config reference
    /// </summary>
    public void SetConfig(BossConfig config)
    {
        _config = config;
        if (CurrentHealth == 0)
        {
            CurrentHealth = MaxHealth;
        }
    }
}
