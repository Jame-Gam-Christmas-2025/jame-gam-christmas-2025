using UnityEngine;

/// <summary>
/// Basic projectile script for boss ranged attacks
/// Extend this class for custom projectile behaviors
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _lifetime = 5f;
    [SerializeField] private bool _useGravity = false;
    
    [Header("Hit Effects")]
    [SerializeField] private GameObject _hitEffectPrefab;
    [SerializeField] private bool _destroyOnHit = true;
    
    private Rigidbody _rb;
    private Vector3 _direction;
    private float _damage;
    private GameObject _owner;
    private ObjectPool _pool;
    private float _spawnTime;
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = _useGravity;
    }
    
    /// <summary>
    /// Initialize the projectile (called when spawned from pool)
    /// </summary>
    public void Initialize(Vector3 direction, float damage, GameObject owner, ObjectPool pool)
    {
        _direction = direction.normalized;
        _damage = damage;
        _owner = owner;
        _pool = pool;
        _spawnTime = Time.time;
        
        // Set velocity
        _rb.linearVelocity = _direction * _speed;
        
        // Rotate to face direction
        if (_direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_direction);
        }
    }
    
    void Update()
    {
        // Check lifetime
        if (Time.time - _spawnTime > _lifetime)
        {
            ReturnToPool();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Don't hit owner
        if (other.gameObject == _owner) return;
        
        // Try to damage
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_damage);
            Debug.Log($"Projectile hit {other.gameObject.name} for {_damage} damage");
            
            // Spawn hit effect
            SpawnHitEffect(other.ClosestPoint(transform.position));
            
            if (_destroyOnHit)
            {
                ReturnToPool();
            }
        }
        // Hit environment
        else if (!other.isTrigger)
        {
            SpawnHitEffect(other.ClosestPoint(transform.position));
            
            if (_destroyOnHit)
            {
                ReturnToPool();
            }
        }
    }
    
    private void SpawnHitEffect(Vector3 hitPoint)
    {
        if (_hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(_hitEffectPrefab, hitPoint, Quaternion.identity);
            Destroy(effect, 2f); // Auto cleanup
        }
    }
    
    private void ReturnToPool()
    {
        // Reset velocity
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        
        // Return to pool
        if (_pool != null)
        {
            _pool.ReturnObject(gameObject);
        }
        else
        {
            // Fallback if pool is missing
            gameObject.SetActive(false);
        }
    }
}
