using System.Collections;
using UnityEngine;

public class EnemyAIStateController : MonoBehaviour
{
    private EnemyPatrolController patrol;
    private EnemyFollowPlayer chase;
    //private EnemyPerceptionSystem perception;
    private IPerceptionSystem perception;

    private Transform player;

    public enum State { Wandering, Watchful, Attacking, Dead }
    private State currentState = State.Wandering;
    public State CurrentState => currentState;
    
    [SerializeField] private float watchfulDuration = 60f;
    [SerializeField] private float watchfulTimer = 8f;
    private PatrolZone lastSeenZone;
    
    private Olle.Scripts.CrouchInvisibility playerInvisibility;
    private Coroutine watchfulRoutine;
    private Coroutine watchfulFromCallRoutine;
    
    private EnemyCallSystem callSystem;
    private EnemyCallReceiver callReceiver;
    [SerializeField] private float callInterval = 20f;
    private Coroutine callLoopRoutine;
    private Vector3 lastCallPosition;
    
    private bool isStunned;
    private Vector3 stunPosition;
    private PatrolZone stunZone;
    public bool IsStunned => isStunned;
    
    private bool isDead;
    public bool IsDead => isDead;
    private EnemyAttackController attackController;

    private void Awake()
    {
        patrol = GetComponent<EnemyPatrolController>();
        chase = GetComponent<EnemyFollowPlayer>();
        perception = GetComponent<IPerceptionSystem>();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        playerInvisibility = player != null 
            ? player.GetComponent<Olle.Scripts.CrouchInvisibility>() 
            : null;
        
        callSystem = GetComponent<EnemyCallSystem>();
        callReceiver = GetComponent<EnemyCallReceiver>();
        attackController = GetComponent<EnemyAttackController>();
    }

    private void Start()
    {
        EnterWandering();
    }

    private void Update()
    {
        if (currentState == State.Dead) return;
        if (player == null) return;
        if (isStunned) return; 
        if (perception == null) return;
        
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
            case State.Dead:
                //ImDead();
                break;
        }
    }

    private void EnterWandering()
    {
        StopCallLoop();
        
        if (watchfulRoutine != null)
            StopCoroutine(watchfulRoutine);
        if (watchfulFromCallRoutine != null)
            StopCoroutine(watchfulFromCallRoutine);
        
        chase.SetFollow(false);
        patrol.StartPatrol();
        currentState = State.Wandering;
        callSystem?.ResetCallState();
        Debug.Log($"{name} entering Wandering state");
    }

    private void EnterAttacking()
    {
        StopCallLoop();
        
        if (watchfulRoutine != null)
        {
            StopCoroutine(watchfulRoutine);
            watchfulRoutine = null;
        }
        if (watchfulFromCallRoutine != null)
        {
            StopCoroutine(watchfulFromCallRoutine);
            watchfulFromCallRoutine = null;
        }
        
        //CancelInvoke(nameof(ReturnToPatrol));
        patrol.StopPatrol();
        chase.SetTarget(player);
        //chase.SetFollow(true);
        currentState = State.Attacking;
        Debug.Log($"{name} entering Attacking state");
        
        StartCallLoop();
    }

    private void EnterWatchful()
    {
        StopCallLoop();
        
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
        if (watchfulFromCallRoutine != null)
            StopCoroutine(watchfulFromCallRoutine);

        watchfulRoutine = StartCoroutine(WatchfulRoutine());
        
        // changing to investigating
        var movement = GetComponent<SimpleMovementAgent>();
        if (movement != null)
            movement.SetMovementState(SimpleMovementAgent.MovementState.Investigating);
        
        StartCallLoop();
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
        EnterWandering();
    }
    
    private IEnumerator WatchfulFromCallRoutine()
    {
        // Small pause to "process" the scream
        //yield return new WaitForSeconds(1.5f);
        //yield return new WaitForSeconds(watchfulDuration);
        
        if (lastSeenZone != null)
        {
            transform.LookAt(lastCallPosition);
            yield return new WaitForSeconds(0.5f);
            
            // patrol.PatrolSingleZone(lastSeenZone);
            if (patrol.CanPatrolZone(lastSeenZone))
            {
                patrol.PatrolSingleZone(lastSeenZone);
                yield return new WaitForSeconds(watchfulDuration);
            }
            else
            {
                // Stay in site but watching
                patrol.WatchInPlace(watchfulDuration);
                yield return new WaitForSeconds(watchfulDuration);
            }
        }

        patrol.PatrolMainZones();
        currentState = State.Wandering;
        EnterWandering();
    }
    
    public void ForceWatchfulFromCall(PatrolZone zone, Vector3 callPosition)
    {
        if (zone == null) return;
        StopCallLoop();
        
        lastSeenZone = zone;
        lastCallPosition = callPosition;

        patrol.StopPatrol();
        chase.SetFollow(false);
        chase.SetTarget(null);

        currentState = State.Watchful;

        if (watchfulRoutine != null)
            StopCoroutine(watchfulRoutine);
        if (watchfulFromCallRoutine != null)
            StopCoroutine(watchfulFromCallRoutine);

        watchfulFromCallRoutine = StartCoroutine(WatchfulFromCallRoutine());
        
        var movement = GetComponent<SimpleMovementAgent>();
        if (movement != null)
            movement.SetMovementState(SimpleMovementAgent.MovementState.Investigating);

        StartCallLoop();
    }
    
    private void StartCallLoop()
    {
        if (callSystem == null) return;

        StopCallLoop();
        callLoopRoutine = StartCoroutine(CallLoopRoutine());
    }

    private void StopCallLoop()
    {
        if (callLoopRoutine != null)
        {
            StopCoroutine(callLoopRoutine);
            callLoopRoutine = null;
        }
    }

    private IEnumerator CallLoopRoutine()
    {
        // Try to call just when entering in Attacking or Watchful modes
        callSystem.TryCall(transform.position);

        while (true)
        {
            yield return new WaitForSeconds(callInterval);

            // Try to call each "callInterval" seconds
            if (currentState != State.Attacking &&
                currentState != State.Watchful)
            {
                yield break;
            }

            callSystem.TryCall(transform.position);
        }
    }
    
    public void OnStunnedStart()
    {
        isStunned = true;
        stunPosition = transform.position;
        stunZone = patrol != null ? patrol.GetCurrentZone() : PatrolPointRegistry.GetClosestZone(transform.position);
        
        // stop patrol
        patrol.StopPatrol();
        chase.SetFollow(false);
        chase.SetTarget(null);

        // deactivate calls
        StopCallLoop();

        if (watchfulRoutine != null)
        {
            StopCoroutine(watchfulRoutine);
            watchfulRoutine = null;
        }

        if (watchfulFromCallRoutine != null)
        {
            StopCoroutine(watchfulFromCallRoutine);
            watchfulFromCallRoutine = null;
        }
    }

    public void OnStunnedEnd()
    {
        isStunned = false;
        PatrolZone zoneToWatch = stunZone != null ? stunZone : PatrolPointRegistry.GetClosestZone(stunPosition);
        
        if (zoneToWatch != null)
        {
            ForceWatchfulFromCall(zoneToWatch, stunPosition);
        }
        else
        {
            EnterWandering(); // Patrolling normally if no zone is found
        }
    }
    
    public void OnStunnedEndAfterLeapAttack()
    {
        isStunned = false;

        StopCallLoop();

        // cut logic from before
        if (watchfulRoutine != null)
        {
            StopCoroutine(watchfulRoutine);
            watchfulRoutine = null;
        }

        if (watchfulFromCallRoutine != null)
        {
            StopCoroutine(watchfulFromCallRoutine);
            watchfulFromCallRoutine = null;
        }

        chase.SetFollow(false);
        chase.SetTarget(null);

        currentState = State.Watchful;

        // stay in place
        patrol.WatchInPlace(6f);

        // Idle
        /*var movement = GetComponent<SimpleMovementAgent>();
        if (movement != null)
            movement.SetMovementState(SimpleMovementAgent.MovementState.Idle);*/

        Debug.Log($"{name} finished Leap stun → Watchful (in place)");
    }
    
    public void ForceBossState()
    {
        StopAllCoroutines();
        StopCallLoop();

        currentState = State.Watchful;

        chase.SetFollow(false);
        chase.SetTarget(null);

        var movement = GetComponent<SimpleMovementAgent>();
        if (movement != null)
            movement.SetMovementState(SimpleMovementAgent.MovementState.Investigating);

        Debug.Log($"{name} forced into Boss Watchful state");
    }
    
    public void OnLostPlayer()
    {
        currentState = State.Watchful;
        //var patrol = GetComponent<EnemyPatrolController>();
        if (patrol != null)
            patrol.WatchInPlace(4f);
    }
    
    /*private void ReturnToPatrol()
    {
        EnterWandering();
    }*/
    
    public void OnDeath()
    {
        if (isDead) return;
        
        currentState = State.Dead;
        
        attackController.deactivateAttacks();
        
        //Debug.Log($"jrv {name} onDeath");
        // stop all logic
        StopAllCoroutines();

        patrol.StopPatrol();
        patrol.enabled = false;
        chase?.SetFollow(false);
        chase?.SetTarget(null);
        callSystem.enabled = false;
        callReceiver.enabled = false;

        var cossBrain = GetComponent<FinalBossBrain>();
        if (cossBrain != null)
            cossBrain.enabled = false;
        
        var movement = GetComponent<SimpleMovementAgent>();
        if (movement != null)
            movement.enabled = false;
        
        var audio = GetComponent<EnemyAudioController>();
        if (audio != null)
            audio.StopAllAudio();
        
        isDead = true;
    }
}