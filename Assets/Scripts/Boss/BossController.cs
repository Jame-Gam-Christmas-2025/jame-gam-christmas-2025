using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class BossController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private BossConfig _config;

    [Header("Scene References")]
    [SerializeField] private Transform _player;
    [SerializeField] private WeaponHitbox[] _attackHitboxes;
    [SerializeField] private ObjectPool[] _projectilePools;
    [SerializeField] private Transform[] _projectileSpawns;

    private Rigidbody _rb;
    private Animator _anim;
    private BossAttack _currentAttack;

    private bool _isAttacking;
    [SerializeField] private bool _isActive;
    private bool _isJumping;

    private Vector3 _jumpTarget;
    private float _lastJumpTime = -999f;

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                if (_isActive)
                {
                    // Immediately block FixedUpdate movement BEFORE the next physics frame
                    if (_config.activationBehavior == BossActivation.JumpToPlayer)
                    {
                        _isJumping = true;
                        _anim.SetFloat("Speed", 0);
                        StartCoroutine(JumpToPlayerRoutine());
                    }
                }
            }
        }
    }

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();

        if (_player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) _player = p.transform;
        }

        AssignAttackReferences();
    }

    void FixedUpdate()
    {
        // 1. Check if boss is active
        if (!_isActive) return;

        // 2. Check if boss is busy. 
        // If _isJumping was set in the property setter, this returns immediately.
        if (_isJumping || _isAttacking) return;

        if (_player == null) return;

        RotateToPlayer();

        float distance = Vector3.Distance(transform.position, _player.position);

        // 3. Combat Jump Logic
        if (_config.canJumpInCombat &&
            Time.time >= _lastJumpTime + _config.jumpCooldown &&
            distance >= _config.combatJumpMinDistance &&
            distance <= _config.combatJumpMaxDistance)
        {
            // Update cooldown timer even if chance fails to prevent frame-spamming
            _lastJumpTime = Time.time;

            if (Random.Range(0f, 100f) < _config.combatJumpChance)
            {
                _isJumping = true;
                StartCoroutine(JumpToPlayerRoutine());
                return;
            }
        }

        // 4. Combat / Movement
        if (distance <= _config.attackDistance)
        {
            TryAttack(distance);
        }
        else
        {
            MoveToPlayer();
        }
    }

    void RotateToPlayer()
    {
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRot, _config.rotationSpeed * Time.fixedDeltaTime));
        }
    }

    void MoveToPlayer()
    {
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0;
        _rb.MovePosition(_rb.position + direction * _config.moveSpeed * Time.fixedDeltaTime);
        _anim.SetFloat("Speed", _config.moveSpeed);
    }

    void TryAttack(float distance)
    {
        _anim.SetFloat("Speed", 0);
        foreach (var attack in _config.attacks)
        {
            if (distance >= attack.minRange && distance <= attack.maxRange && !attack.IsOnCooldown())
            {
                _isAttacking = true;
                _currentAttack = attack;
                _currentAttack.MarkAsUsed();
                _anim.SetTrigger(attack.animationTrigger);
                return;
            }
        }
    }

    IEnumerator JumpToPlayerRoutine()
    {
        _rb.useGravity = false;

#if UNITY_6000_0_OR_NEWER
        _rb.linearVelocity = Vector3.zero;
#else
            _rb.velocity = Vector3.zero;
#endif

        Vector3 startPos = transform.position;
        _jumpTarget = _player.position;

        float timer = 0f;
        float duration = Mathf.Max(0.1f, _config.jumpDuration);

        _anim.SetTrigger("Jump");

        while (timer < duration)
        {
            timer += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(timer / duration);

            Vector3 currentPos = Vector3.Lerp(startPos, _jumpTarget, t);
            float height = 4f * t * (1f - t) * _config.jumpHeight;

            _rb.MovePosition(currentPos + Vector3.up * height);

            yield return new WaitForFixedUpdate();
        }

        CompleteLanding();
    }

    public void OnJumpLand() // Animation Event
    {
        CompleteLanding();
    }

    private void CompleteLanding()
    {
        if (!_isJumping) return;

        _isJumping = false;
        _rb.MovePosition(_jumpTarget);
        _rb.useGravity = true;

#if UNITY_6000_0_OR_NEWER
        _rb.linearVelocity = Vector3.zero;
#else
            _rb.velocity = Vector3.zero;
#endif

        // Landing Damage
        Collider[] hits = Physics.OverlapSphere(transform.position, _config.jumpRadius);
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;
            var dmg = hit.GetComponent<IDamageable>();
            dmg?.TakeDamage(_config.jumpDamage);
        }
    }

    // --- Boilerplate Animation Events ---
    public void OnAttackStart() => _currentAttack?.hitbox?.EnableHitbox();
    public void OnAttackEnd() { _currentAttack?.hitbox?.DisableHitbox(); _isAttacking = false; _currentAttack = null; }
    public void OnProjectileSpawn() { /* logic same as before */ }
    private void AssignAttackReferences() { /* logic same as before */ }
}