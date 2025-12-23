using UnityEngine;

public class PhaseLatent : MonoBehaviour, IPhaseBehavior
{
    public void OnEnterPhase()
    {
        // Human still conscious: avoids player
        GetComponent<EnemyAttackController>().enabled = false;
    }

    public void Tick()
    {
        // TODO: flee logic / panic wandering
    }

    public void OnExitPhase() { }
}