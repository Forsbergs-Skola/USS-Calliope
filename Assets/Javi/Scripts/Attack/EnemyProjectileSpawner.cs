using UnityEngine;

public class EnemyProjectileSpawner : MonoBehaviour
{
    //[SerializeField] private Transform firePoint;

    public void SpawnProjectile(GameObject prefab, Transform firePoint, Vector3 targetPosition, float speed, float damage/*, float spread*/)
    {
        if (prefab == null || firePoint == null) return;
        //Debug.Log($"{name} Spawn: yes");
        //Debug.Log($"FirePoint pos: {firePoint.position}");
        //Debug.Log($"Enemy pos: {transform.position}");
        Vector3 dir = (targetPosition - firePoint.position).normalized;
       //dir += Random.insideUnitSphere * spread;

        GameObject proj = Instantiate(
            prefab,
            firePoint.position,
            Quaternion.LookRotation(dir)
        );

        var projectile = proj.GetComponent<EnemyProjectile>();
        if (projectile != null)
        {
            projectile.Initialize(dir.normalized, speed, damage);
        }
    }
}