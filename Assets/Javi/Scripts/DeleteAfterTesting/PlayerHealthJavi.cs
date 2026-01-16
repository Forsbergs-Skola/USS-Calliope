using UnityEngine;
using UnityEngine.Events;
using System;

public class PlayerHealthJavi : MonoBehaviour, IDamageable, IDamageEvents
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    
    public UnityEvent<float, float> OnHealthChanged = new UnityEvent<float, float>();
    public UnityEvent OnDeath = new UnityEvent();
    public event Action<float> OnDamaged;

    private void Awake()
    {
        currentHealth = maxHealth;
        Debug.Log($"[PlayerHealth] Initialized with {currentHealth} HP");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnDamaged?.Invoke(damage);

        Debug.Log(
            $"[PlayerHealth] Took {damage} damage → {currentHealth}/{maxHealth}"
        );

        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    public void Heal(float amount)
    {
        if (amount <= 0f) return;
        
        float before = currentHealth;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        Debug.Log($"[PlayerHealth] Healed {amount} → {currentHealth}/{maxHealth}");
    }

    private void Die()
    {
        Debug.Log("[PlayerHealth] Player is DEAD");
        OnDeath.Invoke();
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}