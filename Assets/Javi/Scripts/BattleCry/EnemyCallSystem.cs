using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SimpleMovementAgent))]
public class EnemyCallSystem : MonoBehaviour
{
    [Header("Call Probability")]
    [Range(0f, 1f)]
    [SerializeField] private float firstCallChance = 0.6f;
    [Range(0f, 1f)]
    [SerializeField] private float repeatCallChance = 0.25f;
    private bool hasCalledOnce = false;

    [Header("Call Cooldown")]
    [SerializeField] private float callCooldown = 20f;

    [Header("Call Timing")]
    [SerializeField] private float preCallPause = 1f;
    [SerializeField] private float postCallPause = 2f;

    [Header("Call Radius")]
    [SerializeField] private float callRadius = 15f;

    [Header("Who Can Be Called")]
    [SerializeField] private EnemyRank[] callableRanks;

    private float lastCallTime = -999f;
    private bool isCalling;

    private SimpleMovementAgent movement;

    private void Awake()
    {
        movement = GetComponent<SimpleMovementAgent>();
    }

    public void TryCall(Vector3 position)
    {
        if (isCalling) return;
        if (Time.time < lastCallTime + callCooldown) return;

        float chance = hasCalledOnce ? repeatCallChance : firstCallChance;

        if (Random.value > chance)
            return;

        StartCoroutine(CallRoutine(position));
    }

    private IEnumerator CallRoutine(Vector3 position)
    {
        isCalling = true;
        lastCallTime = Time.time;

        movement.SetMovementState(SimpleMovementAgent.MovementState.Idle);
        yield return new WaitForSeconds(preCallPause);

        PerformCall(position);
        hasCalledOnce = true; 

        yield return new WaitForSeconds(postCallPause);
        movement.SetMovementState(SimpleMovementAgent.MovementState.Patrolling);

        isCalling = false;
    }

    private void PerformCall(Vector3 position)
    {
        // Change position to zone
        PatrolZone callZone = PatrolPointRegistry.GetClosestZone(position);

        if (callZone == null)
        {
            //Debug.LogWarning($"{name}: No PatrolZone found near call position {position}");
            return;
        }

        Collider[] hits = Physics.OverlapSphere(position, callRadius);
        
        var audio = GetComponent<EnemyAudioController>();
        if (audio != null)
        {
            audio.PlayBattleCry();
        }

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            IEnemyCallable callable = hit.GetComponent<IEnemyCallable>();
            if (callable == null) continue;

            bool allowed = false;
            foreach (var rank in callableRanks)
            {
                if (rank == callable.Rank)
                {
                    allowed = true;
                    break;
                }
            }

            if (!allowed) continue;
            
            callable.ReceiveCall(callZone, position);
        }

        //Debug.Log($"{name} screamed and called allies to zone {callZone.name}!");
    }
    
    public void ResetCallState()
    {
        hasCalledOnce = false;
    }
}
