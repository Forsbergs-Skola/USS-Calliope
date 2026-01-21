using UnityEngine;

[CreateAssetMenu(menuName = "NPCs/Boss/Fast Ranged Attack")]
public class SO_BossFastRangedAttack : EnemyAttackSOClass
{
    public SO_RangedEnemyAttack baseRanged;
    public float fireRateMultiplier = 2f;

    private float lastFire;

    public override bool CanExecute(EnemyAttackContext context)
    {
        if (Time.time - lastFire < baseRanged.cooldown / fireRateMultiplier)
            return false;

        return baseRanged.CanExecute(context);
    }

    public override void Execute(EnemyAttackContext context)
    {
        lastFire = Time.time;
        baseRanged.Execute(context);
    }
}