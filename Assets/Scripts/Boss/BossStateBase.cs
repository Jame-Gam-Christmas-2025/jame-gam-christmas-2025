using UnityEngine;

/// <summary>
/// Base class for all boss state machine states
/// </summary>
public abstract class BossStateBase
{
    protected BossController controller;
    protected Transform transform;
    protected Animator animator;
    protected BossConfig config;
    
    public BossStateBase(BossController controller)
    {
        this.controller = controller;
        this.transform = controller.transform;
        this.animator = controller.Animator;
        this.config = controller.Config;
    }
    
    /// <summary>
    /// Called when entering this state
    /// </summary>
    public virtual void OnEnter()
    {
    }
    
    /// <summary>
    /// Called every frame while in this state
    /// </summary>
    public virtual void OnUpdate()
    {
    }
    
    /// <summary>
    /// Called every fixed frame while in this state
    /// </summary>
    public virtual void OnFixedUpdate()
    {
    }
    
    /// <summary>
    /// Called when exiting this state
    /// </summary>
    public virtual void OnExit()
    {
    }
    
    /// <summary>
    /// Helper to get distance to player
    /// </summary>
    protected float GetDistanceToPlayer()
    {
        if (controller.Player == null) return Mathf.Infinity;
        return Vector3.Distance(transform.position, controller.Player.position);
    }
    
    /// <summary>
    /// Helper to get direction to player
    /// </summary>
    protected Vector3 GetDirectionToPlayer()
    {
        if (controller.Player == null) return Vector3.zero;
        return (controller.Player.position - transform.position).normalized;
    }
    
    /// <summary>
    /// Helper to rotate towards player
    /// </summary>
    protected void RotateTowardsPlayer(float rotationSpeed)
    {
        if (controller.Player == null) return;
        
        Vector3 direction = GetDirectionToPlayer();
        direction.y = 0; // Keep rotation only on Y axis
        
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
