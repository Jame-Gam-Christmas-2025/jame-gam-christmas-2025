using UnityEngine;

public class PlayerState : MonoBehaviour, IDamageable
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _maxHealth;

    public float MaxHealth => _maxHealth;
    public float CurrentHealth { get; private set; }
    
    private bool _isInvincible = false;

    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        if (_isInvincible)
        {
            //Debug.Log("Player is invincible! Damage ignored.");
            return;
        }

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
        GameManager.Instance.GameOver();
    }

    public void Heal(float healAmount)
    {
        CurrentHealth += healAmount;
        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
        //Debug.Log($"Player healed {healAmount}. Current HP: {CurrentHealth}");
    }

    // Methods for animation events
    public void StartInvincibility()
    {
        _isInvincible = true;
        //Debug.Log("Player is now invincible");
    }

    public void EndInvincibility()
    {
        _isInvincible = false;
        //Debug.Log("Player invincibility ended");
    }
}