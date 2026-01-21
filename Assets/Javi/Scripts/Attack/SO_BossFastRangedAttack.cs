using UnityEngine;

[CreateAssetMenu(menuName = "NPCs/Boss/Fast Ranged Attack")]
public class SO_BossFastRangedAttack : EnemyAttackSOClass
{
    public SO_RangedEnemyAttack baseRanged;
    public float fireRateMultiplier = 2f;

    private float lastFire;

    public override bool CanExecute(EnemyAttackContext context)
    {
        if (context.player == null)
            return false;

        if (Time.time - lastFire < baseRanged.cooldown / fireRateMultiplier)
            return false;

        /*if (!context.perception.CanSeeTarget(context.player))
            return false;*/

        float distance = Vector3.Distance(
            context.enemy.position,
            context.player.position
        );
        
        if (distance < baseRanged.minRange)
            return false;

        return true;
    }

    public override void Execute(EnemyAttackContext context)
    {
        context.movement?.SetFollow(false);

        if (context.firePoint == null)
            return;

        var spawner = context.enemy.GetComponent<EnemyProjectileSpawner>();
        if (spawner == null)
            return;

        spawner.SpawnProjectile(baseRanged.projectilePrefab, context.firePoint, context.player.position, baseRanged.projectileSpeed, baseRanged.damage
        );
    }
}