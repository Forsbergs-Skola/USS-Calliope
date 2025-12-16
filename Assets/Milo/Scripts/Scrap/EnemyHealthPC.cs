using UnityEngine;

public class EnemyHealthPC : MonoBehaviour
{
    [SerializeField] private int maxHealth = 50; 
    
    public int CurrentHealth { get; private set; }

    public int MaxHealth => maxHealth;

    void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        if (CurrentHealth <= 0) 
            return;

        CurrentHealth = Mathf.Max(CurrentHealth - damageAmount, 0);
        
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GetComponent<Collider>().enabled = false; 
        
        if (TryGetComponent<MeshRenderer>(out var meshRenderer))
        {
            meshRenderer.enabled = false;
        }

        Destroy(gameObject, 2f); 
    }
}