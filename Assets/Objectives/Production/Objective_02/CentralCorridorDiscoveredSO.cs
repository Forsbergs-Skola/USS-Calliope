using UnityEngine;

[CreateAssetMenu(fileName = "CentralCorridorDiscoveredSO", menuName = "Objectives/Criteria/CentralCorridorDiscovered")]
public class CentralCorridorDiscoveredSO : CompletionCriteriaSO
{
    public override bool IsCriteriaMet()
    {
        if (DataController.Instance == null) return false;
        ProgressionData progData = DataController.Instance.ProgressionRuntimeData.Value;
        return progData.CentralCorridorDiscovered;
    }
}