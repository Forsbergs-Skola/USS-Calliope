using UnityEngine;
using System.Collections;
using NUnit.Framework;

public class ImpactProcessor : MonoBehaviour
{
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private AudioSource hitAudioSource;

    [SerializeField] private AudioClip PunchHit;

    private SO_WeaponType currentWeapon;
    private UnarmedAttack unarmed;
    public LayerMask HitMask => HitMask1;

    public LayerMask HitMask1 { get => hitMask; set => hitMask = value; }
    public AudioSource HitAudioSource { get => hitAudioSource; set => hitAudioSource = value; }
    public AudioClip PunchHit1 { get => PunchHit; set => PunchHit = value; }
    public SO_WeaponType CurrentWeapon { get => currentWeapon; set => currentWeapon = value; }
    public UnarmedAttack Unarmed { get => unarmed; set => unarmed = value; }

    public void InitializeProcessor(SO_WeaponType weapon)
    {
        CurrentWeapon = weapon;
    }
    
    public void ProcessHit(RaycastHit hit)
    {
        if (!CurrentWeapon) return;

        float calculatedDamage = CurrentWeapon.GetDamageAtDistance(hit.distance);
        
        var damageable = hit.collider.GetComponentInParent<IDamageable>();
        damageable?.TakeDamage(calculatedDamage);
    }
    
    public void ProcessTase(RaycastHit hit)
    {
        if (!CurrentWeapon || CurrentWeapon.AttackCategories != SO_WeaponType.AttackCategory.NonLethal)
        {
            return;
        }

        var stunTime = CurrentWeapon.StunEffectTime;

    }

    public void ProcessMeleeHit(RaycastHit hit, Vector3 attackDirection, float force)
    {
        if (!CurrentWeapon) return;

        var damageable = hit.collider.GetComponentInParent<IDamageable>();
        damageable?.TakeDamage(CurrentWeapon.Damage);

        // TODO:
        // Apply force to the hit object if it has a Rigidbody
    }

    public void ProcessUnarmedHit(RaycastHit hit, Vector3 attackDirection, float force)
    {
        if (Unarmed == null) Unarmed = GetComponent<UnarmedAttack>();
        var damageable = hit.collider.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            StartCoroutine(DelayedUnarmedDamage(damageable, Unarmed.Damage, 0.2f));
            AudioSource.PlayClipAtPoint(PunchHit1, hit.point);
        }

        // TODO:
        // Apply force to the hit object if it has a Rigidbody
    }

    private IEnumerator DelayedUnarmedDamage(IDamageable target, float damage, float delay)
    {
        yield return new WaitForSeconds(delay);
        target.TakeDamage(damage);
    }
}