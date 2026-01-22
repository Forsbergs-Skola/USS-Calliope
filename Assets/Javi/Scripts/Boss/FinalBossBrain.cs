using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyAIStateController))]
[RequireComponent(typeof(EnemyAttackController))]
public class FinalBossBrain : MonoBehaviour, IBossController
{
    private EnemyAIStateController ai;
    private EnemyAttackController attack;
    private IPerceptionSystem perception;
    private EnemyPatrolController patrol;

    private Transform player;
    
    private bool isInvestigating = false;
    private bool isWatching = false;
    public bool IsInvestigating => isInvestigating || isWatching;
    private Transform currentInvestigationPoint;
    
    //private EnemyFollowPlayer chase;
    private float watchTime = 6f;
    private bool hasReachedInvestigationPoint = false;
    

    private void Awake()
    {
        ai = GetComponent<EnemyAIStateController>();
        attack = GetComponent<EnemyAttackController>();
        perception = GetComponent<IPerceptionSystem>();
        patrol = GetComponent<EnemyPatrolController>();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    
    private float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0;
        b.y = 0;
        return Vector3.Distance(a, b);
    }

    private void Update()
    {
        TickBoss();
        
        if (isInvestigating && currentInvestigationPoint != null && !isWatching && !hasReachedInvestigationPoint)
        {
            /*float dist = Vector3.Distance(
                transform.position,
                currentInvestigationPoint.position
            );*/
            
            float dist = FlatDistance(
                transform.position,
                currentInvestigationPoint.position
            );

            if (dist > 0.5f)
            {
                //Debug.Log($"[finalBossBrain] {dist}");
                var movement = GetComponent<SimpleMovementAgent>();
                if (movement != null)
                {
                    movement.MoveTo(currentInvestigationPoint.position);
                }
            }
            else
            {
                hasReachedInvestigationPoint = true;
                StartCoroutine(WatchAtPoint());
            }
        }
        
        /*if (isInvestigating && currentInvestigationPoint !=null && !isWatching)
        {
            float dist = Vector3.Distance(
                transform.position,
                currentInvestigationPoint.position
            );

            if (dist < 0.5f)
            {
                StartCoroutine(WatchAtPoint());
            }
        }*/

        if (ai.CurrentState == EnemyAIStateController.State.Attacking)
        {
            OnPlayerSpotted();
        }
    }

    public void TickBoss()
    {
        if (player == null) return;

        if (!perception.CanSeeTarget(player))
        {
            InvestigatePlayerZone();
            return;
        }

        EnterCombat();
    }

    private void InvestigatePlayerZone()
    {
        if (isInvestigating)
            return;

        /*var zone = PatrolPointRegistry.GetClosestZone(player.position);
        if (zone == null)
            return;*/
        /*if (zone != null)
            patrol.PatrolSingleZone(zone);*/
        /*var point = PatrolPointRegistry.GetClosestPoints(player.position,1);
        if (point == null)
            return;*/
        
        PatrolZone zone = PatrolPointRegistry.GetClosestZone(player.position);
        if (zone == null)
            return;

        Transform point = PatrolPointRegistry.GetRandomPointInZone(zone);
        if (point == null)
            return;
        
        isInvestigating = true;
        currentInvestigationPoint = point;
        hasReachedInvestigationPoint = false;
        
        ai.ForceBossState();
        
        patrol.StopPatrol();
        //chase.SetFollow(false);
        //patrol.GoToPoint(currentInvestigationPoint);
        var movement = GetComponent<SimpleMovementAgent>();
        if (movement != null)
        {
            movement.SetMovementState(SimpleMovementAgent.MovementState.Patrolling);
            //movement.MoveTo(currentInvestigationPoint.position);
        }
    }

    private void EnterCombat()
    {
        if (ai.CurrentState != EnemyAIStateController.State.Attacking)
            ai.SendMessage("EnterAttacking", SendMessageOptions.DontRequireReceiver);
    }
    
    private IEnumerator WatchAtPoint()
    {
        isWatching = true;

        //chase.SetFollow(false);

        yield return new WaitForSeconds(watchTime);

        isWatching = false;
        isInvestigating = false;
        currentInvestigationPoint = null;

        
        var movement = GetComponent<SimpleMovementAgent>();
        if (movement != null)
            movement.SetMovementState(SimpleMovementAgent.MovementState.Patrolling);
        // next point
        InvestigatePlayerZone();
        hasReachedInvestigationPoint = false;
    }
    
    public void OnPlayerSpotted()
    {
        isInvestigating = false;
        isWatching = false;
        hasReachedInvestigationPoint = false;
        currentInvestigationPoint = null;
    }
}