using UnityEngine;

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

    // States
    private bool _isAttacking;
    [SerializeField] private bool _isActive;
    private bool _isJumping;

    // Jump Calculation Vars
    private Vector3 _jumpStartPos;
    private Vector3 _jumpTargetPos;
    private float _jumpTimer;
    private float _lastJumpTime = -999f;

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                if (_isActive && _config.activationBehavior == BossActivation.JumpToPlayer)
                {
                    InitiateJump();
                }
            }
        }
    }

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        if (_player == null) _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        AssignAttackReferences();
    }

    void FixedUpdate()
    {
        if (!_isActive) return;

        // BLOCK 1: Jump update
        if (_isJumping)
        {
            UpdateJumpPhysics();
            return; // Absolute lockout
        }

        if (_isAttacking || _player == null) return;

        RotateToPlayer();

        float distance = Vector3.Distance(transform.position, _player.position);

        // BLOCK 2: Combat Jump Trigger
        if (_config.canJumpInCombat &&
            Time.time >= _lastJumpTime + _config.jumpCooldown &&
            distance >= _config.combatJumpMinDistance &&
            distance <= _config.combatJumpMaxDistance)
        {
            _lastJumpTime = Time.time;

            if (Random.Range(0f, 100f) < _config.combatJumpChance)
            {
                InitiateJump();
                return;
            }
        }

        // BLOCK 3: Standard AI
        if (distance <= _config.attackDistance)
        {
            TryAttack(distance);
        }
        else
        {
            MoveToPlayer();
        }
    }

    private void InitiateJump()
    {
        if (_player == null) return;

        _isJumping = true;
        _jumpTimer = 0f;
        _jumpStartPos = transform.position;
        _jumpTargetPos = _player.position;

        // PHYSICS RESET: Essential for vertical movement
        _rb.useGravity = false;
        _rb.isKinematic = true; // Temporary kinematic to bypass friction/gravity fighting
        _rb.interpolation = RigidbodyInterpolation.None; // Prevent jitter during manual Lerp

        _anim.SetFloat("Speed", 0);
        _anim.SetTrigger("Jump");
    }

    private void UpdateJumpPhysics()
    {
        float duration = Mathf.Max(0.1f, _config.jumpDuration);
        _jumpTimer += Time.fixedDeltaTime;
        float t = _jumpTimer / duration;

        // Parabola logic
        Vector3 horizontal = Vector3.Lerp(_jumpStartPos, _jumpTargetPos, t);
        float height = 4f * t * (1f - t) * _config.jumpHeight;

        // Use movePosition or transform.position since we are Kinematic
        _rb.MovePosition(horizontal + Vector3.up * height);

        if (t >= 1.0f)
        {
            Landing();
        }
    }

    private void Landing()
    {
        _isJumping = false;

        // PHYSICS RESTORE
        _rb.isKinematic = false;
        _rb.useGravity = true;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Final snap to target
        transform.position = _jumpTargetPos;
        _rb.linearVelocity = Vector3.zero;

        // Damage
        Collider[] hits = Physics.OverlapSphere(transform.position, _config.jumpRadius);
        foreach (var hit in hits)
        {
            if (hit.gameObject != gameObject)
                hit.GetComponent<IDamageable>()?.TakeDamage(_config.jumpDamage);
        }
    }

    // --- Standard AI Methods ---

    void MoveToPlayer()
    {
        Vector3 dir = (_player.position - transform.position).normalized;
        dir.y = 0;
        _rb.MovePosition(_rb.position + dir * _config.moveSpeed * Time.fixedDeltaTime);
        _anim.SetFloat("Speed", _config.moveSpeed);
    }

    void RotateToPlayer()
    {
        Vector3 dir = (_player.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion target = Quaternion.LookRotation(dir);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, target, _config.rotationSpeed * Time.fixedDeltaTime));
        }
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

    // --- Boilerplate Setup ---
    public void OnAttackStart() => _currentAttack?.hitbox?.EnableHitbox();
    public void OnAttackEnd() { _currentAttack?.hitbox?.DisableHitbox(); _isAttacking = false; _currentAttack = null; }
    public void OnJumpLand() { if (_isJumping) Landing(); }

    private void AssignAttackReferences()
    {
        if (_config?.attacks == null) return;
        for (int i = 0; i < _config.attacks.Count; i++)
        {
            var a = _config.attacks[i];
            if (!a.isRanged && i < _attackHitboxes.Length) a.hitbox = _attackHitboxes[i];
            if (a.isRanged && i < _projectilePools.Length) a.pool = _projectilePools[i];
            if (a.isRanged && i < _projectileSpawns.Length) a.spawnPoint = _projectileSpawns[i];
        }
    }
}