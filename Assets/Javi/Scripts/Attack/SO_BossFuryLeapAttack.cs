using UnityEngine;

[CreateAssetMenu(menuName = "NPCs/Boss/Fury Leap Attack")]
public class SO_BossFuryLeapAttack : EnemyAttackSOClass
{
    [Header("Distances")]
    public float minDistance = 1.5f;
    public float maxDistance = 8f;

    [Header("Movement")]
    public float leapSpeed = 10f;
    public float duration = 2f;

    [Header("Combat")]
    public float damage = 25f;
    public float pushForce = 55f;

    public override bool CanExecute(EnemyAttackContext context)
    {
        var fury = context.enemy.GetComponent<BossFuryCounter>();
        var perception = context.enemy.GetComponent<BossPerception>();

        if (fury == null || perception == null)
            return false;

        if (!fury.ConsumeLeap())
            return false;

        if (!perception.CanSeePlayer())
            return false;

        float dist = Vector3.Distance(
            context.enemy.position,
            context.player.position
        );

        return dist >= minDistance && dist <= maxDistance;
    }

    public override void Execute(EnemyAttackContext context)
    {
        var runtime = context.enemy.GetComponent<EnemyLeapRuntime>();
        if (runtime == null) return;

        Vector3 dir = (context.player.position - context.enemy.position).normalized;

        runtime.StartLeap(dir, damage, pushForce);

        context.coroutineRunner.StartCoroutine(
            LeapRoutine(context, dir, runtime)
        );
    }

    private System.Collections.IEnumerator LeapRoutine(
        EnemyAttackContext context,
        Vector3 dir,
        EnemyLeapRuntime runtime)
    {
        var rb = context.enemy.GetComponent<Rigidbody>();
        float timer = 0f;

        while (timer < duration && runtime.IsLeapActive)
        {
            rb.linearVelocity = dir * leapSpeed;
            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector3.zero;

        if (runtime.IsLeapActive)
            runtime.EndLeap();
    }
}