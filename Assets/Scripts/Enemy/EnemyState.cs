using System.Collections;
using UnityEngine;
using ImpactFrameSystem;
using UnityEngine.Events;

public class EnemyState : MonoBehaviour, IDamageable
{
    
    [Header("Settings")]
    [SerializeField] private float _deathDelay = 0.3f;
    [SerializeField] private float _maxHealth = 50f;
    public UnityEvent OnDeath;

    [Header("Impact Frame Settings")]
    [SerializeField] private float _impactDuration = 2f;
    [SerializeField] private float _impactIntensity = 1.2f;

    public float CurrentHealth { get; private set; }

    void Start()
    {
        CurrentHealth = _maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        CurrentHealth -= damageAmount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        Debug.Log($"{gameObject.name} took {damageAmount} damage. Current HP: {CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        
        ImpactFrameManager.Instance.TriggerImpactFrame(
            _impactDuration,
            _impactIntensity,
            transform.position
        );

        
        yield return new WaitForSeconds(_deathDelay);

        OnDeath?.Invoke();
        
    }

    public void Heal(float healAmount)
    {
        CurrentHealth += healAmount;
        CurrentHealth = Mathf.Min(CurrentHealth, _maxHealth);

        Debug.Log($"{gameObject.name} healed {healAmount}. Current HP: {CurrentHealth}");
    }
}