using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines what boss does when activated (first action)
/// </summary>
public enum ActivationBehavior
{
    JumpToPlayer,      // Jump to player (Yule Cat - jump down from mountain)
    RunToPlayer,       // Run straight to player
    WalkToPlayer,      // Walk to player
    PerformAttack,     // Stand and attack immediately (ranged boss)
    Custom             // Do nothing, manual control
}

[CreateAssetMenu(fileName = "NewBossConfig", menuName = "Boss System/Boss Config")]
public class BossConfig : ScriptableObject
{
    [Header("Boss Identity")]
    public string bossName = "Boss";
    
    [Header("Health")]
    public float maxHealth = 500f;
    
    [Header("Activation Behavior")]
    [Tooltip("What boss does when activated (first action)")]
    public ActivationBehavior activationBehavior = ActivationBehavior.JumpToPlayer;
    
    [Header("Movement Speeds")]
    [Tooltip("Walking speed")]
    public float walkSpeed = 2f;
    
    [Tooltip("Running speed")]
    public float runSpeed = 5f;
    
    [Header("Distance Thresholds")]
    [Tooltip("Distance at which boss switches from walk to run")]
    public float runThreshold = 10f;
    
    [Tooltip("Distance at which boss stops moving and attacks")]
    public float attackThreshold = 3f;
    
    [Header("Jump Attack Settings")]
    [Tooltip("Enable jump attacks during combat")]
    public bool canJumpDuringCombat = true;
    
    [Tooltip("Cooldown between jumps")]
    public float jumpCooldown = 10f;
    
    [Tooltip("How high the boss jumps")]
    public float jumpHeight = 5f;
    
    [Tooltip("How long the jump takes")]
    public float jumpDuration = 1.5f;
    
    [Tooltip("Damage dealt when boss lands")]
    public float jumpLandingDamage = 20f;
    
    [Tooltip("Radius of landing damage")]
    public float jumpLandingRadius = 3f;
    
    [Tooltip("Minimum distance to consider jumping")]
    public float jumpMinDistance = 8f;
    
    [Tooltip("Maximum distance for jump attacks")]
    public float jumpMaxDistance = 20f;
    
    [Tooltip("Chance to jump instead of running (0-100)")]
    [Range(0f, 100f)]
    public float jumpChance = 30f;
    
    [Header("AI Behavior")]
    [Tooltip("How often AI makes decisions (seconds)")]
    public float decisionInterval = 0.5f;
    
    [Tooltip("How aggressive is the boss? Higher = more attacks")]
    [Range(0f, 100f)]
    public float aggression = 70f;
    
    [Header("Attack Configuration")]
    [Tooltip("List of all attacks this boss can perform")]
    public List<BossAttackData> attacks = new List<BossAttackData>();
    
    [Header("Animation Parameters")]
    [Tooltip("Animator parameter for movement speed (float)")]
    public string speedParameter = "Speed";
    
    [Tooltip("Animator trigger for jump")]
    public string jumpTrigger = "Jump";
    
    [Tooltip("Animator trigger for death")]
    public string deathTrigger = "Die";
    
    [Tooltip("Animator bool for combat state")]
    public string inCombatParameter = "InCombat";
    
    [Header("Death Settings")]
    [Tooltip("Delay before transitioning to death state")]
    public float deathDelay = 0.3f;
    
    [Tooltip("Impact frame duration on death")]
    public float deathImpactDuration = 2f;
    
    [Tooltip("Impact frame intensity on death")]
    public float deathImpactIntensity = 1.2f;
    
    /// <summary>
    /// Get all attacks that are valid for the given distance
    /// </summary>
    public List<BossAttackData> GetValidAttacks(float distance)
    {
        List<BossAttackData> validAttacks = new List<BossAttackData>();
        
        foreach (var attack in attacks)
        {
            if (attack.IsInRange(distance) && !attack.IsOnCooldown())
            {
                validAttacks.Add(attack);
            }
        }
        
        return validAttacks;
    }
    
    /// <summary>
    /// Select a random attack from valid attacks based on weights
    /// </summary>
    public BossAttackData SelectWeightedAttack(List<BossAttackData> validAttacks)
    {
        if (validAttacks.Count == 0) return null;
        
        float totalWeight = 0f;
        foreach (var attack in validAttacks)
        {
            totalWeight += attack.weight;
        }
        
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        
        foreach (var attack in validAttacks)
        {
            currentWeight += attack.weight;
            if (randomValue <= currentWeight)
            {
                return attack;
            }
        }
        
        return validAttacks[0]; // Fallback
    }
}
