using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "NPCs/Attacks/Leap Attack")]
public class SO_LeapAttack : EnemyAttackSOClass
{
    [Header("Distances")]
    [Tooltip("If enemy is closer than this, Leap will NOT start")]
    public float minExecuteDistance = 1.5f;

    [Tooltip("Max distance to start Leap")]
    public float startRunDistance = 6f;

    [Tooltip("Distance to hit the headbutt")]
    public float headbuttDistance = 1.2f;

    [Header("Movement")]
    public float crouchRunSpeed = 6f;
    public float crouchRunDuration = 4f;

    [Header("Combat")]
    public float damage = 20f;

    [Header("Probability")]
    [Range(0f, 1f)]
    public float maxProbability = 0.6f;

    public override bool CanExecute(EnemyAttackContext context)
    {
        float dist = Vector3.Distance(
            context.enemy.position,
            context.player.position
        );

        // Too close
        if (dist < minExecuteDistance)
            return false;

        // Too far
        if (dist > startRunDistance)
            return false;

        // Infection-based probability
        float infection01 = context.infectionPercentage / 100f;
        float chance = Mathf.Lerp(0f, maxProbability, infection01);
        
        return true;
        //return Random.value <= chance;
    }

    public override void Execute(EnemyAttackContext context)
    {
        context.coroutineRunner.StartCoroutine(
            LeapRoutine(context)
        );
    }

    private IEnumerator LeapRoutine(EnemyAttackContext context)
    {
        var enemy = context.enemy;
        var player = context.player;
        var movement = context.movement;

        Debug.Log("[Leap] Started");

        float originalSpeed = movement.MoveSpeed;

        // Crouch run
        movement.MoveSpeed = crouchRunSpeed;

        float timer = 0f;
        while (timer < crouchRunDuration)
        {
            // If we are close -> hit
            if (Vector3.Distance(enemy.position, player.position) <= headbuttDistance)
                break;

            timer += Time.deltaTime;
            yield return null;
        }

        // Stop & hit
        //movement.FollowPlayer = false;

        if (Vector3.Distance(enemy.position, player.position) <= headbuttDistance)
        {
            var damageable = player.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                Debug.Log("[Leap] Headbutt HIT");
            }
        }
        else
        {
            Debug.Log("[Leap] Failed (player escaped)");
        }

        // Restore movement
        movement.MoveSpeed = originalSpeed;
    }
}
