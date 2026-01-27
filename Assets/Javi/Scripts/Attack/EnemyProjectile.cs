using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Collision")]
    [SerializeField] private LayerMask ignoredLayers;
    
    private Vector3 direction;
    private float speed;
    private float damage;

    public void Initialize(Vector3 dir, float spd, float dmg)
    {
        direction = dir;
        speed = spd;
        damage = dmg;

        Destroy(gameObject, 8f);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"jrv Projectile hit {other.name}");
        
        if ((ignoredLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            return;
        }
        
        if (other.CompareTag("Player"))
        {
            var dmg = other.GetComponent<IDamageable>();
            dmg?.TakeDamage(damage);
        }
        
        Destroy(gameObject);
    }
}