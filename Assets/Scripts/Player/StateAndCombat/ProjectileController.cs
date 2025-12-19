using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _lifetime = 5f;
    [SerializeField] private float _launchDelay = 0.2f;

    private float _damage;
    private GameObject _owner;
    private Rigidbody _rigidbody;
    private ObjectPool _pool;
    private float _spawnTime;
    private Collider _projectileCollider;
    private bool _isActive = false;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _projectileCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (!_isActive) return;

        if (Time.time - _spawnTime >= _lifetime)
        {
            ReturnToPool();
        }
    }

    public void Initialize(float damage, GameObject owner, ObjectPool pool)
    {
        _isActive = true;
        _damage = damage;
        _owner = owner;
        _pool = pool;
        _spawnTime = Time.time;

        Collider ownerCollider = owner.GetComponent<Collider>();
        if (ownerCollider != null && _projectileCollider != null)
        {
            Physics.IgnoreCollision(_projectileCollider, ownerCollider, true);
        }

        StartCoroutine(LaunchWithDelay());
    }

    private IEnumerator LaunchWithDelay()
    {
        yield return new WaitForSeconds(_launchDelay);

        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = transform.forward * _speed;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_isActive) return;
        if (other.isTrigger) return;
        if (other.gameObject == _owner) return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_damage);
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        _isActive = false;

        if (_pool != null)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _pool.ReturnObject(gameObject);
        }
    }
}