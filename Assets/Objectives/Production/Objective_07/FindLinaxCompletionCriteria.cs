using UnityEngine;

[CreateAssetMenu(fileName = "FindLinaxCompletionCriteria", menuName = "Objectives/Criteria/FindLinaxCompletionCriteria")]
public class FindLinaxCompletionCriteria : CompletionCriteriaSO
{
    public override bool IsCriteriaMet()
    {
        if (DataController.Instance == null) return false;
        ProgressionData progData = DataController.Instance.ProgressionRuntimeData.Value;

        return progData.LinaxFound;
    }
}
