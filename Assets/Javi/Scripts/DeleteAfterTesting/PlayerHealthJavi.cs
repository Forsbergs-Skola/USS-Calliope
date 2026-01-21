using UnityEngine;
using UnityEngine.Events;
using System;

public class PlayerHealthJavi : MonoBehaviour, IDamageable, IDamageEvents
{
    [Header("Health")]
    //[SerializeField] private float maxHealth = 100f;
    private float maxHealth;
    private float currentHealth;
    
    public UnityEvent<float, float> OnHealthChanged = new UnityEvent<float, float>();
    public UnityEvent OnDeath = new UnityEvent();
    public event Action<float> OnDamaged;

    private void Awake()
    {
        //currentHealth = maxHealth;

        if (TryGetPlayerData(out PlayerData pdata))
        {
            maxHealth = pdata.MaxHealth;
            currentHealth = pdata.Health;
        }
        else
        {
            maxHealth = 100f;
            currentHealth = maxHealth;
        }

        //Debug.Log($"[PlayerHealth] Initialized with {currentHealth} HP");
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

        TryUpdateBackend(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }

    }

    private void Die()
    {
        Debug.Log("[PlayerHealth] Player is DEAD");
        OnDeath.Invoke();
    }

    private void TryUpdateBackend(float _currentHealth, float _maxHealth)
    {

        if (TryGetPlayerData(out PlayerData pData))
        {
            pData.Health = _currentHealth;
            pData.MaxHealth = _maxHealth;
        }
    }
    private bool TryGetSavedHealth(out Vector2 savedHealthData)
    {

        if (TryGetPlayerData(out PlayerData pData))
        {
            float _currentHealth = pData.Health;
            float _maxHealth = pData.MaxHealth;

            savedHealthData = new Vector2(_currentHealth, _maxHealth);
            return true;
        }
        savedHealthData = new Vector2();
        return false;
    }

    private bool TryGetPlayerData(out PlayerData pData)
    {
        if (TryGetComponent<PlayerDataHandler>(out PlayerDataHandler pDataHandler))
        {
            Debug.Log("DATA FOUND");

            pData = pDataHandler.RuntimeData.Value;
            return true;
        }
        pData = null;
        return false;
    }


    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}