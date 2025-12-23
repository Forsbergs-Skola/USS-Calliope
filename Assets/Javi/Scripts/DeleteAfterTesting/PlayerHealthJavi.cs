using UnityEngine;

public class PlayerHealthJavi : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        Debug.Log($"[PlayerHealth] Initialized with {currentHealth} HP");
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f)
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log(
            $"[PlayerHealth] Took {amount} damage → {currentHealth}/{maxHealth}"
        );

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("[PlayerHealth] Player is DEAD");
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}