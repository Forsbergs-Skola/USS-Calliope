using UnityEngine;

public class PhaseFinalBoss : MonoBehaviour, IPhaseBehavior
{
    public void OnEnterPhase(SO_InfectionPhaseData data)
    {
        var attackController = GetComponent<EnemyAttackController>();
        attackController.enabled = true;

        attackController.SetAttacks(data.availableAttacks);
        Debug.Log("[PhaseFinalBoss] FinalBoss attacks set");
    }

    public void Tick()
    {
        // 
    }

    public void OnExitPhase() { }
}
