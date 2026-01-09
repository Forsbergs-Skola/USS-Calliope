using UnityEngine;

public class UnarmedAttack : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float reach = 1.5f;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private ImpactProcessor impactProcessor;
    public bool isUnarmed;

    // Apply physics to AI
    [SerializeField] private int force = 5;

   

    public void Attack(Vector3 aimDirection, Transform firePoint, LayerMask hitMask )
    {
        if (aimDirection == Vector3.zero) aimDirection = transform.forward;

        var origin = firePoint.position;
        var hits = Physics.SphereCastAll(origin, radius, aimDirection, reach, hitMask);

        foreach (var hit in hits)
        {
            if (hit.collider.transform.root == transform.root) continue;

            impactProcessor.ProcessMeleeHit(hit, aimDirection, force);
        }
    }
}
