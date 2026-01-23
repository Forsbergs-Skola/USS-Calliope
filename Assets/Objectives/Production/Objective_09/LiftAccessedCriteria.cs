using UnityEngine;

[CreateAssetMenu(fileName = "LiftAccessedCriteria", menuName = "Objectives/Criteria/LiftAccessedCriteria")]
public class LiftAccessedCriteria : CompletionCriteriaSO
{
    public override bool IsCriteriaMet()
    {
        if (DataController.Instance == null) return false;
        ProgressionData progData = DataController.Instance.ProgressionRuntimeData.Value;

        return progData.LiftAccessed;
    }
}
