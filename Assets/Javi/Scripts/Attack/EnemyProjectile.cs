using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float damage;

    public void Initialize(Vector3 dir, float spd, float dmg)
    {
        direction = dir;
        speed = spd;
        damage = dmg;
        
        Collider myCol = GetComponent<Collider>();
        Collider enemyCol = GetComponentInParent<Collider>();

        if (myCol != null && enemyCol != null)
        {
            Physics.IgnoreCollision(myCol, enemyCol);
        }

        Destroy(gameObject, 8f);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Projectile hit {other.name}");
        if (other.CompareTag("Player"))
        {
            var dmg = other.GetComponent<IDamageable>();
            dmg?.TakeDamage(damage);
        }

        //Destroy(gameObject);
    }
}