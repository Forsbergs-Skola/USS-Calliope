using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarSlider : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private Slider enemyHealthSlider;
    public Transform targetCamera;

    private void Awake()
    {
        if (enemyHealth == null)
        {
            enemyHealth = GetComponentInParent<EnemyHealth>();
        }

        if (enemyHealth != null)
        {
            enemyHealth.OnHealthChanged.AddListener(UpdateHealthBar);
        }
    }

    private void Start()
    {
        UpdateHealthBar(enemyHealth.GetCurrentHealth(), enemyHealth.GetMaxHealth());
    }

    private void UpdateHealthBar(float current, float max)
    {
        enemyHealthSlider.value = current;
    }
    
    void LateUpdate()
    {
        // Makes the health bar always face the camera
        transform.LookAt(targetCamera);
        // transform.rotation = Quaternion.LookRotation(transform.position - targetCamera.position);
    }

    private void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);
        }
    }
}