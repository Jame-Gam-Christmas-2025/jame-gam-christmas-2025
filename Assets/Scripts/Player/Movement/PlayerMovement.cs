using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 8f;
    [SerializeField] private float _rotationSpeed = 15f;

    [Header("Dodge Settings")]
    [SerializeField] private float _dodgeForce = 10f;


    [Header("Camera")]
    [SerializeField] private Transform _cameraTransform;

    [Header("References")]
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Animator _animator;

    [Header("MC_Footsteps")] 
    public AK.Wwise.Event MC_Footsteps;
    
    [Header("Switch Footsteps - Movement")]
    public AK.Wwise.Switch SW_Footsteps_Walk;
    public AK.Wwise.Switch SW_Footsteps_Run;
    
    [Header("Switch Footsteps - Surface")]
    public AK.Wwise.Switch SW_Surface_Concrete;
    
    private Rigidbody rb;
    private Vector2 _moveInput;
    private bool _isSprinting;
    private bool _isDodging;

    public void PlayFootstep()
    {
        // Sécurité : éviter les pas à l'arrêt
        if (_moveInput.magnitude < 0.1f)
            return;

        // MovementType
        if (_isSprinting)
            SW_Footsteps_Run.SetValue(gameObject);
        else
            SW_Footsteps_Walk.SetValue(gameObject);

        // SurfaceType (temporaire)
        SW_Surface_Concrete.SetValue(gameObject);

        // Event
        MC_Footsteps.Post(gameObject);
    }

 
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (_cameraTransform == null)
        {
            _cameraTransform = Camera.main.transform;
        }

        if (_playerInput == null)
        {
            _playerInput = GetComponent<PlayerInput>();
        }
    }

    private void FixedUpdate()
    {
        

        if (!_isDodging)
        {
            MoveCharacter();
            UpdateAnimator();
        }
    }

    public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnDodge(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed && !_isDodging)
        {
            StartDodge();
        }
    }

    public void OnSprint(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        _isSprinting = context.performed;
    }

    private void StartDodge()
    {
        _isDodging = true;

        Vector3 dodgeDirection = transform.forward;
        rb.linearVelocity = dodgeDirection * _dodgeForce;

        if (_animator != null)
        {
            _animator.SetTrigger("Dodge");
        }
    }

    // Called by Animation Event
    public void DisableInput()
    {
        if (_playerInput != null)
        {
            _playerInput.enabled = false;
        }
    }

    // Called by Animation Event
    public void EnableInput()
    {
        if (_playerInput != null)
        {
            _playerInput.enabled = true;
        }
        _isDodging = false;
    }

    private void MoveCharacter()
    {
        Vector3 moveDirection = CalculateMoveDirection(_moveInput);
        float currentSpeed = _isSprinting ? _sprintSpeed : _walkSpeed;
        Vector3 targetVelocity = moveDirection * currentSpeed;
        targetVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = targetVelocity;

        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
        }
    }

  

    private Vector3 CalculateMoveDirection(Vector2 input)
    {
        Vector3 cameraForward = _cameraTransform.forward;
        Vector3 cameraRight = _cameraTransform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        return (cameraForward * input.y + cameraRight * input.x).normalized;
    }

    private void UpdateAnimator()
    {
        if (_animator != null)
        {
            float speed = _moveInput.magnitude * (_isSprinting ? _sprintSpeed : _walkSpeed);
            _animator.SetFloat("Speed", speed);
            _animator.SetBool("IsSprinting", _isSprinting);
        }
    }
}