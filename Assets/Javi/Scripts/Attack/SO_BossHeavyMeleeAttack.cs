using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "NPCs/Boss/Heavy Melee")]
public class SO_BossHeavyMeleeAttack : EnemyAttackSOClass
{
    public float chargeTime = 3f;
    public float damage = 30f;
    public float range = 2f;

    public override bool CanExecute(EnemyAttackContext context)
    {
        float dist = Vector3.Distance(context.enemy.position, context.player.position);
        return dist <= range;
    }

    public override void Execute(EnemyAttackContext context)
    {
        context.coroutineRunner.StartCoroutine(HeavyAttackRoutine(context));
    }

    private IEnumerator HeavyAttackRoutine(EnemyAttackContext context)
    {
        yield return new WaitForSeconds(chargeTime);

        float dist = Vector3.Distance(context.enemy.position, context.player.position);
        if (dist <= range)
        {
            context.player.GetComponent<IDamageable>()?.TakeDamage(damage);
        }
    }
}