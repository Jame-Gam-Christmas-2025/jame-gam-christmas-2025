using UnityEngine;

[System.Serializable]
public class BossAttackData
{
    [Header("Animation")]
    public string animationTrigger = "Attack";

    [Header("Range")]
    public float minRange = 0f;
    public float maxRange = 3f;

    [Header("Type & Damage")]
    public bool isRanged = false;
    public float damage = 10f;

    [Header("Timing")]
    public float cooldown = 2f;

    [Header("Selection Weight")]
    [Range(0f, 100f)]
    public float weight = 50f;

    [HideInInspector] public float lastUsedTime = -999f;

    // Runtime - assigned from BossController
    [System.NonSerialized] public WeaponHitbox weaponHitbox;
    [System.NonSerialized] public ObjectPool projectilePool;
    [System.NonSerialized] public Transform projectileSpawnPoint;

    public bool IsOnCooldown()
    {
        return Time.time < lastUsedTime + cooldown;
    }

    public bool IsInRange(float distance)
    {
        return distance >= minRange && distance <= maxRange;
    }

    public void MarkAsUsed()
    {
        lastUsedTime = Time.time;
    }
}