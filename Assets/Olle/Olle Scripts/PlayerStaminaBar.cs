using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaBar : MonoBehaviour
{
    [SerializeField] private Slider playerStaminaSlider;
    [SerializeField] [ColorUsage(true, true)] private Color normalColor = Color.green;
    [SerializeField] [ColorUsage(true, true)] private Color adrenalineColor = Color.gold;
    
    private PlayerStamina playerStamina;
    private Image fillImage;

    private void Awake()
    {
        playerStamina = GetComponent<PlayerStamina>();
        if (playerStamina == null)
        {
            return;
        }
        
        if (playerStaminaSlider != null)
        {
            fillImage = playerStaminaSlider.fillRect?.GetComponent<Image>();
            if (fillImage == null) ;
        }
    }
    
    private void LateUpdate()
    {
        if (playerStamina == null || playerStaminaSlider == null) return;
        
        playerStaminaSlider.maxValue = playerStamina.maxStamina;
        playerStaminaSlider.value = playerStamina.currentStamina;
        
        if (fillImage != null)
        {
            if (playerStamina.adrenalineRushActive)
            {
                fillImage.color = adrenalineColor;
                playerStaminaSlider.value = playerStamina.maxStamina;
            }
            else
            {
                fillImage.color = normalColor;
            }
        }
    }
}