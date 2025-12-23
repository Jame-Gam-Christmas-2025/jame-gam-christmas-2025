using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Projectile Attack State - For ranged attacks using ObjectPool
/// </summary>
public class BossProjectileAttackState : BossStateBase
{
    private BossAttackData _currentAttack;
    private bool _isAttacking;
    private bool _hasSpawnedProjectile;
    private bool _attackFinished;
    
    public BossProjectileAttackState(BossController controller) : base(controller)
    {
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        
        _isAttacking = false;
        _hasSpawnedProjectile = false;
        _attackFinished = false;
        
        // Stop movement
        if (animator != null && !string.IsNullOrEmpty(config.speedParameter))
        {
            animator.SetFloat(config.speedParameter, 0f);
        }
        
        // Select and perform attack
        SelectAndPerformAttack();
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if (!_isAttacking) return;
        
        // Keep rotating towards player during attack
        RotateTowardsPlayer(5f);
        
        // Wait for attack to finish (based on animation length or manual finish)
        if (_attackFinished)
        {
            FinishAttack();
        }
    }
    
    private void SelectAndPerformAttack()
    {
        float distance = GetDistanceToPlayer();
        
        // Get valid ranged attacks for current distance
        var validAttacks = config.GetValidAttacks(distance);
        validAttacks.RemoveAll(a => !a.isRanged); // Only ranged attacks
        
        if (validAttacks.Count == 0)
        {
            // No valid ranged attacks, switch to melee or movement
            Debug.Log("No valid ranged attacks available");
            
            if (distance > config.attackThreshold)
            {
                controller.ChangeState(new BossWalkState(controller));
            }
            else
            {
                // Try melee attacks
                controller.ChangeState(new BossAttackState(controller));
            }
            return;
        }
        
        // Select attack based on weights
        _currentAttack = config.SelectWeightedAttack(validAttacks);
        
        if (_currentAttack == null)
        {
            Debug.LogWarning("Failed to select projectile attack!");
            controller.ChangeState(new BossWalkState(controller));
            return;
        }
        
        // Execute attack
        ExecuteAttack();
    }
    
    private void ExecuteAttack()
    {
        _isAttacking = true;
        _hasSpawnedProjectile = false;
        _attackFinished = false;
        
        // Trigger animation
        if (animator != null && !string.IsNullOrEmpty(_currentAttack.animationTrigger))
        {
            animator.SetTrigger(_currentAttack.animationTrigger);
        }
        
        // Mark attack as used (for cooldown)
        _currentAttack.MarkAsUsed();
        
        Debug.Log("Boss performing projectile attack");
        
        // Auto-finish after a delay if no animation event is set
        controller.StartCoroutine(AutoFinishAttack());
    }
    
    private System.Collections.IEnumerator AutoFinishAttack()
    {
        // Wait for reasonable attack time (2 seconds as fallback)
        yield return new WaitForSeconds(2f);
        
        if (_isAttacking && !_attackFinished)
        {
            _attackFinished = true;
        }
    }
    
    /// <summary>
    /// Called from Animation Event to spawn projectile
    /// </summary>
    public void OnProjectileSpawn()
    {
        if (_hasSpawnedProjectile) return;
        if (_currentAttack == null) return;
        
        _hasSpawnedProjectile = true;
        
        // Get projectile from pool
        if (_currentAttack.projectilePool != null)
        {
            GameObject projectile = _currentAttack.projectilePool.GetObject();
            
            // Position at spawn point
            Transform spawnPoint = _currentAttack.projectileSpawnPoint != null 
                ? _currentAttack.projectileSpawnPoint 
                : transform;
            
            projectile.transform.position = spawnPoint.position;
            projectile.transform.rotation = spawnPoint.rotation;
            
            // Setup projectile
            var projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                Vector3 direction = GetDirectionToPlayer();
                projectileScript.Initialize(
                    direction, 
                    _currentAttack.damage, 
                    controller.gameObject,
                    _currentAttack.projectilePool
                );
            }
            
            Debug.Log("Projectile spawned");
            
            // Mark attack as finished after spawning projectile
            _attackFinished = true;
        }
        else
        {
            Debug.LogWarning("Projectile pool not assigned for attack");
        }
    }
    
    private void FinishAttack()
    {
        _isAttacking = false;
        
        float distance = GetDistanceToPlayer();
        
        // Decide next action
        if (distance > config.runThreshold)
        {
            controller.ChangeState(new BossRunState(controller));
        }
        else if (distance > config.attackThreshold)
        {
            controller.ChangeState(new BossWalkState(controller));
        }
        else
        {
            // Try another attack based on aggression
            bool shouldAttackAgain = Random.Range(0f, 100f) < config.aggression;
            
            if (shouldAttackAgain)
            {
                // Check if should use ranged or melee
                var validRanged = config.GetValidAttacks(distance);
                validRanged.RemoveAll(a => !a.isRanged);
                
                if (validRanged.Count > 0 && Random.Range(0f, 1f) > 0.5f)
                {
                    // Another projectile attack
                    SelectAndPerformAttack();
                }
                else
                {
                    // Switch to melee
                    controller.ChangeState(new BossAttackState(controller));
                }
            }
            else
            {
                controller.ChangeState(new BossWalkState(controller));
            }
        }
    }
    
    public override void OnExit()
    {
        base.OnExit();
    }
}
