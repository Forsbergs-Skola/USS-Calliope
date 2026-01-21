using UnityEngine;

[RequireComponent(typeof(EnemyAIStateController))]
public class EnemyCallReceiver : MonoBehaviour, IEnemyCallable
{
    private EnemyAIStateController ai;
    private EnemyPhaseStateMachine phaseMachine;

    private void Awake()
    {
        ai = GetComponent<EnemyAIStateController>();
        phaseMachine = GetComponent<EnemyPhaseStateMachine>();

        if (phaseMachine == null)
            Debug.LogError($"{name} needs EnemyPhaseStateMachine");
    }

    public EnemyRank Rank =>
        phaseMachine != null ? phaseMachine.CurrentRank : EnemyRank.Latent;

    public void ReceiveCall(PatrolZone zone, Vector3 callPosition)
    {
        Debug.Log("Receiving call");
        ai?.ForceWatchfulFromCall(zone, callPosition);
    }
}