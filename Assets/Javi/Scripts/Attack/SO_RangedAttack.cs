using UnityEngine;

[CreateAssetMenu(menuName = "NPCs/Attacks/Ranged Attack")]
public class SO_RangedEnemyAttack : EnemyAttackSOClass
{
    [Header("Ranges")]
    public float minRange = 2f;
    public float maxRange = 20f;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;
    public float damage = 15f;
    //public float spread = 0.1f;

    public override bool CanExecute(EnemyAttackContext context)
    {
        if (context.player == null) return false;

        float distance = Vector3.Distance(
            context.enemy.position,
            context.player.position
        );

        // Too close 
        if (distance < minRange)
            return false;

        // too far
        if (distance > maxRange)
        {
            //Debug.Log($"{context.enemy.name} distance: {distance} > maxRange: {maxRange}");
            /*context.movement?.SetTarget(context.player);
            context.movement?.SetFollow(true);*/
            return false;
        }

        return true;
    }

    public override void Execute(EnemyAttackContext context)
    {
        context.movement?.SetFollow(false);

        var spawner = context.enemy.GetComponent<EnemyProjectileSpawner>();
        if (spawner == null) return;
        
        spawner.SpawnProjectile(
            projectilePrefab,
            context.firePoint,
            context.player.position,
            projectileSpeed,
            damage
            //spread
        );
    }
}