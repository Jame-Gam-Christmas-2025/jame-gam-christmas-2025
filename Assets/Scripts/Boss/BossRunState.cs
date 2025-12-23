using UnityEngine;

/// <summary>
/// Run state - Boss runs towards the player when at medium-long distance
/// </summary>
public class BossRunState : BossStateBase
{
    private float _decisionTimer;
    private Rigidbody _rigidbody;

    public BossRunState(BossController controller) : base(controller)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _rigidbody = controller.GetComponent<Rigidbody>();
        _decisionTimer = 0f;

        // Set run animation
        if (animator != null && !string.IsNullOrEmpty(config.speedParameter))
        {
            animator.SetFloat(config.speedParameter, config.runSpeed);
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

        // Check if should switch to jump
        _decisionTimer += Time.deltaTime;
        if (_decisionTimer >= config.decisionInterval)
        {
            _decisionTimer = 0f;

            // Consider jumping
            if (config.canJumpDuringCombat &&
                !controller.IsJumpOnCooldown() &&
                distance >= config.jumpMinDistance &&
                distance <= config.jumpMaxDistance)
            {
                // Random chance to jump
                if (Random.Range(0f, 100f) < config.jumpChance)
                {
                    controller.ChangeState(new BossJumpState(controller, controller.Player.position));
                    return;
                }
            }
        }

        // Check distance transitions
        if (distance <= config.attackThreshold)
        {
            // Close enough to attack
            controller.ChangeState(new BossAttackState(controller));
            return;
        }
        else if (distance <= config.runThreshold)
        {
            // Switch to walking
            controller.ChangeState(new BossWalkState(controller));
            return;
        }

        // Rotate towards player
        RotateTowardsPlayer(10f);
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
        Vector3 newPosition = _rigidbody.position + direction * config.runSpeed * Time.fixedDeltaTime;

        _rigidbody.MovePosition(newPosition);
    }
}