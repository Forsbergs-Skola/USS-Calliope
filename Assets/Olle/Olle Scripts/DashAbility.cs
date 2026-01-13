using UnityEngine;

namespace Olle.Scripts
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(PlayerStamina))]
    public class DashAbility : MonoBehaviour
    {
        public float dashSpeedMultiplier = 3f;
        public float dashDuration = 0.2f;
        public float doubleTapWindow = 0.25f;
        public float maxTapHoldTime = 0.15f; 

        public float dashStaminaCost = 20f;

        PlayerController _controller;
        PlayerStamina _stamina;

        bool _isDashing;
        float _dashTimer;
        
        Vector2 _currentDir;
        bool _isKeyDown;
        float _keyDownTime;

        Vector2 _lastTapDir;
        float _lastTapTime;
        
        void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _stamina    = GetComponent<PlayerStamina>();
        }
        
        void OnEnable()
        {
            _controller.OnMoveEvent += HandleMoveInput;
        }
        
        void OnDisable()
        {
            _controller.OnMoveEvent -= HandleMoveInput;
        }
        
        void HandleMoveInput(Vector2 moveInput)
        {
            if (!_isKeyDown && moveInput != Vector2.zero)
            {
                _isKeyDown = true;
                _keyDownTime = Time.time;
                _currentDir = GetCardinalDirection(moveInput);
            }
            else if (_isKeyDown && moveInput == Vector2.zero)
            {
                float held = Time.time - _keyDownTime;
                if (held <= maxTapHoldTime && _currentDir != Vector2.zero)
                    RegisterTap(_currentDir);

                _isKeyDown = false;
                _currentDir = Vector2.zero;
            }
        }
        
        void RegisterTap(Vector2 dir)
        {
            float now = Time.time;
            
            if (Vector2.Dot(dir, _lastTapDir) > 0.99f &&
                now - _lastTapTime <= doubleTapWindow)
            {
                TryStartDash(dir);
            }

            _lastTapDir = dir;
            _lastTapTime = now;
        }
        
        Vector2 GetCardinalDirection(Vector2 input)
        {
            if (input.sqrMagnitude < 0.1f)
                return Vector2.zero;

            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                return new Vector2(Mathf.Sign(input.x), 0f); 
            else
                return new Vector2(0f, Mathf.Sign(input.y));
        }
        
        void TryStartDash(Vector2 dashDir)
        {
            if (_isDashing || _controller == null || _stamina == null)
                return;

            if (_stamina.isTired || _stamina.currentStamina < dashStaminaCost)
                return;

            _stamina.currentStamina -= dashStaminaCost;

            _isDashing = true;
            _controller.IsDashing = true;

            _dashTimer = dashDuration;
            
            float dashSpeed = _controller.moveSpeed * dashSpeedMultiplier;
            _controller.StartDash(dashDir, dashSpeed);

            Debug.Log("DASH START dir=" + dashDir);
        }

        
        void Update()
        {
            if (!_isDashing)
                return;

            _dashTimer -= Time.deltaTime;
            if (_dashTimer <= 0f)
            {
                _isDashing = false;
                
                if (_controller != null)
                    _controller.EndDash();

                _controller.IsDashing = false;
            }
        }
    }
}
