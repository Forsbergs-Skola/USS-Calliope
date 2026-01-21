using System.Collections;
using UnityEngine;

public class EnemyLeapRuntime : MonoBehaviour
{
    public bool IsLeapActive { get; private set; }

    private float leapDamage;
    private float pushForce;
    private Vector3 leapDirection;

    private EnemyAIStateController ai;
    
    [SerializeField] private float stunDuration = 4f;
    private Coroutine stunRoutine;

    private void Awake()
    {
        ai = GetComponent<EnemyAIStateController>();
    }

    public void StartLeap(Vector3 direction, float damage, float force)
    {
        IsLeapActive = true;
        leapDirection = direction.normalized;
        leapDamage = damage;
        pushForce = force;
    }

    public void EndLeap()
    {
        if (!IsLeapActive) return;

        IsLeapActive = false;
        if (stunRoutine != null)
            StopCoroutine(stunRoutine);
        
        stunRoutine = StartCoroutine(StunRoutine());
    }
    
    private IEnumerator StunRoutine()
    {
        ai.OnStunnedStart();
        yield return new WaitForSeconds(stunDuration);
        ai.OnStunnedEndAfterLeapAttack();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsLeapActive) return;
        
        //Debug.Log($"[Leap] hit {collision.gameObject.name}");
        //Debug.Log($"Collider: {collision.collider.name} | Tag: {collision.collider.tag}");
        /*if (collision.collider.CompareTag("Player"))
        {
            Debug.Log($"[Leap] inside compareTag; Damage:{leapDamage}");
            // Damage
            collision.collider.GetComponent<IDamageable>()?.TakeDamage(leapDamage);

            // collision + push
            var rb = collision.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(leapDirection * pushForce, ForceMode.Impulse);
            }
        }*/
        
        int layer = collision.gameObject.layer;

        // avoid ground
        if (layer == LayerMask.NameToLayer("Ground"))
            return;
        
        var damageable = collision.collider.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            Debug.Log($"[Leap] {collision.gameObject.name} hit; Damage:{leapDamage}");

            // Damage
            damageable.TakeDamage(leapDamage);

            // Push
            var rb = collision.collider.GetComponentInParent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(leapDirection * pushForce, ForceMode.Impulse);
            }
        }
        
        // end leap
        EndLeap();
    }
}