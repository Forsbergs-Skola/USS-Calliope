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

        float calculatedDamage = currentWeapon.GetDamageAtDistance(hit.distance);
        
        var damageable = hit.collider.GetComponentInParent<IDamageable>();
        damageable?.TakeDamage(calculatedDamage);
    }
    
    public void ProcessTase(RaycastHit hit)
    {
        if (!currentWeapon || currentWeapon.AttackCategories != SO_WeaponType.AttackCategory.NonLethal)
        {
            return;
        }

        var stunTime = currentWeapon.StunEffectTime;

    }

    public void ProcessMeleeHit(RaycastHit hit, Vector3 attackDirection, float force)
    {
        if (!currentWeapon) return;

        var damageable = hit.collider.GetComponentInParent<IDamageable>();
        damageable?.TakeDamage(currentWeapon.Damage);
    }
}