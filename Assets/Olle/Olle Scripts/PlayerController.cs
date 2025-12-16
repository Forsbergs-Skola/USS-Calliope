using UnityEngine;
using UnityEngine.InputSystem;

namespace Olle.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")] 
        public float moveSpeed = 5f;
        public float runMoveSpeed = 8f;
        public float crouchMoveSpeed = 2f;

        [Header("Crouch")] 
        public float crouchScaleY = 0.5f;

        [Header("Noise")] 
        public float walkStepInterval   = 0.4f;
        public float runStepInterval    = 0.25f;
        public float crouchStepInterval = 0.6f;

        Rigidbody _rb;
        Vector3 _inputDir;
        Vector2 _moveInput;
        bool _wantsToRun;
        bool _isCrouching;

        float _defaultScaleY;
        float _defaultMoveSpeed;

        NoiseEmitter _noise;
        float _noiseTimer;

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();

            _defaultScaleY    = transform.localScale.y;
            _defaultMoveSpeed = moveSpeed;

            _noise = GetComponent<NoiseEmitter>();
        }
        
        public void OnMove(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.ReadValue<Vector2>();
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

        void Update()
        {
            Vector3 move = new Vector3(_moveInput.x, 0f, _moveInput.y);
            move = Vector3.ClampMagnitude(move, 1f);
            _inputDir = move;

            bool isMoving = _inputDir.sqrMagnitude > 0.01f;
            
            if (!_isCrouching && _wantsToRun && isMoving)
                moveSpeed = runMoveSpeed;
            else if (_isCrouching)
                moveSpeed = crouchMoveSpeed;
            else
                moveSpeed = _defaultMoveSpeed;

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
            // Movement
            if (_inputDir.sqrMagnitude > 0.0001f)
            {
                float step = moveSpeed * Time.fixedDeltaTime;
                Vector3 targetPos = _rb.position + _inputDir * step;
                _rb.MovePosition(targetPos);
            }

            // Face mouse
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

        void ApplyCrouchState()
        {
            Vector3 scale = transform.localScale;
            scale.y = _isCrouching ? _defaultScaleY * crouchScaleY : _defaultScaleY;
            transform.localScale = scale;
        }
    }
}
