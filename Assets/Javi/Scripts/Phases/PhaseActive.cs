using UnityEngine;

public class PhaseActive : MonoBehaviour, IPhaseBehavior
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
        // Basic chase / patrol
    }

    public void OnExitPhase() { }
}