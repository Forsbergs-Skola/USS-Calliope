using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public UnityEvent<float, float> OnHealthChanged = new UnityEvent<float, float>();
    public UnityEvent OnDeath = new UnityEvent();

    private void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log("Enemy took damage: " + damage);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath.Invoke();
        Destroy(gameObject);
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}