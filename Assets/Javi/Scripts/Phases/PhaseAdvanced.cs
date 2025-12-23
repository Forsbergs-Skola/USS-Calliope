using UnityEngine;

public class PhaseAdvanced : MonoBehaviour, IPhaseBehavior
{
    public void OnEnterPhase()
    {
        // Smart hunter behavior
        //GetComponent<EnemyAlertSystem>().RaiseGlobalAlert();
    }

    public void Tick()
    {
        // Flanking, ambush, coordinated attacks
    }

    public void OnExitPhase() { }
}