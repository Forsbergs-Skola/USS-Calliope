using UnityEngine;

[RequireComponent(typeof(EnemyAttackController))]
[RequireComponent(typeof(BossHealth))]
[RequireComponent(typeof(BossFuryCounter))]
public class BossAttackDirector : MonoBehaviour
{
    private EnemyAttackController attackController;
    private BossHealth health;
    private BossFuryCounter fury;

    private bool phase40Triggered = false;

    private void Awake()
    {
        attackController = GetComponent<EnemyAttackController>();
        health = GetComponent<BossHealth>();
        fury = GetComponent<BossFuryCounter>();

        health.OnDamageTaken.AddListener(OnDamageTaken);
    }
    
    private void Update()
    {
        if (phase40Triggered) return;

        if (health.GetCurrentHealth() <= 40f)
        {
            phase40Triggered = true;
            TriggerFinalPhase();
        }
    }

    private void OnDamageTaken(float dmg)
    {
        // 
    }

    public EnemyAttackInstance ChooseBossAttack(EnemyAttackInstance ranged, EnemyAttackInstance melee, EnemyAttackInstance leap, EnemyAttackContext context)
    {
        Debug.Log("En ChooseBossAttack");
        float dist = Vector3.Distance(
            context.enemy.position,
            context.player.position
        );

        // too close, melee
        if (melee != null)
        {
            Debug.Log("En melee");
            float leapMin = GetLeapMin(leap);
            if (dist < leapMin)
                return melee;
        }

        // leap
        if (leap != null && fury.IsLeapReady)
        {
            Debug.Log("En leap");
            return leap;
        }

        // ranged by default
        Debug.Log("En ranged");
        return ranged;
    }

    private float GetLeapMin(EnemyAttackInstance leap)
    {
        if (leap.attack is SO_BossFuryLeapAttack l)
            return l.minDistance;
        return 0f;
    }
    
    private void TriggerFinalPhase()
    {
        GetComponent<EnemyCallSystem>()?.TryCall(transform.position);
        GetComponent<BossEnemySpawner>()?.Spawn(
            FindObjectOfType<PlayerHealthJavi>().transform.position,
            false
        );
    }
}
