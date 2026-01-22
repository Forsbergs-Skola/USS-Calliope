using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "NPCs/Boss/Heavy Melee")]
public class SO_BossHeavyMeleeAttack : EnemyAttackSOClass
{
    //public float chargeTime = 3f;
    public float damage = 30f;
    public float range = 2f;

    public override bool CanExecute(EnemyAttackContext context)
    {
        float dist = Vector3.Distance(context.enemy.position, context.player.position);
        return dist <= range;
    }

    public override void Execute(EnemyAttackContext context)
    {
        Debug.Log($"{context.enemy.name} in melee");
        context.coroutineRunner.StartCoroutine(HeavyAttackRoutine(context));
    }

    private IEnumerator HeavyAttackRoutine(EnemyAttackContext context)
    {
        yield return new WaitForSeconds(2f); // windup

        float dist = Vector3.Distance(context.enemy.position, context.player.position);

        if (dist <= range)
        {
            var dmg = context.player.GetComponent<IDamageable>();
            dmg?.TakeDamage(damage);

            var rb = context.player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDir =
                    (context.player.position - context.enemy.position).normalized;

                rb.AddForce(pushDir * 25f, ForceMode.Impulse);
            }
        }
    }
}