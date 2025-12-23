using UnityEngine;

/// <summary>
/// Walk state - Boss walks towards the player at short-medium distance
/// </summary>
public class BossWalkState : BossStateBase
{
    private Rigidbody _rigidbody;

    public BossWalkState(BossController controller) : base(controller)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _rigidbody = controller.GetComponent<Rigidbody>();

        // Set walk animation
        if (animator != null && !string.IsNullOrEmpty(config.speedParameter))
        {
            animator.SetFloat(config.speedParameter, config.walkSpeed);
        }

        if (animator != null && !string.IsNullOrEmpty(config.inCombatParameter))
        {
            animator.SetBool(config.inCombatParameter, true);
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        float distance = GetDistanceToPlayer();

        // Check distance transitions
        if (distance <= config.attackThreshold)
        {
            // Close enough to attack
            controller.ChangeState(new BossAttackState(controller));
            return;
        }
        else if (distance > config.runThreshold)
        {
            // Too far, switch to running
            controller.ChangeState(new BossRunState(controller));
            return;
        }

        // Rotate towards player
        RotateTowardsPlayer(8f);
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        // Move using Rigidbody in FixedUpdate
        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        if (_rigidbody == null) return;

        Vector3 direction = GetDirectionToPlayer();
        Vector3 newPosition = _rigidbody.position + direction * config.walkSpeed * Time.fixedDeltaTime;

        _rigidbody.MovePosition(newPosition);
    }
}