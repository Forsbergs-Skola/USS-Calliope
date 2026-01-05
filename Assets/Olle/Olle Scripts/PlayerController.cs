using UnityEngine;
using UnityEngine.InputSystem;

namespace Olle.Scripts
{
    [RequireComponent(typeof(Rigidbody), typeof(PlayerAnimationController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Speeds")]
        public float moveSpeed = 5f;
        public float runMoveSpeed = 8f;
        public float crouchMoveSpeed = 2f;

        [Header("Crouch")]
        public float crouchScaleY = 0.5f;

        [Header("Footstep Noise")]
        public float walkStepInterval = 0.4f;
        public float runStepInterval = 0.25f;
        public float crouchStepInterval = 0.6f;

        [Header("Rotation")]
        [SerializeField] private float rotationSpeed = 15f;

        // Components
        private Rigidbody _rb;
        private PlayerAnimationController _anim;
        private NoiseEmitter _noise;
        private PlayerStamina _stamina;
        private CrouchInvisibility _crouchInvis;

        // Input & movement
        private Vector2 _moveInput;
        private Vector3 _inputDir;
        private bool _wantsToRun;
        public bool _isCrouching;
        private float _turnCooldown = 0.5f;
        private float _turnTimer = 0f;

        // Default values
        private float _defaultScaleY;
        private float _defaultMoveSpeed;

        // Rotation
        private Quaternion? _targetRotation;

        // Footstep noise timer
        private float _noiseTimer;

        // Public properties
        public bool IsCrouching => _isCrouching;
        public bool IsDashing { get; set; }
        public bool IsSprinting => _wantsToRun && _inputDir.sqrMagnitude > 0.01f && !_isCrouching && !IsDashing && (_stamina == null || !_stamina.isTired);
        public bool IsMoving => _inputDir.sqrMagnitude > 0.01f;

        public System.Action<Vector2> OnMoveEvent;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            _anim = GetComponent<PlayerAnimationController>();
            _noise = GetComponent<NoiseEmitter>();
            _stamina = GetComponent<PlayerStamina>();
            _crouchInvis = GetComponent<CrouchInvisibility>();

            _defaultScaleY = transform.localScale.y;
            _defaultMoveSpeed = moveSpeed;
        }

        #region Input Callbacks
        public void OnMove(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.performed ? ctx.ReadValue<Vector2>() : Vector2.zero;
            OnMoveEvent?.Invoke(_moveInput);
        }

        public void OnRun(InputAction.CallbackContext ctx)
        {
            _wantsToRun = ctx.ReadValue<float>() > 0.5f;
        }

        public void OnCrouch(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            _isCrouching = !_isCrouching;
            ApplyCrouchState();
        }

        public void OnInteract(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            float interactRadius = 1.5f;
            Vector3 origin = transform.position + transform.forward * 1f;

            foreach (var hit in Physics.OverlapSphere(origin, interactRadius))
            {
                var interactable = hit.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.Trigger(this);
                    break;
                }
            }
        }
        #endregion

        private void Update()
        {
            // Update input direction
            _inputDir = Vector3.ClampMagnitude(new Vector3(_moveInput.x, 0f, _moveInput.y), 1f);
            bool isMoving = IsMoving;

            // Handle stamina
            bool canSprint = false;
            if (_stamina != null)
            {
                bool blockRegen = _crouchInvis != null && _crouchInvis.IsInvisible;
                _stamina.Tick(Time.deltaTime, _wantsToRun && isMoving && !_isCrouching, blockRegen, out canSprint);
            }

            // Update move speed based on state
            moveSpeed = _isCrouching ? crouchMoveSpeed :
                        (!IsDashing ? (IsSprinting ? runMoveSpeed : _defaultMoveSpeed) : moveSpeed);

            HandleNoise(isMoving, Time.deltaTime);
            CalculateRotationTarget();

            _anim?.UpdateMovement(new Vector2(_inputDir.x, _inputDir.z), IsSprinting);
        }

        private void FixedUpdate()
        {
            if (_inputDir.sqrMagnitude > 0.0001f)
            {
                Vector3 move = transform.right * _inputDir.x + transform.forward * _inputDir.z;
                move = Vector3.ClampMagnitude(move, 1f);
                _rb.MovePosition(_rb.position + move * moveSpeed * Time.fixedDeltaTime);
            }

            if (_targetRotation.HasValue)
            {
                _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, _targetRotation.Value, rotationSpeed * Time.fixedDeltaTime));
            }
        }

        private void CalculateRotationTarget()
        {
            if (Camera.main == null || Mouse.current == null) return;

            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane groundPlane = new Plane(Vector3.up, transform.position);

            if (groundPlane.Raycast(ray, out float enter))
            {
                Vector3 targetPoint = ray.GetPoint(enter);
                Vector3 lookDir = targetPoint - transform.position;
                lookDir.y = 0f;

                if (lookDir.sqrMagnitude > 0.001f)
                    _targetRotation = Quaternion.LookRotation(lookDir);
            }
        }

        private void HandleNoise(bool isMoving, float deltaTime)
        {
            if (_noise == null) return;

            if (isMoving)
            {
                _noiseTimer += deltaTime;

                if (!_isCrouching && IsSprinting && _noiseTimer >= runStepInterval)
                {
                    _noise.EmitRun();
                    _noiseTimer = 0f;
                }
                else if (_isCrouching && _noiseTimer >= crouchStepInterval)
                {
                    _noise.EmitCrouch();
                    _noiseTimer = 0f;
                }
                else if (!_isCrouching && !IsSprinting && _noiseTimer >= walkStepInterval)
                {
                    _noise.EmitWalk();
                    _noiseTimer = 0f;
                }
            }
            else
            {
                _noiseTimer = 0f;
            }
        }

        private void ApplyCrouchState()
        {
            Vector3 scale = transform.localScale;
            scale.y = _isCrouching ? _defaultScaleY * crouchScaleY : _defaultScaleY;
            transform.localScale = scale;
        }
    }
}
