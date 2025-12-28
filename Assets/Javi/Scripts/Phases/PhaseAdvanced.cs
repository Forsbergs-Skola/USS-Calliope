using UnityEngine;

public class PhaseAdvanced : MonoBehaviour, IPhaseBehavior
{
    public void OnEnterPhase(SO_InfectionPhaseData data)
    {
        var attackController = GetComponent<EnemyAttackController>();
        attackController.enabled = true;

        attackController.SetAttacks(data.availableAttacks);
        Debug.Log("[PhaseAdvanced] Advanced attacks set");
    }

    public void Tick()
    {
        // Flanking, ambush, coordinated attacks
    }

    public void OnExitPhase() { }
}