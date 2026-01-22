using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace Olle.Scripts
{
    public class CrouchInvisibility : MonoBehaviour
    {
        [Header("Invisibility Settings")]
        [Range(0.1f, 5f)]
        public float crouchTimeToInvisible = 2f;
        
        [Range(1f, 30f)]
        public float invisibleDuration = 10f;
        
        [Range(0f, 0.5f)]
        public float invisibleAlpha = 0.25f;
        
        [Header("Stamina Drain")]
        public float staminaTickAmount = 30f;      
        public float staminaTickInterval = 2f;     

        [Header("Renderers to Affect")]
        public Renderer[] rendersToHide;
        
        [Header("UI Indicator")]
        [SerializeField] private Image invisIcon;
        [SerializeField] private CanvasGroup iconGroup;
        
        PlayerController _controller;
        PlayerStamina _stamina;

        float _crouchTimer;
        bool _isInvisible;
        float _invisibleTimer;
        float _staminaTickTimer;
        bool _invisUsedThisCrouch;

        public bool IsInvisible => _isInvisible;
        
        void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _stamina = GetComponent<PlayerStamina>();
            
            if (rendersToHide == null || rendersToHide.Length == 0)
                rendersToHide = GetComponentsInChildren<Renderer>();
            
            if (invisIcon != null && iconGroup == null)
                iconGroup = invisIcon.GetComponent<CanvasGroup>() ?? invisIcon.gameObject.AddComponent<CanvasGroup>();
            
            UpdateIcon();
        }
        
        void Update()
        {
            if (_controller == null) return;
    
            if (_controller.IsCrouching)
            {
                _crouchTimer += Time.deltaTime;

                bool hasStamina = _stamina == null || _stamina.currentStamina > 0f;
                bool notTired = _stamina == null || !_stamina.isTired;

                if (!_isInvisible && !_invisUsedThisCrouch &&
                    _crouchTimer >= crouchTimeToInvisible && hasStamina && notTired)
                {
                    _invisUsedThisCrouch = true;
                    SetInvisible(true);
                }
            }
            else
            {
                _crouchTimer = 0f;
                _invisUsedThisCrouch = false;

                if (_isInvisible)
                    SetInvisible(false);
            }
            
            if (_isInvisible)
            {
                HandleStaminaDrain();
        
                _invisibleTimer += Time.deltaTime;
                if (_invisibleTimer >= invisibleDuration)
                {
                    SetInvisible(false);
                    return;
                }
            }

            UpdateIcon();
        }

        
        void HandleStaminaDrain()
        {
            if (_stamina == null) return;

            if (_stamina.AdrenalineRushActive) return;

            _staminaTickTimer += Time.deltaTime;
            if (_staminaTickTimer < staminaTickInterval) return;

            _staminaTickTimer = 0f;
            
            if (_stamina.currentStamina <= 0f)
            {
                Debug.Log("INVIS OFF: stamina already 0");
                MakeTiredFromStealth();
                SetInvisible(false);
                return;
            }

            float before = _stamina.currentStamina;
            float after = Mathf.Max(0f, before - staminaTickAmount);
            _stamina.currentStamina = after;

            Debug.Log($"Invis drain: {after:F0}/{_stamina.maxStamina}");

            if (after <= 0f)
            {
                Debug.Log("INVIS OFF: stamina drained to 0!");
                MakeTiredFromStealth();
                SetInvisible(false);
            }
        }
        
        void MakeTiredFromStealth()
        {
            if (_stamina == null) return;

            _stamina.isTired = true;
            
            var type = typeof(PlayerStamina);
            var lastUseField = type.GetField("_lastUseTime",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            lastUseField?.SetValue(_stamina, Time.time);
        }
        
        void SetInvisible(bool value)
        {
            _isInvisible = value;
            _invisibleTimer = 0f;
            _staminaTickTimer = 0f;
            
            UpdateTransparency();
            UpdateIcon();
        }
        
        void UpdateTransparency()
        {
            float targetAlpha = _isInvisible ? invisibleAlpha : 1f;
            
            foreach (var r in rendersToHide)
            {
                if (r == null) continue;
                
                var block = new MaterialPropertyBlock();
                block.SetColor("_BaseColor", new Color(1, 1, 1, targetAlpha));
                block.SetColor("_Color", new Color(1, 1, 1, targetAlpha));
                block.SetFloat("_Mode", _isInvisible ? 3f : 0f);
                block.SetFloat("_SrcBlend", 10f);
                block.SetFloat("_DstBlend", 10f);
                block.SetFloat("_ZWrite", _isInvisible ? 0f : 1f);
                
                r.SetPropertyBlock(block);
            }
        }
        
        void UpdateIcon()
        {
            if (iconGroup != null)
            {
                iconGroup.alpha = _isInvisible ? 1f : 0f;
                iconGroup.interactable = _isInvisible;
                iconGroup.blocksRaycasts = false;
            }
        }
    }
}
