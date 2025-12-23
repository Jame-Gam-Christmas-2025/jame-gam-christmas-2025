using UnityEngine;

/// <summary>
/// Death/Lie Down state - Boss has died and remains in scene for interaction
/// </summary>
public class BossDeathState : BossStateBase
{
    public BossDeathState(BossController controller) : base(controller)
    {
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        
        // Stop all movement
        if (animator != null && !string.IsNullOrEmpty(config.speedParameter))
        {
            animator.SetFloat(config.speedParameter, 0f);
        }
        
        // Set combat to false
        if (animator != null && !string.IsNullOrEmpty(config.inCombatParameter))
        {
            animator.SetBool(config.inCombatParameter, false);
        }
        
        // Trigger death animation
        if (animator != null && !string.IsNullOrEmpty(config.deathTrigger))
        {
            animator.SetTrigger(config.deathTrigger);
        }
        
        // Disable any active hitboxes
        DisableAllHitboxes();
        
        // Optionally disable colliders for ragdoll effect or to prevent blocking
        // Uncomment if needed:
        // DisableColliders();
        
        Debug.Log($"Boss {controller.gameObject.name} has entered death state and will remain in scene");
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        
        // Boss stays in this state permanently
        // No transitions from death state
    }
    
    private void DisableAllHitboxes()
    {
        if (config.attacks == null) return;
        
        foreach (var attack in config.attacks)
        {
            if (attack.weaponHitbox != null)
            {
                attack.weaponHitbox.DisableHitbox();
            }
        }
    }
    
    private void DisableColliders()
    {
        // Disable colliders if you want the boss body to not block movement
        Collider[] colliders = controller.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            // Don't disable trigger colliders (in case you need them for interaction)
            if (!col.isTrigger)
            {
                col.enabled = false;
            }
        }
    }
    
    public override void OnExit()
    {
        base.OnExit();
        // Death state is permanent, should never exit
    }
}
