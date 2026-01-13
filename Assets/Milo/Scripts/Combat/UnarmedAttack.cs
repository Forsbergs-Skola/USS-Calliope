using Olle.Scripts;
using UnityEngine;

public class UnarmedAttack : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float reach = 1.5f;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private ImpactProcessor impactProcessor;

    private PlayerAnimationController animator;

    public void Awake()
    {
        animator = GetComponent<PlayerAnimationController>();
    }
    
    public bool isUnarmed;
    public int Damage => damage;
    public float Cooldown => attackCooldown;

    [SerializeField] private int force = 5;

    public void Attack(Vector3 aimDirection, Transform firePoint, LayerMask hitMask )
    {
        Debug.Log("Performing unarmed attack");
        aimDirection = transform.forward;
        var origin = firePoint.position;

        animator.Punch();

        Debug.DrawRay(origin, aimDirection * reach, Color.yellow, 1.0f);

        var hits = Physics.SphereCastAll(origin, radius, aimDirection, reach, hitMask);

        foreach (var hit in hits)
        {
            if (hit.collider.transform.root == transform.root) continue;

            impactProcessor.ProcessUnarmedHit(hit, aimDirection, force);
        }
    }
}
