using UnityEngine;
using System.Collections.Generic;

public enum BossActivation
{
    JumpToPlayer,
    RunToPlayer,
    None
}

[CreateAssetMenu(fileName = "BossConfig", menuName = "Boss/Config")]
public class BossConfig : ScriptableObject
{
    [Header("Health")]
    public float maxHealth = 1000f;

    [Header("Activation")]
    public BossActivation activationBehavior = BossActivation.JumpToPlayer;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Tooltip("How fast boss rotates to face player (higher = faster)")]
    public float rotationSpeed = 5f;

    [Header("Attack Distance")]
    [Tooltip("Boss attacks when closer than this distance")]
    public float attackDistance = 3f;

    [Header("Jump (for activation only)")]
    public float jumpHeight = 5f;
    public float jumpDuration = 1.5f;
    public float jumpDamage = 30f;
    public float jumpRadius = 4f;

    [Header("Combat Jump (optional)")]
    [Tooltip("Can boss jump during combat?")]
    public bool canJumpInCombat = false;

    [Tooltip("Minimum distance to jump (in combat)")]
    public float combatJumpMinDistance = 8f;

    [Tooltip("Maximum distance to jump (in combat)")]
    public float combatJumpMaxDistance = 20f;

    [Tooltip("Chance to jump instead of running (0-100)")]
    [Range(0f, 100f)]
    public float combatJumpChance = 30f;

    [Tooltip("Cooldown between jumps")]
    public float jumpCooldown = 10f;

    [Header("Attacks")]
    public List<BossAttack> attacks = new List<BossAttack>();
}

[System.Serializable]
public class BossAttack
{
    public string animationTrigger = "Attack";
    public float minRange = 0f;
    public float maxRange = 3f;
    public float damage = 25f;
    public bool isRanged = false;
    public float cooldown = 1f;

    [System.NonSerialized] public float lastUsedTime = -999f;
    [System.NonSerialized] public WeaponHitbox hitbox;
    [System.NonSerialized] public ObjectPool pool;
    [System.NonSerialized] public Transform spawnPoint;

    public bool IsOnCooldown()
    {
        return Time.time < lastUsedTime + cooldown;
    }

    public void MarkAsUsed()
    {
        lastUsedTime = Time.time;
    }
}