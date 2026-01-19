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

        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var dmg = other.GetComponent<IDamageable>();
            dmg?.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}