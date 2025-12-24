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
    private bool _isActive;

    private bool _isJumping;
    private Vector3 _jumpTarget;
    private float _jumpTimer;
    private float _jumpDuration;
    private float _lastJumpTime = -999f;

    public bool IsActive
    {
        get => _isActive;
        set
        {
            // Setting to true - handle activation
            if (!_isActive && value)
            {
                if (_config.activationBehavior == BossActivation.JumpToPlayer)
                {
                    StartCoroutine(ActivationJump());
                }
                else
                {
                    _isActive = true;
                }
            }
            // Setting to false - always disable
            else if (!value)
            {
                _isActive = false;
                // Stop movement animation
                if (_anim != null)
                {
                    _anim.SetFloat("Speed", 0);
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
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _player = playerObj.transform;
            }
        }

        AssignAttackReferences();
    }

    void AssignAttackReferences()
    {
        for (int i = 0; i < _config.attacks.Count; i++)
        {
            var attack = _config.attacks[i];

            if (!attack.isRanged && i < _attackHitboxes.Length)
            {
                attack.hitbox = _attackHitboxes[i];
            }

            if (attack.isRanged)
            {
                if (i < _projectilePools.Length)
                    attack.pool = _projectilePools[i];
                if (i < _projectileSpawns.Length)
                    attack.spawnPoint = _projectileSpawns[i];
            }
        }
    }

    void PerformActivation()
    {
        if (_config.activationBehavior == BossActivation.JumpToPlayer)
        {
            StartCoroutine(ActivationJump());
        }
    }

    void FixedUpdate()
    {
        if (!_isActive) return;

        RotateToPlayer();

        if (_isAttacking || _isJumping) return;

        float distance = Vector3.Distance(transform.position, _player.position);

        // Combat jump check (only when cooldown ready)
        if (_config.canJumpInCombat && Time.time >= _lastJumpTime + _config.jumpCooldown)
        {
            if (distance >= _config.combatJumpMinDistance && distance <= _config.combatJumpMaxDistance)
            {
                if (Random.Range(0f, 100f) < _config.combatJumpChance)
                {
                    _lastJumpTime = Time.time;
                    StartCoroutine(CombatJump());
                    return;
                }
                else
                {
                    _lastJumpTime = Time.time;
                }
            }
        }

        if (distance <= _config.attackDistance)
        {
            TryAttack();
        }
        else
        {
            MoveToPlayer();
        }
    }

    void RotateToPlayer()
    {
        if (_player == null) return;

        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, _config.rotationSpeed * Time.fixedDeltaTime));
        }
    }

    void MoveToPlayer()
    {
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0;

        _rb.MovePosition(_rb.position + direction * _config.moveSpeed * Time.fixedDeltaTime);
        _anim.SetFloat("Speed", _config.moveSpeed);
    }

    void TryAttack()
    {
        _anim.SetFloat("Speed", 0);

        float distance = Vector3.Distance(transform.position, _player.position);

        foreach (var attack in _config.attacks)
        {
            if (distance >= attack.minRange &&
                distance <= attack.maxRange &&
                !attack.IsOnCooldown())
            {
                PerformAttack(attack);
                return;
            }
        }
    }

    void PerformAttack(BossAttack attack)
    {
        _isAttacking = true;
        _currentAttack = attack;
        _currentAttack.MarkAsUsed();
        _anim.SetTrigger(attack.animationTrigger);
    }

    public void OnAttackStart()
    {
        if (_currentAttack != null && _currentAttack.hitbox != null)
        {
            _currentAttack.hitbox.EnableHitbox();
        }
    }

    public void OnAttackEnd()
    {
        if (_currentAttack != null && _currentAttack.hitbox != null)
        {
            _currentAttack.hitbox.DisableHitbox();
        }

        _isAttacking = false;
        _currentAttack = null;
    }

    public void OnProjectileSpawn()
    {
        if (_currentAttack != null && _currentAttack.pool != null)
        {
            GameObject proj = _currentAttack.pool.GetObject();
            Transform spawn = _currentAttack.spawnPoint ?? transform;

            proj.transform.position = spawn.position;

            var projScript = proj.GetComponent<Projectile>();
            if (projScript != null)
            {
                Vector3 dir = (_player.position - spawn.position).normalized;
                projScript.Initialize(dir, _currentAttack.damage, gameObject, _currentAttack.pool);
            }
        }

        _isAttacking = false;
    }

    IEnumerator ActivationJump()
    {
        if (_config == null)
        {
            Debug.LogError("BossConfig is NULL!");
            yield break;
        }

        _rb.useGravity = false;
        _isJumping = true;

        Vector3 start = transform.position;
        Vector3 target = _player.position;
        float timer = 0f;
        float duration = _config.jumpDuration;

        _anim.SetTrigger("Jump");

        // Jump loop
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / duration);

            Vector3 horizontalPos = Vector3.Lerp(start, target, progress);
            float height = 4f * progress * (1f - progress) * _config.jumpHeight;

            _rb.MovePosition(horizontalPos + Vector3.up * height);

            yield return null;
        }

        // Jump finished - land
        _rb.MovePosition(target);
        _rb.useGravity = true;
        _isJumping = false;

        // Deal landing damage
        Collider[] hits = Physics.OverlapSphere(transform.position, _config.jumpRadius);
        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null && hit.gameObject != gameObject)
            {
                damageable.TakeDamage(_config.jumpDamage);
            }
        }

        // NOW activate boss after jump completes
        _isActive = true;
    }

    IEnumerator CombatJump()
    {
        _rb.useGravity = false;
        _isJumping = true;

        Vector3 start = transform.position;
        Vector3 target = _player.position;
        float timer = 0f;
        float duration = _config.jumpDuration;

        _anim.SetTrigger("Jump");

        // Jump loop
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / duration);

            Vector3 horizontalPos = Vector3.Lerp(start, target, progress);
            float height = 4f * progress * (1f - progress) * _config.jumpHeight;

            _rb.MovePosition(horizontalPos + Vector3.up * height);

            yield return null;
        }

        // Jump finished - land
        _rb.MovePosition(target);
        _rb.useGravity = true;
        _isJumping = false;

        // Deal landing damage
        Collider[] hits = Physics.OverlapSphere(transform.position, _config.jumpRadius);
        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null && hit.gameObject != gameObject)
            {
                damageable.TakeDamage(_config.jumpDamage);
            }
        }
    }

    public void OnJumpLand()
    {
        if (!_isJumping) return;

        _isJumping = false;
        _rb.MovePosition(_jumpTarget);
        _rb.useGravity = true;

        Collider[] hits = Physics.OverlapSphere(transform.position, _config.jumpRadius);
        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null && hit.gameObject != gameObject)
            {
                damageable.TakeDamage(_config.jumpDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (_config == null) return;

        // Attack Distance (Yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _config.attackDistance);

        // Combat Jump Min Distance (Green)
        if (_config.canJumpInCombat)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _config.combatJumpMinDistance);

            // Combat Jump Max Distance (Cyan)
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _config.combatJumpMaxDistance);
        }

        // Jump Landing Radius (Red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _config.jumpRadius);

        // Attack ranges for each attack (draw on ground)
        Vector3 groundPos = transform.position;
        groundPos.y = 0.1f;

        foreach (var attack in _config.attacks)
        {
            if (!attack.isRanged)
            {
                // Melee attacks - white
                Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
                Gizmos.DrawWireSphere(groundPos, attack.maxRange);
            }
            else
            {
                // Ranged attacks - blue
                Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
                Gizmos.DrawWireSphere(groundPos, attack.minRange);
                Gizmos.DrawWireSphere(groundPos, attack.maxRange);
            }
        }
    }
}