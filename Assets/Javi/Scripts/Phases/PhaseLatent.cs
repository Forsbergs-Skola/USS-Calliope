using UnityEngine;

public class PhaseLatent : MonoBehaviour, IPhaseBehavior
{
    public void OnEnterPhase(SO_InfectionPhaseData data)
    {
        var attackController = GetComponent<EnemyAttackController>();
        attackController.enabled = true;

        attackController.SetAttacks(data.availableAttacks);
        Debug.Log("[PhaseActive] Attacks set from phase data");
    }

    public void Tick()
    {
        // TODO: flee logic / panic wandering
    }

    public void OnExitPhase() { }
}