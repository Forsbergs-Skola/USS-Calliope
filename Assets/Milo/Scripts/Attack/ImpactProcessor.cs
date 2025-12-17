using UnityEngine;

public class ImpactProcessor : MonoBehaviour
{
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private float fixedMaxDistance = 100f;

    private int currentWeaponDamage;
    public LayerMask HitMask => hitMask;
    public float MaxDistance => fixedMaxDistance;

    public void InitializeProcessor(SO_WeaponType weapon)
    {
        currentWeaponDamage = weapon.WeaponDamage;
    }

    public void ProcessHit(RaycastHit hit)
    {
        Debug.Log($"Hit: {hit.collider.name} at {hit.point} | Dist: {hit.distance:F2}m");

        if (!hit.collider.gameObject.TryGetComponent<EnemyHealthPC>(out var healthComponent)) return;
        healthComponent.TakeDamage(currentWeaponDamage);
        Debug.Log($"Damage dealt: {currentWeaponDamage} to {hit.collider.name}");
    }
}