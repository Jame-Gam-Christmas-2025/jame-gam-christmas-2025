using UnityEngine;

public class EnemyState : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHealth = 50f;

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
        Debug.Log($"{gameObject.name} is dead");
        Destroy(gameObject);
    }

    public void Heal(float healAmount)
    {
        CurrentHealth += healAmount;
        CurrentHealth = Mathf.Min(CurrentHealth, _maxHealth);

        Debug.Log($"{gameObject.name} healed {healAmount}. Current HP: {CurrentHealth}");
    }
}