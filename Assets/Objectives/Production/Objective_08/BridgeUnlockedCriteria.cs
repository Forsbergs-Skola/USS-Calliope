using UnityEngine;

[CreateAssetMenu(fileName = "BridgeUnlockedCriteria", menuName = "Objectives/Criteria/BridgeUnlockedCriteria")]
public class BridgeUnlockedCriteria : CompletionCriteriaSO
{
    public override bool IsCriteriaMet()
    {
        if (DataController.Instance == null) return false;
        ProgressionData progData = DataController.Instance.ProgressionRuntimeData.Value;

        return progData.BridgeUnlocked;
    }
}
