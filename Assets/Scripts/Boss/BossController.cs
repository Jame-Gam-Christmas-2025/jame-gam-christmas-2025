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
    private float _lastJumpTime = -999f;

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (!_isActive && value)
            {
                if (_config != null)
                {
                    BossUIController.Instance.ActivateBossHealthBar(_config.maxHealth, _config.bossName);
                    var impactManager = FindFirstObjectByType<BossImpactFrameManager>();
                    if (impactManager != null)
                    {
                        impactManager.SetColorsForBoss(_config.bossName);
                    }
                }

                if (_config.activationBehavior == BossActivation.JumpToPlayer)
                {
                    
                    StartCoroutine(ActivationJump());
                }
                else
                {
                    _isActive = true;
                }
            }
            else if (!value)
            {
                _isActive = false;
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
        int meleeIndex = 0;
        int rangedIndex = 0;

        for (int i = 0; i < _config.attacks.Count; i++)
        {
            var attack = _config.attacks[i];

            if (!attack.isRanged)
            {
                if (meleeIndex < _attackHitboxes.Length)
                {
                    attack.hitbox = _attackHitboxes[meleeIndex];
                }
                meleeIndex++;
            }
            else
            {
                if (rangedIndex < _projectilePools.Length)
                    attack.pool = _projectilePools[rangedIndex];

                if (rangedIndex < _projectileSpawns.Length)
                    attack.spawnPoint = _projectileSpawns[rangedIndex];

                rangedIndex++;
            }
        }
    }

    void FixedUpdate()
    {
        if (!_isActive) return;

        var enemyState = GetComponent<BossEnemyState>();
        if (enemyState != null && enemyState.IsDead)
        {
            return;
        }

        RotateToPlayer();

        if (_isAttacking || _isJumping) return;

        float distance = Vector3.Distance(transform.position, _player.position);

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

        // Find valid attack (in range AND not on cooldown)
        foreach (var attack in _config.attacks)
        {
            bool inRange = distance >= attack.minRange && distance <= attack.maxRange;
            bool onCooldown = attack.IsOnCooldown();

            if (inRange && !onCooldown)
            {
                PerformAttack(attack);
                return;
            }
        }

        // No valid attack - just wait
        // Boss will keep trying every frame until cooldown is ready
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
            _currentAttack.hitbox.SetDamage(_currentAttack.damage);
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
        if (_config == null) yield break;

        var impactManager = FindFirstObjectByType<BossImpactFrameManager>();
        if (impactManager != null)
        {
            impactManager.SetColorsForBoss(_config.bossName);
        }

        _rb.useGravity = false;
        _isJumping = true;

        Vector3 start = transform.position;
        Vector3 target = _player.position;
        float timer = 0f;
        float duration = _config.jumpDuration;

        _anim.SetTrigger("Jump");

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / duration);

            Vector3 horizontalPos = Vector3.Lerp(start, target, progress);
            float height = 4f * progress * (1f - progress) * _config.jumpHeight;

            _rb.MovePosition(horizontalPos + Vector3.up * height);

            yield return null;
        }

        _rb.MovePosition(target);
        _rb.useGravity = true;
        _isJumping = false;

        Collider[] hits = Physics.OverlapSphere(transform.position, _config.jumpRadius);
        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null && hit.gameObject != gameObject)
            {
                damageable.TakeDamage(_config.jumpDamage);
            }
        }

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

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / duration);

            Vector3 horizontalPos = Vector3.Lerp(start, target, progress);
            float height = 4f * progress * (1f - progress) * _config.jumpHeight;

            _rb.MovePosition(horizontalPos + Vector3.up * height);

            yield return null;
        }

        _rb.MovePosition(target);
        _rb.useGravity = true;
        _isJumping = false;

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
        // Not used anymore
    }

    void OnDrawGizmos()
    {
        if (_config == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _config.attackDistance);

        if (_config.canJumpInCombat)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _config.combatJumpMinDistance);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _config.combatJumpMaxDistance);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _config.jumpRadius);

        Vector3 groundPos = transform.position;
        groundPos.y = 0.1f;

        foreach (var attack in _config.attacks)
        {
            if (!attack.isRanged)
            {
                Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
                Gizmos.DrawWireSphere(groundPos, attack.maxRange);
            }
            else
            {
                Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
                Gizmos.DrawWireSphere(groundPos, attack.minRange);
                Gizmos.DrawWireSphere(groundPos, attack.maxRange);
            }
        }
    }

    /////////////// AUDIO /////////////
    [Header("SFX")]
    public AK.Wwise.Event BossFootsteps;
    public AK.Wwise.Event BossAttack1;
    public AK.Wwise.Event BossAttack2;
    public AK.Wwise.Event BossQueue;

    public void SFX_Boss_Footsteps()
    {
        BossFootsteps?.Post(gameObject);
    }

    public void SFX_BossAttack1()
    {
        BossAttack1?.Post(gameObject);
    }

    public void SFX_BossAttack2()
    {
        BossAttack2?.Post(gameObject);
    }

    public void SFX_BossQueue()
    {
        /* BossQueue.Post(gameObject); */
    }
}