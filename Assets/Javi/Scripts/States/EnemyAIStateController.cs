using System.Collections;
using UnityEngine;

public class EnemyAIStateController : MonoBehaviour
{
    private EnemyPatrolController patrol;
    private EnemyFollowPlayer chase;
    private EnemyPerceptionSystem perception;

    private Transform player;

    private enum State { Wandering, Watchful, Attacking }
    private State currentState = State.Wandering;
    
    [SerializeField] private float watchfulDuration = 60f;
    [SerializeField] private float watchfulTimer = 8f;
    private PatrolZone lastSeenZone;
    
    private Olle.Scripts.CrouchInvisibility playerInvisibility;
    private Coroutine watchfulRoutine;

    private void Awake()
    {
        patrol = GetComponent<EnemyPatrolController>();
        chase = GetComponent<EnemyFollowPlayer>();
        perception = GetComponent<EnemyPerceptionSystem>();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        playerInvisibility = player != null 
            ? player.GetComponent<Olle.Scripts.CrouchInvisibility>() 
            : null;
    }

    private void Start()
    {
        EnterWandering();
    }

    private void Update()
    {
        if (player == null) return;
        
        playerInvisibility = player != null 
            ? player.GetComponent<Olle.Scripts.CrouchInvisibility>() 
            : null;

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
                /*else if (!IsInvoking(nameof(ReturnToPatrol)))
                    Invoke(nameof(ReturnToPatrol), 5f);*/ // looking for 5s
                /*else
                {
                    watchfulTimer -= Time.deltaTime;

                    if (watchfulTimer <= 0f)
                    {
                        EnterWandering();
                    }
                }*/
                break;
        }
    }

    private void EnterWandering()
    {
        if (watchfulRoutine != null)
            StopCoroutine(watchfulRoutine);
        
        chase.SetFollow(false);
        patrol.StartPatrol();
        currentState = State.Wandering;
        Debug.Log($"{name} entering Wandering state");
    }

    private void EnterAttacking()
    {
        if (watchfulRoutine != null)
        {
            StopCoroutine(watchfulRoutine);
            watchfulRoutine = null;
        }
        
        //CancelInvoke(nameof(ReturnToPatrol));
        patrol.StopPatrol();
        chase.SetTarget(player);
        chase.SetFollow(true);
        currentState = State.Attacking;
        Debug.Log($"{name} entering Attacking state");
    }

    private void EnterWatchful()
    {
        //Invoke(nameof(ReturnToPatrol), watchfulDuration);
        patrol.StopPatrol();
        chase.SetFollow(false);
        chase.SetTarget(null);
        //watchfulTimer = watchfulDuration;
        
        lastSeenZone = PatrolPointRegistry.GetClosestZone(player.position);
        Debug.Log("[EnemyAIStateController] EnterWatchful "  + lastSeenZone);
        if (lastSeenZone == null)
        {
            Debug.LogWarning($"{name}: Could not determine last seen zone, falling back to main patrol");
        }
        
        currentState = State.Watchful;
        Debug.Log($"{name} entering Watchful state");
        
        if (watchfulRoutine != null)
            StopCoroutine(watchfulRoutine);

        watchfulRoutine = StartCoroutine(WatchfulRoutine());
        
        // changing to investigating
        var movement = GetComponent<SimpleMovementAgent>();
        if (movement != null)
            movement.SetMovementState(SimpleMovementAgent.MovementState.Investigating);
    }
    
    private IEnumerator WatchfulRoutine()
    {
        // Quiet watch
        yield return new WaitForSeconds(watchfulTimer);

        // Investigate zone
        if (lastSeenZone != null)
        {
            Debug.Log("[EnemyAIStateController] WatchfulRoutine "  + lastSeenZone);
            patrol.PatrolSingleZone(lastSeenZone);
            yield return new WaitForSeconds(watchfulDuration);
        }

        // Back to main patrol
        patrol.PatrolMainZones();
        currentState = State.Wandering;
    }
    
    /*private void ReturnToPatrol()
    {
        EnterWandering();
    }*/
}