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

        [Header("Stamina")]
        public float dashStaminaCost = 20f;

        PlayerController _controller;
        PlayerStamina _stamina;

        Vector2 _lastTapDir;      // last tap direction (cardinal)
        float _lastTapTime;
        bool _isDashing;
        float _dashTimer;
        float _originalMoveSpeed;

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

        // Called every time Move input changes by PlayerController.OnMove
        void HandleMoveInput(Vector2 moveInput)
        {
            // Treat a "tap" as the moment input becomes non‑zero from zero,
            // but since we only see the value, approximate: when magnitude is big enough and direction changed.
            if (moveInput == Vector2.zero)
                return;

            // Reduce to a pure up/down/left/right direction
            Vector2 dir = GetCardinalDirection(moveInput);
            if (dir == Vector2.zero)
                return;

            float now = Time.time;

            // Double tap: same direction as last tap, within time window
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
                return new Vector2(Mathf.Sign(input.x), 0f);   // left/right
            else
                return new Vector2(0f, Mathf.Sign(input.y));   // up/down
        }

        void TryStartDash(Vector2 dashDir)
        {
            if (_isDashing || _controller == null || _stamina == null)
                return;

            if (_stamina.currentStamina < dashStaminaCost)
            {
                Debug.Log("Not enough stamina to dash.");
                return;
            }

            _stamina.currentStamina -= dashStaminaCost;

            _isDashing = true;
            _controller.IsDashing = true;

            _dashTimer = dashDuration;
            _originalMoveSpeed = _controller.moveSpeed;
            _controller.moveSpeed = _originalMoveSpeed * dashSpeedMultiplier;

            // snap movement direction to dashDir so dash goes cleanly in that direction
            // (optional; if you want dash exactly in the tapped dir)
            Debug.Log($"DASH START dir={dashDir}, speed={_controller.moveSpeed}");
        }

        void Update()
        {
            if (!_isDashing)
                return;

            _dashTimer -= Time.deltaTime;
            if (_dashTimer <= 0f)
            {
                _isDashing = false;
                _controller.IsDashing = false;
                _controller.moveSpeed = _originalMoveSpeed;
                Debug.Log("DASH END");
            }
        }
    }
}
