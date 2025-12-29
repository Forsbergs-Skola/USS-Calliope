using UnityEngine;

public abstract class EnemyAttackSOClass : ScriptableObject
{
    [Header("Base Settings")]
    public float cooldown = 1.5f;

    public abstract bool CanExecute(EnemyAttackContext context);
    public abstract void Execute(EnemyAttackContext context);
}
