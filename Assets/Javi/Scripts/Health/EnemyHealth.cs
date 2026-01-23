using UnityEngine;
using UnityEngine.Events;
using System;

public class EnemyHealth : MonoBehaviour, IDamageable, IDamageEvents
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public UnityEvent<float, float> OnHealthChanged = new UnityEvent<float, float>();
    public UnityEvent OnDeath = new UnityEvent();
    public event Action<float> OnDamaged;
    
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;

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

        OnDamaged?.Invoke(damage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        Debug.Log("Enemy took damage: " + damage);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {

        if (TryGetComponent<BloodSampleGiver>(out BloodSampleGiver sampleGiver))
        {
            sampleGiver.TryGiveSample();
        }
        else
        {
            Debug.LogError("Add a BloodSampleGiver to this enemy");
        }


        // tell the sample giver to give the sample

        OnDeath.Invoke();
        Destroy(gameObject);
    }
}