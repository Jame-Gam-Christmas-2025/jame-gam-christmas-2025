using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _walkSpeed = 2.5f;
    [SerializeField] private float _sprintSpeed = 8f;
    [SerializeField] private float _rotationSpeed = 15f;

    [Header("Dodge Settings")]
    [SerializeField] private float _dodgeForce = 10f;


    [Header("Camera")]
    [SerializeField] private Transform _cameraTransform;

    [Header("References")]
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Animator _animator;

    [Header("MC_Presences_Walk")] 
    public AK.Wwise.Event MC_Presences_Walk;
    
    [Header("MC_Presences_Action")] 
    public AK.Wwise.Event MC_Presences_Action;
    
    [Header("FOL_MC_Roll")] 
    public AK.Wwise.Event PlayFOL_MC_Roll;
    
    [Header("MC_Footsteps")] 
    public AK.Wwise.Event MC_Footsteps;
    
    [Header("Switch Footsteps - Movement")]
    public AK.Wwise.Switch SW_Footsteps_Walk;
    public AK.Wwise.Switch SW_Footsteps_Run;
    
    [Header("Switch Footsteps - Surface")]
    public AK.Wwise.Switch SW_Surface_Concrete;
    
    private Rigidbody _rb;
    private Vector2 _moveInput;
    private bool _isSprinting;
    private bool _isDodging;
    private bool _canMove = true;
    private bool _canDodge = true;

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

    public void PlayPresences_Walk()
    {
        MC_Presences_Walk.Post(gameObject);
    }
    
    public void PlayPresences_Action()
    {
        MC_Presences_Action.Post(gameObject);
    }
    public void FOL_MC_Roll()
    {
        PlayFOL_MC_Roll.Post(gameObject);
    }
 
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        if (_cameraTransform == null)
        {
            _cameraTransform = Camera.main.transform;
        }

        if (_playerInput == null)
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        // I know it has nothing to do here but CRUNCH!!!!
        GameObject fadeGameObject = GameObject.FindGameObjectWithTag("Fade");

        if(fadeGameObject != null)
        {
            CanvasGroup canvasGroup = fadeGameObject.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            canvasGroup.DOFade(0f, 4f);
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

    public void EnableMovement()
    {
        _canMove = true;
    }

    public void DisableMovement()
    {
        _canMove = false;
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
        // Stop method if the player is not able to move
        if (!_canDodge) return;

        _isDodging = true;
        GetComponent<PlayerCombatController>().IsAttacking = false;

        Vector3 dodgeDirection = transform.forward;
        _rb.linearVelocity = dodgeDirection * _dodgeForce;

        if (_animator != null)
        {
            _animator.SetTrigger("Dodge");
        }
    }

    // Called by Animation Event
    public void DisableInput()
    {
        Debug.Log("DisableInput");


        if (_playerInput != null)
        {
            _playerInput.enabled = false;
        }
    }

    // Called by Animation Event
    public void EnableInput()
    {
        Debug.Log("EnableInput");

        if (_playerInput != null)
        {
            _playerInput.enabled = true;
        }
        _isDodging = false;
    }

    private void MoveCharacter()
    {
        Vector3 moveDirection = CalculateMoveDirection(_moveInput);

        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            _rb.rotation = Quaternion.Slerp(_rb.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
        }

        // Reset velocity to 0 without overriding _moveInput
        if (!_canMove)
        {
            if(!_rb.linearVelocity.Equals(new Vector3(0, 0, 0)))
            {
                _rb.linearVelocity = new Vector3(0, 0, 0);
            }
            return;
        }

        float currentSpeed = _isSprinting ? _sprintSpeed : _walkSpeed;
        Vector3 targetVelocity = moveDirection * currentSpeed;
        targetVelocity.y = _rb.linearVelocity.y;
        _rb.linearVelocity = targetVelocity;

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
            if(_canMove)
            {
                float speed = _moveInput.magnitude * (_isSprinting ? _sprintSpeed : _walkSpeed);
                _animator.SetFloat("Speed", speed);
                _animator.SetBool("IsSprinting", _isSprinting);
            }
        }
    }
}