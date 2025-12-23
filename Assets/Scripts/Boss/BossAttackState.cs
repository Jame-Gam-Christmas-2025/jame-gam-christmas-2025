using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Attack state - Boss performs melee attacks
/// </summary>
public class BossAttackState : BossStateBase
{
    private BossAttackData _currentAttack;
    private bool _isAttacking;
    private bool _attackFinished;
    
    public BossAttackState(BossController controller) : base(controller)
    {
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        
        _isAttacking = false;
        _attackFinished = false;
        
        // Stop movement
        if (animator != null && !string.IsNullOrEmpty(config.speedParameter))
        {
            animator.SetFloat(config.speedParameter, 0f);
        }
        
        // Select and initiate attack
        SelectAndPerformAttack();
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if (!_isAttacking) return;
        
        // Keep rotating towards player during attack
        RotateTowardsPlayer(5f);
        
        // Wait for OnAttackEnd() to be called from animation event
        if (_attackFinished)
        {
            DecideNextAction();
        }
    }
    
    private void SelectAndPerformAttack()
    {
        float distance = GetDistanceToPlayer();
        
        // Get valid attacks for current distance
        List<BossAttackData> validAttacks = config.GetValidAttacks(distance);
        
        if (validAttacks.Count == 0)
        {
            // No valid attacks, move closer or switch state
            Debug.Log("No valid attacks available, switching to movement");
            
            if (distance > config.attackThreshold)
            {
                controller.ChangeState(new BossWalkState(controller));
            }
            else
            {
                // Too close? Back up slightly (stay in attack state but wait)
                controller.StartCoroutine(WaitAndRetry());
            }
            return;
        }
        
        // Select attack based on weights
        _currentAttack = config.SelectWeightedAttack(validAttacks);
        
        if (_currentAttack == null)
        {
            Debug.LogWarning("Failed to select attack!");
            controller.ChangeState(new BossWalkState(controller));
            return;
        }
        
        // Check if it's a ranged attack
        if (_currentAttack.isRanged)
        {
            // Switch to projectile attack state
            controller.ChangeState(new BossProjectileAttackState(controller));
            return;
        }
        
        // Execute melee attack
        ExecuteAttack();
    }
    
    private void ExecuteAttack()
    {
        _isAttacking = true;
        _attackFinished = false;
        
        // Trigger animation
        if (animator != null && !string.IsNullOrEmpty(_currentAttack.animationTrigger))
        {
            animator.SetTrigger(_currentAttack.animationTrigger);
        }
        
        // Mark attack as used (for cooldown)
        _currentAttack.MarkAsUsed();
        
        Debug.Log("Boss performing melee attack");
    }
    
    private void DecideNextAction()
    {
        _isAttacking = false;
        
        float distance = GetDistanceToPlayer();
        
        // Decide next action based on distance and aggression
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
                // Select new attack
                SelectAndPerformAttack();
            }
            else
            {
                controller.ChangeState(new BossWalkState(controller));
            }
        }
    }
    
    private IEnumerator WaitAndRetry()
    {
        yield return new WaitForSeconds(0.5f);
        SelectAndPerformAttack();
    }
    
    // Called from BossController via Animation Event
    public void OnAttackStart()
    {
        if (_currentAttack == null || _currentAttack.weaponHitbox == null)
        {
            Debug.LogWarning("Cannot start attack - no hitbox assigned!");
            return;
        }
        
        // Enable hitbox
        _currentAttack.weaponHitbox.EnableHitbox();
        
        Debug.Log("Attack hitbox enabled");
    }
    
    // Called from BossController via Animation Event
    public void OnAttackEnd()
    {
        if (_currentAttack == null || _currentAttack.weaponHitbox == null)
        {
            Debug.LogWarning("Cannot end attack - no hitbox assigned!");
            return;
        }
        
        // Disable hitbox
        _currentAttack.weaponHitbox.DisableHitbox();
        
        Debug.Log("Attack hitbox disabled");
        
        // Mark attack as finished
        _attackFinished = true;
    }
    
    public override void OnExit()
    {
        base.OnExit();
        
        // Make sure hitbox is disabled when leaving state
        if (_currentAttack != null && _currentAttack.weaponHitbox != null)
        {
            _currentAttack.weaponHitbox.DisableHitbox();
        }
    }
}
