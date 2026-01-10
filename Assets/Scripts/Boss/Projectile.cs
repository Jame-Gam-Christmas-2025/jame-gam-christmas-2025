using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _lifetime = 5f;
    [SerializeField] private bool _useGravity = false;

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

    public void Initialize(Vector3 direction, float damage, GameObject owner, ObjectPool pool)
    {
        _direction = direction.normalized;
        _damage = damage;
        _owner = owner;
        _pool = pool;
        _spawnTime = Time.time;

        _rb.linearVelocity = _direction * _speed;

        if (_direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_direction);
        }
    }

    void Update()
    {
        if (Time.time - _spawnTime > _lifetime)
        {
            ReturnToPool();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        if (_owner != null)
        {
            if (other.gameObject == _owner) return;
            if (other.transform.IsChildOf(_owner.transform)) return;
        }

        IDamageable damageable = other.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(_damage);

            SpawnHitEffect(other.ClosestPoint(transform.position));

            if (_destroyOnHit)
            {
                ReturnToPool();
            }
        }
        else
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
            Destroy(effect, 2f);
        }
    }

    private void ReturnToPool()
    {
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        if (_pool != null)
        {
            _pool.ReturnObject(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}