using UnityEngine;

[CreateAssetMenu(fileName = "EnterCargoBayCriteria", menuName = "Objectives/Criteria/EnterCargoBayCriteria")]
public class EnterCargoBayCriteria : CompletionCriteriaSO
{
    public override bool IsCriteriaMet()
    {
        if (DataController.Instance == null) return false;
        ProgressionData progData = DataController.Instance.ProgressionRuntimeData.Value;
        return progData.CargoBayEntered;
    }
}
