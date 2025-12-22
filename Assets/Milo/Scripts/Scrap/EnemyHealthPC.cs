using UnityEngine;

public class EnemyHealthPC : MonoBehaviour
{
    [SerializeField] private int maxHealth = 50;

    private int currentHealth;
    private Collider col;

    void Awake()
    {
        currentHealth = maxHealth;
        col = GetComponent<Collider>();
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (col) col.enabled = false;
        Destroy(gameObject);
    }
}