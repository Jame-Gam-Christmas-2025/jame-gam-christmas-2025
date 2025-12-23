using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using ImpactFrameSystem;

public class BossEnemyState : MonoBehaviour, IDamageable
{
    [SerializeField] private BossConfig _config;

    [Header("Impact Frame Settings")]
    [SerializeField] private float _impactDuration = 2f;
    [SerializeField] private float _impactIntensity = 1.2f;

    [Header("Death Settings")]
    [SerializeField] private float _deathDelay = 0.3f;

    public UnityEvent OnDeath;
    public UnityEvent OnDamageTaken;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => _config.maxHealth;
    public bool IsDead { get; private set; }

    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        OnDamageTaken?.Invoke();

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
       
        var bossController = GetComponent<BossController>();
        if (bossController != null)
        {
            bossController.IsActive = false; 
        }

        
        if (ImpactFrameManager.Instance != null)
        {
            ImpactFrameManager.Instance.TriggerImpactFrame(_impactDuration, _impactIntensity, transform.position);
        }

        
        var anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        
        yield return new WaitForSeconds(_deathDelay);

        OnDeath?.Invoke();
    }
}