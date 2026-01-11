using UnityEngine;
using UnityEngine.InputSystem;

namespace Olle.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float runMoveSpeed = 8f;
        public float crouchMoveSpeed = 2f;
        public float rotationSpeed = 10f;

        [Header("Rotation Style")]
        [Tooltip("True: Face mouse smoothly always. False: Face movement direction when not aiming (Old Style).")]
        public bool mouse = true;

        [Header("Crouch Settings")]
        public float crouchScaleY = 0.5f;

        [Header("Footstep Intervals")]
        public float walkStepInterval = 0.4f;
        public float runStepInterval = 0.25f;
        public float crouchStepInterval = 0.6f;

        Rigidbody _rb;
        Vector3 _inputDir;
        Vector2 _moveInput;
        bool _wantsToRun;
        public bool _isCrouching;

        float _defaultScaleY;
        float _defaultMoveSpeed;

        NoiseEmitter _noise;
        float _noiseTimer;

        PlayerStamina _stamina;
        CrouchInvisibility _crouchInvis;
        PlayerAimController _aimController;
        PlayerAnimationController _animatorControl;

        public bool IsCrouching => _isCrouching;
        public bool IsMoving => _inputDir.sqrMagnitude > 0.01f;
        public bool IsSprinting => _wantsToRun && _inputDir.sqrMagnitude > 0.01f && !_isCrouching;
        public bool IsDashing { get; set; }
        public System.Action<Vector2> OnMoveEvent;

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();

            _defaultScaleY = transform.localScale.y;
            _defaultMoveSpeed = moveSpeed;

            _noise = GetComponent<NoiseEmitter>();
            _stamina = GetComponent<PlayerStamina>();
            _crouchInvis = GetComponent<CrouchInvisibility>();
            _aimController = GetComponent<PlayerAimController>();
            _animatorControl = GetComponent<PlayerAnimationController>();
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.ReadValue<Vector2>();
            OnMoveEvent?.Invoke(_moveInput);
        }

        public void OnRun(InputAction.CallbackContext ctx)
        {
            _wantsToRun = ctx.ReadValue<float>() > 0.5f;
        }

        public void OnCrouch(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                _isCrouching = !_isCrouching;
                ApplyCrouchState();
            }
        }

        public void OnInteract(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            float interactRadius = 1.5f;
            Vector3 origin = transform.position + transform.forward * 1f;

            Collider[] hits = Physics.OverlapSphere(origin, interactRadius);
            foreach (Collider hit in hits)
            {
                var interactable = hit.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.Trigger(this);
                    break;
                }
            }
        }

        void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                TogglePause();
            }

            Vector3 move = new Vector3(_moveInput.x, 0f, _moveInput.y);
            _inputDir = Vector3.ClampMagnitude(move, 1f);

            bool isMoving = _inputDir.sqrMagnitude > 0.01f;

            bool canSprint = false;
            if (_stamina != null)
            {
                bool blockRegen = _crouchInvis != null && _crouchInvis.IsInvisible;
                _stamina.Tick(Time.deltaTime,
                              _wantsToRun && isMoving && !_isCrouching,
                              blockRegen,
                              out canSprint);
            }

            if (_isCrouching) moveSpeed = crouchMoveSpeed;
            else if (!IsDashing)
            {
                if (canSprint) moveSpeed = runMoveSpeed;
                else if (_stamina != null && _stamina.isTired) moveSpeed = 2f;
                else moveSpeed = _defaultMoveSpeed;
            }

            HandleNoise(isMoving, canSprint);

            if (_animatorControl != null)
            {
                bool isActuallySprinting = canSprint && isMoving && !_isCrouching;
                _animatorControl.UpdateMovement(_moveInput, isActuallySprinting);
            }

            HandleRotation();
        }

        private void HandleRotation()
        {
            bool isAiming = _aimController != null && _aimController.IsAiming;
            Quaternion targetRot = transform.rotation;
            bool shouldRotate = false;

            if (mouse || isAiming)
            {
                if (Camera.main != null && Mouse.current != null)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                    if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
                    {
                        Vector3 lookDir = hit.point - transform.position;
                        lookDir.y = 0;
                        if (lookDir.sqrMagnitude > 0.001f)
                        {
                            targetRot = Quaternion.LookRotation(lookDir);
                            shouldRotate = true;
                        }
                    }
                }
            }
            else if (_inputDir.sqrMagnitude > 0.01f)
            {
                targetRot = Quaternion.LookRotation(_inputDir);
                shouldRotate = true;
            }

            if (shouldRotate)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }

        void FixedUpdate()
        {
            if (_inputDir.sqrMagnitude > 0.0001f)
            {
                float step = moveSpeed * Time.fixedDeltaTime;
                Vector3 targetPos = _rb.position + _inputDir * step;
                _rb.MovePosition(targetPos);
            }

            _rb.MoveRotation(transform.rotation);
        }

        private void HandleNoise(bool isMoving, bool canSprint)
        {
            if (isMoving && _noise != null)
            {
                _noiseTimer += Time.deltaTime;
                if (!_isCrouching && _wantsToRun && _noiseTimer >= runStepInterval)
                {
                    _noise.EmitRun();
                    _noiseTimer = 0f;
                }
                else if (_isCrouching && _noiseTimer >= crouchStepInterval)
                {
                    _noise.EmitCrouch();
                    _noiseTimer = 0f;
                }
                else if (!_isCrouching && !_wantsToRun && _noiseTimer >= walkStepInterval)
                {
                    _noise.EmitWalk();
                    _noiseTimer = 0f;
                }
            }
            else _noiseTimer = 0f;
        }

        void ApplyCrouchState()
        {
            Vector3 scale = transform.localScale;
            scale.y = _isCrouching ? _defaultScaleY * crouchScaleY : _defaultScaleY;
            transform.localScale = scale;
        }

        public void TogglePause()
        {
            if (!UIController.Instance.GetIsCanvasUp(EnumCanvasUIName.PAUSE))
                UIController.Instance.ShowCanvas(EnumCanvasUIName.PAUSE);
            else
                UIController.Instance.RemoveCanvas(EnumCanvasUIName.PAUSE);
        }
    }
}