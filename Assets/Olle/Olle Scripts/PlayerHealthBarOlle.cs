using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerHealthBarOlle : MonoBehaviour
{
    [Header("Health Bar UI")]
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private Transform targetCamera;
    
    private PlayerHealthJavi playerHealth;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealthJavi>();
        if (playerHealth == null)
        {
            Debug.LogError("[PlayerHealthBarSlider] No PlayerHealthJavi found on this object!");
            return;
        }
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        playerHealth.OnHealthChanged.AddListener(UpdateHealthBar);
    }
    
    private void Start()
    {
        UpdateHealthBar(playerHealth.GetCurrentHealth(), playerHealth.GetMaxHealth());
    }

    private void LateUpdate()
    {
      
        if (targetCamera != null)
            transform.LookAt(targetCamera);
    }

    private void UpdateHealthBar(float current, float max)
    {
        if (playerHealthSlider != null)
        {
            playerHealthSlider.maxValue = max;
            playerHealthSlider.value = current;
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);
    }
}