using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerHealthBarSlider : MonoBehaviour
{
    [SerializeField] private PlayerHealthJavi playerHealth;
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private Renderer playerRenderer;
    
    public Transform targetCamera;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (playerHealth == null)
        {
            playerHealth = GetComponentInParent<PlayerHealthJavi>();
        }
        if (playerRenderer == null)
            playerRenderer = GetComponentInParent<Renderer>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.AddListener(UpdateHealthBar);
        }
    }
    
    private void LateUpdate()
    {
        // Makes the health bar always face the camera
        //transform.LookAt(targetCamera);
        // transform.rotation = Quaternion.LookRotation(transform.position - targetCamera.position);
        if (targetCamera != null)
            transform.LookAt(targetCamera);

        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (playerRenderer == null)
            return;
        
        // If enemy is not visible, hide the health bar
        bool isRendererActive = playerRenderer.enabled;
        canvasGroup.alpha = isRendererActive ? 1f : 0f;
    }

    private void Start()
    {
        UpdateHealthBar(playerHealth.GetCurrentHealth(), playerHealth.GetMaxHealth());
    }

    private void UpdateHealthBar(float current, float max)
    {
        playerHealthSlider.maxValue = max;
        playerHealthSlider.value = current;

    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);
        }
    }
    
}
