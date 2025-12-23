using UnityEngine;

/// <summary>
/// Idle state - Boss stands still until activated
/// </summary>
public class BossIdleState : BossStateBase
{
    public BossIdleState(BossController controller) : base(controller)
    {
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        
        // Set animation to idle
        if (animator != null && !string.IsNullOrEmpty(config.speedParameter))
        {
            animator.SetFloat(config.speedParameter, 0f);
        }
        
        if (animator != null && !string.IsNullOrEmpty(config.inCombatParameter))
        {
            animator.SetBool(config.inCombatParameter, false);
        }
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        
        // Wait for activation
        if (controller.IsActive)
        {
            PerformActivationAction();
        }
    }
    
    private void PerformActivationAction()
    {
        // Do the first action based on config
        switch (config.activationBehavior)
        {
            case ActivationBehavior.JumpToPlayer:
                // Jump to player (Yule Cat style)
                controller.ChangeState(new BossJumpState(controller, controller.Player.position));
                break;
                
            case ActivationBehavior.RunToPlayer:
                // Start running to player
                controller.ChangeState(new BossRunState(controller));
                break;
                
            case ActivationBehavior.WalkToPlayer:
                // Start walking to player
                controller.ChangeState(new BossWalkState(controller));
                break;
                
            case ActivationBehavior.PerformAttack:
                // Attack immediately (ranged boss)
                controller.ChangeState(new BossAttackState(controller));
                break;
                
            case ActivationBehavior.Custom:
                // Do nothing, external control
                Debug.Log("Boss activated with Custom behavior - waiting for manual state change");
                break;
        }
    }
}
