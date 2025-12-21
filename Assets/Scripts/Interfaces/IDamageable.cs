using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(float damageAmount);
    public float CurrentHealth { get; }
}