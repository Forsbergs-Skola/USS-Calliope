using UnityEngine;

public class ImpactProcessor : MonoBehaviour
{
    [SerializeField] private LayerMask hitMask;
    
    private SO_WeaponType currentWeapon;
    
    public LayerMask HitMask => hitMask;

    public void InitializeProcessor(SO_WeaponType weapon)
    {
        currentWeapon = weapon;
    }

    public void ProcessHit(RaycastHit hit)
    {
        if (!currentWeapon) return;

        var calculatedDamage = currentWeapon.GetDamageAtDistance(hit.distance);

        Debug.Log($"Hit: {hit.collider.name} at {hit.point} | Dist: {hit.distance:F2}m");
        if (!hit.collider.gameObject.TryGetComponent<EnemyHealthPC>(out var healthComponent)) return;
        healthComponent.TakeDamage(calculatedDamage);
    }
}