using UnityEngine;

public class BossAttackDirector : MonoBehaviour, IAttackDecisionSystem
{
    private BossHealth health;
    private BossFuryCounter fury;

    private void Awake()
    {
        health = GetComponent<BossHealth>();
        fury = GetComponent<BossFuryCounter>();
    }

    public EnemyAttackInstance ChooseAttack(
        EnemyAttackInstance ranged,
        EnemyAttackInstance melee,
        EnemyAttackInstance leap,
        EnemyAttackContext context)
    {
        float dist = Vector3.Distance(
            context.enemy.position,
            context.player.position
        );
        
        if (melee != null /*&& dist < GetLeapMinDistance(leap)*/)
        {
            float meleeRange = 2.5f; // from SO????
            if (dist <= meleeRange)
                return melee;
        }

        if (leap != null /*&& fury.IsLeapReady && leap.attack.CanExecute(context)*/)
        {
            var fury = context.enemy.GetComponent<BossFuryCounter>();
            var leapSO = leap.attack as SO_BossFuryLeapAttack;

            if (fury != null &&
                fury.IsLeapReady &&
                dist >= leapSO.minDistance &&
                dist <= leapSO.maxDistance)
            {
                return leap;
            }
        }

        if (ranged != null /*&& ranged.attack.CanExecute(context)*/)
        {
            float minRange = GetRangedMin(ranged);
            if (dist >= minRange)
                return ranged;
        }
        
        return melee;
    }
    
    private float GetRangedMin(EnemyAttackInstance ranged)
    {
        if (ranged.attack is SO_RangedEnemyAttack r)
            return r.minRange;

        if (ranged.attack is SO_BossFastRangedAttack b)
            return b.baseRanged.minRange;

        return 0f;
    }

    private float GetLeapMinDistance(EnemyAttackInstance leap)
    {
        if (leap?.attack is SO_BossFuryLeapAttack l)
            return l.minDistance;

        return 0f;
    }
}
