using UnityEngine;
using UnityEngine.InputSystem;

namespace Olle.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float runMoveSpeed = 8f;
        public float crouchMoveSpeed = 2f;
        
        public float crouchScaleY = 0.5f;
        
        public float walkStepInterval   = 0.4f;
        public float runStepInterval    = 0.25f;
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
        
        public bool IsCrouching => _isCrouching;
        
        public bool IsDashing { get; set; }
        
        public System.Action<Vector2> OnMoveEvent;
        
        void Awake()
        {
            _rb = GetComponent<Rigidbody>();

            _defaultScaleY    = transform.localScale.y;
            _defaultMoveSpeed = moveSpeed;

            _noise        = GetComponent<NoiseEmitter>();
            _stamina      = GetComponent<PlayerStamina>();
            _crouchInvis  = GetComponent<CrouchInvisibility>();
        }

        //Move inputs
        public void OnMove(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.ReadValue<Vector2>();

            // Notify dash ability etc.
            OnMoveEvent?.Invoke(_moveInput);
        }
        
        //Run
        public void OnRun(InputAction.CallbackContext ctx)
        {
            _wantsToRun = ctx.ReadValue<float>() > 0.5f;
        }

        //Crouch
        public void OnCrouch(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                _isCrouching = !_isCrouching;
                ApplyCrouchState();
            }
        }

        //Interact
        public void OnInteract(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed)
                return;
            
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
            Vector3 move = new Vector3(_moveInput.x, 0f, _moveInput.y);
            move = Vector3.ClampMagnitude(move, 1f);
            _inputDir = move;

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

            if (_isCrouching)
            {
                moveSpeed = crouchMoveSpeed;
            }
            else if (!IsDashing)
            {
                if (canSprint)
                {
                    moveSpeed = runMoveSpeed;
                }
                else if (_stamina != null && _stamina.isTired)
                {
                    moveSpeed = 2f; // Tired Speed
                }
                else
                {
                    moveSpeed = _defaultMoveSpeed;
                }
            }

            // Noise
            if (isMoving && _noise != null)
            {
                _noiseTimer += Time.deltaTime;

                if (!_isCrouching && _wantsToRun)
                {
                    if (_noiseTimer >= runStepInterval)
                    {
                        _noise.EmitRun();
                        _noiseTimer = 0f;
                    }
                }
                else if (_isCrouching)
                {
                    if (_noiseTimer >= crouchStepInterval)
                    {
                        _noise.EmitCrouch();
                        _noiseTimer = 0f;
                    }
                }
                else
                {
                    if (_noiseTimer >= walkStepInterval)
                    {
                        _noise.EmitWalk();
                        _noiseTimer = 0f;
                    }
                }
            }
            else
            {
                _noiseTimer = 0f;
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
            
            if (Camera.main == null || Mouse.current == null)
                return;

            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                Vector3 lookPos = hit.point;
                lookPos.y = _rb.position.y;

                Vector3 lookDir = lookPos - _rb.position;
                if (lookDir.sqrMagnitude > 0.0001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(lookDir);
                    _rb.MoveRotation(targetRot);
                }
            }
        }
        
        void ApplyCrouchState()
        {
            Vector3 scale = transform.localScale;
            scale.y = _isCrouching ? _defaultScaleY * crouchScaleY : _defaultScaleY;
            transform.localScale = scale;
        }
    }
}
