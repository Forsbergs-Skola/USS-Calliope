using UnityEngine;

public class EnemyAIStateController : MonoBehaviour
{
    private EnemyPatrolController patrol;
    private EnemyFollowPlayer chase;
    private EnemyPerceptionSystem perception;

    private Transform player;

    private enum State { Wandering, Watchful, Attacking }
    private State currentState = State.Wandering;

    private void Awake()
    {
        patrol = GetComponent<EnemyPatrolController>();
        chase = GetComponent<EnemyFollowPlayer>();
        perception = GetComponent<EnemyPerceptionSystem>();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Start()
    {
        EnterWandering();
    }

    private void Update()
    {
        if (player == null) return;

        switch (currentState)
        {
            case State.Wandering:
                if (perception.CanSeeTarget(player))
                    EnterAttacking();
                break;

            case State.Attacking:
                if (!perception.CanSeeTarget(player))
                    EnterWatchful();
                break;

            case State.Watchful:
                // we could addd more logic here
                if (perception.CanSeeTarget(player))
                    EnterAttacking();
                else if (!IsInvoking(nameof(ReturnToPatrol)))
                    Invoke(nameof(ReturnToPatrol), 5f); // looking for 5s
                break;
        }
    }

    private void EnterWandering()
    {
        chase.SetFollow(false);
        patrol.StartPatrol();
        currentState = State.Wandering;
        Debug.Log($"{name} entering Wandering state");
    }

    private void EnterAttacking()
    {
        CancelInvoke(nameof(ReturnToPatrol));
        patrol.StopPatrol();
        chase.SetTarget(player);
        chase.SetFollow(true);
        currentState = State.Attacking;
        Debug.Log($"{name} entering Attacking state");
    }

    private void EnterWatchful()
    {
        patrol.StopPatrol();
        chase.SetFollow(false);
        currentState = State.Watchful;
        Debug.Log($"{name} entering Watchful state");
        
        // changing to investigating
        var movement = GetComponent<SimpleMovementAgent>();
        if (movement != null)
            movement.SetMovementState(SimpleMovementAgent.MovementState.Investigating);
    }
    
    private void ReturnToPatrol()
    {
        EnterWandering();
    }
}