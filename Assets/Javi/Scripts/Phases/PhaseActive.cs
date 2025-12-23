using UnityEngine;

public class PhaseActive : MonoBehaviour, IPhaseBehavior
{
    public void OnEnterPhase()
    {
        GetComponent<EnemyAttackController>().enabled = true;
    }

    public void Tick()
    {
        // Basic chase / patrol
    }

    public void OnExitPhase() { }
}