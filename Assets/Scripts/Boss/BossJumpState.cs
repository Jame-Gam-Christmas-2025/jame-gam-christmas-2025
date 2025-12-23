using UnityEngine;

/// <summary>
/// Jump state - Boss jumps to a target position using Rigidbody physics
/// </summary>
public class BossJumpState : BossStateBase
{
    private Vector3 _targetPosition;
    private Vector3 _startPosition;
    private float _jumpTimer;
    private float _jumpDuration;
    private float _jumpHeight;
    private bool _hasLanded;
    private Rigidbody _rigidbody;

    public BossJumpState(BossController controller, Vector3 targetPosition) : base(controller)
    {
        _targetPosition = targetPosition;
        _hasLanded = false;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _rigidbody = controller.GetComponent<Rigidbody>();

        if (_rigidbody == null)
        {
            Debug.LogError("Boss needs Rigidbody for jump!");
            controller.ChangeState(new BossRunState(controller));
            return;
        }

        _startPosition = transform.position;
        _jumpTimer = 0f;
        _jumpDuration = config.jumpDuration;
        _jumpHeight = config.jumpHeight;
        _hasLanded = false;

        // Disable gravity during jump
        _rigidbody.useGravity = false;

        // Trigger jump animation
        if (animator != null && !string.IsNullOrEmpty(config.jumpTrigger))
        {
            animator.SetTrigger(config.jumpTrigger);
        }

        // Mark jump as used (for cooldown tracking)
        controller.MarkJumpUsed();

        Debug.Log($"Boss jumping from {_startPosition} to {_targetPosition}");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _jumpTimer += Time.deltaTime;
        float progress = _jumpTimer / _jumpDuration;

        if (progress < 1f)
        {
            // Calculate parabolic jump trajectory
            Vector3 horizontalPosition = Vector3.Lerp(_startPosition, _targetPosition, progress);

            // Parabolic height: starts at 0, peaks at 0.5, ends at 0
            float heightProgress = 4f * progress * (1f - progress); // Parabola formula
            float currentHeight = heightProgress * _jumpHeight;

            Vector3 newPosition = horizontalPosition + Vector3.up * currentHeight;

            // Use Rigidbody.MovePosition for physics-based movement
            _rigidbody.MovePosition(newPosition);

            // Rotate towards target
            Vector3 direction = (_targetPosition - _startPosition).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                _rigidbody.MoveRotation(targetRotation);
            }
        }
        else if (!_hasLanded)
        {
            // Landing
            _rigidbody.MovePosition(_targetPosition);
            _hasLanded = true;

            // Re-enable gravity
            _rigidbody.useGravity = true;

            // Deal landing damage
            DealLandingDamage();

            // Transition to combat after landing
            controller.StartCoroutine(TransitionAfterLanding());
        }
    }

    private void DealLandingDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position,
            config.jumpLandingRadius
        );

        foreach (var hitCollider in hitColliders)
        {
            IDamageable damageable = hitCollider.GetComponent<IDamageable>();

            if (damageable != null && hitCollider.gameObject != controller.gameObject)
            {
                damageable.TakeDamage(config.jumpLandingDamage);
                Debug.Log($"Jump landing damage dealt to {hitCollider.name}");
            }
        }
    }

    private System.Collections.IEnumerator TransitionAfterLanding()
    {
        // Small delay for landing
        yield return new WaitForSeconds(0.3f);

        float distance = GetDistanceToPlayer();

        // Decide next state based on distance
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
            controller.ChangeState(new BossAttackState(controller));
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        // Make sure gravity is re-enabled
        if (_rigidbody != null)
        {
            _rigidbody.useGravity = true;
        }
    }
}