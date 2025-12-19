using UnityEngine;
using System.Collections.Generic;

public class WeaponHitbox : MonoBehaviour
{
    private Collider _collider;
    private float _currentDamage;
    private GameObject _owner; 
    private List<GameObject> _hitTargets = new List<GameObject>(); 

    void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
        DisableHitbox(); 
    }

    public void SetOwner(GameObject owner)
    {
        _owner = owner;
    }

    public void SetDamage(float damage)
    {
        _currentDamage = damage;
    }

    public void EnableHitbox()
    {
        _hitTargets.Clear(); 
        _collider.enabled = true;
    }

    public void DisableHitbox()
    {
        _collider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject == _owner) return;

        
        if (_hitTargets.Contains(other.gameObject)) return;

        
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_currentDamage);
            _hitTargets.Add(other.gameObject);

            Debug.Log($"Hit {other.gameObject.name} for {_currentDamage} damage");
        }
    }
}