using UnityEngine;
using System.Collections.Generic;

public class EnemyPhaseStateMachine : MonoBehaviour
{
    [Header("Phase Data")]
    [SerializeField] private List<SO_InfectionPhaseData> phases;

    private IPhaseBehavior currentPhase;
    private InfectionController infection;

    private void Awake()
    {
        infection = GetComponent<InfectionController>();
        if (infection == null)
        {
            Debug.LogError("EnemyPhaseStateMachine requires InfectionController");
            enabled = false;
            return;
        }

        infection.OnInfectionChanged.AddListener(EvaluatePhase);
    }

    private void EvaluatePhase(float infectionValue)
    {
        foreach (var phase in phases)
        {
            if (infectionValue >= phase.minInfection &&
                infectionValue <= phase.maxInfection)
            {
                SwitchPhase(phase);
                return;
            }
        }
    }

    private void SwitchPhase(SO_InfectionPhaseData data)
    {
        currentPhase?.OnExitPhase();

        currentPhase = GetComponent(data.phaseName) as IPhaseBehavior;

        if (currentPhase == null)
        {
            Debug.LogError($"Phase behavior {data.phaseName} not found on enemy.");
            return;
        }

        currentPhase.OnEnterPhase();
    }

    private void Update()
    {
        currentPhase?.Tick();
    }
}