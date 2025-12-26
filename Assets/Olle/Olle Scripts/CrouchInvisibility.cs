using UnityEngine;

namespace Olle.Scripts
{
    public class CrouchInvisibility : MonoBehaviour
    {
        public float crouchTimeToInvisible = 2f;
        public float invisibleDuration = 10f;
        public Renderer[] rendersToHide;
        
        public float staminaTickAmount = 30f;      // drain 30
        public float staminaTickInterval = 2f;     // every 2s

        PlayerController _controller;
        PlayerStamina _stamina;

        float _crouchTimer;
        bool  _isInvisible;
        float _invisibleTimer;
        float _staminaTickTimer;

        bool _invisUsedThisCrouch;

        public bool IsInvisible => _isInvisible;

        // Setup
        void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _stamina    = GetComponent<PlayerStamina>();

            if (rendersToHide == null || rendersToHide.Length == 0)
                rendersToHide = GetComponentsInChildren<Renderer>();
        }

        // Main update
        void Update()
        {
            if (_controller == null)
                return;

            // ===== ENTER INVISIBILITY ONCE PER CROUCH =====
            if (_controller.IsCrouching)
            {
                _crouchTimer += Time.deltaTime;

                bool hasStamina = _stamina == null || _stamina.currentStamina > 0f;
                bool notTired   = _stamina == null || !_stamina.isTired;

                if (!_isInvisible &&
                    !_invisUsedThisCrouch &&
                    _crouchTimer >= crouchTimeToInvisible &&
                    hasStamina &&
                    notTired)
                {
                    _invisUsedThisCrouch = true;
                    SetInvisible(true);
                }
            }
            else
            {
                // Standing up: reset and force visible
                _crouchTimer = 0f;
                _invisUsedThisCrouch = false;

                if (_isInvisible)
                    SetInvisible(false);
            }

            // ===== WHILE INVISIBLE =====
            if (_isInvisible)
            {
                _invisibleTimer += Time.deltaTime;
                if (_invisibleTimer >= invisibleDuration)
                {
                    SetInvisible(false);
                    return;
                }

                HandleStaminaDrain();
            }
        }

        // Drain stamina
        void HandleStaminaDrain()
        {
            if (_stamina == null)
                return;

            _staminaTickTimer += Time.deltaTime;

            if (_staminaTickTimer < staminaTickInterval)
                return;

            _staminaTickTimer = 0f;

            // If already 0, force off and tired
            if (_stamina.currentStamina <= 0f)
            {
                Debug.Log("Invis drain: stamina already 0, off + tired");
                MakeTiredFromStealth();
                SetInvisible(false);
                return;
            }

            float before = _stamina.currentStamina;
            float after  = Mathf.Max(0f, before - staminaTickAmount);

            _stamina.currentStamina = after;
            Debug.Log($"Invisibility stamina drain: {_stamina.currentStamina}/{_stamina.maxStamina}");

            if (after <= 0f)
            {
                Debug.Log("Invis drain hit 0, off + tired");
                MakeTiredFromStealth();
                SetInvisible(false);
            }
        }

        // Mark tired
        void MakeTiredFromStealth()
        {
            if (_stamina == null)
                return;

            _stamina.isTired = true;
            // Regen delay should start from now
            var type = typeof(PlayerStamina);
            var lastUseField = type.GetField("_lastUseTime",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (lastUseField != null)
                lastUseField.SetValue(_stamina, Time.time);
        }

        // Toggle invis
        void SetInvisible(bool value)
        {
            _isInvisible = value;
            _invisibleTimer = 0f;
            _staminaTickTimer = 0f;

            Debug.Log($"SetInvisible({value}) called");
            UpdateTransparency();
        }

        // Change alpha
        void UpdateTransparency()
        {
            float targetAlpha = _isInvisible ? 0.25f : 1f;
            Debug.Log($"UpdateTransparency: invisible={_isInvisible}, targetAlpha={targetAlpha}");

            foreach (var r in rendersToHide)
            {
                if (r == null) continue;

                Color c = r.material.color;
                c.a = targetAlpha;
                r.material.color = c;
            }
        }
    }
}
