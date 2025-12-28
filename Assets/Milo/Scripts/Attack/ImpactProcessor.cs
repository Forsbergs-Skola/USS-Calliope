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

        if (!hit.collider.gameObject.TryGetComponent<EnemyHealthPC>(out var healthComponent)) return;
        healthComponent.TakeDamage(calculatedDamage);
    }
    
    public void ProcessTase(RaycastHit hit)
    {
        if (!currentWeapon || currentWeapon.AttackCategories != SO_WeaponType.AttackCategory.NonLethal)
        {
            return;
        }

        var stunTime = currentWeapon.StunEffectTime;

        // if (!hit.collider.gameObject.TryGetComponent<EnemyStunEffect>(out var stunEffect)) return;
        // stunEffect.GetStunned(stunTime);
    }

    public void ProcessMeleeHit(RaycastHit hit, Vector3 attackDirection, float force)
    {
        if (!currentWeapon) return;

        if (hit.collider.TryGetComponent<EnemyHealthPC>(out var health))
        {
            health.TakeDamage(currentWeapon.Damage);
        }

        if (hit.collider.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForceAtPosition(attackDirection.normalized * force, hit.point, ForceMode.Impulse);
        }
    }


}