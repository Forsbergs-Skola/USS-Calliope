using Olle.Scripts;
using UnityEngine;

public class UnarmedAttack : MonoBehaviour
{
    [SerializeField] SO_UnarmedAttackData data;
    [SerializeField] private ImpactProcessor impactProcessor;


    private PlayerAnimationController animator;

    public void Awake()
    {
        animator = GetComponent<PlayerAnimationController>();
    }
    
    public bool isUnarmed;
    public int Damage => data.Damage;
    public float Cooldown => data.AttackCooldown;

    public void Attack(Vector3 aimDirection, Transform firePoint, LayerMask hitMask )
    {
        aimDirection = transform.forward;
        var origin = firePoint.position;

        animator.Punch();

        var hits = Physics.SphereCastAll(origin, data.Radius, aimDirection, data.Reach, hitMask);

        foreach (var hit in hits)
        {
            if (hit.collider.transform.root == transform.root) continue;

            impactProcessor.ProcessUnarmedHit(hit, aimDirection, data.PushForce);
        }
    }
}
