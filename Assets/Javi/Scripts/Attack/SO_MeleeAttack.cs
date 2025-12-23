using UnityEngine;

[CreateAssetMenu(menuName = "NPCs/Attacks/Melee Attack")]
public class SO_MeleeAttack : EnemyAttackSOClass
{
    public float damage = 10f;
    public float hitRadius = 1.2f;

    public override bool CanExecute(EnemyAttackContext context)
    {
        float dist = Vector3.Distance(
            context.enemy.position,
            context.player.position
        );

        return dist <= hitRadius;
    }

    public override void Execute(EnemyAttackContext context)
    {
        Debug.Log("Melee Execute CALLED");
        var damageable = context.player.GetComponent<IDamageable>();
        if (damageable == null)
        {
            Debug.LogError("Player does NOT implement IDamageable");
            return;
        }

        damageable.TakeDamage(damage);
        Debug.Log("Melee attack hit the player");
    }
}