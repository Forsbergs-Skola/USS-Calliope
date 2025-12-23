using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarSlider : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private Slider enemyHealthSlider;
    [SerializeField] private Renderer enemyRenderer;
    
    public Transform targetCamera;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (enemyHealth == null)
        {
            enemyHealth = GetComponentInParent<EnemyHealth>();
        }
        if (enemyRenderer == null)
            enemyRenderer = GetComponentInParent<Renderer>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (enemyHealth != null)
        {
            enemyHealth.OnHealthChanged.AddListener(UpdateHealthBar);
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
        if (enemyRenderer == null)
            return;
        
        // If enemy is not visible, hide the health bar
        bool isRendererActive = enemyRenderer.enabled;
        canvasGroup.alpha = isRendererActive ? 1f : 0f;
    }

    private void Start()
    {
        UpdateHealthBar(enemyHealth.GetCurrentHealth(), enemyHealth.GetMaxHealth());
    }

    private void UpdateHealthBar(float current, float max)
    {
        enemyHealthSlider.maxValue = max;
        enemyHealthSlider.value = current;

    }

    private void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);
        }
    }
}