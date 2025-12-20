using UnityEngine;

public class PlayerState : MonoBehaviour, IDamageable
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _maxHealth;
    
    public float CurrentHealth { get; private set; }

    
    void Start()
    {
        CurrentHealth = _maxHealth;
    }



    public void TakeDamage(float damageAmount)
    {
        CurrentHealth -= damageAmount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        if (_animator != null)
        {
            _animator.SetTrigger("Hit");
        }

        Debug.Log($"Player took {damageAmount} damage. Current HP: {CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //TODO: add Die() from gameManager later
        Debug.Log("you ar dead");
        Destroy(gameObject);
    }

    public void Heal(float healAmount)
    {
        CurrentHealth += healAmount;
        CurrentHealth = Mathf.Min(CurrentHealth, _maxHealth); 

        Debug.Log($"Player healed {healAmount}. Current HP: {CurrentHealth}");
    }
}
