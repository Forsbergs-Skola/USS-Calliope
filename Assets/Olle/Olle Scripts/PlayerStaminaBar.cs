using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaBar : MonoBehaviour
{
    [SerializeField] private Slider playerStaminaSlider;
    
    private PlayerStamina playerStamina;

    private void Awake()
    {
        playerStamina = GetComponent<PlayerStamina>();
        if (playerStamina == null)
        {
            Debug.LogError("[PlayerStaminaBar] No PlayerStamina found");
            return;
        }
    }
    private void LateUpdate()
    {
        if (playerStaminaSlider != null && playerStamina != null)
        {
            playerStaminaSlider.maxValue = playerStamina.maxStamina;
            playerStaminaSlider.value = playerStamina.currentStamina;
        }
    }
}