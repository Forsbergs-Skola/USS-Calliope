using UnityEngine;
using UnityEngine.Events;

public class BossHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 300f;
    private float currentHealth;

    public UnityEvent<float> OnDamageTaken;
    public UnityEvent OnDeath;
    
    public UnityEvent<float, float> OnHealthChanged = new UnityEvent<float, float>();
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnDamageTaken?.Invoke(amount);

        if (currentHealth <= 0)
            Die();
    }
    
    private void Die()
    {
        OnDeath.Invoke();
        Destroy(gameObject);
    }
}