using UnityEngine;

[System.Serializable] public class EnemyAttackInstance
{
    public EnemyAttackSOClass attack;
    private float lastUseTime;

    public bool IsOnCooldown()
    {
        return Time.time - lastUseTime < attack.cooldown;
    }

    public void MarkUsed()
    {
        lastUseTime = Time.time;
    }
}