using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "NPCs/Attacks/Leap Attack")]
public class SO_LeapAttack : EnemyAttackSOClass
{
    [Header("Leap Settings")]
    public float leapDistance = 3f;
    public float leapSpeed = 8f;
    public float damage = 20f;

    [Header("Probability")]
    [Tooltip("Chance at 100% infection")]
    [Range(0f, 1f)] public float maxProbability = 0.6f;

    public override bool CanExecute(EnemyAttackContext context)
    {
        float dist = Vector3.Distance(
            context.enemy.position,
            context.player.position
        );

        if (dist > leapDistance)
            return false;

        float infection01 = context.infectionPercentage / 100f;
        float chance = Mathf.Lerp(0f, maxProbability, infection01);

        return Random.value <= chance;
    }

    public override void Execute(EnemyAttackContext context)
    {
        context.enemy
            .GetComponent<MonoBehaviour>()
            .StartCoroutine(LeapCoroutine(context));
    }

    private IEnumerator LeapCoroutine(EnemyAttackContext context)
    {
        Transform enemy = context.enemy;
        Transform player = context.player;

        Vector3 start = enemy.position;
        Vector3 target = player.position;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * leapSpeed;
            enemy.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        var damageable = player.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
    }
}