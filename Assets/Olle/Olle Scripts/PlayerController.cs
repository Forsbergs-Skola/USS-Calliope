using UnityEngine;
using UnityEngine.InputSystem;

namespace Olle.Scripts
{
    public class PlayerController : MonoBehaviour
    {

        private const float ADRENALINE_DURATION = 10f;
        private const float HEALTH_PACK_RECOVER_AMOUNT = 10f;

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
        
        bool _dashing;
        Vector3 _dashVelocity;

        public bool IsCrouching => _isCrouching;
        public bool IsMoving => _inputDir.sqrMagnitude > 0.01f;
        public bool IsSprinting => _wantsToRun && _inputDir.sqrMagnitude > 0.01f && !_isCrouching;
        public bool IsDashing { get; set; }
        public System.Action<Vector2> OnMoveEvent;

        [SerializeField] private InventoryRuntimeData inventorySO;
        private PlayerPickupHandler pickupHandler = new PlayerPickupHandler();

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

        private void Start()
        {
            PlayerDataHandler pHandler = GetComponent<PlayerDataHandler>();
            PlayerData data = pHandler.RuntimeData.Value;
            Vector3 lastPos = data.LastPosition;
            if (lastPos != Vector3.zero)
            {

                gameObject.transform.position = lastPos;
            }
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

                if (TryGetPlayerData(out PlayerData pData))
                {
                    if (_isCrouching)
                    {
                        //Debug.Log("FOO");

                        pData.AddActiveStatusEffect(EnumPlayerStatusEffect.IN_STEALTH);
                    }
                    else
                    {
                        //Debug.Log("BAR");
                        pData.RemoveActiveStatusEffect(EnumPlayerStatusEffect.IN_STEALTH);
                    }
                }


                //ApplyCrouchState();
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
            /*
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (Bootstrapper.Instance != null)
                {
                    Bootstrapper.Instance.TogglePause();
                }
            }
            */

            if (Time.timeScale == 0f) return;

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

                    int layerMask = ~LayerMask.GetMask("Player");

                    if (Physics.Raycast(ray, out RaycastHit hit, 1000f, layerMask))
                    {
                        Vector3 lookDir = hit.point - transform.position;
                        lookDir.y = 0;

                        if (lookDir.sqrMagnitude > 0.1f)
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
        [Header("Collision")]
        public LayerMask wallMask = -1; // Walls layer

        void FixedUpdate()
        {

            if (Time.timeScale == 0f) return;

            if (_dashing)
            {
                _rb.linearVelocity = new Vector3(_dashVelocity.x, _rb.linearVelocity.y, _dashVelocity.z);
            }
            else if (_inputDir.sqrMagnitude > 0.0001f)
            {
                Vector3 targetVel = new Vector3(_inputDir.x * moveSpeed, _rb.linearVelocity.y, _inputDir.z * moveSpeed);
                _rb.linearVelocity = targetVel;
            }
            else
            {
                _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
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

        /*
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
       */
        
        public void StartDash(Vector2 dir, float speed)
        {
            _dashing = true;
            IsDashing = true;

            _dashVelocity = new Vector3(dir.x, 0f, dir.y).normalized * speed;
        }

        public void EndDash()
        {
            _dashing = false;
            IsDashing = false;

            _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
        }


        public void HandleConsumablePickup(string worldID, string catalogID, int qty)
        {
            pickupHandler.HandleConsumablePickup(inventorySO.Value, worldID, catalogID, qty);
        }

        public void TryDepleteAdrenaline()
        {
            if (pickupHandler.TryUseConsumable(inventorySO.Value, IDConstants.ADRENALINE, 1))
            {
                // pickuphandler depletes adrenaline
                UseAdrenaline();
                return;
            }
            Debug.Log("You got no adrenaline");
        }

        public void TryDepleteHealthPack()
        {
            if(pickupHandler.TryUseConsumable(inventorySO.Value, IDConstants.HEALTH_PACK, 1))
            {
                UseHealthPack();
            }
        }

        private void UseAdrenaline()
        {
            
            //Debug.Log("Player go fast!");

            // TODO: player observable behavior
            // whatever else happens...
            _stamina.adrenalineRushActive = true;

            StartCoroutine(AdrenalineCoroutine());
        }
        private void UseHealthPack()
        {
            //Debug.Log("USED A HEALTH PACK!!!!!");
            if (TryGetComponent<PlayerHealthJavi>(out PlayerHealthJavi javiHealth))
            {
                javiHealth.RecoverDamage(HEALTH_PACK_RECOVER_AMOUNT);
            }


        }

        private bool TryGetPlayerData(out PlayerData pdata)
        {
            if(TryGetComponent<PlayerDataHandler>(out PlayerDataHandler handler))
            {
                pdata = handler.RuntimeData.Value;
                return true;
            }
            pdata = null;
            return false;
        }

        private System.Collections.IEnumerator AdrenalineCoroutine()
        {

            float oldSpeed = moveSpeed;
            float oldRunSpeed = runMoveSpeed;

            // change stuff
            Debug.Log("ADRENALINE ON");
            // moveSpeed = 10f;
            // runMoveSpeed = 15f;
            // and whatever else we want to adjust...

            yield return new WaitForSeconds(ADRENALINE_DURATION);
            
            // change stuff back
            Debug.Log("ADRENALINE OFF");
            _stamina.adrenalineRushActive = false;
            moveSpeed = oldSpeed;
            runMoveSpeed = oldRunSpeed;
            _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
            // normalize everything else...
        }


    }
}
