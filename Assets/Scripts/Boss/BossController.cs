using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BossEnemyState))]
[RequireComponent(typeof(Animator))]
public class BossController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private BossConfig _config;

    [Header("References")]
    [SerializeField] private Transform _player;
    [SerializeField] private Animator _animator;

    [Header("Attack Hitbox Assignments (from scene)")]
    [SerializeField] private WeaponHitbox[] _attackHitboxes;

    [Header("Projectile Pool Assignments (from scene)")]
    [SerializeField] private ObjectPool[] _projectilePools;
    [SerializeField] private Transform[] _projectileSpawnPoints;

    public BossConfig Config => _config;
    public Transform Player => _player;
    public Animator Animator => _animator;
    public BossStateBase CurrentState { get; private set; }

    private bool _isActive = false;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
            }
        }
    }

    private float _lastJumpTime = -999f;
    private BossEnemyState _enemyState;

    void Awake()
    {
        IsActive = true;
        _animator = GetComponent<Animator>();
        _enemyState = GetComponent<BossEnemyState>();

        if (_enemyState != null && _config != null)
        {
            _enemyState.SetConfig(_config);
        }

        AssignAttackReferences();
    }

    void Start()
    {
        if (_player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _player = playerObj.transform;
            }
        }

        if (_config == null)
        {
            Debug.LogError("BossController: BossConfig not assigned!");
            enabled = false;
            return;
        }

        ChangeState(new BossIdleState(this));
    }

    private void AssignAttackReferences()
    {
        if (_config == null || _config.attacks == null) return;

        for (int i = 0; i < _config.attacks.Count; i++)
        {
            var attack = _config.attacks[i];

            // Assign melee hitbox
            if (!attack.isRanged && i < _attackHitboxes.Length)
            {
                attack.weaponHitbox = _attackHitboxes[i];
            }

            // Assign ranged pool and spawn point
            if (attack.isRanged)
            {
                if (i < _projectilePools.Length)
                {
                    attack.projectilePool = _projectilePools[i];
                }

                if (i < _projectileSpawnPoints.Length)
                {
                    attack.projectileSpawnPoint = _projectileSpawnPoints[i];
                }
            }
        }
    }

    void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.OnUpdate();
        }
    }

    void FixedUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.OnFixedUpdate();
        }
    }

    public void ChangeState(BossStateBase newState)
    {
        if (CurrentState != null)
        {
            CurrentState.OnExit();
        }

        CurrentState = newState;

        if (CurrentState != null)
        {
            CurrentState.OnEnter();
        }
    }

    public void OnBossDied()
    {
        ChangeState(new BossDeathState(this));
    }

    public bool IsJumpOnCooldown()
    {
        return Time.time < _lastJumpTime + _config.jumpCooldown;
    }

    public void MarkJumpUsed()
    {
        _lastJumpTime = Time.time;
    }

    // Animation Event Callbacks
    public void OnAttackStart()
    {
        if (CurrentState is BossAttackState attackState)
        {
            attackState.OnAttackStart();
        }
    }

    public void OnAttackEnd()
    {
        if (CurrentState is BossAttackState attackState)
        {
            attackState.OnAttackEnd();
        }
    }

    public void OnProjectileSpawn()
    {
        if (CurrentState is BossProjectileAttackState projectileState)
        {
            projectileState.OnProjectileSpawn();
        }
    }

    public void OnJumpLand()
    {
    }
}