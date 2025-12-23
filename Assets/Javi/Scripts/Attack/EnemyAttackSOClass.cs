using UnityEngine;

public abstract class EnemyAttackSOClass : ScriptableObject
{
    [Header("Base Settings")]
    public float cooldown = 1.5f;

    protected float lastUseTime;

    public bool IsOnCooldown()
    {
        return Time.time - lastUseTime < cooldown;
    }

    public void MarkUsed()
    {
        lastUseTime = Time.time;
    }
    
    public abstract bool CanExecute(EnemyAttackContext context);
    public abstract void Execute(EnemyAttackContext context);
}